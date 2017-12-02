using System;
using System.Text;
using CuteAnt.Buffers;
using CuteAnt.Runtime;
using Microsoft.Extensions.Logging;
using ServiceStack.Text;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class JsvMessageSerializer : JsvMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly JsvMessageSerializer DefaultInstance = new JsvMessageSerializer();

    /// <summary>Constructor</summary>
    public JsvMessageSerializer()
      : base()
    {
    }

    #region -- ReadFromStream --

    /// <inheritdoc />
    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

      object obj;
      try
      {
        var len = readStream.Read7BitEncodedInt();
        if (len <= SIZE_ZERO) { return GetDefaultValueForType(type); }
        byte[] buffer = null;
        BufferManagerStreamReader binReader = null;
        var bufferManager = BufferManager.GlobalManager;
        try
        {
          buffer = BufferManager.GlobalManager.TakeBuffer(len);
          readStream.Read(buffer, 0, len);
          binReader = BufferManagerStreamReaderManager.Take();
          binReader.Reinitialize(buffer, 0, len, bufferManager);
          obj = TypeSerializer.DeserializeFromStream(type, binReader);
        }
        finally
        {
          if (binReader != null) { BufferManagerStreamReaderManager.Return(binReader); }
          if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        obj = GetDefaultValueForType(type);
      }
      return obj;
    }

    /// <inheritdoc />
    public override T ReadFromStream<T>(BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return default(T); }

      T obj;
      try
      {
        var len = readStream.Read7BitEncodedInt();
        if (len <= SIZE_ZERO) { return default(T); }
        byte[] buffer = null;
        BufferManagerStreamReader binReader = null;
        var bufferManager = BufferManager.GlobalManager;
        try
        {
          buffer = BufferManager.GlobalManager.TakeBuffer(len);
          readStream.Read(buffer, 0, len);
          binReader = BufferManagerStreamReaderManager.Take();
          binReader.Reinitialize(buffer, 0, len, bufferManager);
          {
            obj = TypeSerializer.DeserializeFromStream<T>(binReader);
          }
        }
        finally
        {
          if (binReader != null) { BufferManagerStreamReaderManager.Return(binReader); }
          if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        obj = default(T);
      }
      return obj;
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public override void WriteToStream(Type type, object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value)
      {
        writeStream.Write7BitEncodedInt(SIZE_ZERO);
        return;
      }

      var output = BufferManagerOutputStreamManager.Take();
      output.Reinitialize();
      try
      {
        TypeSerializer.SerializeToStream(value, type, output);
        var objStream = output.ToReadOnlyStream();
        writeStream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, writeStream, true).GetAwaiter().GetResult();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(output);
      }
    }

    /// <inheritdoc />
    public override void WriteToStream<T>(T value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value)
      {
        writeStream.Write7BitEncodedInt(SIZE_ZERO);
        return;
      }

      var output = BufferManagerOutputStreamManager.Take();
      output.Reinitialize();
      try
      {
        TypeSerializer.SerializeToStream<T>(value, output);
        var objStream = output.ToReadOnlyStream();
        writeStream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, writeStream, true).GetAwaiter().GetResult();
      }
      finally
      {
        BufferManagerOutputStreamManager.Return(output);
      }
    }

    #endregion
  }
}
