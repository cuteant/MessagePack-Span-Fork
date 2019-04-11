namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return uint64Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IUInt64Decoder
    {
        UInt64 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixUInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new FixUInt64();

        FixUInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class UInt8UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt8UInt64();

        UInt8UInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt64 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class UInt16UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt16UInt64();

        UInt16UInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((UInt64)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (UInt64)((Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
            }
        }
    }

    internal sealed class UInt32UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt32UInt64();

        UInt32UInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (ulong)(Unsafe.AddByteOffset(ref position, offset) << 24) +
                (ulong)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) +
                (ulong)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) +
                (ulong)Unsafe.AddByteOffset(ref position, offset + 3));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (ulong)(Unsafe.AddByteOffset(ref b, offset) << 24) +
                    (ulong)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) +
                    (ulong)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) +
                    (ulong)Unsafe.AddByteOffset(ref b, offset + 3);
            }
        }
    }

    internal sealed class UInt64UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt64UInt64();

        UInt64UInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (UInt64)Unsafe.AddByteOffset(ref position, offset) << 56 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 1) << 48 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 2) << 40 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 3) << 32 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 4) << 24 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 5) << 16 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 6) << 8 |
                (UInt64)Unsafe.AddByteOffset(ref position, offset + 7));
            reader.AdvanceWithinSpan(9);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static UInt64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(9);
            reader.Advance(9);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (UInt64)Unsafe.AddByteOffset(ref b, offset) << 56 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 1) << 48 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 2) << 40 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 3) << 32 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 4) << 24 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 5) << 16 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 6) << 8 |
                    (UInt64)Unsafe.AddByteOffset(ref b, offset + 7);
            }
        }
    }

    internal sealed class InvalidUInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new InvalidUInt64();

        InvalidUInt64() { }

        public UInt64 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}