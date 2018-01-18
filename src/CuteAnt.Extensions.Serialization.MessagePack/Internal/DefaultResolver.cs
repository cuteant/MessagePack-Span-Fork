using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace CuteAnt.Extensions.Serialization.Internal
{
  internal sealed class DefaultResolver : IFormatterResolver
  {
    public static readonly IFormatterResolver Instance = new DefaultResolver();

    public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(DefaultResolverCore.Instance);

    DefaultResolver()
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
        if (typeof(T) == TypeConstants.ObjectType)
        {
          // final fallback
          formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
        }
        else
        {
          formatter = DefaultResolverCore.Instance.GetFormatter<T>();
        }
      }
    }
  }

  internal static class DefaultResolverHelper
  {
    public static readonly IFormatterResolver[] DefaultResolvers = new[]
    {
      UnsafeBinaryResolver.Instance,
      NativeDateTimeResolver.Instance, // Native c# DateTime format, preserving timezone

      BuiltinResolver.Instance, // Try Builtin
      AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]

      DynamicEnumResolver.Instance, // Try Enum
      DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
      DynamicUnionResolver.Instance, // Try Union(Interface)
    };
  }

  internal sealed class DefaultResolverCore : IFormatterResolver
  {
    public static readonly IFormatterResolver Instance = new DefaultResolverCore();

    static readonly IFormatterResolver[] resolvers = DefaultResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
    {
      DynamicObjectResolverAllowPrivate.Instance, // Try Object
      DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings
    }).ToArray();


    DefaultResolverCore()
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
