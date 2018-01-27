using System;

namespace CuteAnt.Extensions.Serialization
{
    /// <summary>Interface that allows third-party serializers to perform serialization, even when
    /// the types being serialized are not known (generics) at initialization time.
    /// 
    /// Types that inherit this interface are discovered through dependency injection and 
    /// automatically incorporated in the Serialization Manager.</summary>
    public interface IExternalSerializer
    {
        /// <summary>Informs the serialization manager whether this serializer supports the type for serialization.</summary>
        /// <param name="itemType">The type of the item to be serialized</param>
        /// <returns>A value indicating whether the item can be serialized.</returns>
        bool IsSupportedType(Type itemType);

        /// <summary>Tries to serialize an item.</summary>
        /// <param name="item">The instance of the object being serialized</param>
        byte[] SerializeObject(object item);

        /// <summary>Tries to deserialize an item.</summary>
        /// <param name="expectedType">The type that should be deserialized</param>
        /// <param name="data">The data which should be deserialized.</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(Type expectedType, byte[] data);
    }
}
