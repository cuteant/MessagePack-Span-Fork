namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using MessagePack.Formatters;

    // Typeless API
    public static partial class LZ4MessagePackSerializer
    {
        public static class Typeless
        {
            static IFormatterResolver defaultResolver = MessagePack.Resolvers.TypelessContractlessStandardResolver.Instance;

            public static void RegisterDefaultResolver(params IFormatterResolver[] resolvers)
            {
                CompositeResolver.Register(resolvers);
                defaultResolver = CompositeResolver.Instance;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static byte[] Serialize(object obj)
            {
                return LZ4MessagePackSerializer.Serialize(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IOwnedBuffer<byte> SerializeSafe(object obj)
            {
                return LZ4MessagePackSerializer.SerializeSafe(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IOwnedBuffer<byte> SerializeUnsafe(object obj)
            {
                return LZ4MessagePackSerializer.SerializeUnsafe(obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Serialize<T>(IArrayBufferWriter<byte> output, T obj)
            {
                LZ4MessagePackSerializer.Serialize(output, obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Serialize(Stream stream, object obj)
            {
                LZ4MessagePackSerializer.Serialize(stream, obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ValueTask SerializeAsync(Stream stream, object obj)
            {
                return LZ4MessagePackSerializer.SerializeAsync(stream, obj, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(ReadOnlySpan<byte> bytes)
            {
                return LZ4MessagePackSerializer.Deserialize<object>(bytes, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(ReadOnlySequence<byte> sequence)
            {
                return LZ4MessagePackSerializer.Deserialize<object>(sequence, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(Stream stream)
            {
                return LZ4MessagePackSerializer.Deserialize<object>(stream, defaultResolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static object Deserialize(Stream stream, bool readStrict)
            {
                return LZ4MessagePackSerializer.Deserialize<object>(stream, defaultResolver, readStrict);
            }

            class CompositeResolver : FormatterResolver
            {
                public static readonly CompositeResolver Instance = new CompositeResolver();

                static bool isFreezed = false;
                static IFormatterResolver[] resolvers = new IFormatterResolver[0];

                CompositeResolver() { }

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
