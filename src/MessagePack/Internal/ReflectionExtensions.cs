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
#if NET40
            return type.AsType().GetCustomAttributeX<CompilerGeneratedAttribute>() != null
#else
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null
#endif
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static bool IsIndexer(this System.Reflection.PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
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

#endif