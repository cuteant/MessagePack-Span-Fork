using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using ProtoBuf.Meta;
#if NET40
using System.Reflection;
#endif

namespace CuteAnt.Extensions.Serialization
{
    /// <summary><see cref="MessageFormatter"/> class to handle protobuf-net.</summary>
    public partial class ProtoBufMessageFormatter : MessageFormatter
    {
        private static readonly ILogger s_logger = TraceLogger.GetLogger<ProtoBufMessageFormatter>();

        #region @@ Properties @@

        internal protected static readonly RuntimeTypeModel s_model;
        private static readonly ConcurrentHashSet<Type> s_unsupportedTypeInfoSet = new ConcurrentHashSet<Type>();

        /// <summary>The default singlegton instance</summary>
        public static readonly ProtoBufMessageFormatter DefaultInstance = new ProtoBufMessageFormatter();

        /// <summary>Type Model</summary>
        public static RuntimeTypeModel Model => s_model;

        #endregion

        #region -- Constructors --

        static ProtoBufMessageFormatter()
        {
            s_model = RuntimeTypeModel.Default;
            // 考虑到类的继承问题，禁用 [DataContract] / [XmlType]
            s_model.AutoAddProtoContractTypesOnly = true;
        }

        /// <summary>Constructor</summary>
        public ProtoBufMessageFormatter()
        {
        }

        #endregion

        #region -- IsSupportedType --

        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (s_unsupportedTypeInfoSet.Contains(type)) { return false; }

#if NET40
            if (type.IsGenericType && type.IsConstructedGenericType() == false) { s_unsupportedTypeInfoSet.TryAdd(type); return false; }
#else
            if (type.IsGenericType && type.IsConstructedGenericType == false) { s_unsupportedTypeInfoSet.TryAdd(type); return false; }
#endif
            if (!s_model.CanSerialize(type))
            {
                s_unsupportedTypeInfoSet.TryAdd(type); return false;
            }

            return true;
        }

        #endregion

        #region -- DeepCopy --

        public sealed override object DeepCopyObject(object source) => s_model.DeepClone(source);

        #endregion

        #region -- ReadFromStream --

        /// <inheritdoc />
        public sealed override T ReadFromStream<T>(Stream readStream, Encoding effectiveEncoding)
        {
            if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

            try
            {
                return (T)s_model.Deserialize(readStream, null, typeof(T), null);
            }
            catch (Exception ex)
            {
                s_logger.LogError(ex.ToString());
                return default;
            }
        }

        /// <inheritdoc />
        public sealed override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
        {
            if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

            // 不是 Stream 都会实现 Position、Length 这两个属性
            //if (readStream.Position == readStream.Length) { return GetDefaultValueForType(type); }

            try
            {
                // protobuf-net，读取空数据流并不会返回空对象，而是根据类型实例化一个
                return s_model.Deserialize(readStream, null, type, null);
            }
            catch (Exception ex)
            {
                s_logger.LogError(ex.ToString());
                return GetDefaultValueForType(type);
            }
        }

        #endregion

        #region -- WriteToStream --

        /// <inheritdoc />
        public sealed override void WriteToStream<T>(T value, Stream writeStream, Encoding effectiveEncoding)
        {
            if (null == value) { return; }

            if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

            s_model.Serialize(writeStream, value, null);
        }

        /// <inheritdoc />
        public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
        {
            if (null == value) { return; }

            if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

            s_model.Serialize(writeStream, value, null);
        }

        /// <inheritdoc />
        public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
        {
            if (null == value) { return; }

            if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

            s_model.Serialize(writeStream, value, null);
        }

        #endregion
    }
}
