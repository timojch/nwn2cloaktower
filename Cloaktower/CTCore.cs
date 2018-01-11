using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cloaktower
{
  public static class CTCore
  {
    static Dictionary<string, string> s_commandLineArgs = new Dictionary<string, string>();
    static CTProject s_openProject;

    public static string GetArg(string argName, string defaultValue = "")
    {
      if (s_commandLineArgs.ContainsKey(argName))
        return s_commandLineArgs[argName];
      else
        return defaultValue;
    }
    public static void SetArgs(string[] args)
    {
      s_commandLineArgs.Clear();
      foreach(string arg in args)
      {
        string[] tok = arg.Split('=');
        if(tok.Length > 1)
          s_commandLineArgs.Add(tok[0], tok[1]);
      }
    }

    public static void OpenProject(string projPath)
    {
      if (Directory.Exists(projPath))
      {
        Directory.SetCurrentDirectory(projPath);
      }
      else if(projPath != "")
      {
        return;
      }

      string[] manifest = File.ReadAllLines("manifest.ctproj");
      s_openProject = new CTProject();
      s_openProject.ReadManifest(manifest);
    }

    public static void CreateProject()
    {
      s_openProject = new CTProject();
    }

    public static void SaveProject(string projPath)
    {

    }

    public static CTProject GetOpenProject()
    {
      return s_openProject;
    }
  }
}
