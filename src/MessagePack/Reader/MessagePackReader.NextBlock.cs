namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        public void ReadNext()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            readNextDecoders[position].Read(ref this, ref position);
        }

        public void ReadNextBlock()
        {
            switch (MessagePackCode.ToMessagePackType(_currentSpan[_currentSpanIndex]))
            {
                case MessagePackType.Unknown:
                case MessagePackType.Integer:
                case MessagePackType.Nil:
                case MessagePackType.Boolean:
                case MessagePackType.Float:
                case MessagePackType.String:
                case MessagePackType.Binary:
                case MessagePackType.Extension:
                default:
                    ReadNext(); return;
                case MessagePackType.Array:
                    {
                        var header = ReadArrayHeader();
                        for (int i = 0; i < header; i++)
                        {
                            ReadNextBlock();
                        }
                        return;
                    }
                case MessagePackType.Map:
                    {
                        var header = ReadMapHeader();
                        for (int i = 0; i < header; i++)
                        {
                            ReadNextBlock(); // read key block
                            ReadNextBlock(); // read value block
                        }
                        return;
                    }
            }
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IReadNextDecoder
    {
        void Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class ReadNext1 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext1();
        ReadNext1() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(1); }
    }

    internal sealed class ReadNext2 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext2();
        ReadNext2() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(2); }

    }
    internal sealed class ReadNext3 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext3();
        ReadNext3() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(3); }
    }
    internal sealed class ReadNext4 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext4();
        ReadNext4() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(4); }
    }
    internal sealed class ReadNext5 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext5();
        ReadNext5() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(5); }
    }
    internal sealed class ReadNext6 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext6();
        ReadNext6() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(6); }
    }

    internal sealed class ReadNext9 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext9();
        ReadNext9() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(9); }
    }
    internal sealed class ReadNext10 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext10();
        ReadNext10() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(10); }
    }
    internal sealed class ReadNext18 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext18();
        ReadNext18() { }

        public void Read(ref MessagePackReader reader, ref byte position) { reader.Advance(18); }
    }

    internal sealed class ReadNextMap : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextMap();
        ReadNextMap() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            var length = reader.ReadMapHeader();
            for (int i = 0; i < length; i++)
            {
                reader.ReadNext(); // key
                reader.ReadNext(); // value
            }
        }
    }

    internal sealed class ReadNextArray : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextArray();
        ReadNextArray() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            var length = reader.ReadArrayHeader();
            for (int i = 0; i < length; i++)
            {
                reader.ReadNext();
            }
        }
    }

    internal sealed class ReadNextFixStr : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextFixStr();
        ReadNextFixStr() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            var length = position & 0x1F;
            reader.Advance(length + 1);
        }
    }

    internal sealed class ReadNextStr8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr8();
        ReadNextStr8() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(length + 2);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var length = reader.GetRawByte(1);
            reader.Advance(length + 2);
        }
    }

    internal sealed class ReadNextStr16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr16();
        ReadNextStr16() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                (Unsafe.AddByteOffset(ref position, offset) << 8) |
                Unsafe.AddByteOffset(ref position, offset + 1));

            reader.AdvanceWithinSpan(length + 3);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (Unsafe.AddByteOffset(ref b, offset) << 8) |
                    Unsafe.AddByteOffset(ref b, offset + 1);

                reader.Advance(length + 3);
            }
        }
    }

    internal sealed class ReadNextStr32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr32();
        ReadNextStr32() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked((int)(
                (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref position, offset + 3)));

            reader.AdvanceWithinSpan(length + 5);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (int)(
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3));

                reader.Advance(length + 5);
            }
        }
    }

    internal sealed class ReadNextBin8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin8();
        ReadNextBin8() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(length + 2);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(2);
            var length = lenSpan[1];
            reader.Advance(length + 2);
        }
    }

    internal sealed class ReadNextBin16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin16();
        ReadNextBin16() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                (Unsafe.AddByteOffset(ref position, offset) << 8) |
                Unsafe.AddByteOffset(ref position, offset + 1));

            reader.AdvanceWithinSpan(length + 3);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (Unsafe.AddByteOffset(ref b, offset) << 8) |
                    Unsafe.AddByteOffset(ref b, offset + 1);

                reader.Advance(length + 3);
            }
        }
    }

    internal sealed class ReadNextBin32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin32();
        ReadNextBin32() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                (Unsafe.AddByteOffset(ref position, offset) << 24) |
                (Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                Unsafe.AddByteOffset(ref position, offset + 3));

            reader.AdvanceWithinSpan(length + 5);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    Unsafe.AddByteOffset(ref b, offset + 3);

                reader.Advance(length + 5);
            }
        }
    }

    internal sealed class ReadNextExt8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt8();
        ReadNextExt8() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var length = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(length + 3);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(3);
            var length = lenSpan[1];
            reader.Advance(length + 3);
        }
    }

    internal sealed class ReadNextExt16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt16();
        ReadNextExt16() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked(
                (UInt16)(Unsafe.AddByteOffset(ref position, offset) << 8) |
                (UInt16)Unsafe.AddByteOffset(ref position, offset + 1));

            reader.AdvanceWithinSpan(length + 4);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(4);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length =
                    (UInt16)(Unsafe.AddByteOffset(ref b, offset) << 8) |
                    (UInt16)Unsafe.AddByteOffset(ref b, offset + 1);

                reader.Advance(length + 4);
            }
        }
    }

    internal sealed class ReadNextExt32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt32();
        ReadNextExt32() { }

        public void Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                ReadFast(ref reader, ref position);
            }
            else
            {
                ReadMultisegment(ref reader);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var length = unchecked((int)(
                (UInt32)(Unsafe.AddByteOffset(ref position, offset) << 24) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (UInt32)(Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                (UInt32)Unsafe.AddByteOffset(ref position, offset + 3)));

            reader.AdvanceWithinSpan(length + 6);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ReadMultisegment(ref MessagePackReader reader)
        {
            var lenSpan = reader.Peek(6);
            ref byte b = ref MemoryMarshal.GetReference(lenSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                var length = (int)(
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (UInt32)(Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    (UInt32)Unsafe.AddByteOffset(ref b, offset + 3));

                reader.Advance(length + 6);
            }
        }
    }
}