using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CuteAnt.Reflection;
using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
    public sealed class TypelessDefaultResolver : FormatterResolver
    {
        public static readonly IFormatterResolver Instance = new TypelessDefaultResolver();

        private static readonly IFormatterResolver[] s_defaultResolvers = new IFormatterResolver[]
        {
            UnsafeBinaryResolver.Instance,
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

        private const int Locked = 1;
        private const int Unlocked = 0;
        private static int s_isFreezed = Unlocked;
        private static List<IMessagePackFormatter> s_formatters;
        private static List<IFormatterResolver> s_resolvers;

        static TypelessDefaultResolver()
        {
            s_formatters = new List<IMessagePackFormatter>();
            s_resolvers = s_defaultResolvers.ToList();
        }

        public TypelessDefaultResolver() { }

        public static bool TryRegister(params IFormatterResolver[] resolvers)
        {
            if (null == resolvers || resolvers.Length == 0) { return false; }
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
            if (null == resolvers || resolvers.Length == 0) { return; }

            if (TryRegister(resolvers)) { return; }
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
        }

        public static bool TryRegister(params IMessagePackFormatter[] formatters)
        {
            if (null == formatters || formatters.Length == 0) { return false; }
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
            if (null == formatters || formatters.Length == 0) { return; }

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

        public override IDictionary<string, object> Context { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

        public override IDictionary<int, object> Context2 { get; } = new Dictionary<int, object>();

        public override IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                Interlocked.CompareExchange(ref s_isFreezed, Locked, Unlocked);

                var formatters = Volatile.Read(ref s_formatters);
                foreach (var item in formatters)
                {
                    foreach (var implInterface in item.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        var ti = implInterface.GetTypeInfo();
                        if (ti.IsGenericType && ti.GenericTypeArguments[0] == typeof(T))
                        {
                            formatter = (IMessagePackFormatter<T>)item;
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
                        formatter = f;
                        return;
                    }
                }

                {
                    var f = MessagePackStandardResolver.TypelessObjectResolver.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }

                formatter = ActivatorUtils.FastCreateInstance<IMessagePackFormatter<T>>(typeof(DynamicProxyFormatter<>).GetCachedGenericType(typeof(T)));
            }
        }
    }
}
