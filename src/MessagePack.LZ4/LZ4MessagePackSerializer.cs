namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using MessagePack.Internal;
#if NET451
    using LZ4;
#else
    using K4os.Compression.LZ4;
#endif

    /// <summary>LZ4 Compressed special serializer.</summary>
    public static partial class LZ4MessagePackSerializer
    {
        private const uint c_zeroSize = 0u;
        private const int c_lz4PackageHeaderSize = 6 + 5; // (ext header size + fixed length size)

        public const sbyte ExtensionTypeCode = ReservedMessagePackExtensionTypeCode.LZ4;

        public const uint NotCompressionSize = 64u;

        /// <summary>Serialize to binary with default resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T obj) => Serialize(obj, null);

        /// <summary>Serialize to binary with specified resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Serialize<T>(T obj, IFormatterResolver resolver)
        {
            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                Serialize(output, obj, resolver);
                return output.ToArray();
            }
        }

        /// <summary>Serialize to binary. Get the raw <see cref="IOwnedBuffer{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeSafe<T>(T obj)
        {
            var output = new ArrayBufferWriter();
            Serialize(output, obj, null);
            return output.OwnedWrittenBuffer;
        }

        /// <summary>Serialize to binary with specified resolver. Get the raw <see cref="IOwnedBuffer{Byte}"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeSafe<T>(T obj, IFormatterResolver resolver)
        {
            var output = new ArrayBufferWriter();
            Serialize(output, obj, resolver);
            return output.OwnedWrittenBuffer;
        }

        /// <summary>Serialize to binary. Get the raw <see cref="IOwnedBuffer{Byte}"/>.
        /// The result can not share across thread and can not hold, so use quickly.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj)
        {
            var output = new LZ4ThreadLocalBufferWriter();
            Serialize(output, obj, null);
            return output.OwnedWrittenBuffer;
        }

        /// <summary>Serialize to binary with specified resolver. Get the raw <see cref="IOwnedBuffer{Byte}"/>.
        /// The result can not share across thread and can not hold, so use quickly.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj, IFormatterResolver resolver)
        {
            var output = new LZ4ThreadLocalBufferWriter();
            Serialize(output, obj, resolver);
            return output.OwnedWrittenBuffer;
        }

        public static void Serialize<T>(IArrayBufferWriter<byte> output, T obj) => Serialize(output, obj, null);

        public static void Serialize<T>(IArrayBufferWriter<byte> output, T obj, IFormatterResolver resolver)
        {
            using (var serializedData = MessagePackSerializer.SerializeUnsafe(obj, resolver))
            {
                var serializedDataLen = serializedData.Count;
                if ((uint)serializedDataLen < NotCompressionSize)
                {
                    // can't write direct, shoganai...
                    serializedData.Span.CopyTo(output.GetSpan(serializedDataLen));
                    output.Advance(serializedDataLen);
                    return;
                }

#if NET451
                var maxOutCount = LZ4Codec.MaximumOutputLength(serializedDataLen);
                var outputBuffer = output.GetBuffer(c_lz4PackageHeaderSize + maxOutCount);
                // write body
                var serializedBuffer = serializedData.Buffer;
                var lz4Length = LZ4Codec.Encode(serializedBuffer.Array, serializedBuffer.Offset, serializedBuffer.Count, outputBuffer.Array, outputBuffer.Offset + c_lz4PackageHeaderSize, outputBuffer.Count - c_lz4PackageHeaderSize);
#else
                var maxOutCount = LZ4Codec.MaximumOutputSize(serializedDataLen);
                var outputSpan = output.GetSpan(c_lz4PackageHeaderSize + maxOutCount);
                // write body
                var lz4Length = LZ4Codec.Encode(serializedData.Span, outputSpan.Slice(c_lz4PackageHeaderSize));
#endif

                ref byte addr = ref output.Origin;
                MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref addr, 0, ExtensionTypeCode, lz4Length + 5);
                MessagePackBinary.WriteInt32ForceInt32Block(ref addr, 6, serializedDataLen);
                output.Advance(lz4Length + c_lz4PackageHeaderSize);
            }
        }

        /// <summary>Serialize to stream.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(Stream stream, T obj) => Serialize(stream, obj, null);

        /// <summary>Serialize to stream with specified resolver.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
        {
            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                Serialize(output, obj, resolver);
#if NETCOREAPP
                stream.Write(output.WrittenSpan);
#else
                var buffer = output.WrittenBuffer;
                stream.Write(buffer.Array, buffer.Offset, buffer.Count);
#endif
            }
        }

        /// <summary>Serialize to stream(async).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask SerializeAsync<T>(Stream stream, T obj) => SerializeAsync(stream, obj, null);

        /// <summary>Serialize to stream(async) with specified resolver.</summary>
        public static async ValueTask SerializeAsync<T>(Stream stream, T obj, IFormatterResolver resolver)
        {
            using (var output = new ArrayBufferWriter())
            {
                Serialize(output, obj, resolver);
#if NETCOREAPP
                await stream.WriteAsync(output.WrittenMemory);
#else
                var buffer = output.WrittenBuffer;
                await stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
#endif
            }
        }

        [Obsolete("=> Serialize{MessagePackWriter, int, T, IFormatterResolver}")]
        public static void SerializeToBlock<T>(ref MessagePackWriter writer, ref int idx, T obj, IFormatterResolver resolver)
        {
            Serialize(ref writer, ref idx, obj, resolver);
        }

        public static void Serialize<T>(ref MessagePackWriter writer, ref int idx, T obj, IFormatterResolver resolver)
        {
            using (var serializedData = MessagePackSerializer.SerializeUnsafe(obj, resolver))
            {
                var serializedDataLen = serializedData.Count;

                if ((uint)serializedDataLen < NotCompressionSize)
                {
                    // can't write direct, shoganai...
                    UnsafeMemory.WriteRaw(ref writer.PinnableAddress, ref MemoryMarshal.GetReference(serializedData.Span), serializedDataLen, ref idx);
                    return;
                }

#if NET451
                var maxOutCount = LZ4Codec.MaximumOutputLength(serializedDataLen);
#else
                var maxOutCount = LZ4Codec.MaximumOutputSize(serializedDataLen);
#endif

                writer.Ensure(idx, c_lz4PackageHeaderSize + maxOutCount);
                var startOffset = idx;
                idx += c_lz4PackageHeaderSize; // acquire ext header position

#if NET451
                var outputBuffer = writer._borrowedBuffer;
                var serializedBuffer = serializedData.Buffer;
                // write body
                var lz4Length = LZ4Codec.Encode(serializedBuffer.Array, serializedBuffer.Offset, serializedBuffer.Count, outputBuffer, idx, outputBuffer.Length - c_lz4PackageHeaderSize);
                idx += lz4Length;
#else
                // write body
                var lz4Length = LZ4Codec.Encode(serializedData.Span, new Span<byte>(writer._borrowedBuffer, idx, writer._capacity - idx));
                idx += lz4Length;
#endif

                ref byte pinnableAddr = ref writer.PinnableAddress;
                // write extension header(always 6 bytes)
                MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(
                    ref pinnableAddr, startOffset, ExtensionTypeCode, lz4Length + 5);

                // write length(always 5 bytes)
                MessagePackBinary.WriteInt32ForceInt32Block(
                    ref pinnableAddr, startOffset + 6, serializedDataLen);
            }
        }




        public static byte[] ToLZ4Binary(byte[] messagePackBinary)
        {
            var serializedDataLen = messagePackBinary.Length;
            if ((uint)serializedDataLen < NotCompressionSize) { return messagePackBinary; }

            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                ToLZ4Binary(output, messagePackBinary);
                return output.ToArray();
            }
        }

        public static ReadOnlySpan<byte> ToLZ4Binary(ReadOnlySpan<byte> messagePackBinary)
        {
            var serializedDataLen = messagePackBinary.Length;
            if ((uint)serializedDataLen < NotCompressionSize) { return messagePackBinary; }

            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                ToLZ4Binary(output, messagePackBinary);
                return output.ToArray();
            }
        }

        public static IOwnedBuffer<byte> ToLZ4BinarySafe(ReadOnlySpan<byte> messagePackBinary)
        {
            var output = new ArrayBufferWriter();
            ToLZ4Binary(output, messagePackBinary);
            return output.OwnedWrittenBuffer;
        }

        public static IOwnedBuffer<byte> ToLZ4BinaryUnsafe(ReadOnlySpan<byte> messagePackBinary)
        {
            var output = new LZ4ThreadLocalBufferWriter();
            ToLZ4Binary(output, messagePackBinary);
            return output.OwnedWrittenBuffer;
        }

        public static void ToLZ4Binary(IArrayBufferWriter<byte> output, ReadOnlySpan<byte> messagePackBinary)
        {
            var serializedDataLen = messagePackBinary.Length;
            if ((uint)serializedDataLen < NotCompressionSize)
            {
                messagePackBinary.CopyTo(output.GetSpan(serializedDataLen));
                output.Advance(serializedDataLen);
                return;
            }

#if NET451
            var maxOutCount = LZ4Codec.MaximumOutputLength(serializedDataLen);
            var outputBuffer = output.GetBuffer(c_lz4PackageHeaderSize + maxOutCount);
            var intputBuffer = messagePackBinary.ToArray();
            // write body
            var lz4Length = LZ4Codec.Encode(intputBuffer, 0, intputBuffer.Length, outputBuffer.Array, outputBuffer.Offset + c_lz4PackageHeaderSize, outputBuffer.Count - c_lz4PackageHeaderSize);
#else
            var maxOutCount = LZ4Codec.MaximumOutputSize(serializedDataLen);
            // write body
            var lz4Length = LZ4Codec.Encode(messagePackBinary, output.GetSpan(c_lz4PackageHeaderSize + maxOutCount).Slice(c_lz4PackageHeaderSize));
#endif

            ref byte addr = ref output.Origin;
            MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref addr, 0, ExtensionTypeCode, lz4Length + 5);
            MessagePackBinary.WriteInt32ForceInt32Block(ref addr, 6, serializedDataLen);
            output.Advance(lz4Length + c_lz4PackageHeaderSize);
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ref MessagePackReader reader, IFormatterResolver resolver) => DeserializeCore<T>(ref reader, resolver);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> lz4SerializedData) => Deserialize<T>(lz4SerializedData, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySpan<byte> lz4SerializedData, IFormatterResolver resolver)
        {
            if (lz4SerializedData.IsEmpty) { return default; }

            var reader = new MessagePackReader(lz4SerializedData);
            return DeserializeCore<T>(ref reader, resolver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySequence<byte> lz4SerializedData) => Deserialize<T>(lz4SerializedData, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(ReadOnlySequence<byte> lz4SerializedData, IFormatterResolver resolver)
        {
            if (lz4SerializedData.IsEmpty) { return default; }

            var reader = new MessagePackReader(lz4SerializedData);
            return DeserializeCore<T>(ref reader, resolver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream) => Deserialize<T>(stream, null, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream, IFormatterResolver resolver) => Deserialize<T>(stream, resolver, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Deserialize<T>(Stream stream, bool readStrict) => Deserialize<T>(stream, null, readStrict);

        public static T Deserialize<T>(Stream stream, IFormatterResolver resolver, bool readStrict)
        {
            if (!readStrict)
            {
#if NET_4_5_GREATER
                // optimize for MemoryStream
                if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    if (c_zeroSize >= (uint)buffer.Count) { return default; }
                    var reader = new MessagePackReader(buffer);
                    return DeserializeCore<T>(ref reader, resolver);
                }
#endif
                // no else.
                {
                    using (var output = new LZ4ThreadLocalBufferWriter())
                    {
                        MessagePackSerializer.FillFromStream(stream, output);
                        if (c_zeroSize >= (uint)output.WrittenCount) { return default; }
                        var reader = new MessagePackReader(output.WrittenSpan);
                        return DeserializeCore<T>(ref reader, resolver);
                    }
                }
            }
            else
            {
                using (var output = new LZ4ThreadLocalBufferWriter())
                {
                    MessagePackBinary.ReadMessageBlockFromStreamUnsafe(stream, output, false);
                    if (c_zeroSize >= (uint)output.WrittenCount) { return default; }
                    var reader = new MessagePackReader(output.WrittenSpan);
                    return DeserializeCore<T>(ref reader, resolver);
                }
            }
        }

        static T DeserializeCore<T>(ref MessagePackReader reader, IFormatterResolver resolver)
        {
            if (resolver == null) { resolver = MessagePackSerializer.DefaultResolver; }
            var formatter = resolver.GetFormatterWithVerify<T>();

            if (reader.IsLZ4Binary())
            {
                var header = reader.ReadExtensionFormatHeader();
                if (header.TypeCode == ExtensionTypeCode)
                {
                    // decode lz4
                    var length = reader.ReadInt32();

                    using (var output = new LZ4ThreadLocalBufferWriter(length))
                    {
#if NET451
                        var outputBuffer = output.GetBuffer();
                        var inputBuffer = reader.Peek(reader.CurrentSpan.Length - c_lz4PackageHeaderSize).ToArray();
                        LZ4Codec.Decode(inputBuffer, 0, inputBuffer.Length, outputBuffer.Array, outputBuffer.Offset, outputBuffer.Count);
#else
                        var outputSpan = output.GetSpan();
                        var valueSpan = reader.Peek(reader.CurrentSpan.Length - c_lz4PackageHeaderSize);
                        LZ4Codec.Decode(valueSpan, outputSpan);
#endif
                        output.Advance(length);

                        var reader0 = new MessagePackReader(output.WrittenSpan);
                        return formatter.Deserialize(ref reader0, resolver);
                    }
                }
            }

            return formatter.Deserialize(ref reader, resolver);
        }




        public static byte[] Decode(byte[] lz4Data)
        {
            var reader = new MessagePackReader(lz4Data);
            if (!reader.IsLZ4Binary()) { return lz4Data; }

            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                DecodeCore(ref reader, output);
                return output.ToArray();
            }
        }

        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> lz4Data)
        {
            var reader = new MessagePackReader(lz4Data);
            if (!reader.IsLZ4Binary()) { return lz4Data; }

            using (var output = new LZ4ThreadLocalBufferWriter())
            {
                DecodeCore(ref reader, output);
                return output.ToArray();
            }
        }

        public static ReadOnlySpan<byte> Decode(Stream stream, bool readStrict = false)
        {
            if (!readStrict)
            {
#if NET_4_5_GREATER
                // optimize for MemoryStream
                if (stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    if (c_zeroSize >= (uint)buffer.Count) { return default; }
                    return Decode(buffer);
                }
#endif
                // no else.
                {
                    using (var output = new ArrayBufferWriter())
                    {
                        MessagePackSerializer.FillFromStream(stream, output);
                        if (c_zeroSize >= (uint)output.WrittenCount) { return default; }
                        return Decode(output.WrittenSpan);
                    }
                }
            }
            else
            {
                using (var output = new ArrayBufferWriter())
                {
                    MessagePackBinary.ReadMessageBlockFromStreamUnsafe(stream, output, false);
                    if (c_zeroSize >= (uint)output.WrittenCount) { return default; }
                    return Decode(output.WrittenSpan);
                }
            }
        }

        static void DecodeCore(ref MessagePackReader reader, IArrayBufferWriter<byte> output)
        {
            if (!reader.IsLZ4Binary()) { return; }

            var header = reader.ReadExtensionFormatHeader();
            if (header.TypeCode == ExtensionTypeCode)
            {
                // decode lz4
                var length = reader.ReadInt32();

#if NET451
                var outputBuffer = output.GetBuffer();
                var inputBuffer = reader.Peek(reader.CurrentSpan.Length - c_lz4PackageHeaderSize).ToArray();
                LZ4Codec.Decode(inputBuffer, 0, inputBuffer.Length, outputBuffer.Array, outputBuffer.Offset, outputBuffer.Count);
#else
                var outputSpan = output.GetSpan(length);
                var valueSpan = reader.Peek(reader.CurrentSpan.Length - c_lz4PackageHeaderSize);
                LZ4Codec.Decode(valueSpan, outputSpan);
#endif
                output.Advance(length);
            }
        }
    }
}
