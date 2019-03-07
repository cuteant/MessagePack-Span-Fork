using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Tests.TestObjects;
using Xunit;
using JsonExtensions;

namespace CuteAnt.Extensions.Serialization.Tests
{
    public class JsonSerializerTests
    {
        [Fact]
        public void JsonSerializer_IsCheckAdditionalContentSet()
        {
            JsonSerializer jsonSerializer = null;
            Assert.Throws<ArgumentNullException>("jsonSerializer", () => jsonSerializer.IsCheckAdditionalContentSetX());

            jsonSerializer = JsonSerializer.CreateDefault();
            Assert.False(jsonSerializer.IsCheckAdditionalContentSetX());

            jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { CheckAdditionalContent = true });
            Assert.True(jsonSerializer.IsCheckAdditionalContentSetX());

            jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings { CheckAdditionalContent = false });
            Assert.True(jsonSerializer.IsCheckAdditionalContentSetX());

            jsonSerializer.SetCheckAdditionalContent(null);
            Assert.Null(jsonSerializer.GetCheckAdditionalContent());

            jsonSerializer.SetCheckAdditionalContent(true);
            Assert.True(jsonSerializer.GetCheckAdditionalContent());

            jsonSerializer.SetCheckAdditionalContent(false);
            Assert.False(jsonSerializer.GetCheckAdditionalContent());
        }

        [Fact]
        public void JsonSerializer_FormattingField_Test()
        {
            JsonSerializer jsonSerializer = null;
            Assert.Throws<ArgumentNullException>("jsonSerializer", () => jsonSerializer.GetFormatting());

            jsonSerializer = JsonSerializer.CreateDefault();

            jsonSerializer.SetFormatting(null);
            Assert.Null(jsonSerializer.GetFormatting());

            jsonSerializer.SetFormatting(Formatting.None);
            Assert.Equal(Formatting.None, jsonSerializer.GetFormatting());

            jsonSerializer.SetFormatting(Formatting.Indented);
            Assert.Equal(Formatting.Indented, jsonSerializer.GetFormatting());
        }

        [Fact]
        public void Json_Byte_Array_Test()
        {
            var jsonFormatter = JsonMessageFormatter.DefaultInstance;

            byte[] data = new byte[1];
            data[0] = 0;

            byte[] serializedData = jsonFormatter.SerializeObject(data);
            var json = System.Text.Encoding.UTF8.GetString(serializedData);
            Assert.NotNull(serializedData);

            byte[] deserializedData = (byte[])jsonFormatter.Deserialize(null, serializedData);
            Assert.NotNull(deserializedData);
            Assert.Equal(deserializedData.Length, data.Length);
            Assert.Equal(data[0], deserializedData[0]);
        }

        [Fact]
        public void Json_Empty_Byte_Array_Test()
        {
            var jsonFormatter = JsonMessageFormatter.DefaultInstance;

            byte[] data = new byte[0];
            byte[] serializedData = jsonFormatter.SerializeObject(data);
            Assert.NotNull(serializedData);
            byte[] deserializedData = jsonFormatter.Deserialize<byte[]>(serializedData);
            Assert.NotNull(deserializedData);
            Assert.Equal(deserializedData.Length, data.Length);
        }

        [Fact]
        public void Json_DateParseHandling_None_Test()
        {
            var product = new ProductShort { Name = "apple", ExpiryDate = DateTime.Now, Sizes = null };

            var json = JsonConvertX.SerializeObject(product, JsonConvertX.DefaultSettings);

            var newproduct = JsonConvertX.DeserializeObject<ProductShort>(json, JsonConvertX.DefaultSettings);
            Assert.Equal(product.ExpiryDate, newproduct.ExpiryDate);

            var settings = JsonConvertX.CreateSerializerSettings(Formatting.None);
            settings.DateParseHandling = DateParseHandling.None;
            newproduct = JsonConvertX.DeserializeObject<ProductShort>(json, JsonConvertX.DefaultSettings);
            Assert.Equal(product.ExpiryDate, newproduct.ExpiryDate);
        }

        [Fact]
        public void JsonObjectTypeDeserializer_Test()
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["name"] = "apple",
                ["Age"] = 18,
                ["Id"] = Guid.NewGuid(),
                ["createDateTime"] = DateTime.Now,
                ["combGuid"] = CombGuid.NewComb(),
                ["dtOffset"] = DateTimeOffset.Now,
                ["double"] = 10.56d,
                ["float"] = 20.06f,
                ["decimal"] = 8.000056M,
                ["bytes"] = new byte[] { 1 },
                ["emptybytes"] = new byte[0],
                ["Worten1"] = 0,
                ["Worten"] = "second",
            };

            var json = JsonConvertX.SerializeObject(dict, JsonConvertX.DefaultSettings);
            var newDict = JsonConvertX.DeserializeObject<Dictionary<string, object>>(json, JsonConvertX.DefaultSettings);
            Assert.Equal(dict["name"], JsonObjectTypeDeserializer.Deserialize<string>(newDict, "name"));
            Assert.Equal(dict["Age"], JsonObjectTypeDeserializer.Deserialize<int>(newDict, "Age"));
            Assert.Equal(dict["Id"], JsonObjectTypeDeserializer.Deserialize<Guid>(newDict, "Id"));
            // camelcase key test
            Assert.Equal(dict["createDateTime"], JsonObjectTypeDeserializer.Deserialize<DateTime>(newDict, "CreateDateTime"));
            Assert.Equal(dict["createDateTime"], JsonObjectTypeDeserializer.Deserialize<DateTimeOffset>(newDict, "CreateDateTime").DateTime);
            Assert.Equal(dict["combGuid"], JsonObjectTypeDeserializer.Deserialize<CombGuid>(newDict, "combGuid"));
            Assert.Equal(dict["dtOffset"], JsonObjectTypeDeserializer.Deserialize<DateTimeOffset>(newDict, "dtOffset"));
            Assert.Equal(dict["double"], JsonObjectTypeDeserializer.Deserialize<double>(newDict, "double"));
            Assert.Equal(dict["float"], JsonObjectTypeDeserializer.Deserialize<float>(newDict, "float"));
            Assert.Equal(dict["decimal"], JsonObjectTypeDeserializer.Deserialize<decimal>(newDict, "decimal"));
            Assert.Equal(dict["bytes"], JsonObjectTypeDeserializer.Deserialize<byte[]>(newDict, "bytes"));
            Assert.Equal(dict["emptybytes"], JsonObjectTypeDeserializer.Deserialize<byte[]>(newDict, "emptybytes"));
            Assert.Equal(Antworten.First, JsonObjectTypeDeserializer.Deserialize<Antworten>(newDict, "Worten1"));
            Assert.Equal(Antworten.Second, JsonObjectTypeDeserializer.Deserialize<Antworten>(newDict, "Worten"));

            Assert.Equal(dict["name"], newDict.Deserialize<string>("name"));
            Assert.Equal(dict["Age"], newDict.Deserialize<int>("Age"));
            Assert.Equal(dict["Id"], newDict.Deserialize<Guid>("Id"));
            // camelcase key test
            Assert.Equal(dict["createDateTime"], newDict.Deserialize<DateTime>("CreateDateTime"));
            Assert.Equal(dict["combGuid"], newDict.Deserialize<CombGuid>("combGuid"));
            Assert.Equal(dict["dtOffset"], newDict.Deserialize<DateTimeOffset>("dtOffset"));
            Assert.Equal(dict["double"], newDict.Deserialize<double>("double"));
            Assert.Equal(dict["float"], newDict.Deserialize<float>("float"));
            Assert.Equal(dict["decimal"], newDict.Deserialize<decimal>("decimal"));
            Assert.Equal(dict["bytes"], newDict.Deserialize<byte[]>("bytes"));
            Assert.Equal(dict["emptybytes"], newDict.Deserialize<byte[]>("emptybytes"));

            Assert.Equal(dict["name"], dict.Deserialize<string>("name"));
            Assert.Equal(dict["Age"], dict.Deserialize<int>("Age"));
            Assert.Equal(dict["Id"], dict.Deserialize<Guid>("Id"));
            // camelcase key test
            Assert.Equal(dict["createDateTime"], dict.Deserialize<DateTime>("CreateDateTime"));
            Assert.Equal(dict["combGuid"], dict.Deserialize<CombGuid>("combGuid"));
            Assert.Equal(dict["dtOffset"], dict.Deserialize<DateTimeOffset>("dtOffset"));
            Assert.Equal(dict["double"], dict.Deserialize<double>("double"));
            Assert.Equal(dict["float"], dict.Deserialize<float>("float"));
            Assert.Equal(dict["decimal"], dict.Deserialize<decimal>("decimal"));
            Assert.Equal(dict["bytes"], dict.Deserialize<byte[]>("bytes"));
            Assert.Equal(dict["emptybytes"], dict.Deserialize<byte[]>("emptybytes"));

            var combGuid = CombGuid.NewComb();
            var jsonStr = combGuid.ToString();
            Assert.Equal(combGuid, JsonObjectTypeDeserializer.Deserialize<CombGuid>(jsonStr));
            Assert.Equal(combGuid, JsonObjectTypeDeserializer.Deserialize<CombGuid>(combGuid));
            Assert.Equal((Guid)combGuid, JsonObjectTypeDeserializer.Deserialize<Guid>(combGuid));
            Assert.Equal(combGuid.ToString(), JsonObjectTypeDeserializer.Deserialize<string>(combGuid));
            Assert.Equal(combGuid.ToByteArray(), JsonObjectTypeDeserializer.Deserialize<byte[]>(combGuid));
            Assert.Equal(combGuid.DateTime, JsonObjectTypeDeserializer.Deserialize<DateTime>(combGuid));
            Assert.Equal(DateTime.UtcNow.Date, JsonObjectTypeDeserializer.Deserialize<DateTime>(combGuid).Date);
            Assert.Equal(new DateTimeOffset(combGuid.DateTime), JsonObjectTypeDeserializer.Deserialize<DateTimeOffset>(combGuid));
        }

        public class StringEmumObject
        {
            public Antworten Worten { get; set; }

            public MyEnum My { get; set; }
        }

        [Fact]
        public void Json_StringEnumConverter_Test()
        {
            var obj = new StringEmumObject() { Worten = Antworten.Second, My = MyEnum.Value2 };

            var json = JsonConvertX.SerializeObject(obj, JsonConvertX.IndentedSettings);
            var newobj = JsonConvertX.DeserializeObject<StringEmumObject>(json, JsonConvertX.DefaultDeserializerSettings);
            Assert.Equal(obj.Worten, newobj.Worten);
            Assert.Equal(obj.My, newobj.My);

            json = JsonConvertX.SerializeObject(obj, JsonConvertX.IndentedSettings);
            newobj = JsonConvertX.DeserializeObject<StringEmumObject>(json, JsonConvertX.DefaultDeserializerSettings);
            Assert.Equal(obj.Worten, newobj.Worten);
            Assert.Equal(obj.My, newobj.My);

            json = JsonConvertX.SerializeObject(obj, JsonConvertX.IndentedCamelCaseSettings);
            newobj = JsonConvertX.DeserializeObject<StringEmumObject>(json, JsonConvertX.DefaultDeserializerSettings);
            Assert.Equal(obj.Worten, newobj.Worten);
            Assert.Equal(obj.My, newobj.My);
        }

        public interface IPerson
        {
            string Name { get; set; }
            int Age { get; set; }
        }
        public class Staff : IPerson
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class Student : IPerson
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void Json_TypeNameHandling_Test()
        {
            IList<IPerson> persons = new List<IPerson>();
            persons.Add(new Staff { Name = "staff", Age = 32 });
            persons.Add(new Student { Name = "student", Age = 18 });

            var json = JsonConvertX.SerializeObject(persons, JsonConvertX.DefaultSettings);
            var newObj = JsonConvertX.DeserializeObject<IList<IPerson>>(json, JsonConvertX.DefaultDeserializerSettings);
            Assert.Equal(persons[0].Name, newObj[0].Name);
            Assert.Equal(persons[0].Age, newObj[0].Age);
            Assert.Equal(persons[1].Name, newObj[1].Name);
            Assert.Equal(persons[1].Age, newObj[1].Age);

            var jsonSettings = JsonConvertX.CreateSerializerSettings(Formatting.Indented, TypeNameHandling.All, TypeNameAssemblyFormatHandling.Simple);
            json = JsonConvertX.SerializeObject(persons, jsonSettings);
            newObj = JsonConvertX.DeserializeObject<IList<IPerson>>(json, JsonConvertX.DefaultDeserializerSettings);
            Assert.Equal(persons[0].Name, newObj[0].Name);
            Assert.Equal(persons[0].Age, newObj[0].Age);
            Assert.Equal(persons[1].Name, newObj[1].Name);
            Assert.Equal(persons[1].Age, newObj[1].Age);
        }
    }
}
