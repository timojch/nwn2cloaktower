using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloaktower.Tlk;

namespace Cloaktower._2da
{
  public class CellData
  {
    private object DataValue;
    private bool RefResolved = true;
    private readonly DataRow ParentRow;
    private string SourceString;

    public static CellData Null = new CellData(null);

    public CellData(DataRow parentRow)
    {
      ParentRow = parentRow;
    }

    public string ToDataString(ColumnInfo inColumn)
    {
      if (DataValue == null && inColumn.Type != ColumnType.Index)
        return SourceString;
      if (DataValue is int && (int)DataValue == int.MinValue)
        return SourceString;

      switch(inColumn.Type)
      {
        case ColumnType.Index:
          return ParentRow.Index.ToString();
        case ColumnType.Label:
        case ColumnType.String:
        case ColumnType.ScriptRef:
        case ColumnType.ScriptConst:
        case ColumnType.Enum:
          return (string)DataValue;
        case ColumnType.Integer:
          return ((int)DataValue).ToString();
        case ColumnType.Flags:
          return "0x" + ((UInt32)DataValue).ToString("X");
        case ColumnType.Float:
          return ((float)DataValue).ToString();
        case ColumnType.TableRef:
          {
            TableDocument doc = CTCore.GetOpenProject().GetDocument<TableDocument>(inColumn.ReferenceTo);
            Guid refID = (Guid)DataValue;
            if (refID != Guid.Empty)
              return doc.GetRow(refID).Index.ToString();
            else
              return SourceString;
          }
        case ColumnType.TLKRef:
          {
            object[] data = (object[])DataValue;
            if ((int)data[1] == -1)
              return SourceString;

            return CTCore.GetOpenProject().GetTlkStack().GetIndex((int)data[1], (string)data[0]).ToString();
          }
        default: return "INVALID_CELL_DATA";
      }
    }

    public string ToFriendlyString(ColumnInfo inColumn)
    {
      if (DataValue == null && inColumn.Type != ColumnType.Index)
        return SourceString;
      if (DataValue is int && (int)DataValue == int.MinValue)
        return SourceString;

      switch (inColumn.Type)
      {
        case ColumnType.Index:
          return string.Format("{0}", ParentRow.Index.ToString());
        case ColumnType.Label:
        case ColumnType.String:
        case ColumnType.ScriptRef:
        case ColumnType.ScriptConst:
        case ColumnType.Enum:
          return (string)DataValue;
        case ColumnType.Integer:
          return ((int)DataValue).ToString();
        case ColumnType.Flags:
          return "0x" + ((UInt32)DataValue).ToString("X");
        case ColumnType.Float:
          return ((float)DataValue).ToString();
        case ColumnType.TableRef:
          {
            TableDocument doc = CTCore.GetOpenProject().GetDocument<TableDocument>(inColumn.ReferenceTo);
            Guid refID = (Guid)DataValue;
            if (refID != Guid.Empty)
              return doc.GetRow(refID).Label.ToString();
              else
              return SourceString;
          }
        case ColumnType.TLKRef:
          {
            Object[] data = (Object[])DataValue;

            if ((int)data[1] == -1)
              return SourceString;
            else return string.Format("{0}:{1}", (string)data[0], (int)data[1]);
          }
        case ColumnType.ColumnRef:
          {
            return null;
          }
        default: return "INVALID_CELL_DATA";
      }
    }

    public void FromFriendlyString(string input, ColumnInfo inColumn)
    {
      SourceString = input;
      DataValue = ReadFriendlyString(input, inColumn, out RefResolved);
    }

    public void FromDataString(string input, ColumnInfo inColumn)
    {
      SourceString = input;
      DataValue = ReadDataString(input, inColumn, out RefResolved);
    }

    private static Object ReadFriendlyString(string input, ColumnInfo inColumn, out bool refResolved)
    {
      refResolved = true;
      switch(inColumn.Type)
      {
        case ColumnType.Index:
          return null;
        case ColumnType.Label:
        case ColumnType.String:
        case ColumnType.ScriptRef:
        case ColumnType.ScriptConst:
        case ColumnType.Enum:
          return input;
        case ColumnType.Integer:
          return FileUtils.ParseInt(input);
        case ColumnType.Flags:
          if (input[0] == '0')
          {
            UInt32 hex = 0;
            if (UInt32.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out hex))
              return hex;
            else
              return null;
          }
          else
            return null;
        case ColumnType.Float:
        {
          float parsed;
          if (float.TryParse(input, out parsed))
            return parsed;
          else
            return null;      
        }
        case ColumnType.TableRef:
          {
            TableDocument doc = CTCore.GetOpenProject().GetDocument<TableDocument>(inColumn.ReferenceTo);
            if (doc != null && doc.IsLoaded)
              if (input != "****")
                if (doc.GetRow(input) != null)
                  return doc.GetRow(input).ID;
                else
                  return Guid.Empty;
              else
                return Guid.Empty;
            else
            {
              refResolved = false;
              return input;
            }
          }
        case ColumnType.TLKRef:
          {
            TlkDocument doc;

            string[] parsed = input.Split(':');
            if (parsed.Length == 1)
              return new Object[] { "tlk", -1 };
            doc = CTCore.GetOpenProject().GetDocument<TlkDocument>(parsed[0]);
            if (parsed[0] == TlkDocument.DialogTlk.Name)
              doc = TlkDocument.DialogTlk;
            int index;
            int.TryParse(parsed[1], out index);
            return new Object[] { doc.Name, index };
          }
        default: throw new FormatException("Input was incorrectly formatted."); ;
      }
    }

    private static Object ReadDataString(string input, ColumnInfo inColumn, out bool refResolved)
    {
      refResolved = true;
      try
      {
        switch (inColumn.Type)
        {
          case ColumnType.Index:
            return null;
          case ColumnType.Label:
          case ColumnType.String:
          case ColumnType.ScriptRef:
          case ColumnType.ScriptConst:
          case ColumnType.Enum:
            return input;
          case ColumnType.Integer:
            return FileUtils.ParseInt(input);
          case ColumnType.Flags:
            if (input[0] == '0')
            {
              UInt32 hex = 0;
              if (UInt32.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out hex))
                return hex;
              else
                return null;
            }
            else
              return null;
          case ColumnType.Float:
            {
              float parsed;
              if (float.TryParse(input, out parsed))
                return parsed;
              else
                return null;
            }
          case ColumnType.TableRef:
            {
              TableDocument doc = CTCore.GetOpenProject().GetDocument<TableDocument>(inColumn.ReferenceTo);
              if(doc != null && doc.IsLoaded)
              {
                int index = FileUtils.ParseInt(input);
                DataRow row = doc.GetRow(index);
                if (row != null)
                  return row.ID;
                else
                  return Guid.Empty;
              }
              else
              {
                refResolved = false;
                return FileUtils.ParseInt(input);
              }
            }
          case ColumnType.TLKRef:
            {
              string docName;
              int localVal;

              int index;
              if (int.TryParse(input, out index))
              {
                CTCore.GetOpenProject().GetTlkStack().GetEntry(index, out docName, out localVal);

                return new Object[] { docName, localVal };
              }
              else return null;
            }
          default: throw new FormatException("Input was incorrectly formatted."); ;
        }
      }
      catch(FormatException)
      {
        return null;
      }
    }
  
    public void ResolveRef(ColumnInfo inColumn)
    {
      if (RefResolved) return;

      switch(inColumn.Type)
      {
        case ColumnType.TableRef:
          if(DataValue is string)
          {
            DataValue = ReadFriendlyString((string)DataValue, inColumn, out RefResolved);
            System.Diagnostics.Debug.Assert(RefResolved);
          }
          else if(DataValue is int)
          {
            DataValue = ReadDataString(((int)DataValue).ToString(), inColumn, out RefResolved);
            System.Diagnostics.Debug.Assert(RefResolved);
          }
          break;
        default:
          System.Diagnostics.Debug.Assert(RefResolved);
          break;
      }
    }

    public bool NeedsResolution { get { return !this.RefResolved; } }
  }
}
