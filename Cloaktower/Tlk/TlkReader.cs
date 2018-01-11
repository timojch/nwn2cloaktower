using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower.Tlk
{
  public class TLKReader
  {
    public static TlkContents[] GetFileContents(string filePath)
    {
      TLKReader reader = new TLKReader(filePath);
      return reader.m_contents;
    }

    FileStream m_inStream;
    TlkHeader m_header;
    TlkContents[] m_contents;

    public TLKReader(string filePath)
    {
      m_inStream = File.OpenRead(filePath);

      ReadHeader();
      ReadContents();

      m_inStream = null;
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
    private void ReadHeader()
    {
      m_header = new TlkHeader();
      m_inStream.Seek(0, SeekOrigin.Begin);
      BinaryReader reader = new BinaryReader(m_inStream);
      m_header.version = reader.ReadBytes(8);
      m_header.languageID  = reader.ReadUInt32();
      m_header.numEntries  = reader.ReadUInt32();
      m_header.startOffset = reader.ReadUInt32();
    }

    private void ReadContents()
    {
      m_contents = new TlkContents[m_header.numEntries];
      long startPosition = m_header.startOffset;
      TlkEntry[] entryTable = new TlkEntry[m_header.numEntries];

      m_inStream.Seek(20, SeekOrigin.Begin);
      BinaryReader reader = new BinaryReader(m_inStream);

      // Read table of entries
      for (UInt32 i = 0; i < m_header.numEntries; ++i)
      {
        entryTable[i].type = reader.ReadUInt32(); // big-endian
        entryTable[i].resName = reader.ReadBytes(16);
        entryTable[i].soundEntry = reader.ReadBytes(8);
        entryTable[i].startPosition = reader.ReadUInt32();
        entryTable[i].entrySize = reader.ReadUInt32();
        entryTable[i].magic = reader.ReadUInt32();
      }

      // Read contents of strings
      Encoding coder = TlkLanguage.DefaultEncoding;
      for (UInt32 i = 0; i < m_header.numEntries; ++i)
      {
        reader.BaseStream.Seek(startPosition + entryTable[i].startPosition, SeekOrigin.Begin);
        Byte[] content = reader.ReadBytes((int)entryTable[i].entrySize);
        m_contents[i].type = entryTable[i].type;
        m_contents[i].soundResName = coder.GetString(entryTable[i].resName);
        m_contents[i].textContent = coder.GetString(content);
      }
    }
  }
}
