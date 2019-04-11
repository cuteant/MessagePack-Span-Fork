extern alias oldmsgpack;
extern alias newmsgpack;
extern alias newmsgpacklz4;

using Benchmark.Serializers;

public class MessagePack_Official : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return oldmsgpack::MessagePack.MessagePackSerializer.Deserialize<T>((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return oldmsgpack::MessagePack.MessagePackSerializer.Serialize<T>(input);
    }
}

public class MessagePack_Span : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return newmsgpack::MessagePack.MessagePackSerializer.Deserialize<T>((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return newmsgpack::MessagePack.MessagePackSerializer.Serialize<T>(input);
    }
}

public class MessagePackLz4_Official : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return oldmsgpack::MessagePack.LZ4MessagePackSerializer.Deserialize<T>((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return oldmsgpack::MessagePack.LZ4MessagePackSerializer.Serialize<T>(input);
    }
}

public class MessagePackLz4_Span : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return newmsgpacklz4::MessagePack.LZ4MessagePackSerializer.Deserialize<T>((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return newmsgpacklz4::MessagePack.LZ4MessagePackSerializer.Serialize<T>(input);
    }
}

public class Typeless_Official : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return (T)oldmsgpack::MessagePack.MessagePackSerializer.Typeless.Deserialize((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return oldmsgpack::MessagePack.MessagePackSerializer.Typeless.Serialize(input);
    }
}

public class Typeless_Span : SerializerBase
{
    public override T Deserialize<T>(object input)
    {
        return (T)newmsgpack::MessagePack.MessagePackSerializer.Typeless.Deserialize((byte[])input);
    }

    public override object Serialize<T>(T input)
    {
        return newmsgpack::MessagePack.MessagePackSerializer.Typeless.Serialize(input);
    }
}
