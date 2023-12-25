using System;
using System.IO;

namespace FlowstorageDesktop.Model {
    public class AutoLoginModel {

        readonly private string fileName = "CUST_DATAS.txt";
        readonly private string folderName = "FlowStorageInfos";

        /// <summary>
        /// Create file and insert user username into that file in a sub folder 
        /// called FlowStorageInfos located in %appdata%
        /// </summary>
        /// <param name="username">Username of user</param>
        public void SetupAutoLoginData(string username, string email) {

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{folderName}";

            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);

                using (StreamWriter _performWrite = File.CreateText(appDataPath + $"\\{fileName}")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(username));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(email));
                }

                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            } else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + $"\\{fileName}")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(username));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(email));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
        }

        public string ReadAutoLoginData() {

            string startupPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folderName);

            if (!Directory.Exists(startupPath)) {
                return string.Empty;
            }

            new DirectoryInfo(startupPath).Attributes &= ~FileAttributes.Hidden;

            string authFile = Path.Combine(startupPath, fileName);

            if (!File.Exists(authFile) || new FileInfo(authFile).Length == 0) {
                return string.Empty;
            }

            return authFile;

        }

    }
}
