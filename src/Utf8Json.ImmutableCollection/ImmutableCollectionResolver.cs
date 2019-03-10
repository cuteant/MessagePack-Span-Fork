using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using CuteAnt.Reflection;
using Utf8Json.Formatters;

namespace Utf8Json.ImmutableCollection
{
    public sealed class ImmutableCollectionResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new ImmutableCollectionResolver();

        ImmutableCollectionResolver()
        {

        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (IJsonFormatter<T>)ImmutableCollectionGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class ImmutableCollectionGetFormatterHelper
    {
        static readonly Dictionary<Type, Type> formatterMap = new Dictionary<Type, Type>()
        {
              {typeof(ImmutableArray<>), typeof(ImmutableArrayFormatter<>)},
              {typeof(ImmutableList<>), typeof(ImmutableListFormatter<>)},
              {typeof(ImmutableDictionary<,>), typeof(ImmutableDictionaryFormatter<,>)},
              {typeof(ImmutableHashSet<>), typeof(ImmutableHashSetFormatter<>)},
              {typeof(ImmutableSortedDictionary<,>), typeof(ImmutableSortedDictionaryFormatter<,>)},
              {typeof(ImmutableSortedSet<>), typeof(ImmutableSortedSetFormatter<>)},
              {typeof(ImmutableQueue<>), typeof(ImmutableQueueFormatter<>)},
              {typeof(ImmutableStack<>), typeof(ImmutableStackFormatter<>)},
              {typeof(IImmutableList<>), typeof(InterfaceImmutableListFormatter<>)},
              {typeof(IImmutableDictionary<,>), typeof(InterfaceImmutableDictionaryFormatter<,>)},
              {typeof(IImmutableQueue<>), typeof(InterfaceImmutableQueueFormatter<>)},
              {typeof(IImmutableSet<>), typeof(InterfaceImmutableSetFormatter<>)},
              {typeof(IImmutableStack<>), typeof(InterfaceImmutableStackFormatter<>)},
        };

        internal static object GetFormatter(Type t)
        {
            if (t.IsGenericType)
            {
                var genericType = t.GetGenericTypeDefinition();
                var isNullable = genericType.IsNullable();
#if NET40
                var nullableElementType = isNullable ? t.GenericTypeArguments()[0] : null;

                Type formatterType;
                if (formatterMap.TryGetValue(genericType, out formatterType))
                {
                    return CreateInstance(formatterType, t.GenericTypeArguments());
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType() && nullableElementType.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
#else
                var nullableElementType = isNullable ? t.GenericTypeArguments[0] : null;

                Type formatterType;
                if (formatterMap.TryGetValue(genericType, out formatterType))
                {
                    return CreateInstance(formatterType, t.GenericTypeArguments);
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
#endif
                {
                    return CreateInstance(typeof(NullableFormatter<>), nullableElementType);
                }
            }

            return null;
        }

        static object CreateInstance(Type genericType, params Type[] genericTypeArguments)
        {
            return ActivatorUtils.FastCreateInstance(genericType.GetCachedGenericType(genericTypeArguments));
        }
    }

    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this System.Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }
    }
}