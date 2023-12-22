using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.AuthenticationQuery {
    public class StartupQuery {

        readonly private MySqlConnection con = ConnectionModel.con;

        /// <summary>
        /// 
        /// Get user folders name
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
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
