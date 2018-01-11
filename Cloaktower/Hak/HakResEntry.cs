using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Hak
{
  public class HakResEntry
  {
    public const int Size = 8;

    public int OffsetToResource;
    public int ResourceSize;

    public static HakResEntry Read(BinaryReader reader)
    {
      HakResEntry ret = new HakResEntry();

      ret.OffsetToResource = reader.ReadInt32();
      ret.ResourceSize = reader.ReadInt32();

      return ret;
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(OffsetToResource);
      writer.Write(ResourceSize);
    }
  }
}
