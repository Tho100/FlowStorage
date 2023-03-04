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

namespace FlowSERVER1 {
    public class ConnectionModel {

        private static string getConnection = DecryptConnection(ConfigurationManager.ConnectionStrings["CONNECTIONSETUP"].ConnectionString, "abcde0152-connection");
        public static MySqlConnection con = new MySqlConnection(getConnection);
        public static MySqlCommand command;
        public static MySqlCommand commandRead;

        private static string DecryptConnection(string encrypt, string key) {
            using (TripleDESCryptoServiceProvider tripleDESCryptoService = new TripleDESCryptoServiceProvider()) {
                using (MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider()) {
                    byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tripleDESCryptoService.Key = byteHash;
                    tripleDESCryptoService.Mode = CipherMode.ECB;
                    byte[] data = Convert.FromBase64String(encrypt);
                    return Encoding.UTF8.GetString(tripleDESCryptoService.CreateDecryptor().TransformFinalBlock(data, 0, data.Length));
                }
            }
        }
    }
}