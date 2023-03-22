using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Windows;
using System.Data;
using System.IO;

namespace FlowSERVER1 {
    public class ConnectionModel {

        private static string getConnection = ConfigurationManager.ConnectionStrings["CONNECTIONSETUP"].ConnectionString;//DecryptConnection("0afe74-gksuwpe8r",ConfigurationManager.ConnectionStrings["CONNECTIONSETUP"].ConnectionString);
        public static MySqlConnection con = new MySqlConnection(getConnection);
            
        public static MySqlCommand command;
        public static MySqlCommand commandRead;

        private static string DecryptConnection(string key, string cipherText) {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}