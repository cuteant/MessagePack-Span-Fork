using System;
using System.Linq.Expressions;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionExpressionResolver2 : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new HyperionExpressionResolver2();

        HyperionExpressionResolver2()
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
                formatter = (IMessagePackFormatter<T>)HyperionExpressionGetFormatterHelper2.GetFormatter(typeof(T));
            }
        }
    }

    internal static class HyperionExpressionGetFormatterHelper2
    {
        internal static object GetFormatter(Type t)
        {
            var ti = t.GetTypeInfo();
            if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Expression<>))
            {
                return ActivatorUtils.FastCreateInstance(typeof(HyperionExpressionFormatter2<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}