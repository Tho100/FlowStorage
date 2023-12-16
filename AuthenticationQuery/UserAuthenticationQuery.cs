using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowSERVER1.AuthenticationQuery {
    public class UserAuthenticationQuery {

        readonly private MySqlConnection con = ConnectionModel.con;

        public async Task<Dictionary<string, string>> GetAccountAuthentication(string userEmail) {

            var accountInfo = new Dictionary<string, string>();

            string checkPasswordQuery = "SELECT CUST_PASSWORD, CUST_PIN FROM information WHERE CUST_EMAIL = @email";
            using (var command = new MySqlCommand(checkPasswordQuery, con)) {
                command.Parameters.AddWithValue("@email", userEmail);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        accountInfo["password"] = reader.GetString(0);
                        accountInfo["pin"] = reader.GetString(1);
                    }
                }
            }

            return accountInfo;
        }

        public async Task<string> GetUploadLimit() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            Globals.accountType = accountType;
            return Globals.uploadFileLimit[accountType].ToString();

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

            return String.Empty;

        }

    }
}
