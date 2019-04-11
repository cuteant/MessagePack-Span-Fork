namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return singleDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface ISingleDecoder
    {
        float Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixNegativeFloat : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new FixNegativeFloat();

        FixNegativeFloat() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return FixSByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class FixFloat : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new FixFloat();

        FixFloat() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return FixByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int8Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int8Single();

        Int8Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return Int8SByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int16Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int16Single();

        Int16Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return Int16Int16.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int32Single();

        Int32Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return Int32Int32.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int64Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int64Single();

        Int64Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return Int64Int64.Instance.Read(ref reader, ref position);
        }
    }


    internal sealed class UInt8Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt8Single();

        UInt8Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt8Byte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt16Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt16Single();

        UInt16Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt16UInt16.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt32Single();

        UInt32Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt32UInt32.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt64Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt64Single();

        UInt64Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt64UInt64.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Float32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Float32Single();

        Float32Single() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Single ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = new Float32Bits(ref position).Value;
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Single ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);
            return new Float32Bits(ref MemoryMarshal.GetReference(valueSpan)).Value;
        }
    }

    internal sealed class InvalidSingle : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new InvalidSingle();

        InvalidSingle() { }

        public Single Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}