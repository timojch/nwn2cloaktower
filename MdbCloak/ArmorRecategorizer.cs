using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cloaktower.MDB;

namespace ModelTool
{
  class ArmorRecategorizer
  {
    static void Run(string[] args)
    {
      string inDir, outCat, outDir;
      Console.WriteLine("Please enter the input directory");
      inDir = Console.ReadLine();
      Console.WriteLine("Please enter the output category (or leave blank to choose per item)");
      outCat = Console.ReadLine();
      Console.WriteLine("Please enter the output directory");
      outDir = Console.ReadLine();

      foreach(string path in Directory.EnumerateFiles(inDir))
      {
        string fileName = path.Split('\\').Last();
        if(path.ToLowerInvariant().EndsWith(".mdb"))
        {
          string[] nameParts = fileName.Split('_');
          string newName;
          if (nameParts.Length != 4)
          {
            Cloaktower.CTDebug.Warn("Encountered bad filename: {0}", fileName);
            File.Copy(path, Path.Combine(outDir, fileName), true);
            continue;
          }
          else
          {
            nameParts[2] = outCat;
            newName = string.Join("_", nameParts);
          }
          MdbDocument doc = new Cloaktower.MDB.MdbDocument(path);
          doc.Read(path);

          string filePrefix = fileName.Split('.')[0];
          foreach(MdbPacket packet in doc.GetPackets())
          {
            if(packet is NamedMdbPacket)
            {
              NamedMdbPacket named = (NamedMdbPacket)packet;
              if(named.Name.Contents.ToLowerInvariant() == (filePrefix.ToLowerInvariant()))
              {
                string[] subNameParts = named.Name.Contents.Split('_');
                subNameParts[2] = outCat;
                named.Name.Contents = string.Join("_", subNameParts);
              }
            }
          }
          doc.Write(Path.Combine(outDir, newName));
        }
        else
        {
          File.Copy(path, Path.Combine(outDir, fileName), true);
        }
      }
    }
  }
}
