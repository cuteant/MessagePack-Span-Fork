namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return int64Decoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IInt64Decoder
    {
        Int64 Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixNegativeInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new FixNegativeInt64();

        FixNegativeInt64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)position);
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class FixInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new FixInt64();

        FixInt64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class UInt8Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt8Int64();

        UInt8Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }
    internal sealed class UInt16Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt16Int64();

        UInt16Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = (Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1);
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            return (Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1);
        }
    }

    internal sealed class UInt32Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt32Int64();

        UInt32Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (uint)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                ((uint)Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                ((uint)Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref position, offset + 3));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            return unchecked(
                (uint)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                ((uint)Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                ((uint)Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                (uint)Unsafe.AddByteOffset(ref b, offset + 3));
        }
    }

    internal sealed class UInt64Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt64Int64();

        UInt64Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = checked(
                (Int64)Unsafe.AddByteOffset(ref position, offset) << 56 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 1) << 48 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 2) << 40 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 3) << 32 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 4) << 24 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 5) << 16 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 6) << 8 |
                (Int64)Unsafe.AddByteOffset(ref position, offset + 7));
            reader.AdvanceWithinSpan(9);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(9);
            reader.Advance(9);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            checked
            {
                return
                    (Int64)Unsafe.AddByteOffset(ref b, offset) << 56 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 1) << 48 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 2) << 40 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 3) << 32 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 4) << 24 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 5) << 16 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 6) << 8 |
                    (Int64)Unsafe.AddByteOffset(ref b, offset + 7);
            }
        }
    }


    internal sealed class Int8Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int8Int64();

        Int8Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var value = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class Int16Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int16Int64();

        Int16Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((short)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
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

    internal sealed class Int32Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int32Int64();

        Int32Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (long)(Unsafe.AddByteOffset(ref position, offset) << 24) +
                (long)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) +
                (long)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) +
                (long)Unsafe.AddByteOffset(ref position, offset + 3));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (long)(Unsafe.AddByteOffset(ref b, offset) << 24) +
                    (long)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) +
                    (long)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) +
                    (long)Unsafe.AddByteOffset(ref b, offset + 3);
            }
        }
    }

    internal sealed class Int64Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int64Int64();

        Int64Int64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Int64 ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked(
                (long)Unsafe.AddByteOffset(ref position, offset) << 56 |
                (long)Unsafe.AddByteOffset(ref position, offset + 1) << 48 |
                (long)Unsafe.AddByteOffset(ref position, offset + 2) << 40 |
                (long)Unsafe.AddByteOffset(ref position, offset + 3) << 32 |
                (long)Unsafe.AddByteOffset(ref position, offset + 4) << 24 |
                (long)Unsafe.AddByteOffset(ref position, offset + 5) << 16 |
                (long)Unsafe.AddByteOffset(ref position, offset + 6) << 8 |
                (long)Unsafe.AddByteOffset(ref position, offset + 7));
            reader.AdvanceWithinSpan(9);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Int64 ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(9);
            reader.Advance(9);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return
                    (long)Unsafe.AddByteOffset(ref b, offset) << 56 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 1) << 48 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 2) << 40 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 3) << 32 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 4) << 24 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 5) << 16 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 6) << 8 |
                    (long)Unsafe.AddByteOffset(ref b, offset + 7);
            }
        }
    }

    internal sealed class InvalidInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new InvalidInt64();

        InvalidInt64() { }

        public Int64 Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}