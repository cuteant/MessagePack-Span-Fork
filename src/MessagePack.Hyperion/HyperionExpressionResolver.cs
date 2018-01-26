﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack
{
    public sealed class HyperionExpressionResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new HyperionExpressionResolver();

        HyperionExpressionResolver()
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
                formatter = (IMessagePackFormatter<T>)HyperionExpressionGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class HyperionExpressionGetFormatterHelper
    {
        internal static object GetFormatter(Type t)
        {
            var ti = t.GetTypeInfo();
            if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Expression<>))
            {
                return ActivatorUtils.FastCreateInstance(typeof(SimpleHyperionFormatter<>).GetCachedGenericType(t));
            }

            return null;
        }
    }
}