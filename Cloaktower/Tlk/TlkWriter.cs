using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower.Tlk
{
  public class TLKWriter
  {
    public static void SaveFileContents(string filePath, TlkContents[] data)
    {
      TLKWriter writer = new TLKWriter(filePath);
      writer.Save(data);
    }

    FileStream m_outStream;

    public TLKWriter(string filePath)
    {
      m_outStream = File.OpenWrite(filePath);
    }

    private UInt32 SwapEndians(UInt32 liInt)
    {
      UInt32 ret = 0;
      ret += liInt % 256; liInt <<= 8;
      ret += liInt % 256; liInt <<= 8;
      ret += liInt % 256; liInt <<= 8;
      ret += liInt % 256; liInt <<= 8;
      return ret;
    }
    
    private void Save(TlkContents[] data)
    {
      BinaryWriter writer = new BinaryWriter(m_outStream);
      SaveHeader(data, writer);
      SaveContentsTable(data, writer);
      SaveStrings(data, writer);
      writer.Flush();
      writer.Close();
    }

    private void SaveStrings(TlkContents[] data, BinaryWriter writer)
    {
      Encoding coder = TlkLanguage.DefaultEncoding;

      for(int i = 0; i < data.Length; ++i)
      {
        writer.Write(coder.GetBytes(data[i].textContent));
      }
    }

    private void SaveContentsTable(TlkContents[] data, BinaryWriter writer)
    {
      Encoding coder = TlkLanguage.DefaultEncoding;
      uint offset = 0;

      for(int i = 0; i < data.Length; ++i)
      {
        TlkEntry entry;
        entry.type = data[i].type;
        entry.resName = coder.GetBytes(data[i].soundResName);
        Array.Resize<Byte>(ref entry.resName, 16);
        entry.soundEntry = new Byte[8];
        entry.startPosition = offset;
        entry.entrySize = (uint)coder.GetByteCount(data[i].textContent);
        offset += entry.entrySize;
        entry.magic = 0;

        writer.Write(entry.type);
        writer.Write(entry.resName);
        writer.Write(entry.soundEntry);
        writer.Write(entry.startPosition);
        writer.Write(entry.entrySize);
        writer.Write(entry.magic);
      }
    }

    private void SaveHeader(TlkContents[] data, BinaryWriter writer)
    {
      TlkHeader header;
      header.version = TlkHeader.TLKHEADER;
      header.languageID = 0;
      header.numEntries = (uint)data.Length;
      header.startOffset = 40 * header.numEntries + 20;

      writer.Write(header.version);
      writer.Write(header.languageID);
      writer.Write(header.numEntries);
      writer.Write(header.startOffset);
    }
  }
}
