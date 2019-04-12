namespace MessagePack.Formatters
{
    using System.Reflection;

    public sealed class MemberInfoFormatter : MemberInfoFormatter<MemberInfo>
    {
        public static readonly IMessagePackFormatter<MemberInfo> Instance = new MemberInfoFormatter();
        public MemberInfoFormatter() : base() { }

        public MemberInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class MemberInfoFormatter<TMember> : IMessagePackFormatter<TMember>
        where TMember : MemberInfo
    {
        private const int c_count = 2;
        private readonly bool _throwOnError;

        public MemberInfoFormatter() : this(true) { }

        public MemberInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TMember Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_MemberInfo_Format(); }

            var typeCode = reader.ReadByte();
            switch (typeCode)
            {
                case _.EventInfo:
                    var evtFormatter = formatterResolver.GetFormatterWithVerify<EventInfo>();
                    return (TMember)(object)evtFormatter.Deserialize(ref reader, formatterResolver);

                case _.FieldInfo:
                    var fieldFormatter = formatterResolver.GetFormatterWithVerify<FieldInfo>();
                    return (TMember)(object)fieldFormatter.Deserialize(ref reader, formatterResolver);

                case _.MethodInfo:
                    var methodFormatter = formatterResolver.GetFormatterWithVerify<MethodInfo>();
                    return (TMember)(object)methodFormatter.Deserialize(ref reader, formatterResolver);

                case _.PropertyInfo:
                    var propertyFormatter = formatterResolver.GetFormatterWithVerify<PropertyInfo>();
                    return (TMember)(object)propertyFormatter.Deserialize(ref reader, formatterResolver);

                default:
                    ThrowHelper.ThrowInvalidOperationException_MemberInfo_Format(); return null;
            }
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TMember value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            switch (value)
            {
                case EventInfo ei:
                    writer.WriteArrayHeader(c_count, ref idx);
                    writer.WriteByte(_.EventInfo, ref idx);
                    var evtFormatter = formatterResolver.GetFormatterWithVerify<EventInfo>();
                    evtFormatter.Serialize(ref writer, ref idx, ei, formatterResolver);
                    break;

                case FieldInfo fi:
                    writer.WriteArrayHeader(c_count, ref idx);
                    writer.WriteByte(_.FieldInfo, ref idx);
                    var fieldFormatter = formatterResolver.GetFormatterWithVerify<FieldInfo>();
                    fieldFormatter.Serialize(ref writer, ref idx, fi, formatterResolver);
                    break;

                case MethodInfo mi:
                    writer.WriteArrayHeader(c_count, ref idx);
                    writer.WriteByte(_.MethodInfo, ref idx);
                    var methodFormatter = formatterResolver.GetFormatterWithVerify<MethodInfo>();
                    methodFormatter.Serialize(ref writer, ref idx, mi, formatterResolver);
                    break;

                case PropertyInfo pi:
                    writer.WriteArrayHeader(c_count, ref idx);
                    writer.WriteByte(_.PropertyInfo, ref idx);
                    var propertyFormatter = formatterResolver.GetFormatterWithVerify<PropertyInfo>();
                    propertyFormatter.Serialize(ref writer, ref idx, pi, formatterResolver);
                    break;

                default:
                    writer.WriteNil(ref idx); return;
            }
        }

        static class _
        {
            public const byte EventInfo = 0;
            public const byte FieldInfo = 1;
            public const byte MethodInfo = 2;
            public const byte PropertyInfo = 3;
        }
    }
}
