using Cloaktower.MDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRecategorizer
{
  class Inspector
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
          MdbDocument doc = new Cloaktower.MDB.MdbDocument(path);
          doc.Read(path);
          doc.Write(Path.Combine(outDir, fileName));
        }
        else
        {
          File.Copy(path, Path.Combine(outDir, fileName), true);
        }
      }
    }
  }
}
