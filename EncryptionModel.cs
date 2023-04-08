using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public static class EncryptionModel {
        //0123456789085746
        private static readonly Random _random = new Random();
        private static string IsOriginFrom = "";
        private static string RandomString(int size, bool lowerCase = true) {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++) {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public static string Encrypt(String _value, String _key) {
            String toBase64 = "";

            try {

                byte[] iv = new byte[16]; 
                byte[] keyBytes = Encoding.UTF8.GetBytes("0123456789085746");
                byte[] plainBytes = Encoding.UTF8.GetBytes(_value); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        toBase64 = Convert.ToBase64String(encryptedBytes);
                    }
                }

            } catch (Exception) {
                // @
            }

            return toBase64;
        }

        public static string Decrypt(String _value, String _key) {

            String toBase64 = "";

            try {
                
                byte[] iv = new byte[16]; 
                byte[] keyBytes = Encoding.UTF8.GetBytes("0123456789085746"); 
                byte[] encryptedBytes = Convert.FromBase64String(_value); 

                using (Aes aes = Aes.Create()) {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        toBase64 = Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            } catch (Exception) {
                // @
            }

            return toBase64;

        }
         
        public static string EncryptText(String _value) {
            var _setupRandom = new Random();
            var _setupRandInt = _setupRandom.Next(0, 15);
            var _setupCustPs =  _setupRandInt + RandomString(15) + "ð " + _value + " ♀️" + _setupRandInt + RandomString(15);
            return _setupCustPs;
        }

        public static string DecryptText(String _value) {
            var _decrypted = "";
            try {
                var _setupCustDec = GetStringBetweenCharacters(_value, "ð ", " ♀️");
                var _retrieveDec = "" + _setupCustDec.Remove(0,1);
                _decrypted = _retrieveDec;
            } catch (Exception) {
                
            }
            return _decrypted;
        }

        private static string GetStringBetweenCharacters(string input, string charFrom, string charTo) {
            int posFrom = input.IndexOf(charFrom);
            if (posFrom != -1) 
            {
                int posTo = input.IndexOf(charTo, posFrom + 1);
                if (posTo != -1) 
                {
                    return input.Substring(posFrom + 1, posTo - posFrom - 1);
                }
            }

            return string.Empty;
        }

      
    }
}