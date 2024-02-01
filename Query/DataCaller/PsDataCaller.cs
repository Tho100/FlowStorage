using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class PsDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();
        public async Task<List<(string, string, string, string)>> GetFileMetadata(string tableName) {

            if (GlobalsData.filesMetadataCachePs.ContainsKey(tableName)) {
                return GlobalsData.filesMetadataCachePs[tableName];

            } else {

                string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE, CUST_TAG, CUST_TITLE FROM {tableName}";
                using (var command = new MySqlCommand(selectFileData, con)) {
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        var filesInfo = new List<(string, string, string, string)>();
                        while (await reader.ReadAsync()) {
                            string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);
                            string tagValue = reader.GetString(2);
                            string titleValue = reader.GetString(3);
                            filesInfo.Add((fileName, uploadDate, tagValue, titleValue));
                        }

                        GlobalsData.filesMetadataCachePs[tableName] = filesInfo;
                        return filesInfo;
                    }
                }
            }

        }

        public async Task<List<(string, string, string, string)>> GetFileMetadataMyPs(string tableName) {

            string selectFileDataQuery = $"SELECT CUST_FILE_PATH, UPLOAD_DATE, CUST_TAG, CUST_TITLE FROM {tableName} WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    var filesInfo = new List<(string, string, string, string)>();
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        string tagValue = reader.GetString(2);
                        string titleValue = reader.GetString(3);
                        filesInfo.Add((fileName, uploadDate, tagValue, titleValue));
                    }
                    return filesInfo;
                }
            }

        }

        public async Task<List<string>> GetUploaderName(string tableName) {

            var usernameList = new List<string>();

            string selectUploaderNameQuery = $"SELECT CUST_USERNAME FROM {tableName}";
            using (var command = new MySqlCommand(selectUploaderNameQuery, con)) {
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        usernameList.Add(reader.GetString(0));
                    }
                }
            }

            return usernameList;

        }

        public async Task<List<string>> GetUploaderNameMyPs(string tableName) {

            var usernameList = new List<string>();

            string selectUploaderNameQuery = $"SELECT CUST_USERNAME FROM {tableName} WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(selectUploaderNameQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        usernameList.Add(reader.GetString(0));
                    }
                }
            }

            return usernameList;

        }

        public async Task AddImageCaching(bool isFromMyPs) {

            if (!isFromMyPs) {

                const string retrieveImagesQuery = "SELECT CUST_FILE FROM ps_info_image";
                using (var command = new MySqlCommand(retrieveImagesQuery, con)) {
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            string base64String = EncryptionModel.Decrypt(reader.GetString(0));
                            GlobalsData.base64EncodedImagePs.Add(base64String);
                        }
                    }
                }

            } else {

                const string retrieveImagesQuery = "SELECT CUST_FILE FROM ps_info_image WHERE CUST_USERNAME = @username";
                using (var command = new MySqlCommand(retrieveImagesQuery, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            string base64String = EncryptionModel.Decrypt(reader.GetString(0));
                            GlobalsData.base64EncodedImagePs.Add(base64String);
                        }
                    }
                }

            }

        }

        public async Task AddVideoThumbnailCaching(bool isFromMyPs) {

            if (!isFromMyPs) {

                const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM ps_info_video";
                using (var command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            GlobalsData.base64EncodedThumbnailPs.Add(reader.GetString(0));
                        }
                    }
                }

            } else {

                const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM ps_info_video WHERE CUST_USERNAME = @username";
                using (var command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            GlobalsData.base64EncodedThumbnailPs.Add(reader.GetString(0));
                        }
                    }
                }
            }

        }

    }
}
