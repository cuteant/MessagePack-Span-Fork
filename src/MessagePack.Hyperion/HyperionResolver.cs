using System;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new HyperionResolver();

        HyperionResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
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
            if (typeof(IObjectReferences).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
            {
                return ActivatorUtils.FastCreateInstance(typeof(HyperionFormatter<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}