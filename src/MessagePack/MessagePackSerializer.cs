namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Buffers;
#endif

    /// <summary>High-Level API of MessagePack for C#.</summary>
    public static partial class MessagePackSerializer
    {
        private const uint c_zeroSize = 0u;

        static IFormatterResolver s_defaultResolver = MessagePack.Resolvers.StandardResolver.Instance;

        /// <summary>FormatterResolver that used resolver less overloads. If does not set it, used StandardResolver.</summary>
        public static IFormatterResolver DefaultResolver
        {
            get => s_defaultResolver;
            set
            {
                if (value == null) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value); }
                Interlocked.Exchange(ref s_defaultResolver, value);
            }
        }

        /// <summary>Is resolver decided?</summary>
        public static bool IsInitialized => s_defaultResolver != null;

        /// <summary>Set default resolver of MessagePackSerializer APIs.</summary>
        /// <param name="resolver"></param>
        [Obsolete("=> DefaultResolver")]
        public static void SetDefaultResolver(IFormatterResolver resolver)
        {
            Interlocked.Exchange(ref s_defaultResolver, resolver);
        }

        /// <summary>TBD</summary>
        public static T DeepCopy<T>(T source) => DeepCopy(source, null);

        /// <summary>TBD</summary>
        public static T DeepCopy<T>(T source, IFormatterResolver resolver)
        {
            if (source == null) { return default; }
            if (null == resolver) { resolver = s_defaultResolver; }

            var type = source.GetType(); // 要获取对象本身的类型，忽略基类、接口
            using (var serializedObject = SerializeUnsafe<object>(source, resolver))
            {
                return (T)NonGeneric.Deserialize(type, serializedObject.Span, resolver);
            }
        }

        /// <summary>Serialize to binary with default resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T obj) => Serialize(obj, s_defaultResolver);

        /// <summary>Serialize to binary with specified resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T obj, IFormatterResolver resolver)
        {
            if (null == resolver) { resolver = s_defaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();

            var idx = 0;
            var writer = new MessagePackWriter(true);
            formatter.Serialize(ref writer, ref idx, obj, resolver);
            return writer.ToArray(idx);
        }

        /// <summary>Serialize to binary. Get the raw <see cref="IOwnedBuffer{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeSafe<T>(T obj) => SerializeSafe(obj, s_defaultResolver);

        /// <summary>Serialize to binary with specified resolver. Get the raw <see cref="IOwnedBuffer{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeSafe<T>(T obj, IFormatterResolver resolver)
        {
            if (null == resolver) { resolver = s_defaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();

            var idx = 0;
            var writer = new MessagePackWriter(false);
            formatter.Serialize(ref writer, ref idx, obj, resolver);
            return writer.ToOwnedBuffer(idx);
        }

        /// <summary>Serialize to binary. Get the raw <see cref="IOwnedBuffer{Byte}"/>.
        /// The result can not share across thread and can not hold, so use quickly.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj) => SerializeUnsafe(obj, s_defaultResolver);

        /// <summary>Serialize to binary with specified resolver. Get the raw <see cref="IOwnedBuffer{Byte}"/>.
        /// The result can not share across thread and can not hold, so use quickly.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj, IFormatterResolver resolver)
        {
            if (null == resolver) { resolver = s_defaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();

            var idx = 0;
            var writer = new MessagePackWriter(true);
            formatter.Serialize(ref writer, ref idx, obj, resolver);
            return writer.ToOwnedBuffer(idx);
        }

        /// <summary>Serialize to stream.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(Stream stream, T obj) => Serialize(stream, obj, s_defaultResolver);

        /// <summary>Serialize to stream with specified resolver.</summary>
        public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
        {
            using (var output = SerializeUnsafe(obj, resolver))
            {
#if NETCOREAPP
                stream.Write(output.Span);
#else
                var buffer = output.Buffer;
                stream.Write(buffer.Array, buffer.Offset, buffer.Count);
#endif
            }
        }

        /// <summary>Serialize to stream(async).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask SerializeAsync<T>(Stream stream, T obj) => SerializeAsync(stream, obj, s_defaultResolver);

        /// <summary>Serialize to stream(async) with specified resolver.</summary>
        public static async ValueTask SerializeAsync<T>(Stream stream, T obj, IFormatterResolver resolver)
        {
            using (var output = SerializeSafe(obj, resolver))
            {
#if NETCOREAPP
                await stream.WriteAsync(output.Memory);
#else
                var buffer = output.Buffer;
                await stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
#endif
            }
        }

        /// <summary>Reflect of resolver.GetFormatterWithVerify[T].Serialize.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(ref MessagePackWriter writer, ref int idx, T obj, IFormatterResolver resolver)
        {
            resolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref idx, obj, resolver);
        }

        /// <summary>Reflect of resolver.GetFormatterWithVerify[T].Deserialize.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ref MessagePackReader reader, IFormatterResolver resolver)
        {
            return resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, resolver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> bytes) => Deserialize<T>(bytes, s_defaultResolver);

        public static T Deserialize<T>(ReadOnlySpan<byte> bytes, IFormatterResolver resolver)
        {
            if (bytes.IsEmpty) { return default; }

            if (null == resolver) { resolver = s_defaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();
            var reader = new MessagePackReader(bytes);
            return formatter.Deserialize(ref reader, resolver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySequence<byte> sequence) => Deserialize<T>(sequence, s_defaultResolver);

        public static T Deserialize<T>(ReadOnlySequence<byte> sequence, IFormatterResolver resolver)
        {
            if (sequence.IsEmpty) { return default; }

            if (null == resolver) { resolver = s_defaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();
            var reader = new MessagePackReader(sequence);
            return formatter.Deserialize(ref reader, resolver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream) => Deserialize<T>(stream, s_defaultResolver, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream, IFormatterResolver resolver) => Deserialize<T>(stream, resolver, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream, bool readStrict) => Deserialize<T>(stream, s_defaultResolver, readStrict);

        public static T Deserialize<T>(Stream stream, IFormatterResolver resolver, bool readStrict)
        {
            if (!readStrict)
            {
#if NET_4_5_GREATER
                // optimize for MemoryStream
                if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    return Deserialize<T>(buffer, resolver);
                }
#endif
                // no else.
                {
                    using (var output = new ThreadLocalBufferWriter())
                    {
                        FillFromStream(stream, output);
                        return Deserialize<T>(output.WrittenSpan, resolver);
                    }
                }
            }
            else
            {
                using (var output = new ThreadLocalBufferWriter())
                {
                    MessagePackBinary.ReadMessageBlockFromStreamUnsafe(stream, output, false);
                    return Deserialize<T>(output.WrittenSpan, resolver);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void FillFromStream(Stream input, IArrayBufferWriter<byte> bufferWriter)
        {
            int read;
#if NETCOREAPP
            var outputSpan = bufferWriter.GetSpan();
            while ((read = input.Read(outputSpan)) > 0)
            {
                bufferWriter.Advance(read);
                outputSpan = bufferWriter.GetSpan();
            }
#else
            var buffer = bufferWriter.GetBuffer();
            while ((read = input.Read(buffer.Array, buffer.Offset, buffer.Count)) > 0)
            {
                bufferWriter.Advance(read);
                buffer = bufferWriter.GetBuffer();
            }
#endif
        }
    }
}
