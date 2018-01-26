using Hyperion;

namespace MessagePack.Formatters
{
    public class HyperionFormatter<T> : HyperionFormatterBase<T>
    {
        public HyperionFormatter() : base() { }
        public HyperionFormatter(SerializerOptions options) : base(options) { }
    }
}
