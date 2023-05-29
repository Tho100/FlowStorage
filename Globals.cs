using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public class Globals {

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

    }
}
