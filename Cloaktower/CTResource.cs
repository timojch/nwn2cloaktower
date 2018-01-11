using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public enum CTResourceType
  {
    TableRaw,
    TableFriendly,
    TableCT,
    TableInfo,
    TLK,
    TextFile
  }
  public class CTResource
  {
    string m_filename;
    CTResourceType m_resourceType;

    public CTResource(string filename, CTResourceType resource = CTResourceType.TextFile)
    {
      m_filename = filename;
      m_resourceType = resource;
    }

    public string Path
    {
      get { return m_resourceType.ToString() + "\\" + m_filename + "." + Extension; }
    }

    public CTResourceType ResourceType
    {
      get { return m_resourceType; }
    }

    public string Filename
    {
      get { return m_filename; }
    }

    public string Extension
    {
      get
      {
        switch(m_resourceType)
        {
          case CTResourceType.TableRaw: return "2da";
          case CTResourceType.TableFriendly: return "2da";
          case CTResourceType.TableCT: return "ct2da";
          case CTResourceType.TableInfo: return "2dainfo";
          case CTResourceType.TLK: return "tlk";
          case CTResourceType.TextFile: return "txt";
          default: return "dat";
        }
      }
    }

    public string ToString()
    {
      return Filename;
    }
  }
}
