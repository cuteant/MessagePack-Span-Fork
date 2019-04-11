namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetInt32Length()
        {
            return int32LengthDecoders[_currentSpan[_currentSpanIndex]].Read();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return int32Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IInt32Decoder
    {
        Int32 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixNegativeInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new FixNegativeInt32();

        FixNegativeInt32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)position);
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class FixInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new FixInt32();

        FixInt32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class UInt8Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt8Int32();

        UInt8Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }
    internal sealed class UInt16Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt16Int32();

        UInt16Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = (Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1);
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            return (Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1);
        }
    }

    internal sealed class UInt32Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt32Int32();

        UInt32Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = checked((int)(
                (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref position, offset + 3)));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            checked
            {
                return (int)(
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3));
            }
        }
    }

    internal sealed class Int8Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int8Int32();

        Int8Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class Int16Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int16Int32();

        Int16Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((short)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (short)((Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
            }
        }
    }

    internal sealed class Int32Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int32Int32();

        Int32Int32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (Unsafe.AddByteOffset(ref position, offset) << 24) |
                (Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                Unsafe.AddByteOffset(ref position, offset + 3));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    Unsafe.AddByteOffset(ref b, offset + 3);
            }
        }
    }

    internal sealed class InvalidInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new InvalidInt32();

        InvalidInt32() { }

        public Int32 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }

    internal interface IInt32LengthDecoder
    {
        Int32 Read();
    }

    internal sealed class FixNegativeInt32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new FixNegativeInt32Length();

        FixNegativeInt32Length() { }

        public Int32 Read() => 1;
    }

    internal sealed class FixInt32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new FixInt32Length();

        FixInt32Length() { }

        public Int32 Read() => 1;
    }

    internal sealed class UInt8Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new UInt8Int32Length();

        UInt8Int32Length() { }

        public Int32 Read() => 2;
    }
    internal sealed class UInt16Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new UInt16Int32Length();

        UInt16Int32Length() { }

        public Int32 Read() => 3;
    }

    internal sealed class UInt32Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new UInt32Int32Length();

        UInt32Int32Length() { }

        public Int32 Read() => 5;
    }

    internal sealed class Int8Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new Int8Int32Length();

        Int8Int32Length() { }

        public Int32 Read() => 2;
    }

    internal sealed class Int16Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new Int16Int32Length();

        Int16Int32Length() { }

        public Int32 Read() => 3;
    }

    internal sealed class Int32Int32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new Int32Int32Length();

        Int32Int32Length() { }

        public Int32 Read() => 5;
    }

    internal sealed class InvalidInt32Length : IInt32LengthDecoder
    {
        internal static readonly IInt32LengthDecoder Instance = new InvalidInt32Length();

        InvalidInt32Length() { }

        public Int32 Read()
        {
            throw new InvalidOperationException("code is invalid.");
        }
    }
}