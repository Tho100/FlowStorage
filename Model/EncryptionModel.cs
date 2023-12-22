using System;
using System.Text;
using System.Security.Cryptography;

namespace FlowstorageDesktop {
    public static class EncryptionModel {

        private static readonly byte[] keyByte = Encoding.UTF8.GetBytes("Rw2345_789qTz345");

        public static string Encrypt(string value) {

            try {

                byte[] plainBytes = Encoding.UTF8.GetBytes(value); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = keyByte;
                    aes.IV = keyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }

            } catch (Exception) { }

            return string.Empty;

        }

        public static string Decrypt(string encryptedValue) {

            try {

                byte[] iv = Encoding.UTF8.GetBytes("Rw2345_789qTz345");
                byte[] keyBytes = Encoding.UTF8.GetBytes("Rw2345_789qTz345");

                byte[] encryptedBytes = Convert.FromBase64String(encryptedValue); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = keyByte;
                    aes.IV = keyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }

            } catch (Exception) { }

            return string.Empty;

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
