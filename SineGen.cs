using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public partial class SineGen {
        public static double phaseCnt = 1;     // Sample location in sine wave
        public static double sampleRate = 210; // Number samples in one second
        public static double freq = 1000;     // Desired frequency, Default set to 10Khz
        public static double sineWave() {
            double result = Math.Sin(freq * (4 * Math.PI) * (phaseCnt) / sampleRate);
            phaseCnt++;
            if (phaseCnt > sampleRate) phaseCnt = 0;
            return result;
        }
    }
}
