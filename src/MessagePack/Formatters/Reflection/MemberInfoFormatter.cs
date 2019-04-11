using System.Reflection;

namespace MessagePack.Formatters
{
    using MessagePack.Formatters.Internal;

    public sealed class MemberInfoFormatter : MemberInfoFormatter<MemberInfo>
    {
        public static readonly IMessagePackFormatter<MemberInfo> Instance = new MemberInfoFormatter();
        public MemberInfoFormatter() : base() { }

        public MemberInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class MemberInfoFormatter<TMember> : IMessagePackFormatter<TMember>
        where TMember : MemberInfo
    {
        private readonly bool _throwOnError;

        public MemberInfoFormatter() : this(true) { }

        public MemberInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TMember Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var formatter = formatterResolver.GetFormatter<MemberinfoShim>();
            var shim = formatter.Deserialize(ref reader, formatterResolver);
            switch (shim.MemberType)
            {
                case MemberinfoType.EventInfo:
                    return (TMember)(object)shim.Event;
                case MemberinfoType.FieldInfo:
                    return (TMember)(object)shim.Field;
                case MemberinfoType.MethodInfo:
                    return (TMember)(object)shim.Method;
                case MemberinfoType.PropertyInfo:
                    return (TMember)(object)shim.Property;
                default:
                    return null;
            }
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TMember value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            var shim = new MemberinfoShim();
            switch (value)
            {
                case EventInfo ei:
                    shim.MemberType = MemberinfoType.EventInfo;
                    shim.Event = ei;
                    break;

                case FieldInfo fi:
                    shim.MemberType = MemberinfoType.FieldInfo;
                    shim.Field = fi;
                    break;

                case MethodInfo mi:
                    shim.MemberType = MemberinfoType.MethodInfo;
                    shim.Method = mi;
                    break;

                case PropertyInfo pi:
                    shim.MemberType = MemberinfoType.PropertyInfo;
                    shim.Property = pi;
                    break;

                default:
                    writer.WriteNil(ref idx); return;
            }

            var formatter = formatterResolver.GetFormatter<MemberinfoShim>();
            formatter.Serialize(ref writer, ref idx, shim, formatterResolver);
        }
    }
}

namespace MessagePack.Formatters.Internal
{
    public enum MemberinfoType : byte
    {
        EventInfo = 0,
        FieldInfo = 1,
        MethodInfo = 2,
        PropertyInfo = 3
    }
    [MessagePackObject]
    public sealed class MemberinfoShim
    {
        [Key(0)]
        public MemberinfoType MemberType { get; set; }
        [Key(1)]
        public EventInfo Event { get; set; }
        [Key(2)]
        public FieldInfo Field { get; set; }
        [Key(3)]
        public MethodInfo Method { get; set; }
        [Key(4)]
        public PropertyInfo Property { get; set; }
    }
}
