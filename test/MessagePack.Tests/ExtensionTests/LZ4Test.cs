using SharedData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests.ExtensionTests
{

    public class LZ4Test
    {
        T Convert<T>(T value)
        {
            var resolver = new WithImmutableDefaultResolver();
            return LZ4MessagePackSerializer.Deserialize<T>(LZ4MessagePackSerializer.Serialize(value, resolver), resolver);
        }


        [Fact]
        public void TestSmall()
        {
            // small size binary don't use LZ4 Encode
            (new MessagePackReader(LZ4MessagePackSerializer.Serialize(100))).GetMessagePackType().Is(MessagePackType.Integer);
            (new MessagePackReader(LZ4MessagePackSerializer.Serialize("test"))).GetMessagePackType().Is(MessagePackType.String);
            (new MessagePackReader(LZ4MessagePackSerializer.Serialize(false))).GetMessagePackType().Is(MessagePackType.Boolean);
        }

        [Fact]
        public void CompressionData()
        {
            var originalData = Enumerable.Range(1, 1000).Select(x => x).ToArray();

            var lz4Data = LZ4MessagePackSerializer.Serialize(originalData);
            var reader = new MessagePackReader(lz4Data);

            reader.GetMessagePackType().Is(MessagePackType.Extension);
            var header = reader.ReadExtensionFormatHeader();
            header.TypeCode.Is((sbyte)LZ4MessagePackSerializer.ExtensionTypeCode);

            var decompress = LZ4MessagePackSerializer.Deserialize<int[]>(lz4Data);

            decompress.Is(originalData);
        }

        [Fact]
        public void NonGenericAPI()
        {
            var originalData = Enumerable.Range(1, 100).Select(x => new FirstSimpleData { Prop1 = x * x, Prop2 = "hoge", Prop3 = x }).ToArray();

            var lz4Data = LZ4MessagePackSerializer.NonGeneric.Serialize(typeof(FirstSimpleData[]), originalData);
            var reader = new MessagePackReader(lz4Data);

            reader.GetMessagePackType().Is(MessagePackType.Extension);
            var header = reader.ReadExtensionFormatHeader();
            header.TypeCode.Is((sbyte)LZ4MessagePackSerializer.ExtensionTypeCode);

            var decompress = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), lz4Data);

            decompress.IsStructuralEqual(originalData);
        }

        [Fact]
        public void StreamAPI()
        {
            var originalData = Enumerable.Range(1, 100).Select(x => new FirstSimpleData { Prop1 = x * x, Prop2 = "hoge", Prop3 = x }).ToArray();

            var ms = new MemoryStream();
            LZ4MessagePackSerializer.NonGeneric.Serialize(typeof(FirstSimpleData[]), ms, originalData);

            var lz4normal = LZ4MessagePackSerializer.NonGeneric.Serialize(typeof(FirstSimpleData[]), originalData);

            ms.Position = 0;

            lz4normal.SequenceEqual(ms.ToArray()).IsTrue();

            var decompress1 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), ms.ToArray());
            var decompress2 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), lz4normal);
            var decompress3 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), ms);

            decompress1.IsStructuralEqual(originalData);
            decompress2.IsStructuralEqual(originalData);
            decompress3.IsStructuralEqual(originalData);
        }

        [Fact]
        public void ArraySegmentAPI()
        {
            var originalData = Enumerable.Range(1, 100).Select(x => new FirstSimpleData { Prop1 = x * x, Prop2 = "hoge", Prop3 = x }).ToArray();

            var ms = new MemoryStream();
            LZ4MessagePackSerializer.NonGeneric.Serialize(typeof(FirstSimpleData[]), ms, originalData);
            ms.Position = 0;

            var lz4normal = LZ4MessagePackSerializer.Serialize(originalData);

            var paddingOffset = 10;
            var paddedLz4Normal = new byte[lz4normal.Length + paddingOffset + paddingOffset];
            Array.Copy(lz4normal, 0, paddedLz4Normal, paddingOffset, lz4normal.Length);

            var decompress1 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), ms.ToArray());
            var decompress2 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), lz4normal);
            var decompress3 = LZ4MessagePackSerializer.NonGeneric.Deserialize(typeof(FirstSimpleData[]), ms);
            var decompress4 = LZ4MessagePackSerializer.Deserialize<FirstSimpleData[]>(lz4normal);
            var decompress5 = LZ4MessagePackSerializer.Deserialize<FirstSimpleData[]>(new ArraySegment<byte>(lz4normal));
            var decompress6 = LZ4MessagePackSerializer.Deserialize<FirstSimpleData[]>(new ArraySegment<byte>(paddedLz4Normal, paddingOffset, lz4normal.Length));

            decompress1.IsStructuralEqual(originalData);
            decompress2.IsStructuralEqual(originalData);
            decompress3.IsStructuralEqual(originalData);
            decompress4.IsStructuralEqual(originalData);
            decompress5.IsStructuralEqual(originalData);
            decompress6.IsStructuralEqual(originalData);
        }

        [Fact]
        public void SerializeToBlock()
        {
            var originalData = Enumerable.Range(1, 1000).Select(x => x).ToArray();

            var idx = 0;
            var writer = new MessagePackWriter(16);

#pragma warning disable CS0618 // 类型或成员已过时
            LZ4MessagePackSerializer.SerializeToBlock(ref writer, ref idx, originalData, MessagePackSerializer.DefaultResolver);
#pragma warning restore CS0618 // 类型或成员已过时

            var lz4Data = LZ4MessagePackSerializer.Serialize(originalData);

            writer.ToArray(idx).AsSpan().SequenceEqual(lz4Data).Is(true);
        }

        [Fact]
        public void Decode()
        {
            var originalData = Enumerable.Range(1, 100).Select(x => new FirstSimpleData { Prop1 = x * x, Prop2 = "hoge", Prop3 = x }).ToArray();
            var simple = LZ4MessagePackSerializer.Serialize(100);
            var complex = LZ4MessagePackSerializer.Serialize(originalData);

            var msgpack1 = LZ4MessagePackSerializer.Decode(simple);
            var msgpack2 = LZ4MessagePackSerializer.Decode(complex);

            MessagePackSerializer.Deserialize<int>(msgpack1).Is(100);
            MessagePackSerializer.Deserialize<FirstSimpleData[]>(msgpack2).IsStructuralEqual(originalData);
        }

#if NET471
        [Fact]
        public void LZ4NetEncodeAndK4osCompressionDecode()
        {
            var text = "this is a test";
            var originalData = Encoding.UTF8.GetBytes(text);
            var lz4Data = new byte[LZ4.LZ4Codec.MaximumOutputLength(originalData.Length)];
            var lz4Size = LZ4.LZ4Codec.Encode(originalData, 0, originalData.Length, lz4Data, 0, lz4Data.Length);
            var output = new byte[lz4Size * 255];
            var dl = K4os.Compression.LZ4.LZ4Codec.Decode(lz4Data, 0, lz4Size, output, 0, output.Length);
            Assert.Equal(text, Encoding.UTF8.GetString(output, 0, dl));
        }

        [Fact]
        public void K4osCompressionEncodeAndLZ4NetDecode()
        {
            var text = "this is a test";
            var originalData = Encoding.UTF8.GetBytes(text);
            var lz4Data = new byte[K4os.Compression.LZ4.LZ4Codec.MaximumOutputSize(originalData.Length)];
            var lz4Size = K4os.Compression.LZ4.LZ4Codec.Encode(originalData, 0, originalData.Length, lz4Data, 0, lz4Data.Length);
            var output = new byte[lz4Size * 255];
            var dl = LZ4.LZ4Codec.Decode(lz4Data, 0, lz4Size, output, 0, output.Length);
            Assert.Equal(text, Encoding.UTF8.GetString(output, 0, dl));
        }
#endif
    }
}
