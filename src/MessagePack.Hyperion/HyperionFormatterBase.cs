using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using CuteAnt.Buffers;
using Hyperion;
using Hyperion.SerializerFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CuteAnt;
using CuteAnt.Collections;
using CuteAnt.Reflection;

namespace MessagePack.Formatters
{
    public class HyperionFormatterBase<T> : DynamicObjectTypeFormatterBase<T>
    {
        private const int c_initialBufferSize = 1024 * 64;

        private readonly Serializer _serializer;

        protected HyperionFormatterBase(SerializerOptions options, Func<FieldInfo, bool> fieldFilter = null,
            IComparer<FieldInfo> fieldInfoComparer = null, Func<Type, bool> isSupportedFieldType = null)
            : base(fieldFilter, fieldInfoComparer, isSupportedFieldType)
        {
            if (null == options) { throw new ArgumentNullException(nameof(options)); }
            _serializer = new Serializer(options);
        }

        protected override object DeserializeObject(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var serializedObject = MessagePackBinary.ReadBytes(bytes, offset, out readSize);
            var ms = new MemoryStream(serializedObject);
            var result = _serializer.Deserialize<T>(ms);
            return result;
        }

        protected override int SerializeObject(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver)
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

            var size = MessagePackBinary.WriteBytes(ref bytes, offset, buffer, 0, bufferSize);
            bufferPool.Return(buffer);

            return size;
        }
    }
}
