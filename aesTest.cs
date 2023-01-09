using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Globalization;

namespace FlowSERVER1 {
    public class aesTest {
        private static int _iterations = 2;
        private static int _keySize = 256;

        private static string _hash = "SHA1";
        private static string _salt = "aselrias38490a32"; // Random
        private static string _vector = "0123456789085746"; // Random

        public static string decrypter(string text,string  key) {
            using (System.Security.Cryptography.RijndaelManaged rjm =
                        new System.Security.Cryptography.RijndaelManaged {
                            KeySize = 128,
                            BlockSize = 128,
                            Key = ASCIIEncoding.ASCII.GetBytes(key),
                            IV = ASCIIEncoding.ASCII.GetBytes(key)
                        }
            ) {
                byte[] input = Convert.FromBase64String(text);
                byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output);
            }
        }

    }
}
