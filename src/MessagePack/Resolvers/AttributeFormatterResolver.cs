using MessagePack.Formatters;
using System;
using System.Reflection;
using System.Linq; // require UNITY_WSA
using CuteAnt.Reflection;

namespace MessagePack.Resolvers
{
    /// <summary>
    /// Get formatter from [MessaegPackFromatter] attribute.
    /// </summary>
    public sealed class AttributeFormatterResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new AttributeFormatterResolver();

        AttributeFormatterResolver()
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
#if UNITY_WSA && !NETFX_CORE
                var attr = (MessagePackFormatterAttribute)typeof(T).GetCustomAttributes(typeof(MessagePackFormatterAttribute), true).FirstOrDefault();
#else
                var attr = typeof(T).GetCustomAttributeX<MessagePackFormatterAttribute>();
#endif
                if (attr == null) { return; }

                if (attr.FormatterType == MessagePackSerializer.Typeless.TypelessFormatterType)
                {
                    formatter = (IMessagePackFormatter<T>)MessagePackSerializer.Typeless.TypelessFormatter;
                }
                else
                {
                    if (attr.Arguments == null)
                    {
                        formatter = (IMessagePackFormatter<T>)ActivatorUtils.FastCreateInstance(attr.FormatterType);
                    }
                    else
                    {
                        formatter = (IMessagePackFormatter<T>)ActivatorUtil.CreateInstance(attr.FormatterType, attr.Arguments);
                    }
                }
            }
        }
    }
}