namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar() => (char)ReadUInt16();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return uint16Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IUInt16Decoder
    {
        UInt16 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixUInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new FixUInt16();

        FixUInt16() { }

        public UInt16 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class UInt8UInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new UInt8UInt16();

        UInt8UInt16() { }

        public UInt16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt16 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class UInt16UInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new UInt16UInt16();

        UInt16UInt16() { }

        public UInt16 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt16 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((UInt16)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt16 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (UInt16)((Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
            }
        }
    }

    internal sealed class InvalidUInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new InvalidUInt16();

        InvalidUInt16() { }

        public UInt16 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}