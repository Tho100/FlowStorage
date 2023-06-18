using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Spire.Spreadsheet.Forms.Common.Win32;

namespace FlowSERVER1 {
    public class Globals {

        static private readonly Image TextImage = FlowSERVER1.Properties.Resources.icons8_txt_48;
        static private readonly Image CSVImage = FlowSERVER1.Properties.Resources.icons8_csv_48;
        static private readonly Image CssImage = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
        static private readonly Image JsImage = FlowSERVER1.Properties.Resources.icons8_javascript_50;
        static private readonly Image HTMLImage = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
        static private readonly Image SQLImage = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
        static private readonly Image PYImage = FlowSERVER1.Properties.Resources.icons8_python_file_48;

        static public String custUsername = "";
        static public String custEmail = "";
        static public String accountType = "";

        static public readonly HashSet<string> imageTypes = new HashSet<string> 
        {".png",".jpg",".jpeg",".webp",".bmp",".ico"};

        static public readonly HashSet<string> textTypes = new HashSet<string>
        {".txt",".csv",".sql",".html",".css",".py",".xml",".js",".md"};

        static public readonly HashSet<string> videoTypes = new HashSet<string> 
        {".mp4",".mov",".webm",".avi",".wmv"};

        static public readonly HashSet<string> imageTypesFolder = new HashSet<string>
        {"png","jpg","jpeg","webp","bmp","ico"};

        static public readonly HashSet<string> textTypesFolder = new HashSet<string>
        {"txt","csv","sql","html","css","py","xml","js","md"};

        static public readonly HashSet<string> videoTypesFolder = new HashSet<string>
        {"mp4","mov","webm","avi","wmv"};

        static public readonly HashSet<string> publicTables = new HashSet<string> 
        { "file_info", "file_info_expand", "file_info_exe", "file_info_vid", 
          "file_info_excel", "file_info_msi", "file_info_audi", "file_info_apk", 
          "file_info_pdf", "file_info_word", "file_info_ptx", "file_info_directory" 
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

        static public readonly Dictionary<string, Image> textTypeToImage = new Dictionary<string, Image> 
        {
            { ".txt", TextImage},
            { ".md", TextImage},
            { ".csv", CSVImage},
            { ".js", JsImage},
            { ".css", CssImage},
            { ".html", HTMLImage},
            { ".sql", SQLImage},
            { ".py", PYImage},

        };

        static public readonly Dictionary<string, Image> textTypeToImageFolder = new Dictionary<string, Image>
       {
            { "txt", TextImage},
            { "md", TextImage},
            { "csv", CSVImage},
            { "js", JsImage},
            { "css", CssImage},
            { "html", HTMLImage},
            { "sql", SQLImage},
            { "py", PYImage},
        };

    }
}
