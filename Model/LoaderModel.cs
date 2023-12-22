using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace FlowstorageDesktop {

    public partial class LoaderModel {

        readonly private static MySqlConnection con = ConnectionModel.con;
        readonly private static TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public static byte[] universalBytes { get; set; }
        public static bool stopFileRetrievalLoad { get; set; } = false;
        private static string fileName { get; set; }
        private static string fileType { get; set; }

        public static Byte[] LoadFile(string tableName, string directoryName, string selectedFileName, bool isFromSharedFiles = false) {

            try {

                fileName = selectedFileName;
                fileType = $".{selectedFileName.Split('.').Last()}";

                if (GlobalsTable.publicTables.Contains(tableName)) {
                    RetrieveHomeDataAsync(tableName);

                } else if (tableName == GlobalsTable.directoryUploadTable) {
                    RetrieveDirectoryDataAsync(directoryName);
                 
                } else if (tableName == GlobalsTable.folderUploadTable) {
                    RetrieveFolderDataAsync(directoryName);
                    
                } else if (tableName == GlobalsTable.sharingTable && isFromSharedFiles == true) {
                    RetrieveSharedToOtherData();

                } else if (tableName == GlobalsTable.sharingTable) {
                    RetrieveSharedToMeData();

                } else if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                    RetrievePublicStorageData(tableName);

                }

            } catch (Exception) {
                new CustomAlert(title: "An error occurred","Failed to load this file. Are you connected to the internet?").Show();

            }

            return universalBytes;

        }

        private static async void RetrievePublicStorageData(string table) {

            string selectFileDataQuery = $"SELECT CUST_FILE FROM {table} WHERE CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;

                        }


                        var base64Encoded = reader.GetString(0);
                        var decryptValues = UniqueFile.IgnoreEncryption(fileType) 
                                          ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

        private static async void RetrieveSharedToOtherData() {

            const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;

                        }


                        var base64Encoded = reader.GetString(0);
                        var decryptValues = UniqueFile.IgnoreEncryption(fileType) 
                                          ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

        private static async void RetrieveFolderDataAsync(string directoryName) {

            const string selectFileDataQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath AND FOLDER_TITLE = @foldtitle";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(directoryName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;

                        }

                        var base64Encoded = reader.GetString(0);
                        var decryptValues = UniqueFile.IgnoreEncryption(fileType) 
                                          ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

        private static async void RetrieveDirectoryDataAsync(string directoryName) {

            const string selectFileDataQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath AND DIR_NAME = @dirname";

            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;
                            return;

                        }

                        var base64Encoded = reader.GetString(0);
                        var decryptValues = UniqueFile.IgnoreEncryption(fileType) 
                                          ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

        private static async void RetrieveSharedToMeData() {

            const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filepath";

            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;
                            return;
                        }

                        var base64Encoded = reader.GetString(0);
                        var decryptedValues = UniqueFile.IgnoreEncryption(fileType) 
                                            ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptedValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

        private static async void RetrieveHomeDataAsync(string tableName) {

            string selectFileDataQuery = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        ClosePopupForm.CloseRetrievalPopup();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            ClosePopupForm.CloseRetrievalPopup();
                            stopFileRetrievalLoad = false;

                        }

                        string base64Encoded = reader.GetString(0);
                        string decryptedValues = UniqueFile.IgnoreEncryption(fileType) 
                                                ? base64Encoded : EncryptionModel.Decrypt(base64Encoded);

                        byte[] compressedBytesData = Convert.FromBase64String(decryptedValues);
                        byte[] decompressedBytesData = new GeneralCompressor()
                                                    .decompressFileData(compressedBytesData);

                        universalBytes = decompressedBytesData;
                    }
                }
            }
        }

    }
}

