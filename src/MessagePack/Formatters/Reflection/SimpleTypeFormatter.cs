using System;

namespace MessagePack.Formatters
{
    public sealed class SimpleTypeFormatter : SimpleTypeFormatter<Type>
    {
        public static readonly SimpleTypeFormatter Instance = new SimpleTypeFormatter();
        public SimpleTypeFormatter() : base() { }

        public SimpleTypeFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class SimpleTypeFormatter<TType> : IMessagePackFormatter<TType>
        where TType : Type
    {
        private readonly bool _throwOnError;

        public SimpleTypeFormatter() : this(true) { }

        public SimpleTypeFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TType Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            return (TType)reader.ReadNamedType(_throwOnError);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TType value, IFormatterResolver formatterResolver)
        {
            writer.WriteNamedType(value, ref idx);
        }
    }
}
