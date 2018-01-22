using System;
using System.Globalization;

namespace Utf8Json.Formatters
{
    public sealed class CultureInfoFormatter : IJsonFormatter<CultureInfo>
    {
        public static readonly IJsonFormatter<CultureInfo> Default = new CultureInfoFormatter();

        public CultureInfoFormatter()
        {
        }

        public void Serialize(ref JsonWriter writer, CultureInfo value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value?.Name);
        }

        public CultureInfo Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadString();
            return str != null ? new CultureInfo(str) : CultureInfo.InvariantCulture;
        }
    }
}
