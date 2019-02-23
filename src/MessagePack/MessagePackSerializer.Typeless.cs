#if NETSTANDARD || NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using MessagePack.Resolvers;
using MessagePack.Formatters;
using System.IO;

namespace MessagePack
{
    // Typeless API
    public static partial class MessagePackSerializer
    {
        public static class Typeless
        {
            static IFormatterResolver defaultResolver = MessagePack.Resolvers.TypelessContractlessStandardResolver.Instance;

            public static void RegisterDefaultResolver(params IFormatterResolver[] resolvers)
            {
                CompositeResolver.Register(resolvers);
                Interlocked.Exchange(ref defaultResolver, CompositeResolver.Instance);
            }

            internal static readonly Type DefaultTypelessFormatterType = typeof(TypelessFormatter);
            private static readonly IMessagePackFormatter<object> s_defaultTypelessFormatter = MessagePack.Formatters.TypelessFormatter.Instance;
            private static IMessagePackFormatter<object> s_typelessFormatter;
            private static Type s_typelessFormatterType;
            internal static IMessagePackFormatter<object> TypelessFormatter
            {
                [MethodImpl(InlineMethod.Value)]
                get => Volatile.Read(ref s_typelessFormatter) ?? s_defaultTypelessFormatter;
            }
            internal static Type TypelessFormatterType
            {
                [MethodImpl(InlineMethod.Value)]
                get => Volatile.Read(ref s_typelessFormatterType) ?? DefaultTypelessFormatterType;
            }
            public static void RegisterTypelessFormatter(IMessagePackFormatter<object> typelessFormatter)
            {
                if (null == typelessFormatter) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.typelessFormatter); }

                if (Interlocked.CompareExchange(ref s_typelessFormatter, typelessFormatter, null) == null)
                {
                    s_typelessFormatterType = typelessFormatter.GetType();
                }
            }

            public static byte[] Serialize(object obj)
            {
                return MessagePackSerializer.Serialize(obj, defaultResolver);
            }

            public static void Serialize(Stream stream, object obj)
            {
                MessagePackSerializer.Serialize(stream, obj, defaultResolver);
            }

#if !NET40
            public static System.Threading.Tasks.Task SerializeAsync(Stream stream, object obj)
            {
                return MessagePackSerializer.SerializeAsync(stream, obj, defaultResolver);
            }
#endif

            public static object Deserialize(byte[] bytes)
            {
                return MessagePackSerializer.Deserialize<object>(bytes, defaultResolver);
            }

            public static object Deserialize(Stream stream)
            {
                return MessagePackSerializer.Deserialize<object>(stream, defaultResolver);
            }

            public static object Deserialize(Stream stream, bool readStrict)
            {
                return MessagePackSerializer.Deserialize<object>(stream, defaultResolver, readStrict);
            }

#if !NET40
            public static System.Threading.Tasks.Task<object> DeserializeAsync(Stream stream)
            {
                return MessagePackSerializer.DeserializeAsync<object>(stream, defaultResolver);
            }
#endif

            class CompositeResolver : FormatterResolver
            {
                public static readonly CompositeResolver Instance = new CompositeResolver();

                static bool isFreezed = false;
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
    }
}

#endif