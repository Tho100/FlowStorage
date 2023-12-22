using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowstorageDesktop.AuthenticationQuery {
    public class SignUpQuery {

        readonly private MySqlConnection con = ConnectionModel.con;

        public async Task<Dictionary<string, int>> VerifyUsernameAndEmail(string username, string email) {

            var infos = new Dictionary<string, int>();

            using (MySqlCommand command = con.CreateCommand()) {
                const string verifyQueryUser = "SELECT COUNT(CUST_USERNAME) FROM information WHERE CUST_USERNAME = @username";
                command.CommandText = verifyQueryUser;
                command.Parameters.AddWithValue("@username", username);

                infos["username"] = Convert.ToInt32(await command.ExecuteScalarAsync());

                const string verifyQueryMail = "SELECT COUNT(CUST_EMAIL) FROM information WHERE CUST_EMAIL = @email";
                command.CommandText = verifyQueryMail;
                command.Parameters.AddWithValue("@email", email);

                infos["email"] = Convert.ToInt32(await command.ExecuteScalarAsync());

            }

            return infos;

        }

        public async Task InsertUserRegistrationData(string username, string email, string password, string pin) {

            var generateRecoveryToken = RandomString(16) + username;
            var recoveryToken = new string(
                generateRecoveryToken.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            string encryptedRecoveryToken = EncryptionModel.Encrypt(recoveryToken);

            string hashedPassword = EncryptionModel.computeAuthCase(password);
            string hashedPin = EncryptionModel.computeAuthCase(pin);

            string sharingDisabled = "0";
            string sharingPassword = "DEF";
            string sharingPasswordDisabled = "1";

            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");

            using (var transaction = await con.BeginTransactionAsync()) {

                try {

                    MySqlCommand command = con.CreateCommand();

                    command.CommandText = @"INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN,RECOV_TOK)
                            VALUES(@username, @password, @date, @email, @pin, @recovery_token)";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", hashedPassword);
                    command.Parameters.AddWithValue("@pin", hashedPin);
                    command.Parameters.AddWithValue("@date", todayDate);
                    command.Parameters.AddWithValue("@recovery_token", encryptedRecoveryToken);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE)
                            VALUES(@username, @email, @type)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@type", "Basic");

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"INSERT INTO sharing_info(CUST_USERNAME,DISABLED,SET_PASS,PASSWORD_DISABLED)
                            VALUES(@username, @sharing_disabled, @sharing_password, @password_disabled)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@sharing_disabled", sharingDisabled);
                    command.Parameters.AddWithValue("@sharing_password", sharingPassword);
                    command.Parameters.AddWithValue("@password_disabled", sharingPasswordDisabled);

                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();

                } catch (Exception) {
                    transaction.Rollback();

                }

            }
        }

        /// <summary>
        /// Generate random string within range
        /// </summary>
        private string RandomString(int size, bool lowerCase = true) {

            Random _setRandom = new Random();

            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++) {
                var @char = (char)_setRandom.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

    }
}
