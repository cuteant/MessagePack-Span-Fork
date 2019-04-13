namespace MessagePack.Formatters
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ExtBinaryGuidFormatter : IMessagePackFormatter<Guid>
    {
        /// <summary>Unsafe binary Guid formatter. this is only allows on LittleEndian environment.</summary>
        public static readonly IMessagePackFormatter<Guid> Instance = new ExtBinaryGuidFormatter();

        ExtBinaryGuidFormatter() { }

        public unsafe void Serialize(ref MessagePackWriter writer, ref int idx, Guid value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, 18);

            ref byte destination = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;

            Unsafe.AddByteOffset(ref destination, offset) = MessagePackCode.FixExt16;
            Unsafe.AddByteOffset(ref destination, offset + 1) = unchecked((byte)ReservedMessagePackExtensionTypeCode.Guid);

            var guid = new ExtGuidBits(value);
            if (BitConverter.IsLittleEndian)
            {
                Unsafe.AddByteOffset(ref destination, offset + 2) = guid.Byte0;
                Unsafe.AddByteOffset(ref destination, offset + 3) = guid.Byte1;
                Unsafe.AddByteOffset(ref destination, offset + 4) = guid.Byte2;
                Unsafe.AddByteOffset(ref destination, offset + 5) = guid.Byte3;
                Unsafe.AddByteOffset(ref destination, offset + 6) = guid.Byte4;
                Unsafe.AddByteOffset(ref destination, offset + 7) = guid.Byte5;
                Unsafe.AddByteOffset(ref destination, offset + 8) = guid.Byte6;
                Unsafe.AddByteOffset(ref destination, offset + 9) = guid.Byte7;
            }
            else
            {
                Unsafe.AddByteOffset(ref destination, offset + 2) = guid.Byte3;
                Unsafe.AddByteOffset(ref destination, offset + 3) = guid.Byte2;
                Unsafe.AddByteOffset(ref destination, offset + 4) = guid.Byte1;
                Unsafe.AddByteOffset(ref destination, offset + 5) = guid.Byte0;
                Unsafe.AddByteOffset(ref destination, offset + 6) = guid.Byte5;
                Unsafe.AddByteOffset(ref destination, offset + 7) = guid.Byte4;
                Unsafe.AddByteOffset(ref destination, offset + 8) = guid.Byte7;
                Unsafe.AddByteOffset(ref destination, offset + 9) = guid.Byte6;
            }
            Unsafe.AddByteOffset(ref destination, offset + 10) = guid.Byte8;
            Unsafe.AddByteOffset(ref destination, offset + 11) = guid.Byte9;
            Unsafe.AddByteOffset(ref destination, offset + 12) = guid.Byte10;
            Unsafe.AddByteOffset(ref destination, offset + 13) = guid.Byte11;
            Unsafe.AddByteOffset(ref destination, offset + 14) = guid.Byte12;
            Unsafe.AddByteOffset(ref destination, offset + 15) = guid.Byte13;
            Unsafe.AddByteOffset(ref destination, offset + 16) = guid.Byte14;
            Unsafe.AddByteOffset(ref destination, offset + 17) = guid.Byte15;

            idx += 18;
        }

        public unsafe Guid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var result = reader.ReadExtensionFormat();
            var typeCode = result.TypeCode;
            if (typeCode != ReservedMessagePackExtensionTypeCode.Guid)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }
#if NETCOREAPP
            return new Guid(result.Data);
#else
            return new ExtGuidBits(ref MemoryMarshal.GetReference(result.Data)).Value;
#endif
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ExtGuidBits
    {
        [FieldOffset(0)]
        public readonly Guid Value;

        [FieldOffset(0)]
        public readonly byte Byte0;
        [FieldOffset(1)]
        public readonly byte Byte1;
        [FieldOffset(2)]
        public readonly byte Byte2;
        [FieldOffset(3)]
        public readonly byte Byte3;
        [FieldOffset(4)]
        public readonly byte Byte4;
        [FieldOffset(5)]
        public readonly byte Byte5;
        [FieldOffset(6)]
        public readonly byte Byte6;
        [FieldOffset(7)]
        public readonly byte Byte7;
        [FieldOffset(8)]
        public readonly byte Byte8;
        [FieldOffset(9)]
        public readonly byte Byte9;
        [FieldOffset(10)]
        public readonly byte Byte10;
        [FieldOffset(11)]
        public readonly byte Byte11;
        [FieldOffset(12)]
        public readonly byte Byte12;
        [FieldOffset(13)]
        public readonly byte Byte13;
        [FieldOffset(14)]
        public readonly byte Byte14;
        [FieldOffset(15)]
        public readonly byte Byte15;

        public ExtGuidBits(Guid value)
        {
            this = default(ExtGuidBits);
            this.Value = value;
        }

        public ExtGuidBits(ref byte littleEndianBytes)
        {
            this = default(ExtGuidBits);

            IntPtr offset = (IntPtr)0;
            if (BitConverter.IsLittleEndian)
            {
                this.Byte0 = Unsafe.AddByteOffset(ref littleEndianBytes, offset);
                this.Byte1 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 1);
                this.Byte2 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 2);
                this.Byte3 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 3);
                this.Byte4 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 4);
                this.Byte5 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 5);
                this.Byte6 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 6);
                this.Byte7 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 7);
            }
            else
            {
                this.Byte0 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 3);
                this.Byte1 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 2);
                this.Byte2 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 1);
                this.Byte3 = Unsafe.AddByteOffset(ref littleEndianBytes, offset);
                this.Byte4 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 5);
                this.Byte5 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 4);
                this.Byte6 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 7);
                this.Byte7 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 6);
            }
            this.Byte8 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 8);
            this.Byte9 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 9);
            this.Byte10 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 10);
            this.Byte11 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 11);
            this.Byte12 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 12);
            this.Byte13 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 13);
            this.Byte14 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 14);
            this.Byte15 = Unsafe.AddByteOffset(ref littleEndianBytes, offset + 15);
        }
    }

    public sealed class ExtBinaryDecimalFormatter : IMessagePackFormatter<Decimal>
    {
        /// <summary>Unsafe binary Decimal formatter. this is only allows on LittleEndian environment.</summary>
        public static readonly IMessagePackFormatter<Decimal> Instance = new ExtBinaryDecimalFormatter();

        ExtBinaryDecimalFormatter() { }

        public unsafe void Serialize(ref MessagePackWriter writer, ref int idx, Decimal value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, 18);

            ref byte destination = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;

            Unsafe.AddByteOffset(ref destination, offset) = MessagePackCode.FixExt16;
            Unsafe.AddByteOffset(ref destination, offset + 1) = unchecked((byte)ReservedMessagePackExtensionTypeCode.Decimal);

            var bits = decimal.GetBits(value);
            unchecked
            {
                // 此处与.NET Core(Fx) - LittleEndian 保持一致
                var lo = bits[0];
                Unsafe.AddByteOffset(ref destination, offset + 2) = (byte)lo;
                Unsafe.AddByteOffset(ref destination, offset + 3) = (byte)(lo >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 4) = (byte)(lo >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 5) = (byte)(lo >> 24);

                var mid = bits[1];
                Unsafe.AddByteOffset(ref destination, offset + 6) = (byte)mid;
                Unsafe.AddByteOffset(ref destination, offset + 7) = (byte)(mid >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 8) = (byte)(mid >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 9) = (byte)(mid >> 24);

                var hi = bits[2];
                Unsafe.AddByteOffset(ref destination, offset + 10) = (byte)hi;
                Unsafe.AddByteOffset(ref destination, offset + 11) = (byte)(hi >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 12) = (byte)(hi >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 13) = (byte)(hi >> 24);

                var flags = bits[3];
                Unsafe.AddByteOffset(ref destination, offset + 14) = (byte)flags;
                Unsafe.AddByteOffset(ref destination, offset + 15) = (byte)(flags >> 8);
                Unsafe.AddByteOffset(ref destination, offset + 16) = (byte)(flags >> 16);
                Unsafe.AddByteOffset(ref destination, offset + 17) = (byte)(flags >> 24);
            }

            idx += 18;
        }

        public unsafe Decimal Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var result = reader.ReadExtensionFormat();
            var typeCode = result.TypeCode;
            if (typeCode != ReservedMessagePackExtensionTypeCode.Decimal)
            {
                ThrowHelper.ThrowInvalidOperationException_TypeCode(typeCode);
            }

            ref byte source = ref MemoryMarshal.GetReference(result.Data);
            IntPtr offset = (IntPtr)0;

            var lo = Unsafe.AddByteOffset(ref source, offset) |
                (Unsafe.AddByteOffset(ref source, offset + 1) << 8) |
                (Unsafe.AddByteOffset(ref source, offset + 2) << 16) |
                (Unsafe.AddByteOffset(ref source, offset + 3) << 24);

            var mid = Unsafe.AddByteOffset(ref source, offset + 4) |
                (Unsafe.AddByteOffset(ref source, offset + 5) << 8) |
                (Unsafe.AddByteOffset(ref source, offset + 6) << 16) |
                (Unsafe.AddByteOffset(ref source, offset + 7) << 24);

            var hi = Unsafe.AddByteOffset(ref source, offset + 8) |
                (Unsafe.AddByteOffset(ref source, offset + 9) << 8) |
                (Unsafe.AddByteOffset(ref source, offset + 10) << 16) |
                (Unsafe.AddByteOffset(ref source, offset + 11) << 24);

            var flags = Unsafe.AddByteOffset(ref source, offset + 12) |
                (Unsafe.AddByteOffset(ref source, offset + 13) << 8) |
                (Unsafe.AddByteOffset(ref source, offset + 14) << 16) |
                (Unsafe.AddByteOffset(ref source, offset + 15) << 24);

            return new decimal(new int[] { lo, mid, hi, flags });
        }
    }
}
