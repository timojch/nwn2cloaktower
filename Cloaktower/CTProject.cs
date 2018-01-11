using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public class CTProject
  {
    Dictionary<string, CTDocument> m_openDocuments;
    Dictionary<CTResourceType, Dictionary<string, CTResource>> m_resources;
    List<CTArtifact> m_afx;
    Tlk.TlkDocumentStacked m_tlkStack;
    
    public CTProject()
    {
      m_openDocuments = new Dictionary<string, CTDocument>();
      m_resources = new Dictionary<CTResourceType, Dictionary<string, CTResource>>();
      m_afx = new List<CTArtifact>();

      m_resources.Add(CTResourceType.TableCT, new Dictionary<string, CTResource>());
      m_resources.Add(CTResourceType.TableRaw, new Dictionary<string, CTResource>());
      m_resources.Add(CTResourceType.TableFriendly, new Dictionary<string, CTResource>());
      m_resources.Add(CTResourceType.TableInfo, new Dictionary<string, CTResource>());
      m_resources.Add(CTResourceType.TextFile, new Dictionary<string, CTResource>());
      m_resources.Add(CTResourceType.TLK, new Dictionary<string, CTResource>());

      AddTlkStack(new Tlk.TlkDocumentStacked("custom"));
    }

    public Tlk.TlkDocumentStacked GetTlkStack()
    {
      return m_tlkStack;
    }

    public void ReadManifest(string[] manifest)
    {
      ManifestPhase phase = ManifestPhase.Header;
      
      for(int i = 0; i < manifest.Length; ++i)
      {
        string line = manifest[i].TrimStart();
        ManifestPhase parsePhase;
        if(Enum.TryParse<ManifestPhase>(line, out parsePhase))
        {
          CTDebug.Info(parsePhase);
          phase = parsePhase;
          continue;
        }
        else if(line.Length > 0)
        {
          string[] tok = line.Split(':');
          switch(phase)
          {
            case ManifestPhase.Header:
              break;
            case ManifestPhase.Resource:
              CTDebug.Info("Opening {0}", tok[1]);
              AddResource((CTResourceType)Enum.Parse(typeof(CTResourceType), tok[0]), tok[1]);
              break;
            case ManifestPhase.Document:
              if(tok.Length == 2)
                OpenDocument((CTDocument.DocumentType)Enum.Parse(typeof(CTDocument.DocumentType), tok[0]), tok[1]);
              else if(tok.Length == 3)
                OpenDocument((CTDocument.DocumentType)Enum.Parse(typeof(CTDocument.DocumentType), tok[0]), tok[1], tok[2]);
              break;
            case ManifestPhase.Artifact:
              this.AddArtifact(tok[0]);
              break;
          }
        }
      }

      foreach(CTDocument doc in m_openDocuments.Values)
        if(doc is _2da.TableDocument)
        {
          ((_2da.TableDocument)doc).ResolveReferences();
          if(((_2da.TableDocument)doc).CanSave)
            ((_2da.TableDocument)doc).SaveAll();
        }
    }

    private void AddArtifact(string name)
    {
      CTArtifact artifact = CTArtifact.Create(name, System.IO.File.ReadAllLines("Artifacts\\" + name + ".txt"));
      m_afx.Add(artifact);
      if(artifact.ArtifactType == CTArtifactType.TLK)
      {
        AddTlkStack(new Tlk.TlkDocumentStacked(((Tlk.TlkArtifact)artifact).TlkName));
      }
    }

    public void AddResource(CTResourceType toAdd, string name)
    {
      m_resources[toAdd].Add(name, new CTResource(name, toAdd));
    }

    public string GetResourcePath(CTResourceType type, string name, bool force = false, bool saveIfForced = true)
    {
      CTResource res = null;
      if(m_resources[type].ContainsKey(name))
        res = m_resources[type][name];
      else if(force)
      {
        if (saveIfForced)
        {
          AddResource(type, name);
          res = m_resources[type][name];
        }
        else
        {
          res = new CTResource(name, type);
        }
      }
      if (res != null)
        return res.Path;
      else throw new KeyNotFoundException();
    }

    public CTDocument GetDocument(string documentName)
    {
      if (m_openDocuments.ContainsKey(documentName))
      {
        return m_openDocuments[documentName];
      }
      else
      {
        return null;
      }
    }

    public T GetDocument<T>(string docName) where T : CTDocument
    {
      CTDocument rawDoc = GetDocument(docName);
      if (rawDoc != null)
        return (T)rawDoc;
      else
        return null;
    }

    public void CompileProject()
    {
      foreach(CTArtifact afx in m_afx)
      {
        afx.Compile();
      }
    }

    private void OpenDocument(CTDocument.DocumentType type, string name, string args = "")
    {
      if(name.ToLowerInvariant() == "all")
      {
        OpenAllDocuments(type, args);
        return;
      }

      switch(type)
      {
        case CTDocument.DocumentType.Table:
          {
            try
            {
              CTDebug.Info("Opening Table: {0}", name);
              _2da.TableDocument doc = new _2da.TableDocument(name);
              if (args == "raw")
                doc.LoadCompiled();
              else if (args == "friendly")
                doc.LoadFriendly();
              else
                doc.LoadNewest();
              m_openDocuments.Add(name, doc);
            }
            catch(Exception e)
            {
              CTDebug.Error("{0}: {1}", e.GetType().Name, e.Message);
            }
          }
          break;

        case CTDocument.DocumentType.TLK:
          if (name.ToLowerInvariant() != "dialog") // Don't open dialog.tlk as a document.
          {
            CTDebug.Info("Opening TLK: {0}", name);
            Tlk.TlkDocument doc = new Tlk.TlkDocument(name);
            m_openDocuments.Add(name, doc);
            m_tlkStack.AddTlkFile(doc);
            doc.LoadCompiled();
          }
          break;
      }
    }

    private void OpenAllDocuments(CTDocument.DocumentType type, string args)
    {
      switch(type)
      {
        case CTDocument.DocumentType.Table:
          {
            SortedSet<string> names = new SortedSet<string>();
            foreach (var file in System.IO.Directory.GetFiles("TableRaw"))
              if(!names.Contains(file))
                names.Add(file);
            foreach (var file in System.IO.Directory.GetFiles("TableFriendly"))
              if(!names.Contains(file))
                names.Add(file);

            foreach (var path in names)
            {
              string[] filenameDots = path.Split('\\')[1].Split('.');
              string name = string.Join(".", filenameDots, 0, filenameDots.Length - 1);
              if(GetDocument(name) == null)
                OpenDocument(CTDocument.DocumentType.Table, name, args);
            }
          }
          break;
        case CTDocument.DocumentType.TLK:
          {
            SortedSet<string> names = new SortedSet<string>();
            foreach (var file in System.IO.Directory.GetFiles("TLK"))
              if (!names.Contains(file))
                names.Add(file);

            foreach (var path in names)
            {
              string[] filenameDots = path.Split('\\')[1].Split('.');
              string name = string.Join(".", filenameDots, 0, filenameDots.Length - 1);
              if (GetDocument(name) == null)
                OpenDocument(CTDocument.DocumentType.TLK, name, args);
            }
          }
          break;
      }
    }

    public CTArtifact GetArtifact(string name)
    {
      foreach(var artifact in m_afx)
      {
        if (artifact.Name == name)
          return artifact;
      }
      return null;
    }
    
    public void SaveResource(CTResourceType type, string name, string[] value)
    {
      string path = GetResourcePath(type, name, true);
      if (System.IO.File.Exists(path))
        System.IO.File.Delete(path);
      System.IO.File.WriteAllLines(path, value);
    }

    internal CTResource[] GetAllResourcesOfType(CTResourceType resType)
    {
      List<CTResource> ret = new List<CTResource>();
      foreach (CTResource doc in m_resources[resType].Values)
        if (doc.ResourceType == resType)
          ret.Add(doc);

      return ret.ToArray();
    }
  
    private void AddTlkStack(Tlk.TlkDocumentStacked tlkStack)
    {
      m_tlkStack = tlkStack;
      foreach(var pair in m_openDocuments)
      {
        if(pair.Value.DocType == CTDocument.DocumentType.TLK)
        {
          var tlkDoc = (Tlk.TlkDocument)pair.Value;
          m_tlkStack.AddTlkFile(tlkDoc);
        }
      }
    }
  }


  enum ManifestPhase
  {
    Header,
    Resource,
    Document,
    Artifact
  }
}
