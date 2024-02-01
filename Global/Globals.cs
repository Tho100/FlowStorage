using System.Collections.Generic;
using System.Drawing;

namespace FlowstorageDesktop {
    public class Globals {

        static public int PANEL_GAP_TOP = 275;
        static public int PANEL_GAP_HEIGHT = 100;

        static public readonly Image VerticalMenuImage = Properties.Resources.icons8_menu_vertical_30;
        static public readonly Image DirectoryGarbageImage = Properties.Resources.icons8_garbage_66__1_;
        static public readonly Image DIRIcon = Properties.Resources.DirIcon;

        static public readonly Image PDFImage = Properties.Resources.icons8_pdf_60__1_;
        static public readonly Image AudioImage = Properties.Resources.icons8_audio_file_60;
        static public readonly Image VideoImage = Properties.Resources.video_image;

        static public readonly Image DOCImage = Properties.Resources.icons8_microsoft_word_60;
        static public readonly Image PTXImage = Properties.Resources.icons8_microsoft_powerpoint_60;
        static public readonly Image APKImage = Properties.Resources.icons8_microsoft_powerpoint_60;

        static public readonly Image EXCELImage = Properties.Resources.excelIcon;
        static public readonly Image MSIImage = Properties.Resources.icons8_software_installer_32;
        static public readonly Image EXEImage = Properties.Resources.icons8_exe_48;

        static private readonly Image TextImage = Properties.Resources.icons8_txt_48;
        static private readonly Image CSVImage = Properties.Resources.icons8_csv_48;
        static private readonly Image CssImage = Properties.Resources.icons8_css_filetype_48__1_;
        static private readonly Image JsImage = Properties.Resources.icons8_javascript_50;
        static private readonly Image HTMLImage = Properties.Resources.icons8_html_filetype_48__1_;
        static private readonly Image SQLImage = Properties.Resources.icons8_database_50__1_;
        static private readonly Image PYImage = Properties.Resources.icons8_python_file_48;

        static public readonly HashSet<string> imageTypes = new HashSet<string> 
        {"png","jpg","jpeg"};

        static public readonly HashSet<string> textTypes = new HashSet<string>
        {"txt","csv","sql","html","css","py","xml","js","md"};

        static public readonly HashSet<string> videoTypes = new HashSet<string> 
        {"mp4","mov","avi","wmv"};

        static public readonly HashSet<string> audioTypes = new HashSet<string>
        {"mp3","wav"};

        static public readonly HashSet<string> wordTypes = new HashSet<string>
        {"doc","docx"};

        static public readonly HashSet<string> excelTypes = new HashSet<string>
        {"xls","xlsx"};

        static public readonly HashSet<string> ptxTypes = new HashSet<string>
        {"ppt","pptx"};


        static public readonly Dictionary<string, int> uploadFileLimit = new Dictionary<string, int>
        {
            { "Basic", 25 },
            { "Max", 150 },
            { "Express", 800 },
            { "Supreme", 2000 },
        };

        static public readonly Dictionary<int, string> uploadFileLimitToAccountType = new Dictionary<int, string>
        {
            { 25, "Basic" },
            { 150, "Max" },
            { 800, "Express" },
            { 2000, "Supreme" },
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
            { "txt", TextImage},
            { "md", TextImage},
            { "csv", CSVImage},
            { "js", JsImage},
            { "css", CssImage},
            { "html", HTMLImage},
            { "sql", SQLImage},
            { "py", PYImage},
            { "xml", TextImage},
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
            { "xml", TextImage},
        };

        static public readonly string filterFileType = "All Files|*.*|Image|*.jpg;*.jpeg;*.png;|Video|*.mp4;*.avi;.*mov;.*wmv;.*mkv|Text|*.txt;*.md|Excel|*.xlsx;*.xls|Powerpoint|*.pptx;*.ppt|Executable|*.exe|Audio Files|*.mp3;*.wav|Programming/Scripting|*.py;*.sql;*.js;|Markup|*.html;*.css;*.xml|Document|*.pdf;*.csv;*.docx";

    }
}
