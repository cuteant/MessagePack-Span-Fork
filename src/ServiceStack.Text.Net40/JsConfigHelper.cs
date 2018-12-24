using System;
using System.Net;
using CuteAnt;

namespace ServiceStack.Text
{
  /// <summary>DefaultJsConfigExtensions</summary>
  public static class JsConfigHelper
  {
    #region - CombGuid -

    /// <summary>Register Comb GUID Serializer</summary>
    public static void RegisterCombGuidSerializer()
    {
      JsConfig<CombGuid>.SerializeFn = s_serializeCombGuidFunc;
      JsConfig<CombGuid>.RawSerializeFn = s_serializeCombGuidFunc;
      JsConfig<CombGuid>.DeSerializeFn = s_deserializeCombGuidFunc;
      JsConfig<CombGuid>.RawDeserializeFn = s_deserializeCombGuidFunc;
    }

    private static readonly Func<CombGuid, string> s_serializeCombGuidFunc = SerializeCombGuid;
    private static string SerializeCombGuid(CombGuid comb) => comb.ToString(CombGuidFormatStringType.Comb32Digits);

    private static readonly Func<string, CombGuid> s_deserializeCombGuidFunc = DeserializeCombGuid;
    private static CombGuid DeserializeCombGuid(string raw) => new CombGuid(raw, CombGuidSequentialSegmentType.Comb);

    #endregion

    #region - IPAddress -

    /// <summary>Register IPAddress Serializer</summary>
    public static void RegisterIPAddressSerializer()
    {
      JsConfig<IPAddress>.SerializeFn = s_serializeIPAddressFunc;
      JsConfig<IPAddress>.RawSerializeFn = s_serializeIPAddressFunc;
      JsConfig<IPAddress>.DeSerializeFn = s_deserializeIPAddressFunc;
      JsConfig<IPAddress>.RawDeserializeFn = s_deserializeIPAddressFunc;
    }

    private static readonly Func<IPAddress, string> s_serializeIPAddressFunc = SerializeIPAddress;
    private static string SerializeIPAddress(IPAddress ip) => ip.ToString();

    private static readonly Func<string, IPAddress> s_deserializeIPAddressFunc = DeserializeIPAddress;
    private static IPAddress DeserializeIPAddress(string raw) => IPAddress.Parse(raw);
    #endregion

    #region - IPEndPoint -

    /// <summary>Register IPEndPoint Serializer</summary>
    public static void RegisterIPEndPointSerializer()
    {
      JsConfig<IPEndPoint>.SerializeFn = s_serializeIPEndPointFunc;
      JsConfig<IPEndPoint>.RawSerializeFn = s_serializeIPEndPointFunc;
      JsConfig<IPEndPoint>.DeSerializeFn = s_deserializeIPEndPointFunc;
      JsConfig<IPEndPoint>.RawDeserializeFn = s_deserializeIPEndPointFunc;
    }

    private static readonly Func<IPEndPoint, string> s_serializeIPEndPointFunc = SerializeIPEndPoint;
    private static string SerializeIPEndPoint(IPEndPoint ip) => $"{ip.Address.ToString()}:{ip.Port.ToString()}";

    private static readonly Func<string, IPEndPoint> s_deserializeIPEndPointFunc = DeserializeIPEndPoint;
    private static IPEndPoint DeserializeIPEndPoint(string raw)
    {
      if (string.IsNullOrWhiteSpace(raw)) { return null; }

      var ips = raw.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
      if (ips.Length == 1) { return null; }
      return new IPEndPoint(IPAddress.Parse(ips[0]), ips[1].ToInt());
    }
    #endregion
  }
}
