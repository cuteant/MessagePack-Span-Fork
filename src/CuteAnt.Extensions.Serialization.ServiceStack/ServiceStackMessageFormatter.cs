using System;
using System.IO;
using System.Text;
using CuteAnt.IO;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class ServiceStackMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(ServiceStackMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly ServiceStackMessageFormatter DefaultInstance = new ServiceStackMessageFormatter();

    /// <summary>Constructor</summary>
    public ServiceStackMessageFormatter()
    {
    }

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public override object DeepCopy(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      using (var ms = MemoryStreamManager.GetStream())
      {
        JsonSerializer.SerializeToStream(source, type, ms);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return JsonSerializer.DeserializeFromStream(type, ms);
      }
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return JsonSerializer.DeserializeFromStream(type, readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    /// <inheritdoc />
    public override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return default; }

      try
      {
        return JsonSerializer.DeserializeFromStream<T>(readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default;
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.SerializeToStream(value, value.GetType(), writeStream);
    }

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == type) { type = value.GetType(); }
      JsonSerializer.SerializeToStream(value, type, writeStream);
    }

    /// <inheritdoc />
    public override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      JsonSerializer.SerializeToStream<T>(value, writeStream);
    }

    #endregion
  }
}
