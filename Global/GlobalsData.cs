using System.Collections.Generic;

namespace FlowstorageDesktop.Global {
    public class GlobalsData {

        static public List<string> base64EncodedImageSharedOthers = new List<string>();

        static public List<string> base64EncodedImageSharedToMe = new List<string>();

        static public List<string> base64EncodedImageFolder = new List<string>();

        static public Dictionary<string, List<(string, string, string)>> filesMetadataCacheHome = new Dictionary<string, List<(string, string, string)>>();
        static public List<string> base64EncodedImageHome = new List<string>();
        static public List<string> base64EncodedThumbnailHome = new List<string>();

        static public Dictionary<string, List<(string, string, string, string)>> filesMetadataCachePs = new Dictionary<string, List<(string, string, string, string)>>();
        static public List<string> base64EncodedImagePs = new List<string>();
        static public List<string> base64EncodedThumbnailPs = new List<string>();

    }
}