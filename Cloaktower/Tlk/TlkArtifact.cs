using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Tlk
{
  public class TlkArtifact : CTArtifact
  {
    public TlkArtifact(string name, string path)
      :base(CTArtifactType.TLK, name, path)
    {
    }

    public override void LoadManifest(string[] manifest)
    {
      TlkName = manifest[2];
    }

    public override void Compile()
    {
      List<TlkContents> ret = new List<TlkContents>();
      List<TlkDocument> docStack = CTCore.GetOpenProject().GetTlkStack().GetDocuments();
      for(int i = 0; i < docStack.Count; ++i)
      {
        TlkDocument doc = docStack[i];
        CTDebug.Info("Writing from {0}.tlk ({1} lines)", doc.Name, doc.Contents.Length);
        ret.AddRange(doc.Contents);
      }

      try
      {
        TLKWriter.SaveFileContents(Path, ret.ToArray());
      }
      catch(Exception)
      {
        CTDebug.Error("Could not save {0}. Make sure the file is not in use.", Path);
      }
    }

    public string TlkName { get; set; }
  }
}
