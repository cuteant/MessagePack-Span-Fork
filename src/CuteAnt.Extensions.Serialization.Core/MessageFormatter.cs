using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CuteAnt.Buffers;
using CuteAnt.IO;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>Base class to handle serializing and deserializing strongly-typed objects.</summary>
  public abstract partial class MessageFormatter : IMessageFormatter
  {
    private const int c_initialBufferSize = 1024 * 64;
    private const int c_zeroSize = 0;
    private static readonly ArrayPool<byte> s_defaultMemoryPool = BufferManager.Shared;

    #region @@ Constructors @@

    /// <summary>Initializes a new instance of the <see cref="MessageFormatter"/> class.</summary>
    protected MessageFormatter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MessageFormatter"/> class.</summary>
    /// <param name="formatter">The <see cref="MessageFormatter"/> instance to copy settings from.</param>
    protected MessageFormatter(MessageFormatter formatter)
    {
    }

    #endregion

    #region -- DeepCopy --

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    public virtual object DeepCopyObject(object source)
    {
      if (source == null) { return null; }

      var type = source.GetType();
      using (var ms = MemoryStreamManager.GetStream())
      {
        WriteToStream(type, source, ms, Encoding.UTF8);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return ReadFromStream(type, ms, Encoding.UTF8);
      }
    }

    /// <summary>Tries to create a copy of source.</summary>
    /// <param name="source">The item to create a copy of</param>
    /// <returns>The copy</returns>
    public virtual T DeepCopy<T>(T source) => (T)DeepCopyObject(source);

    #endregion

    #region -- CanReadType / CanWriteType --

    /// <summary>Informs the serialization manager whether this serializer supports the type for serialization.</summary>
    /// <param name="type">The type of the item to be serialized</param>
    /// <returns>A value indicating whether the item can be serialized.</returns>
    public abstract bool IsSupportedType(Type type);

    /// <summary>Determines whether this <see cref="MessageFormatter"/> can deserialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be deserialized.</remarks>
    /// <param name="type">The type of object that will be deserialized.</param>
    /// <returns><c>true</c> if this <see cref="MessageFormatter"/> can deserialize an object of that type; otherwise <c>false</c>.</returns>
    public virtual bool CanReadType(Type type) => IsSupportedType(type);

    /// <summary>Determines whether this <see cref="MessageFormatter"/> can serialize an object of the specified type.</summary>
    /// <remarks>Derived classes must implement this method and indicate if a type can or cannot be serialized.</remarks>
    /// <param name="type">The type of object that will be serialized.</param>
    /// <returns><c>true</c> if this <see cref="MessageFormatter"/> can serialize an object of that type; otherwise <c>false</c>.</returns>
    public virtual bool CanWriteType(Type type) => IsSupportedType(type);

    #endregion

    #region --& GetDefaultValueForType &--

    private static Dictionary<Type, object> s_defaultValueTypes = new Dictionary<Type, object>();

    /// <summary>Gets the default value for the specified type.</summary>
    public static object GetDefaultValueForType(Type type)
    {
      if (null == type) { return null; }

      if (!type.IsValueType) return null;

      if (s_defaultValueTypes.TryGetValue(type, out object defaultValue)) return defaultValue;

      defaultValue = Activator.CreateInstance(type);

      Dictionary<Type, object> snapshot, newCache;
      do
      {
        snapshot = s_defaultValueTypes;
        newCache = new Dictionary<Type, object>(s_defaultValueTypes)
        {
          [type] = defaultValue
        };
      } while (!ReferenceEquals(Interlocked.CompareExchange(ref s_defaultValueTypes, newCache, snapshot), snapshot));

      return defaultValue;
    }

    #endregion
  }
}
