namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return uint32Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IUInt32Decoder
    {
        UInt32 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixUInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new FixUInt32();

        FixUInt32() { }

        public UInt32 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class UInt8UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt8UInt32();

        UInt8UInt32() { }

        public UInt32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt32 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class UInt16UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt16UInt32();

        UInt16UInt32() { }

        public UInt32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((UInt32)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (UInt32)((Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
            }
        }
    }

    internal sealed class UInt32UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt32UInt32();

        UInt32UInt32() { }

        public UInt32 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref position, offset + 3));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt32 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3);
            }
        }
    }

    internal sealed class InvalidUInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new InvalidUInt32();

        InvalidUInt32() { }

        public UInt32 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}