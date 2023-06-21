using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task<int> countRowPublicStorage(String tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(CUST_USERNAME) FROM {tableName}";
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

        public async Task Insert(string query, Dictionary<string, string> parameters) {

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.CommandText = query;

                foreach (var parameter in parameters) {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Select(string query, Dictionary<string, string> parameters) {

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.CommandText = query;

                foreach (var parameter in parameters) {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
