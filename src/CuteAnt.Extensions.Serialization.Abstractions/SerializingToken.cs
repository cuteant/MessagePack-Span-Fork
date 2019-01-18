namespace CuteAnt.Extensions.Serialization
{
    public enum SerializingToken
    {
        /// <summary>Using <c>Newtonsoft.Json</c> serialization.</summary>
        Json,

        /// <summary>Using <c>Utf8Json</c> serialization.</summary>
        Utf8Json,

        /// <summary>Using <c>MessagePack</c> serialization.</summary>
        MessagePack,

        /// <summary>Using <c>MessagePack</c> serialization.</summary>
        TypelessMessagePack,

        /// <summary>Using <c>MessagePack</c> serialization and the <c>LZ4</c> loseless compression.</summary>
        Lz4MessagePack,

        /// <summary>Using <c>MessagePack</c> serialization and the <c>LZ4</c> loseless compression.</summary>
        Lz4TypelessMessagePack,

        // /// <summary>Using <c>Hyperion</c> serialization.</summary>
        // Hyperion,

        /// <summary>Using <c>protobuf-net</c> serialization.</summary>
        Protobuf,

        /// <summary>Using external serialization.</summary>
        External
    }
}