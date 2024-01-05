using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class DirectoryDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<List<(string, string, string)>> GetFileMetadata(string fileType, string directoryName) {

            var filesInfo = new List<(string, string, string)>();

            const string query = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                command.Parameters.AddWithValue("@ext", fileType);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate, string.Empty));
                    }
                }
            }

            return filesInfo;

        }

        public async Task<List<string>> GetImages(string fileType, string directoryName) {

            var base64EncodedImages = new List<string>();

            const string query = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                command.Parameters.AddWithValue("@ext", fileType);
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        base64EncodedImages.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                    }
                }
            }

            return base64EncodedImages;

        }

        public async Task<List<string>> GetVideoThumbnail(string fileType, string directoryName) {

            var base64EncodedThumbnails = new List<string>();

            const string query = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                command.Parameters.AddWithValue("@ext", fileType);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        base64EncodedThumbnails.Add(reader.GetString(0));
                    }
                }
            }

            return base64EncodedThumbnails;

        }

        public async Task<int> CountFilesInDirectory(string fileType, string directoryName) {

            string encryptedDirectoryName = EncryptionModel.Encrypt(directoryName);

            const string query = "SELECT COUNT(*) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", encryptedDirectoryName);
                command.Parameters.AddWithValue("@ext", fileType);

                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

        public async Task DeleteDirectory(string directoryName) {

            using (var command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new MySqlCommand("DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                await command.ExecuteNonQueryAsync();
            }

            using (var command = new MySqlCommand("DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                await command.ExecuteNonQueryAsync();
            }

        }

    }
}
