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
            var declaringType = MessagePackBinary.ReadNamedType(bytes, offset + nameSize, out readSize, _throwOnError);
            readSize += nameSize;
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

            var nameSize = MessagePackBinary.WriteString(ref bytes, offset, value.Name);
            var typeSize = MessagePackBinary.WriteNamedType(ref bytes, offset + nameSize, value.DeclaringType);
            return nameSize + typeSize;
        }
    }
}
