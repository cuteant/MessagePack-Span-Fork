namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(this ref MessagePackWriter writer, double value, ref int idx)
        {
            writer.Ensure(idx, 9);

            ref byte b = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;

            Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Float64;

            var num = new Float64Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                Unsafe.AddByteOffset(ref b, offset + 1) = num.Byte7;
                Unsafe.AddByteOffset(ref b, offset + 2) = num.Byte6;
                Unsafe.AddByteOffset(ref b, offset + 3) = num.Byte5;
                Unsafe.AddByteOffset(ref b, offset + 4) = num.Byte4;
                Unsafe.AddByteOffset(ref b, offset + 5) = num.Byte3;
                Unsafe.AddByteOffset(ref b, offset + 6) = num.Byte2;
                Unsafe.AddByteOffset(ref b, offset + 7) = num.Byte1;
                Unsafe.AddByteOffset(ref b, offset + 8) = num.Byte0;
            }
            else
            {
                Unsafe.AddByteOffset(ref b, offset + 1) = num.Byte0;
                Unsafe.AddByteOffset(ref b, offset + 2) = num.Byte1;
                Unsafe.AddByteOffset(ref b, offset + 3) = num.Byte2;
                Unsafe.AddByteOffset(ref b, offset + 4) = num.Byte3;
                Unsafe.AddByteOffset(ref b, offset + 5) = num.Byte4;
                Unsafe.AddByteOffset(ref b, offset + 6) = num.Byte5;
                Unsafe.AddByteOffset(ref b, offset + 7) = num.Byte6;
                Unsafe.AddByteOffset(ref b, offset + 8) = num.Byte7;
            }

            idx += 9;
        }
    }
}
