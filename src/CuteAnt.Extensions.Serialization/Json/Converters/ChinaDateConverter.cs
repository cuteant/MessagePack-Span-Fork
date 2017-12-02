using System;
using System.Globalization;

namespace Newtonsoft.Json.Converters
{
  /// <summary>Converts a <see cref="DateTime"/> to and from the ISO 8601 date format (e.g. 2008-04-12).</summary>
  public sealed class ChinaDateConverter : DateTimeConverterBase
  {
    private static readonly IsoDateTimeConverter s_dtConverter = new IsoDateTimeConverter { Culture = CultureInfo.InvariantCulture, DateTimeFormat = "yyyy-MM-dd" };

    /// <summary>Reads the JSON representation of the object.</summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
    {
      return s_dtConverter.ReadJson(reader, objectType, existingValue, serializer);
    }

    /// <summary>Writes the JSON representation of the object.</summary>
    /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
    {
      s_dtConverter.WriteJson(writer, value, serializer);
    }
  }
}
