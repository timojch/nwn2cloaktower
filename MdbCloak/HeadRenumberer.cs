using Cloaktower;
using Cloaktower.MDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRecategorizer
{
  class HeadRenumberer
  {
    public static void Run(string[] args)
    {
      string inDir = "in", outDir = "out";
      //Console.WriteLine("Please enter the input directory");
      //inDir = Console.ReadLine();
      //Console.WriteLine("Please enter the output directory");
      //outDir = Console.ReadLine();

      foreach (string path in Directory.EnumerateFiles(inDir))
      {
        string fileName = path.Split('\\').Last();
        if (path.ToLowerInvariant().EndsWith(".mdb"))
        {
          string[] nameParts = fileName.Split('.')[0].Split('_');
          string newName;
          int number;
          if (nameParts.Length != 3)
          {
            Cloaktower.CTDebug.Warn("Encountered bad filename: {0}", fileName);
            File.Copy(path, Path.Combine(outDir, fileName), true);
            continue;
          }
          else
          {
            number = int.Parse(nameParts[2].Substring(4, 2)) + 30;
            nameParts[2] = nameParts[2].Substring(0, 4) + (number).ToString();
            newName = string.Join("_", nameParts);
          }
          MdbDocument doc = new Cloaktower.MDB.MdbDocument(path);
          doc.Read(path);

          string filePrefix = fileName.Split('.')[0];
          foreach (MdbPacket packet in doc.GetPackets())
          {
            if (packet is NamedMdbPacket)
            {
              NamedMdbPacket named = (NamedMdbPacket)packet;
              if (named.Signature == "SKIN")
              {
                string[] split = named.Name.Contents.Split('_');
                split[2] = split[2].Substring(0, split[2].Length-2) + (number).ToString();
                string newInternalName = named.Name.Contents.Substring(0, named.Name.Contents.Length - 2) + (number).ToString();
                CTDebug.Info("Renaming {0} to {1}", named.Name.Contents, newInternalName);
                named.Name.Contents = newInternalName;
                if(split[2].Substring(0,4) == "Head" && split.Length == 3)
                  newName = newInternalName;
              }
              else if(named.Signature == "RIGD")
              {
                CTDebug.Warn("Found a RIGD in {0}", fileName);
              }
            }
          }
          doc.Write(Path.Combine(outDir, newName + ".mdb"));
        }
        else
        {
          File.Copy(path, Path.Combine(outDir, fileName), true);
        }
      }
    }
  }
}
