using System.Globalization;

namespace MessagePack.Formatters
{
    public sealed class CultureInfoFormatter : IMessagePackFormatter<CultureInfo>
    {
        public static readonly IMessagePackFormatter<CultureInfo> Instance = new CultureInfoFormatter();

        public CultureInfoFormatter() { }

        public CultureInfo Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return CultureInfo.InvariantCulture; }
            return new CultureInfo(reader.ReadString());
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, CultureInfo value, IFormatterResolver formatterResolver)
        {
            if (value != null)
            {
                writer.WriteString(value.Name, ref idx);
                return;
            }
            writer.WriteNil(ref idx);
        }
    }
}
