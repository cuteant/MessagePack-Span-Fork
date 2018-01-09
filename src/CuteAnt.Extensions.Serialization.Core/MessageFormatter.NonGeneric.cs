using System;
using System.IO;
using System.Text;
#if !NET40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace CuteAnt.Extensions.Serialization
{
  partial class MessageFormatter
  {
    #region -- Read --

    /// <summary>Returns an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <returns>an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public object ReadFromStream(Type type, Stream readStream)
    {
      return ReadFromStream(type, readStream, null);
    }

    /// <summary>Returns an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      throw Error.NotSupported(FormattingSR.MediaTypeFormatterCannotRead, GetType().Name);
    }

#if !NET40
    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<Object> ReadFromStreamAsync(Type type, Stream readStream)
    {
      return ReadFromStreamAsync(type, readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<Object> ReadFromStreamAsync(Type type, Stream readStream, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }
      return ReadFromStreamAsync(type, readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual async Task<Object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding)
    {
#if NET451
      await TaskAsyncHelper.Completed;
#else
      await Task.CompletedTask;
#endif
      return ReadFromStream(type, readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<Object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }

      return ReadFromStreamAsync(type, readStream, effectiveEncoding);
    }
#endif

    #endregion

    #region -- Write --

    /// <summary>Tries to serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public void WriteToStream(Type type, Object value, Stream writeStream)
    {
      WriteToStream(type, value, writeStream, null);
    }

    /// <summary>Tries to serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      throw Error.NotSupported(FormattingSR.MediaTypeFormatterCannotWrite, GetType().Name);
    }


#if !NET40
    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Type type, Object value, Stream writeStream)
    {
      return WriteToStreamAsync(type, value, writeStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Type type, Object value, Stream writeStream, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }

      return WriteToStreamAsync(type, value, writeStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(type, value, writeStream, effectiveEncoding);
#if NET451
      return TaskAsyncHelper.Completed;
#else
      return Task.CompletedTask;
#endif
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given <paramref name="type"/>
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to write.</param>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }

      return WriteToStreamAsync(type, value, writeStream, effectiveEncoding);
    }
#endif

    #endregion
  }
}
