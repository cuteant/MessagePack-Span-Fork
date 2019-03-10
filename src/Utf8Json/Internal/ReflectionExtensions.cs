using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Utf8Json.Internal
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
                && typeName.Contains("AnonymousType")
                && (typeName.StartsWith("<>", StringComparison.Ordinal) || typeName.StartsWith("VB$", StringComparison.Ordinal))
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