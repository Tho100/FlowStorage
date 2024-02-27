using System.Collections.Generic;
using System.Drawing;

namespace FlowstorageDesktop {
    public class Globals {

        static public int PANEL_GAP_TOP = 275;
        static public int PANEL_GAP_HEIGHT = 100;

        static public readonly Image VerticalMenuImage = Properties.Resources.more_options_icon;
        static public readonly Image DirectoryGarbageImage = Properties.Resources.delete_directory_icon;
        static public readonly Image DIRIcon = Properties.Resources.directory_icon;

        static public readonly Image PDFImage = Properties.Resources.pdf_icon;
        static public readonly Image AudioImage = Properties.Resources.audio_icon;
        static public readonly Image VideoImage = Properties.Resources.video_image;

        static public readonly Image DOCImage = Properties.Resources.doc_icon;
        static public readonly Image PTXImage = Properties.Resources.presentation_icon;
        static public readonly Image APKImage = Properties.Resources.apk_icon;

        static public readonly Image EXCELImage = Properties.Resources.excel_icon;
        static public readonly Image MSIImage = Properties.Resources.installer_icon;
        static public readonly Image EXEImage = Properties.Resources.exe_icon;

        static public readonly Image TextImage = Properties.Resources.text_icon;

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

        static public readonly string filterFileType = "All Files|*.*|Image|*.jpg;*.jpeg;*.png;|Video|*.mp4;*.avi;.*mov;.*wmv;.*mkv|Text|*.txt;*.md|Excel|*.xlsx;*.xls|Powerpoint|*.pptx;*.ppt|Executable|*.exe|Audio Files|*.mp3;*.wav|Programming/Scripting|*.py;*.sql;*.js;|Markup|*.html;*.css;*.xml|Document|*.pdf;*.csv;*.docx";

    }
}
