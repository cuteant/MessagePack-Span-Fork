using System;
using System.Collections.Concurrent;
using System.Data;
using System.Text;
using CuteAnt.Buffers;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class JsvMessageFormatter : MessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public static readonly JsvMessageFormatter DefaultInstance = new JsvMessageFormatter();

    /// <summary>Constructor</summary>
    public JsvMessageFormatter()
    {
    }

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type) => true;

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      try
      {
        return TypeSerializer.DeserializeFromStream(type, readStream);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    /// <inheritdoc />
    public override T ReadFromStream<T>(BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return default(T); }

      try
      {
        return TypeSerializer.DeserializeFromStream<T>(readStream);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return default(T);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      TypeSerializer.SerializeToStream(value, type, writeStream);
    }

    /// <inheritdoc />
    public override void WriteToStream<T>(T value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      TypeSerializer.SerializeToStream<T>(value, writeStream);
    }

    #endregion
  }
}
