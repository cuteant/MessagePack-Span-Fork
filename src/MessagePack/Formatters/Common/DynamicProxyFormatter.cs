namespace MessagePack.Formatters
{
    public sealed class DynamicProxyFormatter<T> : IMessagePackFormatter<T>
    {
        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var formatter = formatterResolver.GetFormatter<WrappedObject>();
            var shim = formatter.Deserialize(ref reader, formatterResolver);
            return (T)shim.Data;
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var wrappedObject = new WrappedObject(value);
            var formatter = formatterResolver.GetFormatter<WrappedObject>();
            formatter.Serialize(ref writer, ref idx, wrappedObject, formatterResolver);
        }

        [MessagePackObject]
        public readonly struct WrappedObject
        {
            [Key(0)]
            public readonly object Data;

            [SerializationConstructor]
            public WrappedObject(object data) => Data = data;
        }
    }
}
