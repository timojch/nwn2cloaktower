using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.MDB
{
  public class MdbPacket
  {
    public string Signature;
    public UInt32 Size;
  }

  public class NamedMdbPacket : MdbPacket
  {
    public Name32 Name;

    public override string ToString()
    {
      return Name.Contents;
    }
  }

  public class MdbPacket_RIGD : NamedMdbPacket
  {
    public Material Mat;
    public UInt32   NumVerts;
    public UInt32   NumFaces;
    public RVert[]  Verts;
    public Tri[]    Faces;
  }
  public class MdbPacket_SKIN : NamedMdbPacket
  {
    public Name32 Skeleton;
    public Material Mat;
    public UInt32 NumVerts;
    public UInt32 NumFaces;
    public SVert[] Verts;
    public Tri[] Faces;
  }
  public class MdbPacket_COLN : NamedMdbPacket
  {
    public Material Mat;
    public UInt32 NumVerts;
    public UInt32 NumFaces;
    public CVert[] Verts;
    public Tri[] Faces;
  }
  public class MdbPacket_COLS : MdbPacket
  {
    public UInt32 NumItems;
    public CSphere[] Items;
  }
  public class MdbPacket_WALK : NamedMdbPacket
  {
    public UInt32 uiFlags;
    public UInt32 NumVerts;
    public UInt32 NumFaces;
    public Point3[] Verts;
    public WTri[] Faces;
  }
  public class MdbPacket_HOOK : NamedMdbPacket
  {
    public UInt16   HookType;
    public UInt16   HookSize;
    public Point3   Position;
    public RHMatrix RotationMatrix;
  }
  public class MdbPacket_TRRN : MdbPacket
  {
    public byte[] data;
  }
  public class MdbPacket_HELM : NamedMdbPacket
  {
    public UInt32    HelmHidingBehavior;
    public Point3    Position;
    public RHMatrix  Rotation;
  }
  public class MdbPacket_HAIR : NamedMdbPacket
  {
    public UInt32 HairShorteningBehavior;
    public Point3 Position;
    public RHMatrix Rotation;
  }

}
