using System.Linq.Expressions;

namespace MessagePack.Formatters
{
    public sealed class ElementInitFormatter : DynamicObjectTypeFormatterBase<ElementInit>
    {
        public static readonly IMessagePackFormatter<ElementInit> Instance = new ElementInitFormatter();
    }
}
