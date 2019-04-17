using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharedData;
using Xunit;

namespace MessagePack.Tests
{
    public class FormatterTest
    {
        T Convert<T>(T value)
        {
            return MessagePackSerializer.Deserialize<T>(MessagePackSerializer.Serialize(value));
        }

        public static IEnumerable<object[]> primitiveFormatterTestData = new[]
        {
            new object[] { Int16.MinValue, Int16.MaxValue },
            new object[] { (Int16?)100, null },
            new object[] { Int32.MinValue, Int32.MaxValue },
            new object[] { (Int32?)100, null },
            new object[] { (Int32)127, (Int32)255 },
            new object[] { (Int32)ushort.MaxValue, (Int32)(ushort.MaxValue + 1) },
            new object[] { Int64.MinValue, Int64.MaxValue },
            new object[] { (Int64?)100L, null },
            new object[] { UInt16.MinValue, UInt16.MaxValue },
            new object[] { (UInt16?)100, null },
            new object[] { UInt32.MinValue, UInt32.MaxValue },
            new object[] { (UInt32?)100u, null },
            new object[] { UInt64.MinValue, UInt64.MaxValue },
            new object[] { (UInt64?)100ul, null },
            new object[] { Single.MinValue, Single.MaxValue },
            new object[] { (Single?)100.100f, null },
            new object[] { Double.MinValue, Double.MaxValue },
            new object[] { (Double?)100.100d, null },
            new object[] { true, false },
            new object[] { (Boolean?)true, null },
            new object[] { Byte.MinValue, Byte.MaxValue },
            new object[] { (Byte?)100.100, null },
            new object[] { SByte.MinValue, SByte.MaxValue },
            new object[] { (SByte?)100.100, null },
            new object[] { Char.MinValue, Char.MaxValue },
            new object[] { (Char?)'a', null },
            new object[] { DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime() },
            new object[] { (DateTime?)DateTime.UtcNow, null },
        };

        [Theory]
        [MemberData(nameof(primitiveFormatterTestData))]
        public void PrimitiveFormatterTest(object x, object y)
        {
            var helper = typeof(FormatterTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(PrimitiveFormatterTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());

            helperClosedGeneric.Invoke(this, new object[] { x });
            helperClosedGeneric.Invoke(this, new object[] { y });
        }

        private void PrimitiveFormatterTestHelper<T>(T? x)
            where T : struct
        {
            Convert(x).Is(x);
        }

        public static IEnumerable<object[]> enumFormatterTestData = new[]
        {
            new object[] { ByteEnum.A, ByteEnum.B },
            new object[] { (ByteEnum?)ByteEnum.C, null },
            new object[] { SByteEnum.A, SByteEnum.B },
            new object[] { (SByteEnum?)SByteEnum.C, null },
            new object[] { ShortEnum.A, ShortEnum.B },
            new object[] { (ShortEnum?)ShortEnum.C, null },
            new object[] { UShortEnum.A, UShortEnum.B },
            new object[] { (UShortEnum?)UShortEnum.C, null },
            new object[] { IntEnum.A, IntEnum.B },
            new object[] { (IntEnum?)IntEnum.C, null },
            new object[] { UIntEnum.A, UIntEnum.B },
            new object[] { (UIntEnum?)UIntEnum.C, null },
            new object[] { LongEnum.A, LongEnum.B },
            new object[] { (LongEnum?)LongEnum.C, null },
            new object[] { ULongEnum.A, ULongEnum.B },
            new object[] { (ULongEnum?)ULongEnum.C, null },
        };

        [Theory]
        [MemberData(nameof(enumFormatterTestData))]
        public void EnumFormatterTest(object x, object y)
        {
            var helper = typeof(FormatterTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(EnumFormatterTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());

            helperClosedGeneric.Invoke(this, new object[] { x });
            helperClosedGeneric.Invoke(this, new object[] { y });
        }

        private void EnumFormatterTestHelper<T>(T? x)
            where T : struct
        {
            Convert(x).Is(x);
        }

        [Fact]
        public void NilFormatterTest()
        {
            Convert(Nil.Default).Is(Nil.Default);
            Convert((Nil?)null).Is(Nil.Default);
        }

        public static IEnumerable<object[]> standardStructFormatterTestData = new[]
        {
            new object[] { decimal.MaxValue, decimal.MinValue, null },
            new object[] { TimeSpan.MaxValue, TimeSpan.MinValue, null },
            new object[] { DateTimeOffset.MaxValue, DateTimeOffset.MinValue, null },
            new object[] { Guid.NewGuid(), Guid.Empty, null },
            new object[] { new KeyValuePair<int,string>(10, "hoge"), default(KeyValuePair<int, string>), null },
            new object[] { System.Numerics.BigInteger.Zero, System.Numerics.BigInteger.One, null },
            new object[] { System.Numerics.Complex.Zero, System.Numerics.Complex.One, null },
        };

        [Fact]
        public void PrimitiveStringTest()
        {
            Convert("a").Is("a");
            Convert("test").Is("test");
            Convert("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")
                .Is("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest");
            Convert((string)null).IsNull();
        }

        [Theory]
        [MemberData(nameof(standardStructFormatterTestData))]
        public void StandardClassLibraryStructFormatterTest(object x, object y, object z)
        {
            var helper = typeof(FormatterTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(StandardClassLibraryStructFormatterTest_Helper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());

            helperClosedGeneric.Invoke(this, new object[] { x });
            helperClosedGeneric.Invoke(this, new object[] { y });
            helperClosedGeneric.Invoke(this, new object[] { z });
        }

        private void StandardClassLibraryStructFormatterTest_Helper<T>(T? value) where T : struct => Convert(value).Is(value);

        public static IEnumerable<object[]> standardClassFormatterTestData = new[]
        {
            new object[] { new byte[] { 1, 10, 100 }, new byte[0] { }, null },
            new object[] { "aaa", "", null },
            new object[] { new Uri("Http://hogehoge.com"), new Uri("Https://hugahuga.com"), null },
            new object[] { new Version(0,0), new Version(1,2,3), new Version(255,100,30) },
            new object[] { new Version(1,2), new Version(100, 200,300,400), null },
            new object[] { new BitArray(new[] { true, false, true }), new BitArray(1), null },
        };

        [Theory]
        [MemberData(nameof(standardClassFormatterTestData))]
        public void StandardClassLibraryFormatterTest(object x, object y, object z)
        {
            var helper = typeof(FormatterTest).GetTypeInfo().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(m => m.Name == nameof(StandardClassLibraryFormatterTestHelper));
            var helperClosedGeneric = helper.MakeGenericMethod(x.GetType());

            helperClosedGeneric.Invoke(this, new object[] { x });
            helperClosedGeneric.Invoke(this, new object[] { y });
            helperClosedGeneric.Invoke(this, new object[] { z });
        }

        private void StandardClassLibraryFormatterTestHelper<T>(T x)
        {
            Convert(x).Is(x);
        }

        [Fact]
        public void StringBuilderTest()
        {
            var sb = new StringBuilder("aaa");
            Convert(sb).ToString().Is("aaa");

            StringBuilder nullSb = null;
            Convert(nullSb).IsNull();
        }

        [Fact]
        public void LazyTest()
        {
            var lz = new Lazy<int>(() => 100);
            Convert(lz).Value.Is(100);

            Lazy<int> nullLz = null;
            Convert(nullLz).IsNull();
        }

#if !TEST40
        [Fact]
        public void TaskTest()
        {
            var intTask = Task.Run(() => 100);
            Convert(intTask).Result.Is(100);

            Task<int> nullTask = null;
            Convert(nullTask).IsNull();

            Task unitTask = Task.Run(() => 100);
            Convert(unitTask).Status.Is(TaskStatus.RanToCompletion);

            Task nullUnitTask = null;
            Convert(nullUnitTask).Status.Is(TaskStatus.RanToCompletion); // write to nil

            ValueTask<int> valueTask = new ValueTask<int>(100);
            Convert(valueTask).Result.Is(100);

            ValueTask<int>? nullValueTask = new ValueTask<int>(100);
            Convert(nullValueTask).Value.Result.Is(100);

            ValueTask<int>? nullValueTask2 = null;
            Convert(nullValueTask2).IsNull();
        }
#endif

        [Fact]
        public void DateTimeOffsetTest()
        {
            DateTimeOffset now = new DateTime(DateTime.UtcNow.Ticks + TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time").BaseUtcOffset.Ticks, DateTimeKind.Local);
            var binary = MessagePackSerializer.Serialize(now);
            MessagePackSerializer.Deserialize<DateTimeOffset>(binary).Is(now);
        }

        [Fact]
        public void StringTest_Part2()
        {
            var a = "あいうえお";
            var b = new String('あ', 20);
            var c = new String('あ', 130);
            var d = new String('あ', 40000);

            var idx = 0;
            var writer = new MessagePackWriter(16);
            writer.WriteString(a, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(a) + 1);
            var reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(a);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteStringWithCache(a, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(a) + 1);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadStringWithCache().Is(a);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(b, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(b) + 2);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(b);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteStringWithCache(b, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(b) + 2);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadStringWithCache().Is(b);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(c, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(c) + 3);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(c);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(d, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(d) + 5);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(d);
        }

        [Fact]
        public void StringTest_Part()
        {
            var a = "身无彩凤双飞翼，心有灵犀一点通。";
            var b = new String('笑', 20);
            var c = new String('风', 130);
            var d = new String('电', 40000);

            var idx = 0;
            var writer = new MessagePackWriter(16);
            writer.WriteString(a, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(a) + 2);
            var reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(a);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteStringWithCache(a, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(a) + 2);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadStringWithCache().Is(a);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(b, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(b) + 2);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(b);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteStringWithCache(b, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(b) + 2);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadStringWithCache().Is(b);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(c, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(c) + 3);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(c);

            writer = new MessagePackWriter(16);
            idx = 0;
            writer.WriteString(d, ref idx);
            idx.Is(Encoding.UTF8.GetByteCount(d) + 5);
            reader = new MessagePackReader(writer.ToArray(idx));
            reader.ReadString().Is(d);
        }

        // https://github.com/neuecc/MessagePack-CSharp/issues/22
        [Fact]
        public void DecimalLang()
        {
#if NET_4_5_GREATER
            var estonian = new CultureInfo("et-EE");
            CultureInfo.CurrentCulture = estonian;
#endif

            var b = MessagePackSerializer.Serialize(12345.6789M);
            var d = MessagePackSerializer.Deserialize<decimal>(b);

            d.Is(12345.6789M);
        }

        [Fact]
        public void UriTest()
        {
            var absolute = new Uri("http://google.com/");
            Convert(absolute).ToString().Is("http://google.com/");

            var relative = new Uri("/me/", UriKind.Relative);
            Convert(relative).ToString().Is("/me/");
        }
    }
}
