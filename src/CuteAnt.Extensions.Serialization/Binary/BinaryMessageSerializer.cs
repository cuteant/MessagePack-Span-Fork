#if !NETSTANDARD
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Runtime;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  public class BinaryMessageSerializer : BinaryMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly BinaryMessageSerializer DefaultInstance = new BinaryMessageSerializer();

    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      //if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      if (readStream.Position == readStream.Length) { return type != null ? GetDefaultValueForType(type) : null; }

      var len = readStream.Read7BitEncodedInt();
      if (len <= SIZE_ZERO) { return type != null ? GetDefaultValueForType(type) : null; }

      var bufferManager = BufferManager.GlobalManager;
      object obj;
      byte[] buffer = null;
      BufferManagerStreamReader binReader = null;
      try
      {
        buffer = BufferManager.GlobalManager.TakeBuffer(len);
        readStream.Read(buffer, 0, len);
        binReader = BufferManagerStreamReaderManager.Take();
        binReader.Reinitialize(buffer, 0, len, bufferManager);
        var formatter = new BinaryFormatter();
        obj = formatter.Deserialize(binReader);
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        obj = type != null ? GetDefaultValueForType(type) : null;
      }
      finally
      {
        BufferManagerStreamReaderManager.Return(binReader);
        if (buffer != null) { bufferManager.ReturnBuffer(buffer); }
      }
      return obj;
    }

    public override void WriteToStream(Type type, object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      //if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == value)
      {
        writeStream.Write7BitEncodedInt(SIZE_ZERO);
        return;
      }
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var output = pooledStream.Object;
        output.Reinitialize();
        var formatter = new BinaryFormatter();
        formatter.Serialize(output, value);
        var objStream = output.ToReadOnlyStream();
        writeStream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, writeStream, true).WaitAndUnwrapException();
        writeStream.Flush();
      }
    }
  }
}
#endif