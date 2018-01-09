using System;
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;

namespace CuteAnt.Extensions.Serialization
{
  internal sealed class TypelessCompositeResolver : IFormatterResolver
  {
    public static readonly TypelessCompositeResolver Instance = new TypelessCompositeResolver();

    static bool isFreezed = false;
    static IMessagePackFormatter[] formatters = new IMessagePackFormatter[0];
    static IFormatterResolver[] resolvers = new IFormatterResolver[0];

    TypelessCompositeResolver()
    {
    }

    public static void Register(params IFormatterResolver[] resolvers)
    {
      if (isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      TypelessCompositeResolver.resolvers = resolvers;
    }

    public static void Register(params IMessagePackFormatter[] formatters)
    {
      if (isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      TypelessCompositeResolver.formatters = formatters;
    }

    public static void Register(IMessagePackFormatter[] formatters, IFormatterResolver[] resolvers)
    {
      if (isFreezed)
      {
        throw new InvalidOperationException("Register must call on startup(before use GetFormatter<T>).");
      }

      TypelessCompositeResolver.resolvers = resolvers;
      TypelessCompositeResolver.formatters = formatters;
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
        isFreezed = true;

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
