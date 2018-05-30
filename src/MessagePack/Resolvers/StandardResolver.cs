using MessagePack.Formatters;
using System.Linq;
using MessagePack.Internal;
using MessagePack.Resolvers;

namespace MessagePack.Resolvers
{
    /// <summary>
    /// Default composited resolver, builtin -> attribute -> dynamic enum -> dynamic generic -> dynamic union -> dynamic object -> primitive.
    /// </summary>
    public sealed class StandardResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolver();

#if NETSTANDARD || DESKTOPCLR
        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(StandardResolverCore.Instance);
#endif

        StandardResolver()
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
                if (typeof(T) == typeof(object))
                {
                    // final fallback
#if NETSTANDARD || DESKTOPCLR
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
#else
                    formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
#endif
                }
                else
                {
                    formatter = StandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class ContractlessStandardResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolver();

#if NETSTANDARD || DESKTOPCLR
        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(ContractlessStandardResolverCore.Instance);
#endif

        ContractlessStandardResolver()
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
                if (typeof(T) == typeof(object))
                {
                    // final fallback
#if NETSTANDARD || DESKTOPCLR
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
#else
                    formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
#endif
                }
                else
                {
                    formatter = ContractlessStandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class StandardResolverAllowPrivate : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverAllowPrivate();

#if NETSTANDARD || DESKTOPCLR
        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(StandardResolverAllowPrivateCore.Instance);
#endif

        StandardResolverAllowPrivate()
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
                if (typeof(T) == typeof(object))
                {
                    // final fallback
#if NETSTANDARD || DESKTOPCLR
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
#else
                    formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
#endif
                }
                else
                {
                    formatter = StandardResolverAllowPrivateCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class ContractlessStandardResolverAllowPrivate : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverAllowPrivate();

#if NETSTANDARD || DESKTOPCLR
        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(ContractlessStandardResolverAllowPrivateCore.Instance);
#endif

        ContractlessStandardResolverAllowPrivate()
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
                if (typeof(T) == typeof(object))
                {
                    // final fallback
#if NETSTANDARD || DESKTOPCLR
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
#else
                    formatter = PrimitiveObjectResolver.Instance.GetFormatter<T>();
#endif
                }
                else
                {
                    formatter = ContractlessStandardResolverAllowPrivateCore.Instance.GetFormatter<T>();
                }
            }
        }
    }
}

namespace MessagePack.Internal
{
    internal static class StandardResolverHelper
    {
        public static readonly IFormatterResolver[] DefaultResolvers = new[]
        {
            BuiltinResolver.Instance, // Try Builtin
            AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]

#if !(NETSTANDARD || DESKTOPCLR)
            MessagePack.Unity.UnityResolver.Instance,
#endif

#if !ENABLE_IL2CPP && !UNITY_WSA

            DynamicEnumResolver.Instance, // Try Enum
            DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
            DynamicUnionResolver.Instance, // Try Union(Interface)
#endif
        };
    }

    internal sealed class StandardResolverCore : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolver.Instance, // Try Object
#endif
        }).ToArray();

        StandardResolverCore()
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

    internal sealed class ContractlessStandardResolverCore : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolver.Instance, // Try Object
            DynamicContractlessObjectResolver.Instance, // Serializes keys as strings
#endif
        }).ToArray();


        ContractlessStandardResolverCore()
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

    internal sealed class StandardResolverAllowPrivateCore : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverAllowPrivateCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolverAllowPrivate.Instance, // Try Object
#endif
        }).ToArray();

        StandardResolverAllowPrivateCore()
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

    internal sealed class ContractlessStandardResolverAllowPrivateCore : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverAllowPrivateCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolverAllowPrivate.Instance, // Try Object
            DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings
#endif
        }).ToArray();


        ContractlessStandardResolverAllowPrivateCore()
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