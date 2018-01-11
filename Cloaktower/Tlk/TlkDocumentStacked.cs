using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Tlk
{
  public class TlkDocumentStacked : CTDocument
  {
    List<TlkDocument> m_subDocuments;

    public TlkDocumentStacked(string name)
      :base(CTCore.GetOpenProject(), name)
    {
      m_subDocuments = new List<TlkDocument>();
    }

    public void LoadCompiled()
    {
      foreach(var doc in m_subDocuments)
      {
        doc.LoadCompiled();
      }
    }

    public void AddTlkFile(TlkDocument doc)
    {
      if(doc != TlkDocument.DialogTlk)
        m_subDocuments.Add(doc);
    }

    public string GetEntry(int globalRow, out string subDoc, out int subDocRow)
    {
      if(globalRow < TlkDocument.UserTlkOffset)
      {
        subDoc = "tlk";
        subDocRow = globalRow;
        if (globalRow < TlkDocument.DialogTlk.Contents.Length)
          return TlkDocument.DialogTlk.Contents[globalRow].textContent;
        else
          return "";
      }
      else
      {
        globalRow -= TlkDocument.UserTlkOffset;
      }
      int docIndex = 0;
      TlkDocument doc;

      while(docIndex < m_subDocuments.Count)
      {
        doc = m_subDocuments[docIndex];
        if(globalRow < doc.Contents.Length)
        {
          subDocRow = globalRow;
          subDoc = doc.Name;
          return doc.Contents[subDocRow].textContent;
        }
        else
        {
          globalRow -= doc.Contents.Length;
          docIndex++;
        }
      }

      // If we got here, it's out of bounds.
      doc = m_subDocuments[m_subDocuments.Count];
      subDocRow = globalRow;
      subDoc = doc.Name;
      return "";
    }
  
    public int GetIndex(int localRow, string subDoc)
    {
      if(subDoc == "tlk")
      {
        return localRow;
      }
      else
      {
        int docZero = TlkDocument.UserTlkOffset;
        for (int docIndex = 0; docIndex < m_subDocuments.Count; ++docIndex)
        {
          TlkDocument doc = m_subDocuments[docIndex];
          if (doc.Name.ToLowerInvariant() == subDoc.ToLowerInvariant())
          {
            return localRow + docZero;
          }
          else
          {
            docZero += doc.Contents.Length;
          }
        }

        return localRow + docZero;
      }
    }

    public List<TlkDocument> GetDocuments()
    {
      return m_subDocuments;
    }
  }
}
