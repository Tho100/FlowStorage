using System;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using FlowSERVER1.Helper;

namespace FlowSERVER1 {

    public partial class LoaderModel {

        readonly private static MySqlConnection con = ConnectionModel.con;
        public static byte[] universalBytes { get; set; }
        public static bool stopFileRetrievalLoad { get; set; } = false;
        private static string fileName { get; set; }
        private static string fileType { get; set; }

        public static Byte[] LoadFile(String _TableName, String _DirectoryName,String _FileName, bool _isFromSharedTo = false) {

            try {

                fileName = _FileName;
                fileType = $".{_FileName.Split('.').Last()}";

                if (GlobalsTable.publicTables.Contains(_TableName)) {
                    RetrieveHomeDataAsync(_TableName);
                }

                else if (_TableName == GlobalsTable.directoryUploadTable) {
                    RetrieveDirectoryDataAsync(_DirectoryName);
                 
                }

                else if (_TableName == GlobalsTable.folderUploadTable) {
                    RetrieveFolderDataAsync(_DirectoryName);
                    

                } else if (_TableName == GlobalsTable.sharingTable && _isFromSharedTo == true) {
                    RetrieveSharedTootherData();
                }

                else if (_TableName == GlobalsTable.sharingTable) {
                    RetrieveSharedToMeData();

                }
                else if (GlobalsTable.publicTablesPs.Contains(_TableName)) {
                    RetrievePublicStorageData(_TableName);

                }

            } catch (Exception) {
                new CustomAlert(title: "An error occurred","Failed to load this file. Are you connected to the internet?").Show();

            }

            return universalBytes;

        }

        private static async void RetrievePublicStorageData(String table) {

            string selectFileDataQuery = $"SELECT CUST_FILE FROM {table} WHERE CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        var retrievalAlertFormsOne = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                        retrievalAlertFormsOne.ForEach(form => form.Close());

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            var retrievalAlertForms = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                            retrievalAlertForms.ForEach(form => form.Close());
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

        private static async void RetrieveSharedTootherData() {

            const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        var retrievalAlertFormsOne = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                        retrievalAlertFormsOne.ForEach(form => form.Close());

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            var retrievalAlertForms = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                            retrievalAlertForms.ForEach(form => form.Close());
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(directoryName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {

                        var retrievalAlertFormsOne = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                        retrievalAlertFormsOne.ForEach(form => form.Close());

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            var retrievalAlertForms = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
                            retrievalAlertForms.ForEach(form => form.Close());
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        CloseRetrievalAlertForms();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            CloseRetrievalAlertForms();
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        CloseRetrievalAlertForms();

                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            CloseRetrievalAlertForms();
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

        private static async void RetrieveHomeDataAsync(String TableName) {

            string selectFileDataQuery = $"SELECT CUST_FILE FROM {TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
            using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filepath", EncryptionModel.Encrypt(fileName));

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        CloseRetrievalAlertForms();
                        if (stopFileRetrievalLoad) {
                            reader.Close();
                            CloseRetrievalAlertForms();
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

        private static void CloseRetrievalAlertForms() {
            var retrievalAlertForms = Application.OpenForms.OfType<Form>().Where(form => form.Name == "RetrievalAlert").ToList();
            retrievalAlertForms.ForEach(form => form.Close());
        }
    }
}

