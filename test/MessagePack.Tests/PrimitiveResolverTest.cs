using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class PrimitiveResolverTest
    {
        [Theory]
        [InlineData((bool)true)]
        [InlineData((byte)10)]
        [InlineData((sbyte)123)]
        [InlineData((short)(10))]
        [InlineData((short)(200))]
        [InlineData((short)(1000))]
        [InlineData((short)(-10))]
        [InlineData((short)(-32))]
        [InlineData((short)(-60))]
        [InlineData((short)(-128))]
        [InlineData((short)(-190))]
        [InlineData((short)(-4123))]
        [InlineData((ushort)42342)]
        [InlineData((int)(127))]
        [InlineData((int)(255))]
        [InlineData((int)(ushort.MaxValue))]
        [InlineData((int)(ushort.MaxValue + 1))]
        [InlineData((int)(int.MaxValue))]
        [InlineData((int)(-31))]
        [InlineData((int)(-127))]
        [InlineData((int)(short.MinValue))]
        [InlineData((int)(-ushort.MaxValue))]
        [InlineData((UInt32)432423u)]
        [InlineData((long)(235L))]
        [InlineData((UInt64)65346464UL)]
        [InlineData((float)1241.42342f)]
        [InlineData((double)1241312.4242342D)]
        [InlineData("hogehoge")]
        [InlineData(new byte[] { 1, 10, 100 })]
        public void PrimitiveObjectTest(object x)
        {
            var bin = MessagePackSerializer.Serialize<object>(x);

            var helper = typeof(PrimitiveResolverTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(PrimitiveObjectTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());
            var bin2 = helperClosedGeneric.Invoke(this, new object[] { x });

            bin.Is(bin2);
            //var re1 = MessagePackSerializer.Deserialize<object>(bin);
            //((T)re1).Is(x);
        }

        private byte[] PrimitiveObjectTestHelper<T>(T x)
        {
            return MessagePackSerializer.Serialize<T>(x);
        }

        [Fact]
        public void PrimitiveTest2()
        {
            {
                var x = DateTime.UtcNow;
                var bin = MessagePackSerializer.Serialize<object>(x);
                var re1 = MessagePackSerializer.Deserialize<object>(bin);
                (re1).Is(x);
            }
            {
                var x = 'あ';
                var bin = MessagePackSerializer.Serialize<object>(x);
                var re1 = MessagePackSerializer.Deserialize<object>(bin);
                ((char)(ushort)re1).Is(x);
            }
            {
                var x = SharedData.IntEnum.C;
                var bin = MessagePackSerializer.Serialize<object>(x);
                var re1 = MessagePackSerializer.Deserialize<object>(bin);
                ((SharedData.IntEnum)(int)(byte)re1).Is(x);
            }
            {
                var x = new object[] { 1, 10, 1000, new[] { 999, 424 }, new Dictionary<string, int> { { "hoge", 100 }, { "foo", 999 } }, true };

                var bin = MessagePackSerializer.Serialize<object>(x);
                var re1 = (object[])MessagePackSerializer.Deserialize<object>(bin);

                x[0].Is((int)(byte)re1[0]);
                x[1].Is((int)(byte)re1[1]);
                x[2].Is((int)(ushort)re1[2]);
                x[5].Is(re1[5]);

                ((int[])x[3])[0].Is((ushort)((object[])re1[3])[0]);
                ((int[])x[3])[1].Is((ushort)((object[])re1[3])[1]);

                (x[4] as Dictionary<string, int>)["hoge"].Is((int)(byte)(re1[4] as Dictionary<object, object>)["hoge"]);
                (x[4] as Dictionary<string, int>)["foo"].Is((ushort)(re1[4] as Dictionary<object, object>)["foo"]);
            }
        }
    }
}
