using System;
using System.IO;
using System.Text;
using CuteAnt.IO;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class CsvMessageFormatter : MessageFormatter
  {
    protected static readonly ILogger s_logger = TraceLogger.GetLogger(typeof(CsvMessageFormatter));

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

    public override object DeepCopy(object source)
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
    public override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

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
    public override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return default(T); }

      try
      {
        return CsvSerializer.DeserializeFromStream<T>(readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return default(T);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == type) { type = value.GetType(); }
      CsvSerializer.SerializeToStream(value, writeStream);
    }

    /// <inheritdoc />
    public override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      CsvSerializer.SerializeToStream<T>(value, writeStream);
    }

    #endregion
  }
}
