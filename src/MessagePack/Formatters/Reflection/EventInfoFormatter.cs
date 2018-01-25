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

        public EventInfoFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public TEvent Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var name = MessagePackBinary.ReadString(bytes, offset, out var nameSize);
            var declaringType = MessagePackBinary.ReadNamedType(bytes, offset + nameSize, out readSize, _throwOnError);
            readSize += nameSize;
            return (TEvent)declaringType
#if !NET40
                .GetTypeInfo()
#endif
                .GetEvent(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public int Serialize(ref byte[] bytes, int offset, TEvent value, IFormatterResolver formatterResolver)
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
