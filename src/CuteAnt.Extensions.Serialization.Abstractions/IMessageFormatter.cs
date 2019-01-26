using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Interface that allows third-party serializers to perform serialization, even when
  /// the types being serialized are not known (generics) at initialization time.
  /// 
  /// Types that inherit this interface are discovered through dependency injection and 
  /// automatically incorporated in the Serialization Manager.
  /// </summary>
  public interface IMessageFormatter
  {
    /// <summary>Determines whether this <see cref="IMessageFormatter"/> can deserialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be deserialized.</remarks>
    /// <param name="type">The type of object that will be deserialized.</param>
    /// <returns><c>true</c> if this <see cref="IMessageFormatter"/> can deserialize an object of that type; otherwise <c>false</c>.</returns>
    bool CanReadType(Type type);

    /// <summary>Determines whether this <see cref="IMessageFormatter"/> can serialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be serialized.</remarks>
    /// <param name="type">The type of object that will be serialized.</param>
    /// <returns><c>true</c> if this <see cref="IMessageFormatter"/> can serialize an object of that type; otherwise <c>false</c>.</returns>
    bool CanWriteType(Type type);

    /// <summary>Informs the serialization manager whether this serializer supports the type for serialization.</summary>
    /// <param name="type">The type of the item to be serialized</param>
    /// <returns>A value indicating whether the item can be serialized.</returns>
    bool IsSupportedType(Type type);

    #region -- DeepCopy --

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    object DeepCopyObject(object source);

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    T DeepCopy<T>(T source);

    #endregion

    #region -- Serialize --

    byte[] Serialize<T>(T item);

    byte[] Serialize<T>(T item, int initialBufferSize);

    Task<byte[]> SerializeAsync<T>(T item);
    Task<byte[]> SerializeAsync<T>(T item, int initialBufferSize);

    #endregion

    #region -- SerializeObject --

    byte[] SerializeObject(object item);
    byte[] SerializeObject(object item, int initialBufferSize);

    Task<byte[]> SerializeObjectAsync(object item);
    Task<byte[]> SerializeObjectAsync(object item, int initialBufferSize);

    #endregion

    #region -- WriteToMemoryPool --

    ArraySegment<byte> WriteToMemoryPool<T>(T item);
    ArraySegment<byte> WriteToMemoryPool<T>(T item, int initialBufferSize);

    ArraySegment<byte> WriteToMemoryPool(object item);
    ArraySegment<byte> WriteToMemoryPool(object item, int initialBufferSize);

    Task<ArraySegment<byte>> WriteToMemoryPoolAsync<T>(T item);
    Task<ArraySegment<byte>> WriteToMemoryPoolAsync<T>(T item, int initialBufferSize);

    Task<ArraySegment<byte>> WriteToMemoryPoolAsync(object item);
    Task<ArraySegment<byte>> WriteToMemoryPoolAsync(object item, int initialBufferSize);

    #endregion

    #region -- Deserialize --

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    object Deserialize(Type type, byte[] serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    object Deserialize(Type type, ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    object Deserialize(Type type, byte[] serializedObject, int offset, int count);




    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    T Deserialize<T>(byte[] serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    T Deserialize<T>(ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    T Deserialize<T>(byte[] serializedObject, int offset, int count);




    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    T Deserialize<T>(Type type, byte[] serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    T Deserialize<T>(Type type, ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    T Deserialize<T>(Type type, byte[] serializedObject, int offset, int count);




    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<object> DeserializeAsync(Type type, byte[] serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<object> DeserializeAsync(Type type, ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<object> DeserializeAsync(Type type, byte[] serializedObject, int offset, int count);




    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(byte[] serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(byte[] serializedObject, int offset, int count);




    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(Type type, byte[] serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(Type type, ArraySegment<byte> serializedObject);

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<T> DeserializeAsync<T>(Type type, byte[] serializedObject, int offset, int count);

    #endregion

    #region -- ReadFromStream --

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
    object ReadFromStream(Type type, Stream readStream);

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
    object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding);




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
    T ReadFromStream<T>(Stream readStream);

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
    T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding);




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
    T ReadFromStream<T>(Type type, Stream readStream);

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
    T ReadFromStream<T>(Type type, Stream readStream, Encoding effectiveEncoding);


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
    Task<Object> ReadFromStreamAsync(Type type, Stream readStream);

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
    Task<Object> ReadFromStreamAsync(Type type, Stream readStream, CancellationToken cancellationToken);

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
    Task<object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding);

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
    Task<object> ReadFromStreamAsync(Type type, Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken);




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
    Task<T> ReadFromStreamAsync<T>(Stream readStream);

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
    Task<T> ReadFromStreamAsync<T>(Stream readStream, CancellationToken cancellationToken);

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
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
    Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding);

    /// <summary>Returns a <see cref="Task"/> to deserialize an object of the given type from the given <paramref name="readStream"/></summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
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
    Task<T> ReadFromStreamAsync<T>(Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken);




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
    Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream);

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
    Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, CancellationToken cancellationToken);

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
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support reading.</exception>
    /// <seealso cref="CanReadType(Type)"/>
    Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, Encoding effectiveEncoding);

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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
    /// <seealso cref="CanReadType(Type)"/>
    Task<T> ReadFromStreamAsync<T>(Type type, Stream readStream, Encoding effectiveEncoding, CancellationToken cancellationToken);
#endif

    #endregion

    #region -- WriteToStream --

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
    void WriteToStream(Object value, Stream writeStream);

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
    void WriteToStream(Object value, Stream writeStream, Encoding effectiveEncoding);




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
    void WriteToStream(Type type, Object value, Stream writeStream);

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
    void WriteToStream(Type type, Object value, Stream writeStream, Encoding effectiveEncoding);




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
    void WriteToStream<T>(T value, Stream writeStream);

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
    void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding);




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
    Task WriteToStreamAsync(Object value, Stream writeStream);

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
    Task WriteToStreamAsync(Object value, Stream writeStream, CancellationToken cancellationToken);

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
    Task WriteToStreamAsync(Object value, Stream writeStream, Encoding effectiveEncoding);

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
    Task WriteToStreamAsync(Object value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken);




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
    Task WriteToStreamAsync(Type type, Object value, Stream writeStream);

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
    Task WriteToStreamAsync(Type type, Object value, Stream writeStream, CancellationToken cancellationToken);

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
    Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding);

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
    Task WriteToStreamAsync(Type type, Object value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken);




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
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    Task WriteToStreamAsync<T>(T value, Stream writeStream);

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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    Task WriteToStreamAsync<T>(T value, Stream writeStream, CancellationToken cancellationToken);

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given type
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
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <exception cref="NotSupportedException">Derived types need to support writing.</exception>
    /// <seealso cref="CanWriteType(Type)"/>
    Task WriteToStreamAsync<T>(T value, Stream writeStream, Encoding effectiveEncoding);

    /// <summary>Returns a <see cref="Task"/> that serializes the given <paramref name="value"/> of the given type
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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that will perform the write.</returns>
    /// <seealso cref="CanWriteType(Type)"/>
    Task WriteToStreamAsync<T>(T value, Stream writeStream, Encoding effectiveEncoding, CancellationToken cancellationToken);
#endif

    #endregion
  }
}
