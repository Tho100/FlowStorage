using MySql.Data.MySqlClient;

namespace FlowSERVER1 {

    public class GetComment {

        readonly private static MySqlConnection con = ConnectionModel.con;

        public static string getCommentSharedToMe(string fileName) {

            string returnComment = "";

            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName)); 
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        public static string getCommentSharedToOthers(string fileName) {

            string returnComment = "";

            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName)); 
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        public static string getCommentPublicStorage(string tableName, string fileName, string uploaderName) {

            string returnComment = "";

            using (MySqlCommand command = new MySqlCommand($"SELECT CUST_COMMENT FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", uploaderName);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }

            return returnComment;
        }


    }
}