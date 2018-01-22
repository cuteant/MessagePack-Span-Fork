using System.Reflection;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    public sealed class PropertyInfoFormatter : PropertyInfoFormatter<PropertyInfo>
    {
        public static readonly IMessagePackFormatter<PropertyInfo> Instance = new PropertyInfoFormatter();
        public PropertyInfoFormatter() : base() { }

        public PropertyInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class PropertyInfoFormatter<TProperty> : IMessagePackFormatter<TProperty>
        where TProperty : PropertyInfo
    {
        private readonly bool _throwOnError;

        public PropertyInfoFormatter() : this(true) { }

        public PropertyInfoFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public TProperty Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
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
            return (TProperty)declaringType
#if !NET40
                .GetTypeInfo()
#endif
                .GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public int Serialize(ref byte[] bytes, int offset, TProperty value, IFormatterResolver formatterResolver)
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
