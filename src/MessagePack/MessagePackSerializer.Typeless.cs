namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using MessagePack.Formatters;

    // Typeless API
    public static partial class MessagePackSerializer
    {
        public static class Typeless
        {
            static IFormatterResolver defaultResolver = MessagePack.Resolvers.TypelessContractlessStandardResolver.Instance;

            public static void RegisterDefaultResolver(params IFormatterResolver[] resolvers)
            {
                CompositeResolver.Register(resolvers);
                Interlocked.Exchange(ref defaultResolver, CompositeResolver.Instance);
            }

            internal static readonly Type DefaultTypelessFormatterType = typeof(TypelessFormatter);
            private static readonly IMessagePackFormatter<object> s_defaultTypelessFormatter = MessagePack.Formatters.TypelessFormatter.Instance;
            private static IMessagePackFormatter<object> s_typelessFormatter;
            private static Type s_typelessFormatterType;
            internal static IMessagePackFormatter<object> TypelessFormatter
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Volatile.Read(ref s_typelessFormatter) ?? s_defaultTypelessFormatter;
            }
            internal static Type TypelessFormatterType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Volatile.Read(ref s_typelessFormatterType) ?? DefaultTypelessFormatterType;
            }
            public static void RegisterTypelessFormatter(IMessagePackFormatter<object> typelessFormatter)
            {
                if (null == typelessFormatter) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.typelessFormatter); }

                if (Interlocked.CompareExchange(ref s_typelessFormatter, typelessFormatter, null) == null)
                {
                    s_typelessFormatterType = typelessFormatter.GetType();
                }
            }

            /// <summary>TBD</summary>
            public static object DeepCopy(object source) => DeepCopy(source, null);

            /// <summary>TBD</summary>
            public static object DeepCopy(object source, IFormatterResolver resolver)
            {
                if (source == null) { return null; }
                if (null == resolver) { resolver = defaultResolver; }

                var type = source.GetType();
                using (var serializedObject = MessagePackSerializer.SerializeUnsafe(source, resolver))
                {
                    return MessagePackSerializer.Deserialize<object>(serializedObject.Span, resolver);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte[] Serialize(object obj)
            {
                return MessagePackSerializer.Serialize(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IOwnedBuffer<byte> SerializeSafe(object obj)
            {
                return MessagePackSerializer.SerializeSafe(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IOwnedBuffer<byte> SerializeUnsafe(object obj)
            {
                return MessagePackSerializer.SerializeUnsafe(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Serialize(Stream stream, object obj)
            {
                MessagePackSerializer.Serialize(stream, obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ValueTask SerializeAsync(Stream stream, object obj)
            {
                return MessagePackSerializer.SerializeAsync(stream, obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(ReadOnlySpan<byte> bytes)
            {
                return MessagePackSerializer.Deserialize<object>(bytes, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(ReadOnlySequence<byte> sequence)
            {
                return MessagePackSerializer.Deserialize<object>(sequence, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(Stream stream)
            {
                return MessagePackSerializer.Deserialize<object>(stream, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(Stream stream, bool readStrict)
            {
                return MessagePackSerializer.Deserialize<object>(stream, defaultResolver, readStrict);
            }

            sealed class CompositeResolver : FormatterResolver
            {
                public static readonly CompositeResolver Instance = new CompositeResolver();

                static bool isFreezed = false;
                static IFormatterResolver[] resolvers = new IFormatterResolver[0];

                CompositeResolver()
                {
                }

                public static void Register(params IFormatterResolver[] resolvers)
                {
                    if (isFreezed)
                    {
                        ThrowHelper.ThrowInvalidOperationException_Register_Resolvers();
                    }

                    CompositeResolver.resolvers = resolvers;
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
                        isFreezed = true;

                        foreach (var item in resolvers)
                        {
                            var f = item.GetFormatter<T>();
                            if (f != null)
                            {
                                formatter = f;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
