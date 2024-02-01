using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {

    public class HomeDataCaller { 

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        /// <summary>
        /// 
        /// Get user Home files metadata: file name, upload date
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<(string, string, string)>> GetFileMetadata(string tableName) {

            if (GlobalsData.filesMetadataCacheHome.ContainsKey(tableName)) {
                return GlobalsData.filesMetadataCacheHome[tableName];

            } else {
                string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM {tableName} WHERE CUST_USERNAME = @username";
                using (var command = new MySqlCommand(selectFileData, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        var filesInfo = new List<(string, string, string)>();
                        while (await reader.ReadAsync()) {
                            string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);
                            filesInfo.Add((fileName, uploadDate, string.Empty));
                        }

                        GlobalsData.filesMetadataCacheHome[tableName] = filesInfo;
                        return filesInfo;
                    }
                }
            }

        }

        public async Task<List<string>> GetDirectories() {

            var directories = new List<string>();

            const string selectFileData = "SELECT DIR_NAME FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);

                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        directories.Add(fileName);
                    }
                }
            }

            return directories;

        }

        public async Task AddImageCaching() {

            const string retrieveImgQuery = "SELECT CUST_FILE FROM file_info_image WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string base64String = EncryptionModel.Decrypt(reader.GetString(0));
                        GlobalsData.base64EncodedImageHome.Add(base64String);
                    }
                }
            }

        }

        public async Task AddVideoThumbnailCaching() {

            const string retrieveImgQuery = "SELECT CUST_THUMB FROM file_info_video WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        GlobalsData.base64EncodedThumbnailHome.Add(reader.GetString(0));
                    }
                }
            }

        }

    }
}
