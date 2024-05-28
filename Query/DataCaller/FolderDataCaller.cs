using DocumentFormat.OpenXml.Spreadsheet;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class FolderDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<List<(string, string, string)>> GetFileMetadata(string folderName) {

            var filesInfo = new List<(string, string, string)>();
            var filePaths = new HashSet<string>();

            const string query = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_NAME = @foldname";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@foldname", EncryptionModel.Encrypt(folderName));
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {

                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);

                        if (filePaths.Contains(fileName)) {
                            continue;
                        }

                        filePaths.Add(fileName);
                        filesInfo.Add((fileName, uploadDate, string.Empty));
                    }
                }
            }

            return filesInfo;

        }

        public async Task<List<(string fileName, byte[] fileBytes)>> GetDownloadFolderData(string folderName) {

            var filesData = new List<(string fileName, byte[] fileBytes)>();

            const string query = "SELECT CUST_FILE_PATH, CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_NAME = @foldtitle";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@foldtitle", folderName);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        var fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        var base64Encoded = EncryptionModel.Decrypt(reader.GetString(1));
                        var fileBytes = Convert.FromBase64String(base64Encoded);
                        filesData.Add((fileName, fileBytes));
                    }
                }
            }

            return filesData;

        }

        public async Task AddImageCaching(string folderName) {

            const string query = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_NAME = @foldtitle";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(folderName));
                using (var readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readBase64.ReadAsync()) {
                        string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                        GlobalsData.base64EncodedImageFolder.Add(base64String);
                    }
                }
            }

        }

        public async Task DeleteFolder(string folderName) {
            
            const string query = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_NAME = @foldername";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(folderName));
                await command.ExecuteNonQueryAsync();
            }

        }

    }
}
