using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowSERVER1.AuthenticationQuery {
    public class StartupQuery {

        readonly private MySqlConnection con = ConnectionModel.con;

        /// <summary>
        /// 
        /// Retrieve user username based on the parameter 'username'
        /// which is retrieved from the cust_datas.txt. 
        /// 
        /// Usage; If returns String.Empty then show sign up panel else load data
        ///        based on the username
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> IsAccountExist(string username) {

            const string getUsername = "SELECT COUNT(CUST_USERNAME) FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getUsername, con)) {
                command.Parameters.AddWithValue("@username", username);
                int countUsername = Convert.ToInt32(await command.ExecuteScalarAsync());
                return countUsername > 0;
            }

        }

        public async Task<List<string>> GetFolders(string username) {

            var folders = new List<string>();

            const string getTitles = "SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getTitles, con)) {
                command.Parameters.AddWithValue("@username", username);
                using (MySqlDataReader fold_Reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await fold_Reader.ReadAsync()) {
                        folders.Add(EncryptionModel.Decrypt(fold_Reader.GetString(0)));
                    }
                }
            }

            return folders;

        }

    }
}
