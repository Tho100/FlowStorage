using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public class Globals {

        static public String custUsername = "";
        static public String custEmail = "";
        static public String accountType = "";

        static public readonly HashSet<string> imageTypes = new HashSet<string> 
        {".png",".jpg",".jpeg",".webp",".bmp",".ico"};

        static public readonly HashSet<string> textTypes = new HashSet<string>
        {".txt",".csv",".sql",".html",".css",".py",".xml",".js"};

        static public readonly HashSet<string> videoTypes = new HashSet<string> 
        {".mp4",".mov",".webm",".avi",".wmv"};

        static public readonly string[] publicTables = 
        { "file_info", "file_info_expand", "file_info_exe", "file_info_vid", 
          "file_info_excel", "file_info_msi", "file_info_audi", "file_info_apk", 
          "file_info_pdf", "file_info_word", "file_info_ptx", "file_info_gif", "file_info_directory" 
        };

        static public readonly Dictionary<string, string> tableToFileType = new Dictionary<string, string>
        {
            { "file_info", "imgFile" },
            { "file_info_expand", "txtFile" },
            { "file_info_exe", "exeFile" },
            { "file_info_vid", "vidFile" },
            { "file_info_excel", "exlFile" },
            { "file_info_pdf", "pdfFile" },
            { "file_info_apk", "apkFile" },
            { "file_info_word", "wordFile" },
            { "file_info_ptx", "ptxFile" },
            { "file_info_gif", "gifFile" },
            { "file_info_directory", null }
        };

        static public readonly Dictionary<string, int> uploadFileLimit = new Dictionary<string, int>
        {
            { "Basic", 25 },
            { "Max", 500 },
            { "Express", 1000 },
            { "Supreme", 2000 },
        };

        static public readonly Dictionary<string, int> uploadDirectoryLimit = new Dictionary<string, int>
        {
            { "Basic", 2 },
            { "Max", 2 },
            { "Express", 2},
            { "Supreme", 5},
        };

        static public readonly Dictionary<string, int> uploadFolderLimit = new Dictionary<string, int>
       {
            { "Basic", 3 },
            { "Max", 5 },
            { "Express", 10 },
            { "Supreme", 20 },
        };

    }
}
