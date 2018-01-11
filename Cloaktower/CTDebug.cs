using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public static class CTDebug
  {
    public static bool HasError = false;
    private static System.IO.TextWriter s_outputLog;

    public static void Error(string format, params object[] args)
    {
      ctlog(format, ConsoleColor.Red, "ERROR:   ", args);
      HasError = true;
    }
    public static void Error(object obj)
    {
      Error(obj.ToString());
    }

    public static void Warn(string format, params object[] args)
    {
      ctlog(format, ConsoleColor.Yellow, "WARNING: ", args);
    }
    public static void Warn(object obj)
    {
      Warn(obj.ToString());
    }
    public static void Info(string format, params object[] args)
    {
      ctlog(format, ConsoleColor.White, "INFO:    ", args);
    }
    public static void Info(object obj)
    {
      Info(obj.ToString());
    }

    private static void ctlog(string format, ConsoleColor color, string level, object[] args)
    {
      if (s_outputLog == null)
        s_outputLog = System.IO.File.CreateText("cloaktower.log");
      string message = string.Format(format, args);
      var storeColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
      Console.WriteLine("{0} {1}", level, message);
      s_outputLog.WriteLine("{0} {1}", level, message);
      s_outputLog.Flush();
      Console.ForegroundColor = storeColor;
    }
  }
}
