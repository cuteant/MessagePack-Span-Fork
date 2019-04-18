namespace MessagePack.Formatters
{
    using System;
    using System.Net;

    public sealed class IPEndPointFormatter : IMessagePackFormatter<IPEndPoint>
    {
        public static readonly IMessagePackFormatter<IPEndPoint> Instance = new IPEndPointFormatter();

        private IPEndPointFormatter() { }

        const int c_count = 2;

        public IPEndPoint Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_IPEndPoint_Format(); }

            var port = reader.ReadInt32();
#if NETCOREAPP
            var addressBytes = reader.ReadSpan();
            return new IPEndPoint(new IPAddress(addressBytes), port);
#else
            var addressBytes = reader.ReadBytes();
            return new IPEndPoint(new IPAddress(addressBytes), port);
#endif
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, IPEndPoint value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteFixedArrayHeaderUnsafe(c_count, ref idx);

            writer.WriteInt32(value.Port, ref idx);

            var addressBytes = value.Address.GetAddressBytes();
            writer.WriteBytes(addressBytes, ref idx);
        }
    }
}
