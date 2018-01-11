using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower._2da
{
  public class DataRow : ICTReferenceable
  {
    public readonly TableDocument InTable;
    private CTRef m_id;
    public bool Exists = false;
    public bool HasUnresolvedReferences { get; private set; }
    private int m_dupeIndex;
    private string m_rawLabel = "";

    public Guid ID
    {
      get { return m_id.ID; }
    }

    Dictionary<string, CellData> m_data;

    public DataRow(TableDocument table)
    {
      InTable = table;
      m_data = new Dictionary<string, CellData>();
      m_id = new CTRef(this);
      Exists = false;
      HasUnresolvedReferences = false;
      m_dupeIndex = 0;
    }

    public ColumnInfo GetColumn(string name)
    {
      return InTable.GetColumn(name);
    }

    public CellData GetData(ColumnInfo column)
    {
      if (m_data.ContainsKey(column.Name))
        return m_data[column.Name];
      else
        return CellData.Null;
    }

    public static DataRow ReadFriendly(TableDocument table, string data)
    {
      DataRow ret = new DataRow(table);
      string[] columnValues = FileUtils.Tokenize(data);
      ret.Exists = true;

      if (columnValues.Length != table.Columns.Count)
        throw new FormatException("Row is not correctly formatted.");
      else 
        for (int i = 0; i < columnValues.Length; ++i)
        {
          ColumnInfo ci = table.Columns[i];

          CellData cellContent = new CellData(ret);
          cellContent.FromFriendlyString(columnValues[i], ci);
          ret.m_data.Add(ci.Name, cellContent);
          if (ci.Type == ColumnType.Label && columnValues[i] == "****")
            ret.Exists = false;
          if (ci.Name.ToLowerInvariant() == "removed")
            if (!char.IsNumber(columnValues[i][0]) || Int32.Parse(columnValues[i]) != 0)
            {
              ret.Exists = false;
            }
          if (cellContent.NeedsResolution)
            ret.HasUnresolvedReferences = true;
        }


      return ret;
    }

    public static DataRow ReadRaw(TableDocument table, string data)
    {
      DataRow ret = new DataRow(table);
      string[] columnValues = FileUtils.Tokenize(data);
      ret.Exists = true;

      if (columnValues.Length == 0)
        return null;

      if (columnValues.Length != table.Columns.Count)
        throw new FormatException(string.Format("Row {0} is not correctly formatted.", columnValues[0]));
      else
        for (int i = 0; i < columnValues.Length; ++i)
        {
          ColumnInfo ci = table.Columns[i];
          CellData cellContent = new CellData(ret);
          cellContent.FromDataString(columnValues[i], ci);
          ret.m_data.Add(ci.Name, cellContent);
          if (ci.Type == ColumnType.Label && columnValues[i] == "****")
            ret.Exists = false;
          if(ci.Name.ToLowerInvariant() == "removed")
            if(!char.IsNumber(columnValues[i][0]) || Int32.Parse(columnValues[i]) != 0)
            {
              ret.Exists = false;
            }
          if (cellContent.NeedsResolution)
            ret.HasUnresolvedReferences = true;
        }

      return ret;
    }

    public string Label
    {
      get
      {
        if (m_data.ContainsKey(ColumnInfo.LabelColumn.Name))
          return m_data[ColumnInfo.LabelColumn.Name].ToDataString(ColumnInfo.LabelColumn);
        else
          return Index.ToString();
      }
      set
      {
        m_rawLabel = value;
        if (m_data.ContainsKey(ColumnInfo.LabelColumn.Name))
        {
          m_data[ColumnInfo.LabelColumn.Name] = new CellData(this);
          if (m_dupeIndex > 0)
          {
            m_data[ColumnInfo.LabelColumn.Name].FromDataString(string.Format("{0}_{1}", value, m_dupeIndex), ColumnInfo.LabelColumn);
          }
          else
          {
            m_data[ColumnInfo.LabelColumn.Name].FromDataString(value, ColumnInfo.LabelColumn);
          }
        }
      }
    }
    public int Index
    {
      get;
      set;
    }
    public int DuplicateIndex
    {
      get { return m_dupeIndex; }
      set
      {
        m_dupeIndex += 1;
        if (m_rawLabel.Length == 0)
          m_rawLabel = Label;
        Label = m_rawLabel;
      }
    }

    public void ResolveReferences()
    {
      foreach (string colName in m_data.Keys)
        try
        {
          m_data[colName].ResolveRef(InTable.GetColumn(colName));
        }
        catch (Exception)
        {
          CTDebug.Error("Could not resolve reference in table {0}, row {1} column {2}", InTable.Name, Index, colName);
          m_data[colName].FromDataString("****", new ColumnInfo("string", ColumnType.String));
          InTable.CanSave = false;
        }
      HasUnresolvedReferences = false;
    }
  }
}
