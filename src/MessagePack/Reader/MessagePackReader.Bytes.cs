namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEncodedBytesLength()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return bytesLengthDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return bytesDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> ReadSpan()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return spanDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ReadOnlySpan<byte> PeekFast(int offset, int count)
        {
            return _currentSpan.Slice(_currentSpanIndex + offset, count);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IBytesDecoder
    {
        byte[] Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilBytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new NilBytes();

        NilBytes() { }

        public byte[] Read(ref MessagePackReader reader, ref byte position)
        {
            reader.Advance(1);
            return null;
        }
    }

    internal sealed class Bin8Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin8Bytes();

        Bin8Bytes() { }

        public byte[] Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var length = Unsafe.AddByteOffset(ref position, offset);

            if (0u >= length)
            {
                reader.AdvanceWithinSpan(2);
                return MessagePackBinary.Empty;
            }

            var newBytes = new byte[length];
            MessagePackBinary.CopyMemory(ref Unsafe.AddByteOffset(ref position, offset + 1), ref newBytes[0], length);
            reader.AdvanceWithinSpan(length + 2);
            return newBytes;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte[] ReadMultisegment(ref MessagePackReader reader)
        {
            var length = reader.GetRawByte(1);

            if (0u >= length)
            {
                reader.Advance(2);
                return MessagePackBinary.Empty;
            }

            var readSize = length + 2;
            var valueSpan = reader.Peek(readSize);
            var newBytes = new byte[length];
            valueSpan.Slice(2, length).CopyTo(newBytes);
            reader.Advance(readSize);
            return newBytes;
        }
    }

    internal sealed class Bin16Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin16Bytes();

        Bin16Bytes() { }

        public byte[] Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 1));

            var newBytes = new byte[length];
            MessagePackBinary.CopyMemory(ref Unsafe.AddByteOffset(ref position, offset + 2), ref newBytes[0], length);
            reader.AdvanceWithinSpan(length + 3);
            return newBytes;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte[] ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 1));

            var readSize = length + 3;
            var valueSpan = reader.Peek(readSize);
            var newBytes = new byte[length];
            valueSpan.Slice(3, length).CopyTo(newBytes);
            reader.Advance(readSize);
            return newBytes;
        }
    }

    internal sealed class Bin32Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin32Bytes();

        Bin32Bytes() { }

        public byte[] Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;

            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 24 |
                Unsafe.AddByteOffset(ref position, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref position, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 3));

            var newBytes = new byte[length];
            MessagePackBinary.CopyMemory(ref Unsafe.AddByteOffset(ref position, offset + 4), ref newBytes[0], length);
            reader.AdvanceWithinSpan(length + 5);
            return newBytes;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte[] ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 24 |
                Unsafe.AddByteOffset(ref b, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref b, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 3));

            var readSize = length + 5;
            var valueSpan = reader.Peek(readSize);
            var newBytes = new byte[length];
            valueSpan.Slice(5, length).CopyTo(newBytes);
            reader.Advance(readSize);
            return newBytes;
        }
    }

    internal sealed class InvalidBytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new InvalidBytes();

        InvalidBytes() { }

        public byte[] Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }

    internal interface ISpanDecoder
    {
        ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilBytesSpan : ISpanDecoder
    {
        internal static readonly ISpanDecoder Instance = new NilBytesSpan();

        NilBytesSpan() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            reader.Advance(1);
            return ReadOnlySpan<byte>.Empty;
        }
    }

    internal sealed class Bin8Span : ISpanDecoder
    {
        internal static readonly ISpanDecoder Instance = new Bin8Span();

        Bin8Span() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = Unsafe.AddByteOffset(ref position, (IntPtr)1);

            if (0u >= length)
            {
                reader.AdvanceWithinSpan(2);
                return ReadOnlySpan<byte>.Empty;
            }

            var valueSpan = reader.PeekFast(2, length);
            reader.AdvanceWithinSpan(length + 2);
            return valueSpan;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> ReadMultisegment(ref MessagePackReader reader)
        {
            var length = reader.GetRawByte(1);

            if (0u >= length)
            {
                reader.Advance(2);
                return ReadOnlySpan<byte>.Empty;
            }

            var readSize = length + 2;
            var valueSpan = reader.Peek(readSize).Slice(2, length);
            reader.Advance(readSize);
            return valueSpan;
        }
    }

    internal sealed class Bin16Span : ISpanDecoder
    {
        internal static readonly ISpanDecoder Instance = new Bin16Span();

        Bin16Span() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 1));

            var valueSpan = reader.PeekFast(3, length);
            reader.AdvanceWithinSpan(length + 3);
            return valueSpan;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 1));

            var readSize = length + 3;
            var valueSpan = reader.Peek(readSize).Slice(3, length);
            reader.Advance(readSize);
            return valueSpan;
        }
    }

    internal sealed class Bin32Span : ISpanDecoder
    {
        internal static readonly ISpanDecoder Instance = new Bin32Span();

        Bin32Span() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 24 |
                Unsafe.AddByteOffset(ref position, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref position, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 3));

            var valueSpan = reader.PeekFast(5, length);
            reader.AdvanceWithinSpan(length + 5);
            return valueSpan;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 24 |
                Unsafe.AddByteOffset(ref b, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref b, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 3));

            var readSize = length + 5;
            var valueSpan = reader.Peek(readSize).Slice(5, length);
            reader.Advance(readSize);
            return valueSpan;
        }
    }

    internal sealed class InvalidSpan : ISpanDecoder
    {
        internal static readonly ISpanDecoder Instance = new InvalidSpan();

        InvalidSpan() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }

    internal interface IBytesLengthDecoder
    {
        int Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilBytesLength : IBytesLengthDecoder
    {
        internal static readonly IBytesLengthDecoder Instance = new NilBytesLength();

        NilBytesLength() { }

        public int Read(ref MessagePackReader reader, ref byte position) => 1;
    }

    internal sealed class Bin8BytesLength : IBytesLengthDecoder
    {
        internal static readonly IBytesLengthDecoder Instance = new Bin8BytesLength();

        Bin8BytesLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = Unsafe.AddByteOffset(ref position, (IntPtr)1);

            if (0u >= length) { return 2; }

            return length + 2;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var length = reader.GetRawByte(1);

            if (0u >= length) { return 2; }

            return length + 2;
        }
    }

    internal sealed class Bin16BytesLength : IBytesLengthDecoder
    {
        internal static readonly IBytesLengthDecoder Instance = new Bin16BytesLength();

        Bin16BytesLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 1));

            return length + 3;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 1));

            return length + 3;
        }
    }

    internal sealed class Bin32BytesLength : IBytesLengthDecoder
    {
        internal static readonly IBytesLengthDecoder Instance = new Bin32BytesLength();

        Bin32BytesLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref position, offset) << 24 |
                Unsafe.AddByteOffset(ref position, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref position, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref position, offset + 3));

            return length + 5;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                Unsafe.AddByteOffset(ref b, offset) << 24 |
                Unsafe.AddByteOffset(ref b, offset + 1) << 16 |
                Unsafe.AddByteOffset(ref b, offset + 2) << 8 |
                Unsafe.AddByteOffset(ref b, offset + 3));

            return length + 5;
        }
    }

    internal sealed class InvalidBytesLength : IBytesLengthDecoder
    {
        internal static readonly IBytesLengthDecoder Instance = new InvalidBytesLength();

        InvalidBytesLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}