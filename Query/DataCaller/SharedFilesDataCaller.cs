using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class SharedFilesDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<List<(string, string, string)>> GetFileMetadata() {

            List<(string, string, string)> filesInfo = new List<(string, string, string)>();

            const string query = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_FROM = @username";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate, string.Empty));
                    }
                }
            }

            return filesInfo;

        }

        public async Task<List<string>> GetSharedToUsername() {

            var uploadToNameList = new List<string>();

            const string selectUploadToName = "SELECT CUST_TO FROM cust_sharing WHERE CUST_FROM = @username";
            using (var command = new MySqlCommand(selectUploadToName, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        uploadToNameList.Add(reader.GetString(0));
                    }
                }
            }

            return uploadToNameList;

        }

        public async Task<List<string>> GetFileType() {

            var typeValues = new List<string>();

            const string query = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        typeValues.Add(reader.GetString(0));
                    }
                }
            }

            return typeValues;

        }

        public async Task AddFileTypeCaching() {

            const string query = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        GlobalsData.fileTypeValuesSharedToOthers.Add(reader.GetString(0));
                    }
                }
            }

        }

        public async Task AddImageCaching() {

            const string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
            using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        GlobalsData.base64EncodedImageSharedOthers.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                    }
                }
            }

        }

        public async Task AddVideoThumbnailCaching(string fileType) {

            const string query = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_FROM = @username AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@ext", fileType);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        GlobalsData.base64EncodedThumbnailSharedOthers.Add(reader.GetString(0));
                    }
                }
            }

        }

    }
}
