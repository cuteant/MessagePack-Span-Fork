
namespace MessagePack.Formatters
{
    // marker
    public interface IMessagePackFormatter
    {
    }

    public interface IMessagePackFormatter<T> : IMessagePackFormatter
    {
        void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver);

        T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver);
    }
}
