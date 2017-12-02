using System;
using ProtoBuf;

namespace CuteAnt.Extensions.Serialization.Protobuf.Internal
{
  /// <summary>Read and write <see cref="DateTimeOffset"/> objects</summary>
  public static class DateTimeOffsetSerializer
  {
    private const int FieldDateTimeTicks = 0x01, FieldOffsetTicks = 0x02;

    /// <summary>Writes a <see cref="DateTimeOffset"/> to a protobuf stream</summary>
    public static void WriteDateTimeOffset(DateTimeOffset value, ProtoWriter dest)
    {
      SubItemToken token = ProtoWriter.StartSubItem(null, dest);
      // Write the date time as ticks
      ProtoWriter.WriteFieldHeader(FieldDateTimeTicks, WireType.SignedVariant, dest);
      ProtoWriter.WriteInt64(value.Ticks, dest);
      // Then the offset timespan as ticks too
      ProtoWriter.WriteFieldHeader(FieldOffsetTicks, WireType.SignedVariant, dest);
      ProtoWriter.WriteInt64(value.Offset.Ticks, dest);
      ProtoWriter.EndSubItem(token, dest);
    }

    /// <summary>Reads a <see cref="DateTimeOffset"/> from a protobuf stream</summary>
    public static DateTimeOffset ReadDateTimeOffset(ProtoReader source)
    {
      SubItemToken token = ProtoReader.StartSubItem(source);

      long dateTimeTicks = 0, dateTimeOffset = 0;
      int fieldNumber;
      while ((fieldNumber = source.ReadFieldHeader()) > 0)
      {
        switch (fieldNumber)
        {
          case FieldDateTimeTicks:
            source.Assert(WireType.SignedVariant);
            dateTimeTicks = source.ReadInt64();
            break;
          case FieldOffsetTicks:
            source.Assert(WireType.SignedVariant);
            dateTimeOffset = source.ReadInt64();
            break;
          default:
            source.SkipField();
            break;
        }
      }

      ProtoReader.EndSubItem(token, source);
      return new DateTimeOffset(dateTimeTicks, TimeSpan.FromTicks(dateTimeOffset));
    }
  }
}