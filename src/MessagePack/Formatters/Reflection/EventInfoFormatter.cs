﻿namespace MessagePack.Formatters
{
    using System.Reflection;
    using MessagePack.Internal;

    public sealed class EventInfoFormatter : EventInfoFormatter<EventInfo>
    {
        public static readonly IMessagePackFormatter<EventInfo> Instance = new EventInfoFormatter();

        public EventInfoFormatter() : base() { }

        public EventInfoFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class EventInfoFormatter<TEvent> : IMessagePackFormatter<TEvent>
        where TEvent : EventInfo
    {
        private const int c_count = 2;
        private readonly bool _throwOnError;

        public EventInfoFormatter() : this(true) { }

        public EventInfoFormatter(bool throwOnError) => _throwOnError = throwOnError;

        public TEvent Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return null; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_EventInfo_Format(); }

            var name = MessagePackBinary.ResolveString(reader.ReadUtf8Span());
            var declaringType = reader.ReadNamedType(_throwOnError);
            return (TEvent)declaringType
                .GetEvent(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TEvent value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(c_count, ref idx);

            var encodedName = MessagePackBinary.GetEncodedStringBytes(value.Name);
            UnsafeMemory.WriteRaw(ref writer, encodedName, ref idx);
            writer.WriteNamedType(value.DeclaringType, ref idx);
        }
    }
}
