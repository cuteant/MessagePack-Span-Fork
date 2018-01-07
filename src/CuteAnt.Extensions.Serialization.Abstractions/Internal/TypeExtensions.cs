/* 采用 CuteAnt.Reflection.ReflectionShim替换 */

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
#if NETFX_CORE
using System.Reflection;
#endif
using System.Runtime.CompilerServices;

namespace CuteAnt.Extensions.Serialization
{
  internal static class TypeExtensions
  {
    private const MethodImplOptions s_aggressiveInlining = (MethodImplOptions)256;

#if NETFX_CORE
    private static bool EqualTo(this Type[] t1, Type[] t2)
    {
      if (t1.Length != t2.Length)
      {
        return false;
      }

      for (int idx = 0; idx < t1.Length; ++idx)
      {
        if (t1[idx] != t2[idx])
        {
          return false;
        }
      }

      return true;
    }

    public static ConstructorInfo GetConstructor(this Type type, Type[] types)
    {
      return type.GetTypeInfo().DeclaredConstructors
                               .Where(c => c.IsPublic)
                               .SingleOrDefault(c => c.GetParameters()
                                                      .Select(p => p.ParameterType).ToArray().EqualTo(types));
    }
#endif

    [MethodImpl(s_aggressiveInlining)]
    public static Type ExtractGenericInterface(this Type queryType, Type interfaceType)
    {
      Func<Type, bool> matchesInterface = t => t.IsGenericType() && t.GetGenericTypeDefinition() == interfaceType;
      return (matchesInterface(queryType)) ? queryType : queryType.GetInterfaces().FirstOrDefault(matchesInterface);
    }

#if NETFX_CORE
    [MethodImpl(s_aggressiveInlining)]
    public static Type[] GetGenericArguments(this Type type)
    {
      return type.GetTypeInfo().GenericTypeArguments;
    }

    [MethodImpl(s_aggressiveInlining)]
    public static Type[] GetInterfaces(this Type type)
    {
      return type.GetTypeInfo().ImplementedInterfaces.ToArray();
    }
#endif

#if NETFX_CORE
    [MethodImpl(s_aggressiveInlining)]
    public static bool IsAssignableFrom(this Type type, Type c)
    {
      return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
    }
#endif

    [MethodImpl(s_aggressiveInlining)]
    public static bool IsGenericType(this Type type)
    {
#if NETFX_CORE
      return type.GetTypeInfo().IsGenericType;
#else
      return type.IsGenericType;
#endif
    }

    [MethodImpl(s_aggressiveInlining)]
    public static bool IsInterface(this Type type)
    {
#if NETFX_CORE
      return type.GetTypeInfo().IsInterface;
#else
      return type.IsInterface;
#endif
    }

    [MethodImpl(s_aggressiveInlining)]
    public static bool IsValueType(this Type type)
    {
#if NETFX_CORE
      return type.GetTypeInfo().IsValueType;
#else
      return type.IsValueType;
#endif
    }
  }
}
