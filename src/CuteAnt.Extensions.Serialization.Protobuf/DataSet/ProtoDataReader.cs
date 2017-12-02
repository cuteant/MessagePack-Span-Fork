// Copyright 2012 Richard Dingwall - http://richarddingwall.name
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#pragma warning disable 1591
namespace CuteAnt.Extensions.Serialization.Protobuf
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.IO;
  using CuteAnt.Extensions.Serialization.Protobuf.Internal;
  using ProtoBuf;

  /// <summary>A custom <see cref="System.Data.IDataReader"/> for de-serializing a protocol-buffer binary stream back into a tabular form.</summary>
  public sealed class ProtoDataReader : IDataReader
  {
    private readonly List<ColReader> m_colReaders;

    private Stream m_stream;
    private object[] m_currentRow;
    private DataTable m_dataTable;
    private bool m_disposed;
    private ProtoReader m_reader;
    private int m_currentField;
    private SubItemToken m_currentTableToken;
    private bool m_reachedEndOfCurrentTable;

    /// <summary>Initializes a new instance of the <see cref="ProtoDataReader"/> class.</summary>
    /// <param name="stream">The <see cref="System.IO.Stream"/> to read from.</param>
    public ProtoDataReader(Stream stream)
    {
      if (stream == null) { throw new ArgumentNullException(nameof(stream)); }

      m_stream = stream;
      m_reader = new ProtoReader(stream, null, null);
      m_colReaders = new List<ColReader>();

      AdvanceToNextField();
      if (m_currentField != 1)
      {
        throw new InvalidOperationException("No results found! Invalid/corrupt stream.");
      }

      ReadNextTableHeader();
    }

    ~ProtoDataReader()
    {
      Dispose(false);
    }

    private delegate object ColReader();

    public int FieldCount
    {
      get
      {
        ErrorIfClosed();
        return m_dataTable.Columns.Count;
      }
    }

    public int Depth
    {
      get
      {
        ErrorIfClosed();
        return 1;
      }
    }

    public bool IsClosed { get; private set; }

    /// <summary>Gets the number of rows changed, inserted, or deleted.</summary>
    /// <returns>This is always zero in the case of <see cref="CuteAnt.Extensions.Serialization.Protobuf.ProtoDataReader" />.</returns>
    public int RecordsAffected
    {
      get { return 0; }
    }

    object IDataRecord.this[int i]
    {
      get
      {
        ErrorIfClosed();
        return GetValue(i);
      }
    }

    object IDataRecord.this[string name]
    {
      get
      {
        ErrorIfClosed();
        return GetValue(GetOrdinal(name));
      }
    }

    public string GetName(int i)
    {
      ErrorIfClosed();
      return m_dataTable.Columns[i].ColumnName;
    }

    public string GetDataTypeName(int i)
    {
      ErrorIfClosed();
      return m_dataTable.Columns[i].DataType.Name;
    }

    public Type GetFieldType(int i)
    {
      ErrorIfClosed();
      return m_dataTable.Columns[i].DataType;
    }

    public object GetValue(int i)
    {
      ErrorIfClosed();
      return m_currentRow[i];
    }

    public int GetValues(object[] values)
    {
      ErrorIfClosed();

      int length = Math.Min(values.Length, m_dataTable.Columns.Count);

      Array.Copy(m_currentRow, values, length);

      return length;
    }

    public int GetOrdinal(string name)
    {
      ErrorIfClosed();
      return m_dataTable.Columns[name].Ordinal;
    }

    public bool GetBoolean(int i)
    {
      ErrorIfClosed();
      return (bool)m_currentRow[i];
    }

    public byte GetByte(int i)
    {
      ErrorIfClosed();
      return (byte)m_currentRow[i];
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      ErrorIfClosed();
      var sourceBuffer = (byte[])m_currentRow[i];
      length = Math.Min(length, m_currentRow.Length - (int)fieldOffset);
      Array.Copy(sourceBuffer, fieldOffset, buffer, bufferoffset, length);
      return length;
    }

    public char GetChar(int i)
    {
      ErrorIfClosed();
      return (char)m_currentRow[i];
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      ErrorIfClosed();
      var sourceBuffer = (char[])m_currentRow[i];
      length = Math.Min(length, m_currentRow.Length - (int)fieldoffset);
      Array.Copy(sourceBuffer, fieldoffset, buffer, bufferoffset, length);
      return length;
    }

    public Guid GetGuid(int i)
    {
      ErrorIfClosed();
      return (Guid)m_currentRow[i];
    }

    public short GetInt16(int i)
    {
      ErrorIfClosed();
      return (short)m_currentRow[i];
    }

    public int GetInt32(int i)
    {
      ErrorIfClosed();
      return (int)m_currentRow[i];
    }

    public long GetInt64(int i)
    {
      ErrorIfClosed();
      return (long)m_currentRow[i];
    }

    public float GetFloat(int i)
    {
      ErrorIfClosed();
      return (float)m_currentRow[i];
    }

    public double GetDouble(int i)
    {
      ErrorIfClosed();
      return (double)m_currentRow[i];
    }

    public string GetString(int i)
    {
      ErrorIfClosed();
      return (string)m_currentRow[i];
    }

    public decimal GetDecimal(int i)
    {
      ErrorIfClosed();
      return (decimal)m_currentRow[i];
    }

    public DateTime GetDateTime(int i)
    {
      ErrorIfClosed();
      return (DateTime)m_currentRow[i];
    }

    public IDataReader GetData(int i)
    {
      ErrorIfClosed();
      return ((DataTable)m_currentRow[i]).CreateDataReader();
    }

    public bool IsDBNull(int i)
    {
      ErrorIfClosed();
      return m_currentRow[i] == null || m_currentRow[i] is DBNull;
    }

    public void Close()
    {
      m_stream.Close();
      IsClosed = true;
    }

    public bool NextResult()
    {
      ErrorIfClosed();

      ConsumeAnyRemainingRows();

      AdvanceToNextField();

      if (m_currentField == 0)
      {
        IsClosed = true;
        return false;
      }

      m_reachedEndOfCurrentTable = false;

      ReadNextTableHeader();

      return true;
    }

    public DataTable GetSchemaTable()
    {
      ErrorIfClosed();
      using (var schemaReader = m_dataTable.CreateDataReader())
      {
        return schemaReader.GetSchemaTable();
      }
    }

    public bool Read()
    {
      ErrorIfClosed();

      if (m_reachedEndOfCurrentTable)
      {
        return false;
      }

      if (m_currentField == 0)
      {
        ProtoReader.EndSubItem(m_currentTableToken, m_reader);
        m_reachedEndOfCurrentTable = true;
        return false;
      }

      ReadCurrentRow();
      AdvanceToNextField();

      return true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (!m_disposed)
      {
        if (disposing)
        {
          if (m_reader != null)
          {
            m_reader.Dispose();
            m_reader = null;
          }

          if (m_stream != null)
          {
            m_stream.Dispose();
            m_stream = null;
          }

          if (m_dataTable != null)
          {
            m_dataTable.Dispose();
            m_dataTable = null;
          }
        }

        m_disposed = true;
      }
    }

    private void ConsumeAnyRemainingRows()
    {
      // Unfortunately, protocol buffers doesn't let you seek - we have
      // to consume all the remaining tokens up anyway
      while (Read())
      {
      }
    }

    private void ReadNextTableHeader()
    {
      ResetSchemaTable();
      m_currentRow = null;

      m_currentTableToken = ProtoReader.StartSubItem(m_reader);

      AdvanceToNextField();

      if (m_currentField == 0)
      {
        m_reachedEndOfCurrentTable = true;
        ProtoReader.EndSubItem(m_currentTableToken, m_reader);
        return;
      }

      if (m_currentField != 2)
      {
        throw new InvalidOperationException("No header found! Invalid/corrupt stream.");
      }

      ReadHeader();
    }

    private void AdvanceToNextField()
    {
      m_currentField = m_reader.ReadFieldHeader();
    }

    private void ResetSchemaTable()
    {
      m_dataTable = new DataTable();
      m_colReaders.Clear();
    }

    private void ReadHeader()
    {
      do
      {
        ReadColumn();
        AdvanceToNextField();
      }
      while (m_currentField == 2);
    }

    private void ReadColumn()
    {
      var token = ProtoReader.StartSubItem(m_reader);
      int field;
      string name = null;
      var protoDataType = (ProtoDataType)(-1);
      while ((field = m_reader.ReadFieldHeader()) != 0)
      {
        switch (field)
        {
          case 1:
            name = m_reader.ReadString();
            break;
          case 2:
            protoDataType = (ProtoDataType)m_reader.ReadInt32();
            break;
          default:
            m_reader.SkipField();
            break;
        }
      }

      switch (protoDataType)
      {
        case ProtoDataType.Int:
          m_colReaders.Add(() => m_reader.ReadInt32());
          break;
        case ProtoDataType.Short:
          m_colReaders.Add(() => m_reader.ReadInt16());
          break;
        case ProtoDataType.Decimal:
          m_colReaders.Add(() => BclHelpers.ReadDecimal(m_reader));
          break;
        case ProtoDataType.String:
          m_colReaders.Add(() => m_reader.ReadString());
          break;
        case ProtoDataType.Guid:
          m_colReaders.Add(() => BclHelpers.ReadGuid(m_reader));
          break;
        case ProtoDataType.DateTime:
          m_colReaders.Add(() => BclHelpers.ReadDateTime(m_reader));
          break;
        case ProtoDataType.Bool:
          m_colReaders.Add(() => m_reader.ReadBoolean());
          break;

        case ProtoDataType.Byte:
          m_colReaders.Add(() => m_reader.ReadByte());
          break;

        case ProtoDataType.Char:
          m_colReaders.Add(() => (char)m_reader.ReadInt16());
          break;

        case ProtoDataType.Double:
          m_colReaders.Add(() => m_reader.ReadDouble());
          break;

        case ProtoDataType.Float:
          m_colReaders.Add(() => m_reader.ReadSingle());
          break;

        case ProtoDataType.Long:
          m_colReaders.Add(() => m_reader.ReadInt64());
          break;

        case ProtoDataType.ByteArray:
          m_colReaders.Add(() => ProtoReader.AppendBytes(null, m_reader));
          break;

        case ProtoDataType.CharArray:
          m_colReaders.Add(() => m_reader.ReadString().ToCharArray());
          break;

        case ProtoDataType.TimeSpan:
          m_colReaders.Add(() => BclHelpers.ReadTimeSpan(m_reader));
          break;

        case ProtoDataType.DateTimeOffset:
          m_colReaders.Add(() => DateTimeOffsetSerializer.ReadDateTimeOffset(m_reader));
          break;

        default:
          throw new NotSupportedException(protoDataType.ToString());
      }

      ProtoReader.EndSubItem(token, m_reader);
      m_dataTable.Columns.Add(name, ConvertProtoDataType.ToClrType(protoDataType));
    }

    private void ReadCurrentRow()
    {
      int field;

      if (m_currentRow == null)
      {
        m_currentRow = new object[m_colReaders.Count];
      }
      else
      {
        Array.Clear(m_currentRow, 0, m_currentRow.Length);
      }

      SubItemToken token = ProtoReader.StartSubItem(m_reader);
      while ((field = m_reader.ReadFieldHeader()) != 0)
      {
        if (field > m_currentRow.Length)
        {
          m_reader.SkipField();
        }
        else
        {
          int i = field - 1;
          m_currentRow[i] = m_colReaders[i]();
        }
      }

      ProtoReader.EndSubItem(token, m_reader);
    }

    private void ErrorIfClosed()
    {
      if (IsClosed)
      {
        throw new InvalidOperationException("Attempt to access ProtoDataReader which was already closed.");
      }
    }
  }
}

#pragma warning restore 1591