using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CuteAnt.Buffers;
using Newtonsoft.Json;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>MessageFormatterExtensions</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class JsonMessageFormatterExtensions
    {
        internal const int c_initialBufferSize = 1024 * 64;
        private const int c_zeroSize = 0;

        #region -- Serialize --

        /// <summary>Serializes the specified item.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static byte[] Serialize(this IJsonMessageFormatter formatter, object item,
            Encoding effectiveEncoding, int initialBufferSize = c_initialBufferSize)
        {
            return Serialize(formatter, item, null, effectiveEncoding, initialBufferSize);
        }

        /// <summary>Serializes the specified item.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static byte[] Serialize(this IJsonMessageFormatter formatter, object item,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int initialBufferSize = c_initialBufferSize)
        {
            //#if NET40
            using (var pooledStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledStream.Object;
                outputStream.Reinitialize(initialBufferSize, BufferManager.Shared);

                formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
                return outputStream.ToByteArray();
            }
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
            //        pipe.Flush();
            //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
            //        return readBuffer.ToArray();
            //      }
            //#endif
        }

        #endregion

        #region -- WriteToMemoryPool --

        /// <summary>Serializes the specified item.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static ArraySegment<byte> WriteToMemoryPool(this IJsonMessageFormatter formatter, object item,
            Encoding effectiveEncoding, int initialBufferSize = c_initialBufferSize)
        {
            return WriteToMemoryPool(formatter, item, null, effectiveEncoding, initialBufferSize);
        }

        /// <summary>Serializes the specified item.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static ArraySegment<byte> WriteToMemoryPool(this IJsonMessageFormatter formatter, object item,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int initialBufferSize = c_initialBufferSize)
        {
            //#if NET40
            using (var pooledStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledStream.Object;
                outputStream.Reinitialize(initialBufferSize, BufferManager.Shared);

                formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
                return outputStream.ToArraySegment();
            }
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
            //        pipe.Flush();
            //        var readBuffer = pipe.Reader.ReadAsync().GetResult().Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return s_emptySegment; }
            //        var buffer = BufferManager.Shared.Rent(length);
            //        readBuffer.CopyTo(buffer);
            //        return new ArraySegment<byte>(buffer, 0, length);
            //      }
            //#endif
        }

        #endregion

        #region -- SerializeAsync --

        /// <summary>Serializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static Task<byte[]> SerializeAsync(this IJsonMessageFormatter formatter, object item,
            Encoding effectiveEncoding, int initialBufferSize = c_initialBufferSize)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Serialize(formatter, item, null, effectiveEncoding, initialBufferSize));
            //#if NET40
            //      return TaskEx.FromResult(SerializeToBytes(formatter, item, null, effectiveEncoding, initialBufferSize));
            //#else
            //      return SerializeToBytesAsync(formatter, item, null, effectiveEncoding, initialBufferSize);
            //#endif
        }

        /// <summary>Serializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static Task<byte[]> SerializeAsync(this IJsonMessageFormatter formatter, object item,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int initialBufferSize = c_initialBufferSize)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Serialize(formatter, item, serializerSettings, effectiveEncoding, initialBufferSize));
            //#if NET40
            //      await TaskConstants.Completed;
            //      return SerializeToBytes(formatter, item, serializerSettings, effectiveEncoding, initialBufferSize);
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
            //        await pipe.FlushAsync();
            //        var readBuffer = (await pipe.Reader.ReadAsync()).Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return EmptyArray<byte>.Instance; }
            //        return readBuffer.ToArray();
            //      }
            //#endif
        }

        #endregion

        #region -- WriteToMemoryPoolAsync --

        /// <summary>Serializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static Task<ArraySegment<byte>> WriteToMemoryPoolAsync(this IJsonMessageFormatter formatter, object item,
            Encoding effectiveEncoding, int initialBufferSize = c_initialBufferSize)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(WriteToMemoryPool(formatter, item, null, effectiveEncoding, initialBufferSize));
            //#if NET40
            //      return TaskEx.FromResult(SerializeToByteArraySegment(formatter, item, null, effectiveEncoding, initialBufferSize));
            //#else
            //      return SerializeToByteArraySegmentAsync(formatter, item, null, effectiveEncoding, initialBufferSize);
            //#endif
        }

        /// <summary>Serializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="item">The item.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <param name="initialBufferSize">The initial buffer size.</param>
        /// <returns></returns>
        public static Task<ArraySegment<byte>> WriteToMemoryPoolAsync(this IJsonMessageFormatter formatter, object item,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null, int initialBufferSize = c_initialBufferSize)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(WriteToMemoryPool(formatter, item, serializerSettings, effectiveEncoding, initialBufferSize));
            //#if NET40
            //      await TaskConstants.Completed;
            //      return SerializeToByteArraySegment(formatter, item, serializerSettings, effectiveEncoding, initialBufferSize);
            //#else
            //      using (var pooledPipe = PipelineManager.Create())
            //      {
            //        var pipe = pooledPipe.Object;
            //        var outputStream = new PipelineStream(pipe, initialBufferSize);
            //        formatter.WriteToStream(item?.GetType(), item, outputStream, effectiveEncoding, serializerSettings);
            //        await pipe.FlushAsync();
            //        var readBuffer = (await pipe.Reader.ReadAsync()).Buffer;
            //        var length = (int)readBuffer.Length;
            //        if (c_zeroSize == length) { return s_emptySegment; }
            //        var buffer = BufferManager.Shared.Rent(length);
            //        readBuffer.CopyTo(buffer);
            //        return new ArraySegment<byte>(buffer, 0, length);
            //      }
            //#endif
        }

        #endregion

        #region -- Deserialize Methods --

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static object Deserialize(this IJsonMessageFormatter formatter, Type type,
            byte[] serializedObject, Encoding effectiveEncoding)
        {
            return Deserialize(formatter, type, serializedObject, null, effectiveEncoding);
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static object Deserialize(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            if (null == serializedObject) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.serializedObject); }

            using (var ms = new MemoryStream(serializedObject))
            {
                return formatter.ReadFromStream(type, ms, effectiveEncoding, serializerSettings);
            }
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static object Deserialize(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
            int offset, int count, Encoding effectiveEncoding)
        {
            return Deserialize(formatter, type, serializedObject, offset, count, null, effectiveEncoding);
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static object Deserialize(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
            int offset, int count, JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            if (null == serializedObject) { ThrowHelper.ThrowArgumentNullException(ExceptionArgument.serializedObject); }

            using (var ms = new MemoryStream(serializedObject, offset, count))
            {
                return formatter.ReadFromStream(type, ms, effectiveEncoding, serializerSettings);
            }
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<object> DeserializeAsync(this IJsonMessageFormatter formatter, Type type,
            byte[] serializedObject, Encoding effectiveEncoding)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize(formatter, type, serializedObject, null, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<object> DeserializeAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize(formatter, type, serializedObject, serializerSettings, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<object> DeserializeAsync(this IJsonMessageFormatter formatter, Type type,
            byte[] serializedObject, int offset, int count, Encoding effectiveEncoding)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize(formatter, type, serializedObject, offset, count, null, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<object> DeserializeAsync(this IJsonMessageFormatter formatter, Type type, byte[] serializedObject,
            int offset, int count, JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize(formatter, type, serializedObject, offset, count, serializerSettings, effectiveEncoding));
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static T Deserialize<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, Encoding effectiveEncoding)
        {
            return (T)Deserialize(formatter, typeof(T), serializedObject, null, effectiveEncoding);
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static T Deserialize<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return (T)Deserialize(formatter, typeof(T), serializedObject, serializerSettings, effectiveEncoding);
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static T Deserialize<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
            int offset, int count, Encoding effectiveEncoding)
        {
            return (T)Deserialize(formatter, typeof(T), serializedObject, offset, count, null, effectiveEncoding);
        }

        /// <summary>Deserializes the specified serialized object.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static T Deserialize<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return (T)Deserialize(formatter, typeof(T), serializedObject, offset, count, serializerSettings, effectiveEncoding);
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<T> DeserializeAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, Encoding effectiveEncoding)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize<T>(formatter, serializedObject, null, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<T> DeserializeAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize<T>(formatter, serializedObject, serializerSettings, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<T> DeserializeAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject,
            int offset, int count, Encoding effectiveEncoding)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize<T>(formatter, serializedObject, offset, count, null, effectiveEncoding));
        }

        /// <summary>Deserializes the asynchronous.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="serializedObject">The serialized object.</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.</param>
        /// <param name="effectiveEncoding">The encoding.</param>
        /// <returns></returns>
        public static Task<T> DeserializeAsync<T>(this IJsonMessageFormatter formatter, byte[] serializedObject, int offset, int count,
            JsonSerializerSettings serializerSettings, Encoding effectiveEncoding = null)
        {
            return
        #if NET40
                TaskEx
        #else
                Task
        #endif
                .FromResult(Deserialize<T>(formatter, serializedObject, offset, count, serializerSettings, effectiveEncoding));
        }

        #endregion
    }
}
