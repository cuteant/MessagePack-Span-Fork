using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class FloatConversionTest
    {
        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006f)]
        [InlineData(byte.MaxValue)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(int.MaxValue)]
        [InlineData(long.MaxValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(ulong.MaxValue)]
        public void FloatTest(object value)
        {
            var helper = typeof(FloatConversionTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(FloatTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(value.GetType());

            helperClosedGeneric.Invoke(this, new object[] { value });
        }

        private void FloatTestHelper<T>(T value)
        {
            var bin = MessagePackSerializer.Serialize(value);
            MessagePackSerializer.Deserialize<float>(bin).Is(Convert.ToSingle(value));
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006d)]
        [InlineData(byte.MaxValue)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(short.MaxValue)]
        [InlineData(int.MaxValue)]
        [InlineData(long.MaxValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(uint.MaxValue)]
        [InlineData(ulong.MaxValue)]
        public void DoubleTest(object value)
        {
            var helper = typeof(FloatConversionTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(DoubleTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(value.GetType());

            helperClosedGeneric.Invoke(this, new object[] { value });
        }

        private void DoubleTestHelper<T>(T value)
        {
            var bin = MessagePackSerializer.Serialize(value);
            MessagePackSerializer.Deserialize<double>(bin).Is(Convert.ToDouble(value));
        }
    }
}
