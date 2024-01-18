using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class DirectoryDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<List<(string, string, string)>> GetFileMetadata(string directoryName) {

            var filesInfo = new List<(string, string, string)>();

            const string query = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
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

        public async Task<List<string>> GetImage(string directoryName) {

            var base64EncodedImage = new List<string>();

            const string query = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        base64EncodedImage.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                    }
                }
            }

            return base64EncodedImage;

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
