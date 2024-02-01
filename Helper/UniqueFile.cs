using System.Collections.Generic;

namespace FlowstorageDesktop.Helper {
    public class UniqueFile {
        
        public static bool IgnoreEncryption(string fileType) {

            var fileToIgnore = new HashSet<string>();
            fileToIgnore.UnionWith(Globals.audioTypes);
            fileToIgnore.UnionWith(Globals.videoTypes);
            fileToIgnore.Add("exe");
            fileToIgnore.Add("msi");
            fileToIgnore.Add("apk");

            return fileToIgnore.Contains(fileType);

        }

    }
}
