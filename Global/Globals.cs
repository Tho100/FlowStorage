using System.Collections.Generic;
using System.Drawing;

namespace FlowSERVER1 {
    public class Globals {

        static public int PANEL_GAP_TOP = 275;
        static public int PANEL_GAP_HEIGHT = 100;

        static public string sharingDisabledStatus = "";
        static public string sharingAuthStatus = "";

        static public string currentLanguage = "";

        static public string custUsername = "";
        static public string custEmail = "";
        static public string accountType = "";

        static public readonly Image VerticalMenuImage = FlowSERVER1.Properties.Resources.icons8_menu_vertical_30;
        static public readonly Image DirectoryGarbageImage = FlowSERVER1.Properties.Resources.icons8_garbage_66__1_;
        static public readonly Image DIRIcon = FlowSERVER1.Properties.Resources.DirIcon;

        static public readonly Image PDFImage = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
        static public readonly Image AudioImage = FlowSERVER1.Properties.Resources.icons8_audio_file_60;

        static public readonly Image DOCImage = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
        static public readonly Image PTXImage = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
        static public readonly Image APKImage = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;

        static public readonly Image EXCELImage = FlowSERVER1.Properties.Resources.excelIcon;
        static public readonly Image MSIImage = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
        static public readonly Image EXEImage = FlowSERVER1.Properties.Resources.icons8_exe_48;

        static private readonly Image TextImage = FlowSERVER1.Properties.Resources.icons8_txt_48;
        static private readonly Image CSVImage = FlowSERVER1.Properties.Resources.icons8_csv_48;
        static private readonly Image CssImage = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
        static private readonly Image JsImage = FlowSERVER1.Properties.Resources.icons8_javascript_50;
        static private readonly Image HTMLImage = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
        static private readonly Image SQLImage = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
        static private readonly Image PYImage = FlowSERVER1.Properties.Resources.icons8_python_file_48;

        static public readonly HashSet<string> imageTypes = new HashSet<string> 
        {".png",".jpg",".jpeg"};

        static public readonly HashSet<string> textTypes = new HashSet<string>
        {".txt",".csv",".sql",".html",".css",".py",".xml",".js",".md"};

        static public readonly HashSet<string> videoTypes = new HashSet<string> 
        {".mp4",".mov",".webm",".avi",".wmv"};

        static public readonly HashSet<string> audioTypes = new HashSet<string>
        {"mp3","wav"};

        static public readonly HashSet<string> imageTypesFolder = new HashSet<string>
        {"png","jpg","jpeg"};

        static public readonly HashSet<string> textTypesFolder = new HashSet<string>
        {"txt","csv","sql","html","css","py","xml","js","md"};

        static public readonly HashSet<string> videoTypesFolder = new HashSet<string>
        {"mp4","mov","webm","avi","wmv"};

        static public readonly HashSet<string> audioTypesFolder = new HashSet<string>
        {"mp3","wav"};

        static public readonly Dictionary<string, int> uploadFileLimit = new Dictionary<string, int>
        {
            { "Basic", 25 },
            { "Max", 150 },
            { "Express", 800 },
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

        static public readonly string filterFileType = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;|Video Files|*.mp4;*.webm;.mov;.wmv|Text Files|*.txt;*.md|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv";

    }
}
