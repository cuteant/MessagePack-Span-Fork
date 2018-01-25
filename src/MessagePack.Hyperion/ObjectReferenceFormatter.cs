using System;
using System.Collections.Generic;
using System.IO;
using CuteAnt.Buffers;
using Hyperion;
using Hyperion.SerializerFactories;

namespace MessagePack.Formatters
{
    public class ObjectReferenceFormatter<T> : IMessagePackFormatter<T>
    {
        private const int c_initialBufferSize = 1024 * 64;

        private readonly Serializer _serializer;

        public ObjectReferenceFormatter() : this(surrogates: null) { }
        public ObjectReferenceFormatter(IEnumerable<Surrogate> surrogates,
          IEnumerable<ValueSerializerFactory> serializerFactories = null,
          IEnumerable<Type> knownTypes = null, bool ignoreISerializable = false)
        {
            var options = new SerializerOptions(
                versionTolerance: true,
                preserveObjectReferences: true,
                surrogates: surrogates,
                serializerFactories: serializerFactories,
                knownTypes: knownTypes,
                ignoreISerializable: ignoreISerializable
            );
            _serializer = new Serializer(options);
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var ms = new MemoryStream(bytes, offset, bytes.Length);
            var result = _serializer.Deserialize<T>(ms);
            readSize = (int)ms.Position;
            return result;
        }

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            var bufferPool = BufferManager.Shared;

            byte[] buffer; int bufferSize;
            using (var pooledStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledStream.Object;
                outputStream.Reinitialize(c_initialBufferSize, bufferPool);

                _serializer.Serialize(value, outputStream);
                buffer = outputStream.ToArray(out bufferSize);
            }

            MessagePackBinary.WriteBytes(ref bytes, offset, buffer, 0, bufferSize);
            bufferPool.Return(buffer);

            return bufferSize;
        }
    }
}
