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
    #region -- ReadFromStream (Generic) --

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public T ReadFromStream<T>(Stream readStream)
    {
      return ReadFromStream<T>(readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
    {
      return (T)ReadFromStream(typeof(T), readStream, effectiveEncoding);
    }




    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public T ReadFromStream<T>(Type type, Stream readStream)
    {
      return (T)ReadFromStream(type, readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
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
    public T ReadFromStream<T>(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      return (T)ReadFromStream(type, readStream, effectiveEncoding);
    }




#if !NET40
    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<T> ReadFromStreamAsync<T>(Stream readStream)
    {
      return ReadFromStreamAsync<T>(readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<T> ReadFromStreamAsync<T>(Stream readStream, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<T>();
#else
        return Task.FromCanceled<T>(cancellationToken);
#endif
      }

      return ReadFromStreamAsync<T>(readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual async Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding)
    {
#if NET451
      await TaskAsyncHelper.Completed;
#else
      await Task.CompletedTask;
#endif
      return ReadFromStream<T>(readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="Stream"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<T>();
#else
        return Task.FromCanceled<T>(cancellationToken);
#endif
      }

      return ReadFromStreamAsync<T>(readStream, effectiveEncoding);
    }




    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream)
    {
      return ReadFromStreamAsync<T>(type, readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<T>();
#else
        return Task.FromCanceled<T>(cancellationToken);
#endif
      }

      return ReadFromStreamAsync<T>(type, readStream, null);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public async Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, Encoding effectiveEncoding)
    {
#if NET451
      await TaskAsyncHelper.Completed;
#else
      await Task.CompletedTask;
#endif
      return (T)ReadFromStream(type, readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<T>();
#else
        return Task.FromCanceled<T>(cancellationToken);
#endif
      }

      return ReadFromStreamAsync<T>(type, readStream, effectiveEncoding);
    }
#endif

    #endregion

    #region -- ReadFromStream (NonGeneric) --

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
  }
}
