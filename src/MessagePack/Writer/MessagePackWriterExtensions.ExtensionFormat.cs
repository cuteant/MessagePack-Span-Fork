namespace MessagePack
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using MessagePack.Internal;

    public static partial class MessagePackWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormatHeader(this ref MessagePackWriter writer, sbyte typeCode, int dataLength, ref int idx)
        {
            Debug.Assert(dataLength >= 0);

            uint nLen = (uint)dataLength;
            ref byte b = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            switch (nLen)
            {
                case 1u:
                    writer.Ensure(idx, 3);
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt1;
                    Unsafe.AddByteOffset(ref b, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 2u:
                    writer.Ensure(idx, 4);
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt2;
                    Unsafe.AddByteOffset(ref b, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 4u:
                    writer.Ensure(idx, 6);
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt4;
                    Unsafe.AddByteOffset(ref b, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 8u:
                    writer.Ensure(idx, 10);
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt8;
                    Unsafe.AddByteOffset(ref b, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                case 16u:
                    writer.Ensure(idx, 18);
                    Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.FixExt16;
                    Unsafe.AddByteOffset(ref b, offset + 1) = unchecked((byte)typeCode);
                    idx += 2; break;
                default:
                    writer.Ensure(idx, dataLength + 6);
                    if (nLen <= ByteMaxValue)
                    {
                        Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Ext8;
                        Unsafe.AddByteOffset(ref b, offset + 1) = (byte)dataLength;
                        Unsafe.AddByteOffset(ref b, offset + 2) = (byte)typeCode;
                        idx += 3;
                    }
                    else if (nLen <= UShortMaxValue)
                    {
                        Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Ext16;
                        unchecked
                        {
                            Unsafe.AddByteOffset(ref b, offset + 1) = (byte)((ushort)dataLength >> 8);
                            Unsafe.AddByteOffset(ref b, offset + 2) = (byte)dataLength;
                            Unsafe.AddByteOffset(ref b, offset + 3) = (byte)typeCode;
                        }

                        idx += 4;
                    }
                    else
                    {
                        Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Ext32;
                        unchecked
                        {
                            Unsafe.AddByteOffset(ref b, offset + 1) = (byte)(nLen >> 24);
                            Unsafe.AddByteOffset(ref b, offset + 2) = (byte)(nLen >> 16);
                            Unsafe.AddByteOffset(ref b, offset + 3) = (byte)(nLen >> 8);
                            Unsafe.AddByteOffset(ref b, offset + 4) = (byte)nLen;
                            Unsafe.AddByteOffset(ref b, offset + 5) = (byte)typeCode;
                        }
                        idx += 6;
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
            ref byte b = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            uint nLen = (uint)dataLength;
            unchecked
            {
                Unsafe.AddByteOffset(ref b, offset) = MessagePackCode.Ext32;
                Unsafe.AddByteOffset(ref b, offset + 1) = (byte)(nLen >> 24);
                Unsafe.AddByteOffset(ref b, offset + 2) = (byte)(nLen >> 16);
                Unsafe.AddByteOffset(ref b, offset + 3) = (byte)(nLen >> 8);
                Unsafe.AddByteOffset(ref b, offset + 4) = (byte)nLen;
                Unsafe.AddByteOffset(ref b, offset + 5) = (byte)typeCode;
            }
            idx += 6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteExtensionFormat(this ref MessagePackWriter writer, sbyte typeCode, byte[] data, ref int idx)
        {
            var length = data.Length;
            uint nLen = (uint)length;

            ref byte self = ref writer.PinnableAddress;
            ref byte src = ref data[0];
            IntPtr offset = (IntPtr)idx;
            IntPtr srcOffset = (IntPtr)0;

            switch (nLen)
            {
                case 0u: return;
                case 1u:
                    writer.Ensure(idx, 3);
                    Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.FixExt1;
                    Unsafe.AddByteOffset(ref self, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref self, offset + 2) = src;
                    idx += 3; return;
                case 2u:
                    writer.Ensure(idx, 4);
                    Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.FixExt2;
                    Unsafe.AddByteOffset(ref self, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref self, offset + 2) = src;
                    Unsafe.AddByteOffset(ref self, offset + 3) = Unsafe.AddByteOffset(ref src, srcOffset + 1);
                    idx += 4; return;
                case 4u:
                    writer.Ensure(idx, 6);
                    Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.FixExt4;
                    Unsafe.AddByteOffset(ref self, offset + 1) = unchecked((byte)typeCode);
                    Unsafe.AddByteOffset(ref self, offset + 2) = src;
                    Unsafe.AddByteOffset(ref self, offset + 3) = Unsafe.AddByteOffset(ref src, srcOffset + 1);
                    Unsafe.AddByteOffset(ref self, offset + 4) = Unsafe.AddByteOffset(ref src, srcOffset + 2);
                    Unsafe.AddByteOffset(ref self, offset + 5) = Unsafe.AddByteOffset(ref src, srcOffset + 3);
                    idx += 6; return;
                case 8u:
                    writer.Ensure(idx, 10);
                    Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.FixExt8;
                    Unsafe.AddByteOffset(ref self, offset + 1) = unchecked((byte)typeCode);
                    ref byte dest = ref Unsafe.AddByteOffset(ref self, offset + 2);
                    if (UnsafeMemory.Is64BitProcess)
                    {
                        Unsafe.As<byte, long>(ref dest) = Unsafe.As<byte, long>(ref src);
                    }
                    else
                    {
                        Unsafe.As<byte, int>(ref dest) = Unsafe.As<byte, int>(ref src);
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest, 4)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref src, 4));
                    }
                    idx += 10; return;
                case 16u:
                    writer.Ensure(idx, 18);
                    Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.FixExt16;
                    Unsafe.AddByteOffset(ref self, offset + 1) = unchecked((byte)typeCode);
                    ref byte dest1 = ref Unsafe.AddByteOffset(ref self, offset + 2);
                    if (UnsafeMemory.Is64BitProcess)
                    {
                        Unsafe.As<byte, long>(ref dest1) = Unsafe.As<byte, long>(ref src);
                        Unsafe.As<byte, long>(ref Unsafe.Add(ref dest1, 8)) = Unsafe.As<byte, long>(ref Unsafe.Add(ref src, 8)); // [0,16]
                    }
                    else
                    {
                        Unsafe.As<byte, int>(ref dest1) = Unsafe.As<byte, int>(ref src);
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 4)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref src, 4));
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 8)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref src, 8));
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref dest1, 12)) = Unsafe.As<byte, int>(ref Unsafe.Add(ref src, 12)); // [0,16]
                    }
                    idx += 18; return;
                default:
                    writer.Ensure(idx, length + 6);
                    unchecked
                    {
                        if (nLen <= ByteMaxValue)
                        {
                            Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.Ext8;
                            Unsafe.AddByteOffset(ref self, offset + 1) = (byte)length;
                            Unsafe.AddByteOffset(ref self, offset + 2) = (byte)typeCode;
                            idx += 3;
                        }
                        else if (nLen <= UShortMaxValue)
                        {
                            Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.Ext16;
                            Unsafe.AddByteOffset(ref self, offset + 1) = (byte)((ushort)length >> 8);
                            Unsafe.AddByteOffset(ref self, offset + 2) = (byte)length;
                            Unsafe.AddByteOffset(ref self, offset + 3) = (byte)typeCode;
                            idx += 4;
                        }
                        else
                        {
                            Unsafe.AddByteOffset(ref self, offset) = MessagePackCode.Ext32;
                            Unsafe.AddByteOffset(ref self, offset + 1) = (byte)(nLen >> 24);
                            Unsafe.AddByteOffset(ref self, offset + 2) = (byte)(nLen >> 16);
                            Unsafe.AddByteOffset(ref self, offset + 3) = (byte)(nLen >> 8);
                            Unsafe.AddByteOffset(ref self, offset + 4) = (byte)nLen;
                            Unsafe.AddByteOffset(ref self, offset + 5) = (byte)typeCode;
                            idx += 6;
                        }
                    }
                    writer.WriteRawBytes(data, ref idx);
                    return;
            }
        }
    }
}
