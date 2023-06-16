using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public class FileSize {
        public static double fileSize(byte[] getByte) {
            return getByte.Length / 1000000.0;
        }
    }
}
