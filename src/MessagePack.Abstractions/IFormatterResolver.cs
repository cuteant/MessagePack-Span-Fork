using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MessagePack.Formatters;

namespace MessagePack
{
    public interface IFormatterResolver
    {
        IMessagePackFormatter<T> GetFormatter<T>();
    }

    public interface IFormatterResolverContext<T>
    {
        T Value { get; }
    }

    public abstract class FormatterResolver : IFormatterResolver
    {
        public abstract IMessagePackFormatter<T> GetFormatter<T>();
    }

    public static class FormatterResolverExtensions
    {
        public static IMessagePackFormatter<T> GetFormatterWithVerify<T>(this IFormatterResolver resolver)
        {
            IMessagePackFormatter<T> formatter = null;
            try
            {
                formatter = resolver.GetFormatter<T>();
            }
            catch (TypeInitializationException ex)
            {
                ThrowTypeInitializationException(ex);
            }

            if (null == formatter)
            {
                ThrowFormatterNotRegisteredException<T>(resolver);
            }

            return formatter;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowTypeInitializationException(TypeInitializationException ex)
        {
            throw GetException();
            Exception GetException()
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                return inner;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowFormatterNotRegisteredException<T>(IFormatterResolver resolver)
        {
            throw GetException();
            FormatterNotRegisteredException GetException()
            {
                return new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name);
            }
        }

#if !UNITY_WSA
        private static readonly MethodInfo s_getFormatterMethod = typeof(IFormatterResolver)
#if NET40
                .GetMethod
#else
                .GetRuntimeMethod
#endif
                ("GetFormatter", Type.EmptyTypes);
        public static object GetFormatterDynamic(this IFormatterResolver resolver, Type type)
        {
            var formatter = s_getFormatterMethod.MakeGenericMethod(type).Invoke(resolver, null);
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