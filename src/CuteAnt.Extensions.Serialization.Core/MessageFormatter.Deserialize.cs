using System;
using System.IO;
using System.Threading.Tasks;

namespace CuteAnt.Extensions.Serialization
{
  partial class MessageFormatter
  {
    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public virtual object Deserialize(Type type, byte[] serializedObject)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject))
      {
        return ReadFromStream(type, ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public virtual object Deserialize(Type type, in ArraySegment<byte> serializedObject)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject.Array, serializedObject.Offset, serializedObject.Count))
      {
        return ReadFromStream(type, ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual object Deserialize(Type type, byte[] serializedObject, int offset, int count)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject, offset, count))
      {
        return ReadFromStream(type, ms);
      }
    }




    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public virtual T Deserialize<T>(byte[] serializedObject)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject))
      {
        return ReadFromStream<T>(ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public virtual T Deserialize<T>(in ArraySegment<byte> serializedObject)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject.Array, serializedObject.Offset, serializedObject.Count))
      {
        return ReadFromStream<T>(ms);
      }
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual T Deserialize<T>(byte[] serializedObject, int offset, int count)
    {
      if (serializedObject == null) { throw new ArgumentNullException(nameof(serializedObject)); }

      using (var ms = new MemoryStream(serializedObject, offset, count))
      {
        return ReadFromStream<T>(ms);
      }
    }




    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public T Deserialize<T>(Type type, byte[] serializedObject)
    {
      return (T)Deserialize(type, serializedObject);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public T Deserialize<T>(Type type, in ArraySegment<byte> serializedObject)
    {
      return (T)Deserialize(type, serializedObject);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public T Deserialize<T>(Type type, byte[] serializedObject, int offset, int count)
    {
      return (T)Deserialize(type, serializedObject, offset, count);
    }




    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<object> DeserializeAsync(Type type, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize(type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<object> DeserializeAsync(Type type, in ArraySegment<byte> serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize(type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public Task<object> DeserializeAsync(Type type, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize(type, serializedObject, offset, count));
    }




    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(in ArraySegment<byte> serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(serializedObject, offset, count));
    }




    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(Type type, byte[] serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(Type type, in ArraySegment<byte> serializedObject)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(type, serializedObject));
    }

    /// <summary>Deserializes the asynchronous.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public Task<T> DeserializeAsync<T>(Type type, byte[] serializedObject, int offset, int count)
    {
      return
#if NET40
        TaskEx
#else
        Task
#endif
        .FromResult(Deserialize<T>(type, serializedObject, offset, count));
    }
  }
}
