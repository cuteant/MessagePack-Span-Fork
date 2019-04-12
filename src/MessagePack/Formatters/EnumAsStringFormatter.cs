namespace MessagePack.Formatters
{
    using System;
    using System.Collections.Generic;
    using MessagePack.Internal;

    // Note:This implemenataion is 'not' fastest, should more improve.
    public sealed class EnumAsStringFormatter<T> : IMessagePackFormatter<T>
    {
        readonly Dictionary<string, T> nameValueMapping;
        readonly Dictionary<T, string> valueNameMapping;

        public EnumAsStringFormatter()
        {
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));

            nameValueMapping = new Dictionary<string, T>(names.Length);
            valueNameMapping = new Dictionary<T, string>(names.Length);

            for (int i = 0; i < names.Length; i++)
            {
                nameValueMapping[names[i]] = (T)values.GetValue(i);
                valueNameMapping[(T)values.GetValue(i)] = names[i];
            }
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (!valueNameMapping.TryGetValue(value, out string name))
            {
                name = value.ToString(); // fallback for flags etc, But Enum.ToString is too slow.
            }

            var encodedName = MessagePackBinary.GetEncodedStringBytes(name);
            UnsafeMemory.WriteRaw(ref writer, encodedName, ref idx);
        }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var name = MessagePackBinary.ResolveString(reader.ReadUtf8Span());

            if (!nameValueMapping.TryGetValue(name, out T value))
            {
                value = (T)Enum.Parse(typeof(T), name); // Enum.Parse is too slow
            }
            return value;
        }
    }
}
