using System;
using System.Linq;
using System.Runtime.InteropServices;
using MessagePack.Internal;
using Xunit;

namespace MessagePack.Tests
{
    public class UnsafeMemoryTest
    {
        delegate void WriteDelegate(ref MessagePackWriter writer, byte[] ys, ref int idx);
        delegate void WriteDelegate1(ref byte writerSpace, ref byte src, ref int idx);

        [Theory]
        [InlineData('a', 1)]
        [InlineData('b', 10)]
        [InlineData('c', 100)]
        [InlineData('d', 1000)]
        [InlineData('e', 10000)]
        [InlineData('f', 100000)]
        public void GetEncodedStringBytes(char c, int count)
        {
            var s = new string(c, count);
            var bin1 = MessagePackBinary.GetEncodedStringBytes(s);
            var bin2 = MessagePackSerializer.Serialize(s);

            var idx = 0;
            var writer = new MessagePackWriter(16);
            writer.WriteRawBytes(bin1, ref idx);

            MessagePack.Internal.ByteArrayComparer.Equals(bin1, 0, bin1.Length, bin2).IsTrue();
            var buf = writer.ToArray(idx);
            MessagePack.Internal.ByteArrayComparer.Equals(bin1, 0, bin1.Length, buf, 0, buf.Length).IsTrue();
        }

        [Fact]
        public void WriteRaw()
        {
            const int MaxFixStringLength = MessagePackRange.MaxFixStringLength;
            // x86
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();

                var idx = 0;
                var writer = new MessagePackWriter(16);

                ((typeof(UnsafeMemory32)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(byte[]), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate)) as WriteDelegate)
                    .Invoke(ref writer, src, ref idx);

                idx.Is(i);

                var buf = writer.ToArray(idx);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, buf, 0, buf.Length).IsTrue();
            }
            // x64
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
                var idx = 0;
                var writer = new MessagePackWriter(16);

                ((typeof(UnsafeMemory64)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(byte[]), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate)) as WriteDelegate)
                    .Invoke(ref writer, src, ref idx);

                idx.Is(i);

                var buf = writer.ToArray(idx);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, buf, 0, buf.Length).IsTrue();
            }
            // x86, offset
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
                var idx = 3;
                var writer = new MessagePackWriter(16);

                ((typeof(UnsafeMemory32)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(byte[]), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate)) as WriteDelegate)
                    .Invoke(ref writer, src, ref idx);

                idx.Is(i + 3);

                var buf = writer.ToArray(idx);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, buf, 3, buf.Length - 3).IsTrue();
            }
            // x64, offset
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
                var idx = 3;
                var writer = new MessagePackWriter(16);

                ((typeof(UnsafeMemory64)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(MessagePackWriter).MakeByRefType(), typeof(byte[]), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate)) as WriteDelegate)
                    .Invoke(ref writer, src, ref idx);

                idx.Is(i + 3);

                var buf = writer.ToArray(idx);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, buf, 3, buf.Length - 3).IsTrue();
            }
        }

        [Fact]
        public void WriteRaw1()
        {
            const int MaxFixStringLength = MessagePackRange.MaxFixStringLength;
            // x86
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = new ReadOnlySpan<byte>(Enumerable.Range(0, i).Select(x => (byte)x).ToArray());
                var idx = 0;
                var writer = new MessagePackWriter(16);

                writer.Ensure(idx, i);
                ((typeof(UnsafeMemory32)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType(), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate1)) as WriteDelegate1)
                    .Invoke(ref writer.PinnableAddress, ref MemoryMarshal.GetReference(src), ref idx);

                idx.Is(i);

                src.SequenceEqual(writer.ToArray(idx)).IsTrue();
            }
            // x64
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = new ReadOnlySpan<byte>(Enumerable.Range(0, i).Select(x => (byte)x).ToArray());
                var idx = 0;
                var writer = new MessagePackWriter(16);

                writer.Ensure(idx, i);
                ((typeof(UnsafeMemory64)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType(), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate1)) as WriteDelegate1)
                    .Invoke(ref writer.PinnableAddress, ref MemoryMarshal.GetReference(src), ref idx);

                idx.Is(i);

                src.SequenceEqual(writer.ToArray(idx)).IsTrue();
            }
            // x86, offset
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = new ReadOnlySpan<byte>(Enumerable.Range(0, i).Select(x => (byte)x).ToArray());
                var idx = 3;
                var writer = new MessagePackWriter(16);

                writer.Ensure(idx, i);
                ((typeof(UnsafeMemory32)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType(), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate1)) as WriteDelegate1)
                    .Invoke(ref writer.PinnableAddress, ref MemoryMarshal.GetReference(src), ref idx);

                idx.Is(i + 3);

                src.SequenceEqual(writer.ToArray(idx).AsSpan().Slice(3)).IsTrue();
            }
            // x64, offset
            for (int i = 1; i <= MaxFixStringLength; i++)
            {
                var src = new ReadOnlySpan<byte>(Enumerable.Range(0, i).Select(x => (byte)x).ToArray());
                var idx = 3;
                var writer = new MessagePackWriter(16);

                writer.Ensure(idx, i);
                ((typeof(UnsafeMemory64)
                    .GetMethod("WriteRaw" + i, new Type[] { typeof(byte).MakeByRefType(), typeof(byte).MakeByRefType(), typeof(int).MakeByRefType() }))
                    .CreateDelegate(typeof(WriteDelegate1)) as WriteDelegate1)
                    .Invoke(ref writer.PinnableAddress, ref MemoryMarshal.GetReference(src), ref idx);

                idx.Is(i + 3);

                src.SequenceEqual(writer.ToArray(idx).AsSpan().Slice(3)).IsTrue();
            }
        }
    }
}
