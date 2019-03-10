using System.Reflection;
using System.Collections;
using System.Linq;
using Utf8Json.Internal;
using System;
using CuteAnt.Reflection;
using Utf8Json.Formatters;
using System.Collections.Generic;
using Utf8Json.Resolvers.Internal;
using System.Collections.ObjectModel;

#if NETSTANDARD || DESKTOPCLR
using System.Threading.Tasks;
#endif

namespace Utf8Json.Resolvers
{
    public sealed class DynamicGenericResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new DynamicGenericResolver();

        DynamicGenericResolver()
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
                formatter = (IJsonFormatter<T>)DynamicGenericResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }
}

namespace Utf8Json.Resolvers.Internal
{
    internal static class DynamicGenericResolverGetFormatterHelper
    {
        static readonly Dictionary<Type, Type> formatterMap = new Dictionary<Type, Type>()
        {
            {typeof(List<>), typeof(ListFormatter<>)},
            {typeof(LinkedList<>), typeof(LinkedListFormatter<>)},
            {typeof(Queue<>), typeof(QeueueFormatter<>)},
            {typeof(Stack<>), typeof(StackFormatter<>)},
            {typeof(HashSet<>), typeof(HashSetFormatter<>)},
            {typeof(ReadOnlyCollection<>), typeof(ReadOnlyCollectionFormatter<>)},
            {typeof(IList<>), typeof(InterfaceListFormatter<>)},
            {typeof(ICollection<>), typeof(InterfaceCollectionFormatter<>)},
            {typeof(IEnumerable<>), typeof(InterfaceEnumerableFormatter<>)},
            {typeof(Dictionary<,>), typeof(DictionaryFormatter<,>)},
            {typeof(IDictionary<,>), typeof(InterfaceDictionaryFormatter<,>)},
            {typeof(SortedDictionary<,>), typeof(SortedDictionaryFormatter<,>)},
            {typeof(SortedList<,>), typeof(SortedListFormatter<,>)},
            {typeof(ILookup<,>), typeof(InterfaceLookupFormatter<,>)},
            {typeof(IGrouping<,>), typeof(InterfaceGroupingFormatter<,>)},
            #if NETSTANDARD || DESKTOPCLR
            {typeof(ObservableCollection<>), typeof(ObservableCollectionFormatter<>)},
            {typeof(ReadOnlyObservableCollection<>),(typeof(ReadOnlyObservableCollectionFormatter<>))},
#if !NET40
            {typeof(IReadOnlyList<>), typeof(InterfaceReadOnlyListFormatter<>)},
            {typeof(IReadOnlyCollection<>), typeof(InterfaceReadOnlyCollectionFormatter<>)},
#endif
            {typeof(ISet<>), typeof(InterfaceSetFormatter<>)},
            {typeof(System.Collections.Concurrent.ConcurrentBag<>), typeof(ConcurrentBagFormatter<>)},
            {typeof(System.Collections.Concurrent.ConcurrentQueue<>), typeof(ConcurrentQueueFormatter<>)},
            {typeof(System.Collections.Concurrent.ConcurrentStack<>), typeof(ConcurrentStackFormatter<>)},
#if !NET40
            {typeof(ReadOnlyDictionary<,>), typeof(ReadOnlyDictionaryFormatter<,>)},
            {typeof(IReadOnlyDictionary<,>), typeof(InterfaceReadOnlyDictionaryFormatter<,>)},
#endif
            {typeof(System.Collections.Concurrent.ConcurrentDictionary<,>), typeof(ConcurrentDictionaryFormatter<,>)},
            {typeof(Lazy<>), typeof(LazyFormatter<>)},
#if !NET40
            {typeof(Task<>), typeof(TaskValueFormatter<>)},
#endif
            #endif
        };

        // Reduce IL2CPP code generate size(don't write long code in <T>)
        internal static object GetFormatter(Type t)
        {
            var ti = t.GetTypeInfo();

            if (t.IsArray)
            {
                var rank = t.GetArrayRank();
                if (rank == 1)
                {
                    if (t.GetElementType() == typeof(byte)) // byte[] is also supported in builtin formatter.
                    {
                        return ByteArrayFormatter.Default;
                    }

                    return ActivatorUtils.FastCreateInstance(typeof(ArrayFormatter<>).GetCachedGenericType(t.GetElementType()));
                }
                else if (rank == 2)
                {
                    return ActivatorUtils.FastCreateInstance(typeof(TwoDimentionalArrayFormatter<>).GetCachedGenericType(t.GetElementType()));
                }
                else if (rank == 3)
                {
                    return ActivatorUtils.FastCreateInstance(typeof(ThreeDimentionalArrayFormatter<>).GetCachedGenericType(t.GetElementType()));
                }
                else if (rank == 4)
                {
                    return ActivatorUtils.FastCreateInstance(typeof(FourDimentionalArrayFormatter<>).GetCachedGenericType(t.GetElementType()));
                }
                else
                {
                    return null; // not supported built-in
                }
            }
            else if (t.IsGenericType)
            {
                var genericType = t.GetGenericTypeDefinition();
                var isNullable = genericType.IsNullable();
#if NET40
                var genericTypeArguments = t.GenericTypeArguments();
#else
                var genericTypeArguments = t.GenericTypeArguments;
#endif
                var nullableElementType = isNullable ? genericTypeArguments[0] : null;

                const string _systemTupleType = "System.Tuple";
                const string _systemValueTupleType = "System.ValueTuple";

                if (genericType == typeof(KeyValuePair<,>))
                {
                    return CreateInstance(typeof(KeyValuePairFormatter<,>), genericTypeArguments);
                }
#if NET40
                else if (isNullable && nullableElementType.IsConstructedGenericType() && nullableElementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
#else
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
#endif
                {
                    return CreateInstance(typeof(NullableFormatter<>), new[] { nullableElementType });
                }

#if NETSTANDARD || DESKTOPCLR

#if !NET40
                // ValueTask
                else if (genericType == typeof(ValueTask<>))
                {
                    return CreateInstance(typeof(ValueTaskFormatter<>), genericTypeArguments);
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    return CreateInstance(typeof(NullableFormatter<>), new[] { nullableElementType });
                }
#endif

                // Tuple
                else if (t.FullName.StartsWith(_systemTupleType, StringComparison.Ordinal))
                {
                    Type tupleFormatterType = null;
                    switch (genericTypeArguments.Length)
                    {
                        case 1:
                            tupleFormatterType = typeof(TupleFormatter<>);
                            break;
                        case 2:
                            tupleFormatterType = typeof(TupleFormatter<,>);
                            break;
                        case 3:
                            tupleFormatterType = typeof(TupleFormatter<,,>);
                            break;
                        case 4:
                            tupleFormatterType = typeof(TupleFormatter<,,,>);
                            break;
                        case 5:
                            tupleFormatterType = typeof(TupleFormatter<,,,,>);
                            break;
                        case 6:
                            tupleFormatterType = typeof(TupleFormatter<,,,,,>);
                            break;
                        case 7:
                            tupleFormatterType = typeof(TupleFormatter<,,,,,,>);
                            break;
                        case 8:
                            tupleFormatterType = typeof(TupleFormatter<,,,,,,,>);
                            break;
                        default:
                            break;
                    }

                    return CreateInstance(tupleFormatterType, genericTypeArguments);
                }

                // ValueTuple
                else if (t.FullName.StartsWith(_systemValueTupleType, StringComparison.Ordinal))
                {
                    Type tupleFormatterType = null;
                    switch (genericTypeArguments.Length)
                    {
                        case 1:
                            tupleFormatterType = typeof(ValueTupleFormatter<>);
                            break;
                        case 2:
                            tupleFormatterType = typeof(ValueTupleFormatter<,>);
                            break;
                        case 3:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,>);
                            break;
                        case 4:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,,>);
                            break;
                        case 5:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,,,>);
                            break;
                        case 6:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,,,,>);
                            break;
                        case 7:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,,,,,>);
                            break;
                        case 8:
                            tupleFormatterType = typeof(ValueTupleFormatter<,,,,,,,>);
                            break;
                        default:
                            break;
                    }

                    return CreateInstance(tupleFormatterType, genericTypeArguments);
                }

#endif

                // ArraySegement
                else if (genericType == typeof(ArraySegment<>))
                {
                    if (genericTypeArguments[0] == typeof(byte))
                    {
                        return ByteArraySegmentFormatter.Default;
                    }
                    else
                    {
                        return CreateInstance(typeof(ArraySegmentFormatter<>), genericTypeArguments);
                    }
                }
#if NET40
                else if (isNullable && nullableElementType.IsConstructedGenericType() && nullableElementType.GetGenericTypeDefinition() == typeof(ArraySegment<>))
#else
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(ArraySegment<>))
#endif
                {
                    if (nullableElementType == typeof(ArraySegment<byte>))
                    {
                        return new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Default);
                    }
                    else
                    {
                        return CreateInstance(typeof(NullableFormatter<>), new[] { nullableElementType });
                    }
                }

                // Mapped formatter
                else
                {
                    Type formatterType;
                    if (formatterMap.TryGetValue(genericType, out formatterType))
                    {
                        return CreateInstance(formatterType, genericTypeArguments);
                    }

                    // generic collection
                    else if (genericTypeArguments.Length == 1
#if NET40
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType() && x.GetGenericTypeDefinition() == typeof(ICollection<>))
#else
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
#endif
                          && ti.DeclaredConstructors.Any(x => x.GetParameters().Length == 0))
                    {
                        var elemType = genericTypeArguments[0];
                        return CreateInstance(typeof(GenericCollectionFormatter<,>), new[] { elemType, t });
                    }
                    // generic dictionary
                    else if (genericTypeArguments.Length == 2
#if NET40
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType() && x.GetGenericTypeDefinition() == typeof(IDictionary<,>))
#else
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>))
#endif
                          && ti.DeclaredConstructors.Any(x => x.GetParameters().Length == 0))
                    {
                        var keyType = genericTypeArguments[0];
                        var valueType = genericTypeArguments[1];
                        return CreateInstance(typeof(GenericDictionaryFormatter<,,>), new[] { keyType, valueType, t });
                    }
                }
            }
            else
            {
                // NonGeneric Collection
                if (t == typeof(IEnumerable))
                {
                    return NonGenericInterfaceEnumerableFormatter.Default;
                }
                else if (t == typeof(ICollection))
                {
                    return NonGenericInterfaceCollectionFormatter.Default;
                }
                else if (t == typeof(IList))
                {
                    return NonGenericInterfaceListFormatter.Default;
                }
                else if (t == typeof(IDictionary))
                {
                    return NonGenericInterfaceDictionaryFormatter.Default;
                }
                if (typeof(IList).IsAssignableFrom(t) && ti.DeclaredConstructors.Any(x => x.GetParameters().Length == 0))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(NonGenericListFormatter<>).GetCachedGenericType(t));
                }
                else if (typeof(IDictionary).IsAssignableFrom(t) && ti.DeclaredConstructors.Any(x => x.GetParameters().Length == 0))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(NonGenericDictionaryFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(Type).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(SimpleTypeFormatter<>).GetCachedGenericType(t));
                }
            }

            return null;
        }

        static object CreateInstance(Type genericType, Type[] genericTypeArguments, params object[] arguments)
        {
            return ActivatorUtil.CreateInstance(genericType.GetCachedGenericType(genericTypeArguments), arguments);
        }
    }
}

