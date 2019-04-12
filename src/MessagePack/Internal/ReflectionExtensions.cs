
namespace MessagePack.Internal
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

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
    }
}
