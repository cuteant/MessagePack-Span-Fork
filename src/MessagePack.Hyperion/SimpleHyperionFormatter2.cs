using System;
using System.IO;
using CuteAnt.Buffers;
using Hyperion;

namespace MessagePack.Formatters
{
    public class SimpleHyperionFormatter2<T> : IMessagePackFormatter<T>
    {
        public static readonly IMessagePackFormatter<T> Instance = new SimpleHyperionFormatter2<T>();

        private const int c_initialBufferSize = 1024 * 64;

        public SimpleHyperionFormatter2() { }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset)) { readSize = 1; return default; }

            var serializer = ((IFormatterResolverContext<Serializer>)formatterResolver).Value;
            var serializedObject = MessagePackBinary.ReadBytesSegment(bytes, offset, out readSize);
            using (var ms = new MemoryStream(serializedObject.Array, serializedObject.Offset, serializedObject.Count, false))
            {
                var result = serializer.Deserialize<T>(ms);
                return result;
            }
        }

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { return MessagePackBinary.WriteNil(ref bytes, offset); }

            var bufferPool = BufferManager.Shared;
            byte[] buffer = null; int bufferSize;

            try
            {
                var serializer = ((IFormatterResolverContext<Serializer>)formatterResolver).Value;
                using (var pooledStream = BufferManagerOutputStreamManager.Create())
                {
                    var outputStream = pooledStream.Object;
                    outputStream.Reinitialize(c_initialBufferSize, bufferPool);

                    serializer.Serialize(value, outputStream);
                    buffer = outputStream.ToArray(out bufferSize);
                }

                return MessagePackBinary.WriteBytes(ref bytes, offset, buffer, 0, bufferSize);
            }
            finally { if (buffer != null) { bufferPool.Return(buffer); } }
        }
    }
}
