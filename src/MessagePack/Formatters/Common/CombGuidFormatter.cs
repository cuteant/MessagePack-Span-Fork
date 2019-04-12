#if DEPENDENT_ON_CUTEANT

namespace MessagePack.Formatters
{
    using System;
    using System.Runtime.CompilerServices;
    using CuteAnt;
    using MessagePack.Internal;

    public sealed class CombGuidFormatter : IMessagePackFormatter<CombGuid>
    {
        public static readonly IMessagePackFormatter<CombGuid> Instance = new CombGuidFormatter();

        CombGuidFormatter() { }

        const int c_totalSize = 18;
        const byte c_valueSize = 16;

        public void Serialize(ref MessagePackWriter writer, ref int idx, CombGuid value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, c_totalSize);

            var buffer = value.GetByteArray(CombGuidSequentialSegmentType.Guid);

            ref byte pinnableAddr = ref writer.PinnableAddress;
            IntPtr offset = (IntPtr)idx;
            Unsafe.AddByteOffset(ref pinnableAddr, offset) = MessagePackCode.Bin8;
            Unsafe.AddByteOffset(ref pinnableAddr, offset + 1) = c_valueSize;
            idx += 2;

            if (UnsafeMemory.Is64BitProcess)
            {
                UnsafeMemory64.WriteRaw16(ref pinnableAddr, ref buffer[0], ref idx);
            }
            else
            {
                UnsafeMemory32.WriteRaw16(ref pinnableAddr, ref buffer[0], ref idx);
            }
        }

        public CombGuid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var valueBytes = reader.ReadBytes();
            return new CombGuid(valueBytes, CombGuidSequentialSegmentType.Guid, true);
        }
    }
}

#endif