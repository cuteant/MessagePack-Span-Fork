using System;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionExceptionResolver2 : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new HyperionExceptionResolver2();

        HyperionExceptionResolver2()
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
                formatter = (IMessagePackFormatter<T>)HyperionExceptionGetFormatterHelper2.GetFormatter(typeof(T));
            }
        }
    }

    internal static class HyperionExceptionGetFormatterHelper2
    {
        internal static object GetFormatter(Type t)
        {
            if (typeof(Exception).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
            {
                return ActivatorUtils.FastCreateInstance(typeof(HyperionExceptionFormatter2<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}