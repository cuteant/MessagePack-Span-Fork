namespace MessagePack.Formatters
{
    using Hyperion;

    public class HyperionFormatter<T> : HyperionFormatterBase<T>
    {
        public HyperionFormatter() : base() { }
        public HyperionFormatter(SerializerOptions options) : base(options) { }
    }
}
