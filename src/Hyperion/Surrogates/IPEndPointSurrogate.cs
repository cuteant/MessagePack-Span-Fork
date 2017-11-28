using System;
using System.ComponentModel;
using System.Net;

namespace Hyperion.Surrogates
{
  public sealed class IPEndPointSurrogate
  {
    public byte[] A;
    public int P;

    public static IPEndPointSurrogate ToSurrogate(IPEndPoint ep)
        => new IPEndPointSurrogate() { A = ep.Address.GetAddressBytes(), P = ep.Port };

    public static IPEndPoint FromSurrogate(IPEndPointSurrogate surrogate)
        => new IPEndPoint(new IPAddress(surrogate.A), surrogate.P);
  }
}
