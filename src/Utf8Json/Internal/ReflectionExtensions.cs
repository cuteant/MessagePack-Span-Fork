using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utf8Json.Internal
{
    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this System.Reflection.TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }

        public static bool IsPublic(this System.Reflection.TypeInfo type)
        {
            return type.IsPublic;
        }

        public static bool IsAnonymous(this System.Reflection.TypeInfo type)
        {
            return type.AsType().GetCustomAttributeX<CompilerGeneratedAttribute>() != null
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

#if NETSTANDARD || DESKTOPCLR

        public static bool IsConstructedGenericType(this System.Reflection.TypeInfo type)
        {
#if NET40
            return type.AsType().IsConstructedGenericType();
#else
            return type.AsType().IsConstructedGenericType;
#endif
        }

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