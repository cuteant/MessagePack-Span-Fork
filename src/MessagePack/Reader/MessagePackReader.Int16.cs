namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return int16Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IInt16Decoder
    {
        Int16 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixNegativeInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new FixNegativeInt16();

        FixNegativeInt16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)position);
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class FixInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new FixInt16();

        FixInt16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class UInt8Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new UInt8Int16();

        UInt8Int16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int16 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class UInt16Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new UInt16Int16();

        UInt16Int16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = checked((short)((Unsafe.AddByteOffset(ref position, offset) << 8) + Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int16 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            return checked((short)((Unsafe.AddByteOffset(ref b, offset) << 8) + Unsafe.AddByteOffset(ref b, offset + 1)));
        }
    }

    internal sealed class Int8Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new Int8Int16();

        Int8Int16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int16 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class Int16Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new Int16Int16();

        Int16Int16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((short)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int16 ReadMultisegment(ref MessagePackReader reader)
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

    internal sealed class InvalidInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new InvalidInt16();

        InvalidInt16() { }

        public Int16 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}