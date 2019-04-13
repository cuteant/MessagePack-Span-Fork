namespace MessagePack
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormatHeader(this ref MessagePackWriter writer, sbyte typeCode, int dataLength, ref int idx)
        {
            Debug.Assert(dataLength >= 0);

            uint nLen = (uint)dataLength;
            ref byte destinationSpace = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            switch (nLen)
            {
                case 1u:
                    writer.Ensure(idx, 3);
                    Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.FixExt1;
                    Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 2u:
                    writer.Ensure(idx, 4);
                    Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.FixExt2;
                    Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 4u:
                    writer.Ensure(idx, 6);
                    Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.FixExt4;
                    Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 8u:
                    writer.Ensure(idx, 10);
                    Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.FixExt8;
                    Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 16u:
                    writer.Ensure(idx, 18);
                    Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.FixExt16;
                    Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                default:
                    writer.Ensure(idx, dataLength + 6);
                    unchecked
                    {
                        if (nLen <= ByteMaxValue)
                        {
                            Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Ext8;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)dataLength;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)typeCode;
                            idx += 3;
                        }
                        else if (nLen <= UShortMaxValue)
                        {
                            Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Ext16;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)((ushort)dataLength >> 8);
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)dataLength;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)typeCode;
                            idx += 4;
                        }
                        else
                        {
                            Unsafe.AddByteOffset(ref destinationSpace, offset) = MessagePackCode.Ext32;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 1) = (byte)(nLen >> 24);
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 2) = (byte)(nLen >> 16);
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 3) = (byte)(nLen >> 8);
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 4) = (byte)nLen;
                            Unsafe.AddByteOffset(ref destinationSpace, offset + 5) = (byte)typeCode;
                            idx += 6;
                        }
                    }
                    break;
            }
        }

        /// <summary>Write extension format header, always use ext32 format(length is fixed, 6).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormatHeaderForceExt32Block(this ref MessagePackWriter writer, sbyte typeCode, int dataLength, ref int idx)
        {
            Debug.Assert(dataLength >= 0);

            writer.Ensure(idx, 6);
            MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref writer.PinnableAddress, idx, typeCode, dataLength);
            idx += 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormat(this ref MessagePackWriter writer, sbyte typeCode, byte[] data, ref int idx)
        {
            WriteExtensionFormat(ref writer, typeCode, ref data[0], data.Length, ref idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormat(this ref MessagePackWriter writer, sbyte typeCode, ReadOnlySpan<byte> data, ref int idx)
        {
            WriteExtensionFormat(ref writer, typeCode, ref MemoryMarshal.GetReference(data), data.Length, ref idx);
        }

        public static void WriteExtensionFormat(this ref MessagePackWriter writer, sbyte typeCode, ref byte source, int dataLength, ref int idx)
        {
            uint nLen = (uint)dataLength;

            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            IntPtr srcOffset = (IntPtr)0;

            switch (nLen)
            {
                case 0u: return;
                case 1u:
                    writer.Ensure(idx, 3);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt1;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = source;
                    idx += 3; return;
                case 2u:
                    writer.Ensure(idx, 4);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt2;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = source;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = Unsafe.AddByteOffset(ref source, srcOffset + 1);
                    idx += 4; return;
                case 4u:
                    writer.Ensure(idx, 6);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt4;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = source;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = Unsafe.AddByteOffset(ref source, srcOffset + 1);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 4) = Unsafe.AddByteOffset(ref source, srcOffset + 2);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 5) = Unsafe.AddByteOffset(ref source, srcOffset + 3);
                    idx += 6; return;
                case 8u:
                    writer.Ensure(idx, 10);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt8;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)typeCode);
                    ref byte dest = ref Unsafe.AddByteOffset(ref pinnableAddr, offset + 2);
                    if (UnsafeMemory.Is64BitProcess)
                    {
                        Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref source);
                    }
                    else
                    {
                        Unsafe.As<byte, int>(ref dest) = Unsafe.As<byte, int>(ref source);
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest, 4)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref source, 4));
                    }
                    idx += 10; return;
                case 16u:
                    writer.Ensure(idx, 18);
                    Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.FixExt16;
                    Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = unchecked((byte)typeCode);
                    ref byte dest1 = ref Unsafe.AddByteOffset(ref pinnableAddr, offset + 2);
                    if (UnsafeMemory.Is64BitProcess)
                    {
                        Unsafe.As<byte, long>(ref dest1) = Unsafe.As<byte, long>(ref source);
                        Unsafe.As<byte, long>(ref Unsafe.Add(ref dest1, 8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref source, 8)); // [0,16]
                    }
                    else
                    {
                        Unsafe.As<byte, int>(ref dest1) = Unsafe.As<byte, int>(ref source);
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 4)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref source, 4));
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 8)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref source, 8));
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 12)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref source, 12)); // [0,16]
                    }
                    idx += 18; return;
                default:
                    writer.Ensure(idx, dataLength + 6);
                    unchecked
                    {
                        if (nLen <= ByteMaxValue)
                        {
                            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Ext8;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = (byte)dataLength;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = (byte)typeCode;
                            idx += 3;
                        }
                        else if (nLen <= UShortMaxValue)
                        {
                            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Ext16;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = (byte)((ushort)dataLength >> 8);
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = (byte)dataLength;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = (byte)typeCode;
                            idx += 4;
                        }
                        else
                        {
                            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Ext32;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = (byte)(nLen >> 24);
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 2) = (byte)(nLen >> 16);
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 3) = (byte)(nLen >> 8);
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 4) = (byte)nLen;
                            Unsafe.AddByteOffset(ref pinnableAddr, offset + 5) = (byte)typeCode;
                            idx += 6;
                        }
                    }
                    UnsafeMemory.WriteRaw(ref pinnableAddr, ref source, dataLength, ref idx);
                    return;
            }
        }
    }
}
