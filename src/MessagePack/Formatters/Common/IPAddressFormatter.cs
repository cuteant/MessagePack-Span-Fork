namespace MessagePack.Formatters
{
    using System;
    using System.Net;

    public sealed class IPAddressFormatter : IMessagePackFormatter<IPAddress>
    {
        public static readonly IMessagePackFormatter<IPAddress> Instance = new IPAddressFormatter();

        private IPAddressFormatter() { }

        public IPAddress Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

#if NETCOREAPP
            var addressBytes = reader.ReadBytesSegment();
            return new IPAddress(addressBytes);
#else
            var addressBytes = reader.ReadBytes();
            return new IPAddress(addressBytes);
#endif

        }


        public void Serialize(ref MessagePackWriter writer, ref int idx, IPAddress value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteBytes(value.GetAddressBytes(), ref idx);
        }
    }
}
