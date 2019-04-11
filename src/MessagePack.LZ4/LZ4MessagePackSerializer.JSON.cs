namespace MessagePack
{
    using System;
    using System.IO;

    // JSON API
    public static partial class LZ4MessagePackSerializer
    {
        /// <summary>Dump message-pack binary to JSON string.</summary>
        public static string ToJson(byte[] lz4SerializedData)
        {
            if (lz4SerializedData == null || 0u >= (uint)lz4SerializedData.Length) { return ""; }

            return MessagePackSerializer.ToJson(Decode(lz4SerializedData));
        }

        /// <summary>Dump message-pack binary to JSON string.</summary>
        public static string ToJson(ReadOnlySpan<byte> lz4SerializedData)
        {
            if (lz4SerializedData.IsEmpty) { return ""; }

            return MessagePackSerializer.ToJson(Decode(lz4SerializedData));
        }

        /// <summary>From Json String to LZ4MessagePack binary</summary>
        public static byte[] FromJson(string str)
        {
            using (var sr = new StringReader(str))
            {
                return FromJson(sr);
            }
        }

        /// <summary>From Json String to LZ4MessagePack binary</summary>
        public static byte[] FromJson(TextReader reader)
        {
            using (var buffer = MessagePackSerializer.FromJsonUnsafe(reader))
            {
                return LZ4MessagePackSerializer.ToLZ4Binary(buffer.Span).ToArray();
            }
        }
    }
}
