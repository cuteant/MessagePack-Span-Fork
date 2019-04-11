﻿#if (NETSTANDARD2_0 || NET471 || NET451)

using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePack.Internal
{
    // Helpers to write Memory<byte> to Stream on netstandard 2.0
    internal static class StreamExtensions
    {
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            var pool = MessagePackBinary.Shared;
            byte[] sharedBuffer = pool.Rent(buffer.Length);
            try
            {
                var result = stream.Read(sharedBuffer, 0, buffer.Length);
                new Span<byte>(sharedBuffer, 0, result).CopyTo(buffer);
                return result;
            }
            finally
            {
                pool.Return(sharedBuffer);
            }
        }

        public static ValueTask<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return new ValueTask<int>(stream.ReadAsync(array.Array, array.Offset, array.Count, cancellationToken));
            }
            else
            {
                byte[] sharedBuffer = MessagePackBinary.Shared.Rent(buffer.Length);
                return FinishReadAsync(stream.ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer, buffer);

                async ValueTask<int> FinishReadAsync(Task<int> readTask, byte[] localBuffer, Memory<byte> localDestination)
                {
                    try
                    {
                        int result = await readTask.ConfigureAwait(false);
                        new Span<byte>(localBuffer, 0, result).CopyTo(localDestination.Span);
                        return result;
                    }
                    finally
                    {
                        MessagePackBinary.Shared.Return(localBuffer);
                    }
                }
            }
        }

        public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            var pool = MessagePackBinary.Shared;
            byte[] sharedBuffer = pool.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                pool.Return(sharedBuffer);
            }

        }

        public static ValueTask WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return new ValueTask(stream.WriteAsync(array.Array, array.Offset, array.Count, cancellationToken));
            }
            else
            {
                byte[] sharedBuffer = MessagePackBinary.Shared.Rent(buffer.Length);
                buffer.Span.CopyTo(sharedBuffer);
                return new ValueTask(FinishWriteAsync(stream.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer));
            }
        }

        private static async Task FinishWriteAsync(Task writeTask, byte[] localBuffer)
        {
            try
            {
                await writeTask.ConfigureAwait(false);
            }
            finally
            {
                MessagePackBinary.Shared.Return(localBuffer);
            }
        }
    }
}

#endif