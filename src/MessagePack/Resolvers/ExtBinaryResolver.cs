using MessagePack.Formatters;
using MessagePack.Internal;
using System;

namespace MessagePack.Resolvers
{
    public sealed class ExtBinaryResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ExtBinaryResolver();

        ExtBinaryResolver() { }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (IMessagePackFormatter<T>)ExtBinaryResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }
}

namespace MessagePack.Internal
{
    internal static class ExtBinaryResolverGetFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
            if (t == typeof(Guid))
            {
                return ExtBinaryGuidFormatter.Instance;
            }
            else if (t == typeof(Guid?))
            {
                return new StaticNullableFormatter<Guid>(ExtBinaryGuidFormatter.Instance);
            }
            else if (t == typeof(Decimal))
            {
                return ExtBinaryDecimalFormatter.Instance;
            }
            else if (t == typeof(Decimal?))
            {
                return new StaticNullableFormatter<Decimal>(ExtBinaryDecimalFormatter.Instance);
            }

            return null;
        }
    }
}
