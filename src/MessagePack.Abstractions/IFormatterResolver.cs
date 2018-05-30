
using MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessagePack
{
    public interface IFormatterResolver
    {
        IDictionary<string, object> Context { get; }
        T GetContextValue<T>(string key);
        IMessagePackFormatter<T> GetFormatter<T>();
    }

    public abstract class FormatterResolver : IFormatterResolver
    {
        private IDictionary<string, object> _context;
        public virtual IDictionary<string, object> Context => _context ?? (_context = new Dictionary<string, object>(StringComparer.Ordinal));

        public virtual T GetContextValue<T>(string key)
        {
            if (Context.TryGetValue(key, out var v)) { return (T)v; }
            return default;
        }

        public abstract IMessagePackFormatter<T> GetFormatter<T>();
    }

    public static class FormatterResolverExtensions
    {
        public static IMessagePackFormatter<T> GetFormatterWithVerify<T>(this IFormatterResolver resolver)
        {
            IMessagePackFormatter<T> formatter;
            try
            {
                formatter = resolver.GetFormatter<T>();
            }
            catch (TypeInitializationException ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (formatter == null)
            {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name);
            }

            return formatter;
        }

#if !UNITY_WSA

        public static object GetFormatterDynamic(this IFormatterResolver resolver, Type type)
        {
            var methodInfo = typeof(IFormatterResolver)
#if NET40
                .GetMethod
#else
                .GetRuntimeMethod
#endif
                ("GetFormatter", Type.EmptyTypes);

            var formatter = methodInfo.MakeGenericMethod(type).Invoke(resolver, null);
            return formatter;
        }

#endif
    }

    public class FormatterNotRegisteredException : Exception
    {
        public FormatterNotRegisteredException(string message) : base(message)
        {
        }
    }
}