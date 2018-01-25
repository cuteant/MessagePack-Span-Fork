using System;
using CuteAnt;

namespace MessagePack.Formatters
{
    public sealed class CombGuidFormatter : IMessagePackFormatter<CombGuid>
    {
        public static readonly IMessagePackFormatter<CombGuid> Instance = new CombGuidFormatter();


        CombGuidFormatter()
        {
        }

        const int c_totalSize = 18;
        const int c_valueSize = 16;

        public int Serialize(ref byte[] bytes, int offset, CombGuid value, IFormatterResolver formatterResolver)
        {
            const byte _byteSize = 16;

            var buffer = value.GetByteArray(CombGuidSequentialSegmentType.Guid);

            MessagePackBinary.EnsureCapacity(ref bytes, offset, c_totalSize);

            bytes[offset] = MessagePackCode.Bin8;
            bytes[offset + 1] = _byteSize;

            Buffer.BlockCopy(buffer, 0, bytes, offset + 2, c_valueSize);

            return c_totalSize;
        }

        public CombGuid Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var newBytes = new byte[c_valueSize];
            Buffer.BlockCopy(bytes, offset + 2, newBytes, 0, c_valueSize);

            readSize = c_totalSize;

            return new CombGuid(newBytes, CombGuidSequentialSegmentType.Guid, true);
        }
    }
}
