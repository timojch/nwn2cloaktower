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
  class ArmorRenumberer
  {
    public static void Run(string[] args)
    {
      string inDir, outDir;
      int increment;
      inDir = "in";
      Console.WriteLine("Please enter how many lines to increment");
      increment = Int32.Parse(Console.ReadLine());
      //Console.WriteLine("Please enter the output directory");
      outDir = "out";

      foreach (string path in Directory.EnumerateFiles(inDir))
      {
        string fileName = path.Split('\\').Last();
        if (path.ToLowerInvariant().EndsWith(".mdb"))
        {
          string[] nameParts = fileName.Split('.')[0].Split('_');
          string newName;
          string newNumberPart;
          if (nameParts.Length != 4)
          {
            Cloaktower.CTDebug.Warn("Encountered bad filename: {0}", fileName);
            File.Copy(path, Path.Combine(outDir, fileName), true);
            continue;
          }
          else
          {
            string numberPart = nameParts[3];
            int numberIndex = numberPart.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }, 0);
            int number = int.Parse(numberPart.Substring(numberIndex, numberPart.Length-numberIndex)) + increment;
            newNumberPart = numberPart.Substring(0, numberIndex) + (number < 10?"0":"") + (number.ToString());

            nameParts[3] = newNumberPart;
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
              if (named.Name.Contents.ToLowerInvariant() == (filePrefix.ToLowerInvariant()))
              {
                string[] subNameParts = named.Name.Contents.Split('_');
                subNameParts[3] = newNumberPart;
                CTDebug.Info("Renaming {0} to {1}", named.Name.Contents, string.Join("_", subNameParts));
                named.Name.Contents = string.Join("_", subNameParts);
              }
            }
          }
          doc.Write(Path.Combine(outDir, newName + ".MDB"));
        }
        else
        {
          File.Copy(path, Path.Combine(outDir, fileName), true);
        }
      }
    }
  }
}
