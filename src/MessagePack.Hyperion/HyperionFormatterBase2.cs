using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using CuteAnt;
using CuteAnt.Buffers;
using CuteAnt.Reflection;
using Hyperion;
using Hyperion.Extensions;

namespace MessagePack.Formatters
{
    public abstract class HyperionFormatterBase2<T> : DynamicObjectTypeFormatterBase<T>
    {
        private const int c_initialBufferSize = 1024 * 80;
        private static readonly HashSet<Type> s_primitiveTypes = new HashSet<Type>(new[]
        {
            typeof(Int32), typeof(Int64), typeof(Int16), typeof(UInt32), typeof(UInt64), typeof(UInt16),
            typeof(Single), typeof(Double), typeof(Decimal), typeof(Byte), typeof(SByte), typeof(Char),
            typeof(Boolean), typeof(TimeSpan), typeof(DateTime), typeof(DateTimeOffset), typeof(String),
            typeof(Guid), typeof(CombGuid)
        });

        protected HyperionFormatterBase2(Func<FieldInfo, bool> fieldFilter = null,
            IComparer<FieldInfo> fieldInfoComparer = null, Func<Type, bool> isSupportedFieldType = null)
            : base(fieldFilter, fieldInfoComparer, isSupportedFieldType)
        {
        }

        public override T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var startOffset = offset;

            var actualType = MessagePackBinary.ReadNamedType(bytes, offset, out var typeSize, true);
            var serializedObject = MessagePackBinary.ReadBytesSegment(bytes, offset + typeSize, out readSize);
            readSize += typeSize;

            var obj = ActivatorUtils.FastCreateInstance(actualType);
            var serializer = formatterResolver.GetContextValue<Serializer>(HyperionConstants.HyperionSerializer);
            using (var pooledSession = DeserializerSessionManager.Create(serializer))
            {
                var session = pooledSession.Object;
                session.TrackDeserializedObject(obj);
                session.TrackDeserializedType(actualType);

                using (var ms = new MemoryStream(serializedObject.Array, serializedObject.Offset, serializedObject.Count, false))
                {
                    var fields = GetFieldsFromCache(actualType);
                    foreach (var (field, getter, setter) in fields)
                    {
                        var fieldType = field.FieldType;
                        object fieldValue;
                        if (s_primitiveTypes.Contains(fieldType))
                        {
                            var valueSerializer = serializer.GetSerializerByType(fieldType);
                            fieldValue = valueSerializer.ReadValue(ms, session);
                        }
                        else
                        {
                            var valueType = fieldType;
                            if (fieldType.IsNullableType())
                            {
                                valueType = Nullable.GetUnderlyingType(fieldType);
                            }
                            var valueSerializer = serializer.GetSerializerByType(valueType);
                            fieldValue = ms.ReadObject(session);
                        }
                        setter(obj, fieldValue);
                    }
                }
            }

            return (T)obj;
        }

        public override int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var actualType = value.GetType();
            if (!IsSupportedType(actualType))
            {
                ThrowInvalidOperationException(actualType);
            }

            var typeSize = MessagePackBinary.WriteNamedType(ref bytes, offset, actualType);

            var bufferPool = BufferManager.Shared;
            byte[] buffer; int bufferSize;
            var serializer = formatterResolver.GetContextValue<Serializer>(HyperionConstants.HyperionSerializer);
            using (var pooledSession = SerializerSessionManager.Create(serializer))
            {
                var session = pooledSession.Object;
                session.TrackSerializedType(actualType);
                session.TrackSerializedObject(value);

                using (var pooledStream = BufferManagerOutputStreamManager.Create())
                {
                    var outputStream = pooledStream.Object;
                    outputStream.Reinitialize(c_initialBufferSize, bufferPool);

                    var fields = GetFieldsFromCache(actualType);
                    foreach (var (field, getter, setter) in fields)
                    {
                        var fieldType = field.FieldType;
                        var v = GetFieldValue(value, field, getter);
                        if (s_primitiveTypes.Contains(fieldType))
                        {
                            var valueSerializer = serializer.GetSerializerByType(fieldType);
                            valueSerializer.WriteValue(outputStream, v, session);
                        }
                        else
                        {
                            var valueType = fieldType;
                            if (fieldType.IsNullableType())
                            {
                                valueType = Nullable.GetUnderlyingType(fieldType);
                            }
                            var valueSerializer = serializer.GetSerializerByType(valueType);
                            outputStream.WriteObject(v, valueType, valueSerializer, true, session);
                        }
                    }

                    buffer = outputStream.ToArray(out bufferSize);
                }
            }

            try
            {
                return MessagePackBinary.WriteBytes(ref bytes, offset + typeSize, buffer, 0, bufferSize) + typeSize;
            }
            finally { bufferPool.Return(buffer); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowInvalidOperationException(Type type)
        {
            throw GetInvalidOperationException();
            InvalidOperationException GetInvalidOperationException()
            {
                return new InvalidOperationException($"Type '{type}' is an interface or abstract class and cannot be serialized.");
            }
        }
    }
}
