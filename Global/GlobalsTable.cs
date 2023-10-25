using System.Collections.Generic;

namespace FlowSERVER1.Global {
    public class GlobalsTable {

        static public readonly string homeImageTable = "file_info_image";
        static public readonly string homeVideoTable = "file_info_video";
        static public readonly string homeTextTable = "file_info_text";
        static public readonly string homePdfTable = "file_info_pdf";
        static public readonly string homeExcelTable = "file_info_excel";
        static public readonly string homePtxTable = "file_info_ptx";
        static public readonly string homeMsiTable = "file_info_msi";
        static public readonly string homeAudioTable = "file_info_audio";
        static public readonly string homeWordTable = "file_info_word";
        static public readonly string homeApkTable = "file_info_apk";
        static public readonly string homeExeTable = "file_info_exe";

        static public readonly string folderUploadTable = "folder_upload_info";
        static public readonly string sharingTable = "cust_sharing";
        static public readonly string directoryInfoTable = "file_info_directory";
        static public readonly string directoryUploadTable = "upload_info_directory";


        static public readonly Dictionary<string, string> tableToFileType = new Dictionary<string, string>
        {
            { homeImageTable, "imgFile" },
            { homeAudioTable, "audFile" },
            { homeTextTable, "txtFile" },
            { homeExeTable, "exeFile" },
            { homeVideoTable, "vidFile" },
            { homeExcelTable, "exlFile" },
            { homeMsiTable, "msiFile" },
            { homePdfTable, "pdfFile" },
            { homeApkTable, "apkFile" },
            { homeWordTable, "wordFile" },
            { homePtxTable, "ptxFile" },            
            { "file_info_directory", null }
        };

        static public readonly HashSet<string> publicTablesPs = new HashSet<string>
        { "ps_info_image", "ps_info_text", "ps_info_exe", "ps_info_video",
          "ps_info_excel", "ps_info_msi", "ps_info_audio", "ps_info_apk",
          "ps_info_pdf", "ps_info_word", "ps_info_ptx"
        };

        static public readonly HashSet<string> publicTables = new HashSet<string>
        { directoryInfoTable, homeImageTable, homeTextTable, homeExeTable, homeVideoTable,
          homeExcelTable, homeMsiTable, homeAudioTable, homeApkTable,
          homePdfTable, homeWordTable, homePtxTable
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
