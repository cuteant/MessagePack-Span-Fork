using System.Linq.Expressions;

namespace MessagePack.Formatters
{
    public sealed class LabelTargetFormatter : DynamicObjectTypeFormatterBase<LabelTarget>
    {
        public static readonly IMessagePackFormatter<LabelTarget> Instance = new LabelTargetFormatter();
    }
}
