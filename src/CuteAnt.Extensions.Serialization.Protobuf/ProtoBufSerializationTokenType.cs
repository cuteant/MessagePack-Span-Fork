using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuteAnt.Extensions.Serialization.Protobuf
{
  internal enum ProtoBufSerializationTokenType
  {
    /// <summary>Object</summary>
    Default = 0,

    /// <summary>DataSet</summary>
    DataSet = 1,

    /// <summary>DataTable</summary>
    DataTable = 2
  }
}
