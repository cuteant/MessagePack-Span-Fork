namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        /// <summary>Return map count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadMapHeader()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return checked((int)mapHeaderDecoders[position].Read(ref this, ref position));
        }

        /// <summary>Return map count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadMapHeaderRaw()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return mapHeaderDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IMapHeaderDecoder
    {
        uint Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixMapHeader : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new FixMapHeader();

        FixMapHeader() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            var value = (uint)(position & 0xF);
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class Map16Header : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new Map16Header();

        Map16Header() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((uint)((Unsafe.AddByteOffset(ref position, offset) << 8) | Unsafe.AddByteOffset(ref position, offset + 1)));
            reader.AdvanceWithinSpan(3);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static uint ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            reader.Advance(3);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (uint)((Unsafe.AddByteOffset(ref b, offset) << 8) | Unsafe.AddByteOffset(ref b, offset + 1));
            }
        }
    }

    internal sealed class Map32Header : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new Map32Header();

        Map32Header() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ReadFast(ref MessagePackReader reader, ref byte position)
        {
            IntPtr offset = (IntPtr)1;
            var value = unchecked((uint)(
                (Unsafe.AddByteOffset(ref position, offset) << 24) |
                (Unsafe.AddByteOffset(ref position, offset + 1) << 16) |
                (Unsafe.AddByteOffset(ref position, offset + 2) << 8) |
                Unsafe.AddByteOffset(ref position, offset + 3)));
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static uint ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);

            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;
            unchecked
            {
                return (uint)(
                    (Unsafe.AddByteOffset(ref b, offset) << 24) |
                    (Unsafe.AddByteOffset(ref b, offset + 1) << 16) |
                    (Unsafe.AddByteOffset(ref b, offset + 2) << 8) |
                    Unsafe.AddByteOffset(ref b, offset + 3));
            }
        }
    }

    internal sealed class InvalidMapHeader : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new InvalidMapHeader();

        InvalidMapHeader() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}