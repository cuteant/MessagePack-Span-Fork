using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack.ImmutableCollection
{
    public sealed class ImmutableCollectionResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ImmutableCollectionResolver();

        ImmutableCollectionResolver()
        {

        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (IMessagePackFormatter<T>)ImmutableCollectionGetFormatterHelper.GetFormatter(typeof(T));
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
            var ti = t.GetTypeInfo();

            if (ti.IsGenericType)
            {
                var genericType = ti.GetGenericTypeDefinition();
                var genericTypeInfo = genericType.GetTypeInfo();
                var isNullable = genericTypeInfo.IsNullable();
                var nullableElementType = isNullable ? ti.GenericTypeArguments[0] : null;

                Type formatterType;
                if (formatterMap.TryGetValue(genericType, out formatterType))
                {
                    return CreateInstance(formatterType, ti.GenericTypeArguments);
                }
#if NET40
                else if (isNullable && nullableElementType.IsConstructedGenericType() && nullableElementType.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
#else
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
            return ActivatorUtils.CreateInstance(genericType.GetCachedGenericType(genericTypeArguments));
        }
    }

    internal static class ReflectionExtensions
    {
        public static bool IsNullable(this System.Reflection.TypeInfo type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>);
        }
    }
}