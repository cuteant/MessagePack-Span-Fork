using System.Runtime.CompilerServices;
using System.Threading;
using CuteAnt;
using MessagePack.Formatters;

namespace MessagePack.Resolvers
{
  public static class MessagePackStandardResolver
  {
    public static readonly IFormatterResolver Default = DefaultResolver.Instance;
    public static readonly IFormatterResolver Typeless = TypelessDefaultResolver.Instance;

    private static IFormatterResolver s_typelessObjectResolver;
    public static IFormatterResolver TypelessObjectResolver
    {
      [MethodImpl(InlineMethod.Value)]
      get => Volatile.Read(ref s_typelessObjectResolver) ?? MessagePack.Resolvers.TypelessObjectResolver.Instance;
    }

    public static void RegisterTypelessObjectResolver(IFormatterResolver typelessObjectResolver)
    {
      Interlocked.CompareExchange(ref s_typelessObjectResolver, typelessObjectResolver, null);
    }

    public static void Register(params IFormatterResolver[] resolvers)
    {
      DefaultResolverCore.Register(resolvers);
      TypelessDefaultResolver.Register(resolvers);
    }

    public static void Register(params IMessagePackFormatter[] formatters)
    {
      DefaultResolverCore.Register(formatters);
      TypelessDefaultResolver.Register(formatters);
    }

    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      DefaultResolverCore.Register(formatters, resolvers);
      TypelessDefaultResolver.Register(formatters, resolvers);
    }
  }
}
