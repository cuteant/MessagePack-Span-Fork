using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.IO;
using Hyperion;
using Hyperion.SerializerFactories;
using Microsoft.Extensions.Logging;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class HyperionMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(HyperionMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly HyperionMessageFormatter DefaultInstance = new HyperionMessageFormatter();

    internal readonly Hyperion.Serializer _serializer;
    internal readonly Hyperion.Serializer _copier;

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

    /// <summary>SerializerOptions</summary>
    public SerializerOptions SerializerOptions => _serializer.Options;

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    public override object DeepCopy(object source)
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

    /// <inheritdoc />
    public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

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

#if !NET40
    /// <inheritdoc />
    public override async Task<Object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      await TaskConstants.Completed;
      return ReadFromStream(type, readStream, effectiveEncoding);
    }
#endif

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value) { return; }
      _serializer.Serialize(value, writeStream);
    }

#if !NET40
    /// <inheritdoc />
    public override async Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      WriteToStream(type, value, writeStream, effectiveEncoding);
      await TaskConstants.Completed;
    }
#endif
  }
}
