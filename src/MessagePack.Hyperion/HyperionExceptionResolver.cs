using System;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionExceptionResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new HyperionExceptionResolver();

        HyperionExceptionResolver()
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
                formatter = (IMessagePackFormatter<T>)HyperionExceptionGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class HyperionExceptionGetFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
            if (typeof(Exception).IsAssignableFrom(t))
            {
                return ActivatorUtils.FastCreateInstance(typeof(HyperionExceptionFormatter<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}