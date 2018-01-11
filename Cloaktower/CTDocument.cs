using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public class CTDocument : ICTReferenceable
  {
    public CTProject Owner;
    public string Name;
    public bool IsLoaded { get; protected set; }
    public DocumentType DocType { get; set; }

    private CTRef m_id;

    public Guid ID
    {
      get { return m_id.ID; }
    }

    public CTDocument(CTProject owner, string name)
    {
      Owner = owner;
      Name = name;
      m_id = new CTRef(this);
    }

    public enum DocumentType
    {
      Table,
      TLK,
      MDB,
    }
  }
}
