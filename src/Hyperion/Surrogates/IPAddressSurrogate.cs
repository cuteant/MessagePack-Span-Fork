using System;
using System.ComponentModel;
using System.Net;

namespace Hyperion.Surrogates
{
  public sealed class IPAddressSurrogate
  {
    public byte[] A;

    public static IPAddressSurrogate ToSurrogate(IPAddress ip)
        => new IPAddressSurrogate() { A = ip.GetAddressBytes() };

    public static IPAddress FromSurrogate(IPAddressSurrogate surrogate)
        => new IPAddress(surrogate.A);
  }
}
