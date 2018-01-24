using System.Reflection;

namespace MessagePack.Formatters
{
    public sealed class FieldInfoFormatter : FieldInfoFormatter<FieldInfo>
    {
        public static readonly IMessagePackFormatter<FieldInfo> Instance = new FieldInfoFormatter();
        public FieldInfoFormatter() : base() { }

        public FieldInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class FieldInfoFormatter<TField> : IMessagePackFormatter<TField>
        where TField : FieldInfo
    {
        private readonly bool _throwOnError;

        public FieldInfoFormatter() : this(true) { }

        public FieldInfoFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public TField Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var name = MessagePackBinary.ReadString(bytes, offset, out var nameSize);
            var declaringType = MessagePackBinary.ReadNamedType(bytes, offset + nameSize, out readSize, _throwOnError);
            readSize += nameSize;
            return (TField)declaringType
#if !NET40
                .GetTypeInfo()
#endif
                .GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public int Serialize(ref byte[] bytes, int offset, TField value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var nameSize = MessagePackBinary.WriteString(ref bytes, offset, value.Name);
            var typeSize = MessagePackBinary.WriteNamedType(ref bytes, offset + nameSize, value.DeclaringType);
            return nameSize + typeSize;
        }
    }
}
