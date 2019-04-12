namespace MessagePack.Formatters
{
    using System;
    using System.Reflection;

    public sealed class DelegateFormatter : DelegateFormatter<Delegate>
    {
        public static readonly IMessagePackFormatter<Delegate> Instance = new DelegateFormatter();
    }

    public class DelegateFormatter<TDelegate> : IMessagePackFormatter<TDelegate>
    {
        const int c_count = 3;

        public TDelegate Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var count = reader.ReadArrayHeader();
            if (count != c_count) { ThrowHelper.ThrowInvalidOperationException_Delegate_Format(); }

            var delegateType = reader.ReadNamedType(true);
            var target = MessagePackSerializer.Typeless.TypelessFormatter.Deserialize(ref reader, formatterResolver);
            var miFormatter = formatterResolver.GetFormatterWithVerify<MethodInfo>();
            var mi = miFormatter.Deserialize(ref reader, formatterResolver);

            return (TDelegate)(object)mi.CreateDelegate(delegateType, target);

        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TDelegate value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

            writer.WriteArrayHeader(c_count, ref idx);

            writer.WriteNamedType(value.GetType(), ref idx);
            var d = value as Delegate;
            MessagePackSerializer.Typeless.TypelessFormatter.Serialize(ref writer, ref idx, d.Target, formatterResolver);
            var miFormatter = formatterResolver.GetFormatterWithVerify<MethodInfo>();
            miFormatter.Serialize(ref writer, ref idx, d.GetMethodInfo(), formatterResolver);
        }
    }
}
