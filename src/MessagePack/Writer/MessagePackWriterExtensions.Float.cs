namespace MessagePack
{
    using System;
    using System.Runtime.CompilerServices;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingle(this ref MessagePackWriter writer, float value, ref int idx)
        {
            writer.Ensure(idx, 5);

            ref byte b = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;

            Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Float32;

            var num = new Float32Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                Unsafe.AddByteOffset(ref b, offset + 1) = num.Byte3;
                Unsafe.AddByteOffset(ref b, offset + 2) = num.Byte2;
                Unsafe.AddByteOffset(ref b, offset + 3) = num.Byte1;
                Unsafe.AddByteOffset(ref b, offset + 4) = num.Byte0;
            }
            else
            {
                Unsafe.AddByteOffset(ref b, offset + 1) = num.Byte0;
                Unsafe.AddByteOffset(ref b, offset + 2) = num.Byte1;
                Unsafe.AddByteOffset(ref b, offset + 3) = num.Byte2;
                Unsafe.AddByteOffset(ref b, offset + 4) = num.Byte3;
            }

            idx += 5;
        }
    }
}
