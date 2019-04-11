using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MessagePack
{
    // safe accessor of Single/Double's underlying byte.
    // This code is borrowed from MsgPack-Cli https://github.com/msgpack/msgpack-cli

    [StructLayout(LayoutKind.Explicit)]
    internal struct Float32Bits
    {
        [FieldOffset(0)]
        public readonly float Value;

        [FieldOffset(0)]
        public readonly Byte Byte0;

        [FieldOffset(1)]
        public readonly Byte Byte1;

        [FieldOffset(2)]
        public readonly Byte Byte2;

        [FieldOffset(3)]
        public readonly Byte Byte3;

        public Float32Bits(float value)
        {
            this = default(Float32Bits);
            this.Value = value;
        }

        public Float32Bits(ref byte bigEndianBytes)
        {
            this = default(Float32Bits);

            IntPtr offset = (IntPtr)1;
            if (BitConverter.IsLittleEndian)
            {
                this.Byte0 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 3);
                this.Byte1 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 2);
                this.Byte2 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 1);
                this.Byte3 = Unsafe.AddByteOffset(ref bigEndianBytes, offset);
            }
            else
            {
                this.Byte0 = Unsafe.AddByteOffset(ref bigEndianBytes, offset);
                this.Byte1 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 1);
                this.Byte2 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 2);
                this.Byte3 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 3);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct Float64Bits
    {
        [FieldOffset(0)]
        public readonly double Value;

        [FieldOffset(0)]
        public readonly Byte Byte0;

        [FieldOffset(1)]
        public readonly Byte Byte1;

        [FieldOffset(2)]
        public readonly Byte Byte2;

        [FieldOffset(3)]
        public readonly Byte Byte3;

        [FieldOffset(4)]
        public readonly Byte Byte4;

        [FieldOffset(5)]
        public readonly Byte Byte5;

        [FieldOffset(6)]
        public readonly Byte Byte6;

        [FieldOffset(7)]
        public readonly Byte Byte7;

        public Float64Bits(double value)
        {
            this = default(Float64Bits);
            this.Value = value;
        }

        public Float64Bits(ref byte bigEndianBytes)
        {
            this = default(Float64Bits);

            IntPtr offset = (IntPtr)1;
            if (BitConverter.IsLittleEndian)
            {
                this.Byte0 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 7);
                this.Byte1 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 6);
                this.Byte2 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 5);
                this.Byte3 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 4);
                this.Byte4 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 3);
                this.Byte5 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 2);
                this.Byte6 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 1);
                this.Byte7 = Unsafe.AddByteOffset(ref bigEndianBytes, offset);
            }
            else
            {
                this.Byte0 = Unsafe.AddByteOffset(ref bigEndianBytes, offset);
                this.Byte1 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 1);
                this.Byte2 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 2);
                this.Byte3 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 3);
                this.Byte4 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 4);
                this.Byte5 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 5);
                this.Byte6 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 6);
                this.Byte7 = Unsafe.AddByteOffset(ref bigEndianBytes, offset + 7);
            }
        }
    }
}
