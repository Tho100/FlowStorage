using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1.Helper {
    public class UpdateComment {

        readonly private MySqlConnection con = ConnectionModel.con;

        public async Task saveChangesComment(string updatedComment, string fileName) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", EncryptionModel.Encrypt(updatedComment));
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                await command.ExecuteNonQueryAsync();
            }

        }

    }
}
