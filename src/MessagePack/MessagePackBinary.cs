namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using MessagePack.Internal;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Collections;
    using CuteAnt.Reflection;
#endif

    public static class MessagePackBinary
    {
#if NET451
        internal static readonly byte[] Empty = new byte[0];
#else
        internal static readonly byte[] Empty = Array.Empty<byte>();
#endif

        // writer & reader 提取扩展方法
        private static ArrayPool<byte> s_shared = ArrayPool<byte>.Shared;
        public static ArrayPool<byte> Shared
        {
            get => s_shared;
            set
            {
                if (null == value) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value); }
                Interlocked.Exchange(ref s_shared, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static byte* GetPointer(ReadOnlySpan<byte> span)
        {
            return (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        }

        static readonly int MaxBytesPerCharUtf8 = Encoding.UTF8.GetMaxByteCount(1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Utf8MaxBytes(string seq) => seq.Length * MaxBytesPerCharUtf8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SingleToInt32Bits(float value)
        {
            return *((int*)&value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Int32BitsToSingle(int value)
        {
            return *((float*)&value);
        }

        #region -- CopyMemory --

#if NET451
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
        }
#elif NET471
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyMemory(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            fixed (byte* source = &src[srcOffset])
            {
                fixed (byte* destination = &dst[dstOffset])
                {
                    Buffer.MemoryCopy(source, destination, count, count);
                }
            }
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            Unsafe.CopyBlockUnaligned(ref dst[dstOffset], ref src[srcOffset], unchecked((uint)count));
        }
#endif

#if NET471
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyMemory(ref byte source, ref byte destination, int length)
        {
            fixed (byte* src = &source)
            {
                fixed (byte* dst = &destination)
                {
                    Buffer.MemoryCopy(src, dst, length, length);
                }
            }
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(ref byte src, ref byte dst, int length)
        {
            Unsafe.CopyBlockUnaligned(ref dst, ref src, unchecked((uint)length));
        }
#endif

        #endregion

        #region -- WriteInt32ForceInt32Block --

        /// <summary>Acquire static message block(always 5 bytes).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32ForceInt32Block(ref byte destinationSpace, int destOffset, int value)
        {
            uint nValue = (uint)value;
            IntPtr offset = (IntPtr)destOffset;
            unchecked
            {
                Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Int32;
                Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(nValue >> 24);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(nValue >> 16);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)(nValue >> 8);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 4) = (byte)nValue;
            }
        }

        #endregion

        #region -- ArrayHeader --

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArrayHeaderLength(int count)
        {
            if (count <= MessagePackRange.MaxFixArrayCount)
            {
                return 1;
            }
            else if (count <= ushort.MaxValue)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>Write array format header, always use array32 format(length is fixed, 5).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteArrayHeaderForceArray32Block(ref byte destinationSpace, int destOffset, uint count)
        {
            IntPtr offset = (IntPtr)destOffset;
            unchecked
            {
                Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Array32;
                Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(count >> 24);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(count >> 16);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)(count >> 8);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 4) = (byte)count;
            }
        }

        #endregion

        #region -- WriteMapHeaderForceMap32Block --

        /// <summary>Write map format header, always use map32 format(length is fixed, 5).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteMapHeaderForceMap32Block(ref byte destinationSpace, int destOffset, uint count)
        {
            IntPtr offset = (IntPtr)destOffset;
            unchecked
            {
                Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Map32;
                Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(count >> 24);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(count >> 16);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)(count >> 8);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 4) = (byte)count;
            }
        }

        #endregion

        #region -- ExtensionFormat --

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetExtensionFormatHeaderLength(int dataLength)
        {
            const uint ByteMaxValue = byte.MaxValue;
            const uint UShortMaxValue = ushort.MaxValue;

            uint nLen = (uint)dataLength;

            switch (nLen)
            {
                case 1u:
                case 2u:
                case 4u:
                case 8u:
                case 16u:
                    return 2;
                default:
                    if (nLen <= ByteMaxValue)
                    {
                        return 3;
                    }
                    else if (nLen <= UShortMaxValue)
                    {
                        return 4;
                    }
                    return 6;
            }
        }

        /// <summary>Write extension format header, always use ext32 format(length is fixed, 6).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormatHeaderForceExt32Block(ref byte destinationSpace, int destOffset, sbyte typeCode, int dataLength)
        {
            IntPtr offset = (IntPtr)destOffset;
            uint nLen = (uint)dataLength;
            unchecked
            {
                Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Ext32;
                Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(nLen >> 24);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(nLen >> 16);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)(nLen >> 8);
                Unsafe.AddByteOffset(ref destinationSpace, offset + 4) = (byte)nLen;
                Unsafe.AddByteOffset(ref destinationSpace, offset + 5) = (byte)typeCode;
            }
        }

        #endregion

        #region -- Short strings --


        static readonly AsymmetricKeyHashTable<string> s_stringCache = new AsymmetricKeyHashTable<string>(new StringReadOnlySpanByteAscymmetricEqualityComparer());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ResolveString(ReadOnlySpan<byte> utf8Source)
        {
            if (utf8Source.IsEmpty) { return string.Empty; }
            if (!s_stringCache.TryGetValue(utf8Source, out var value))
            {
                ResolveStringSlow(utf8Source, out value);
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ResolveStringSlow(ReadOnlySpan<byte> typeName, out string value)
        {
            if (typeName.IsEmpty)
            {
                value = string.Empty;
                s_stringCache.TryAdd(Empty, value);
            }
            else
            {
                var buffer = typeName.ToArray();
                value = Encoding.UTF8.GetString(buffer);
                s_stringCache.TryAdd(buffer, value);
            }
        }


        static readonly CachedReadConcurrentDictionary<string, byte[]> s_encodedStringCache =
            new CachedReadConcurrentDictionary<string, byte[]>(DictionaryCacheConstants.SIZE_MEDIUM, StringComparer.Ordinal);
        static readonly Func<string, byte[]> s_getEncodedStringBytesFunc = s => GetEncodedStringBytesImpl(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetEncodedStringBytes(string value)
        {
            return s_encodedStringCache.GetOrAdd(value, s_getEncodedStringBytesFunc);
        }

        private static byte[] GetEncodedStringBytesImpl(string value)
        {
            //s_encodedStringCache.GetOrAdd(value,)
            var byteCount = StringEncoding.UTF8.GetByteCount(value);
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                var bytes = new byte[byteCount + 1];
                bytes[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 1);
                return bytes;
            }
            else if (byteCount <= byte.MaxValue)
            {
                var bytes = new byte[byteCount + 2];
                bytes[0] = MessagePackCode.Str8;
                bytes[1] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 2);
                return bytes;
            }
            else if (byteCount <= ushort.MaxValue)
            {
                var bytes = new byte[byteCount + 3];
                bytes[0] = MessagePackCode.Str16;
                bytes[1] = unchecked((byte)(byteCount >> 8));
                bytes[2] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 3);
                return bytes;
            }
            else
            {
                var bytes = new byte[byteCount + 5];
                bytes[0] = MessagePackCode.Str32;
                bytes[1] = unchecked((byte)(byteCount >> 24));
                bytes[2] = unchecked((byte)(byteCount >> 16));
                bytes[3] = unchecked((byte)(byteCount >> 8));
                bytes[4] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 5);
                return bytes;
            }
        }

        #endregion

        #region -- Type --

        static readonly AsymmetricKeyHashTable<Type> s_typeCache = new AsymmetricKeyHashTable<Type>(new StringReadOnlySpanByteAscymmetricEqualityComparer());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ResolveType(ReadOnlySpan<byte> typeName, bool throwOnError)
        {
            if (!s_typeCache.TryGetValue(typeName, out var type))
            {
                ResolveTypeSlow(typeName, throwOnError, out type);
            }
            return type;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ResolveTypeSlow(ReadOnlySpan<byte> typeName, bool throwOnError, out Type type)
        {
            var buffer = typeName.ToArray();
            var str = Encoding.UTF8.GetString(buffer);
            if (throwOnError)
            {
                type = TypeUtils.ResolveType(str);
            }
            else
            {
                TypeUtils.TryResolveType(str, out type);
            }
            if (type != null) { s_typeCache.TryAdd(buffer, type); }
        }

        static readonly ThreadsafeTypeKeyHashTable<byte[]> s_encodedTypeNameCache = new ThreadsafeTypeKeyHashTable<byte[]>(capacity: 64);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetEncodedTypeName(Type type)
        {
            if (!s_encodedTypeNameCache.TryGetValue(type, out byte[] typeName))
            {
                typeName = GetEncodedTypeNameSlow(type);
            }
            return typeName;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte[] GetEncodedTypeNameSlow(Type type)
        {
            var typeName = RuntimeTypeNameFormatter.Format(type);
            var encodedTypeName = MessagePackBinary.GetEncodedStringBytes(typeName);
            s_encodedTypeNameCache.TryAdd(type, encodedTypeName);
            return encodedTypeName;
        }

        #endregion

        #region == ReadMessageBlockFromStreamUnsafe ==

        /// <summary>Read MessageBlock, returns byte[] block is in MemoryPool so careful to use.</summary>
        internal static void ReadMessageBlockFromStreamUnsafe(Stream stream, IArrayBufferWriter<byte> output)
        {
            ReadMessageBlockFromStreamCore(stream, output, false);
        }

        /// <summary>Read MessageBlock, returns byte[] block is in MemoryPool so careful to use.</summary>
        internal static void ReadMessageBlockFromStreamUnsafe(Stream stream, IArrayBufferWriter<byte> output, bool readOnlySingleMessage)
        {
            ReadMessageBlockFromStreamCore(stream, output, readOnlySingleMessage);
        }

        static void ReadMessageBlockFromStreamCore(Stream stream, IArrayBufferWriter<byte> output, bool readOnlySingleMessage)
        {
            var byteCode = stream.ReadByte();
            if (byteCode < 0 || byte.MaxValue < byteCode)
            {
                ThrowHelper.ThrowInvalidOperationException_Code_Detected(byteCode);
            }

            var code = (byte)byteCode;

            var span = output.GetSpan();
            span[0] = code;
            output.Advance(1);

            var type = MessagePackCode.ToMessagePackType(code);
            switch (type)
            {
                case MessagePackType.Integer:
                    {
                        var readCount = 0;
                        if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt) return;
                        else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt) return;

                        switch (code)
                        {
                            case MessagePackCode.Int8: readCount = 1; break;
                            case MessagePackCode.Int16: readCount = 2; break;
                            case MessagePackCode.Int32: readCount = 4; break;
                            case MessagePackCode.Int64: readCount = 8; break;
                            case MessagePackCode.UInt8: readCount = 1; break;
                            case MessagePackCode.UInt16: readCount = 2; break;
                            case MessagePackCode.UInt32: readCount = 4; break;
                            case MessagePackCode.UInt64: readCount = 8; break;
                            default: throw new InvalidOperationException("Invalid Code");
                        }
                        ReadFully(stream, output, readCount);

                        return;
                    }
                case MessagePackType.Unknown:
                case MessagePackType.Nil:
                case MessagePackType.Boolean:
                    return;

                case MessagePackType.Float:
                    if (code == MessagePackCode.Float32)
                    {
                        ReadFully(stream, output, 4);

                        return;
                    }
                    else
                    {
                        ReadFully(stream, output, 8);

                        return;
                    }
                case MessagePackType.String:
                    {
                        if (MessagePackCode.MinFixStr <= code && code <= MessagePackCode.MaxFixStr)
                        {
                            var length = code & 0x1F;
                            ReadFully(stream, output, length);

                            return;
                        }

                        switch (code)
                        {
                            case MessagePackCode.Str8:
                                {
                                    ReadFully(stream, output, 1);
                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 1);
                                    var length = lenSpan[0];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            case MessagePackCode.Str16:
                                {
                                    ReadFully(stream, output, 2);
                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 2);
                                    var length = (lenSpan[0] << 8) | lenSpan[1];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            case MessagePackCode.Str32:
                                {
                                    ReadFully(stream, output, 4);
                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 4);
                                    var length = (lenSpan[0] << 24) | (lenSpan[1] << 16) | (lenSpan[2] << 8) | lenSpan[3];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            default: throw new InvalidOperationException("Invalid Code");
                        }
                    }
                case MessagePackType.Binary:
                    {
                        switch (code)
                        {
                            case MessagePackCode.Bin8:
                                {
                                    ReadFully(stream, output, 1);

                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 1);
                                    var length = lenSpan[0];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            case MessagePackCode.Bin16:
                                {
                                    ReadFully(stream, output, 2);
                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 2);
                                    var length = (lenSpan[0] << 8) | lenSpan[1];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            case MessagePackCode.Bin32:
                                {
                                    ReadFully(stream, output, 4);
                                    var lenSpan = output.WrittenSpan.Slice(output.WrittenCount - 4);
                                    var length = (lenSpan[0] << 24) | (lenSpan[1] << 16) | (lenSpan[2] << 8) | lenSpan[3];
                                    ReadFully(stream, output, length);

                                    return;
                                }
                            default: throw new InvalidOperationException("Invalid Code");
                        }
                    }
                case MessagePackType.Array:
                    {
                        var readHeaderSize = 0;

                        if (MessagePackCode.MinFixArray <= code && code <= MessagePackCode.MaxFixArray) readHeaderSize = 0;
                        else if (code == MessagePackCode.Array16) readHeaderSize = 2;
                        else if (code == MessagePackCode.Array32) readHeaderSize = 4;
                        if (readHeaderSize != 0)
                        {
                            ReadFully(stream, output, readHeaderSize);
                        }

                        var reader = new MessagePackReader(output.WrittenSpan.Slice(output.WrittenCount - readHeaderSize - 1));
                        var length = reader.ReadArrayHeaderRaw();
                        if (!readOnlySingleMessage)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                ReadMessageBlockFromStreamCore(stream, output, readOnlySingleMessage);
                            }
                        }

                        return;
                    }
                case MessagePackType.Map:
                    {
                        var readHeaderSize = 0;

                        if (MessagePackCode.MinFixMap <= code && code <= MessagePackCode.MaxFixMap) readHeaderSize = 0;
                        else if (code == MessagePackCode.Map16) readHeaderSize = 2;
                        else if (code == MessagePackCode.Map32) readHeaderSize = 4;
                        if (readHeaderSize != 0)
                        {
                            ReadFully(stream, output, readHeaderSize);
                        }

                        var reader = new MessagePackReader(output.WrittenSpan.Slice(output.WrittenCount - readHeaderSize - 1));
                        var length = reader.ReadMapHeaderRaw();
                        if (!readOnlySingleMessage)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                ReadMessageBlockFromStreamCore(stream, output, readOnlySingleMessage); // key
                                ReadMessageBlockFromStreamCore(stream, output, readOnlySingleMessage); // value
                            }
                        }

                        return;
                    }
                case MessagePackType.Extension:
                    {
                        var readHeaderSize = 0;

                        switch (code)
                        {
                            case MessagePackCode.FixExt1: readHeaderSize = 1; break;
                            case MessagePackCode.FixExt2: readHeaderSize = 1; break;
                            case MessagePackCode.FixExt4: readHeaderSize = 1; break;
                            case MessagePackCode.FixExt8: readHeaderSize = 1; break;
                            case MessagePackCode.FixExt16: readHeaderSize = 1; break;
                            case MessagePackCode.Ext8: readHeaderSize = 2; break;
                            case MessagePackCode.Ext16: readHeaderSize = 3; break;
                            case MessagePackCode.Ext32: readHeaderSize = 5; break;
                            default: throw new InvalidOperationException("Invalid Code");
                        }

                        ReadFully(stream, output, readHeaderSize);

                        if (!readOnlySingleMessage)
                        {
                            var reader = new MessagePackReader(output.WrittenSpan.Slice(output.WrittenCount - readHeaderSize - 1));
                            var header = reader.ReadExtensionFormatHeader();

                            ReadFully(stream, output, (int)header.Length);
                        }
                        return;
                    }
                default: throw new InvalidOperationException("Invalid Code");
            }
        }

#if NETCOREAPP
        static void ReadFully(Stream stream, IArrayBufferWriter<byte> bufferWriter, int readSize)
        {
            var nextLen = readSize;
            var outputSpan = bufferWriter.GetSpan(readSize);
            while (nextLen != 0)
            {
                var len = stream.Read(outputSpan.Slice(0, nextLen));
                if (len == -1) return;

                bufferWriter.Advance(len);
                outputSpan = bufferWriter.GetSpan();
                nextLen = nextLen - len;
            }
        }
#else
        static void ReadFully(Stream stream, IArrayBufferWriter<byte> bufferWriter, int readSize)
        {
            var nextLen = readSize;
            var buffer = bufferWriter.GetBuffer(readSize);
            while (nextLen != 0)
            {
                var len = stream.Read(buffer.Array, buffer.Offset, nextLen);
                if (len == -1) return;

                bufferWriter.Advance(len);
                buffer = bufferWriter.GetBuffer();
                nextLen = nextLen - len;
            }
        }
#endif

        #endregion
    }
}
