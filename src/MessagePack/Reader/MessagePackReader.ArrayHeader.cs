namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        /// <summary>Return array count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadArrayHeader()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return checked((int)arrayHeaderDecoders[position].Read(ref this, ref position));
        }

        /// <summary>Return array count.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadArrayHeaderRaw()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return arrayHeaderDecoders[position].Read(ref this, ref position);
        }
    }
}

namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IArrayHeaderDecoder
    {
        uint Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixArrayHeader : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new FixArrayHeader();

        FixArrayHeader() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            var value = (uint)(position & 0xF);
            reader.AdvanceWithinSpan(1);
            return value;
        }
    }

    internal sealed class Array16Header : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new Array16Header();

        Array16Header() { }

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

            unchecked
            {
                var value =
                    Unsafe.AddByteOffset(ref position, offset) << 8 |
                    Unsafe.AddByteOffset(ref position, offset + 1);
                reader.AdvanceWithinSpan(3);
                return (uint)value;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static uint ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(3);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;

            unchecked
            {
                var value =
                    Unsafe.AddByteOffset(ref b, offset) << 8 |
                    Unsafe.AddByteOffset(ref b, offset + 1);
                reader.Advance(3);
                return (uint)value;
            }
        }
    }

    internal sealed class Array32Header : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new Array32Header();

        Array32Header() { }

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

            unchecked
            {
                var value =
                    Unsafe.AddByteOffset(ref position, offset) << 24 |
                    Unsafe.AddByteOffset(ref position, offset + 1) << 16 |
                    Unsafe.AddByteOffset(ref position, offset + 2) << 8 |
                    Unsafe.AddByteOffset(ref position, offset + 3);
                reader.AdvanceWithinSpan(5);
                return (uint)value;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static uint ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            ref byte b = ref MemoryMarshal.GetReference(valueSpan);
            IntPtr offset = (IntPtr)1;

            unchecked
            {
                var value =
                    Unsafe.AddByteOffset(ref b, offset) << 24 |
                    Unsafe.AddByteOffset(ref b, offset + 1) << 16 |
                    Unsafe.AddByteOffset(ref b, offset + 2) << 8 |
                    Unsafe.AddByteOffset(ref b, offset + 3);
                reader.Advance(5);
                return (uint)value;
            }
        }
    }

    internal sealed class InvalidArrayHeader : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new InvalidArrayHeader();

        InvalidArrayHeader() { }

        public uint Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}
