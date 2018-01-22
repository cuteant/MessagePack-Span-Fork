using System.Net;

namespace Utf8Json.Formatters
{
    public sealed class IPAddressFormatter : IJsonFormatter<IPAddress>
    {
        public static readonly IJsonFormatter<IPAddress> Default = new IPAddressFormatter();

        public IPAddressFormatter()
        {
        }

        public IPAddress Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var raw = reader.ReadString();
            return raw != null ? IPAddress.Parse(raw) : null;
        }

        public void Serialize(ref JsonWriter writer, IPAddress value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value?.ToString());
        }
    }
}
