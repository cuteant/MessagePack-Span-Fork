using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Base class to handle serializing and deserializing strongly-typed objects.</summary>
  public abstract class MessageFormatter : IMessageFormatter
  {
    #region @@ Fields @@

    public const int SIZE_ZERO = 0;

    /// <summary>A <see cref="Type"/> representing <see cref="DelegatingEnumerable{T}"/>.</summary>
    internal static readonly Type DelegatingEnumerableGenericType = typeof(DelegatingEnumerable<>);

    /// <summary>A <see cref="Type"/> representing <see cref="IEnumerable{T}"/>.</summary>
    internal static readonly Type EnumerableInterfaceGenericType = typeof(IEnumerable<>);

    /// <summary>A <see cref="Type"/> representing <see cref="IQueryable{T}"/>.</summary>
    internal static readonly Type QueryableInterfaceGenericType = typeof(IQueryable<>);

    private static readonly ConcurrentDictionary<Type, Type> _delegatingEnumerableCache = new ConcurrentDictionary<Type, Type>();
    private static ConcurrentDictionary<Type, ConstructorInfo> _delegatingEnumerableConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

    private IRequiredMemberSelector _requiredMemberSelector;

    #endregion

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="MessageFormatter"/> class.</summary>
    protected MessageFormatter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="MessageFormatter"/> instance to copy settings from.</param>
    protected MessageFormatter(MessageFormatter formatter)
    {
      if (formatter == null) { throw new ArgumentNullException(nameof(formatter)); }

      _requiredMemberSelector = formatter._requiredMemberSelector;
    }

    #endregion

    #region @@ Properties @@

    /// <summary>Gets or sets the <see cref="IRequiredMemberSelector"/> used to determine required members.</summary>
    public virtual IRequiredMemberSelector RequiredMemberSelector
    {
      get { return _requiredMemberSelector; }
      set { _requiredMemberSelector = value; }
    }

    //internal virtual bool CanWriteAnyTypes { get { return true; } }

    #endregion

    #region -- DeepCopy --

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    public abstract object DeepCopy(object source);

    #endregion

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

#if !NET40
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
    public async Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding)
    {
      return (T)(await ReadFromStreamAsync(typeof(T), readStream, effectiveEncoding).ConfigureAwait(false));
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

    /// <summary>Tries to serializes the given <paramref name="value"/> of the given type
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <typeparam name="T">The type of the object to write.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="writeStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="value">The object value to write.  It may be <c>null</c>.</param>
    /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public void WriteToStream<T>(T value, Stream writeStream)
    {
      WriteToStream(typeof(T), value, writeStream, null);
    }

    /// <summary>Tries to serializes the given <paramref name="value"/> of the given type
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <typeparam name="T">The type of the object to write.</typeparam>
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
    public virtual void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(typeof(T), value, writeStream, effectiveEncoding);
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
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual async Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding)
    {
#if NET451
      await TaskAsyncHelper.Completed;
#else
      await Task.CompletedTask;
#endif
      WriteToStream(type, value, writeStream, effectiveEncoding);
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

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given type
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public Task WriteToStreamAsync<T>(T value, Stream writeStream, Encoding effectiveEncoding)
    {
      return WriteToStreamAsync(typeof(T), value, writeStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given type
    /// to the given <paramref name="writeStream"/>.</summary>
    /// <typeparam name="T">Target type.</typeparam>
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
    public Task WriteToStreamAsync<T>(T value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
#if NET451
        return TaskAsyncHelper.Canceled<T>();
#else
        return Task.FromCanceled<T>(cancellationToken);
#endif
      }

      return WriteToStreamAsync<T>(value, writeStream, effectiveEncoding);
    }
#endif

    #endregion

    #region -- CanReadType / CanWriteType --

    /// <summary>Informs the serialization manager whether this serializer supports the type for serialization.</summary>
    /// <param name="type">The type of the item to be serialized</param>
    /// <returns>A value indicating whether the item can be serialized.</returns>
    public abstract bool IsSupportedType(Type type);

    /// <summary>Determines whether this <see cref="MessageFormatter"/> can deserialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be deserialized.</remarks>
    /// <param name="type">The type of object that will be deserialized.</param>
    /// <returns><c>true</c> if this <see cref="MessageFormatter"/> can deserialize an object of that type; otherwise <c>false</c>.</returns>
    public virtual bool CanReadType(Type type) => IsSupportedType(type);

    /// <summary>Determines whether this <see cref="MessageFormatter"/> can serialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be serialized.</remarks>
    /// <param name="type">The type of object that will be serialized.</param>
    /// <returns><c>true</c> if this <see cref="MessageFormatter"/> can serialize an object of that type; otherwise <c>false</c>.</returns>
    public virtual bool CanWriteType(Type type) => IsSupportedType(type);

    #endregion

    #region **& TryGetDelegatingType &**

    private static Boolean TryGetDelegatingType(Type interfaceType, ref Type type)
    {
      if (type != null && type.IsInterface() && type.IsGenericType())
      {
        Type genericType = type.ExtractGenericInterface(interfaceType);

        if (genericType != null)
        {
          type = GetOrAddDelegatingType(type, genericType);
          return true;
        }
      }

      return false;
    }

    #endregion

    #region ==& TryGetDelegatingTypeForIEnumerableGenericOrSame &==

    /// <summary>This method converts <see cref="IEnumerable{T}"/> (and interfaces that mandate it) to a <see cref="DelegatingEnumerable{T}"/> for serialization purposes.</summary>
    /// <param name="type">The type to potentially be wrapped. If the type is wrapped, it's changed in place.</param>
    /// <returns>Returns <c>true</c> if the type was wrapped; <c>false</c>, otherwise</returns>
    internal static Boolean TryGetDelegatingTypeForIEnumerableGenericOrSame(ref Type type)
    {
      return TryGetDelegatingType(EnumerableInterfaceGenericType, ref type);
    }

    #endregion

    #region ==& TryGetDelegatingTypeForIQueryableGenericOrSame &==

    /// <summary>This method converts <see cref="IQueryable{T}"/> (and interfaces that mandate it) to a <see cref="DelegatingEnumerable{T}"/> for serialization purposes.</summary>
    /// <param name="type">The type to potentially be wrapped. If the type is wrapped, it's changed in place.</param>
    /// <returns>Returns <c>true</c> if the type was wrapped; <c>false</c>, otherwise</returns>
    internal static Boolean TryGetDelegatingTypeForIQueryableGenericOrSame(ref Type type)
    {
      return TryGetDelegatingType(QueryableInterfaceGenericType, ref type);
    }

    #endregion

    #region ==& GetTypeRemappingConstructor &==

    internal static ConstructorInfo GetTypeRemappingConstructor(Type type)
    {
      _delegatingEnumerableConstructorCache.TryGetValue(type, out ConstructorInfo constructorInfo);
      return constructorInfo;
    }

    #endregion

    #region **& GetOrAddDelegatingType &**

    private static Type GetOrAddDelegatingType(Type type, Type genericType)
    {
      return _delegatingEnumerableCache.GetOrAdd(
          type,
          (typeToRemap) =>
          {
            // The current method is called by methods that already checked the type for is not null, is generic and is or implements IEnumerable<T>
            // This retrieves the T type of the IEnumerable<T> interface.
            Type elementType = genericType.GetGenericArguments()[0];
            Type delegatingType = DelegatingEnumerableGenericType.MakeGenericType(elementType);
            ConstructorInfo delegatingConstructor = delegatingType.GetConstructor(new Type[] { EnumerableInterfaceGenericType.MakeGenericType(elementType) });
            _delegatingEnumerableConstructorCache.TryAdd(delegatingType, delegatingConstructor);

            return delegatingType;
          });
    }

    #endregion

    #region --& GetDefaultValueForType &--

    private const MethodImplOptions s_aggressiveInlining = (MethodImplOptions)256;
    private static Dictionary<Type, object> s_defaultValueTypes = new Dictionary<Type, object>();

    /// <summary>Gets the default value for the specified type.</summary>
    public static object GetDefaultValueForType(Type type)
    {
      if (null == type) { return null; }

      if (!type.IsValueType) return null;

      if (s_defaultValueTypes.TryGetValue(type, out object defaultValue)) return defaultValue;

      defaultValue = Activator.CreateInstance(type);

      Dictionary<Type, object> snapshot, newCache;
      do
      {
        snapshot = s_defaultValueTypes;
        newCache = new Dictionary<Type, object>(s_defaultValueTypes)
        {
          [type] = defaultValue
        };
      } while (!ReferenceEquals(Interlocked.CompareExchange(ref s_defaultValueTypes, newCache, snapshot), snapshot));

      return defaultValue;
    }

    #endregion
  }
}
