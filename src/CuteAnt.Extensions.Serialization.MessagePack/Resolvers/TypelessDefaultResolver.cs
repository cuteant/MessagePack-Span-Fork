using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;

namespace MessagePack.Resolvers
{
  public sealed class TypelessDefaultResolver : FormatterResolver
  {
    public static readonly IFormatterResolver Instance = new TypelessDefaultResolver();

    private static readonly IFormatterResolver[] s_defaultResolvers = new IFormatterResolver[]
    {
      ImmutableCollectionResolver.Instance,

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
    private static IMessagePackFormatter[] s_formatters = new IMessagePackFormatter[0];
    private static IFormatterResolver[] s_resolvers = s_defaultResolvers;

    public TypelessDefaultResolver() { }

    public static void Register(params IFormatterResolver[] resolvers)
    {
      if (null == resolvers || resolvers.Length == 0) { return; }
      if (Locked == s_isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      Interlocked.Exchange(ref s_resolvers, resolvers.Concat(s_defaultResolvers).ToArray());
    }

    public static void Register(params IMessagePackFormatter[] formatters)
    {
      if (null == formatters || formatters.Length == 0) { return; }
      if (Locked == s_isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      Interlocked.Exchange(ref s_formatters, formatters);
    }

    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      if (Locked == s_isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      if (formatters != null && formatters.Length > 0)
      {
        Interlocked.Exchange(ref s_formatters, formatters);
      }
      if (resolvers != null && resolvers.Length > 0)
      {
        Interlocked.Exchange(ref s_resolvers, resolvers.Concat(s_defaultResolvers).ToArray());
      }
    }

    public override IDictionary<string, object> Context => new Dictionary<string, object>(StringComparer.Ordinal);

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

        foreach (var item in s_formatters)
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

        foreach (var item in s_resolvers)
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
      }
    }
  }
}
