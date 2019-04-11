namespace MessagePack.Formatters
{
    public sealed class NullableFormatter<T> : IMessagePackFormatter<T?>
        where T : struct
    {
        public void Serialize(ref MessagePackWriter writer, ref int idx, T? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            formatterResolver.GetFormatterWithVerify<T>().Serialize(ref writer, ref idx, value.Value, formatterResolver);
        }

        public T? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            return formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref reader, formatterResolver);
        }
    }

    public sealed class StaticNullableFormatter<T> : IMessagePackFormatter<T?>
        where T : struct
    {
        readonly IMessagePackFormatter<T> underlyingFormatter;

        public StaticNullableFormatter(IMessagePackFormatter<T> underlyingFormatter) => this.underlyingFormatter = underlyingFormatter;

        public void Serialize(ref MessagePackWriter writer, ref int idx, T? value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            underlyingFormatter.Serialize(ref writer, ref idx, value.Value, formatterResolver);
        }

        public T? Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            return underlyingFormatter.Deserialize(ref reader, formatterResolver);
        }
    }
}