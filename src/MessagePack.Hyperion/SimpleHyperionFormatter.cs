using System;
using System.IO;
using CuteAnt.Buffers;
using Hyperion;

namespace MessagePack.Formatters
{
    public class SimpleHyperionFormatter<T> : IMessagePackFormatter<T>
    {
        public static readonly IMessagePackFormatter<T> Instance = new SimpleHyperionFormatter<T>();

        private const int c_initialBufferSize = 1024 * 64;

        private readonly Serializer _serializer;

        public SimpleHyperionFormatter()
        {
            _serializer = new Serializer(new SerializerOptions(versionTolerance: false, preserveObjectReferences: true));
        }

        public SimpleHyperionFormatter(SerializerOptions options)
        {
            if (null == options) { throw new ArgumentNullException(nameof(options)); }

            _serializer = new Serializer(options.Clone(false, true));
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var serializedObject = MessagePackBinary.ReadBytesSegment(bytes, offset, out readSize);
            using (var ms = new MemoryStream(serializedObject.Array, serializedObject.Offset, serializedObject.Count, false))
            {
                var result = _serializer.Deserialize<T>(ms);
                return result;
            }
        }

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var bufferPool = BufferManager.Shared;
            byte[] buffer; int bufferSize;

            using (var pooledStream = BufferManagerOutputStreamManager.Create())
            {
                var outputStream = pooledStream.Object;
                outputStream.Reinitialize(c_initialBufferSize, bufferPool);

                _serializer.Serialize(value, outputStream);
                buffer = outputStream.ToArray(out bufferSize);
            }

            var objSize = MessagePackBinary.WriteBytes(ref bytes, offset, buffer, 0, bufferSize);
            bufferPool.Return(buffer);

            return objSize;
        }
    }
}
