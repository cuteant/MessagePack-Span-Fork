using System;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>IStringSerializer</summary>
  public interface IStringSerializer
  {
    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedText">The serialized text.</param>
    T DeserializeFromString<T>(string serializedText);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="serializedText">The serialized text.</param>
    /// <param name="expectedType">The type that the deserializer will expect</param>
    object DeserializeFromString(string serializedText, Type expectedType);

    //DateTime 

    /// <summary>Serializes the specified item.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    string SerializeToString<T>(T item);

    /// <summary>Serializes the specified item.</summary>
    /// <param name="item">The item.</param>
    /// <param name="expectedType">The type that should be deserialized</param>
    /// <returns></returns>
    string SerializeToString(object item, Type expectedType);
  }
}