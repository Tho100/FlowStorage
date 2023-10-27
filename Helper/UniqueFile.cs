using System;
using System.Collections.Generic;

namespace FlowSERVER1.Helper {
    public class UniqueFile {
        
        public static bool IgnoreEncryption(String fileType) {

            HashSet<String> fileToIgnore = new HashSet<string>();
            fileToIgnore.UnionWith(Globals.audioTypes);
            fileToIgnore.UnionWith(Globals.videoTypes);

            return fileToIgnore.Contains(fileType);

        }

    }
}
