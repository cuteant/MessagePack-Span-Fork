using System.Reflection;

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

        public PropertyInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TProperty Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var name = reader.ReadString();
            var declaringType = reader.ReadNamedType(_throwOnError);
            return (TProperty)declaringType
                .GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TProperty value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteString(value.Name, ref idx);
            writer.WriteNamedType(value.DeclaringType, ref idx);
        }
    }
}
