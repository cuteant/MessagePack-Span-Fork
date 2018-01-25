using System.Linq.Expressions;

namespace MessagePack.Formatters
{
    public sealed class CatchBlockFormatter : DynamicObjectTypeFormatterBase<CatchBlock>
    {
        public static readonly IMessagePackFormatter<CatchBlock> Instance = new CatchBlockFormatter();
    }
}
