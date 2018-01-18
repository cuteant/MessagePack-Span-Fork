using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace CuteAnt.Extensions.Serialization.Internal
{
  internal sealed class TypelessDefaultResolver : IFormatterResolver
  {
    public static readonly IFormatterResolver Instance = new TypelessDefaultResolver();

    static readonly IFormatterResolver[] resolvers = new[]
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
      TypelessObjectResolver.Instance
    };

    TypelessDefaultResolver()
    {
    }

    public IMessagePackFormatter<T> GetFormatter<T>()
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
