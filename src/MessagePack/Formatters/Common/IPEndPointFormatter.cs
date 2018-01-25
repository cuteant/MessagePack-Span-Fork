using System;
using System.Net;

namespace MessagePack.Formatters
{
    public sealed class IPEndPointFormatter : IMessagePackFormatter<IPEndPoint>
    {
        public static readonly IMessagePackFormatter<IPEndPoint> Instance = new IPEndPointFormatter();

        public IPEndPointFormatter()
        {
        }

        public IPEndPoint Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            var port = MessagePackBinary.ReadInt32(bytes, offset, out var portSize);
            offset += portSize;
            var addressBytes = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            readSize += portSize;
            return new IPEndPoint(new IPAddress(addressBytes), port);
        }

        public int Serialize(ref byte[] bytes, int offset, IPEndPoint value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;

            var portSize = MessagePackBinary.WriteInt32(ref bytes, offset, value.Port);
            offset += portSize;

            var addressBytes = value.Address.GetAddressBytes();
            var length = addressBytes.Length;
            var totalSize = length + 2;
            MessagePackBinary.EnsureCapacity(ref bytes, offset, totalSize);
            bytes[offset] = MessagePackCode.Bin8;
            bytes[offset + 1] = (byte)length;

            Buffer.BlockCopy(addressBytes, 0, bytes, offset + 2, length);
            return totalSize + portSize;
        }
    }
}
