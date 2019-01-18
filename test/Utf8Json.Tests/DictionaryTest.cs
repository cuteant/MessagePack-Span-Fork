using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Utf8Json.Tests
{
    public class DictionaryTest
    {
        T Convert<T>(T value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }

        public static IEnumerable<object[]> dictionaryTestData = new []
        {
            new object[]{ new Dictionary<int, int>() { { 1, 100 } }, null },
#if !TEST40
            new object[]{ new ReadOnlyDictionary<int,int>(new Dictionary<int, int>() { { 1, 100 } }), null },
#endif
            new object[]{ new SortedList<int, int>() { { 1, 100 } }, null },
            new object[]{ new SortedDictionary<int, int>() { { 1, 100 } }, null },
        };

        [Theory]
        [MemberData(nameof(dictionaryTestData))]
        public void DictionaryTestAll<T>(T x, T y)
        {
            Convert(x).IsStructuralEqual(x);
            Convert(y).IsStructuralEqual(y);
        }

        [Fact]
        public void InterfaceDictionaryTest()
        {
            var a = (IDictionary<int, int>)new Dictionary<int, int>() { { 1, 100 } };
#if !TEST40
            var b = (IReadOnlyDictionary<int, int>)new Dictionary<int, int>() { { 1, 100 } };
#endif
            var c = (IDictionary<int, int>)null;
#if !TEST40
            var d = (IReadOnlyDictionary<int, int>)null;
#endif

            var huga = JsonSerializer.ToJsonString(a);

            Convert(a).IsStructuralEqual(a);
#if !TEST40
            Convert(b).IsStructuralEqual(b);
#endif
            Convert(c).IsStructuralEqual(c);
#if !TEST40
            Convert(d).IsStructuralEqual(d);
#endif
        }

        [Fact]
        public void ConcurrentDictionaryTest()
        {
            var cd = new ConcurrentDictionary<int, int>();

            cd.TryAdd(1, 100);
            cd.TryAdd(2, 200);
            cd.TryAdd(3, 300);

            var conv = Convert(cd);
            conv[1].Is(100);
            conv[2].Is(200);
            conv[3].Is(300);

            cd = null;
            Convert(cd).IsNull();
        }

        [Fact]
        public void DictionaryObjectValueTest()
        {
            var x = new Dictionary<string, object>() { { "a", 100 }, { "b", "str" }, { "c", Guid.NewGuid() } };

            var conv = Convert(x);
            Assert.NotNull(conv);
        }
    }
}
