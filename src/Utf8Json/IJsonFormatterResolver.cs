using System;
using System.Reflection;

namespace Utf8Json
{
    public interface IJsonFormatterResolver
    {
        IJsonFormatter<T> GetFormatter<T>();
    }

    public static class JsonFormatterResolverExtensions
    {
        public static IJsonFormatter<T> GetFormatterWithVerify<T>(this IJsonFormatterResolver resolver)
        {
            IJsonFormatter<T> formatter;
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
                ThrowHelper.ThrowFormatterNotRegisteredException<T>(resolver);
            }

            return formatter;
        }

        private static readonly MethodInfo s_getFormatterMethod = typeof(IJsonFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);
        public static object GetFormatterDynamic(this IJsonFormatterResolver resolver, Type type)
        {
            var formatter = s_getFormatterMethod.MakeGenericMethod(type).Invoke(resolver, null);
            return formatter;
        }
    }

    public class FormatterNotRegisteredException : Exception
    {
        public FormatterNotRegisteredException(string message) : base(message)
        {
        }
    }
}