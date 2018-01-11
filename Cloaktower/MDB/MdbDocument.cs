using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower.MDB
{
  public class MdbDocument : CTDocument
  {
    public MdbDocument(string name)
      : base(CTCore.GetOpenProject(), name)
    {
      m_packetHeaders = new List<MdbPacketHeader>();
      m_packets = new List<MdbPacket>();
    }

    public void Read(string filename)
    {
      BinaryReader reader = new BinaryReader(File.OpenRead(filename));

      m_header = reader.ReadMdbFileHeader();
      for(int i = 0; i < m_header.NumPackets; ++i)
      {
        m_packetHeaders.Add(reader.ReadMdbPacketHeader());
      }
      for(int i =0 ;i < m_header.NumPackets; ++i)
      {
        if (reader.BaseStream.Position != m_packetHeaders[i].Offset)
        {
          CTDebug.Warn("At position {0}, expecting {1}", reader.BaseStream.Position, m_packetHeaders[i].Offset);
          reader.BaseStream.Seek(m_packetHeaders[i].Offset, SeekOrigin.Begin);
        }
        m_packets.Add(reader.ReadMdbPacket());
      }
    }

    public void Write(string filename)
    {
      BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename));

      writer.Write(m_header);
      foreach (var i in m_packetHeaders)
        writer.Write(i);
      foreach (var i in m_packets)
        writer.Write(i);

    }

    MdbFileHeader m_header;
    List<MdbPacketHeader> m_packetHeaders;
    List<MdbPacket> m_packets;

    public IEnumerable<MdbPacket> GetPackets()
    {
      return m_packets;
    }
  }
}
