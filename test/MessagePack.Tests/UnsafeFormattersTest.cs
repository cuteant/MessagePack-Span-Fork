﻿using System;
using MessagePack.Formatters;
using Xunit;

namespace MessagePack.Tests
{
    public class UnsafeFormattersTest
    {
        [Fact]
        public void GuidTest()
        {
            var guid = Guid.NewGuid();

            var idx = 0;
            var writer = new MessagePackWriter(16);
            BinaryGuidFormatter.Instance.Serialize(ref writer, ref idx, guid, null);
            idx.Is(18);
            var buffer = writer.ToArray(idx);

            var reader = new MessagePackReader(buffer);
            BinaryGuidFormatter.Instance.Deserialize(ref reader, null).Is(guid);
            reader.CurrentSpanIndex.Is(18);
        }

        [Fact]
        public void DecimalTest()
        {
            var d = new Decimal(1341, 53156, 61, true, 3);

            var idx = 0;
            var writer = new MessagePackWriter(16);
            BinaryDecimalFormatter.Instance.Serialize(ref writer, ref idx, d, null);
            idx.Is(18);
            var buffer = writer.ToArray(idx);

            var reader = new MessagePackReader(buffer);
            BinaryDecimalFormatter.Instance.Deserialize(ref reader, null).Is(d);
            reader.CurrentSpanIndex.Is(18);
        }
    }
}
