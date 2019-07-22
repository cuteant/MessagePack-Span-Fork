namespace MessagePack
{
    using System;
    using System.Buffers.Binary;
    using System.Runtime.CompilerServices;

    internal static class MessagePackWriterHelper
    {
        public const uint IntMaxValue = int.MaxValue;
        public const uint MaxFixPositiveInt = MessagePackRange.MaxFixPositiveInt;
        public const uint MaxFixMapCount = MessagePackRange.MaxFixMapCount;
        public const uint MaxFixArrayCount = MessagePackRange.MaxFixArrayCount;
        public const uint ByteMaxValue = byte.MaxValue;
        public const uint UShortMaxValue = ushort.MaxValue;
        public const uint MinFixNegativeInt = unchecked((uint)MessagePackRange.MinFixNegativeInt);
        public const uint SByteMinValue = unchecked((uint)sbyte.MinValue);
        public const uint ShortMinValue = unchecked((uint)short.MinValue);
        public const uint IntMinValue = unchecked((uint)int.MinValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteShort(ref byte destination, byte packCode, ushort value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            unchecked
            {
                Unsafe.AddByteOffset(ref destination, offset) = packCode;
                Unsafe.AddByteOffset(ref destination, offset + 1) = (byte)(value >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 2) = (byte)value;
            }
            idx += 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt(ref byte destination, byte packCode, uint value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            unchecked
            {
                Unsafe.AddByteOffset(ref destination, offset) = packCode;
                Unsafe.AddByteOffset(ref destination, offset + 1) = (byte)(value >> 24);
                Unsafe.AddByteOffset(ref destination, offset + 2) = (byte)(value >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 3) = (byte)(value >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 4) = (byte)value;
            }
            idx += 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLong(ref byte destination, byte packCode, ulong value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            unchecked
            {
                Unsafe.AddByteOffset(ref destination, offset) = packCode;
                Unsafe.AddByteOffset(ref destination, offset + 1) = (byte)(value >> 56);
                Unsafe.AddByteOffset(ref destination, offset + 2) = (byte)(value >> 48);
                Unsafe.AddByteOffset(ref destination, offset + 3) = (byte)(value >> 40);
                Unsafe.AddByteOffset(ref destination, offset + 4) = (byte)(value >> 32);
                Unsafe.AddByteOffset(ref destination, offset + 5) = (byte)(value >> 24);
                Unsafe.AddByteOffset(ref destination, offset + 6) = (byte)(value >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 7) = (byte)(value >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 8) = (byte)value;
            }
            idx += 9;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, short value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, ushort value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, int value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, uint value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, long value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 9;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNumber(ref byte destination, byte packCode, ulong value, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = packCode;

            if (BitConverter.IsLittleEndian) { value = BinaryPrimitives.ReverseEndianness(value); }
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, offset + 1), value);
            idx += 9;
        }
    }
}
