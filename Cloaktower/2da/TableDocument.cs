using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower._2da
{
  public class TableDocument : CTDocument
  {
    static Dictionary<string, TableDocument> s_allOpenTables = new Dictionary<string, TableDocument>();
    public static IReadOnlyDictionary<string, TableDocument> OpenTables
    {
      get { return s_allOpenTables; }
    }

    TableInfo m_columns;
    List<DataRow> m_docRows;
    Dictionary<string, DataRow> m_byLabels;
    Dictionary<string, DataRow> m_removedRows;
    Dictionary<string, int> m_columnIndeces;

    List<string> m_dupeWarnings;

    public bool HasUnresolvedReferences { get; private set; }

    public string Category { get { return m_columns.Group; } }

    public TableDocument(string name)
      :base(CTCore.GetOpenProject(), name)
    {
      m_columns = new TableInfo();
      m_docRows = new List<DataRow>();
      m_byLabels = new Dictionary<string, DataRow>();
      m_removedRows = new Dictionary<string, DataRow>();
      m_columnIndeces = new Dictionary<string, int>();
      m_dupeWarnings = new List<string>();
      DocType = CTDocument.DocumentType.Table;
      HasUnresolvedReferences = false;
      CanSave = true;
      s_allOpenTables.Add(name, this);
    }

    public List<ColumnInfo> Columns { get { return m_columns; } }

    public void AddRow(DataRow row)
    {
      row.Index = m_docRows.Count;
      m_docRows.Add(row);
      if (row.Label != "****" && row.Exists)
      {
        while (m_byLabels.ContainsKey(row.Label))
        {
          row.DuplicateIndex++;
          //m_dupeWarnings.Add(string.Format("Warning: Duplicate {0} in {1}.2da. (rows {2} and {3})", row.Label, Name, m_docRows.Count - 1, m_byLabels[row.Label].Index));
        }
        
        m_byLabels.Add(row.Label, row);
      }
      else
      {
        if(!m_removedRows.ContainsKey(row.Label))
          m_removedRows.Add(row.Label, row);
      }
      if (row.HasUnresolvedReferences)
        HasUnresolvedReferences = true;
    }

    public void AddRow(DataRow row, int atLine)
    {
      throw new NotImplementedException();
    }

    public int GetLineNumber(Guid rowID)
    {
      return m_docRows.IndexOf(CTRef.Lookup<DataRow>(rowID));
    }
    public DataRow GetRow(int index)
    {
      if (index < 0 || index > m_docRows.Count)
        return null;
      return m_docRows[index];
    }
    public DataRow GetRow(Guid rowID)
    {
      return CTRef.Lookup<DataRow>(rowID);
    }
    public DataRow GetRow(string label)
    {
      if (m_byLabels.ContainsKey(label))
        return m_byLabels[label];
      else if (m_removedRows.ContainsKey(label))
        return m_removedRows[label];
      else
        return null;
    }

    public void LoadNewest()
    {
      string compiledPath = Owner.GetResourcePath(CTResourceType.TableRaw, Name, true);
      string friendlyPath = Owner.GetResourcePath(CTResourceType.TableFriendly, Name, true);
      string infoPath = GetTableInfoPath();

      int newestOne = 0;
      DateTime newestDate = DateTime.MinValue;
      
      if (File.Exists(friendlyPath))
        if (File.GetLastWriteTimeUtc(friendlyPath) > newestDate)
        {
          // Prefer Friendly over Compiled in case 2da stack changes
          newestDate = File.GetLastWriteTimeUtc(friendlyPath);
          newestOne = 1;
        }
      if (File.Exists(compiledPath))
        if (File.GetLastWriteTimeUtc(compiledPath) > newestDate)
        {
          newestDate = File.GetLastWriteTimeUtc(compiledPath);
          newestOne = 2;
        }
      if (File.Exists(infoPath))
        if (File.GetLastWriteTimeUtc(infoPath) > newestDate)
        {
          // Prefer compiled over friendly only when the tableinfo has changed
          newestDate = File.GetLastWriteTimeUtc(infoPath);
          newestOne = 3;
        }


      m_docRows.Clear();
      m_byLabels.Clear(); 
      IsLoaded = false;

      switch(newestOne)
      {
        case 1: // Friendly
          LoadFriendly();
          break;
        case 2: // Compiled
          LoadCompiled();
          break;
        case 3: // Info
          if(File.Exists(compiledPath))
            LoadCompiled();
          else if(File.Exists(friendlyPath))
            LoadFriendly();
          else
            throw new FileNotFoundException(string.Format("No instance of {0}.2da could be found.", Name));
          break;
        default: // File not found
        throw new FileNotFoundException(string.Format("No instance of {0}.2da could be found.", Name));
      }

      IsLoaded = true;
    }

    public void SaveAll()
    {
      Owner.SaveResource(CTResourceType.TableRaw, Name, SaveCompiled());
      Owner.SaveResource(CTResourceType.TableFriendly, Name, SaveFriendly());
    }

    public void LoadCompiled()
    {
      string name = this.Name;
      string dataPath = Owner.GetResourcePath(CTResourceType.TableRaw, name);
      string infoPath = GetTableInfoPath();
      string[] table = System.IO.File.ReadAllLines(dataPath);

      // First two lines are ignored.
      // Third line is columns.

      int firstRow = 1;
      while (table[firstRow].Trim().Length == 0)
        firstRow++;

      string[] columnNames = FileUtils.Tokenize(table[firstRow]);
      m_columns = new TableInfo(columnNames);
      for (int i = 0; i < m_columns.Count; ++i)
        m_columnIndeces[m_columns[i].Name] = i;
      m_columns.Load(infoPath);
      if(!File.Exists(infoPath))
        m_columns.Save(infoPath);

      int firstIndex = firstRow+1;
      for(int i = 0; i + firstIndex < table.Length; ++i)
      {
        string line = table[i + firstIndex];
        DataRow row = DataRow.ReadRaw(this, line);
        if(row != null)
          AddRow(row);
      }

      this.IsLoaded = true;
    }
    public string[] SaveCompiled()
    {
      foreach (ColumnInfo col in m_columns)
        col.Width = col.Name.Length + 1;
      foreach(DataRow row in m_docRows)
        foreach(ColumnInfo col in m_columns)
        {
          int width = row.GetData(col).ToDataString(col).Length + 1;
          if (width > col.Width)
            col.Width = width;
        }

      string[] write = new string[m_docRows.Count + 3];
      write[0] = FileUtils.Header;

      foreach (ColumnInfo col in m_columns)
      {
        string output = "";
        if (col.Type != ColumnType.Index)
          output = col.Name.ToString();
        if (m_columns.Minify)
          output += " ";
        else
          output = output.PadRight(col.Width);
        write[2] += output;
      }
      int i = 3;
      foreach(DataRow row in m_docRows)
      {
        foreach (ColumnInfo col in m_columns)
        {
          string output = row.GetData(col).ToDataString(col);
          if (m_columns.Minify)
            output += " ";
          else
            output = output.PadRight(col.Width);
          write[i] += output;

        }
        i++;
      }

      return write;
    }
    public void LoadFriendly()
    {
      string name = this.Name;
      string dataPath = Owner.GetResourcePath(CTResourceType.TableFriendly, name);
      string infoPath = GetTableInfoPath();
      string[] table = System.IO.File.ReadAllLines(dataPath);

      // First two lines are ignored.
      // Third line is columns.

      string[] columnNames = FileUtils.Tokenize(table[2]);
      m_columns = new TableInfo(columnNames);
      for (int i = 0; i < m_columns.Count; ++i)
        m_columnIndeces[m_columns[i].Name] = i;
      m_columns.Load(infoPath);
      if(!File.Exists(infoPath))
        m_columns.Save(infoPath);

      int firstIndex = 3;
      for(int i = 0; i + firstIndex < table.Length; ++i)
      {
        string line = table[i + firstIndex];
        DataRow row = DataRow.ReadFriendly(this, line);
        AddRow(row);
      }

      this.IsLoaded = true;
    }
    public string[] SaveFriendly()
    {
      foreach (ColumnInfo col in m_columns)
        col.Width = col.Name.Length + 1;
      foreach(DataRow row in m_docRows)
        foreach(ColumnInfo col in m_columns)
        {
          int width = row.GetData(col).ToFriendlyString(col).Length + 1;
          if (width > col.Width)
            col.Width = width;
        }

      string[] write = new string[m_docRows.Count + 3];
      write[0] = FileUtils.Header;

      foreach(ColumnInfo col in m_columns)
      {
        string output = "";
        if(col.Type != ColumnType.Index)
          output = col.Name.ToString();
        output = output.PadRight(col.Width);
        write[2] += output;
      }
      int i = 3;
      foreach (DataRow row in m_docRows)
      {
        foreach (ColumnInfo col in m_columns)
        {
          string output = row.GetData(col).ToFriendlyString(col);
          output = output.PadRight(col.Width);
          write[i] += output;
        }
        i++;
      }

      return write;
    }

    public void ResolveReferences()
    {
      if(HasUnresolvedReferences)
      {
        CTDebug.Info("Resolving references in {0}", Name);
        foreach(ColumnInfo col in m_columns)
          if(col.Type == ColumnType.TableRef)
          {
            TableDocument table = CTCore.GetOpenProject().GetDocument<TableDocument>(col.ReferenceTo);
            table.PrintDuplicationWarnings();
          }
        foreach (DataRow row in m_docRows)
          row.ResolveReferences();
        HasUnresolvedReferences = false;
      }
    }

    public void PrintDuplicationWarnings()
    {
      foreach(string warn in m_dupeWarnings)
      {
        CTDebug.Warn(warn);
      }
      m_dupeWarnings.Clear();
    }

    internal ColumnInfo GetColumn(string colName)
    {
      return m_columns[m_columnIndeces[colName]];
    }

    private class ResourceLengthComparer : IComparer<CTResource>
    {

      public int Compare(CTResource x, CTResource y)
      {
        return y.Filename.Length - x.Filename.Length;
      }
    }

    private string GetTableInfoPath()
    {
      // If there's a resource just for us, use it
      string defaultResourcePath = (Owner.GetResourcePath(CTResourceType.TableInfo, Name, true, false));
      if (File.Exists(defaultResourcePath))
        return defaultResourcePath;

      // If not, check wildcards
      var collection = CTCore.GetOpenProject().GetAllResourcesOfType(CTResourceType.TableInfo);
      Array.Sort<CTResource>(collection, new ResourceLengthComparer());
      foreach(CTResource infoResource in collection)
      {
        if (Name.StartsWith(infoResource.Filename))
          return infoResource.Path;
      }

      // If there's nothing for us yet, make one.
      return defaultResourcePath;
    }

    public bool CanSave { get; set; }
  }
}
