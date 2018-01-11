using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Tlk
{
  public class TlkDocument : CTDocument
  {
    static TlkDocument s_dialogTlk;
    public static TlkDocument DialogTlk
    {
      get
      {
        if (s_dialogTlk != null)
          return s_dialogTlk;
        else
        {
          s_dialogTlk = new TlkDocument("tlk");
          string path = s_dialogTlk.Owner.GetResourcePath(CTResourceType.TLK, "dialog", true);
          s_dialogTlk.m_contents = TLKReader.GetFileContents(path);
          return s_dialogTlk;
        }
      }
    }
    public const int UserTlkOffset = 16777216;

    private TlkContents[] m_contents;

    public TlkDocument(string name)
      : base(CTCore.GetOpenProject(), name)
    {
      DocType = DocumentType.TLK;
    }

    public string this[int index]
    {
      get { return m_contents[index].textContent; }
      set { m_contents[index].textContent = value; m_contents[index].type = 1; }
    }

    public int GetOffset()
    {
      if (DialogTlk == this)
        return 0;
      else return UserTlkOffset + Offset;
    }

    public void LoadCompiled()
    {
      string path = Owner.GetResourcePath(CTResourceType.TLK, Name, true);
      m_contents = TLKReader.GetFileContents(path);
    }

    public TlkContents[] Contents { get { return m_contents; } }
    public int Offset { get; set; }
  }
}
