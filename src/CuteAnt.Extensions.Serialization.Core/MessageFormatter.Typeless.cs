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
    #region -- Write --

    /// <summary>Tries to serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public void WriteToStream(Object value, Stream writeStream)
    {
      WriteToStream(value, writeStream, null);
    }

    /// <summary>Tries to serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual void WriteToStream(Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(null, value, writeStream, effectiveEncoding);
    }

#if !NET40
    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Object value, Stream writeStream)
    {
      return WriteToStreamAsync(value, writeStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Object value, Stream writeStream, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }

      return WriteToStreamAsync(value, writeStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual Task WriteToStreamAsync(Object value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(value, writeStream, effectiveEncoding);
#if NET451
      return TaskAsyncHelper.Completed;
#else
      return Task.CompletedTask;
#endif
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> 
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Object value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<object>();
#else
        return Task.FromCanceled<object>(cancellationToken);
#endif
      }

      return WriteToStreamAsync(value, writeStream, effectiveEncoding);
    }
#endif

    #endregion
  }
}
