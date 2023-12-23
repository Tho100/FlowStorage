using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FlowstorageDesktop.Query {
    public class DeleteFileDataQuery {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task DeleteFileData(string tableName, string fileName, string directoryName, string sharedToName = null) {

            string encryptedFileName = EncryptionModel.Encrypt(fileName);

            string encryptedDirectoryName = !string.IsNullOrEmpty(directoryName) 
                ? EncryptionModel.Encrypt(directoryName) : null;

            using (MySqlCommand command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                await command.ExecuteNonQueryAsync();
            }

            if (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName)) {

                string query = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                var param = new Dictionary<string, string>
                {
                    {"@username", tempDataUser.Username},
                    {"@filename", encryptedFileName},
                };

                await PerformDeletion(query, param);

                GlobalsData.filesMetadataCacheHome.Clear();

            } else if (tableName == GlobalsTable.directoryUploadTable) {

                const string query = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                var param = new Dictionary<string, string>
                {
                    {"@username", tempDataUser.Username},
                    {"@filename", encryptedFileName},
                    {"@dirname", encryptedDirectoryName}
                };

                await PerformDeletion(query, param);

            } else if (tableName == GlobalsTable.folderUploadTable) {

                const string query = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername";
                var param = new Dictionary<string, string>
                {
                    {"@username", tempDataUser.Username},
                    {"@filename", encryptedFileName},
                    {"@foldername", encryptedDirectoryName}
                };

                await PerformDeletion(query, param);

            } else if (tableName == GlobalsTable.sharingTable && sharedToName != "sharedToName") {

                const string query = "DELETE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                var param = new Dictionary<string, string>
                {
                    {"@username", tempDataUser.Username},
                    {"@filename", encryptedFileName},
                    {"@sharedname", sharedToName}
                };

                await PerformDeletion(query, param);

            } else if (tableName == GlobalsTable.sharingTable && sharedToName == "sharedToName") {

                const string query = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                var param = new Dictionary<string, string> 
                {
                    {"@username", tempDataUser.Username},
                    {"@filename", encryptedFileName}
                };

                await PerformDeletion(query, param);

            }

        }

        private async Task PerformDeletion(string query, Dictionary<string, string> parameters) {

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.CommandText = query;

                foreach (var parameter in parameters) {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
