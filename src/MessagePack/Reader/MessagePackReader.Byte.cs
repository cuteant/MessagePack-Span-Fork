namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public ref partial struct MessagePackReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetRawByte(int index = 0)
        {
            if ((uint)index > TooBigOrNegative)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
            }

            if ((uint)(_currentSpan.Length - _currentSpanIndex) > (uint)index)
            {
                return _currentSpan[_currentSpanIndex + index];
            }

            if (_sequenceWrapper.TryGetRawMultisegment(ref this, index, out byte value))
            {
                return value;
            }

            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index); return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            ref byte position = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_currentSpan), (IntPtr)_currentSpanIndex);
            return byteDecoders[position].Read(ref this, ref position);
        }
    }
}

namespace MessagePack.Decoders
{
    using System;
    using System.Runtime.CompilerServices;

    internal interface IByteDecoder
    {
        byte Read(ref MessagePackReader reader, ref byte position);
    }

    internal sealed class FixByte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new FixByte();

        FixByte() { }

        public byte Read(ref MessagePackReader reader, ref byte position)
        {
            var value = position;
            reader.Advance(1);
            return value;
        }
    }

    internal sealed class UInt8Byte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new UInt8Byte();

        UInt8Byte() { }

        public byte Read(ref MessagePackReader reader, ref byte position)
        {
            if (reader._isSingleSegment)
            {
                return ReadFast(ref reader, ref position);
            }
            return ReadMultisegment(ref reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ReadFast(ref MessagePackReader reader, ref byte position)
        {
            var value = Unsafe.AddByteOffset(ref position, (IntPtr)1);
            reader.AdvanceWithinSpan(2);
            return value;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static byte ReadMultisegment(ref MessagePackReader reader)
        {
            var value = reader.GetRawByte(1);
            reader.Advance(2);
            return value;
        }
    }

    internal sealed class InvalidByte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new InvalidByte();

        InvalidByte() { }

        public byte Read(ref MessagePackReader reader, ref byte position)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", position, MessagePackCode.ToFormatName(position)));
        }
    }
}