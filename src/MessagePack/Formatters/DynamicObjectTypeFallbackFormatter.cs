namespace MessagePack.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#else
    using MessagePack.Internal;
#endif

    public sealed class DynamicObjectTypeFallbackFormatter : IMessagePackFormatter<object>
    {
        delegate void SerializeMethod(object dynamicFormatter, ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver);

        readonly MessagePack.Internal.ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>> serializers = new MessagePack.Internal.ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>>();

        readonly IFormatterResolver[] innerResolvers;

        public DynamicObjectTypeFallbackFormatter(params IFormatterResolver[] innerResolvers)
        {
            this.innerResolvers = innerResolvers;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var type = value.GetType();

            if (type == typeof(object))
            {
                // serialize to empty map
                writer.WriteFixedMapHeaderUnsafe(0, ref idx); return;
            }

            if (!serializers.TryGetValue(type, out var formatterAndDelegate))
            {
                GetFormatterSlow(type, out formatterAndDelegate);
            }

            formatterAndDelegate.Value(formatterAndDelegate.Key, ref writer, ref idx, value, formatterResolver);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GetFormatterSlow(Type type, out KeyValuePair<object, SerializeMethod> formatterAndDelegate)
        {
            lock (serializers)
            {
                if (!serializers.TryGetValue(type, out formatterAndDelegate))
                {
                    var aliasType = type;
                    if (typeof(Type).IsAssignableFrom(type))
                    {
                        aliasType = typeof(Type);
                    }
                    else if (typeof(ConstructorInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(ConstructorInfo);
                    }
                    else if (typeof(EventInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(EventInfo);
                    }
                    else if (typeof(FieldInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(FieldInfo);
                    }
                    else if (typeof(PropertyInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(PropertyInfo);
                    }
                    else if (typeof(MethodInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(MethodInfo);
                    }
                    else if (typeof(MemberInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(MemberInfo);
                    }
                    else if (typeof(Delegate).IsAssignableFrom(type))
                    {
                        aliasType = typeof(Delegate);
                    }
                    else if (typeof(Exception).IsAssignableFrom(type))
                    {
                        aliasType = typeof(Exception);
                    }
                    else if (typeof(Expression).IsAssignableFrom(type))
                    {
                        aliasType = typeof(Expression);
                    }
                    else if (typeof(SymbolDocumentInfo).IsAssignableFrom(type))
                    {
                        aliasType = typeof(SymbolDocumentInfo);
                    }
                    else if (typeof(MemberBinding).IsAssignableFrom(type))
                    {
                        aliasType = typeof(MemberBinding);
                    }
                    object formatter = null;
                    foreach (var innerResolver in innerResolvers)
                    {
                        formatter = innerResolver.GetFormatterDynamic(aliasType);
                        if (formatter != null) break;
                    }
                    if (formatter == null)
                    {
                        ThrowHelper.ThrowFormatterNotRegisteredException(type, innerResolvers);
                    }

                    var t = aliasType;
                    {
                        var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(t);
                        var param0 = Expression.Parameter(typeof(object), "formatter");
                        var param1 = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                        var param2 = Expression.Parameter(typeof(int).MakeByRefType(), "idx");
                        var param3 = Expression.Parameter(typeof(object), "value");
                        var param4 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(MessagePackWriter).MakeByRefType(), typeof(int).MakeByRefType(), t, typeof(IFormatterResolver) });

                        var body = Expression.Call(
                            Expression.Convert(param0, formatterType),
                            serializeMethodInfo,
                            param1,
                            param2,
                            aliasType.IsValueType ? Expression.Unbox(param3, t) : Expression.Convert(param3, t),
                            param4);

                        var lambda = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3, param4).Compile();

                        formatterAndDelegate = new KeyValuePair<object, SerializeMethod>(formatter, lambda);
                    }

                    serializers.TryAdd(type, formatterAndDelegate);
                }
            }
        }

        public object Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return PrimitiveObjectFormatter.Instance.Deserialize(ref reader, formatterResolver);
        }
    }
}
