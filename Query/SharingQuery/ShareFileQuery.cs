﻿using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FlowstorageDesktop.SharingQuery {
    public class ShareFileQuery {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string _fileName { get; set; }
        private string _fileExtension { get; set; }
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private string _receiverUsername{ get; set; }
        private string _comment { get; set; }
        private bool _isFromShared { get; set; }

        /// <summary>
        /// Verify if file is already shared
        /// </summary>
        /// <param name="_custUsername">Username of receiver</param>
        /// <param name="_fileName">File name to be send</param>
        /// <returns></returns>
        public async Task<int> FileIsUploadedVerification(string username, string fileName) {

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @sender AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";
            using (MySqlCommand command = new MySqlCommand(queryRetrieveCount, con)) {
                command.Parameters.AddWithValue("@sender", tempDataUser.Username);
                command.Parameters.AddWithValue("@receiver", username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }

        }

        /// <summary>
        /// This function will count the number of 
        /// files the user has been shared
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        public async Task<int> CountReceiverTotalShared(string username) {

            const string countFileSharedQuery = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @receiver";

            using (MySqlCommand command = new MySqlCommand(countFileSharedQuery, con)) {
                command.Parameters.AddWithValue("@receiver", username);

                object result = await command.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value) {
                    return Convert.ToInt32(result);
                }
            }

            return 0;

        }

        private async Task<string> GetFileMetadata(string fileName, string tableName) {

            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        private async Task<string> GetFileMetadataSharedToMe(string fileName) {

            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        private async Task<string> GetFileMetadataSharedToOthers(string fileName) {

            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        private async Task<string> GetFileMetadataExtra(string tableName, string columnName, string directoryName, string fileName) {

            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;
        }

        private async Task<string> RetrieveThumbnails(string tableName, string fileName) {

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        private async Task<string> RetrieveThumbnailsExtra(string tableName, string columnName, string directoryName, string fileName) {

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;

        }

        private async Task<string> RetrieveThumbnailShared(string fileName, string columnName) {

            string queryGetFileByte = $"SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = {columnName} AND CUST_FILE_PATH = @filename LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return string.Empty;
        }


        private async Task InsertFileData(string fileDataBase64, string thumbnailBase64 = null) {

            string encryptedFileName = EncryptionModel.Encrypt(_fileName);
            string encryptedComment = string.IsNullOrEmpty(_comment)
                ? string.Empty : EncryptionModel.Encrypt(_comment);

            string thumbnail = thumbnailBase64 ?? string.Empty;

            string todayDate = DateTime.Now.ToString("dd/MM/yyyy");

            const string insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,CUST_THUMB,CUST_COMMENT) VALUES (@receiver, @from, @file_name, @date, @file_data, @thumbnail, @comment)";

            using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@from", tempDataUser.Username);
                command.Parameters.AddWithValue("@receiver", _receiverUsername);
                command.Parameters.AddWithValue("@file_name", encryptedFileName);
                command.Parameters.AddWithValue("@date", todayDate);
                command.Parameters.AddWithValue("@file_data", fileDataBase64);
                command.Parameters.AddWithValue("@comment", encryptedComment);
                command.Parameters.AddWithValue("@thumbnail", thumbnail);

                await command.ExecuteNonQueryAsync();
            }

        }

        public async Task InitializeFileShare(string receiverUsername, string comment, string fileName, string tableName, string directoryName, bool isFromShared) {

            this._fileName = fileName;
            this._receiverUsername = receiverUsername;
            this._comment = comment;
            this._isFromShared = isFromShared;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._fileExtension = fileName.Split('.').Last();

            if (!_isFromShared && _tableName == GlobalsTable.sharingTable) {
                string getThumbnails = await RetrieveThumbnailShared(_fileName, "CUST_TO");
                await InsertFileData(await GetFileMetadataSharedToMe(_fileName), getThumbnails);

            } else if (_tableName != GlobalsTable.directoryUploadTable && _tableName != GlobalsTable.folderUploadTable) {

                if (Globals.imageTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psImage
                        ? GlobalsTable.psImage : GlobalsTable.homeImageTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (Globals.videoTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psVideo
                        ? GlobalsTable.psVideo : GlobalsTable.homeVideoTable;
                    string getThumbnails = await RetrieveThumbnails(finalTable, _fileName);
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable), getThumbnails);

                } else if (Globals.textTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psText
                        ? GlobalsTable.psText : GlobalsTable.homeTextTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (Globals.excelTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psExcel
                        ? GlobalsTable.psExcel : GlobalsTable.homeExcelTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (_fileExtension == "pdf") {
                    string finalTable = _tableName == GlobalsTable.psPdf
                        ? GlobalsTable.psPdf : GlobalsTable.homePdfTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psPtx
                        ? GlobalsTable.psPtx : GlobalsTable.homePtxTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (Globals.wordTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psWord
                        ? GlobalsTable.psWord : GlobalsTable.homeWordTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (Globals.audioTypes.Contains(_fileExtension)) {
                    string finalTable = _tableName == GlobalsTable.psAudio
                        ? GlobalsTable.psAudio : GlobalsTable.homeAudioTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (_fileExtension == "exe") {
                    string finalTable = _tableName == GlobalsTable.psExe
                        ? GlobalsTable.psExe : GlobalsTable.homeExeTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                } else if (_fileExtension == "apk") {
                    string finalTable = _tableName == GlobalsTable.psApk
                        ? GlobalsTable.psApk : GlobalsTable.homeApkTable;
                    await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                }

            } else if (_isFromShared && _tableName == GlobalsTable.sharingTable) {
                string getThumbnails = await RetrieveThumbnailShared(_fileName, "CUST_FROM");
                await InsertFileData(await GetFileMetadataSharedToOthers(_fileName), getThumbnails);

            } else if (_tableName == GlobalsTable.directoryUploadTable) {
                string getThumbnails = await RetrieveThumbnailsExtra(
                    GlobalsTable.directoryUploadTable, "DIR_NAME", _directoryName, _fileName);

                await InsertFileData(await GetFileMetadataExtra(
                    GlobalsTable.directoryUploadTable, "DIR_NAME", _directoryName, _fileName), getThumbnails);

            } else if (_tableName == GlobalsTable.folderUploadTable) {
                string getThumbnails = await RetrieveThumbnailsExtra(
                    GlobalsTable.folderUploadTable, "FOLDER_NAME", _directoryName, _fileName);

                await InsertFileData(await GetFileMetadataExtra(
                    GlobalsTable.folderUploadTable, "FOLDER_NAME", _directoryName, _fileName), getThumbnails);
            }

        }

    }
}
