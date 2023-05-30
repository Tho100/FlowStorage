using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowSERVER1 {
    public class Crud {

        private readonly MySqlConnection con = ConnectionModel.con;
        public async Task<int> countRow(String tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(CUST_USERNAME) FROM {tableName} WHERE CUST_USERNAME = @username";
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }
    }
}
