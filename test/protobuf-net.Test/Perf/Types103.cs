﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Types103.proto
namespace ProtoBuf.unittest.Perf.Issue103Types
{
#if !COREFX
    [global::System.Serializable]
#endif
    [global::ProtoBuf.ProtoContract(Name=@"TypeA")]
  public partial class TypeA : global::ProtoBuf.IExtensible
  {
    public TypeA() {}
    
    private readonly global::System.Collections.Generic.List<string> _param1 = new global::System.Collections.Generic.List<string>();
    [global::ProtoBuf.ProtoMember(1, Name=@"param1", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<string> param1
    {
      get { return _param1; }
    }
  
    private readonly global::System.Collections.Generic.List<long> _param2 = new global::System.Collections.Generic.List<long>();
    [global::ProtoBuf.ProtoMember(2, Name=@"param2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<long> param2
    {
      get { return _param2; }
    }
  
    private readonly global::System.Collections.Generic.List<bool> _param3 = new global::System.Collections.Generic.List<bool>();
    [global::ProtoBuf.ProtoMember(3, Name=@"param3", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<bool> param3
    {
      get { return _param3; }
    }
  
    private readonly global::System.Collections.Generic.List<bool> _param4 = new global::System.Collections.Generic.List<bool>();
    [global::ProtoBuf.ProtoMember(4, Name=@"param4", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<bool> param4
    {
      get { return _param4; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
#if !COREFX
    [global::System.Serializable]
#endif
    [global::ProtoBuf.ProtoContract(Name=@"ContainedType")]
  public partial class ContainedType : global::ProtoBuf.IExtensible
  {
    public ContainedType() {}
    
    private string _param1;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"param1", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string param1
    {
      get { return _param1; }
      set { _param1 = value; }
    }

    private long _param2 = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"param2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long param2
    {
      get { return _param2; }
      set { _param2 = value; }
    }

    private bool _param3 = default(bool);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"param3", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool param3
    {
      get { return _param3; }
      set { _param3 = value; }
    }

    private bool _param4 = default(bool);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"param4", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool param4
    {
      get { return _param4; }
      set { _param4 = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }

#if !COREFX
    [global::System.Serializable]
#endif
    [global::ProtoBuf.ProtoContract(Name=@"TypeB")]
  public partial class TypeB : global::ProtoBuf.IExtensible
  {
    public TypeB() {}
    
    private readonly global::System.Collections.Generic.List<ProtoBuf.unittest.Perf.Issue103Types.ContainedType> _containedType = new global::System.Collections.Generic.List<ProtoBuf.unittest.Perf.Issue103Types.ContainedType>();
    [global::ProtoBuf.ProtoMember(1, Name=@"containedType", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoBuf.unittest.Perf.Issue103Types.ContainedType> containedType
    {
      get { return _containedType; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
