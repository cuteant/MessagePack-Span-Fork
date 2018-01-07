using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>IJsonMessageFormatter</summary>
  public interface IJsonMessageFormatter : IMessageFormatter
  {
    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    JsonSerializerSettings DefaultSerializerSettings { get; set; }

    /// <summary>Gets or sets the default <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</summary>
    JsonSerializerSettings DefaultDeserializerSettings { get; set; }

    /// <summary>Gets or sets a value indicating whether to indent elements when writing data.</summary>
    [Obsolete("Indent is obsolete. Use Formatting instead.")]
    bool Indent { get; set; }
    /// <summary>Indicates how JSON text output is formatted.</summary>
    Formatting? JsonFormatting { get; set; }

    /// <summary>Gets or sets the maximum depth allowed by this formatter.</summary>
    /// <remarks>Any override must call the base getter and setter. The setter may be called before a derived class
    /// constructor runs, so any override should be very careful about using derived class state.</remarks>
    Int32 MaxDepth { get; set; }

    bool IsStrictMode { get; set; }

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the default settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    JsonSerializerSettings CreateDefaultSerializerSettings();

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the limit properties settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    JsonSerializerSettings CreateLimitPropsSerializerSettings(ICollection<String> limitProps);

    /// <summary>Creates a <see cref="JsonSerializerSettings"/> instance with the property mappings settings used by the <see cref="BaseJsonMessageFormatter"/>.</summary>
    /// <param name="propertyMappings"></param>
    /// <param name="limitProps"></param>
    /// <returns></returns>
    JsonSerializerSettings CreatePropertyMappingSerializerSettings(IDictionary<String, String> propertyMappings, Boolean limitProps = false);

    /// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    /// from the specified <paramref name="readStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    /// <returns>The <see cref="object"/> instance that has been read.</returns>
    Object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings);

    /// <summary>Called during deserialization to get the <see cref="JsonReader"/>.</summary>
    /// <remarks>Public for delegating wrappers of this class.</remarks>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="charPool">The reader's character buffer pool.</param>
    /// <returns>The <see cref="JsonWriter"/> used during deserialization.</returns>
    JsonReader CreateJsonReader(Type type, Stream readStream, Encoding effectiveEncoding, IArrayPool<char> charPool);

    /// <summary>Called during serialization to write an object of the specified <paramref name="type"/>
    /// to the specified <paramref name="writeStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="value">The object to write.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
    void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, JsonSerializerSettings serializerSettings);

    /// <summary>Called during serialization to get the <see cref="JsonWriter"/>.</summary>
    /// <remarks>Public for delegating wrappers of this class.  Expected to be called only from</remarks>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="charPool">The writer's character array pool.</param>
    /// <returns>The <see cref="JsonWriter"/> used during serialization.</returns>
    JsonWriter CreateJsonWriter(Type type, Stream writeStream, Encoding effectiveEncoding, IArrayPool<char> charPool);
  }
}
