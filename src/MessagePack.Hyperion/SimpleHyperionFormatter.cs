namespace MessagePack.Formatters
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using CuteAnt.Buffers;
    using Hyperion;

    public class SimpleHyperionFormatter<T> : IMessagePackFormatter<T>
    {
        public static readonly IMessagePackFormatter<T> Instance = new SimpleHyperionFormatter<T>();

#if DESKTOPCLR
        private const int c_initialBufferSize = 1024 * 80;
#else
        private const int c_initialBufferSize = 1024 * 64;
#endif

        private readonly Serializer _serializer;

        public SimpleHyperionFormatter()
        {
            _serializer = new Serializer(new SerializerOptions(versionTolerance: false, preserveObjectReferences: true));
        }

        public SimpleHyperionFormatter(SerializerOptions options)
        {
            if (null == options) { ThrowArgumentNullException(); }

            _serializer = new Serializer(options);
        }

        public T Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
        {
            if (reader.IsNil()) { return default; }

            var serializedObject = reader.ReadBytes();
            using (var ms = new MemoryStream(serializedObject, false))
            {
                var result = _serializer.Deserialize<T>(ms);
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
                using (var pooledStream = BufferManagerOutputStreamManager.Create())
                {
                    var outputStream = pooledStream.Object;
                    outputStream.Reinitialize(c_initialBufferSize, bufferPool);

                    _serializer.Serialize(value, outputStream);
                    buffer = outputStream.ToArray(out bufferSize);
                }

                writer.WriteBytes(buffer, 0, bufferSize, ref idx);
            }
            finally { if (buffer != null) { bufferPool.Return(buffer); } }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentNullException()
        {
            throw GetArgumentNullException();
            ArgumentNullException GetArgumentNullException()
            {
                return new ArgumentNullException("options");
            }
        }
    }
}
