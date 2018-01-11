using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.MDB
{
  public class MdbFileHeader
  {
    public string Signature;
    public UInt16 MajorVersion;
    public UInt16 MinorVersion;
    public UInt32 NumPackets;
  }
}
