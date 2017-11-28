using System;

namespace Hyperion.Surrogates
{
  public sealed class TimeSpanSurrogate
  {
    public long Ts;

    public static TimeSpanSurrogate ToSurrogate(TimeSpan ts)
        => new TimeSpanSurrogate() { Ts = ts.Ticks };

    public static TimeSpan FromSurrogate(TimeSpanSurrogate surrogate)
        => new TimeSpan(surrogate.Ts);
  }
}
