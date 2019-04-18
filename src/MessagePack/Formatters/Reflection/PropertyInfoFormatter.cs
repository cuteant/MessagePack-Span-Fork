namespace MessagePack.Formatters
{
    using System.Reflection;
    using MessagePack.Internal;

    public sealed class PropertyInfoFormatter : PropertyInfoFormatter<PropertyInfo>
    {
        public static readonly IMessagePackFormatter<PropertyInfo> Instance = new PropertyInfoFormatter();
        public PropertyInfoFormatter() : base() { }

        public PropertyInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class PropertyInfoFormatter<TProperty> : IMessagePackFormatter<TProperty>
        where TProperty : PropertyInfo
    {
        private const int c_count = 2;
        private readonly bool _throwOnError;

        public PropertyInfoFormatter() : this(true) { }

        public PropertyInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TProperty Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_PropertyInfo_Format(); }

            var name = MessagePackBinary.ResolveString(reader.ReadUtf8Span());
            var declaringType = reader.ReadNamedType(_throwOnError);
            return (TProperty)declaringType
                .GetProperty(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TProperty value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteFixedArrayHeaderUnsafe(c_count, ref idx);

            var encodedName = MessagePackBinary.GetEncodedStringBytes(value.Name);
            UnsafeMemory.WriteRaw(ref writer, encodedName, ref idx);

            writer.WriteNamedType(value.DeclaringType, ref idx);
        }
    }
}
