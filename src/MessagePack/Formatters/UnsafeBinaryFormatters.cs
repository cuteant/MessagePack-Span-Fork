namespace MessagePack.Formatters
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class BinaryGuidFormatter : IMessagePackFormatter<Guid>
    {
        /// <summary>Unsafe binary Guid formatter. this is only allows on LittleEndian environment.</summary>
        public static readonly IMessagePackFormatter<Guid> Instance = new BinaryGuidFormatter();

        BinaryGuidFormatter() { }

        // Guid's underlying _a,...,_k field is sequential and same layuout as .NET Framework and Mono(Unity).
        // But target machines must be same endian so restrict only for little endian.

        public unsafe void Serialize(ref MessagePackWriter writer, ref int idx, Guid value, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) ThrowHelper.ThrowException_Guid_Little_Endian();

            writer.Ensure(idx, 18);

            ref byte buffer = ref Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx);
            fixed (byte* dst = &buffer)
            {
                var src = &value;

                dst[0] = MessagePackCode.Bin8;
                dst[1] = 16;

                *(Guid*)(dst + 2) = *src;
            }
            idx += 18;
        }

        public unsafe Guid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) ThrowHelper.ThrowException_Guid_Little_Endian();

            //if (!(offset + 18 <= bytes.Length))
            //{
            //    ThrowHelper.ThrowArgumentOutOfRangeException();
            //}

            var valueSpan = reader.Peek(18);
            reader.Advance(18);
            fixed (byte* src = &MemoryMarshal.GetReference(valueSpan))
            {
                if (src[0] != MessagePackCode.Bin8)
                {
                    ThrowHelper.ThrowInvalidOperationException_Code(valueSpan[0]);
                }
                if (src[1] != 16)
                {
                    ThrowHelper.ThrowInvalidOperationException_Guid_Size();
                }

                return *(Guid*)(src + 2);
            }
        }
    }

    public sealed class BinaryDecimalFormatter : IMessagePackFormatter<Decimal>
    {
        /// <summary>Unsafe binary Decimal formatter. this is only allows on LittleEndian environment.</summary>
        public static readonly IMessagePackFormatter<Decimal> Instance = new BinaryDecimalFormatter();

        BinaryDecimalFormatter() { }

        // decimal underlying "flags, hi, lo, mid" fields are sequential and same layuout with .NET Framework and Mono(Unity)
        // But target machines must be same endian so restrict only for little endian.

        public unsafe void Serialize(ref MessagePackWriter writer, ref int idx, Decimal value, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) ThrowHelper.ThrowException_Decimal_Little_Endian();

            writer.Ensure(idx, 18);

            ref byte buffer = ref Unsafe.AddByteOffset(ref writer.PinnableAddress, (IntPtr)idx);
            fixed (byte* dst = &buffer)
            {
                var src = &value;

                dst[0] = MessagePackCode.Bin8;
                dst[1] = 16;

                *(Decimal*)(dst + 2) = *src;
            }
            idx += 18;
        }

        public unsafe Decimal Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (!BitConverter.IsLittleEndian) ThrowHelper.ThrowException_Decimal_Little_Endian();

            //if (!(offset + 18 <= bytes.Length))
            //{
            //    ThrowHelper.ThrowArgumentOutOfRangeException();
            //}

            var valueSpan = reader.Peek(18);
            reader.Advance(18);
            fixed (byte* src = &MemoryMarshal.GetReference(valueSpan))
            {
                if (src[0] != MessagePackCode.Bin8)
                {
                    ThrowHelper.ThrowInvalidOperationException_Code(src[0]);
                }
                if (src[1] != 16)
                {
                    ThrowHelper.ThrowInvalidOperationException_Guid_Size();
                }

                return *(Decimal*)(src + 2);
            }
        }
    }
}
