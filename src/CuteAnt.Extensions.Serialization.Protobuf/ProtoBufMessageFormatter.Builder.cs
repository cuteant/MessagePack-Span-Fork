using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CuteAnt.Extensions.Serialization
{
  partial class ProtoBufMessageFormatter
  {
    private const BindingFlags Flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly Dictionary<Type, HashSet<Type>> s_subTypes = new Dictionary<Type, HashSet<Type>>();
    private static readonly ConcurrentHashSet<Type> s_builtTypes = new ConcurrentHashSet<Type>();
    private static readonly Type s_objectType = typeof(object);

    /// <summary>Build the ProtoBuf serializer from the generic <see cref="Type">type</see>.</summary>
    /// <typeparam name="T">The type of build the serializer for.</typeparam>
    public static void Build<T>() => Build(typeof(T));

    /// <summary>Build the ProtoBuf serializer from the data's <see cref="Type">type</see>.</summary>
    /// <typeparam name="T">The type of build the serializer for.</typeparam>
    /// <param name="data">The data who's type a serializer will be made.</param>
    public static void Build<T>(T data) => Build(typeof(T));

    /// <summary>Build the ProtoBuf serializer for the <see cref="Type">type</see>.</summary>
    /// <param name="type">The type of build the serializer for.</param>
    public static void Build(Type type)
    {
      if (s_builtTypes.Contains(type)) { return; }

      lock (type)
      {
        if (s_model.CanSerialize(type))
        {
          if (type.IsGenericType)
          {
            BuildGenerics(type);
          }

          return;
        }

        var meta = s_model.Add(type, false);
        var fields = GetFields(type);

        meta.Add(fields.Select(m => m.Name).ToArray());
        meta.UseConstructor = false;

        BuildBaseClasses(type);
        BuildGenerics(type);

        foreach (var memberType in fields.Select(f => f.FieldType).Where(t => !t.IsPrimitive))
        {
          Build(memberType);
        }

        s_builtTypes.TryAdd(type);
      }
    }

    /// <summary>Gets the fields for a type.</summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private static FieldInfo[] GetFields(Type type)
    {
      return type.GetFields(Flags);
    }

    /// <summary>Builds the base class serializers for a type.</summary>
    /// <param name="type">The type.</param>
    private static void BuildBaseClasses(Type type)
    {
      var baseType = type.BaseType;
      var inheritingType = type;


      while (baseType != null && baseType != s_objectType)
      {
        HashSet<Type> baseTypeEntry;

        if (!s_subTypes.TryGetValue(baseType, out baseTypeEntry))
        {
          baseTypeEntry = new HashSet<Type>();
          s_subTypes.Add(baseType, baseTypeEntry);
        }

        if (!baseTypeEntry.Contains(inheritingType))
        {
          Build(baseType);
          s_model[baseType].AddSubType(baseTypeEntry.Count + 500, inheritingType);
          baseTypeEntry.Add(inheritingType);
        }

        inheritingType = baseType;
        baseType = baseType.BaseType;
      }
    }

    /// <summary>Builds the serializers for the generic parameters for a given type.</summary>
    /// <param name="type">The type.</param>
    private static void BuildGenerics(Type type)
    {
      if (type.IsGenericType || (type.BaseType != null && type.BaseType.IsGenericType))
      {
        var generics = type.IsGenericType ? type.GetGenericArguments() : type.BaseType.GetGenericArguments();

        foreach (var generic in generics)
        {
          Build(generic);
        }
      }
    }
  }
}
