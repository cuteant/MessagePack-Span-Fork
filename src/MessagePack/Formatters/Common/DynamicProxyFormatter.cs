namespace MessagePack.Formatters
{
    public sealed class DynamicProxyFormatter<T> : IMessagePackFormatter<T>
    {
        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var formatter = formatterResolver.GetFormatter<WrappedObject>();
            var shim = formatter.Deserialize(bytes, offset, formatterResolver, out readSize);
            return (T)shim.Data;
        }

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var wrappedObject = new WrappedObject(value);
            var formatter = formatterResolver.GetFormatter<WrappedObject>();
            return formatter.Serialize(ref bytes, offset, wrappedObject, formatterResolver);
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
