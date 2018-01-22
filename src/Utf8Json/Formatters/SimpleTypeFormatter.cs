using System;
using CuteAnt.Reflection;

namespace Utf8Json.Formatters
{
    public sealed class SimpleTypeFormatter : SimpleTypeFormatter<Type>
    {
        public static readonly SimpleTypeFormatter Default = new SimpleTypeFormatter();

        public SimpleTypeFormatter() : base() { }

        public SimpleTypeFormatter(bool throwOnError) : base(throwOnError) { }
    }

    public class SimpleTypeFormatter<TType> : IJsonFormatter<TType>
        where TType : Type
    {
        private readonly bool _throwOnError;

        public SimpleTypeFormatter() : this(true) { }

        public SimpleTypeFormatter(bool throwOnError)
        {
            _throwOnError = throwOnError;
        }

        public void Serialize(ref JsonWriter writer, TType value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNull(); return; }

            writer.WriteString(RuntimeTypeNameFormatter.Format(value));
        }

        public TType Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
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
            return (TType)result;
        }
    }
}
