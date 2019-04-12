namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return sbyteDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;

    internal interface ISByteDecoder
    {
        sbyte Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixSByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new FixSByte();

        FixSByte() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)position);
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class Int8SByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new Int8SByte();

        Int8SByte() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = unchecked((sbyte)Unsafe.AddByteOffset(ref position, (IntPtr)1));
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public sbyte ReadMultisegment(ref MessagePackReader reader)
        {
            var value = unchecked((sbyte)reader.GetRawByte(1));
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class InvalidSByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new InvalidSByte();

        InvalidSByte() { }

        public sbyte Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}