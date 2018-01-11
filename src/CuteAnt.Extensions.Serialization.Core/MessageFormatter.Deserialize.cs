using System;
using System.IO;
using System.Text;
using CuteAnt.Buffers;
using System.Threading;
using System.Threading.Tasks;
#if !NET40
//using CuteAnt.IO.Pipelines;
#endif

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
      return (T)Deserialize(typeof(T), serializedObject);
    }

    /// <summary>Deserializes the specified serialized object.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject">The serialized object.</param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual T Deserialize<T>(byte[] serializedObject, int offset, int count)
    {
      return (T)Deserialize(typeof(T), serializedObject, offset, count);
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
  }
}
