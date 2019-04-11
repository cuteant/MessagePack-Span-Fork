﻿namespace MessagePack
{
    using System.Runtime.CompilerServices;
    using System.Threading;
    using MessagePack.Formatters;
    using MessagePack.Resolvers;

    /// <summary>MessagePackStandardResolver
    /// TypelessContractlessStandardResolver 和 StandardResolver 扩展性较差，无法实现较为复杂的应用
    /// MessagePackStandardResolver如何使用，参考 https://github.com/cuteant/akka.net/tree/future/src/Akka/Serialization
    /// </summary>
    public static class MessagePackStandardResolver
    {
        public static readonly IFormatterResolver Default = DefaultResolver.Instance;
        public static readonly IFormatterResolver Typeless = TypelessDefaultResolver.Instance;

        private static IFormatterResolver s_typelessObjectResolver;
        public static IFormatterResolver TypelessObjectResolver
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Volatile.Read(ref s_typelessObjectResolver) ?? MessagePack.Resolvers.TypelessObjectResolver.Instance;
        }

        public static void RegisterTypelessObjectResolver(IFormatterResolver typelessObjectResolver)
        {
            Interlocked.CompareExchange(ref s_typelessObjectResolver, typelessObjectResolver, null);
        }

        public static void RegisterTypelessObjectResolver(IFormatterResolver typelessObjectResolver, IMessagePackFormatter<object> typelessFormatter)
        {
            RegisterTypelessObjectResolver(typelessObjectResolver);
            MessagePackSerializer.Typeless.RegisterTypelessFormatter(typelessFormatter);
        }

        public static void Register(params IFormatterResolver[] resolvers)
        {
            DefaultResolverCore.Register(resolvers);
            TypelessDefaultResolverCore.Register(resolvers);
        }

        public static void Register(params IMessagePackFormatter[] formatters)
        {
            DefaultResolverCore.Register(formatters);
            TypelessDefaultResolverCore.Register(formatters);
        }

        public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            DefaultResolverCore.Register(formatters, resolvers);
            TypelessDefaultResolverCore.Register(formatters, resolvers);
        }

        public static bool TryRegister(params IFormatterResolver[] resolvers)
        {
            if (!DefaultResolverCore.TryRegister(resolvers)) { return false; }
            if (!TypelessDefaultResolverCore.TryRegister(resolvers)) { return false; }
            return true;
        }

        public static bool TryRegister(params IMessagePackFormatter[] formatters)
        {
            if (!DefaultResolverCore.TryRegister(formatters)) { return false; }
            if (!TypelessDefaultResolverCore.TryRegister(formatters)) { return false; }
            return true;
        }

        public static bool TryRegister(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
        {
            if (!DefaultResolverCore.TryRegister(formatters, resolvers)) { return false; }
            if (!TypelessDefaultResolverCore.TryRegister(formatters, resolvers)) { return false; }
            return true;
        }
    }
}
