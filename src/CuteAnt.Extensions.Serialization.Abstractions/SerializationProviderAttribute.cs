using System;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>SerializationProviderType</summary>
  public enum SerializationProviderType
  {
    /// <summary>Auto</summary>
    Auto,

    /// <summary>Protobuf-Net</summary>
    Protobuf,

    /// <summary>Newtonsoft.Json</summary>
    Json,

    /// <summary>Newtonsoft.Bson</summary>
    Bson,

    /// <summary>ServiceStack.Json</summary>
    ServiceStackJson,

    /// <summary>ServiceStack.Jsv</summary>
    ServiceStackJsv,

    /// <summary>ServiceStack.Csv</summary>
    ServiceStackCsv,

    /// <summary>Jil</summary>
    JilJson,

    /// <summary>Ms.DataContractJson</summary>
    DataContractJson,

#if !NETSTANDARD
    /// <summary>Ms.BinaryFormatter</summary>
    BinaryFormatter,
#endif
    /// <summary>Ms.XmlFormatter</summary>
    XmlFormatter,

    /// <summary>Hyperion</summary>
    Hyperion,

    /// <summary>Bond</summary>
    Bond,

    /// <summary>Google.Protobuf</summary>
    GoogleProtobuf,
  }

  /// <summary>SerializationProviderAttribute</summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class SerializationProviderAttribute : Attribute
  {
    /// <summary>Provider</summary>
    public SerializationProviderType Provider { get; }

    /// <summary>Constructor</summary>
    /// <param name="provider"></param>
    public SerializationProviderAttribute(SerializationProviderType provider)
    {
      Provider = provider;
    }
  }
}
