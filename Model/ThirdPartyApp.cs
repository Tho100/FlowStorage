using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Model {
    public class ThirdPartyApp {

        /*public void openThirdPartyApp() {
            using (MemoryStream ms = new MemoryStream(fileData)) {
                ProcessStartInfo psi = new ProcessStartInfo {
                    FileName = "WINWORD.EXE",
                    Arguments = "-",
                    UseShellExecute = false,
                    RedirectStandardInput = true
                };

                Process process = Process.Start(psi);

                if (process != null) {
                    // Write the file data to the standard input stream of Word
                    using (BinaryWriter bw = new BinaryWriter(process.StandardInput.BaseStream)) {
                        bw.Write(fileData);
                    }

                    process.WaitForExit();
                }
                else {
                    Console.WriteLine("Failed to start Microsoft Word process.");
                }
            }
        }*/

    }
}
