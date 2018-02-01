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

        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return GetAllPropertiesCore(type, new HashSet<string>(StringComparer.Ordinal));
        }

        static IEnumerable<PropertyInfo> GetAllPropertiesCore(Type type, HashSet<string> nameCheck)
        {
            foreach (var item in type.GetRuntimeProperties())
            {
                if (nameCheck.Add(item.Name))
                {
                    yield return item;
                }
            }
            if (type.BaseType != null)
            {
                foreach (var item in GetAllPropertiesCore(type.BaseType, nameCheck))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            return GetAllFieldsCore(type, new HashSet<string>(StringComparer.Ordinal));
        }

        static IEnumerable<FieldInfo> GetAllFieldsCore(Type type, HashSet<string> nameCheck)
        {
            foreach (var item in type.GetRuntimeFields())
            {
                if (nameCheck.Add(item.Name))
                {
                    yield return item;
                }
            }
            if (type.BaseType != null)
            {
                foreach (var item in GetAllFieldsCore(type.BaseType, nameCheck))
                {
                    yield return item;
                }
            }
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