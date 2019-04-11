using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MessagePack.Formatters;
using MessagePack.Internal;
#if DEPENDENT_ON_CUTEANT
using CuteAnt.Reflection;
#endif

namespace MessagePack.Resolvers
{
    public sealed class DynamicGenericResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new DynamicGenericResolver();

        DynamicGenericResolver()
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
                formatter = (IMessagePackFormatter<T>)DynamicGenericResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }
}

namespace MessagePack.Internal
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
              {typeof(ObservableCollection<>), typeof(ObservableCollectionFormatter<>)},
              {typeof(ReadOnlyObservableCollection<>),(typeof(ReadOnlyObservableCollectionFormatter<>))},
              {typeof(IReadOnlyList<>), typeof(InterfaceReadOnlyListFormatter<>)},
              {typeof(IReadOnlyCollection<>), typeof(InterfaceReadOnlyCollectionFormatter<>)},
              {typeof(ISet<>), typeof(InterfaceSetFormatter<>)},
              {typeof(System.Collections.Concurrent.ConcurrentBag<>), typeof(ConcurrentBagFormatter<>)},
              {typeof(System.Collections.Concurrent.ConcurrentQueue<>), typeof(ConcurrentQueueFormatter<>)},
              {typeof(System.Collections.Concurrent.ConcurrentStack<>), typeof(ConcurrentStackFormatter<>)},
              {typeof(ReadOnlyDictionary<,>), typeof(ReadOnlyDictionaryFormatter<,>)},
              {typeof(IReadOnlyDictionary<,>), typeof(InterfaceReadOnlyDictionaryFormatter<,>)},
              {typeof(System.Collections.Concurrent.ConcurrentDictionary<,>), typeof(ConcurrentDictionaryFormatter<,>)},
              {typeof(Lazy<>), typeof(LazyFormatter<>)},
              {typeof(Task<>), typeof(TaskValueFormatter<>)},
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
                        return ByteArrayFormatter.Instance;
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
                var genericTypeArguments = t.GenericTypeArguments;
                var nullableElementType = isNullable ? genericTypeArguments[0] : null;

                const string _systemTupleType = "System.Tuple";
                const string _systemValueTupleType = "System.ValueTuple";

                if (genericType == typeof(KeyValuePair<,>))
                {
                    return CreateInstance(typeof(KeyValuePairFormatter<,>), genericTypeArguments);
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return CreateInstance(typeof(NullableFormatter<>), new[] { nullableElementType });
                }

                // ValueTask
                else if (genericType == typeof(ValueTask<>))
                {
                    return CreateInstance(typeof(ValueTaskFormatter<>), genericTypeArguments);
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    return CreateInstance(typeof(NullableFormatter<>), new[] { nullableElementType });
                }

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

                // ArraySegement
                else if (genericType == typeof(ArraySegment<>))
                {
                    if (genericTypeArguments[0] == typeof(byte))
                    {
                        return ByteArraySegmentFormatter.Instance;
                    }
                    else
                    {
                        return CreateInstance(typeof(ArraySegmentFormatter<>), genericTypeArguments);
                    }
                }
                else if (isNullable && nullableElementType.IsConstructedGenericType && nullableElementType.GetGenericTypeDefinition() == typeof(ArraySegment<>))
                {
                    if (nullableElementType == typeof(ArraySegment<byte>))
                    {
                        return new StaticNullableFormatter<ArraySegment<byte>>(ByteArraySegmentFormatter.Instance);
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
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                          && ti.DeclaredConstructors.Any(x => 0u >= (uint)x.GetParameters().Length))
                    {
                        var elemType = genericTypeArguments[0];
                        return CreateInstance(typeof(GenericCollectionFormatter<,>), new[] { elemType, t });
                    }
                    // generic dictionary
                    else if (genericTypeArguments.Length == 2
                          && ti.ImplementedInterfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                          && ti.DeclaredConstructors.Any(x => 0u >= (uint)x.GetParameters().Length))
                    {
                        var keyType = genericTypeArguments[0];
                        var valueType = genericTypeArguments[1];
                        return CreateInstance(typeof(GenericDictionaryFormatter<,,>), new[] { keyType, valueType, t });
                    }

                    if (typeof(Delegate).IsAssignableFrom(t))
                    {
                        return ActivatorUtils.FastCreateInstance(typeof(DelegateFormatter<>).GetCachedGenericType(t));
                    }
                    if (typeof(Exception).IsAssignableFrom(t))
                    {
                        return ActivatorUtils.FastCreateInstance(typeof(SimpleExceptionFormatter<>).GetCachedGenericType(t));
                    }
                    if (typeof(Expression).IsAssignableFrom(t))
                    {
                        return ActivatorUtils.FastCreateInstance(typeof(SimpleExpressionFormatter<>).GetCachedGenericType(t));
                    }
                    if (typeof(SymbolDocumentInfo).IsAssignableFrom(t))
                    {
                        return ActivatorUtils.FastCreateInstance(typeof(SymbolDocumentInfoFormatter<>).GetCachedGenericType(t));
                    }
                    if (typeof(MemberBinding).IsAssignableFrom(t))
                    {
                        return ActivatorUtils.FastCreateInstance(typeof(MemberBindingFormatter<>).GetCachedGenericType(t));
                    }
                }
            }
            else
            {
                // NonGeneric Collection
                if (t == typeof(IList))
                {
                    return NonGenericInterfaceListFormatter.Instance;
                }
                else if (t == typeof(IDictionary))
                {
                    return NonGenericInterfaceDictionaryFormatter.Instance;
                }
                if (typeof(IList).IsAssignableFrom(t) && ti.DeclaredConstructors.Any(x => 0u >= (uint)x.GetParameters().Length))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(NonGenericListFormatter<>).GetCachedGenericType(t));
                }
                else if (typeof(IDictionary).IsAssignableFrom(t) && ti.DeclaredConstructors.Any(x => 0u >= (uint)x.GetParameters().Length))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(NonGenericDictionaryFormatter<>).GetCachedGenericType(t));
                }

                if (typeof(Type).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(SimpleTypeFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(ConstructorInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(ConstructorInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(EventInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(EventInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(FieldInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(FieldInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(PropertyInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(PropertyInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(MethodInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(MethodInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(MemberInfo).IsAssignableFrom(t)) // 是否无用
                {
                    return ActivatorUtils.FastCreateInstance(typeof(MemberInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(Delegate).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(DelegateFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(Exception).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(SimpleExceptionFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(Expression).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(SimpleExpressionFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(SymbolDocumentInfo).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(SymbolDocumentInfoFormatter<>).GetCachedGenericType(t));
                }
                if (typeof(MemberBinding).IsAssignableFrom(t))
                {
                    return ActivatorUtils.FastCreateInstance(typeof(MemberBindingFormatter<>).GetCachedGenericType(t));
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
