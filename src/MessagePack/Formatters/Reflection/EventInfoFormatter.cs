using System.Reflection;

namespace MessagePack.Formatters
{
    public sealed class EventInfoFormatter : EventInfoFormatter<EventInfo>
    {
        public static readonly IMessagePackFormatter<EventInfo> Instance = new EventInfoFormatter();
        public EventInfoFormatter() : base() { }

        public EventInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class EventInfoFormatter<TEvent> : IMessagePackFormatter<TEvent>
        where TEvent : EventInfo
    {
        private readonly bool _throwOnError;

        public EventInfoFormatter() : this(true) { }

        public EventInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TEvent Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var name = reader.ReadString();
            var declaringType = reader.ReadNamedType(_throwOnError);
            return (TEvent)declaringType
                .GetEvent(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TEvent value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteString(value.Name, ref idx);
            writer.WriteNamedType(value.DeclaringType, ref idx);
        }
    }
}
