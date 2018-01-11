using CuteAnt;

namespace Utf8Json.Formatters
{
    public sealed class CombGuidFormatter : IJsonFormatter<CombGuid>, IObjectPropertyNameFormatter<CombGuid>
    {
        public static readonly IJsonFormatter<CombGuid> Default = new CombGuidFormatter();

        public void Serialize(ref JsonWriter writer, CombGuid value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString(CombGuidFormatStringType.Comb));
        }

        public CombGuid Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadString();
            return CombGuid.Parse(str, CombGuidSequentialSegmentType.Comb);
        }

        public void SerializeToPropertyName(ref JsonWriter writer, CombGuid value, IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value, formatterResolver);
        }

        public CombGuid DeserializeFromPropertyName(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return Deserialize(ref reader, formatterResolver);
        }
    }
}
