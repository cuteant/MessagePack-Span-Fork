#if !UNITY_WSA

using MessagePack.Formatters;
using MessagePack.Internal;
using System;
using System.Reflection;
using CuteAnt.Reflection;

namespace MessagePack.Resolvers
{
    public sealed class DynamicEnumAsStringResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new DynamicEnumAsStringResolver();

        DynamicEnumAsStringResolver()
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
                var ti = typeof(T);

                if (ti.IsNullable())
                {
                    // build underlying type and use wrapped formatter.
#if NET40
                    ti = ti.GenericTypeArguments()[0];
#else
                    ti = ti.GenericTypeArguments[0];
#endif
                    if (!ti.IsEnum)
                    {
                        return;
                    }

                    var innerFormatter = DynamicEnumAsStringResolver.Instance.GetFormatterDynamic(ti);
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IMessagePackFormatter<T>)ActivatorUtils.CreateInstance(typeof(StaticNullableFormatter<>).GetCachedGenericType(ti), new object[] { innerFormatter });
                    return;
                }
                else if (!ti.IsEnum)
                {
                    return;
                }

                formatter = (IMessagePackFormatter<T>)(object)new EnumAsStringFormatter<T>();
            }
        }
    }
}

#endif