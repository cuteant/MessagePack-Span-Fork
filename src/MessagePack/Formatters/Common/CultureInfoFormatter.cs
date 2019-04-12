namespace MessagePack.Formatters
{
    using System.Globalization;
    using MessagePack.Internal;

    public sealed class CultureInfoFormatter : IMessagePackFormatter<CultureInfo>
    {
        public static readonly IMessagePackFormatter<CultureInfo> Instance = new CultureInfoFormatter();

        public CultureInfoFormatter() { }

        public CultureInfo Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return CultureInfo.InvariantCulture; }

            return new CultureInfo(MessagePackBinary.ResolveString(reader.ReadUtf8Span()));
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, CultureInfo value, IFormatterResolver formatterResolver)
        {
            if (value != null)
            {
                var encodedBytes = MessagePackBinary.GetEncodedStringBytes(value.Name);
                UnsafeMemory.WriteRaw(ref writer, encodedBytes, ref idx);
                return;
            }
            writer.WriteNil(ref idx);
        }
    }
}
