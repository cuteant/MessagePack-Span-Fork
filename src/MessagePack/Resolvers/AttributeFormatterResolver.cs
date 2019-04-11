namespace MessagePack.Resolvers
{
    using System;
    using System.Reflection;
    using System.Linq; // require UNITY_WSA
    using MessagePack.Formatters;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#endif

    /// <summary>Get formatter from [MessaegPackFromatter] attribute.</summary>
    public sealed class AttributeFormatterResolver : FormatterResolver
    {
        public static IFormatterResolver Instance = new AttributeFormatterResolver();

        AttributeFormatterResolver() { }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
#if DEPENDENT_ON_CUTEANT
                var attr = typeof(T).GetCustomAttributeX<MessagePackFormatterAttribute>();
#else
                var attr = (MessagePackFormatterAttribute)typeof(T).GetCustomAttributes(typeof(MessagePackFormatterAttribute), true).FirstOrDefault();
#endif
                if (attr == null) { return; }

                if (attr.FormatterType == MessagePackSerializer.Typeless.TypelessFormatterType ||
                    attr.FormatterType == MessagePackSerializer.Typeless.DefaultTypelessFormatterType)
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