using System;
using System.IO;
using System.Text;
using CuteAnt.IO;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class CsvMessageFormatter : MessageFormatter
  {
    private static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(CsvMessageFormatter));

    /// <summary>The default singlegton instance</summary>
    public static readonly CsvMessageFormatter DefaultInstance = new CsvMessageFormatter();

    /// <summary>Constructor</summary>
    public CsvMessageFormatter()
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
        CsvSerializer.SerializeToStream(source, ms);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return CsvSerializer.DeserializeFromStream(type, ms);
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
        return CsvSerializer.DeserializeFromStream(type, readStream);
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

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return default(T); }

      try
      {
        return CsvSerializer.DeserializeFromStream<T>(readStream);
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

      CsvSerializer.SerializeToStream(value, writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      CsvSerializer.SerializeToStream(value, writeStream);
    }

    /// <inheritdoc />
    public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (null == writeStream) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writeStream); }

      CsvSerializer.SerializeToStream<T>(value, writeStream);
    }

    #endregion
  }
}
