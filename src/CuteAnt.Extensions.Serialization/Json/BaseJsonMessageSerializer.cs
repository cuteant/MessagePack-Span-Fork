using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Runtime;
using CuteAnt.Text;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Abstract <see cref="MessageFormatter"/> class to support Bson and Json.</summary>
  public abstract class BaseJsonMessageSerializer : BaseJsonMessageFormatter
  {
    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="BaseJsonMessageFormatter"/> class.</summary>
    protected BaseJsonMessageSerializer()
      : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="BaseJsonMessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="BaseJsonMessageFormatter"/> instance to copy settings from.</param>
#if !NETFX_CORE
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors",
        Justification = "MaxDepth is sealed in existing subclasses and its documentation carries warnings.")]
#endif
    protected BaseJsonMessageSerializer(BaseJsonMessageFormatter formatter)
      : base(formatter)
    {
    }

    #endregion

    #region -- ReadFromStream --

    /// <summary>Called during deserialization to read an object of the specified <paramref name="type"/>
    /// from the specified <paramref name="readStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to read.</param>
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> from which to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>The <see cref="object"/> instance that has been read.</returns>
    public override Object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (IsStrictMode && type == null) { throw new ArgumentNullException(nameof(type)); }
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // If content length is 0 then return default value for this type
      if (readStream.Position == readStream.Length)
      {
        return type != null ? GetDefaultValueForType(type) : null;
      }

      var len = readStream.Read7BitEncodedInt();
      if (len <= SIZE_ZERO)
      {
        return type != null ? GetDefaultValueForType(type) : null;
      }

      if (effectiveEncoding == null) { effectiveEncoding = Encoding.UTF8; }

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
        obj = ReadFromStream(type, binReader, effectiveEncoding, null);
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

    #endregion

    #region -- WriteToStream --

    /// <summary>Called during serialization to write an object of the specified <paramref name="type"/>
    /// to the specified <paramref name="writeStream"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of object to write.</param>
    /// <param name="value">The object to write.</param>
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    public override void WriteToStream(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }
      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      if (null == effectiveEncoding) { effectiveEncoding = StringHelper.UTF8NoBOM; }

      if (null == value)
      {
        writeStream.Write7BitEncodedInt(SIZE_ZERO);
        return;
      }
      using (var pooledStream = BufferManagerOutputStreamManager.Create())
      {
        var output = pooledStream.Object;
        output.Reinitialize();
        WriteToStream(type, value, output, effectiveEncoding, null);
        var objStream = output.ToReadOnlyStream();
        writeStream.Write7BitEncodedInt((int)objStream.Length);
        StreamToStreamCopy.CopyAsync(objStream, writeStream, true).WaitAndUnwrapException();
      }
    }

    #endregion
  }
}
