using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class ValueTupleTest
    {
        T Convert<T>(T value)
        {
            return MessagePackSerializer.Deserialize<T>(MessagePackSerializer.Serialize(value));
        }

        public static IEnumerable<object[]> valueTupleData = new []
        {
            new object[] { (1, 2) },
            new object[] { (1, 2, 3) },
            new object[] { (1, 2, 3, 4) },
            new object[] { (1, 2, 3, 4, 5) },
            new object[] { (1, 2, 3, 4, 5,6) },
            new object[] { (1, 2, 3, 4, 5,6,7) },
            new object[] { (1, 2, 3, 4, 5,6,7,8) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16,17) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16,17,18) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19) },
            new object[] { (1, 2, 3, 4, 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20) },
        };

        [Theory]
        [MemberData(nameof(valueTupleData))]
        public void ValueTuple(object x)
        {
            var helper = typeof(ValueTupleTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(ValueTupleHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());

            helperClosedGeneric.Invoke(this, new object[] { x });
        }

        private void ValueTupleHelper<T>(T x)
        {
            Convert(x).Is(x);
        }
    }
}
