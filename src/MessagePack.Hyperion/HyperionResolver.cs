using System;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new HyperionResolver();

        HyperionResolver()
        {
        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                formatter = (IMessagePackFormatter<T>)HyperionGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class HyperionGetFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
            if (typeof(IObjectReferences).IsAssignableFrom(t))
            {
                return ActivatorUtils.FastCreateInstance(typeof(HyperionFormatter<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}