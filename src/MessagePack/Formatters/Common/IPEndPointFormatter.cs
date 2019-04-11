using System;
using System.Net;

namespace MessagePack.Formatters
{
    public sealed class IPEndPointFormatter : IMessagePackFormatter<IPEndPoint>
    {
        public static readonly IMessagePackFormatter<IPEndPoint> Instance = new IPEndPointFormatter();

        public IPEndPointFormatter() { }

        public IPEndPoint Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var port = reader.ReadInt32();
#if NETCOREAPP
            var addressBytes = reader.ReadBytesSegment();
            return new IPEndPoint(new IPAddress(addressBytes), port);
#else
            var addressBytes = reader.ReadBytes();
            return new IPEndPoint(new IPAddress(addressBytes), port);
#endif
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, IPEndPoint value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteInt32(value.Port, ref idx);

            var addressBytes = value.Address.GetAddressBytes();
            writer.WriteBytes(addressBytes, ref idx);
        }
    }
}
