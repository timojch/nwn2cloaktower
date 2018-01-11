using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower.Hak
{
  public struct HakKeyEntry
  {
    static Encoding coder = Encoding.UTF8;
    public const int Size = 40;

    public string Resref;
    public int ResID;
    public Int16 ResType;
    public Int16 Reserved;

    public static HakKeyEntry Read(BinaryReader read)
    {
      
      HakKeyEntry ret = new HakKeyEntry();
      ret.Resref = coder.GetString(read.ReadBytes(32)).TrimEnd();
      ret.ResID = read.ReadInt32();
      ret.ResType = read.ReadInt16();
      ret.Reserved = read.ReadInt16();

      return ret;
    }

    public void Write(BinaryWriter write)
    {
      Byte[] resrefBytes = coder.GetBytes(Resref);
      Array.Resize<Byte>(ref resrefBytes, 32);
      write.Write(resrefBytes);
      write.Write(ResID);
      write.Write(ResType);
      write.Write(Reserved);
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", Resref, ResType);
    }
  }
}
