using FlowSERVER1.Global;
using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlowSERVER1.Helper {
    public class UpdateChanges {

        readonly private MySqlConnection con = ConnectionModel.con;

        public async Task SaveChangesUpdate(string fileName, string updatedValues, string tableName, bool isFromSharing, string directoryOrFolderName) {

            string encryptedFileName = EncryptionModel.Encrypt(fileName);

            byte[] toBytes = Convert.FromBase64String(updatedValues);
            string compressedUpdatedValues = Convert.ToBase64String(new GeneralCompressor().compressFileData(toBytes));
            string encryptUpdatedValues = EncryptionModel.Encrypt(compressedUpdatedValues);
            
            if (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName)) {
                string updateQue = $"UPDATE {tableName} SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.AddWithValue("@update", encryptUpdatedValues);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@filename", encryptedFileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.sharingTable && isFromSharing == false) {

                const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.AddWithValue("@update", encryptUpdatedValues);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@filename", encryptedFileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.sharingTable && isFromSharing == true) {

                const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.AddWithValue("@update", encryptUpdatedValues);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@filename", encryptedFileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.directoryUploadTable) {

                const string updateQue = "UPDATE upload_info_directory SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.AddWithValue("@update", encryptUpdatedValues);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@filename", encryptedFileName);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryOrFolderName));
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

    }
}
