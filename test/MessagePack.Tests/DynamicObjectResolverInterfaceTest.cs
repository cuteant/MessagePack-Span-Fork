using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MessagePack.Tests
{
    public class DynamicObjectResolverInterfaceTest
    {
        [Fact]
        void TestConstructorWithParentInterface()
        {
            var myClass = new ConstructorEnumerableTest(new[] { "0", "2", "3" });
            var serialized = MessagePackSerializer.Serialize(myClass);
            var deserialized =
                MessagePackSerializer.Deserialize<ConstructorEnumerableTest>(serialized);
            deserialized.Values.IsStructuralEqual(myClass.Values);

            var myClass1 = new ConstructorEnumerableTest(new[] { "0", "2", "3" });
            var serialized1 = MessagePackSerializer.Serialize(myClass1);
            var deserialized1 =
                MessagePackSerializer.Deserialize<ConstructorEnumerableTest>(serialized1);
            deserialized1.Values.IsStructuralEqual(myClass1.Values);
        }

    }

    [MessagePackObject]
    public class ConstructorEnumerableTest
    {
        [SerializationConstructor]
        public ConstructorEnumerableTest(IEnumerable<string> values)
        {
            Values = values.ToList().AsReadOnly();
        }

        [Key(0)]
        public IReadOnlyList<string> Values { get; }
    }

    [MessagePackObject(true)]
    public class ConstructorEnumerableNameKeyTest
    {
        [SerializationConstructor]
        public ConstructorEnumerableNameKeyTest(IEnumerable<string> values)
        {
            Values = values.ToList().AsReadOnly();
        }

        public IReadOnlyList<string> Values { get; }
    }
}
