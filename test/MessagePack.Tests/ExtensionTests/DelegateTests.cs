using System;
using System.Globalization;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests.ExtensionTests
{
    public class DelegateTests
    {
        public class Dummy
        {
            public int Prop { get; set; }
        }

        public class HasClosure
        {
            public int A { get; set; }
            public Func<int> Del;
            public string W { get; set; }

            public void Create()
            {
                A = 10;
                W = "test";
                var a = 3;
                Del = () => a + 1;
            }
        }

        [Fact]
        public void CanSerializeMemberMethod()
        {
            Func<string> a = 123.ToString;
            var bytes = MessagePackSerializer.Serialize(a);
            var json = MessagePackSerializer.ToJson(bytes);
            var res = MessagePackSerializer.Deserialize<Func<string>>(bytes);
            Assert.NotNull(res);
            var actual = res();
            Assert.Equal("123", actual);

            res = MessagePackSerializer.DeepCopy(a);
            Assert.NotNull(res);
            actual = res();
            Assert.Equal("123", actual);

            res = (Func<string>)MessagePackSerializer.Typeless.DeepCopy(a);
            Assert.NotNull(res);
            actual = res();
            Assert.Equal("123", actual);
        }

        [Fact]
        public void CanSerializeDelegate()
        {
            Action<Dummy> a = dummy => dummy.Prop = 1;
            var bytes = MessagePackSerializer.Serialize(a, ContractlessStandardResolverAllowPrivate.Instance);
            var res = MessagePackSerializer.Deserialize<Action<Dummy>>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
            Assert.NotNull(res);

            var d = new Dummy { Prop = 0 };
            res(d);
            Assert.Equal(1, d.Prop);
        }

        delegate bool TryGet(int key, out int value);

        private bool TryGetImpl(int key, out int value)
        {
            value = key + 1;
            return true;
        }

        delegate void TransIndex(ref int index);

        private void TransIndexImpl(ref int index)
        {
            index += 10;
        }

        [Fact]
        public void CanSerializeMemberMethod1()
        {
            TryGet tryGet = TryGetImpl;
            var bytes = MessagePackSerializer.Serialize(tryGet, DefaultResolver.Instance);
            var res = MessagePackSerializer.Deserialize<TryGet>(bytes, DefaultResolver.Instance);
            Assert.NotNull(res);
            var key = 1;
            Assert.True(res(key, out var value));
            Assert.Equal(2, value);

            TransIndex trans = TransIndexImpl;
            bytes = MessagePackSerializer.Serialize(trans, DefaultResolver.Instance);
            var res1 = MessagePackSerializer.Deserialize<TransIndex>(bytes, DefaultResolver.Instance);
            Assert.NotNull(res1);
            var idx = 1;
            res1(ref idx);
            Assert.Equal(11, idx);
        }

        private static int StaticFunc(int a)
        {
            return a + 1;
        }

        [Fact]
        public void CanSerializeStaticDelegate()
        {
            Func<int, int> fun = StaticFunc;

            var bytes = MessagePackSerializer.Serialize(fun);
            var res = MessagePackSerializer.Deserialize<Func<int, int>>(bytes);
            Assert.NotNull(res);
            var actual = res(4);

            Assert.Equal(5, actual);
        }

        [Fact]
        public void CanSerializeObjectWithClosure()
        {
            var hasClosure = new HasClosure();
            hasClosure.Create();

            var bytes = MessagePackSerializer.Serialize(hasClosure, ContractlessStandardResolverAllowPrivate.Instance);
            var res = MessagePackSerializer.Deserialize<HasClosure>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
            Assert.NotNull(res);
            var actual = res.Del();
            Assert.Equal(4, actual);

            //var json = bytes.ToHex();
            //var json = "83A1410AA157A474657374A344656CC90000018B64D9C54D6573736167655061636B2E466F726D6174746572732E496E7465726E616C2E44656C65676174655368696D60315B5B43757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E54657374732E44656C656761746554657374732B486173436C6F737572652B3C3E635F5F446973706C6179436C617373395F302C2043757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E457874656E73696F6E7354657374735D5D2C204D6573736167655061636B93CE1778943AC41D53797374656D2E46756E6360315B5B53797374656D2E496E7433325D5D81A16103AC3C4372656174653E625F5F30D284E4A3CCC48643757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E54657374732E44656C656761746554657374732B486173436C6F737572652B3C3E635F5F446973706C6179436C617373395F302C2043757465416E742E457874656E73696F6E732E53657269616C697A6174696F6E2E457874656E73696F6E73546573747390";
            //bytes = ToHex(json);
            res = MessagePackSerializer.Deserialize<HasClosure>(bytes, ContractlessStandardResolverAllowPrivate.Instance);
            Assert.NotNull(res);
            actual = res.Del();
            Assert.Equal(4, actual);
        }

        private static Byte[] ToHex(String data)
        {
            if (String.IsNullOrEmpty(data)) return null;

            // 过滤特殊字符
            data = data.Trim()
                    .Replace("-", null)
                    .Replace("0x", null)
                    .Replace("0X", null)
                    .Replace(" ", null)
                    .Replace("\r", null)
                    .Replace("\n", null)
                    .Replace(",", null);

            var length = data.Length;

            var bts = new Byte[length / 2];
            for (int i = 0; i < bts.Length; i++)
            {
                bts[i] = Byte.Parse(data.Substring(2 * i, 2), NumberStyles.HexNumber);
            }
            return bts;
        }
    }
}