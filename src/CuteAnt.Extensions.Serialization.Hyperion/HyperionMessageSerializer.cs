using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Runtime;
using Hyperion;
using Hyperion.SerializerFactories;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary><see cref="MessageFormatter"/> class to handle wire.</summary>
  public class HyperionMessageSerializer : HyperionMessageFormatter
  {
    /// <summary>The default singlegton instance</summary>
    public new static readonly HyperionMessageSerializer DefaultInstance = new HyperionMessageSerializer();

    /// <summary>Constructor</summary>
    public HyperionMessageSerializer()
      : base()
    {
    }

    /// <summary>Constructor</summary>
    public HyperionMessageSerializer(IEnumerable<Surrogate> surrogates,
      IEnumerable<ValueSerializerFactory> serializerFactories = null,
      IEnumerable<Type> knownTypes = null)
      : base(surrogates, serializerFactories, knownTypes)
    {
    }

    /// <inheritdoc />
    public override object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
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
          obj = _serializer.Deserialize(binReader);
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
    public override async Task<Object> ReadFromStreamAsync(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      await TaskConstants.Completed;
      return ReadFromStream(type, readStream, effectiveEncoding);
    }

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
        _serializer.Serialize(value, output);
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
    public override async Task WriteToStreamAsync(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      WriteToStream(type, value, writeStream, effectiveEncoding);
      await TaskConstants.Completed;
    }
  }
}
