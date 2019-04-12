namespace MessagePack.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using MessagePack.Internal;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    /// <summary>For `object` field that holds derived from `object` value, ex: var arr = new object[] { 1, "a", new Model() };</summary>
    public class TypelessFormatter : IMessagePackFormatter<object>
    {
        public const sbyte ExtensionTypeCode = 100;

        static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        delegate void SerializeMethod(object dynamicContractlessFormatter, ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver);
        delegate object DeserializeMethod(object dynamicContractlessFormatter, ref MessagePackReader reader, IFormatterResolver formatterResolver);

        public static readonly IMessagePackFormatter<object> Instance = new TypelessFormatter();

        static readonly ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>> s_serializers = new ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>>();
        static readonly ThreadsafeTypeKeyHashTable<KeyValuePair<object, DeserializeMethod>> s_deserializers = new ThreadsafeTypeKeyHashTable<KeyValuePair<object, DeserializeMethod>>();
        static readonly ThreadsafeTypeKeyHashTable<Type> s_typeNameCache = new ThreadsafeTypeKeyHashTable<Type>();

        static readonly HashSet<string> s_blacklistCheck;
        static readonly HashSet<Type> s_useBuiltinTypes = new HashSet<Type>()
        {
            typeof(Boolean),
            // typeof(Char),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(string),
            typeof(byte[]),

            // array should save there types.
            //typeof(Boolean[]),
            //typeof(Char[]),
            //typeof(SByte[]),
            //typeof(Int16[]),
            //typeof(UInt16[]),
            //typeof(Int32[]),
            //typeof(UInt32[]),
            //typeof(Int64[]),
            //typeof(UInt64[]),
            //typeof(Single[]),
            //typeof(Double[]),
            //typeof(string[]),

            typeof(Boolean?),
            // typeof(Char?),
            typeof(SByte?),
            typeof(Byte?),
            typeof(Int16?),
            typeof(UInt16?),
            typeof(Int32?),
            typeof(UInt32?),
            typeof(Int64?),
            typeof(UInt64?),
            typeof(Single?),
            typeof(Double?),
        };

        static TypelessFormatter()
        {
            s_blacklistCheck = new HashSet<string>(StringComparer.Ordinal)
            {
                "System.CodeDom.Compiler.TempFileCollection",
                "System.IO.FileSystemInfo",
                "System.Management.IWbemClassObjectFreeThreaded"
            };

            s_serializers.TryAdd(typeof(object), _ => new KeyValuePair<object, SerializeMethod>(null, (object p1, ref MessagePackWriter p2, ref int p3, object p4, IFormatterResolver p5) => { }));
            s_deserializers.TryAdd(typeof(object), _ => new KeyValuePair<object, DeserializeMethod>(null, (object p1, ref MessagePackReader p2, IFormatterResolver p3) =>
            {
                return new object();
            }));
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var type = value.GetType();

            if (!s_typeNameCache.TryGetValue(type, out Type expectedType))
            {
                GetExpectedTypeSlow(type, out expectedType);
            }

            if (expectedType == null)
            {
                Resolvers.TypelessFormatterFallbackResolver.Instance.GetFormatter<object>().Serialize(ref writer, ref idx, value, formatterResolver);
                return;
            }

            // don't use GetOrAdd for avoid closure capture.
            if (!s_serializers.TryGetValue(expectedType, out var formatterAndDelegate))
            {
                GetFormatterAndDelegateSlow(expectedType, formatterResolver, out formatterAndDelegate);
            }

            // mark as extension with code 100
            writer.Ensure(idx, 6);
            var startOffset = idx;
            idx += 6; // mark will be written at the end, when size is known

            var typeName = MessagePackBinary.GetEncodedTypeName(expectedType);
            UnsafeMemory.WriteRaw(ref writer, typeName, ref idx);
            formatterAndDelegate.Value(formatterAndDelegate.Key, ref writer, ref idx, value, formatterResolver);

            var dataLength = idx - startOffset - 6;
            MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref writer.PinnableAddress, startOffset, ExtensionTypeCode, dataLength);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GetExpectedTypeSlow(Type type, out Type expectedType)
        {
            if (s_blacklistCheck.Contains(type.FullName))
            {
                ThrowHelper.ThrowInvalidOperationException_Blacklist(type);
            }

            if (type.IsAnonymous() || s_useBuiltinTypes.Contains(type))
            {
                expectedType = null;
            }
            else
            {
                expectedType = TranslateTypeName(type);
            }
            s_typeNameCache.TryAdd(type, expectedType);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GetFormatterAndDelegateSlow(Type expectedType, IFormatterResolver formatterResolver, out KeyValuePair<object, SerializeMethod> formatterAndDelegate)
        {
            lock (s_serializers) // double check locking...
            {
                if (!s_serializers.TryGetValue(expectedType, out formatterAndDelegate))
                {
                    var formatter = formatterResolver.GetFormatterDynamic(expectedType);
                    if (formatter == null)
                    {
                        ThrowHelper.ThrowFormatterNotRegisteredException(expectedType, formatterResolver);
                    }

                    var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(expectedType);
                    var param0 = Expression.Parameter(typeof(object), "formatter");
                    var param1 = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                    var param2 = Expression.Parameter(typeof(int).MakeByRefType(), "idx");
                    var param3 = Expression.Parameter(typeof(object), "value");
                    var param4 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                    var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(MessagePackWriter).MakeByRefType(), typeof(int).MakeByRefType(), expectedType, typeof(IFormatterResolver) });

                    var body = Expression.Call(
                        Expression.Convert(param0, formatterType),
                        serializeMethodInfo,
                        param1,
                        param2,
                        expectedType.IsValueType ? Expression.Unbox(param3, expectedType) : Expression.Convert(param3, expectedType),
                        param4);

                    var lambda = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3, param4).Compile();

                    formatterAndDelegate = new KeyValuePair<object, SerializeMethod>(formatter, lambda);
                    s_serializers.TryAdd(expectedType, formatterAndDelegate);
                }
            }
        }

        protected virtual Type TranslateTypeName(Type actualType) => actualType;

        public object Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var packType = reader.GetMessagePackType();
            if (packType == MessagePackType.Extension)
            {
                var ext = reader.ReadExtensionFormatHeader();
                if (ext.TypeCode == ExtensionTypeCode)
                {
                    // it has type name serialized
                    var typeName = reader.ReadStringSegment();
                    var result = DeserializeByTypeName(typeName, ref reader, formatterResolver);
                    return result;
                }
            }

            // fallback
            return Resolvers.TypelessFormatterFallbackResolver.Instance.GetFormatter<object>().Deserialize(ref reader, formatterResolver);
        }

        /// <summary>Does not support deserializing of anonymous types
        /// Type should be covered by preceeding resolvers in complex/standard resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object DeserializeByTypeName(ReadOnlySpan<byte> typeName, ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            // try get type with assembly name, throw if not found
            var type = MessagePackBinary.ResolveType(typeName, true);

            if (!s_deserializers.TryGetValue(type, out var formatterAndDelegate))
            {
                GetFormatterAndDelegateSlow(type, formatterResolver, out formatterAndDelegate);
            }

            return formatterAndDelegate.Value(formatterAndDelegate.Key, ref reader, formatterResolver);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void GetFormatterAndDelegateSlow(Type type, IFormatterResolver formatterResolver, out KeyValuePair<object, DeserializeMethod> formatterAndDelegate)
        {
            lock (s_deserializers)
            {
                if (!s_deserializers.TryGetValue(type, out formatterAndDelegate))
                {
                    var formatter = formatterResolver.GetFormatterDynamic(type);
                    if (formatter == null)
                    {
                        ThrowHelper.ThrowFormatterNotRegisteredException(type, formatterResolver);
                    }

                    var formatterType = typeof(IMessagePackFormatter<>).GetCachedGenericType(type);
                    var param0 = Expression.Parameter(typeof(object), "formatter");
                    var param1 = Expression.Parameter(typeof(MessagePackReader).MakeByRefType(), "reader");
                    var param2 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                    var deserializeMethodInfo = formatterType.GetRuntimeMethod("Deserialize", new[] { typeof(MessagePackReader).MakeByRefType(), typeof(IFormatterResolver) });

                    var deserialize = Expression.Call(
                        Expression.Convert(param0, formatterType),
                        deserializeMethodInfo,
                        param1,
                        param2);

                    Expression body = deserialize;
                    if (type.IsValueType)
                        body = Expression.Convert(deserialize, typeof(object));
                    var lambda = Expression.Lambda<DeserializeMethod>(body, param0, param1, param2).Compile();

                    formatterAndDelegate = new KeyValuePair<object, DeserializeMethod>(formatter, lambda);
                    s_deserializers.TryAdd(type, formatterAndDelegate);
                }
            }
        }
    }

}
