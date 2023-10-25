using System;
using System.Text;
using System.Security.Cryptography;
using System.Windows;

namespace FlowSERVER1 {
    public static class EncryptionModel {

        static private readonly byte[] _iv = new byte[16];
        static private readonly byte[] _keyBytes = Encoding.UTF8.GetBytes("9h3GKpL_vXeQsZ6F");

        public static string Encrypt(String _value) {

            try {

                byte[] plainBytes = Encoding.UTF8.GetBytes(_value); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = _keyBytes;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }

            } catch (Exception) { }

            return String.Empty;
        }

        public static string Decrypt(String _value) {

            try {
                
                byte[] encryptedBytes = Convert.FromBase64String(_value); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = _keyBytes;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            } catch (Exception) { }

            return String.Empty;

        }

        static public string computeAuthCase(string inputStr) {

            SHA256 sha256 = SHA256.Create();

            string getAuthStrCase0 = inputStr;
            byte[] getAuthBytesCase0 = Encoding.UTF8.GetBytes(getAuthStrCase0);
            byte[] authHashCase0 = sha256.ComputeHash(getAuthBytesCase0);
            string authStrCase0 = BitConverter.ToString(authHashCase0).Replace("-", "");

            return authStrCase0;
        }

    }
}
