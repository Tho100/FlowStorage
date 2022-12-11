using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public class ConnectionModel {
        public static string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
        public static string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
        public static string username = "root"; // epiz_33067528 | root
        public static string password = "nfreal-yt10";
        public static int mainPort_ = 11433;
        public static string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
        public static MySqlConnection con = new MySqlConnection(constring);
        public static MySqlCommand command;
        public static MySqlCommand commandRead;
    }
}
