using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1.Global {
    public class GlobalsData {

        static public List<string> fileTypeValuesSharedToOthers = new List<string>();
        static public List<string> fileTypeValuesSharedToMe = new List<string>();

        static public List<string> base64EncodedImageSharedOthers = new List<string>();
        static public List<string> base64EncodedThumbnailSharedOthers = new List<string>();

        static public List<string> base64EncodedThumbnailSharedToMe = new List<string>();
        static public List<string> base64EncodedImageSharedToMe = new List<string>();

        static public List<string> base64EncodedImageFolder = new List<string>();
        static public List<string> base64EncodedThumbnailFolder = new List<string>();

        static public List<string> filesNamesValuesHome = new List<string>();
        static public List<string> base64EncodedImageHome = new List<string>();
        static public List<string> base64EncodedThumbnailHome = new List<string>();

        static public List<string> base64EncodedImagePs = new List<string>();
        static public List<string> base64EncodedThumbnailPs = new List<string>();

    }
}
