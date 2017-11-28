//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using CuteAnt.Reflection;

//namespace Hyperion.Surrogates
//{
//  public sealed class TypeSurrogate : StringPayloadSurrogate
//  {
//    public static TypeSurrogate ToSurrogate(Type type)
//        => new TypeSurrogate() { S = ConvertTypeToString(type) };

//    public static Type FromSurrogate(TypeSurrogate surrogate) => ResolveTypeName(surrogate.S);

//    #region ** ConvertTypeToString **

//    private static readonly ConcurrentDictionary<Type, string> _typeKeyStringCache = new ConcurrentDictionary<Type, string>();

//    private static string ConvertTypeToString(Type t)
//    {
//      string key;
//      if (_typeKeyStringCache.TryGetValue(t, out key)) { return key; }

//#if NET40
//      var typeInfo = t;
//#else
//      var typeInfo = t.GetTypeInfo();
//#endif
//      var sb = new StringBuilder();
//      if (typeInfo.IsGenericTypeDefinition)
//      {
//        sb.Append(GetBaseTypeKey(t));
//        sb.Append('\'');
//        sb.Append(typeInfo.GetGenericArguments().Length);
//      }
//      else if (typeInfo.IsGenericType)
//      {
//        sb.Append(GetBaseTypeKey(t));
//        sb.Append('<');
//        var first = true;
//        foreach (var genericArgument in t.GetGenericArguments())
//        {
//          if (!first)
//          {
//            sb.Append(',');
//          }
//          first = false;
//          sb.Append(ConvertTypeToString(genericArgument));
//        }
//        sb.Append('>');
//      }
//      else if (t.IsArray)
//      {
//        sb.Append(ConvertTypeToString(t.GetElementType()));
//        sb.Append('[');
//        if (t.GetArrayRank() > 1)
//        {
//          sb.Append(',', t.GetArrayRank() - 1);
//        }
//        sb.Append(']');
//      }
//      else
//      {
//        sb.Append(GetBaseTypeKey(t));
//      }

//      key = sb.ToString();
//      _typeKeyStringCache[t] = key;

//      return key;
//    }

//    private static string GetBaseTypeKey(Type t)
//    {
//      var typeInfo = t.GetTypeInfo();

//      string namespacePrefix = "";
//      if ((typeInfo.Namespace != null) && !typeInfo.Namespace.StartsWith("System.") && !typeInfo.Namespace.Equals("System"))
//      {
//        namespacePrefix = typeInfo.Namespace + '.';
//      }

//      if (typeInfo.IsNestedPublic)
//      {
//        return namespacePrefix + ConvertTypeToString(typeInfo.DeclaringType) + "." + typeInfo.Name;
//      }

//      return namespacePrefix + typeInfo.Name;
//    }

//    #endregion

//    #region ** ResolveTypeName **

//    private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>(StringComparer.Ordinal);

//    private static Type ResolveTypeName(string typeName)
//    {
//      Type t;

//      if (_typeCache.TryGetValue(typeName, out t)) { return t; }

//      if (typeName[typeName.Length - 1] == ']')
//      {
//        // It's an array type declaration: elementType[,,,]
//        var j = typeName.LastIndexOf('[');
//        // The rank of the array will be the length of the string, minus the index of the [, minus 1; it's the number of commas between the [ and the ]
//        var rank = typeName.Length - j - 1;
//        var baseName = typeName.Substring(0, j);
//        var baseType = ResolveTypeName(baseName);
//        return rank == 1 ? baseType.MakeArrayType() : baseType.MakeArrayType(rank);
//      }

//      var i = typeName.IndexOf('<');
//      if (i >= 0)
//      {
//        // It's a generic type, definitionType<arg1,arg2,arg3,...>
//        var baseName = typeName.Substring(0, i) + "'";
//        var typeArgs = new List<Type>();
//        i++; // Skip the <
//        while (i < typeName.Length - 1)
//        {
//          // Get the next type argument, watching for matching angle brackets
//          int n = i;
//          int nestingDepth = 0;
//          while (n < typeName.Length - 1)
//          {
//            if (typeName[n] == '<')
//            {
//              nestingDepth++;
//            }
//            else if (typeName[n] == '>')
//            {
//              if (nestingDepth == 0)
//                break;

//              nestingDepth--;
//            }
//            else if (typeName[n] == ',')
//            {
//              if (nestingDepth == 0)
//                break;
//            }
//            n++;
//          }
//          typeArgs.Add(ResolveTypeName(typeName.Substring(i, n - i)));
//          i = n + 1;
//        }
//        var baseType = ResolveTypeName(baseName + typeArgs.Count);
//        //return baseType.MakeGenericType(typeArgs.ToArray<Type>());
//        return baseType.GetCachedGenericType(typeArgs.ToArray<Type>());
//      }

//      throw new TypeAccessException("Type string \"" + typeName + "\" cannot be resolved.");
//    }

//    #endregion
//  }
//}
