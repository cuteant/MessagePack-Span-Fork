using Newtonsoft.Json.Serialization;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>The default serialization binder used when resolving and loading classes from type names.</summary>
  public sealed class JsonSerializationBinder : CuteAnt.Serialization.DefaultSerializationBinder, ISerializationBinder
  {
    public static readonly JsonSerializationBinder Instance = new JsonSerializationBinder();

    /// <summary>Initializes a new instance of the <see cref="JsonSerializationBinder"/> class.</summary>
    public JsonSerializationBinder()
    {
    }
  }
}
