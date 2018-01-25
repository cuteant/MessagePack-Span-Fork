using System;
using System.Net;

namespace MessagePack.Formatters
{
    public sealed class IPAddressFormatter : IMessagePackFormatter<IPAddress>
    {
        public static readonly IMessagePackFormatter<IPAddress> Instance = new IPAddressFormatter();

        public IPAddressFormatter()
        {
        }

        public IPAddress Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var addressBytes = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            return new IPAddress(addressBytes);

        }


        public int Serialize(ref byte[] bytes, int offset, IPAddress value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var addressBytes = value.GetAddressBytes();
            var length = addressBytes.Length;
            var totalSize = length + 2;
            MessagePackBinary.EnsureCapacity(ref bytes, offset, totalSize);
            bytes[offset] = MessagePackCode.Bin8;
            bytes[offset + 1] = (byte)length;

            Buffer.BlockCopy(addressBytes, 0, bytes, offset + 2, length);
            return totalSize;
        }
    }
}
