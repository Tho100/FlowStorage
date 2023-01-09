using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace FlowSERVER1 {
    public static class EncryptionModel {
        //0123456789085746
        public static string Decrypt(String _InEnc, String _Key) {
            using (System.Security.Cryptography.RijndaelManaged rjm =
                        new System.Security.Cryptography.RijndaelManaged {
                            KeySize = 128,
                            BlockSize = 128,
                           // Padding = PaddingMode.Zeros,
                            Key = ASCIIEncoding.ASCII.GetBytes(_Key),
                            IV = ASCIIEncoding.ASCII.GetBytes(_Key)
                        }
            ) {
                byte[] input = Convert.FromBase64String(_InEnc);
                byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output);
            }
        }
        public static string Encrypt(String _InEnc, String _Key) {
            using (System.Security.Cryptography.RijndaelManaged rjm =
                        new System.Security.Cryptography.RijndaelManaged {
                            KeySize = 128,
                            BlockSize = 128,
                          //  Padding = PaddingMode.Zeros,
                            Key = ASCIIEncoding.ASCII.GetBytes(_Key),
                            IV = ASCIIEncoding.ASCII.GetBytes(_Key)
                        }
            ) {
                byte[] input = Encoding.UTF8.GetBytes(_InEnc);
                byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output);
            }
        }
    }
}
