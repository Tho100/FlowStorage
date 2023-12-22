using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Helper {
    public class UpdateChanges {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private async Task UpdateData(string query, string updatedData, string fileName) {
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@update", updatedData);
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", fileName);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task SaveChangesUpdate(string fileName, string updatedValues, string tableName, bool isFromSharing, string directoryOrFolderName) {

            string encryptedFileName = EncryptionModel.Encrypt(fileName);

            byte[] toBytes = Convert.FromBase64String(updatedValues);
            string compressedUpdatedData = Convert.ToBase64String(new GeneralCompressor().compressFileData(toBytes));
            string encryptedUpdatedData = EncryptionModel.Encrypt(compressedUpdatedData);
            
            if (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName)) {
                string updateQuery = $"UPDATE {tableName} SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                await UpdateData(updateQuery, encryptedUpdatedData, encryptedFileName);

            } else if (tableName == GlobalsTable.sharingTable && isFromSharing == false) {
                const string updateQuery = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                await UpdateData(updateQuery, encryptedUpdatedData, encryptedFileName);

            } else if (tableName == GlobalsTable.sharingTable && isFromSharing == true) {
                const string updateQuery = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                await UpdateData(updateQuery, encryptedUpdatedData, encryptedFileName);

            } else if (tableName == GlobalsTable.directoryUploadTable) {
                const string updateQuery = "UPDATE upload_info_directory SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                using (MySqlCommand command = new MySqlCommand(updateQuery, con)) {
                    command.Parameters.AddWithValue("@update", encryptedUpdatedData);
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@filename", encryptedFileName);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryOrFolderName));
                    await command.ExecuteNonQueryAsync();
                }

            }

        }

    }
}
