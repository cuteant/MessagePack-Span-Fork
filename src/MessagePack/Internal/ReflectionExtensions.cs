#if !UNITY_WSA

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagePack.Internal
{
    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this System.Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }

        public static bool IsAnonymous(this System.Type type)
        {
            var typeName = type.Name;
            return type.GetCustomAttributeX<CompilerGeneratedAttribute>() != null
                && type.IsGenericType && typeName.Contains("AnonymousType")
                && (typeName.StartsWith("<>", StringComparison.Ordinal) || typeName.StartsWith("VB$", StringComparison.Ordinal))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static bool IsIndexer(this System.Reflection.PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

#if NETSTANDARD || NETFRAMEWORK

        public static MethodInfo GetGetMethod(this PropertyInfo propInfo)
        {
#if NET40
            return propInfo.GetGetMethod(true);
#else
            return propInfo.GetMethod;
#endif
        }

        public static MethodInfo GetSetMethod(this PropertyInfo propInfo)
        {
#if NET40
            return propInfo.GetSetMethod(true);
#else
            return propInfo.SetMethod;
#endif
        }

#endif
    }
}

#endif