using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuteAnt.AsyncEx;
using CuteAnt.Buffers;
using CuteAnt.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
      Logger = NullLogger.Instance;
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

    /// <summary>Gets or sets the <see cref="ILogger"/>.</summary>
    public ILogger Logger { get; set; }

    //internal virtual bool CanWriteAnyTypes { get { return true; } }

    #endregion

    #region -- Initialize --

    /// <summary>Initializes the external serializer. Called once when the serialization manager creates 
    /// an instance of this type</summary>
    public virtual void Initialize(ILogger logger)
    {
      Logger = logger ?? NullLogger.Instance;
    }

    #endregion

    #region -- DeepCopy --

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    public virtual object DeepCopy(object source)
    {
      if (source == null) { return null; }

      const int _bufferSize = 2 * 1024;

      var type = source.GetType();
      using (var pooledOutputStream = BufferManagerOutputStreamManager.Create())
      {
        var outputStream = pooledOutputStream.Object;
        outputStream.Reinitialize(_bufferSize);
        WriteToStream(type, source, outputStream);
        using (var pooledStreamReader = BufferManagerStreamReaderManager.Create())
        {
          var inputStream = pooledStreamReader.Object;
          inputStream.Reinitialize(outputStream.ToReadOnlyStream(), false);
          return ReadFromStream(type, inputStream);
        }
      }
    }

    #endregion

    #region -- Read --

    /// <summary>Tries to deserialize an item.</summary>
    /// <param name="reader">The reader used for binary deserialization</param>
    /// <param name="expectedType">The type that should be deserialzied</param>
    /// <returns>The deserialized object</returns>
    public object Deserialize(Type expectedType, BufferManagerStreamReader reader)
    {
      return ReadFromStream(expectedType, reader, null);
    }

    /// <summary>Returns an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <returns>an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public object ReadFromStream(Type type, BufferManagerStreamReader readStream)
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
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual object ReadFromStream(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
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
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public T ReadFromStream<T>(BufferManagerStreamReader readStream)
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
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>An object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual T ReadFromStream<T>(BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      return (T)ReadFromStream(typeof(T), readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public virtual async Task<Object> ReadFromStreamAsync(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw Error.ArgumentNull(nameof(type)); }
      if (readStream == null) { throw Error.ArgumentNull(nameof(readStream)); }

      await TaskConstants.Completed;
      return ReadFromStream(type, readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given <paramref name="type"/> from the given <paramref name="readStream"/></summary>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<Object> ReadFromStreamAsync(Type type, BufferManagerStreamReader readStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested) { return TaskConstants<Object>.Canceled; }

      return ReadFromStreamAsync(type, readStream, effectiveEncoding);
    }

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <remarks>
    /// <para>This implementation throws a <see cref="NotSupportedException"/>. Derived types should override this method if the formatter
    /// supports reading.</para>
    /// <para>An implementation of this method should NOT close <paramref name="readStream"/> upon completion.</para>
    /// </remarks>
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    public async Task<T> ReadFromStreamAsync<T>(BufferManagerStreamReader readStream, Encoding effectiveEncoding)
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
    /// <param name="readStream">The <see cref="BufferManagerStreamReader"/> to read.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    public Task<T> ReadFromStreamAsync<T>(BufferManagerStreamReader readStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested) { return TaskConstants<T>.Canceled; }

      return ReadFromStreamAsync<T>(readStream, effectiveEncoding);
    }

    #endregion

    #region -- Write --

    /// <summary>Tries to serialize an item.</summary>
    /// <param name="item">The instance of the object being serialized</param>
    /// <param name="writer">The writer used for serialization</param>
    /// <param name="expectedType">The type that the deserializer will expect</param>
    public void Serialize(object item, BufferManagerOutputStream writer, Type expectedType)
    {
      WriteToStream(expectedType, item, writer, null);
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public void WriteToStream(Type type, Object value, BufferManagerOutputStream writeStream)
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual void WriteToStream(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public void WriteToStream<T>(T value, BufferManagerOutputStream writeStream)
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual void WriteToStream<T>(T value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      WriteToStream(typeof(T), value, writeStream, effectiveEncoding);
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public virtual async Task WriteToStreamAsync(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
    {
      if (type == null) { throw Error.ArgumentNull(nameof(type)); }
      if (writeStream == null) { throw Error.ArgumentNull(nameof(writeStream)); }

      WriteToStream(type, value, writeStream, effectiveEncoding);
      await TaskConstants.Completed;
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync(Type type, Object value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested) { return TaskConstants.Canceled; }

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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync<T>(T value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding)
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
    /// <param name="writeStream">The <see cref="BufferManagerOutputStream"/> to which to write.</param>
    /// <param name="effectiveEncoding">The <see cref="Encoding"/> to use when writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    public Task WriteToStreamAsync<T>(T value, BufferManagerOutputStream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested) { return TaskConstants.Canceled; }

      return WriteToStreamAsync<T>(value, writeStream, effectiveEncoding);
    }

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
      ConstructorInfo constructorInfo;
      _delegatingEnumerableConstructorCache.TryGetValue(type, out constructorInfo);
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

    #region **& GetDefaultValueForType &**

    /// <summary>Gets the default value for the specified type.</summary>
    [MethodImpl(InlineMethod.Value)]
    public static Object GetDefaultValueForType(Type type) => type?.GetDefaultValue();

    #endregion
  }
}
