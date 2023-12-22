using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop {
    public class Crud {

        private readonly MySqlConnection con = ConnectionModel.con;
        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<int> CountTableRow(string tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

        public async Task<int> CountUserTableRow(string tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE CUST_USERNAME = @username";
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
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

    }
}
