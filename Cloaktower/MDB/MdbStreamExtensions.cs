using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.MDB
{
  public static class MDBPacket_ReaderExtensions
  {
    public static string ReadAscii(this BinaryReader reader, int length)
    {
      byte[] bytes = reader.ReadBytes(length);
      string ret = Encoding.ASCII.GetString(bytes);
      ret = ret.TrimEnd('\0');

      return ret;
    }
    public static Name32 ReadName32(this BinaryReader reader)
    {
      Name32 ret = new Name32();

      ret.Contents = reader.ReadAscii(32);

      return ret;
    }
    public static Point3 ReadPoint3(this BinaryReader reader)
    {
      Point3 ret = new Point3();

      ret.x = reader.ReadSingle();
      ret.y = reader.ReadSingle();
      ret.z = reader.ReadSingle();

      return ret;
    }
    public static Color3 ReadColor3(this BinaryReader reader)
    {
      var ret = new Color3();

      ret.r = reader.ReadSingle();
      ret.g = reader.ReadSingle();
      ret.b = reader.ReadSingle();

      return ret;
    }
    public static Tri ReadTri(this BinaryReader reader)
    {
      var ret = new Tri();

      ret.p1 = reader.ReadUInt16();
      ret.p2 = reader.ReadUInt16();
      ret.p3 = reader.ReadUInt16();

      return ret;
    }
    public static Quat ReadQuat(this BinaryReader reader)
    {
      var ret = new Quat();

      ret.x = reader.ReadSingle();
      ret.y = reader.ReadSingle();
      ret.z = reader.ReadSingle();
      ret.w = reader.ReadSingle();

      return ret;
    }
    public static RHMatrix ReadRHMatrix(this BinaryReader reader)
    {
      var ret = new RHMatrix();

      ret.m11 = reader.ReadSingle();
      ret.m12 = reader.ReadSingle();
      ret.m13 = reader.ReadSingle();
      ret.m21 = reader.ReadSingle();
      ret.m22 = reader.ReadSingle();
      ret.m23 = reader.ReadSingle();
      ret.m31 = reader.ReadSingle();
      ret.m32 = reader.ReadSingle();
      ret.m33 = reader.ReadSingle();

      return ret;
    }
    public static Material ReadMaterial(this BinaryReader reader)
    {
      var ret = new Material();

      ret.DiffuseMap = reader.ReadName32();
      ret.NormalMap = reader.ReadName32();
      ret.TintMap = reader.ReadName32();
      ret.GlowMap = reader.ReadName32();
      ret.Kd = reader.ReadColor3();
      ret.Ks = reader.ReadColor3();
      ret.SpecularPower = reader.ReadSingle();
      ret.SpecularValue = reader.ReadSingle();
      ret.TexFlags = reader.ReadUInt32();

      return ret;
    }
    public static RVert ReadRVert(this BinaryReader reader)
    {
      var ret = new RVert();

      ret.Position = reader.ReadPoint3();
      ret.Normal = reader.ReadPoint3();
      ret.Tangent = reader.ReadPoint3();
      ret.Binormal = reader.ReadPoint3();
      ret.UVW = reader.ReadPoint3();

      return ret;
    }
    public static SVert ReadSVert(this BinaryReader reader)
    {
      var ret = new SVert();

      ret.Position = reader.ReadPoint3();
      ret.Normal = reader.ReadPoint3();
      ret.BoneWeights = reader.ReadQuat();
      ret.BoneIndices = reader.ReadBytes(4);
      ret.Tangent = reader.ReadPoint3();
      ret.Binormal = reader.ReadPoint3();
      ret.UVW = reader.ReadPoint3();
      ret.BoneCount = reader.ReadSingle();

      return ret;
    }
    public static CVert ReadCVert(this BinaryReader reader)
    {
      var ret = new CVert();

      ret.Position = reader.ReadPoint3();
      ret.Normal = reader.ReadPoint3();
      ret.UVW = reader.ReadPoint3();

      return ret;
    }
    public static CSphere ReadCSphere(this BinaryReader reader)
    {
      var ret = new CSphere();

      ret.Origin = reader.ReadUInt32();
      ret.Radius = reader.ReadSingle();

      return ret;
    }
    public static WTri ReadWTri(this BinaryReader reader)
    {
      var ret = new WTri();

      ret.p1 = reader.ReadUInt16();
      ret.p2 = reader.ReadUInt16();
      ret.p3 = reader.ReadUInt16();
      ret.flags = reader.ReadUInt32();

      return ret;
    }
    
    public static MdbPacket ReadMdbPacket(this BinaryReader reader)
    {
      string Signature = reader.ReadAscii(4);
      UInt32 Size = reader.ReadUInt32();
      MdbPacket ret = null;
      long debugCheckPos = reader.BaseStream.Position;

      switch(Signature)
      {
        case "RIGD":
          ret = reader.ReadMdbPacket_RIGD(Size);
          break;
        case "SKIN":
          ret = reader.ReadMdbPacket_SKIN(Size);
          break;
        case "COL2":
        case "COL3":
          ret = reader.ReadMdbPacket_COLN(Size);
          break;
        case "COLS":
          ret = reader.ReadMdbPacket_COLS(Size);
          break;
        case "WALK":
          ret = reader.ReadMdbPacket_WALK(Size);
          break;
        case "HOOK":
          ret = reader.ReadMdbPacket_HOOK(Size);
          break;
        case "TRRN":
          ret = reader.ReadMdbPacket_TRRN(Size);
          break;
        case "HELM":
          ret = reader.ReadMdbPacket_HELM(Size);
          break;
        case "HAIR":
          ret = reader.ReadMdbPacket_HAIR(Size);
          break;
        default:
          CTDebug.Error("Unrecognized object format: {0}", Signature);
          break;
      }

      long diff = reader.BaseStream.Position - debugCheckPos;
      if(diff != Size)
      {
        CTDebug.Warn("Invalid size of MDB Packet.");
      }
      ret.Size = Size;
      ret.Signature = Signature;

      return ret;
    }

    private static MdbPacket_RIGD ReadMdbPacket_RIGD(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_RIGD();

      ret.Name = reader.ReadName32();
      ret.Mat = reader.ReadMaterial();
      ret.NumVerts = reader.ReadUInt32();
      ret.NumFaces = reader.ReadUInt32();
      ret.Verts = new RVert[ret.NumVerts];
      ret.Faces = new Tri[ret.NumFaces];
      for (int i = 0; i < ret.NumVerts; ++i)
        ret.Verts[i] = reader.ReadRVert();
      for (int i = 0; i < ret.NumFaces; ++i)
        ret.Faces[i] = reader.ReadTri();

      return ret;
    }
    private static MdbPacket_SKIN ReadMdbPacket_SKIN(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_SKIN();

      ret.Name = reader.ReadName32();
      ret.Skeleton = reader.ReadName32();
      ret.Mat = reader.ReadMaterial();
      ret.NumVerts = reader.ReadUInt32();
      ret.NumFaces = reader.ReadUInt32();
      ret.Verts = new SVert[ret.NumVerts];
      ret.Faces = new Tri[ret.NumFaces];
      for (int i = 0; i < ret.NumVerts; ++i)
        ret.Verts[i] = reader.ReadSVert();
      for (int i = 0; i < ret.NumFaces; ++i)
        ret.Faces[i] = reader.ReadTri();

      return ret;
    }
    private static MdbPacket_COLN ReadMdbPacket_COLN(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_COLN();

      ret.Name = reader.ReadName32();
      ret.Mat = reader.ReadMaterial();
      ret.NumVerts = reader.ReadUInt32();
      ret.NumFaces = reader.ReadUInt32();
      ret.Verts = new CVert[ret.NumVerts];
      ret.Faces = new Tri[ret.NumFaces];
      for (int i = 0; i < ret.NumVerts; ++i)
        ret.Verts[i] = reader.ReadCVert();
      for (int i = 0; i < ret.NumFaces; ++i)
        ret.Faces[i] = reader.ReadTri();

      return ret;
    }
    private static MdbPacket_COLS ReadMdbPacket_COLS(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_COLS();

      ret.NumItems = reader.ReadUInt32();
      ret.Items = new CSphere[ret.NumItems];
      for (int i = 0; i < ret.NumItems; ++i)
        ret.Items[i] = reader.ReadCSphere();

        return ret;
    }
    private static MdbPacket_WALK ReadMdbPacket_WALK(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_WALK();

      ret.Name = reader.ReadName32();
      ret.uiFlags = reader.ReadUInt32();
      ret.NumVerts = reader.ReadUInt32();
      ret.NumFaces = reader.ReadUInt16();
      ret.Verts = new Point3[ret.NumVerts];
      ret.Faces = new WTri[ret.NumFaces];
      for (int i = 0; i < ret.NumVerts; ++i)
        ret.Verts[i] = reader.ReadPoint3();
      for (int i = 0; i < ret.NumFaces; ++i)
        ret.Faces[i] = reader.ReadWTri();

      return ret;
    }
    private static MdbPacket_HOOK ReadMdbPacket_HOOK(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_HOOK();

      ret.Name = reader.ReadName32();
      ret.HookType = reader.ReadUInt16();
      ret.HookSize = reader.ReadUInt16();
      ret.Position = reader.ReadPoint3();
      ret.RotationMatrix = reader.ReadRHMatrix();

      return ret;
    }
    private static MdbPacket_TRRN ReadMdbPacket_TRRN(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_TRRN();
      ret.data = reader.ReadBytes((int)size);
      return ret;
    }
    private static MdbPacket_HELM ReadMdbPacket_HELM(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_HELM();

      ret.Name = reader.ReadName32();
      ret.HelmHidingBehavior = reader.ReadUInt32();
      ret.Position = reader.ReadPoint3();
      ret.Rotation = reader.ReadRHMatrix();

      return ret;
    }
    private static MdbPacket_HAIR ReadMdbPacket_HAIR(this BinaryReader reader, UInt32 size)
    {
      var ret = new MdbPacket_HAIR();

      ret.Name = reader.ReadName32();
      ret.HairShorteningBehavior = reader.ReadUInt32();
      ret.Position = reader.ReadPoint3();
      ret.Rotation = reader.ReadRHMatrix();

      return ret;
    }

    public static MdbPacketHeader ReadMdbPacketHeader(this BinaryReader reader)
    {
      var ret = new MdbPacketHeader();

      ret.Signature = reader.ReadAscii(4);
      ret.Offset = reader.ReadUInt32();

      return ret;
    }

    public static MdbFileHeader ReadMdbFileHeader(this BinaryReader reader)
    {
      var ret = new MdbFileHeader();

      ret.Signature    = reader.ReadAscii(4);
      ret.MajorVersion = reader.ReadUInt16();
      ret.MinorVersion = reader.ReadUInt16();
      ret.NumPackets   = reader.ReadUInt32();

      return ret;
    }
  }

  public static class MDBPacket_WriterExtensions
  {
    public static void WriteAscii(this BinaryWriter writer, string ascii, int length)
    {
      byte[] toWrite = Encoding.ASCII.GetBytes(ascii);
      Array.Resize(ref toWrite, length);
      writer.Write(toWrite);
    }
    public static void Write(this BinaryWriter writer, Name32 name)
    {
      writer.WriteAscii(name.Contents, 32);
    }
    public static void Write(this BinaryWriter writer, Point3 point)
    {
      writer.Write(point.x);
      writer.Write(point.y);
      writer.Write(point.z);
    }
    public static void Write(this BinaryWriter writer, Color3 color)
    {
      writer.Write(color.r);
      writer.Write(color.g);
      writer.Write(color.b);
    }
    public static void Write(this BinaryWriter writer, Tri triangle)
    {
      writer.Write(triangle.p1);
      writer.Write(triangle.p2);
      writer.Write(triangle.p3);
    }
    public static void Write(this BinaryWriter writer, Quat quat)
    {
      writer.Write(quat.x);
      writer.Write(quat.y);
      writer.Write(quat.z);
      writer.Write(quat.w);
    }
    public static void Write(this BinaryWriter writer, RHMatrix mat)
    {
      writer.Write(mat.m11);
      writer.Write(mat.m12);
      writer.Write(mat.m13);
      writer.Write(mat.m21);
      writer.Write(mat.m22);
      writer.Write(mat.m23);
      writer.Write(mat.m31);
      writer.Write(mat.m32);
      writer.Write(mat.m33);
    }
    public static void Write(this BinaryWriter writer, Material mat)
    {
      writer.Write(mat.DiffuseMap);
      writer.Write(mat.NormalMap);
      writer.Write(mat.TintMap);
      writer.Write(mat.GlowMap);
      writer.Write(mat.Kd);
      writer.Write(mat.Ks);
      writer.Write(mat.SpecularPower);
      writer.Write(mat.SpecularValue);
      writer.Write(mat.TexFlags);
    }
    public static void Write(this BinaryWriter writer, RVert vert)
    {
      writer.Write(vert.Position);
      writer.Write(vert.Normal);
      writer.Write(vert.Tangent);
      writer.Write(vert.Binormal);
      writer.Write(vert.UVW);
    }
    public static void Write(this BinaryWriter writer, SVert vert)
    {
      writer.Write(vert.Position);
      writer.Write(vert.Normal);
      writer.Write(vert.BoneWeights);
      writer.Write(vert.BoneIndices);
      writer.Write(vert.Tangent);
      writer.Write(vert.Binormal);
      writer.Write(vert.UVW);
      writer.Write(vert.BoneCount);
    }
    public static void Write(this BinaryWriter writer, CVert vert)
    {
      writer.Write(vert.Position);
      writer.Write(vert.Normal);
      writer.Write(vert.UVW);
    }
    public static void Write(this BinaryWriter writer, CSphere sphere)
    {
      writer.Write(sphere.Origin);
      writer.Write(sphere.Radius);
    }
    public static void Write(this BinaryWriter writer, WTri triangle)
    {
      writer.Write(triangle.p1);
      writer.Write(triangle.p2);
      writer.Write(triangle.p3);
      writer.Write(triangle.flags);
    }

    public static void Write(this BinaryWriter writer, MdbPacket packet)
    {
      writer.WriteAscii(packet.Signature, 4);
      writer.Write(packet.Size);
      long mark1 = writer.BaseStream.Position;
      switch (packet.Signature)
      {
        case "RIGD":
          writer.WriteMdbPacket((MdbPacket_RIGD)packet);
          break;
        case "SKIN":
          writer.WriteMdbPacket((MdbPacket_SKIN)packet);
          break;
        case "COL2":
        case "COL3":
          writer.WriteMdbPacket((MdbPacket_COLN)packet);
          break;
        case "COLS":
          writer.WriteMdbPacket((MdbPacket_COLS)packet);
          break;
        case "WALK":
          writer.WriteMdbPacket((MdbPacket_WALK)packet);
          break;
        case "HOOK":
          writer.WriteMdbPacket((MdbPacket_HOOK)packet);
          break;
        case "TRRN":
          writer.WriteMdbPacket((MdbPacket_TRRN)packet);
          break;
        case "HELM":
          writer.WriteMdbPacket((MdbPacket_HELM)packet);
          break;
        case "HAIR":
          writer.WriteMdbPacket((MdbPacket_HAIR)packet);
          break;
        default:
          CTDebug.Error("Unrecognized object format: {0}", packet.Signature);
          break;
      }
      long mark2 = writer.BaseStream.Position;
      if(mark2 - mark1 != packet.Size)
      {
        CTDebug.Error("Incorrectly sized packet!");
      }
    }

    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_RIGD packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.Mat);
      writer.Write(packet.NumVerts);
      writer.Write(packet.NumFaces);
      foreach (var i in packet.Verts)
        writer.Write(i);
      foreach (var i in packet.Faces)
        writer.Write(i);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_SKIN packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.Skeleton);
      writer.Write(packet.Mat);
      writer.Write(packet.NumVerts);
      writer.Write(packet.NumFaces);
      foreach (var i in packet.Verts)
        writer.Write(i);
      foreach (var i in packet.Faces)
        writer.Write(i);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_COLN packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.Mat);
      writer.Write(packet.NumVerts);
      writer.Write(packet.NumFaces);
      foreach (var i in packet.Verts)
        writer.Write(i);
      foreach (var i in packet.Faces)
        writer.Write(i);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_COLS packet)
    {

      writer.Write(packet.NumItems);
      foreach (var i in packet.Items)
        writer.Write(i);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_WALK packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.uiFlags);
      writer.Write(packet.NumVerts);
      writer.Write(packet.NumFaces);
      foreach (var i in packet.Verts)
        writer.Write(i);
      foreach (var i in packet.Faces)
        writer.Write(i);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_HOOK packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.HookType);
      writer.Write(packet.HookSize);
      writer.Write(packet.Position);
      writer.Write(packet.RotationMatrix);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_TRRN packet)
    {
      writer.Write(packet.data);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_HELM packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.HelmHidingBehavior);
      writer.Write(packet.Position);
      writer.Write(packet.Rotation);
    }
    private static void WriteMdbPacket(this BinaryWriter writer, MdbPacket_HAIR packet)
    {
      writer.Write(packet.Name);
      writer.Write(packet.HairShorteningBehavior);
      writer.Write(packet.Position);
      writer.Write(packet.Rotation);
    }

    public static void Write(this BinaryWriter writer, MdbPacketHeader packet)
    {
      writer.WriteAscii(packet.Signature, 4);
      writer.Write(packet.Offset);
    }

    public static void Write(this BinaryWriter writer, MdbFileHeader header)
    {
      writer.WriteAscii(header.Signature, 4);
      writer.Write(header.MajorVersion);
      writer.Write(header.MinorVersion);
      writer.Write(header.NumPackets);
    }
  }

}
