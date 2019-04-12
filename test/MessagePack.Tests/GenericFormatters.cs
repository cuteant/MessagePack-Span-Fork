using System;
using System.Collections.Generic;
using Xunit;

namespace MessagePack.Tests
{
    public class GenericFormatters
    {
        object Convert(object value)
        {
            return MessagePackSerializer.DeepCopy(value);
        }

        public static IEnumerable<object[]> tupleTestData = new []
        {
            new object[] { Tuple.Create(1) },
            new object[] { Tuple.Create(1,2) },
            new object[] { Tuple.Create(1,2,3) },
            new object[] { Tuple.Create(1,2,3,4) },
            new object[] { Tuple.Create(1,2,3,4,5) },
            new object[] { Tuple.Create(1,2,3,4,5,6) },
            new object[] { Tuple.Create(1,2,3,4,5,6,7) },
            new object[] { Tuple.Create(1,2,3,4,5,6,7,8) },
        };

        [Theory()]
        [MemberData(nameof(tupleTestData))]
        public void TupleTest(object data)
        {
            Convert(data).IsStructuralEqual(data);
        }

        public static IEnumerable<object[]> valueTupleTestData = new []
        {
            new object[] { ValueTuple.Create(1),null },
            new object[] { ValueTuple.Create(1,2),null },
            new object[] { ValueTuple.Create(1,2,3),null },
            new object[] { ValueTuple.Create(1,2,3,4),null },
            new object[] { ValueTuple.Create(1,2,3,4,5) ,null},
            new object[] { ValueTuple.Create(1,2,3,4,5,6) ,null},
            new object[] { ValueTuple.Create(1,2,3,4,5,6,7) ,null},
            new object[] { ValueTuple.Create(1,2,3,4,5,6,7,8) ,null},
        };

        [Theory()]
        [MemberData(nameof(valueTupleTestData))]
        public void TupleTest2(object data, object @null)
        {
            Convert(data).IsStructuralEqual(data);
            Convert(@null).IsNull();
        }

        public static IEnumerable<object[]> keyValuePairData = new []
        {
            new object[] { new KeyValuePair<int, int>(1,2), null },
            new object[] { new KeyValuePair<int, int>(3,4), new KeyValuePair<int, int>(5,6) },
        };

        [Theory()]
        [MemberData(nameof(keyValuePairData))]
        public void KeyValuePairTest(object t, object t2)
        {
            Convert(t).IsStructuralEqual(t);
            Convert(t2).IsStructuralEqual(t2);
        }

        public static IEnumerable<object[]> byteArraySegementData = new []
        {
            new object[] { new ArraySegment<byte>(new byte[] { 0, 0, 1, 2, 3 }, 2, 3), null, new byte[] { 1, 2, 3 }  },
            new object[] { new ArraySegment<byte>(new byte[0], 0, 0), null, new byte[0] },
        };

        [Theory()]
        [MemberData(nameof(byteArraySegementData))]
        public void ByteArraySegmentTest(ArraySegment<byte> t, ArraySegment<byte>? t2, byte[] reference)
        {
            MessagePackSerializer.Serialize(t).Is(MessagePackSerializer.Serialize(reference));
            ((ArraySegment<byte>)Convert(t)).Array.Is(reference);
            var reader = new MessagePackReader(MessagePackSerializer.Serialize(t2));
            reader.IsNil().IsTrue();
        }


    }
}
