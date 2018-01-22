using System;
using System.Net;
using CuteAnt;

namespace Utf8Json.Formatters
{
    public sealed class IPEndPointFormatter : IJsonFormatter<IPEndPoint>
    {
        public static readonly IJsonFormatter<IPEndPoint> Default = new IPEndPointFormatter();

        public IPEndPointFormatter()
        {
        }

        public IPEndPoint Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var raw = reader.ReadString();
            if (null == raw) { return null; }
            var ips = raw.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length == 1) { return null; }
            return new IPEndPoint(IPAddress.Parse(ips[0]), ips[1].ToInt());
        }

        public void Serialize(ref JsonWriter writer, IPEndPoint value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value != null ? $"{value.Address.ToString()}:{value.Port.ToString()}" : null);
        }
    }
}
