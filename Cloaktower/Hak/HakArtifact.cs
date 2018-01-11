using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Hak
{
  public class HakArtifact : CTArtifact
  {
    Dictionary<string, CTArtifact> m_afx;
    static Encoding coder = Encoding.UTF8;

    public HakArtifact(string name, string path)
      :base(CTArtifactType.Hak, name, path)
    {
      m_afx = new Dictionary<string, CTArtifact>();
    }

    public override void Compile()
    {
      HakHeader header = new HakHeader();
      List<HakKeyEntry> keys = new List<HakKeyEntry>();
      List<HakResEntry> resEntries = new List<HakResEntry>();
      List<Byte[]> resources = new List<byte[]>();

      foreach(KeyValuePair<string, CTArtifact> pair in m_afx)
      {
        if (!File.Exists(pair.Value.Path))
          continue;

        HakKeyEntry key = new HakKeyEntry();
        key.ResID = keys.Count;
        key.Resref = pair.Key;
        key.ResType = pair.Value.ErfID;
        key.Reserved = 0;
        keys.Add(key);

        byte[] resVal = File.ReadAllBytes(pair.Value.Path);
        resources.Add(resVal);

        HakResEntry res = new HakResEntry();
        res.ResourceSize = resVal.Length;
        resEntries.Add(res);

        
      }

      try
      {
        if (File.Exists(Path))
          File.Delete(Path);
        BinaryWriter writer = new BinaryWriter(File.OpenWrite(Path));

        header.Header = "HAK V1.1";
        header.StringTableSize = 0;
        header.StringCount = 0;
        header.EntryCount = keys.Count;
        header.OffsetToString = HakHeader.Size;
        header.OffsetToKeyList = HakHeader.Size;
        header.OffsetToResources = HakHeader.Size + (keys.Count * HakKeyEntry.Size);
        header.BuildYear = DateTime.Now.Year - 1900;
        header.BuildDay = DateTime.Now.Day;
        header.DescriptionIndex = 0;

        header.Write(writer);
        foreach (var key in keys)
        {
          key.Write(writer);
        }

        int ResourceOffset = header.OffsetToResources + (resEntries.Count * HakResEntry.Size);
        for (int i = 0; i < resEntries.Count; ++i)
        {
          resEntries[i].OffsetToResource = ResourceOffset;
          resEntries[i].Write(writer);
          ResourceOffset += resEntries[i].ResourceSize;
        }

        for (int i = 0; i < resources.Count; ++i)
        {
          writer.Write(resources[i]);
        }

        writer.Flush();
      }
      catch(Exception)
      {
        CTDebug.Error("Could not save {0}. Make sure the file is not in use.", Path);
      }
    }

    public override void LoadManifest(string[] manifest)
    {
      for (int i = 2; i < manifest.Length; ++i )
      {
        string name = manifest[i];
        ReadManifestLine(name);
      }
    }

    private void ReadManifestLine(string input)
    {
      string[] tokens = FileUtils.Tokenize(input); 
      switch (tokens[1].ToLowerInvariant())
      {
        case "2da":
          tokens[0] = tokens[0].ToLowerInvariant();
          foreach(var doc in _2da.TableDocument.OpenTables)
            if(tokens[0] == "all" || tokens[0] == doc.Value.Category)
            {
              string name = doc.Key;
              string path = CTCore.GetOpenProject().GetResourcePath(CTResourceType.TableRaw, name);
              CTArtifact afx = new HakComponentArtifact(CTArtifactType.TableRaw, name, path);

              m_afx.Add(name, afx);
            }
          break;
      }
    }

    private class HakComponentArtifact : CTArtifact
    {
      private CTArtifactType cTArtifactType;

      public HakComponentArtifact(CTArtifactType cTArtifactType, string name, string path)
        : base(cTArtifactType, name, path)
      {
        // TODO: Complete member initialization
        this.cTArtifactType = cTArtifactType;
        this.Name = name;
        this.Path = path;
      }


      public override void Compile()
      {
        
      }

      public override void LoadManifest(string[] manifest)
      {
        
      }
    }
  }
}
