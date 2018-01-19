using CuteAnt.Extensions.Serialization.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace CuteAnt.Extensions.Serialization
{
  public static class MessagePackStandardResolver
  {
    public static readonly IFormatterResolver Default = DefaultResolver.Instance;
    public static readonly IFormatterResolver Typeless = TypelessDefaultResolver.Instance;


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
