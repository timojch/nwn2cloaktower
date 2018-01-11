using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloaktower.Tlk
{
  public struct TlkLanguage
  {
    public static Encoding DefaultEncoding
    {
      get { return Encoding.UTF8; }
    }

    public System.Text.Encoding Encoding { get; set; }
    public int LanguageCode;
    public string Version;

    public TlkLanguage(int code, string version)
      :this(code, version, Encoding.UTF8)
    {

    }
    
    public TlkLanguage(int code, string version, Encoding encoding)
      :this()
    {
      LanguageCode = code;
      Version = version;
      Encoding = encoding;
    }

    //public static TlkLanguage NWN2_English = new TlkLanguage(0, "NWN2", Encoding.UTF8);
    //public static TlkLanguage CurrentLanguage = NWN2_English;
    //
    //public static TlkLanguage NWN1_English  = new TlkLanguage(0, "NWN1", Encoding.GetEncoding(1252));
    //public static TlkLanguage NWN1_French   = new TlkLanguage(1, "NWN1", Encoding.GetEncoding(1252));
    //public static TlkLanguage NWN1_Italian  = new TlkLanguage(2, "NWN1", Encoding.GetEncoding(1252));
    //public static TlkLanguage NWN1_German   = new TlkLanguage(3, "NWN1", Encoding.GetEncoding(1252));
    //public static TlkLanguage NWN1_Spanish  = new TlkLanguage(4, "NWN1", Encoding.GetEncoding(1252));
    //public static TlkLanguage NWN1_Polish   = new TlkLanguage(5, "NWN1", Encoding.GetEncoding(1250));
    //public static TlkLanguage NWN1_Korean   = new TlkLanguage(128, "NWN1", Encoding.GetEncoding(949));
    //public static TlkLanguage NWN1_Chinese_Traditional = new TlkLanguage(129, "NWN1", Encoding.GetEncoding(959));
    //public static TlkLanguage NWN1_Chinese_Simplified = new TlkLanguage(130, "NWN1", Encoding.GetEncoding(936));
    //public static TlkLanguage NWN1_Japanese = new TlkLanguage(131, "NWN1", Encoding.GetEncoding(932));
    //
    //public static TlkLanguage NWN2_French   = new TlkLanguage(1, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Italian  = new TlkLanguage(2, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_German   = new TlkLanguage(3, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Spanish  = new TlkLanguage(4, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Polish   = new TlkLanguage(5, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Russian  = new TlkLanguage(6, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Korean   = new TlkLanguage(128, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Chinese_Traditional = new TlkLanguage(129, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Chinese_Simplified = new TlkLanguage(130, "NWN2", Encoding.UTF8);
    //public static TlkLanguage NWN2_Japanese  = new TlkLanguage(131, "NWN2", Encoding.UTF8);
  }
}
