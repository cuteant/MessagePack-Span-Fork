#if DEPENDENT_ON_CUTEANT

namespace MessagePack.Formatters
{
    using System;
    using CuteAnt;

    public sealed class CombGuidFormatter : IMessagePackFormatter<CombGuid>
    {
        public static readonly IMessagePackFormatter<CombGuid> Instance = new CombGuidFormatter();

        CombGuidFormatter() { }

        const int c_totalSize = 18;
        const int c_valueSize = 16;

        public void Serialize(ref MessagePackWriter writer, ref int idx, CombGuid value, IFormatterResolver formatterResolver)
        {
            writer.Ensure(idx, c_totalSize);

            var buffer = value.GetByteArray(CombGuidSequentialSegmentType.Guid);

            writer.WriteBytes(buffer, ref idx);
        }

        public CombGuid Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var valueBytes = reader.ReadBytes();
            return new CombGuid(valueBytes, CombGuidSequentialSegmentType.Guid, true);
        }
    }
}

#endif