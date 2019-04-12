
namespace MessagePack.Internal
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>GetMemberFunc</summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public delegate object MemberGetter(object instance);
    /// <summary>SetMemberAction</summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public delegate void MemberSetter(object instance, object value);

    internal static class FieldInfoExtensions
    {

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
    }
}
