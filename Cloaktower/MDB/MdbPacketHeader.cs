using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.MDB
{
  public class MdbPacketHeader
  {
    public string Signature;
    public UInt32 Offset;
    public override string ToString() { return string.Format("{0}: {1}", Signature, Offset); }
  }
}
