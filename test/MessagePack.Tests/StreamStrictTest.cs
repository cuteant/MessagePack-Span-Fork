using System;
using System.IO;
using MessagePack.Internal;
using SharedData;
using Xunit;

namespace MessagePack.Tests
{
    public class StreamStrictTest
    {
        static void SerializeWithLengthPrefixExt<T>(Stream stream, T data, IFormatterResolver resolver)
        {
            const int ExtTypeCode = 111; // sample ext code

            var serializedData = MessagePackSerializer.Serialize(data, resolver);
            var tmp = MessagePackSerializer.Deserialize<T>(serializedData, resolver);

            var dataSize = serializedData.Length;
            var idx = 0;
            var writer = new MessagePackWriter(16);

            var headerLength = MessagePackBinary.GetExtensionFormatHeaderLength(dataSize);
            writer.Ensure(idx, headerLength + dataSize);
            writer.WriteExtensionFormatHeader(ExtTypeCode, dataSize, ref idx);
            UnsafeMemory.WriteRaw(ref writer, serializedData, ref idx);
            var buffer = writer.ToArray(idx);
            stream.Write(buffer);
        }

        static T DeserializeWithLengthPrefixExt<T>(Stream stream, IFormatterResolver resolver)
        {
            const int ExtTypeCode = 111; // sample ext code

            using (var output = new ArrayBufferWriter(16))
            {
                MessagePackBinary.ReadMessageBlockFromStreamUnsafe(stream, output);

                var reader = new MessagePackReader(output.WrittenSpan);
                var header = reader.ReadExtensionFormatHeader();
                if (header.TypeCode == ExtTypeCode)
                {
                    // memo, read fully
                    var valueSpan = reader.Peek((int)header.Length);
                    var reader0 = new MessagePackReader(valueSpan);
                    return resolver.GetFormatter<T>().Deserialize(ref reader0, resolver);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        static T DeserializeWithLengthPrefixExt2<T>(Stream stream, IFormatterResolver resolver)
        {
            const int ExtTypeCode = 111; // sample ext code

            using (var output = new ArrayBufferWriter(16))
            {
                MessagePackBinary.ReadMessageBlockFromStreamUnsafe(stream, output);

                var reader = new MessagePackReader(output.WrittenSpan);
                var header = reader.ReadExtensionFormat();
                if (header.TypeCode == ExtTypeCode)
                {
                    return MessagePackSerializer.Deserialize<T>(header.Data, resolver);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        [Fact]
        public void Deserialize()
        {
            var testData = new SharedData.MyClass { MyProperty1 = int.MaxValue, MyProperty2 = int.MaxValue, MyProperty3 = int.MaxValue };

            var ms = new MemoryStream();

            SerializeWithLengthPrefixExt(ms, testData, Resolvers.ContractlessStandardResolver.Instance);

            ms.Position = 0;
            var data = DeserializeWithLengthPrefixExt<MyClass>(ms, Resolvers.ContractlessStandardResolver.Instance);
        }

        [Fact]
        public void Deserialize2()
        {
            var testData = new SharedData.MyClass { MyProperty1 = int.MaxValue, MyProperty2 = 99, MyProperty3 = 1341 };

            Stream ms = new MemoryStream();

            SerializeWithLengthPrefixExt(ms, testData, Resolvers.ContractlessStandardResolver.Instance);

            ms.Position = 0;

            ms = new FixedSizeReadStream(ms, 2);

            var data = DeserializeWithLengthPrefixExt2<MyClass>(ms, Resolvers.ContractlessStandardResolver.Instance);
        }
    }

    public class FixedSizeReadStream : Stream
    {
        readonly Stream stream;
        readonly int readSize;

        public FixedSizeReadStream(Stream stream, int readSize)
        {
            this.stream = stream;
            this.readSize = readSize;
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

#if NETCOREAPP_2_0_GREATER
        public override int Read(Span<byte> buffer)
        {
            return stream.Read(buffer);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            stream.Write(buffer);
        }
#endif

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, Math.Min(readSize, count));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
