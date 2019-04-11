﻿using MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{

    public class SpecifiedFormatterResolverTest
    {
        [MessagePackFormatter(typeof(NoObjectFormatter))]
        class CustomClassObject
        {
            int X;

            public CustomClassObject(int x)
            {
                this.X = x;
            }

            public int GetX()
            {
                return X;
            }

            class NoObjectFormatter : IMessagePackFormatter<CustomClassObject>
            {
                public CustomClassObject Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
                {
                    var r = reader.ReadInt32();
                    return new CustomClassObject(r);
                }

                public void Serialize(ref MessagePackWriter writer, ref int offset, CustomClassObject value, IFormatterResolver formatterResolver)
                {
                    writer.WriteInt32(value.X, ref offset);
                }
            }
        }

        [MessagePackFormatter(typeof(CustomStructObjectFormatter))]
        struct CustomStructObject
        {
            int X;

            public CustomStructObject(int x)
            {
                this.X = x;
            }

            public int GetX()
            {
                return X;
            }

            class CustomStructObjectFormatter : IMessagePackFormatter<CustomStructObject>
            {
                public CustomStructObject Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
                {
                    var r = reader.ReadInt32();
                    return new CustomStructObject(r);
                }

                public void Serialize(ref MessagePackWriter writer, ref int offset, CustomStructObject value, IFormatterResolver formatterResolver)
                {
                    writer.WriteInt32(value.X, ref offset);
                }
            }
        }

        [MessagePackFormatter(typeof(CustomEnumObjectFormatter))]
        enum CustomyEnumObject
        {
            A = 0, B = 1, C = 2
        }

        class CustomEnumObjectFormatter : IMessagePackFormatter<CustomyEnumObject>
        {
            public CustomyEnumObject Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
            {
                var r = reader.ReadInt32();
                if (r == 0)
                {
                    return CustomyEnumObject.A;
                }
                else if (r == 2)
                {
                    return CustomyEnumObject.C;
                }
                return CustomyEnumObject.B;
            }

            public void Serialize(ref MessagePackWriter writer, ref int offset, CustomyEnumObject value, IFormatterResolver formatterResolver)
            {
                writer.WriteInt32((int)value, ref offset);
            }
        }

        [MessagePackFormatter(typeof(CustomInterfaceObjectFormatter))]
        interface ICustomInterfaceObject
        {
            int A { get; }
        }

        class CustomInterfaceObjectFormatter : IMessagePackFormatter<ICustomInterfaceObject>
        {
            public ICustomInterfaceObject Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
            {
                var r = reader.ReadInt32();
                return new InheritDefault(r);
            }

            public void Serialize(ref MessagePackWriter writer, ref int offset, ICustomInterfaceObject value, IFormatterResolver formatterResolver)
            {
                writer.WriteInt32(value.A, ref offset);
            }
        }

        class InheritDefault : ICustomInterfaceObject
        {
            public int A { get; }

            public InheritDefault(int a)
            {
                this.A = a;
            }
        }

        class HogeMoge : ICustomInterfaceObject
        {
            public int A { get; set; }
        }


        [MessagePackFormatter(typeof(NoObjectFormatter), CustomyEnumObject.C)]
        class CustomClassObjectWithArgument
        {
            int X;

            public CustomClassObjectWithArgument(int x)
            {
                this.X = x;
            }

            public int GetX()
            {
                return X;
            }

            class NoObjectFormatter : IMessagePackFormatter<CustomClassObjectWithArgument>
            {
                CustomyEnumObject x;

                public NoObjectFormatter(CustomyEnumObject x)
                {
                    this.x = x;
                }

                public CustomClassObjectWithArgument Deserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver)
                {
                    var r = reader.ReadInt32();
                    return new CustomClassObjectWithArgument(r);
                }

                public void Serialize(ref MessagePackWriter writer, ref int offset, CustomClassObjectWithArgument value, IFormatterResolver formatterResolver)
                {
                    writer.WriteInt32(value.X * (int)x, ref offset);
                }
            }
        }

        T Convert<T>(T value)
        {
            return MessagePackSerializer.Deserialize<T>(MessagePackSerializer.Serialize(value, MessagePack.Resolvers.StandardResolver.Instance), MessagePack.Resolvers.StandardResolver.Instance);
        }

        [Fact]
        public void CustomFormatters()
        {
            Convert(new CustomClassObject(999)).GetX().Is(999);
            Convert(new CustomStructObject(1234)).GetX().Is(1234);
            Convert(CustomyEnumObject.C).Is(CustomyEnumObject.C);
            Convert((CustomyEnumObject)(1234)).Is(CustomyEnumObject.B);
            Convert<ICustomInterfaceObject>(new HogeMoge { A = 999 }).A.Is(999);
        }

        [Fact]
        public void WithArg()
        {
            Convert(new CustomClassObjectWithArgument(999)).GetX().Is(999 * 2);
        }
    }
}
