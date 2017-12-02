using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuteAnt.Extensions.Serialization
{
	/// <summary>FormattingSR</summary>
	internal sealed class FormattingSR
	{
		internal const String AsyncResult_CallbackThrewException = "Async Callback threw an exception.";
		internal const String AsyncResult_MultipleCompletes = "The IAsyncResult implementation '{0}' tried to complete a single operation multiple times. This could be caused by an incorrect application IAsyncResult implementation or other extensibility code, such as an IAsyncResult that returns incorrect CompletedSynchronously values or invokes the AsyncCallback multiple times.";
		internal const String AsyncResult_MultipleEnds = "End cannot be called twice on an AsyncResult.";
		internal const String AsyncResult_ResultMismatch = "An incorrect IAsyncResult was provided to an 'End' method. The IAsyncResult object passed to 'End' must be the one returned from the matching 'Begin' or passed to the callback provided to 'Begin'.";
		internal const String ByteRangeStreamContentNoRanges = "Found zero byte ranges. There must be at least one byte range provided.";
		internal const String ByteRangeStreamContentNotBytesRange = "The range unit '{0}' is not valid. The range must have a unit of '{1}'.";
		internal const String ByteRangeStreamEmpty = "The stream over which '{0}' provides a range view must have a length greater than or equal to 1.";
		internal const String ByteRangeStreamInvalidFrom = "The 'From' value of the range must be less than or equal to {0}.";
		internal const String ByteRangeStreamNoneOverlap = "None of the requested ranges ({0}) overlap with the current extent of the selected resource.";
		internal const String ByteRangeStreamNoOverlap = "The requested range ({0}) does not overlap with the current extent of the selected resource.";
		internal const String ByteRangeStreamNotSeekable = "The stream over which '{0}' provides a range view must be seekable.";
		internal const String ByteRangeStreamReadOnly = "This is a read-only stream.";
		internal const String CannotHaveNullInList = "A null '{0}' is not valid.";
		internal const String CannotUseMediaRangeForSupportedMediaType = "The '{0}' of '{1}' cannot be used as a supported media type because it is a media range.";
		internal const String CannotUseNullValueType = "The '{0}' type cannot accept a null value for the value type '{1}'.";
		internal const String CookieInvalidName = "The specified value is not a valid cookie name.";
		internal const String CookieNull = "Cookie cannot be null.";
		internal const String DelegatingHandlerArrayContainsNullItem = "The '{0}' list is invalid because it contains one or more null items.";
		internal const String DelegatingHandlerArrayHasNonNullInnerHandler = "The '{0}' list is invalid because the property '{1}' of '{2}' is not null.";
		internal const String ErrorReadingFormUrlEncodedStream = "Error reading HTML form URL-encoded data stream.";
		internal const String FormUrlEncodedMismatchingTypes = "Mismatched types at node '{0}'.";
		internal const String FormUrlEncodedParseError = "Error parsing HTML form URL-encoded data, byte {0}.";
		internal const String HttpInvalidStatusCode = "Invalid HTTP status code: '{0}'. The status code must be between {1} and {2}.";
		internal const String HttpInvalidVersion = "Invalid HTTP version: '{0}'. The version must start with the characters '{1}'.";
		internal const String HttpMessageContentAlreadyRead = "The '{0}' of the '{1}' has already been read.";
		internal const String HttpMessageContentStreamMustBeSeekable = "The '{0}' must be seekable in order to create an '{1}' instance containing an entity body.";
		internal const String HttpMessageErrorReading = "Error reading HTTP message.";
		internal const String HttpMessageInvalidMediaType = "Invalid '{0}' instance provided. It does not have a content type header with a value of '{1}'.";
		internal const String HttpMessageParserEmptyUri = "HTTP Request URI cannot be an empty string.";
		internal const String HttpMessageParserError = "Error parsing HTTP message header byte {0} of message {1}.";
		internal const String HttpMessageParserInvalidHostCount = "An invalid number of '{0}' header fields were present in the HTTP Request. It must contain exactly one '{0}' header field but found {1}.";
		internal const String HttpMessageParserInvalidUriScheme = "Invalid URI scheme: '{0}'. The URI scheme must be a valid '{1}' scheme.";
		internal const String InvalidArrayInsert = "Invalid array at node '{0}'.";
		internal const String JQuery13CompatModeNotSupportNestedJson = "Traditional style array without '[]' is not supported with nested object at location {0}.";
		internal const String JsonSerializerFactoryReturnedNull = "The '{0}' method returned null. It must return a JSON serializer instance.";
		internal const String JsonSerializerFactoryThrew = "The '{0}' method threw an exception when attempting to create a JSON serializer.";
		internal const String MaxDepthExceeded = "The maximum read depth ({0}) has been exceeded because the form url-encoded data being read has more levels of nesting than is allowed.";
		internal const String MaxHttpCollectionKeyLimitReached = "The number of keys in a NameValueCollection has exceeded the limit of '{0}'. You can adjust it by modifying the MaxHttpCollectionKeys property on the '{1}' class.";
		internal const String MediaTypeFormatter_BsonParseError_MissingData = "Error parsing BSON data; unable to read content as a {0}.";
		internal const String MediaTypeFormatter_BsonParseError_UnexpectedData = "Error parsing BSON data; unexpected dictionary content: {0} entries, first key '{1}'.";
		internal const String MediaTypeFormatter_JsonReaderFactoryReturnedNull = "The '{0}' method returned null. It must return a JSON reader instance.";
		internal const String MediaTypeFormatter_JsonWriterFactoryReturnedNull = "The '{0}' method returned null. It must return a JSON writer instance.";
		internal const String MediaTypeFormatterCannotRead = "The media type formatter of type '{0}' does not support reading because it does not implement the ReadFromStreamAsync method.";
		internal const String MediaTypeFormatterCannotReadSync = "The media type formatter of type '{0}' does not support reading because it does not implement the ReadFromStream method.";
		internal const String MediaTypeFormatterCannotWrite = "The media type formatter of type '{0}' does not support writing because it does not implement the WriteToStreamAsync method.";
		internal const String MediaTypeFormatterCannotWriteSync = "The media type formatter of type '{0}' does not support writing because it does not implement the WriteToStream method.";
		internal const String MediaTypeFormatterNoEncoding = "No encoding found for media type formatter '{0}'. There must be at least one supported encoding registered in order for the media type formatter to read or write content.";
		internal const String MimeMultipartParserBadBoundary = "MIME multipart boundary cannot end with an empty space.";
		internal const String MultipartFormDataStreamProviderNoContentDisposition = "Did not find required '{0}' header field in MIME multipart body part.";
		internal const String MultipartStreamProviderInvalidLocalFileName = "Could not determine a valid local file name for the multipart body part.";
		internal const String NestedBracketNotValid = "Nested bracket is not valid for '{0}' data at position {1}.";
		internal const String NonNullUriRequiredForMediaTypeMapping = "A non-null request URI must be provided to determine if a '{0}' matches a given request or response message.";
		internal const String NoReadSerializerAvailable = "No MediaTypeFormatter is available to read an object of type '{0}' from content with media type '{1}'.";
		internal const String ObjectAndTypeDisagree = "An object of type '{0}' cannot be used with a type parameter of '{1}'.";
		internal const String ObjectContent_FormatterCannotWriteType = "The configured formatter '{0}' cannot write an object of type '{1}'.";
		internal const String QueryStringNameShouldNotNull = "Query string name cannot be null.";
		internal const String ReadAsHttpMessageUnexpectedTermination = "Unexpected end of HTTP message stream. HTTP message is not complete.";
		internal const String ReadAsMimeMultipartArgumentNoBoundary = "Invalid '{0}' instance provided. It does not have a '{1}' content-type header with a '{2}' parameter.";
		internal const String ReadAsMimeMultipartArgumentNoContentType = "Invalid '{0}' instance provided. It does not have a content-type header value. '{0}' instances must have a content-type header starting with '{1}'.";
		internal const String ReadAsMimeMultipartArgumentNoMultipart = "Invalid '{0}' instance provided. It does not have a content type header starting with '{1}'.";
		internal const String ReadAsMimeMultipartErrorReading = "Error reading MIME multipart body part.";
		internal const String ReadAsMimeMultipartErrorWriting = "Error writing MIME multipart body part to output stream.";
		internal const String ReadAsMimeMultipartHeaderParseError = "Error parsing MIME multipart body part header byte {0} of data segment {1}.";
		internal const String ReadAsMimeMultipartParseError = "Error parsing MIME multipart message byte {0} of data segment {1}.";
		internal const String ReadAsMimeMultipartStreamProviderException = "The stream provider of type '{0}' threw an exception.";
		internal const String ReadAsMimeMultipartStreamProviderNull = "The stream provider of type '{0}' returned null. It must return a writable '{1}' instance.";
		internal const String ReadAsMimeMultipartStreamProviderReadOnly = "The stream provider of type '{0}' returned a read-only stream. It must return a writable '{1}' instance.";
		internal const String ReadAsMimeMultipartUnexpectedTermination = "Unexpected end of MIME multipart stream. MIME multipart message is not complete.";
		internal const String RemoteStreamInfoCannotBeNull = "The '{0}' method in '{1}' returned null. It must return a RemoteStreamResult instance containing a writable stream and a valid URL.";
		internal const String SerializerCannotSerializeType = "The '{0}' serializer cannot serialize the type '{1}'.";
		internal const String UnMatchedBracketNotValid = "There is an unmatched opened bracket for the '{0}' at position {1}.";
		internal const String UnsupportedIndent = "Indentation is not supported by '{0}'.";
		internal const String XmlMediaTypeFormatter_InvalidSerializerType = "The object of type '{0}' returned by {1} must be an instance of either XmlObjectSerializer or XmlSerializer.";
		internal const String XmlMediaTypeFormatter_NullReturnedSerializer = "The object returned by {0} must not be a null value.";
	}
}
