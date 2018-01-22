using System;
using CuteAnt.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class SimpleTypeFormatter : IJsonFormatter<Type>
    {
        public static readonly SimpleTypeFormatter Default = new SimpleTypeFormatter();
        private readonly bool _throwOnError;

        public SimpleTypeFormatter() : this(true) { }

        public SimpleTypeFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public void Serialize(ref JsonWriter writer, Type value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNull(); return; }

            writer.WriteString(RuntimeTypeNameFormatter.Format(value));
        }

        public Type Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            var s = reader.ReadString();
            Type result;
            if (_throwOnError)
            {
                result = TypeUtils.ResolveType(s);
            }
            else
            {
                TypeUtils.TryResolveType(s, out result);
            }
            return result;
        }
    }
}
