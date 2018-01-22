using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MessagePack.Formatters
{
    public sealed class CultureInfoFormatter : IMessagePackFormatter<CultureInfo>
    {
        public static readonly IMessagePackFormatter<CultureInfo> Instance = new CultureInfoFormatter();

        public CultureInfoFormatter()
        {
        }

        public CultureInfo Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return CultureInfo.InvariantCulture;
            }
            else
            {
                return new CultureInfo(MessagePackBinary.ReadString(bytes, offset, out readSize));
            }
        }

        public int Serialize(ref byte[] bytes, int offset, CultureInfo value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteString(ref bytes, offset, value.Name);
            }
        }
    }
}
