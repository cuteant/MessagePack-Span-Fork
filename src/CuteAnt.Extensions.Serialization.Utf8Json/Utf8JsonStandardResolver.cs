using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.ImmutableCollection;
using Utf8Json.Resolvers;

namespace CuteAnt.Extensions.Serialization
{
    public static class Utf8JsonStandardResolver
    {
        /// <summary>AllowPrivate:True,  ExcludeNull:True,  NameMutate:Original</summary>
        public static readonly IJsonFormatterResolver Default = AllowPrivateExcludeNullStandardResolver.Instance;
        /// <summary>AllowPrivate:True,  ExcludeNull:True,  NameMutate:CamelCase</summary>
        public static readonly IJsonFormatterResolver CamelCase = AllowPrivateExcludeNullCamelCaseStandardResolver.Instance;
        /// <summary>AllowPrivate:True,  ExcludeNull:True,  NameMutate:SnakeCase</summary>
        public static readonly IJsonFormatterResolver SnakeCase = AllowPrivateExcludeNullSnakeCaseStandardResolver.Instance;

        public static void Register(params IJsonFormatterResolver[] resolvers)
        {
            AllowPrivateExcludeNullStandardResolverCore.Register(resolvers);
            AllowPrivateExcludeNullCamelCaseStandardResolverCore.Register(resolvers);
            AllowPrivateExcludeNullSnakeCaseStandardResolverCore.Register(resolvers);
        }

        public static void Register(params IJsonFormatter[] formatters)
        {
            AllowPrivateExcludeNullStandardResolverCore.Register(formatters);
            AllowPrivateExcludeNullCamelCaseStandardResolverCore.Register(formatters);
            AllowPrivateExcludeNullSnakeCaseStandardResolverCore.Register(formatters);
        }

        public static void Register(IJsonFormatter[] formatters, IJsonFormatterResolver[] resolvers)
        {
            AllowPrivateExcludeNullStandardResolverCore.Register(formatters, resolvers);
            AllowPrivateExcludeNullCamelCaseStandardResolverCore.Register(formatters, resolvers);
            AllowPrivateExcludeNullSnakeCaseStandardResolverCore.Register(formatters, resolvers);
        }
    }

    internal static class DefaultResolverHelper
    {
        internal static readonly IJsonFormatterResolver[] CompositeResolverBase = new[]
        {
            ImmutableCollectionResolver.Instance,

            BuiltinResolver.Instance, // Builtin
            EnumResolver.Default,     // Enum(default => string)
            DynamicGenericResolver.Instance, // T[], List<T>, etc...
            AttributeFormatterResolver.Instance // [JsonFormatter]
        };
    }

    internal sealed class AllowPrivateExcludeNullStandardResolver : IJsonFormatterResolver
    {
        // configure
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullStandardResolver();

        private static IJsonFormatter<object> s_objectFallbackFormatter;
        public static IJsonFormatter<object> ObjectFallbackFormatter
        {
            [MethodImpl(InlineMethod.Value)]
            get { return Volatile.Read(ref s_objectFallbackFormatter) ?? EnsureObjectFallbackFormatterCreated(); }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IJsonFormatter<object> EnsureObjectFallbackFormatterCreated()
        {
            Interlocked.CompareExchange(ref s_objectFallbackFormatter, new DynamicObjectTypeFallbackFormatter(AllowPrivateExcludeNullStandardResolverCore.Instance), null);
            return s_objectFallbackFormatter;
        }

        AllowPrivateExcludeNullStandardResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    formatter = (IJsonFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = AllowPrivateExcludeNullStandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }
    internal sealed class AllowPrivateExcludeNullStandardResolverCore : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullStandardResolverCore();

        static readonly IJsonFormatterResolver[] s_defaultResolvers;
        private const int Locked = 1;
        private const int Unlocked = 0;
        private static int s_isFreezed = Unlocked;
        private static List<IJsonFormatter> s_formatters;
        private static List<IJsonFormatterResolver> s_resolvers;

        static AllowPrivateExcludeNullStandardResolverCore()
        {
            s_defaultResolvers = DefaultResolverHelper.CompositeResolverBase.Concat(new[] { DynamicObjectResolver.AllowPrivateExcludeNull }).ToArray();
            s_formatters = new List<IJsonFormatter>();
            s_resolvers = s_defaultResolvers.ToList();
        }
        AllowPrivateExcludeNullStandardResolverCore()
        {
        }

        public static void Register(params IJsonFormatterResolver[] resolvers)
        {
            if (null == resolvers || resolvers.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatterResolver> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_resolvers);
                newCache = new List<IJsonFormatterResolver>();
                newCache.AddRange(resolvers);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_resolvers, newCache, snapshot), snapshot));
        }

        public static void Register(params IJsonFormatter[] formatters)
        {
            if (null == formatters || formatters.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatter> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_formatters);
                newCache = new List<IJsonFormatter>();
                newCache.AddRange(formatters);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_formatters, newCache, snapshot), snapshot));
        }

        public static void Register(IJsonFormatter[] formatters, IJsonFormatterResolver[] resolvers)
        {
            Register(formatters);
            Register(resolvers);
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

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
                            formatter = (IJsonFormatter<T>)item;
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
            }
        }
    }

    internal sealed class AllowPrivateExcludeNullCamelCaseStandardResolver : IJsonFormatterResolver
    {
        // configure
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullCamelCaseStandardResolver();

        private static IJsonFormatter<object> s_objectFallbackFormatter;
        public static IJsonFormatter<object> ObjectFallbackFormatter
        {
            [MethodImpl(InlineMethod.Value)]
            get { return Volatile.Read(ref s_objectFallbackFormatter) ?? EnsureObjectFallbackFormatterCreated(); }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IJsonFormatter<object> EnsureObjectFallbackFormatterCreated()
        {
            Interlocked.CompareExchange(ref s_objectFallbackFormatter, new DynamicObjectTypeFallbackFormatter(AllowPrivateExcludeNullCamelCaseStandardResolverCore.Instance), null);
            return s_objectFallbackFormatter;
        }

        AllowPrivateExcludeNullCamelCaseStandardResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    formatter = (IJsonFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = AllowPrivateExcludeNullCamelCaseStandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }
    internal sealed class AllowPrivateExcludeNullCamelCaseStandardResolverCore : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullCamelCaseStandardResolverCore();

        private static readonly IJsonFormatterResolver[] s_defaultResolvers;
        private const int Locked = 1;
        private const int Unlocked = 0;
        private static int s_isFreezed = Unlocked;
        private static List<IJsonFormatter> s_formatters;
        private static List<IJsonFormatterResolver> s_resolvers;

        static AllowPrivateExcludeNullCamelCaseStandardResolverCore()
        {
            s_defaultResolvers = DefaultResolverHelper.CompositeResolverBase.Concat(new[] { DynamicObjectResolver.AllowPrivateExcludeNullCamelCase }).ToArray();
            s_resolvers = s_defaultResolvers.ToList();
            s_formatters = new List<IJsonFormatter>();
        }
        AllowPrivateExcludeNullCamelCaseStandardResolverCore()
        {
        }

        public static void Register(params IJsonFormatterResolver[] resolvers)
        {
            if (null == resolvers || resolvers.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatterResolver> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_resolvers);
                newCache = new List<IJsonFormatterResolver>();
                newCache.AddRange(resolvers);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_resolvers, newCache, snapshot), snapshot));
        }

        public static void Register(params IJsonFormatter[] formatters)
        {
            if (null == formatters || formatters.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatter> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_formatters);
                newCache = new List<IJsonFormatter>();
                newCache.AddRange(formatters);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_formatters, newCache, snapshot), snapshot));
        }

        public static void Register(IJsonFormatter[] formatters, IJsonFormatterResolver[] resolvers)
        {
            Register(formatters);
            Register(resolvers);
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

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
                            formatter = (IJsonFormatter<T>)item;
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
            }
        }
    }

    internal sealed class AllowPrivateExcludeNullSnakeCaseStandardResolver : IJsonFormatterResolver
    {
        // configure
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullSnakeCaseStandardResolver();

        private static IJsonFormatter<object> s_objectFallbackFormatter;
        public static IJsonFormatter<object> ObjectFallbackFormatter
        {
            [MethodImpl(InlineMethod.Value)]
            get { return Volatile.Read(ref s_objectFallbackFormatter) ?? EnsureObjectFallbackFormatterCreated(); }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IJsonFormatter<object> EnsureObjectFallbackFormatterCreated()
        {
            Interlocked.CompareExchange(ref s_objectFallbackFormatter, new DynamicObjectTypeFallbackFormatter(AllowPrivateExcludeNullSnakeCaseStandardResolverCore.Instance), null);
            return s_objectFallbackFormatter;
        }

        AllowPrivateExcludeNullSnakeCaseStandardResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    formatter = (IJsonFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = AllowPrivateExcludeNullSnakeCaseStandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }
    internal sealed class AllowPrivateExcludeNullSnakeCaseStandardResolverCore : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new AllowPrivateExcludeNullSnakeCaseStandardResolverCore();

        private static readonly IJsonFormatterResolver[] s_defaultResolvers;
        private const int Locked = 1;
        private const int Unlocked = 0;
        private static int s_isFreezed = Unlocked;
        private static List<IJsonFormatter> s_formatters;
        private static List<IJsonFormatterResolver> s_resolvers;

        static AllowPrivateExcludeNullSnakeCaseStandardResolverCore()
        {
            s_defaultResolvers = DefaultResolverHelper.CompositeResolverBase.Concat(new[] { DynamicObjectResolver.AllowPrivateExcludeNullSnakeCase }).ToArray();
            s_resolvers = s_defaultResolvers.ToList();
            s_formatters = new List<IJsonFormatter>();
        }
        AllowPrivateExcludeNullSnakeCaseStandardResolverCore()
        {
        }

        public static void Register(params IJsonFormatterResolver[] resolvers)
        {
            if (null == resolvers || resolvers.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatterResolver> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_resolvers);
                newCache = new List<IJsonFormatterResolver>();
                newCache.AddRange(resolvers);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_resolvers, newCache, snapshot), snapshot));
        }

        public static void Register(params IJsonFormatter[] formatters)
        {
            if (null == formatters || formatters.Length == 0) { return; }
            if (Locked == s_isFreezed)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
            }

            List<IJsonFormatter> snapshot, newCache;
            do
            {
                snapshot = Volatile.Read(ref s_formatters);
                newCache = new List<IJsonFormatter>();
                newCache.AddRange(formatters);
                if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref s_formatters, newCache, snapshot), snapshot));
        }

        public static void Register(IJsonFormatter[] formatters, IJsonFormatterResolver[] resolvers)
        {
            Register(formatters);
            Register(resolvers);
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

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
                            formatter = (IJsonFormatter<T>)item;
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
            }
        }
    }
}
