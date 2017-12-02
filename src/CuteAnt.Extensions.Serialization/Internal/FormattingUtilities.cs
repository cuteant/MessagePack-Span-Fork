// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Provides various internal utility functions</summary>
  internal static class FormattingUtilities
  {
    // Supported date formats for input.
    private static readonly string[] dateFormats = new string[]
        {
            // "r", // RFC 1123, required output format but too strict for input
            "ddd, d MMM yyyy H:m:s 'GMT'", // RFC 1123 (r, except it allows both 1 and 01 for date and time)
            "ddd, d MMM yyyy H:m:s", // RFC 1123, no zone - assume GMT
            "d MMM yyyy H:m:s 'GMT'", // RFC 1123, no day-of-week
            "d MMM yyyy H:m:s", // RFC 1123, no day-of-week, no zone
            "ddd, d MMM yy H:m:s 'GMT'", // RFC 1123, short year
            "ddd, d MMM yy H:m:s", // RFC 1123, short year, no zone
            "d MMM yy H:m:s 'GMT'", // RFC 1123, no day-of-week, short year
            "d MMM yy H:m:s", // RFC 1123, no day-of-week, short year, no zone

            "dddd, d'-'MMM'-'yy H:m:s 'GMT'", // RFC 850
            "dddd, d'-'MMM'-'yy H:m:s", // RFC 850 no zone
            "ddd MMM d H:m:s yyyy", // ANSI C's asctime() format

            "ddd, d MMM yyyy H:m:s zzz", // RFC 5322
            "ddd, d MMM yyyy H:m:s", // RFC 5322 no zone
            "d MMM yyyy H:m:s zzz", // RFC 5322 no day-of-week
            "d MMM yyyy H:m:s", // RFC 5322 no day-of-week, no zone
        };

    // Valid header token characters are within the range 0x20 < c < 0x7F excluding the following characters
    private const string NonTokenChars = "()<>@,;:\\\"/[]?={}";

    /// <summary>Quality factor to indicate a perfect match.</summary>
    internal const double Match = 1.0;

    /// <summary>Quality factor to indicate no match.</summary>
    internal const double NoMatch = 0.0;

    /// <summary>The default max depth for our formatter is 256</summary>
    internal const int DefaultMaxDepth = 256;

    /// <summary>The default min depth for our formatter is 1</summary>
    internal const int DefaultMinDepth = 1;

    /// <summary>HTTP X-Requested-With header field name</summary>
    internal const string HttpRequestedWithHeader = @"x-requested-with";

    /// <summary>HTTP X-Requested-With header field value</summary>
    internal const string HttpRequestedWithHeaderValue = @"XMLHttpRequest";

    /// <summary>HTTP Host header field name</summary>
    internal const string HttpHostHeader = "Host";

    /// <summary>HTTP Version token</summary>
    internal const string HttpVersionToken = "HTTP";

    /// <summary>A <see cref="Type"/> representing <see cref="DelegatingEnumerable{T}"/>.</summary>
    internal static readonly Type DelegatingEnumerableGenericType = typeof(DelegatingEnumerable<>);

    /// <summary>A <see cref="Type"/> representing <see cref="IEnumerable{T}"/>.</summary>
    internal static readonly Type EnumerableInterfaceGenericType = typeof(IEnumerable<>);

    /// <summary>A <see cref="Type"/> representing <see cref="IQueryable{T}"/>.</summary>
    internal static readonly Type QueryableInterfaceGenericType = typeof(IQueryable<>);

#if !NETFX_CORE // XsdDataContractExporter is not supported in portable libraries
    /// <summary>An instance of <see cref="XsdDataContractExporter"/>.</summary>
    internal static readonly XsdDataContractExporter XsdDataContractExporter = new XsdDataContractExporter();
#endif

    /// <summary>Determines whether <paramref name="type"/> is a <see cref="JToken"/> type.</summary>
    /// <param name="type">The type to test.</param>
    /// <returns><c>true</c> if <paramref name="type"/> is a <see cref="JToken"/> type; otherwise, <c>false</c>.</returns>
    internal static bool IsJTokenType(Type type)
    {
      return typeof(JToken).IsAssignableFrom(type);
    }

    /// <summary>Create a default reader quotas with a default depth quota of 1K</summary>
    /// <returns></returns>
    internal static XmlDictionaryReaderQuotas CreateDefaultReaderQuotas()
    {
#if NETFX_CORE // MaxDepth is a DOS mitigation. We don't support MaxDepth in portable libraries because it is strictly client side.
            return XmlDictionaryReaderQuotas.Max;
#else
      return new XmlDictionaryReaderQuotas()
      {
        MaxArrayLength = Int32.MaxValue,
        MaxBytesPerRead = Int32.MaxValue,
        MaxDepth = DefaultMaxDepth,
        MaxNameTableCharCount = Int32.MaxValue,
        MaxStringContentLength = Int32.MaxValue
      };
#endif
    }

    /// <summary>Remove bounding quotes on a token if present</summary>
    /// <param name="token">Token to unquote.</param>
    /// <returns>Unquoted token.</returns>
    internal static string UnquoteToken(string token)
    {
      if (String.IsNullOrWhiteSpace(token))
      {
        return token;
      }

      if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
      {
        return token.Substring(1, token.Length - 2);
      }

      return token;
    }

    internal static bool ValidateHeaderToken(string token)
    {
      if (token == null)
      {
        return false;
      }

      foreach (char c in token)
      {
        if (c < 0x21 || c > 0x7E || NonTokenChars.IndexOf(c) != -1)
        {
          return false;
        }
      }

      return true;
    }

    internal static string DateToString(DateTimeOffset dateTime)
    {
      // Format according to RFC1123; 'r' uses invariant info (DateTimeFormatInfo.InvariantInfo)
      return dateTime.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture);
    }

    internal static bool TryParseDate(string input, out DateTimeOffset result)
    {
      return DateTimeOffset.TryParseExact(input, dateFormats, DateTimeFormatInfo.InvariantInfo,
                                          DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                          out result);
    }

    /// <summary>Parses valid integer strings with no leading signs, whitespace or other <see cref="NumberStyles"/></summary>
    /// <param name="value">The value to parse</param>
    /// <param name="result">The result</param>
    /// <returns>True if value was valid; false otherwise.</returns>
    internal static bool TryParseInt32(string value, out int result)
    {
      return Int32.TryParse(value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out result);
    }
  }
}
