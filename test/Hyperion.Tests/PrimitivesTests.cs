#region copyright

// -----------------------------------------------------------------------
//  <copyright file="PrimitivesTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------

#endregion

using System;
using System.Net;
using Xunit;

namespace Hyperion.Tests
{
  public class PrimitivesTest : TestBase
  {
    [Fact]
    public void CanSerializeTuple1()
    {
      SerializeAndAssert(Tuple.Create("abc"));
    }

    [Fact]
    public void CanSerializeTuple2()
    {
      SerializeAndAssert(Tuple.Create(1, 123));
    }

    [Fact]
    public void CanSerializeTuple3()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3));
    }

    [Fact]
    public void CanSerializeTuple4()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3, 4));
    }

    [Fact]
    public void CanSerializeTuple5()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5));
    }

    [Fact]
    public void CanSerializeTuple6()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6));
    }

    [Fact]
    public void CanSerializeTuple7()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7));
    }

    [Fact]
    public void CanSerializeTuple8()
    {
      SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8));
    }

    [Fact]
    public void CanSerializeBool()
    {
      SerializeAndAssert(true);
    }

    [Fact]
    public void CanSerializeGuid()
    {
      SerializeAndAssert(Guid.NewGuid());
    }

    [Fact]
    public void CanSerializeDateTime()
    {
      SerializeAndAssert(DateTime.UtcNow);
    }

    [Fact]
    public void CanSerializeDecimal()
    {
      SerializeAndAssert(123m);
    }

    [Fact]
    public void CanSerializeDouble()
    {
      SerializeAndAssert(123d);
    }

    [Fact]
    public void CanSerializeByte()
    {
      SerializeAndAssert((byte)123);
    }

    [Fact]
    public void CanSerializeSByte()
    {
      SerializeAndAssert((sbyte)123);
    }

    [Fact]
    public void CanSerializeInt16()
    {
      SerializeAndAssert((short)123);
    }

    [Fact]
    public void CanSerializeInt64()
    {
      SerializeAndAssert(123L);
    }

    [Fact]
    public void CanSerializeInt32()
    {
      SerializeAndAssert(123);
    }

    [Fact]
    public void CanSerializeUInt16()
    {
      SerializeAndAssert((ushort)123);
    }

    [Fact]
    public void CanSerializeUInt64()
    {
      SerializeAndAssert((ulong)123);
    }

    [Fact]
    public void CanSerializeUInt32()
    {
      SerializeAndAssert((uint)123);
    }

    [Fact]
    public void CanSerializeLongString()
    {
      var s = new string('x', 1000);
      SerializeAndAssert(s);
    }

    [Fact]
    public void CanSerializeString()
    {
      SerializeAndAssert("hello");
    }

    [Fact]
    public void CanSerializeDateTimeOffset()
    {
      // Surrogate：字节 124 耗时：5ms
      // 原生：字节 272 耗时：9ms
      SerializeAndAssert(DateTimeOffset.Now);
    }

    [Fact]
    public void CanSerializeTimeSpan()
    {
      // Surrogate：字节 110 耗时：5ms
      // 原生：字节 45 耗时：3ms
      SerializeAndAssert(new TimeSpan(8, 8, 8));
    }

    [Fact]
    public void CanSerializeUri()
    {
      // Surrogate：字节 143 耗时：125ms
      // 原生：字节 256 耗时：123ms
      SerializeAndAssert(new Uri(@"http://www.cuteant.net"));
    }

    [Fact]
    public void CanSerializeIpAddress()
    {
      // Surrogate：字节 112 耗时：121ms
      // 原生：字节 138 耗时：135ms
      SerializeAndAssert(IPAddress.Parse("192.168.1.108"));
    }

    [Fact]
    public void CanSerializeIPEndPoint()
    {
      // Surrogate：字节 117 耗时：4ms
      // 原生：字节 183 耗时：5ms
      SerializeAndAssert(new IPEndPoint(IPAddress.Parse("192.168.1.108"), 10080));
    }

    [Fact]
    public void CanSerializeVersion()
    {
      // 原生：字节 52 耗时：115ms
      SerializeAndAssert(new Version(2, 6, 168));
    }

    [Fact]
    public void CanSerializeType()
    {
      SerializeAndAssert(typeof(int));
    }

    [Fact]
    public void CanSerializeType1()
    {
      SerializeAndAssert(typeof(PrimitivesTest));
    }

    [Fact]
    public void CanSerializeType2()
    {
      var obj = new GenericClassTest<int, Guid, string, long>();
      SerializeAndAssert(obj.GetType());
    }

    private void SerializeAndAssert(object expected)
    {
      Serialize(expected);
      Reset();
      var res = Deserialize<object>();
      Assert.Equal(expected, res);
      AssertMemoryStreamConsumed();
    }

    private class GenericClassTest<T1, T2, T3, T4>
    {
      public T1 A;
      public T2 B;
      public T3 C;
      public T4 D;
    }

  }
}