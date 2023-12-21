using FlowSERVER1.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowSERVER1.AuthenticationQuery {
    public class UserAuthenticationQuery {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<Dictionary<string, string>> GetAccountAuthentication(string email) {

            var accountInfo = new Dictionary<string, string>();

            const string query = "SELECT CUST_PASSWORD, CUST_PIN FROM information WHERE CUST_EMAIL = @email";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@email", email);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        accountInfo["password"] = reader.GetString(0);
                        accountInfo["pin"] = reader.GetString(1);
                    }
                }
            }

            return accountInfo;
        }

        public async Task<int> GetUploadLimit(string username) {

            string accountType = "";
            
            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", username);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            return Globals.uploadFileLimit[accountType];

        }

        public async Task<string> GetAccountType(string username) {

            const string getAccountTypeQuery = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getAccountTypeQuery, con)) {
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        public async Task<string> GetUsernameByEmail(string email) {

            const string selectUsernameQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_EMAIL = @email";

            using (var command = new MySqlCommand(selectUsernameQuery, con)) {
                command.Parameters.AddWithValue("@email", email);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        public async Task<string> GetEmailByEmail(string email) {

            const string selectUsernameQuery = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";

            using (var command = new MySqlCommand(selectUsernameQuery, con)) {
                command.Parameters.AddWithValue("@email", email);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        public async Task<string> GetRecoveryKeyByEmail(string email) {

            const string query = "SELECT RECOV_TOK FROM information WHERE CUST_EMAIL = @email";

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@email", email);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        if(string.IsNullOrEmpty(reader.GetString(0))) {
                            return EncryptionModel.Decrypt(reader.GetString(0));
                        }
                    }
                }
            }

            return string.Empty;

        }

        public async Task<string> GetPassword() {

            const string query = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        public async Task<string> GetPIN() {

            const string query = "SELECT CUST_PIN FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

    }
}
