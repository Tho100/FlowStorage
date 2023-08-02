using FlowSERVER1.Global;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace FlowSERVER1.Helper {
    public class UpdateChanges {

        readonly private MySqlConnection con = ConnectionModel.con;

        public async Task SaveChangesUpdate(string fileName, string updatedValues, string tableName, bool isFromSharing, string directoryOrFolderName) {

            if (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName)) {
                string updateQue = $"UPDATE {tableName} SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.Add("@update", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(updatedValues);
                    command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                    command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(fileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.sharingTable && isFromSharing == false) {

                const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.Add("@update", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(updatedValues); ;
                    command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                    command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(fileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.sharingTable && isFromSharing == true) {

                const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.Add("@update", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(updatedValues); ;
                    command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                    command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(fileName);
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == GlobalsTable.directoryUploadTable) {

                const string updateQue = "UPDATE upload_info_directory SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                    command.Parameters.Add("@update", MySqlDbType.LongBlob).Value = EncryptionModel.Encrypt(updatedValues); ;
                    command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                    command.Parameters.Add("@dirname", MySqlDbType.Text).Value = EncryptionModel.Encrypt(directoryOrFolderName);
                    command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(fileName);
                    command.ExecuteNonQuery();
                }
            }

        }

    }
}
