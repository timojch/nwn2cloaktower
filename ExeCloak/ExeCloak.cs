using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  class ExeCloak
  {
    static void Main(string[] args)
    {
      //Hak.HakHeader read = Hak.HakHeader.Read("01_don_2da.hak");
      //List<Hak.HakKeyEntry> keys = new List<Hak.HakKeyEntry>();
      //List<Hak.HakResEntry> res = new List<Hak.HakResEntry>();
      //FileStream stream = File.OpenRead("01_don_2da.hak");
      //stream.Seek(read.OffsetToKeyList, SeekOrigin.Begin);
      //BinaryReader reader = new BinaryReader(stream);
      //for (int i = 0; i < read.EntryCount; ++i)
      //{
      //  keys.Add(Hak.HakKeyEntry.Read(reader));
      //}
      //for (int i = 0; i < read.EntryCount; ++i)
      //{
      //  res.Add(Hak.HakResEntry.Read(reader));
      //}

      CTCore.SetArgs(args);

      if (args.Length > 0)
      {
        CTCore.OpenProject(args[0]);
      }
      else
      {
        CTCore.OpenProject("");
      }
      if(CTCore.GetOpenProject() != null)
      {
        CTCore.GetOpenProject().CompileProject();
      }
      else
      {
        CTDebug.Error("Could not find manifest.ctproj.");
      }

      if(CTDebug.HasError)
      {
        Console.WriteLine("Cloaktower completed, with errors.");
        Console.WriteLine("Press any key to continue.");
        if(!Console.IsOutputRedirected)
        {
          Console.ReadKey();
        }
      }
    }
  }
}
