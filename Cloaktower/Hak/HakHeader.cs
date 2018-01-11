using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Hak
{
  public class HakHeader
  {
    static System.Text.Encoding coder = System.Text.Encoding.UTF8;
    public const int Size = 160;

    public string Header = "ERF V1.1";
    public int StringCount = 0;
    public int StringTableSize = 0;
    public int EntryCount = 0;
    public int OffsetToString = 0;
    public int OffsetToKeyList = 0;
    public int OffsetToResources = 0;
    public int BuildYear = 0;
    public int BuildDay = 0;
    public int DescriptionIndex = 0;
    public Byte[] Reserved = new Byte[116];

    public static HakHeader Read(string filename)
    {
      HakHeader ret = new HakHeader();
      BinaryReader read = new BinaryReader(File.OpenRead(filename));

      ret.Header = coder.GetString(read.ReadBytes(8));
      ret.StringCount       = read.ReadInt32();
      ret.StringTableSize   = read.ReadInt32();
      ret.EntryCount        = read.ReadInt32();
      ret.OffsetToString    = read.ReadInt32();
      ret.OffsetToKeyList   = read.ReadInt32();
      ret.OffsetToResources = read.ReadInt32();
      ret.BuildYear         = read.ReadInt32();
      ret.BuildDay          = read.ReadInt32();
      ret.DescriptionIndex  = read.ReadInt32();
      ret.Reserved          = read.ReadBytes(116);

      return ret;
    }

    public void Write(BinaryWriter writer)
    {
      Byte[] headerBytes = coder.GetBytes(Header);
      Array.Resize<Byte>(ref headerBytes, 8);
      writer.Write(headerBytes);
      writer.Write(StringCount);
      writer.Write(StringTableSize);
      writer.Write(EntryCount);
      writer.Write(OffsetToString);
      writer.Write(OffsetToKeyList);
      writer.Write(OffsetToResources);
      writer.Write(BuildYear);
      writer.Write(BuildDay);
      writer.Write(DescriptionIndex);
      writer.Write(Reserved);

    }
  }
}
