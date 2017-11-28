using System;
using System.ComponentModel;

namespace Hyperion.Surrogates
{
  public sealed class UriSurrogate : StringPayloadSurrogate
  {
    [ThreadStatic]
    private static TypeConverter _uriConverter;

    public static UriSurrogate ToSurrogate(Uri uri)
    {
      if (_uriConverter == null) _uriConverter = TypeDescriptor.GetConverter(typeof(Uri));
      return new UriSurrogate() { S = _uriConverter.ConvertToInvariantString(uri) };
    }

    public static Uri FromSurrogate(UriSurrogate surrogate)
    {
      if (_uriConverter == null) _uriConverter = TypeDescriptor.GetConverter(typeof(Uri));
      return (Uri)_uriConverter.ConvertFromInvariantString(surrogate.S);
    }
  }
}
