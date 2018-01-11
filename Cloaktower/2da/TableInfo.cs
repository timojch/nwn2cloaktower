using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower._2da
{
  public class TableInfo : List<ColumnInfo>
  {
    public static List<TableInfo> Wildcards
    {
      get; private set;
    }

    public string Group { get; set; }
    public bool Minify { get; set; }

    public TableInfo()
    {
      this.Add(new ColumnInfo("Index", ColumnType.Index));
      if (Wildcards == null)
        Wildcards = new List<TableInfo>();
    }

    public TableInfo(string[] columnNames)
    {
      Group = CTCore.GetArg("default2dagroup", "default");
      this.Add(new ColumnInfo("Index", ColumnType.Index));
      foreach(string column in columnNames)
      {
        ColumnType type;
        string name = column;
        switch(column.ToLowerInvariant())
        {
          case "label": type = ColumnType.Label; name = "Label"; break;
          case "removed": type = ColumnType.Integer; break;
          default: type = ColumnType.String; break;
        }
        
        this.Add(new ColumnInfo(name, type));
      }
    }

    public void Save(string path)
    {
      List<string> toWrite = new List<string>();
      toWrite.Add("!" + Group);
      foreach(ColumnInfo info in this)
      {
        toWrite.Add(info.ToString());
      }
      File.WriteAllLines(path, toWrite.ToArray());
    }

    public void Load(string path)
    {
      if(File.Exists(path))
      {
        string[] input = File.ReadAllLines(path);
        for (int i = 0; i < input.Length; ++i)
        {
          if(input[i][0] == '!')
          {
            Group = input[i].Substring(1);
          }
          else if(input[i][0] == '@')
          {
            switch(input[i].Substring(1).ToLowerInvariant())
            {
              case "minify":
                Minify = true;
                break;
            }
          }
          else try
          {
            ColumnInfo readInfo = ColumnInfo.Parse(input[i]);
            int colIndex = this.FindIndex( (ColumnInfo checkColumn) => { return checkColumn.Name == readInfo.Name; });
            if (colIndex >= 0 && this[colIndex].Name == readInfo.Name)
            {
              this[colIndex] = readInfo;
            }
            else
            {
              CTDebug.Warn("Could not find column {0}", readInfo.Name);
            }
          }
          catch (FormatException e)
          {
            CTDebug.Error(e.ToString());
          }
        }
      }
    }
  }
}
