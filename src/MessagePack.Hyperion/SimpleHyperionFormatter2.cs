namespace MessagePack.Formatters
{
    using System;
    using System.IO;
    using CuteAnt.Buffers;
    using Hyperion;

    public class SimpleHyperionFormatter2<T> : IMessagePackFormatter<T>
    {
        public static readonly IMessagePackFormatter<T> Instance = new SimpleHyperionFormatter2<T>();

#if DESKTOPCLR
        private const int c_initialBufferSize = 1024 * 80;
#else
        private const int c_initialBufferSize = 1024 * 64;
#endif

        public SimpleHyperionFormatter2() { }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            var serializedObject = reader.ReadBytes();
            if (null == serializedObject) { return default; }

            var serializer = ((IFormatterResolverContext<Serializer>)formatterResolver).Value;
            using (var ms = new MemoryStream(serializedObject, false))
            {
                var result = serializer.Deserialize<T>(ms);
                return result;
            }
        }

        public void Serialize(ref MessagePackWriter writer, ref int idx, T value, IFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNil(ref idx); return; }

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

                writer.WriteBytes(buffer, 0, bufferSize, ref idx);
            }
            finally { if (buffer != null) { bufferPool.Return(buffer); } }
        }
    }
}
