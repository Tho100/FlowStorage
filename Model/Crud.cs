using FlowSERVER1.Global;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public class Crud {

        private readonly MySqlConnection con = ConnectionModel.con;
        public async Task<int> CountTableRow(String tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

        public async Task<int> CountUserTableRow(String tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE CUST_USERNAME = @username";
                command.Parameters.AddWithValue("@username",Globals.custUsername);
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

        public async Task<string> ReturnUserAuth() {

            string authValue = null;

            const string getAuthQuery = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getAuthQuery, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader =  (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if(await reader.ReadAsync()) {
                        authValue = reader.GetString(0);
                    }
                }
            }

            return authValue;

        }

        public async Task<string> ReturnUserPIN() {

            string authValue = null;

            const string getAuthQuery = "SELECT CUST_PIN FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getAuthQuery, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        authValue = reader.GetString(0);
                    }
                }
            }

            return authValue;

        }

    }
}
