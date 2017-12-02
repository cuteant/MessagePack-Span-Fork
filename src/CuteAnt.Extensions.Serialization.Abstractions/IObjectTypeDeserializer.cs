using System;
using System.Collections.Generic;

namespace CuteAnt.Extensions.Serialization
{
  public interface IObjectTypeDeserializer
  {
    TValue Deserialize<TValue>(IDictionary<string, object> dictionary);
    TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key);
    TValue Deserialize<TValue>(IDictionary<string, object> dictionary, string key, TValue defaultValue);


    TValue Deserialize<TValue>(object value);
    TValue Deserialize<TValue>(object value, TValue defaultValue);
    object Deserialize(object value, Type objectType, bool allowNull = false);
  }
}