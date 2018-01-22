using System.Reflection;
using CuteAnt.Reflection;

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
            offset += nameSize;
            var hashCode = MessagePackBinary.ReadInt32(bytes, offset, out var hashCodeSize);
            offset += hashCodeSize;
            var typeName = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            readSize += (nameSize + hashCodeSize);
            var declaringType = TypeSerializer.GetTypeFromTypeKey(new TypeKey(hashCode, typeName), _throwOnError);
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

            var declaringType = value.DeclaringType;
            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Name);
            var typeKey = TypeSerializer.GetTypeKeyFromType(declaringType);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, typeKey.HashCode);
            var typeName = typeKey.TypeName;
            offset += MessagePackBinary.WriteBytes(ref bytes, offset, typeName, 0, typeName.Length);
            return offset - startOffset;
        }
    }
}
