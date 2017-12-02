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

namespace CuteAnt.Extensions.Serialization.Protobuf.Internal
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using ProtoBuf;

  internal sealed class RowWriter
  {
    private readonly ProtoWriter m_writer;
    private readonly IEnumerable<ProtoDataColumn> m_columns;
    private readonly ProtoDataWriterOptions m_options;
    private int m_rowIndex;

    internal RowWriter(ProtoWriter writer, IEnumerable<ProtoDataColumn> columns, ProtoDataWriterOptions options)
    {
      if (writer == null) { throw new ArgumentNullException("writer"); }

      if (columns == null) { throw new ArgumentNullException("columns"); }

      if (options == null) { throw new ArgumentNullException("options"); }

      m_writer = writer;
      m_columns = columns;
      m_options = options;
      m_rowIndex = 0;
    }

    internal void WriteRow(IDataRecord row)
    {
      int fieldIndex = 1;
      ProtoWriter.WriteFieldHeader(3, WireType.StartGroup, m_writer);
      SubItemToken token = ProtoWriter.StartSubItem(m_rowIndex, m_writer);

      foreach (ProtoDataColumn column in m_columns)
      {
        object value = row[column.ColumnIndex];
        if (value == null || value is DBNull || (m_options.SerializeEmptyArraysAsNull && IsZeroLengthArray(value)))
        {
          // don't write anything
        }
        else
        {
          switch (column.ProtoDataType)
          {
            case ProtoDataType.String:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.String, m_writer);
              ProtoWriter.WriteString((string)value, m_writer);
              break;

            case ProtoDataType.Short:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteInt16((short)value, m_writer);
              break;

            case ProtoDataType.Decimal:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.StartGroup, m_writer);
              BclHelpers.WriteDecimal((decimal)value, m_writer);
              break;

            case ProtoDataType.Int:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteInt32((int)value, m_writer);
              break;

            case ProtoDataType.Guid:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.StartGroup, m_writer);
              BclHelpers.WriteGuid((Guid)value, m_writer);
              break;

            case ProtoDataType.DateTime:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.StartGroup, m_writer);
              BclHelpers.WriteDateTime((DateTime)value, m_writer);
              break;

            case ProtoDataType.Bool:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteBoolean((bool)value, m_writer);
              break;

            case ProtoDataType.Byte:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteByte((byte)value, m_writer);
              break;

            case ProtoDataType.Char:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteInt16((short)(char)value, m_writer);
              break;

            case ProtoDataType.Double:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Fixed64, m_writer);
              ProtoWriter.WriteDouble((double)value, m_writer);
              break;

            case ProtoDataType.Float:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Fixed32, m_writer);
              ProtoWriter.WriteSingle((float)value, m_writer);
              break;

            case ProtoDataType.Long:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.Variant, m_writer);
              ProtoWriter.WriteInt64((long)value, m_writer);
              break;

            case ProtoDataType.ByteArray:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.String, m_writer);
              ProtoWriter.WriteBytes((byte[])value, 0, ((byte[])value).Length, m_writer);
              break;

            case ProtoDataType.CharArray:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.String, m_writer);
              ProtoWriter.WriteString(new string((char[])value), m_writer);
              break;

            case ProtoDataType.TimeSpan:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.StartGroup, m_writer);
              BclHelpers.WriteTimeSpan((TimeSpan)value, m_writer);
              break;

            case ProtoDataType.DateTimeOffset:
              ProtoWriter.WriteFieldHeader(fieldIndex, WireType.StartGroup, m_writer);
              DateTimeOffsetSerializer.WriteDateTimeOffset((DateTimeOffset)value, m_writer);
              break;

            default:
              throw new UnsupportedColumnTypeException(
                  ConvertProtoDataType.ToClrType(column.ProtoDataType));
          }
        }

        fieldIndex++;
      }

      ProtoWriter.EndSubItem(token, m_writer);
      m_rowIndex++;
    }

    private static bool IsZeroLengthArray(object value)
    {
      var array = value as Array;

      if (array == null)
      {
        return false;
      }

      return array.Length == 0;
    }
  }
}