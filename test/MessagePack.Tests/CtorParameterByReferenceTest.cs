using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests
{
    public class CtorParameterByReferenceTest
    {
        [MessagePackObject]
        public readonly struct Payload
        {
            [Key(0)]
            public readonly Type MessageType;
            [Key(1)]
            public readonly byte[] Data;

            [SerializationConstructor]
            public Payload(Type type, byte[] data)
            {
                MessageType = type;
                Data = data;
            }
        }

        [MessagePackObject]
        public readonly struct MyStruct
        {
            [Key(0)]
            public readonly Payload Msg;
            [Key(1)]
            public readonly int Version;

            [SerializationConstructor]
            public MyStruct(in Payload msg, int version)
            {
                Msg = msg;
                Version = version;
            }
        }

        public readonly struct Payload1
        {
            public readonly Type MessageType;
            public readonly byte[] Data;

            public Payload1(Type messageType, byte[] data)
            {
                MessageType = messageType;
                Data = data;
            }
        }

        public readonly struct MyStruct1
        {
            public readonly Payload1 Msg;
            public readonly int Version;

            public MyStruct1(in Payload1 msg, int version)
            {
                Msg = msg;
                Version = version;
            }
        }

        [Fact]
        public void SerializeIntKey()
        {
            var msg = new Payload(typeof(int), new byte[] { 1, 2, 3 });
            var ms = new MyStruct(msg, 2);

            var bin = MessagePackSerializer.Serialize(ms);

            var ms2 = MessagePackSerializer.Deserialize<MyStruct>(bin);

            ms.Version.Is(ms2.Version);
            ms.Msg.MessageType.Is(ms2.Msg.MessageType);
            ms.Msg.Data.Is(ms2.Msg.Data);
        }

        [Fact]
        public void SerializeStringKey()
        {
            var msg = new Payload1(typeof(int), new byte[] { 1, 2, 3 });
            var ms = new MyStruct1(msg, 2);

            var bin = MessagePackSerializer.Serialize(ms, ContractlessStandardResolverAllowPrivate.Instance);

            var ms2 = MessagePackSerializer.Deserialize<MyStruct1>(bin, ContractlessStandardResolverAllowPrivate.Instance);

            ms.Version.Is(ms2.Version);
            ms.Msg.MessageType.Is(ms2.Msg.MessageType);
            ms.Msg.Data.Is(ms2.Msg.Data);
        }
    }
}
