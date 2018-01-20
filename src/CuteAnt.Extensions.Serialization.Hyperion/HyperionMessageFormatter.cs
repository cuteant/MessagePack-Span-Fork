using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CuteAnt.IO;
using Hyperion;
using Hyperion.SerializerFactories;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class HyperionMessageFormatter : MessageFormatter
  {
    #region @@ Fields @@

    private static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(HyperionMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly HyperionMessageFormatter DefaultInstance = new HyperionMessageFormatter();

    private readonly Hyperion.Serializer _serializer;
    private readonly Hyperion.Serializer _copier;

    #endregion

    #region @@ Properties @@

    /// <summary>SerializerOptions</summary>
    public SerializerOptions SerializerOptions => _serializer.Options;

    #endregion

    #region @@ Constructos @@

    /// <summary>Constructor</summary>
    public HyperionMessageFormatter()
      : this(surrogates: null)
    {
    }

    /// <summary>Constructor</summary>
    public HyperionMessageFormatter(IEnumerable<Surrogate> surrogates,
      IEnumerable<ValueSerializerFactory> serializerFactories = null,
      IEnumerable<Type> knownTypes = null, bool ignoreISerializable = false)
    {
      var options = new SerializerOptions(
          versionTolerance: true,
          preserveObjectReferences: true,
          surrogates: surrogates,
          serializerFactories: serializerFactories,
          knownTypes: knownTypes,
          ignoreISerializable: ignoreISerializable
      );
      _serializer = new Hyperion.Serializer(options);
      options = new SerializerOptions(
          versionTolerance: false,
          preserveObjectReferences: true,
          surrogates: surrogates,
          serializerFactories: serializerFactories,
          knownTypes: knownTypes,
          ignoreISerializable: ignoreISerializable
      );
      _copier = new Hyperion.Serializer(options);
    }

    #endregion

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public sealed override object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      using (var ms = MemoryStreamManager.GetStream())
      {
        _copier.Serialize(source, ms);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return _copier.Deserialize(ms);
      }
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public sealed override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return (T)_serializer.Deserialize(readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }

    /// <inheritdoc />
    public sealed override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return _serializer.Deserialize(readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      _serializer.Serialize(value, writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      _serializer.Serialize(value, writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      _serializer.Serialize(value, writeStream);
    }

    #endregion
  }
}
