using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using CuteAnt;
using CuteAnt.Extensions.Serialization;
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;

namespace MessagePack.Resolvers
{
  public sealed class DefaultResolver : FormatterResolver
  {
    public static readonly DefaultResolver Instance = new DefaultResolver();

    private static IMessagePackFormatter<object> s_objectFallbackFormatter;
    public static IMessagePackFormatter<object> ObjectFallbackFormatter
    {
      [MethodImpl(InlineMethod.Value)]
      get { return Volatile.Read(ref s_objectFallbackFormatter) ?? EnsureObjectFallbackFormatterCreated(); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IMessagePackFormatter<object> EnsureObjectFallbackFormatterCreated()
    {
      Interlocked.CompareExchange(ref s_objectFallbackFormatter, new DynamicObjectTypeFallbackFormatter(DefaultResolverCore.Instance), null);
      return s_objectFallbackFormatter;
    }

    public DefaultResolver() { }

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

  internal sealed class DefaultResolverCore : FormatterResolver
  {
    public static readonly IFormatterResolver Instance = new DefaultResolverCore();

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
    };

    private const int Locked = 1;
    private const int Unlocked = 0;
    private static int s_isFreezed = Unlocked;
    private static List<IMessagePackFormatter> s_formatters;
    private static List<IFormatterResolver> s_resolvers;

    static DefaultResolverCore()
    {
      s_formatters = new List<IMessagePackFormatter>();
      s_resolvers = s_defaultResolvers.ToList();
    }

    DefaultResolverCore()
    {
    }

    public static void Register(params IFormatterResolver[] resolvers)
    {
      if (null == resolvers || resolvers.Length == 0) { return; }
      if (Locked == s_isFreezed)
      {
        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
      }

      List<IFormatterResolver> snapshot, newCache;
      do
      {
        snapshot = Volatile.Read(ref s_resolvers);
        newCache = new List<IFormatterResolver>();
        newCache.AddRange(resolvers);
        if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
      } while (!ReferenceEquals(
          Interlocked.CompareExchange(ref s_resolvers, newCache, snapshot), snapshot));
    }

    public static void Register(params IMessagePackFormatter[] formatters)
    {
      if (null == formatters || formatters.Length == 0) { return; }
      if (Locked == s_isFreezed)
      {
        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MessagePack_Register_Err);
      }

      List<IMessagePackFormatter> snapshot, newCache;
      do
      {
        snapshot = Volatile.Read(ref s_formatters);
        newCache = new List<IMessagePackFormatter>();
        newCache.AddRange(formatters);
        if (snapshot.Count > 0) { newCache.AddRange(snapshot); }
      } while (!ReferenceEquals(
          Interlocked.CompareExchange(ref s_formatters, newCache, snapshot), snapshot));
    }

    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      Register(formatters);
      Register(resolvers);
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
      }
    }
  }
}
