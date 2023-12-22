using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FlowstorageDesktop {
    public class ConnectionModel {

        private static string getConnection = ConfigurationManager.ConnectionStrings["djkioJ33AW=KDOdsad"].ConnectionString;//DecryptConnection("0afe74-gksuwpe8r",ConfigurationManager.ConnectionStrings["CONNECTIONSETUP"].ConnectionString);
        public static MySqlConnection con = new MySqlConnection(getConnection);

        //public static MySqlCommand command;

        private static string DecryptConnection(string key, string cipherText) {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}