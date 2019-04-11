
namespace MessagePack.Internal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#else
    using System.Linq;

    /// <summary>GetMemberFunc</summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate object MemberGetter(object instance);
    /// <summary>GetMemberFunc</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate object MemberGetter<T>(T instance);
    /// <summary>SetMemberAction</summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void MemberSetter(object instance, object value);
    /// <summary>SetMemberAction</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void MemberSetter<T>(T instance, object value);
#endif

    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this System.Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }

        public static bool IsAnonymous(this System.Type type)
        {
            var typeName = type.Name;
            return type
#if DEPENDENT_ON_CUTEANT
                .GetCustomAttributeX
#else
                .GetCustomAttribute
#endif
                <CompilerGeneratedAttribute>() != null
                && type.IsGenericType && typeName.Contains("AnonymousType")
                && (typeName.StartsWith("<>", StringComparison.Ordinal) || typeName.StartsWith("VB$", StringComparison.Ordinal))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static bool IsIndexer(this System.Reflection.PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

        static readonly ThreadsafeTypeKeyHashTable<byte[]> s_encodedTypeNameCache = new ThreadsafeTypeKeyHashTable<byte[]>(capacity: 64);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetEncodedTypeName(this Type type)
        {
            if (!s_encodedTypeNameCache.TryGetValue(type, out byte[] typeName))
            {
                typeName = GetEncodedTypeNameSlow(type);
            }
            return typeName;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte[] GetEncodedTypeNameSlow(Type type)
        {
            var typeName = RuntimeTypeNameFormatter.Format(type);
            var encodedTypeName = MessagePackBinary.GetEncodedStringBytes(typeName);
            s_encodedTypeNameCache.TryAdd(type, encodedTypeName);
            return encodedTypeName;
        }

#if !DEPENDENT_ON_CUTEANT
        public static MemberGetter GetValueGetter(this FieldInfo fieldInfo) => fieldInfo.GetValue;
        public static MemberSetter GetValueSetter(this FieldInfo fieldInfo) => fieldInfo.SetValue;

        public static bool HasAttributeNamed(this FieldInfo fieldInfo, string name, bool inherit = true)
        {
            if (null == fieldInfo) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.fieldInfo); }
            if (string.IsNullOrWhiteSpace(name)) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.name); }

            var normalizedAttr = name.Replace("Attribute", "");
            return fieldInfo.GetCustomAttributes(inherit).Any(_ =>
                   string.Equals(_.GetType().Name.Replace("Attribute", ""), normalizedAttr, StringComparison.OrdinalIgnoreCase));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetCachedGenericType(this Type type, params Type[] argTypes)
        {
            if (argTypes == null) { argTypes = Type.EmptyTypes; }

            return type.MakeGenericType(argTypes);
        }
#endif
    }
}
