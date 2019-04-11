using System;
using System.Reflection;
#if DEPENDENT_ON_CUTEANT
using CuteAnt.Reflection;
#else
using MessagePack.Internal;
#endif

namespace MessagePack.Formatters
{
    using MessagePack.Formatters.Internal;

    public sealed class DelegateFormatter : DelegateFormatter<Delegate>
    {
        public static readonly IMessagePackFormatter<Delegate> Instance = new DelegateFormatter();
    }

    public class DelegateFormatter<TDelegate> : IMessagePackFormatter<TDelegate>
    {
        public TDelegate Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var delegateShim = (IDelegateShim)MessagePackSerializer.Typeless.TypelessFormatter.Deserialize(ref reader, formatterResolver);
            return (TDelegate)(object)delegateShim.Method.CreateDelegate(delegateShim.DelegateType, delegateShim.GetTarget());

        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, TDelegate value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNil(ref idx); return;
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
            MessagePackSerializer.Typeless.TypelessFormatter.Serialize(ref writer, ref idx, delegateShim, formatterResolver);
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
