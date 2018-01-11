using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower._2da
{
  public class TableArtifact : CTArtifact
  {
    string m_tableName;

    public TableArtifact(CTArtifactType type, string name, string path)
      : base(type, name, path)
    {

    }

    public override void LoadManifest(string[] manifest)
    {
      m_tableName = manifest[2];
    }

    public override void Compile()
    {
      TableDocument doc = CTCore.GetOpenProject().GetDocument<TableDocument>(m_tableName);
      string[] output = new string[] {"Invalid table."};
      switch(ArtifactType)
      {
        case CTArtifactType.TableCT:
          break;
        case CTArtifactType.TableFriendly:
          output = doc.SaveFriendly();
          break;
        case CTArtifactType.TableRaw:
          output = doc.SaveCompiled();
          break;
      }

      System.IO.File.WriteAllLines(Path, output);
    }
  }
}
