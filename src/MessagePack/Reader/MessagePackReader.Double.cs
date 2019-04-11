namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return doubleDecoders[position].Read(ref this, ref position);
        }
    }
}


namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal interface IDoubleDecoder
    {
        double Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixNegativeDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new FixNegativeDouble();

        FixNegativeDouble() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return FixSByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class FixDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new FixDouble();

        FixDouble() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return FixByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int8Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int8Double();

        Int8Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return Int8SByte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int16Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int16Double();

        Int16Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return Int16Int16.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int32Double();

        Int32Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return Int32Int32.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Int64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int64Double();

        Int64Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return Int64Int64.Instance.Read(ref reader, ref position);
        }
    }


    internal sealed class UInt8Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt8Double();

        UInt8Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt8Byte.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt16Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt16Double();

        UInt16Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt16UInt16.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt32Double();

        UInt32Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt32UInt32.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class UInt64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt64Double();

        UInt64Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            return UInt64UInt64.Instance.Read(ref reader, ref position);
        }
    }

    internal sealed class Float32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Float32Double();

        Float32Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Double ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = new Float32Bits(ref position).Value;
            reader.AdvanceWithinSpan(5);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Double ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(5);
            reader.Advance(5);
            return new Float32Bits(ref MemoryMarshal.GetReference(valueSpan)).Value;
        }
    }

    internal sealed class Float64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Float64Double();

        Float64Double() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Double ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = new Float64Bits(ref position).Value;
            reader.AdvanceWithinSpan(9);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Double ReadMultisegment(ref MessagePackReader reader)
        {
            var valueSpan = reader.Peek(9);
            reader.Advance(9);
            return new Float64Bits(ref MemoryMarshal.GetReference(valueSpan)).Value;
        }
    }

    internal sealed class InvalidDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new InvalidDouble();

        InvalidDouble() { }

        public Double Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}