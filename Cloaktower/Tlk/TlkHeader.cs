using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Tlk
{
  public struct TlkHeader
  {
    //"TLK V3.0"
    public static byte[] TLKHEADER = new byte[] { 0x54, 0x4c, 0x4b, 0x20, 0x56, 0x33, 0x2e, 0x30 };

    public byte[] version;
    public UInt32 languageID;
    public UInt32 numEntries;
    public UInt32 startOffset; // 40*numEntries + 20
  }

  struct TlkEntry
  {
    public UInt32 type;
    public Byte[] resName;
    public Byte[] soundEntry;
    public UInt32 startPosition;
    public UInt32 entrySize;
    public UInt32 magic;
  }

  // Post-digestion contents
  public struct TlkContents
  {
    public UInt32 type;
    public string soundResName;
    public string textContent;
  }
}
