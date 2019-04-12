﻿namespace MessagePack.Formatters
{
    using System.Reflection;
    using MessagePack.Internal;

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

        public FieldInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TField Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var name = MessagePackBinary.ResolveString(reader.ReadStringSegment());
            var declaringType = reader.ReadNamedType(_throwOnError);
            return (TField)declaringType
                .GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TField value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var encodedName = MessagePackBinary.GetEncodedStringBytes(value.Name);
            UnsafeMemory.WriteRaw(ref writer, encodedName, ref idx);
            writer.WriteNamedType(value.DeclaringType, ref idx);
        }
    }
}
