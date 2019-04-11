namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetStringLength()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return stringLengthDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return stringDecoders[position].Read(ref this, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> ReadStringSegment()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return stringSegmentDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using MessagePack.Internal;

    internal interface IStringDecoder
    {
        String Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new NilString();

        NilString() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            reader.AdvanceWithinSpan(1);
            return null;
        }
    }

    internal sealed class FixString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new FixString();

        FixString() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, position);
            }
            return ReadMultisegment(ref reader, position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String ReadFast(ref MessagePackReader reader, byte position)
        {
            var length = position & 0x1F;
            var valueSpan = reader.PeekFast(1, length);
            reader.AdvanceWithinSpan(length + 1);
            return EncodingUtils.ToString(valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static String ReadMultisegment(ref MessagePackReader reader, byte position)
        {
            var length = position & 0x1F;
            var readSize = length + 1;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return EncodingUtils.ToString(valueSpan.Slice(1, length));
        }
    }

    internal sealed class Str8String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str8String();

        Str8String() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = (int)Unsafe.AddByteOffset(ref position, (IntPtr)1);
            var valueSpan = reader.PeekFast(2, length);
            reader.AdvanceWithinSpan(length + 2);
            return EncodingUtils.ToString(valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static String ReadMultisegment(ref MessagePackReader reader)
        {
            var length = (int)reader.GetRawByte(1);
            var readSize = length + 2;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return EncodingUtils.ToString(valueSpan.Slice(2, length));
        }
    }

    internal sealed class Str16String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str16String();

        Str16String() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked((Unsafe.AddByteOffset(ref position, offset) << 8) + Unsafe.AddByteOffset(ref position, offset + 1));
            var valueSpan = reader.PeekFast(3, length);
            reader.AdvanceWithinSpan(length + 3);
            return EncodingUtils.ToString(valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static String ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;

            var length = unchecked((Unsafe.AddByteOffset(ref b, offset) << 8) + Unsafe.AddByteOffset(ref b, offset + 1));
            var readSize = length + 3;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return EncodingUtils.ToString(valueSpan.Slice(3, length));
        }
    }

    internal sealed class Str32String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str32String();

        Str32String() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref position, offset + 3)));
            var valueSpan = reader.PeekFast(5, length);
            reader.AdvanceWithinSpan(length + 5);
            return EncodingUtils.ToString(valueSpan);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static String ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;

            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref b, offset + 3)));
            var readSize = length + 5;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return EncodingUtils.ToString(valueSpan.Slice(5, length));
        }
    }

    internal sealed class InvalidString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new InvalidString();

        InvalidString() { }

        public String Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }

    internal interface IStringSegmentDecoder
    {
        ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new NilStringSegment();

        NilStringSegment() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            reader.AdvanceWithinSpan(1);
            return ReadOnlySpan<byte>.Empty;
        }
    }

    internal sealed class FixStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new FixStringSegment();

        FixStringSegment() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, position);
            }
            return ReadMultisegment(ref reader, position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<byte> ReadFast(ref MessagePackReader reader, byte position)
        {
            var length = position & 0x1F;
            var valueSpan = reader.PeekFast(1, length);
            reader.AdvanceWithinSpan(length + 1);
            return valueSpan;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> ReadMultisegment(ref MessagePackReader reader, byte position)
        {
            var length = position & 0x1F;
            var readSize = length + 1;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return valueSpan.Slice(1, length);
        }
    }

    internal sealed class Str8StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str8StringSegment();

        Str8StringSegment() { }

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
            var length = (int)Unsafe.AddByteOffset(ref position, (IntPtr)1);
            var valueSpan = reader.PeekFast(2, length);
            reader.AdvanceWithinSpan(length + 2);
            return valueSpan;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<byte> ReadMultisegment(ref MessagePackReader reader)
        {
            var length = (int)reader.GetRawByte(1);
            var readSize = length + 2;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return valueSpan.Slice(2, length);
        }
    }

    internal sealed class Str16StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str16StringSegment();

        Str16StringSegment() { }

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
            var length = unchecked((Unsafe.AddByteOffset(ref position, offset) << 8) + Unsafe.AddByteOffset(ref position, offset + 1));
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

            var length = unchecked((Unsafe.AddByteOffset(ref b, offset) << 8) + Unsafe.AddByteOffset(ref b, offset + 1));
            var readSize = length + 3;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return valueSpan.Slice(3, length);
        }
    }

    internal sealed class Str32StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str32StringSegment();

        Str32StringSegment() { }

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
            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref position, offset + 3)));
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

            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref b, offset + 3)));
            var readSize = length + 5;
            var valueSpan = reader.Peek(readSize);
            reader.Advance(readSize);
            return valueSpan.Slice(5, length);
        }
    }

    internal sealed class InvalidStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new InvalidStringSegment();

        InvalidStringSegment() { }

        public ReadOnlySpan<byte> Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }

    internal interface IStringLengthDecoder
    {
        int Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class NilStringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new NilStringLength();

        NilStringLength() { }

        public int Read(ref MessagePackReader reader, ref byte position) => 1;
    }

    internal sealed class FixStringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new FixStringLength();

        FixStringLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            var length = position & 0x1F;
            return length + 1;
        }
    }

    internal sealed class Str8StringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new Str8StringLength();

        Str8StringLength() { }

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
            var length = (int)Unsafe.AddByteOffset(ref position, (IntPtr)1);
            return length + 2;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var length = (int)reader.GetRawByte(1);
            return length + 2;
        }
    }

    internal sealed class Str16StringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new Str16StringLength();

        Str16StringLength() { }

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
            var length = unchecked((Unsafe.AddByteOffset(ref position, offset) << 8) + Unsafe.AddByteOffset(ref position, offset + 1));
            return length + 3;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;

            var length = unchecked((Unsafe.AddByteOffset(ref b, offset) << 8) + Unsafe.AddByteOffset(ref b, offset + 1));
            return length + 3;
        }
    }

    internal sealed class Str32StringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new Str32StringLength();

        Str32StringLength() { }

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
            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref position, offset + 3)));
            return length + 5;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;

            var length = unchecked((int)(
                (uint)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                (uint)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref b, offset + 3)));
            return length + 5;
        }
    }

    internal sealed class InvalidStringLength : IStringLengthDecoder
    {
        internal static readonly IStringLengthDecoder Instance = new InvalidStringLength();

        InvalidStringLength() { }

        public int Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}