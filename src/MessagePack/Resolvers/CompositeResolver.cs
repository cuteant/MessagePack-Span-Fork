using MessagePack.Formatters;
using System;
using System.Reflection;

namespace MessagePack.Resolvers
{
    public sealed class CompositeResolver : FormatterResolver
    {
        public static readonly CompositeResolver Instance = new CompositeResolver();

        static bool isFreezed = false;
        static IMessagePackFormatter[] formatters = new IMessagePackFormatter[0];
        static IFormatterResolver[] resolvers = new IFormatterResolver[0];

        CompositeResolver()
        {
        }

        public static void Register(params IFormatterResolver[] resolvers)
        {
            if (isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException_Register_Resolvers();
            }

            CompositeResolver.resolvers = resolvers;
        }

        public static void Register(params IMessagePackFormatter[] formatters)
        {
            if (isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException_Register_Resolvers();
            }

            CompositeResolver.formatters = formatters;
        }

        public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            if (isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException_Register_Resolvers();
            }

            CompositeResolver.resolvers = resolvers;
            CompositeResolver.formatters = formatters;
        }

        public static void RegisterAndSetAsDefault(params IFormatterResolver[] resolvers)
        {
            Register(resolvers);
            MessagePack.MessagePackSerializer.SetDefaultResolver(Instance);
        }

        public static void RegisterAndSetAsDefault(params IMessagePackFormatter[] formatters)
        {
            Register(formatters);
            MessagePack.MessagePackSerializer.SetDefaultResolver(Instance);
        }

        public static void RegisterAndSetAsDefault(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            Register(formatters);
            Register(resolvers);
            MessagePack.MessagePackSerializer.SetDefaultResolver(Instance);
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
                isFreezed = true;

                foreach (var item in formatters)
                {
                    foreach (var implInterface in item.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        if (implInterface.IsGenericType && implInterface.GenericTypeArguments[0] == typeof(T))
                        {
                            formatter = (IMessagePackFormatter<T>)item;
                            return;
                        }
                    }
                }

                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }
}
