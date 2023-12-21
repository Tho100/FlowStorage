using FlowSERVER1.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace FlowSERVER1.SharingQuery {
    public class SharingOptionsQuery {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        /// <summary>
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        public async Task<int> UserExistVerification(String receiverUsername) {

            int count = 0;

            const string countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(countUser, con)) {
                command.Parameters.AddWithValue("@username", receiverUsername);
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return count;

        }

        /// <summary>
        /// This function will delete user file sharing password 
        /// if they have one setup
        /// </summary>
        public async Task RemoveSharingAuth() {

            const string setPassSharingQuery = "UPDATE sharing_info SET SET_PASS = @setPass WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(setPassSharingQuery, con)) {
                command.Parameters.AddWithValue("@setPass", "DEF");
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                await command.ExecuteNonQueryAsync();
            }

        }

        /// <summary>
        /// Function to start disabling file sharing
        /// </summary>
        public async Task DisableSharing() {

            const string disableSharingQuery = "UPDATE sharing_info SET DISABLED = 1 WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(disableSharingQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                await command.ExecuteNonQueryAsync();
            }

        }

        /// <summary>
        /// Function to enable file sharing
        /// </summary>
        public async Task EnableSharing() {

            const string enabelSharingQuery = "UPDATE sharing_info SET DISABLED = 0 WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(enabelSharingQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                await command.ExecuteNonQueryAsync();
            }

        }

        /// <summary>
        /// This function will retrieve the 
        /// current status of user file sharing (disabled, or enabled)
        /// </summary>
        public async Task<string> RetrieveIsSharingDisabled(string username) {

            const string querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@username", username);

                string isEnabled = Convert.ToString(await command.ExecuteScalarAsync());
                string concludeOutput = isEnabled == "1" ? "1" : "0";
                return concludeOutput;
            }

        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        public async Task<string> ReceiverHasAuthVerification(String username) {

            string storeVal = "";

            const string queryGetSharingAuth = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(queryGetSharingAuth, con)) {
                command.Parameters.AddWithValue("@username", username);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        if (reader.GetString(0) == "DEF") {
                            storeVal = "";

                        }
                        else {
                            storeVal = reader.GetString(0);

                        }
                    }
                }
            }

            return storeVal;

        }

        public async Task<string> GetReceiverAuth(String receiverUsername) {

            const string query = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", receiverUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        /// <summary>
        /// Verify if file is already shared
        /// </summary>
        /// <param name="_custUsername">Username of receiver</param>
        /// <param name="_fileName">File name to be send</param>
        /// <returns></returns>
        public async Task<int> FileUploadCount(string receiverUsername, string fileName) {

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";

            using (MySqlCommand command = new MySqlCommand(queryRetrieveCount, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@receiver", receiverUsername);
                command.Parameters.AddWithValue("@filename", fileName);
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }

        }

    }
}
