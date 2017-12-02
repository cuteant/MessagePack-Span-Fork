// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CuteAnt.Collections;
using CuteAnt.Extensions.Serialization.Json.Utilities;
using CuteAnt.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CuteAnt.Extensions.Serialization
{
  #region -- class JsonContractResolver --

  /// <summary>Represents the default <see cref="IContractResolver"/> used by <see cref="BaseJsonMessageFormatter"/>.</summary>
  public class JsonContractResolver : DefaultContractResolver
  {
    public JsonContractResolver() : base() { }

    protected override JsonContract CreateContract(Type objectType)
    {
      if (JsonContractResolverHelper.IsSupportedType(objectType))
      {
        return base.CreateArrayContract(objectType);
      }

      return base.CreateContract(objectType);
    }
  }

  #endregion

  #region == class JsonContractResolverHelper ==

  internal sealed class JsonContractResolverHelper
  {
    internal static readonly DictionaryCache<Type, bool> s_supportedTypeInfoSet;

    static JsonContractResolverHelper()
    {
      s_supportedTypeInfoSet = new DictionaryCache<Type, bool>(DictionaryCacheConstants.SIZE_MEDIUM);
    }

    public static bool IsSupportedType(Type objectType)
    {
      return s_supportedTypeInfoSet.GetItem(objectType, s_isSupportedTypeFunc);
    }

    private static readonly Func<Type, bool> s_isSupportedTypeFunc = IsSupportedTypeInternal;
    private static bool IsSupportedTypeInternal(Type underlyingType)
    {
      if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out Type genericCollectionDefinitionType))
      {
        var keyType = genericCollectionDefinitionType.GetGenericArguments()[0];
        return keyType == TypeConstants.CombGuidType;
      }
      return false;
    }
  }

  #endregion

  #region -- class JsonLimitPropsContractResolver --

  /// <summary>Represents the default <see cref="IContractResolver"/> used by <see cref="BaseJsonMessageFormatter"/>.</summary>
  public class JsonLimitPropsContractResolver : JsonContractResolver
  {
    private HashSet<string> _props = null;

    /// <summary>Initializes a new instance of the <see cref="JsonLimitPropsContractResolver" /> class.</summary>
    /// <param name="props"></param>
    public JsonLimitPropsContractResolver(ICollection<string> props)
    {
      if (null == props) { throw new ArgumentNullException(nameof(props)); }
      _props = new HashSet<string>(props, StringComparer.Ordinal);
    }

    /// <summary>Creates properties for the given <see cref="JsonContract"/>.</summary>
    /// <param name="type">The type to create properties for.</param>
    /// <param name="memberSerialization">The member serialization mode for the type.</param>
    /// <returns>Properties for the given <see cref="JsonContract"/>.</returns>
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      var list = base.CreateProperties(type, memberSerialization);

      // 只保留清單有列出的屬性
      return list.Where(p => _props.Contains(p.PropertyName)).ToList();
    }
  }

  #endregion

  #region -- class JsonCamelCasePropertyNamesContractResolver --

  /// <summary>Resolves member mappings for a type, camel casing property names.</summary>
  public class JsonCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
  {
    /// <summary>Initializes a new instance of the <see cref="JsonCamelCasePropertyNamesContractResolver"/> class.</summary>
    public JsonCamelCasePropertyNamesContractResolver()
      : base()
    {
    }

    protected override JsonContract CreateContract(Type objectType)
    {
      if (JsonContractResolverHelper.IsSupportedType(objectType))
      {
        return base.CreateArrayContract(objectType);
      }

      return base.CreateContract(objectType);
    }
  }

  #endregion

  #region -- class JsonCamelCasePropertyNamesLimitPropsContractResolver --

  /// <summary>Represents the default <see cref="IContractResolver"/> used by <see cref="BaseJsonMessageFormatter"/>.</summary>
  public class JsonCamelCasePropertyNamesLimitPropsContractResolver : JsonCamelCasePropertyNamesContractResolver
  {
    private HashSet<string> _props = null;

    /// <summary>Initializes a new instance of the <see cref="JsonCamelCasePropertyNamesLimitPropsContractResolver" /> class.</summary>
    /// <param name="props"></param>
    public JsonCamelCasePropertyNamesLimitPropsContractResolver(ICollection<string> props)
    {
      if (null == props) { throw new ArgumentNullException(nameof(props)); }
      _props = new HashSet<string>(props, StringComparer.Ordinal);
    }

    /// <summary>Creates properties for the given <see cref="JsonContract"/>.</summary>
    /// <param name="type">The type to create properties for.</param>
    /// <param name="memberSerialization">The member serialization mode for the type.</param>
    /// <returns>Properties for the given <see cref="JsonContract"/>.</returns>
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      var list = base.CreateProperties(type, memberSerialization);

      // 只保留清單有列出的屬性
      return list.Where(p => _props.Contains(p.PropertyName)).ToList();
    }
  }

  #endregion

  #region -- class JsonPropertyMappingContractResolver --

  /// <summary>Represents the default <see cref="IContractResolver"/> used by <see cref="BaseJsonMessageFormatter"/>.</summary>
  public class JsonPropertyMappingContractResolver : JsonContractResolver
  {
    private bool m_limitProps;
    private HashSet<string> m_limitProperties;
    private IDictionary<string, string> m_propertyMappings = null;
    /// <summary>PropertyMappings</summary>
    public IDictionary<string, string> PropertyMappings { get { return m_propertyMappings; } }

    /// <summary>Initializes a new instance of the <see cref="JsonPropertyMappingContractResolver" /> class.</summary>
    /// <param name="propertyMappings"></param>
    /// <param name="limitProps"></param>
    public JsonPropertyMappingContractResolver(IDictionary<string, string> propertyMappings, bool limitProps)
    {
      if (null == propertyMappings) { throw new ArgumentNullException(nameof(propertyMappings)); }
      m_propertyMappings = new Dictionary<string, string>(propertyMappings, StringComparer.Ordinal);
      m_limitProps = limitProps;
      m_limitProperties = new HashSet<string>(propertyMappings.Values, StringComparer.Ordinal);
    }

    /// <summary>Resolves the name of the property.</summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The property name.</returns>
    protected override string ResolvePropertyName(string propertyName)
    {
      return m_propertyMappings.TryGetValue(propertyName, out string resolveName) ? resolveName : propertyName;
    }

    /// <summary>Creates properties for the given <see cref="JsonContract"/>.</summary>
    /// <param name="type">The type to create properties for.</param>
    /// <param name="memberSerialization">The member serialization mode for the type.</param>
    /// <returns>Properties for the given <see cref="JsonContract"/>.</returns>
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      var list = base.CreateProperties(type, memberSerialization);

      if (!m_limitProps) { return list; }

      // 只保留清單有列出的屬性
      return list.Where(p => m_limitProperties.Contains(p.PropertyName)).ToList();
    }
  }

  #endregion
}
