using FlowSERVER1.Temporary;
using MySql.Data.MySqlClient;
using System;

namespace FlowSERVER1 {

    public class GetComment {

        readonly private static MySqlConnection con = ConnectionModel.con;
        readonly private static TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public static string getCommentSharedToMe(string fileName) {

            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName)); 
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        return EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }

            return String.Empty;
        }

        public static string getCommentSharedToOthers(string fileName) {

            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName)); 
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        return EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }

            return String.Empty;
        }

        public static string getCommentPublicStorage(string fileName) {

            using (MySqlCommand command = new MySqlCommand($"SELECT CUST_COMMENT FROM ps_info_comment WHERE CUST_FILE_NAME = @filename", con)) {
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        return EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }

            return String.Empty;
        }


    }
}