using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace FlowSERVER1 {
    public class ConnectionModel {
        private static string getConnection = ConfigurationManager.ConnectionStrings["CONNECTIONSETUP"].ConnectionString;
        public static MySqlConnection con = new MySqlConnection(getConnection);
        public static MySqlCommand command;
        public static MySqlCommand commandRead;
    }
}