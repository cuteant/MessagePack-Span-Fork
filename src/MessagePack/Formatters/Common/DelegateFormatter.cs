using System;
using System.Reflection;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    using MessagePack.Formatters.Internal;

    public sealed class DelegateFormatter : DelegateFormatter<Delegate>
    {
        public static readonly IMessagePackFormatter<Delegate> Instance = new DelegateFormatter();
    }

    public class DelegateFormatter<TDelegate> : IMessagePackFormatter<TDelegate>
    {
        public TDelegate Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var delegateShim = (IDelegateShim)MessagePackSerializer.Typeless.TypelessFormatter.Deserialize(bytes, offset, formatterResolver, out readSize);
            return (TDelegate)(object)delegateShim.Method.CreateDelegate(delegateShim.DelegateType, delegateShim.GetTarget());

        }

        public int Serialize(ref byte[] bytes, int offset, TDelegate value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var d = value as Delegate;
            var target = d.Target;
            IDelegateShim delegateShim;
            if (target != null)
            {
                delegateShim = ActivatorUtils.FastCreateInstance<IDelegateShim>(typeof(DelegateShim<>).GetCachedGenericType(target.GetType()));
            }
            else
            {
                delegateShim = new DelegateShim<object>();
            }
            delegateShim.DelegateType = value.GetType();
            delegateShim.SetTarget(target);
            delegateShim.Method = d.GetMethodInfo();
            return MessagePackSerializer.Typeless.TypelessFormatter.Serialize(ref bytes, offset, delegateShim, formatterResolver);
        }
    }
}

namespace MessagePack.Formatters.Internal
{
    [MessagePackObject]
    public class DelegateShim<T> : IDelegateShim
    {
        [Key(0)]
        public Type DelegateType { get; set; }
        [Key(1)]
        public T Target { get; set; }
        [Key(2)]
        public MethodInfo Method { get; set; }

        public object GetTarget() => Target;
        public void SetTarget(object target) => Target = (T)target;
    }

    public interface IDelegateShim
    {
        Type DelegateType { get; set; }
        MethodInfo Method { get; set; }
        object GetTarget();
        void SetTarget(object target);
    }
}
