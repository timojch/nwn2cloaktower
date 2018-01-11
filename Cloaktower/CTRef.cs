using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public interface ICTReferenceable
  {
    Guid ID { get; }
  }

  public struct CTRef
  {
    static Dictionary<Guid, CTRef> s_dict = new Dictionary<Guid, CTRef>();

    readonly Guid m_ID;
    ICTReferenceable m_target;

    public CTRef(ICTReferenceable target)
    {
      m_ID = Guid.NewGuid();
      m_target = target;
      s_dict[m_ID] = this;
    }
    public CTRef(Guid id, ICTReferenceable target)
    {
      m_ID = id;
      m_target = target;
      s_dict[m_ID] = this;
    }

    public void Release()
    {
      s_dict.Remove(m_ID);
    }

    public Guid ID { get { return m_ID; } }

    public static ICTReferenceable Lookup(Guid id)
    {
      return s_dict[id].m_target;
    }
    public static T Lookup<T>(Guid id) where T : ICTReferenceable
    {
      return (T)(s_dict[id].m_target);
    }

    public static CTRef Parse(string input, ICTReferenceable target)
    {
      return new CTRef(Guid.Parse(input), target);
    }

    public override string ToString()
    {
      return m_ID.ToString();
    }
  }
}
