namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBytes(this ref MessagePackWriter writer, byte[] value, ref int idx)
        {
            if (null == value) { WriteNil(ref writer, ref idx); return; }
            InternalWriteBytes(ref writer, value, 0, value.Length, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBytes(this ref MessagePackWriter writer, byte[] value, int offset, int count, ref int idx)
        {
            if (null == value) { WriteNil(ref writer, ref idx); return; }
            InternalWriteBytes(ref writer, value, offset, count, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalWriteBytes(ref MessagePackWriter writer, byte[] value, int offset, int count, ref int idx)
        {
            var nCount = (uint)count;
            if (0u >= nCount) { WriteEmptyBytes(ref writer, ref idx); return; }

            writer.Ensure(idx, count + 5);
            ref byte pinnableAddr = ref writer.PinnableAddress;

            if (nCount <= ByteMaxValue)
            {
                WriteBin8Header(ref pinnableAddr, count, ref idx);
            }
            else if (nCount <= UShortMaxValue)
            {
                WriteShort(ref pinnableAddr, MessagePackCode.Bin16, unchecked((ushort)count), ref idx);
            }
            else
            {
                WriteInt(ref pinnableAddr, MessagePackCode.Bin32, nCount, ref idx);
            }

            UnsafeMemory.WriteRaw(ref pinnableAddr, ref value[offset], count, ref idx);
            //MessagePackBinary.CopyMemory(value, offset, writer._borrowedBuffer, idx, count);
            //idx += count;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void WriteEmptyBytes(ref MessagePackWriter writer, ref int idx)
        {
            writer.Ensure(idx, 2);
            WriteBin8Header(ref writer.PinnableAddress, 0, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteBin8Header(ref byte destination, int count, ref int idx)
        {
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref destination, offset) = MessagePackCode.Bin8;
            Unsafe.AddByteOffset(ref destination, offset + 1) = unchecked((byte)count);
            idx += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawBytes(this ref MessagePackWriter writer, byte[] value, ref int idx)
        {
            if (null == value) { return; }
            UnsafeMemory.WriteRaw(ref writer, value, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRawBytes(this ref MessagePackWriter writer, ReadOnlySpan<byte> value, ref int idx)
        {
            var count = value.Length;
            if (0u >= (uint)count) { return; }

            writer.Ensure(idx, count);
            UnsafeMemory.WriteRaw(ref MemoryMarshal.GetReference(value), ref Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx), count, ref idx);
        }
    }
}
