using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1.Global {
    public class GlobalsTable {

        static public readonly string homeImageTable = "file_info";
        static public readonly string homeVideoTable = "file_info_vid";
        static public readonly string homeTextTable = "file_info_expand";
        static public readonly string homePdfTable = "file_info_pdf";
        static public readonly string homeExcelTable = "file_info_excel";
        static public readonly string homePtxTable = "file_info_ptx";
        static public readonly string homeAudioTable = "file_info_audi";
        static public readonly string homeWordTable = "file_info_word";
        static public readonly string homeApkTable = "file_info_apk";
        static public readonly string homeExeTable = "file_info_exe";

        static public readonly string folderUploadTable = "folder_upload_info";
        static public readonly string sharingTable = "cust_sharing";

        static public readonly Dictionary<string, string> tableToFileType = new Dictionary<string, string>
        {
            { "file_info", "imgFile" },
            { "file_info_audi", "audFile" },
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

        static public readonly HashSet<string> publicTablesPs = new HashSet<string>
        { "ps_info_image", "ps_info_text", "ps_info_exe", "ps_info_video",
          "ps_info_excel", "ps_info_msi", "ps_info_audio", "ps_info_apk",
          "ps_info_pdf", "ps_info_word", "ps_info_ptx"
        };

        static public readonly HashSet<string> publicTables = new HashSet<string>
        { "file_info", "file_info_expand", "file_info_exe", "file_info_vid",
          "file_info_excel", "file_info_msi", "file_info_audi", "file_info_apk",
          "file_info_pdf", "file_info_word", "file_info_ptx", "file_info_directory"
        };

        static public readonly Dictionary<string, string> tableToFileTypePs = new Dictionary<string, string>
        {
            { "ps_info_image", "imgFile" },
            { "ps_info_text", "txtFile" },
            { "ps_info_exe", "exeFile" },
            { "ps_info_video", "vidFile" },
            { "ps_info_excel", "exlFile" },
            { "ps_info_pdf", "pdfFile" },
            { "ps_info_apk", "apkFile" },
            { "ps_info_word", "wordFile" },
            { "ps_info_ptx", "ptxFile" },
            { "ps_info_audio", "audFile" },

        };
    }
}
