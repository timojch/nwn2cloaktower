using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower.MDB
{
  public struct Name32
  {
    public string Contents;
    public override string ToString() { return Contents; }
  }

  public struct Point3
  {
    public float x, y, z;
    public override string ToString() { return string.Format("({0}, {1}, {2})", x, y, z); }
  }

  public struct Tri
  {
    public UInt16 p1, p2, p3;
    public override string ToString() { return string.Format("({0}, {1}, {2})", p1, p2, p3); }
  }

  public struct WTri
  {
    public UInt16 p1, p2, p3;
    public UInt32 flags;
    public override string ToString() { return string.Format("({0}, {1}, {2})", p1, p2, p3, flags); }
  }

  public struct Color3
  {
    public float r, g, b;
    public override string ToString() { return string.Format("({0}, {1}, {2})", r, g, b); }
  }

  public struct Quat
  {
    public float x, y, z, w;
    public override string ToString() { return string.Format("({0}, {1}, {2}, {3})", x, y, z, w); }
  }

  public struct RHMatrix
  {
    public float m11, m12, m13,
          m21, m22, m23,
          m31, m32, m33;
  }

  public struct Material
  {
    public Name32 DiffuseMap;
    public Name32 NormalMap;
    public Name32 TintMap;
    public Name32 GlowMap;
    public Color3 Kd;
    public Color3 Ks;
    public float SpecularPower;
    public float SpecularValue;
    public UInt32 TexFlags;
  }

  public struct RVert
  {
    public Point3 Position;
    public Point3 Normal;
    public Point3 Tangent;
    public Point3 Binormal;
    public Point3 UVW;
    public override string ToString() { return Position.ToString(); }
  }

  public struct SVert
  {
    public Point3 Position;
    public Point3 Normal;
    public Quat   BoneWeights;
    public byte[] BoneIndices;
    public Point3 Tangent;
    public Point3 Binormal;
    public Point3 UVW;
    public float  BoneCount;
    public override string ToString() { return Position.ToString(); }
  }

  public struct CVert
  {
    public Point3 Position;
    public Point3 Normal;
    public Point3 UVW;
    public override string ToString() { return Position.ToString(); }
  }

  public struct CSphere
  {
    public UInt32 Origin;
    public float Radius;
    public override string ToString() { return Origin.ToString(); }
  }
}
