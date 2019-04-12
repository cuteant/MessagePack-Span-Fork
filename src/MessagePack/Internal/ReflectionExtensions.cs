
namespace MessagePack.Internal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
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
    }
}
