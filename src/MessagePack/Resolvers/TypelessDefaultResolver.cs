namespace MessagePack.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using MessagePack.Formatters;
#if DEPENDENT_ON_CUTEANT
    using CuteAnt.Reflection;
#else
    using MessagePack.Internal;
#endif

    /// <summary>TypelessDefaultResolver 使用详情，参考 https://github.com/cuteant/akka.net/tree/future/src/Akka/Serialization </summary>
    public class TypelessDefaultResolver : FormatterResolver
    {
        public static readonly TypelessDefaultResolver Instance = new TypelessDefaultResolver();

        public TypelessDefaultResolver() { }

        public sealed override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return TypelessDefaultResolverFormatterCache<T>.Formatter;
        }
    }

    internal static class TypelessDefaultResolverFormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T> Formatter;

        static TypelessDefaultResolverFormatterCache()
        {
            Formatter = TypelessDefaultResolverCore.Instance.GetFormatter<T>();
        }
    }

    internal sealed class TypelessDefaultResolverCore : FormatterResolver
    {
        public static readonly IFormatterResolver Instance;

        private static readonly IFormatterResolver[] s_defaultResolvers;

        private const int Locked = 1;
        private const int Unlocked = 0;
        private static int s_isFreezed = Unlocked;
        private static List<IMessagePackFormatter> s_formatters;
        private static List<IFormatterResolver> s_resolvers;

        static TypelessDefaultResolverCore()
        {
            s_defaultResolvers = new IFormatterResolver[]
            {
                NativeDateTimeResolver.Instance, // Native c# DateTime format, preserving timezone

                BuiltinResolver.Instance, // Try Builtin
                AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]

                DynamicEnumResolver.Instance, // Try Enum
                DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
                DynamicUnionResolver.Instance, // Try Union(Interface)

                DynamicObjectResolverAllowPrivate.Instance, // Try Object
                DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings

                //TypelessObjectResolver.Instance
            };

            s_formatters = new List<IMessagePackFormatter>();
            s_resolvers = s_defaultResolvers.ToList();

            Instance = new TypelessDefaultResolverCore();
        }

        public TypelessDefaultResolverCore() { }

        public static bool TryRegister(params IFormatterResolver[] resolvers)
        {
            if (null == resolvers || 0u >= (uint)resolvers.Length) { return false; }
            if (Locked == Volatile.Read(ref s_isFreezed)) { return false; }

            List<IFormatterResolver> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_resolvers);
                newCache = new List<IFormatterResolver>();
                newCache.AddRange(resolvers);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_resolvers, newCache, snapshot), snapshot));
            return true;
        }

        public static void Register(params IFormatterResolver[] resolvers)
        {
            if (null == resolvers || 0u >= (uint)resolvers.Length) { return; }

            if (TryRegister(resolvers)) { return; }
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
        }

        public static bool TryRegister(params IMessagePackFormatter[] formatters)
        {
            if (null == formatters || 0u >= (uint)formatters.Length) { return false; }
            if (Locked == Volatile.Read(ref s_isFreezed)) { return false; }

            List<IMessagePackFormatter> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_formatters);
                newCache = new List<IMessagePackFormatter>();
                newCache.AddRange(formatters);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_formatters, newCache, snapshot), snapshot));
            return true;
        }

        public static void Register(params IMessagePackFormatter[] formatters)
        {
            if (null == formatters || 0u >= (uint)formatters.Length) { return; }

            if (TryRegister(formatters)) { return; }
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
        }

        public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            Register(formatters);
            Register(resolvers);
        }

        public static bool TryRegister(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            if (!TryRegister(formatters)) { return false; }
            if (!TryRegister(resolvers)) { return false; }
            return true;
        }

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                Interlocked.CompareExchange(ref s_isFreezed, Locked, Unlocked);

                var formatters = Volatile.Read(ref s_formatters);
                foreach (var item in formatters)
                {
                    foreach (var implInterface in item.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        if (implInterface.IsGenericType && implInterface.GenericTypeArguments[0] == typeof(T))
                        {
                            Formatter = (IMessagePackFormatter<T>)item;
                            return;
                        }
                    }
                }

                var resolvers = Volatile.Read(ref s_resolvers);
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }

                {
                    var f = MessagePackStandardResolver.TypelessObjectResolver.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }

                Formatter = ActivatorUtils.FastCreateInstance<IMessagePackFormatter<T>>(typeof(DynamicProxyFormatter<>).GetCachedGenericType(typeof(T)));
            }
        }
    }
}
