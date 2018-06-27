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
    private static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(ServiceStackMessageFormatter));

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
    public sealed override object DeepCopyObject(object source)
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
    public sealed override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (null == readStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.readStream); }

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
    public sealed override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (null == readStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.readStream); }

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
    public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      JsonSerializer.SerializeToStream(value, value.GetType(), writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      if (null == type) { type = value.GetType(); }
      JsonSerializer.SerializeToStream(value, type, writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      JsonSerializer.SerializeToStream<T>(value, writeStream);
    }

    #endregion
  }
}
