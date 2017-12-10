//using System;

//namespace Hyperion.Surrogates
//{
//  public sealed class DateTimeOffsetSurrogate
//  {
//    public long Dts;

//    public long Ots;

//    public static DateTimeOffsetSurrogate ToSurrogate(DateTimeOffset dts)
//        => new DateTimeOffsetSurrogate() { Dts = dts.DateTime.Ticks, Ots = dts.Offset.Ticks };

//    public static DateTimeOffset FromSurrogate(DateTimeOffsetSurrogate surrogate)
//        => new DateTimeOffset(surrogate.Dts, new TimeSpan(surrogate.Ots));
//  }
//}
