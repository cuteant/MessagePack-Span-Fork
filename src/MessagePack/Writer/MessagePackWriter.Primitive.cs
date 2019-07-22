namespace MessagePack
{
    using System;
    using System.Buffers.Binary;
    using System.Runtime.CompilerServices;

    partial struct MessagePackWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteNil(this ref MessagePackWriter writer, ref int idx)
        {
            writer.Ensure(idx, 1);
            Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = MessagePackCode.Nil;
            idx++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteBoolean(this ref MessagePackWriter writer, bool value, ref int idx)
        {
            writer.Ensure(idx, 1);
            Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = (value ? MessagePackCode.True : MessagePackCode.False);
            idx++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteSByte(this ref MessagePackWriter writer, sbyte value, ref int idx)
        {
            writer.Ensure(idx, 2);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            if (value < MessagePackRange.MinFixNegativeInt)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Int8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                idx += 2;
            }
            else
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                idx++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteSByteForceSByteBlock(this ref MessagePackWriter writer, sbyte value, ref int idx)
        {
            writer.Ensure(idx, 2);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Int8;
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
            idx += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteByte(this ref MessagePackWriter writer, byte value, ref int idx)
        {
            writer.Ensure(idx, 2);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            if (value <= MessagePackCode.MaxFixInt)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = value;
                idx++;
            }
            else
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = value;
                idx += 2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteByteForceByteBlock(this ref MessagePackWriter writer, byte value, ref int idx)
        {
            writer.Ensure(idx, 2);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = value;
            idx += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteChar(this ref MessagePackWriter writer, char value, ref int idx)
        {
            writer.WriteUInt16((ushort)value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteInt16(this ref MessagePackWriter writer, short value, ref int idx)
        {
            writer.Ensure(idx, 3);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            uint nValue = (uint)value;
            if (nValue < IntMaxValue)
            {
                // positive int(use uint)
                if (nValue <= MessagePackRange.MaxFixPositiveInt)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (nValue <= ByteMaxValue)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.UInt16, (ushort)value, ref idx);
                }
            }
            else
            {
                // negative int(use int)
                if (nValue >= MinFixNegativeInt)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (nValue >= SByteMinValue)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Int8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.Int16, (ushort)value, ref idx);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteInt16ForceInt16Block(this ref MessagePackWriter writer, short value, ref int idx)
        {
            writer.Ensure(idx, 3);
            WriteShort(ref writer.PinnableAddress, MessagePackCode.Int16, (ushort)value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteUInt16(this ref MessagePackWriter writer, ushort value, ref int idx)
        {
            writer.Ensure(idx, 3);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            uint nValue = value;
            if (nValue <= MaxFixPositiveInt)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                idx++;
            }
            else if (nValue <= ByteMaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                idx += 2;
            }
            else
            {
                WriteShort(ref pinnableAddr, MessagePackCode.UInt16, value, ref idx);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteUInt16ForceUInt16Block(this ref MessagePackWriter writer, ushort value, ref int idx)
        {
            writer.Ensure(idx, 3);
            WriteShort(ref writer.PinnableAddress, MessagePackCode.UInt16, value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public  void WriteInt32(this ref MessagePackWriter writer, int value, ref int idx)
        {
            writer.Ensure(idx, 5);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            uint nValue = (uint)value;
            if (nValue <= IntMaxValue)
            {
                // positive int(use uint)
                if (nValue <= MaxFixPositiveInt)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (nValue <= ByteMaxValue)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else if (nValue <= UShortMaxValue)
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.UInt16, unchecked((ushort)value), ref idx);
                }
                else
                {
                    WriteInt(ref pinnableAddr, MessagePackCode.UInt32, nValue, ref idx);
                }
            }
            else
            {
                // negative int(use int)
                if (nValue >= MinFixNegativeInt)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (nValue >= SByteMinValue)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Int8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else if (nValue >= ShortMinValue)
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.Int16, unchecked((ushort)value), ref idx);
                }
                else
                {
                    WriteInt(ref pinnableAddr, MessagePackCode.Int32, nValue, ref idx);
                }
            }
        }

        /// <summary>Acquire static message block(always 5 bytes).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32ForceInt32Block(this ref MessagePackWriter writer, int value, ref int idx)
        {
            writer.Ensure(idx, 5);
            WriteInt(ref writer.PinnableAddress, MessagePackCode.Int32, (uint)value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(this ref MessagePackWriter writer, uint value, ref int idx)
        {
            writer.Ensure(idx, 5);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            if (value <= MaxFixPositiveInt)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                idx++;
            }
            else if (value <= ByteMaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                idx += 2;
            }
            else if (value <= UShortMaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.UInt16, unchecked((ushort)value), ref idx);
            }
            else
            {
                WriteInt(ref pinnableAddr, MessagePackCode.UInt32, value, ref idx);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32ForceUInt32Block(this ref MessagePackWriter writer, uint value, ref int idx)
        {
            writer.Ensure(idx, 5);
            WriteInt(ref writer.PinnableAddress, MessagePackCode.UInt32, value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(this ref MessagePackWriter writer, long value, ref int idx)
        {
            writer.Ensure(idx, 9);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            ulong nValue = (ulong)value;
            if (value >= 0L)
            {
                // positive int(use uint)
                if (nValue <= MessagePackRange.MaxFixPositiveInt)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (nValue <= byte.MaxValue)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else if (nValue <= ushort.MaxValue)
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.UInt16, unchecked((ushort)value), ref idx);
                }
                else if (nValue <= uint.MaxValue)
                {
                    WriteInt(ref pinnableAddr, MessagePackCode.UInt32, unchecked((uint)value), ref idx);
                }
                else
                {
                    WriteLong(ref pinnableAddr, MessagePackCode.UInt64, nValue, ref idx);
                }
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                    idx++;
                }
                else if (sbyte.MinValue <= value)
                {
                    IntPtr offset = (IntPtr)idx;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Int8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                    idx += 2;
                }
                else if (short.MinValue <= value)
                {
                    WriteShort(ref pinnableAddr, MessagePackCode.Int16, unchecked((ushort)value), ref idx);
                }
                else if (int.MinValue <= value)
                {
                    WriteInt(ref pinnableAddr, MessagePackCode.Int32, unchecked((uint)value), ref idx);
                }
                else
                {
                    WriteLong(ref pinnableAddr, MessagePackCode.Int64, nValue, ref idx);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64ForceInt64Block(this ref MessagePackWriter writer, long value, ref int idx)
        {
            writer.Ensure(idx, 9);
            WriteLong(ref writer.PinnableAddress, MessagePackCode.Int64, (ulong)value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(this ref MessagePackWriter writer, ulong value, ref int idx)
        {
            writer.Ensure(idx, 9);
            ref byte pinnableAddr = ref writer.PinnableAddress;
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                Unsafe.AddByteOffset(ref pinnableAddr, (IntPtr)idx) = unchecked((byte)value);
                idx++;
            }
            else if (value <= byte.MaxValue)
            {
                IntPtr offset = (IntPtr)idx;
                Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.UInt8;
                Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)value);
                idx += 2;
            }
            else if (value <= ushort.MaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.UInt16, unchecked((ushort)value), ref idx);
            }
            else if (value <= uint.MaxValue)
            {
                WriteInt(ref pinnableAddr, MessagePackCode.UInt32, unchecked((uint)value), ref idx);
            }
            else
            {
                WriteLong(ref pinnableAddr, MessagePackCode.UInt64, value, ref idx);
            }
        }

        /// <summary>Unsafe. If value is guranteed 0 ~ MessagePackCode.MaxFixInt(127), can use this method.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePositiveFixedIntUnsafe(this ref MessagePackWriter writer, int value, ref int idx)
        {
            writer.Ensure(idx, 1);
            Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx) = (byte)value;
            idx++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64ForceUInt64Block(this ref MessagePackWriter writer, ulong value, ref int idx)
        {
            writer.Ensure(idx, 9);
            WriteLong(ref writer.PinnableAddress, MessagePackCode.UInt64, value, ref idx);
        }
    }
}
