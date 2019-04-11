namespace MessagePack
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static partial class LZ4MessagePackSerializer
    {
        public static class NonGeneric
        {
            delegate object DeserializeSpan(ReadOnlySpan<byte> bytes);
            delegate object DeserializeSpanWithResolver(ReadOnlySpan<byte> bytes, IFormatterResolver resolver);
            delegate object DeserializeSequence(ReadOnlySequence<byte> sequence);
            delegate object DeserializeSequenceWithResolver(ReadOnlySequence<byte> sequence, IFormatterResolver resolver);

            delegate void RawFormatterSerialize(ref MessagePackWriter writer, ref int idx, object value, IFormatterResolver formatterResolver);
            delegate object RawFormatterDeserialize(ref MessagePackReader reader, IFormatterResolver formatterResolver);

            static readonly Func<Type, CompiledMethods> CreateCompiledMethods;
            static readonly MessagePack.Internal.ThreadsafeTypeKeyHashTable<CompiledMethods> serializes = new MessagePack.Internal.ThreadsafeTypeKeyHashTable<CompiledMethods>(capacity: 64);

            static NonGeneric()
            {
                CreateCompiledMethods = t => new CompiledMethods(t);
            }

            public static byte[] Serialize(Type type, object obj)
            {
                return GetOrAdd(type).serialize1.Invoke(obj);
            }

            public static byte[] Serialize(Type type, object obj, IFormatterResolver resolver)
            {
                return GetOrAdd(type).serialize2.Invoke(obj, resolver);
            }

            public static IOwnedBuffer<byte> SerializeSafe(Type type, object obj)
            {
                return GetOrAdd(type).serialize3.Invoke(obj);
            }

            public static IOwnedBuffer<byte> SerializeSafe(Type type, object obj, IFormatterResolver resolver)
            {
                return GetOrAdd(type).serialize4.Invoke(obj, resolver);
            }

            public static IOwnedBuffer<byte> SerializeUnsafe(Type type, object obj)
            {
                return GetOrAdd(type).serialize5.Invoke(obj);
            }

            public static IOwnedBuffer<byte> SerializeUnsafe(Type type, object obj, IFormatterResolver resolver)
            {
                return GetOrAdd(type).serialize6.Invoke(obj, resolver);
            }

            public static void Serialize(Type type, IArrayBufferWriter<byte> output, object obj)
            {
                GetOrAdd(type).serialize7.Invoke(output, obj);
            }

            public static void Serialize(Type type, IArrayBufferWriter<byte> output, object obj, IFormatterResolver resolver)
            {
                GetOrAdd(type).serialize8.Invoke(output, obj, resolver);
            }

            public static void Serialize(Type type, Stream stream, object obj)
            {
                GetOrAdd(type).serialize9.Invoke(stream, obj);
            }

            public static void Serialize(Type type, Stream stream, object obj, IFormatterResolver resolver)
            {
                GetOrAdd(type).serialize10.Invoke(stream, obj, resolver);
            }

            public static void SerializeToBlock(Type type, ref MessagePackWriter writer, ref int idx, object obj, IFormatterResolver resolver)
            {
                GetOrAdd(type).serialize11.Invoke(ref writer, ref idx, obj, resolver);
            }

            public static object Deserialize(Type type, ReadOnlySpan<byte> bytes)
            {
                return GetOrAdd(type).deserialize1.Invoke(bytes);
            }

            public static object Deserialize(Type type, ReadOnlySpan<byte> bytes, IFormatterResolver resolver)
            {
                return GetOrAdd(type).deserialize2.Invoke(bytes, resolver);
            }

            public static object Deserialize(Type type, ReadOnlySequence<byte> sequence)
            {
                return GetOrAdd(type).deserialize3.Invoke(sequence);
            }

            public static object Deserialize(Type type, ReadOnlySequence<byte> sequence, IFormatterResolver resolver)
            {
                return GetOrAdd(type).deserialize4.Invoke(sequence, resolver);
            }

            public static object Deserialize(Type type, Stream stream)
            {
                return GetOrAdd(type).deserialize5.Invoke(stream);
            }

            public static object Deserialize(Type type, Stream stream, IFormatterResolver resolver)
            {
                return GetOrAdd(type).deserialize6.Invoke(stream, resolver);
            }

            public static object Deserialize(Type type, Stream stream, bool readStrict)
            {
                return GetOrAdd(type).deserialize7.Invoke(stream, readStrict);
            }

            public static object Deserialize(Type type, Stream stream, IFormatterResolver resolver, bool readStrict)
            {
                return GetOrAdd(type).deserialize8.Invoke(stream, resolver, readStrict);
            }

            public static object Deserialize(Type type, ref MessagePackReader reader, IFormatterResolver resolver)
            {
                return GetOrAdd(type).deserialize9.Invoke(ref reader, resolver);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static CompiledMethods GetOrAdd(Type type)
            {
                return serializes.GetOrAdd(type, CreateCompiledMethods);
            }

            sealed class CompiledMethods
            {
                public readonly Func<object, byte[]> serialize1;
                public readonly Func<object, IFormatterResolver, byte[]> serialize2;
                public readonly Func<object, IOwnedBuffer<byte>> serialize3;
                public readonly Func<object, IFormatterResolver, IOwnedBuffer<byte>> serialize4;
                public readonly Func<object, IOwnedBuffer<byte>> serialize5;
                public readonly Func<object, IFormatterResolver, IOwnedBuffer<byte>> serialize6;
                public readonly Action<IArrayBufferWriter<byte>, object> serialize7;
                public readonly Action<IArrayBufferWriter<byte>, object, IFormatterResolver> serialize8;
                public readonly Action<Stream, object> serialize9;
                public readonly Action<Stream, object, IFormatterResolver> serialize10;
                public readonly RawFormatterSerialize serialize11;

                public readonly DeserializeSpan deserialize1;
                public readonly DeserializeSpanWithResolver deserialize2;
                public readonly DeserializeSequence deserialize3;
                public readonly DeserializeSequenceWithResolver deserialize4;
                public readonly Func<Stream, object> deserialize5;
                public readonly Func<Stream, IFormatterResolver, object> deserialize6;
                public readonly Func<Stream, bool, object> deserialize7;
                public readonly Func<Stream, IFormatterResolver, bool, object> deserialize8;
                public readonly RawFormatterDeserialize deserialize9;

                public CompiledMethods(Type type)
                {
                    {
                        // public static byte[] Serialize<T>(T obj)
                        var serialize = GetMethod(type, new Type[] { null });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type));
                        var lambda = Expression.Lambda<Func<object, byte[]>>(body, param1).Compile();

                        this.serialize1 = lambda;
                    }
                    {
                        // public static byte[] Serialize<T>(T obj, IFormatterResolver resolver)
                        var serialize = GetMethod(type, new Type[] { null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type), param2);
                        var lambda = Expression.Lambda<Func<object, IFormatterResolver, byte[]>>(body, param1, param2).Compile();

                        this.serialize2 = lambda;
                    }
                    {
                        // public static IOwnedBuffer<byte> SerializeSafe<T>(T obj)
                        var serialize = GetSafeMethod(type, new Type[] { null });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type));
                        var lambda = Expression.Lambda<Func<object, IOwnedBuffer<byte>>>(body, param1).Compile();

                        this.serialize3 = lambda;
                    }
                    {
                        // public static IOwnedBuffer<byte> SerializeSafe<T>(T obj, IFormatterResolver resolver)
                        var serialize = GetSafeMethod(type, new Type[] { null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type), param2);
                        var lambda = Expression.Lambda<Func<object, IFormatterResolver, IOwnedBuffer<byte>>>(body, param1, param2).Compile();

                        this.serialize4 = lambda;
                    }
                    {
                        // public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj)
                        var serialize = GetUnsafeMethod(type, new Type[] { null });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type));
                        var lambda = Expression.Lambda<Func<object, IOwnedBuffer<byte>>>(body, param1).Compile();

                        this.serialize5 = lambda;
                    }
                    {
                        // public static IOwnedBuffer<byte> SerializeUnsafe<T>(T obj, IFormatterResolver resolver)
                        var serialize = GetUnsafeMethod(type, new Type[] { null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(object), "obj");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, type.IsValueType
                            ? Expression.Unbox(param1, type)
                            : Expression.Convert(param1, type), param2);
                        var lambda = Expression.Lambda<Func<object, IFormatterResolver, IOwnedBuffer<byte>>>(body, param1, param2).Compile();

                        this.serialize6 = lambda;
                    }
                    {
                        // public static void Serialize<T>(IArrayBufferWriter<byte> output, T obj)
                        var serialize = GetMethod(type, new Type[] { typeof(IArrayBufferWriter<byte>), null });

                        var param1 = Expression.Parameter(typeof(IArrayBufferWriter<byte>), "output");
                        var param2 = Expression.Parameter(typeof(object), "obj");

                        var body = Expression.Call(serialize, param1, type.IsValueType
                            ? Expression.Unbox(param2, type)
                            : Expression.Convert(param2, type));
                        var lambda = Expression.Lambda<Action<IArrayBufferWriter<byte>, object>>(body, param1, param2).Compile();

                        this.serialize7 = lambda;
                    }
                    {
                        // public static void Serialize<T>(IArrayBufferWriter<byte> output, T obj, IFormatterResolver resolver)
                        var serialize = GetMethod(type, new Type[] { typeof(IArrayBufferWriter<byte>), null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(IArrayBufferWriter<byte>), "output");
                        var param2 = Expression.Parameter(typeof(object), "obj");
                        var param3 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, param1, type.IsValueType
                            ? Expression.Unbox(param2, type)
                            : Expression.Convert(param2, type), param3);
                        var lambda = Expression.Lambda<Action<IArrayBufferWriter<byte>, object, IFormatterResolver>>(body, param1, param2, param3).Compile();

                        this.serialize8 = lambda;
                    }
                    {
                        // public static void Serialize<T>(Stream stream, T obj)
                        var serialize = GetMethod(type, new Type[] { typeof(Stream), null });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var param2 = Expression.Parameter(typeof(object), "obj");

                        var body = Expression.Call(serialize, param1, type.IsValueType
                            ? Expression.Unbox(param2, type)
                            : Expression.Convert(param2, type));
                        var lambda = Expression.Lambda<Action<Stream, object>>(body, param1, param2).Compile();

                        this.serialize9 = lambda;
                    }
                    {
                        // public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
                        var serialize = GetMethod(type, new Type[] { typeof(Stream), null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var param2 = Expression.Parameter(typeof(object), "obj");
                        var param3 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, param1, type.IsValueType
                            ? Expression.Unbox(param2, type)
                            : Expression.Convert(param2, type), param3);
                        var lambda = Expression.Lambda<Action<Stream, object, IFormatterResolver>>(body, param1, param2, param3).Compile();

                        this.serialize10 = lambda;
                    }
                    {
                        //  public static void Serialize<T>(ref MessagePackWriter writer, ref int idx, T obj, IFormatterResolver resolver)
                        var serialize = GetMethod(type, new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(int).MakeByRefType(), null, typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(MessagePackWriter).MakeByRefType(), "writer");
                        var param2 = Expression.Parameter(typeof(int).MakeByRefType(), "idx");
                        var param3 = Expression.Parameter(typeof(object), "obj");
                        var param4 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var body = Expression.Call(serialize, param1, param2, type.IsValueType
                            ? Expression.Unbox(param3, type)
                            : Expression.Convert(param3, type), param4);
                        var lambda = Expression.Lambda<RawFormatterSerialize>(body, param1, param2, param3, param4).Compile();

                        this.serialize11 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(ReadOnlySpan<byte> bytes)
                        var deserialize = GetMethod(type, new Type[] { typeof(ReadOnlySpan<byte>) });

                        var param1 = Expression.Parameter(typeof(ReadOnlySpan<byte>), "bytes");
                        var body = Expression.Convert(Expression.Call(deserialize, param1), typeof(object));
                        var lambda = Expression.Lambda<DeserializeSpan>(body, param1).Compile();

                        this.deserialize1 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(ReadOnlySpan<byte> bytes, IFormatterResolver resolver)
                        var deserialize = GetMethod(type, new Type[] { typeof(ReadOnlySpan<byte>), typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(ReadOnlySpan<byte>), "bytes");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "resolver");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2), typeof(object));
                        var lambda = Expression.Lambda<DeserializeSpanWithResolver>(body, param1, param2).Compile();

                        this.deserialize2 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(ReadOnlySequence<byte> sequence)
                        var deserialize = GetMethod(type, new Type[] { typeof(ReadOnlySequence<byte>) });

                        var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");
                        var body = Expression.Convert(Expression.Call(deserialize, param1), typeof(object));
                        var lambda = Expression.Lambda<DeserializeSequence>(body, param1).Compile();

                        this.deserialize3 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(ReadOnlySequence<byte> sequence, IFormatterResolver resolver)
                        var deserialize = GetMethod(type, new Type[] { typeof(ReadOnlySequence<byte>), typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "resolver");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2), typeof(object));
                        var lambda = Expression.Lambda<DeserializeSequenceWithResolver>(body, param1, param2).Compile();

                        this.deserialize4 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(Stream stream)
                        var deserialize = GetMethod(type, new Type[] { typeof(Stream) });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var body = Expression.Convert(Expression.Call(deserialize, param1), typeof(object));
                        var lambda = Expression.Lambda<Func<Stream, object>>(body, param1).Compile();

                        this.deserialize5 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(Stream stream, IFormatterResolver resolver)
                        var deserialize = GetMethod(type, new Type[] { typeof(Stream), typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "resolver");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2), typeof(object));
                        var lambda = Expression.Lambda<Func<Stream, IFormatterResolver, object>>(body, param1, param2).Compile();

                        this.deserialize6 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(Stream stream, bool readStrict)
                        var deserialize = GetMethod(type, new Type[] { typeof(Stream), typeof(bool) });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var param2 = Expression.Parameter(typeof(bool), "readStrict");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2), typeof(object));
                        var lambda = Expression.Lambda<Func<Stream, bool, object>>(body, param1, param2).Compile();

                        this.deserialize7 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(Stream stream, IFormatterResolver resolver, bool readStrict)
                        var deserialize = GetMethod(type, new Type[] { typeof(Stream), typeof(IFormatterResolver), typeof(bool) });

                        var param1 = Expression.Parameter(typeof(Stream), "stream");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "resolver");
                        var param3 = Expression.Parameter(typeof(bool), "readStrict");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2, param3), typeof(object));
                        var lambda = Expression.Lambda<Func<Stream, IFormatterResolver, bool, object>>(body, param1, param2, param3).Compile();

                        this.deserialize8 = lambda;
                    }
                    {
                        // public static T Deserialize<T>(ref MessagePackReader reader, IFormatterResolver resolver)
                        var deserialize = GetMethod(type, new Type[] { typeof(MessagePackReader).MakeByRefType(), typeof(IFormatterResolver) });

                        var param1 = Expression.Parameter(typeof(MessagePackReader).MakeByRefType(), "reader");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "resolver");
                        var body = Expression.Convert(Expression.Call(deserialize, param1, param2), typeof(object));
                        var lambda = Expression.Lambda<RawFormatterDeserialize>(body, param1, param2).Compile();

                        this.deserialize9 = lambda;
                    }
                }

                // null is generic type marker.
                static MethodInfo GetMethod(Type type, Type[] parameters)
                {
                    return typeof(LZ4MessagePackSerializer).GetRuntimeMethods().Where(x =>
                    {
                        if (!(x.Name == "Serialize" || x.Name == "Deserialize")) return false;
                        var ps = x.GetParameters();
                        if (ps.Length != parameters.Length) return false;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            if (parameters[i] == null && ps[i].ParameterType.IsGenericParameter) continue;
                            if (ps[i].ParameterType != parameters[i]) return false;
                        }
                        return true;
                    })
                    .Single()
                    .MakeGenericMethod(type);
                }

                static MethodInfo GetSafeMethod(Type type, Type[] parameters)
                {
                    return typeof(LZ4MessagePackSerializer).GetRuntimeMethods().Where(x =>
                    {
                        if (!(x.Name == "SerializeSafe")) return false;
                        var ps = x.GetParameters();
                        if (ps.Length != parameters.Length) return false;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            if (parameters[i] == null && ps[i].ParameterType.IsGenericParameter) continue;
                            if (ps[i].ParameterType != parameters[i]) return false;
                        }
                        return true;
                    })
                    .Single()
                    .MakeGenericMethod(type);
                }

                static MethodInfo GetUnsafeMethod(Type type, Type[] parameters)
                {
                    return typeof(LZ4MessagePackSerializer).GetRuntimeMethods().Where(x =>
                    {
                        if (!(x.Name == "SerializeUnsafe")) return false;
                        var ps = x.GetParameters();
                        if (ps.Length != parameters.Length) return false;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            if (parameters[i] == null && ps[i].ParameterType.IsGenericParameter) continue;
                            if (ps[i].ParameterType != parameters[i]) return false;
                        }
                        return true;
                    })
                    .Single()
                    .MakeGenericMethod(type);
                }
            }
        }
    }
}
