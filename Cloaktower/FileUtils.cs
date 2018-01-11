using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower
{
  public static class FileUtils
  {
    public static string Header = "2DA V2.0";
    public static string[] Tokenize(string line)
    {
      if (line.Length == 0)
        return new string[0];
      int index = 0;
      bool isWhiteSpace = char.IsWhiteSpace(line[index]);
      bool isInQuotes = false;
      List<string> output = new List<string>();
      int beginString = 0;
      for(index = 0; index < line.Length; ++index)
      {
        if(line[index] == '"')
        {
          isInQuotes = !isInQuotes;
          if (isWhiteSpace)
          {
            isWhiteSpace = false;
            beginString = index;
          }
        }
        else if(!isInQuotes)
        {
          if (isWhiteSpace != char.IsWhiteSpace(line[index]))
          {
            if (isWhiteSpace)
            {
              beginString = index;
            }
            else
            {
              output.Add(line.Substring(beginString, index - beginString));
            }
            isWhiteSpace = !isWhiteSpace;
          }
        }
      }
      if(!isWhiteSpace)
      {
        output.Add(line.Substring(beginString, index - beginString));
      }

      return output.ToArray();
    }

    internal static int ParseInt(string input)
    {
      int output;
      if (int.TryParse(input, out output))
        return output;
      else
        return int.MinValue;
      //if (char.IsNumber(input[0]) || input[0] == '-')
      //  return int.Parse(input);
      //else
      //  return int.MinValue; // Hacky sentinel value for ****
    }
  }
}
