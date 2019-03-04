using System;
using CuteAnt;
using Newtonsoft.Json.Linq;

namespace Newtonsoft.Json.Converters
{
  /// <summary>Converts a <see cref="CombGuid"/> to and from a string.</summary>
  public sealed class CombGuidConverter : JsonConverter
  {
    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
      {
        return CombGuid.Empty;
      }
      else
      {
        JToken token = JToken.Load(reader);
        var str = token.Value<string>();
        if (CombGuid.TryParse(str, CombGuidSequentialSegmentType.Comb, out CombGuid v))
        {
          return v;
        }
        if (CombGuid.TryParse(str, CombGuidSequentialSegmentType.Guid, out v))
        {
          return v;
        }
        return CombGuid.Empty;
      }
    }

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value == null)
      {
        writer.WriteNull();
        return;
      }
      CombGuid comb = (CombGuid)value;
      writer.WriteValue(comb.ToString());
    }

    /// <summary>Determines whether this instance can convert the specified object type.</summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
    public override Boolean CanConvert(Type objectType)
    {
      return objectType == TypeConstants.CombGuidType || objectType == typeof(CombGuid?);
    }
  }
}
