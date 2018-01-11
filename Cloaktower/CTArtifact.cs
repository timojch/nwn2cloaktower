using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public enum CTArtifactType
  {
    TableRaw,
    TableFriendly,
    TableCT,
    TLK,
    TextFile,
    ScriptSource,
    Hak
  }
  public abstract class CTArtifact
  {
    public CTArtifactType ArtifactType{get; set;}

    public string Path { get; set; }
    public string Name {get; set;}

    public static CTArtifact Create(string name, string[] manifest)
    {
      CTArtifactType type = (CTArtifactType)Enum.Parse(typeof(CTArtifactType), manifest[0]);
      string path = manifest[1];
      CTArtifact ret = null;
      switch(type)
      {
        case CTArtifactType.TableCT:
        case CTArtifactType.TableFriendly:
        case CTArtifactType.TableRaw:
          ret = new _2da.TableArtifact(type, name, path);
          ret.LoadManifest(manifest);
          break;
        case CTArtifactType.TextFile:
        case CTArtifactType.ScriptSource:
          break;
        case CTArtifactType.Hak:
          ret = new Hak.HakArtifact(name, path);
          ret.LoadManifest(manifest);
          break;
        case CTArtifactType.TLK:
          ret = new Tlk.TlkArtifact(name, path);
          ret.LoadManifest(manifest);
          break;
      }
      return ret;
    }

    protected CTArtifact(CTArtifactType type, string name, string path)
    {
      ArtifactType = type;
      Name = name;
      Path = path;
    }

    public abstract void Compile();

    public abstract void LoadManifest(string[] manifest);

    public string Extension
    {
      get
      {
        switch (ArtifactType)
        {
          case CTArtifactType.TableRaw: return "2da";
          case CTArtifactType.TableFriendly: return "2da";
          case CTArtifactType.TableCT: return "ct2da";
          case CTArtifactType.TLK: return "tlk";
          case CTArtifactType.TextFile: return "txt";
          case CTArtifactType.ScriptSource: return "nss";
          case CTArtifactType.Hak: return "hak";
          default: return "dat";
        }
      }
    }

    public Int16 ErfID
    {
      get
      {
        switch(ArtifactType)
        {
          case CTArtifactType.TableRaw: return 2017;
          default: return -1;
        }
      }
    }
  }
}
