using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CuteAnt.IO;
using Microsoft.Extensions.Logging;

namespace CuteAnt.Extensions.Serialization
{
  /// <summary>BinaryMessageFormatter</summary>
  public class BinaryMessageFormatter : MessageFormatter
  {
    private static readonly ILogger s_logger = TraceLogger.GetLogger<BinaryMessageFormatter>();

    /// <summary>The default singlegton instance</summary>
    public static readonly BinaryMessageFormatter DefaultInstance = new BinaryMessageFormatter();

    #region -- IsSupportedType --

    /// <inheritdoc />
    public override bool IsSupportedType(Type type)
    {
      if (type == null) { throw new ArgumentNullException(nameof(type)); }

      return type.
#if !NET40
             GetTypeInfo().
#endif
             IsSerializable;
    }

    #endregion

    #region -- DeepCopy --

    /// <inheritdoc />
    public sealed override object DeepCopyObject(object source)
    {
      if (null == source) { return null; }

      var formatter = new BinaryFormatter();

      using (var ms = MemoryStreamManager.GetStream())
      {
        formatter.Serialize(ms, source);
        ms.Seek(0, System.IO.SeekOrigin.Begin);
        return formatter.Deserialize(ms);
      }
    }

    #endregion

    #region -- ReadFromStream --

    /// <inheritdoc />
    public sealed override object ReadFromStream(Type type, Stream readStream, Encoding effectiveEncoding)
    {
      if (readStream == null) { throw new ArgumentNullException(nameof(readStream)); }

      // 不是 Stream 都会实现 Position、Length 这两个属性
      //if (readStream.Position == readStream.Length) { return type != null ? GetDefaultValueForType(type) : null; }

      try
      {
        var formatter = new BinaryFormatter();
        return formatter.Deserialize(readStream);
      }
      catch (Exception ex)
      {
        s_logger.LogError(ex.ToString());
        return GetDefaultValueForType(type);
      }
    }

    #endregion

    #region -- WriteToStream --

    /// <inheritdoc />
    public sealed override void WriteToStream(object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      var formatter = new BinaryFormatter();
      formatter.Serialize(writeStream, value);
      writeStream.Flush();
    }

    /// <inheritdoc />
    public sealed override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
    {
      if (null == value) { return; }

      if (writeStream == null) { throw new ArgumentNullException(nameof(writeStream)); }

      var formatter = new BinaryFormatter();
      formatter.Serialize(writeStream, value);
      writeStream.Flush();
    }

    #endregion

    #region -- class DynamicBinder --

    /// <summary>This appears necessary because the BinaryFormatter by default will not see types
    /// that are defined by the InvokerGenerator.
    /// Needs to be public since it used by generated client code.</summary>
    class DynamicBinder : SerializationBinder
    {
      public static readonly SerializationBinder Instance = new DynamicBinder();

      private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

      public override Type BindToType(string assemblyName, string typeName)
      {
        lock (this.assemblies)
        {
          Assembly result;
          if (!this.assemblies.TryGetValue(assemblyName, out result))
          {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
              this.assemblies[assembly.GetName().FullName] = assembly;

            // in some cases we have to explicitly load the assembly even though it seems to be already loaded but for some reason it's not listed in AppDomain.CurrentDomain.GetAssemblies()
            if (!this.assemblies.TryGetValue(assemblyName, out result))
              this.assemblies[assemblyName] = Assembly.Load(new AssemblyName(assemblyName));

            result = this.assemblies[assemblyName];
          }

          return result.GetType(typeName);
        }
      }
    }

    #endregion
  }
}
