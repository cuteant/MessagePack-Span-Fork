﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public abstract class AbstractBase
    {
        [DataMember(Order = 0)]
        public UInt32 Id = 0xaa00aa00;
    }

    public sealed class RealClass : AbstractBase
    {
        public String Str;
    }

    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public class OrderOrder
    {
        [DataMember(Order = 5)]
        public int Foo { get; set; }

        [DataMember(Order = 2)]
        public int Moge { get; set; }

        [DataMember(Order = 10)]
        public int FooBar;

        public string NoBar;

        [DataMember(Order = 0)]
        public string Bar;
    }

    public class DynamicObjectResolverOrderTest
    {
        IList<string> IteratePropertyNames(byte[] bin)
        {
            var reader = new MessagePackReader(bin);
            var mapCount = reader.ReadMapHeader();
            var list = new List<string>();
            for (int i = 0; i < mapCount; i++)
            {
                list.Add(reader.ReadString());
                reader.ReadNext();
            }
            return list;
        }

        [Fact]
        public void OrderTest()
        {
            var msgRawData = MessagePack.MessagePackSerializer.Serialize(new OrderOrder());
            IteratePropertyNames(msgRawData).Is("Bar", "Moge", "Foo", "FooBar", "NoBar");
        }

        [Fact]
        public void InheritIterateOrder()
        {
            RealClass realClass = new RealClass { Str = "X" };
            var msgRawData = MessagePack.MessagePackSerializer.Serialize(realClass);

            IteratePropertyNames(msgRawData).Is("Id", "Str");
        }
    }
}
