using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower._2da
{
  public enum ColumnType
  {
    Index,
    Label,
    String,
    ScriptConst,
    Integer,
    Flags,
    Float,
    TableRef,
    ColumnRef,
    ScriptRef,
    TLKRef,
    Enum
  }

  public class ColumnInfo : IComparable<ColumnInfo>
  {
    public static ColumnInfo LabelColumn = new ColumnInfo("Label", ColumnType.Label);
    public static ColumnInfo IndexColumn = new ColumnInfo("Index", ColumnType.Index);

    public ColumnType Type;
    public string ReferenceTo;
    public string Name;
    public int Width;

    public override string ToString()
    {
      if (ReferenceTo == null || ReferenceTo.Length == 0)
        return String.Format("{0}:{1}", Name, Type);
      else
        return String.Format("{0}:{1}:{2}", Name, Type, ReferenceTo);
    }

    public static ColumnInfo Parse(string data)
    {
      string[] split = data.Split(':');
      if (split.Length < 2)
        throw new FormatException("Not enough values to parse column info");

      string name = split[0];
      ColumnType ctype = ColumnType.String;

      switch(split[1].ToLowerInvariant())
      {
        case "index": ctype = ColumnType.Index; break;
        case "label": ctype = ColumnType.Label; break;
        case "string": ctype = ColumnType.String; break;
        case "scriptconst": ctype = ColumnType.ScriptConst; break;
        case "integer": ctype = ColumnType.Integer; break;
        case "flags": ctype = ColumnType.Flags; break;
        case "float": ctype = ColumnType.Float; break;
        case "tableref": ctype = ColumnType.TableRef; break;
        case "columnref": ctype = ColumnType.ColumnRef; break;
        case "scriptref": ctype = ColumnType.ScriptRef; break;
        case "tlkref": ctype = ColumnType.TLKRef; break;
        case "enum": ctype = ColumnType.Enum; break;
        default:
          CTDebug.Error("Could not parse column type: {0}", split[1]);
          break;
      }
      
      string refTo = "";
      if (split.Length == 3)
        refTo = split[2];

      return new ColumnInfo(name, ctype, refTo);
    }

    public ColumnInfo(string name, ColumnType type, string refTo = "")
    {
      Type = type;
      Name = name; 
      ReferenceTo = refTo;
    }

    public int CompareTo(ColumnInfo other)
    {
      if (Type != other.Type || Type == ColumnType.Index || Type == ColumnType.Label)
        return Type.CompareTo(other.Type);
      else if (Name != other.Name)
        return Name.CompareTo(other.Name);
      else
        return 0;
    }
  }
}
