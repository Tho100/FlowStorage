﻿using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query.DataCaller {
    public class SharedToMeDataCaller {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public async Task<List<(string, string, string)>> GetFileMetadata() {

            List<(string, string, string)> filesInfo = new List<(string, string, string)>();

            const string query = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_TO = @username";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate, string.Empty));
                    }
                }
            }

            return filesInfo;

        }

        /// <summary>
        /// Retrieve username of file that has been shared to
        /// </summary>
        /// <returns></returns>
        public async Task<string> SharedToMeUploaderName() {

            const string query = "SELECT CUST_FROM FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        public async Task AddImageCaching() {

            const string query = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        GlobalsData.base64EncodedImageSharedToMe.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                    }
                }
            }

        }

    }
}
