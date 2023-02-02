using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public class ConnectionModel {
        public static string _SERVER = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
        public static string _MAINDB = "flowserver_db"; // epiz_33067528_information | flowserver_db
        public static string _USER = "0015connectionlover"; // epiz_33067528 | root
        public static string _PASSWORD = "nfreal-yt10";
        public static int _MAINPORT = 15272;
        public static string _FULLCONNECTION = "SERVER=" + _SERVER + ";" + "Port=" + _MAINPORT + ";" + "DATABASE=" + _MAINDB + ";" + "UID=" + _USER + ";" + "PASSWORD=" + _PASSWORD + ";";
        public static MySqlConnection con = new MySqlConnection(_FULLCONNECTION);
        public static MySqlCommand command;
        public static MySqlCommand commandRead;
    }
}