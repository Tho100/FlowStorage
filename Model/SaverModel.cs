using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {

    /// <summary>
    /// Download file class
    /// </summary>

    public partial class SaverModel {

        private static readonly MySqlConnection con = ConnectionModel.con;
        public static readonly SaverModel Instance = new SaverModel();
        public static bool stopFileRetrieval { get; set; } = false;
        private static string fileExtension { get; set; }

        /// <summary>
        /// 
        /// Retrieve data (byte) from references and save them
        /// by opening SaveFileDialog
        /// 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_getBytes"></param>
        private static void _openDialog(String _FileTitle, Byte[] _getBytes) {

            closeRetrievalAlert();

            var dialog = new SaveFileDialog {
                Filter = $"|*.{fileExtension}",
                FileName = _FileTitle
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(dialog.FileName, _getBytes);
            }

        }

        public static async void SaveSelectedFile(String _FileTitle, String _TableName, String _DirectoryName, bool _isFromShared = false) {

            fileExtension = _FileTitle.Split('.').Last();

            try {

                List<String> _base64Encoded = new List<string>();

                new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog()).Start();

                if (_TableName == GlobalsTable.directoryUploadTable) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_DirectoryName));

                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();
                                
                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                } else if (_TableName == GlobalsTable.folderUploadTable) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(_DirectoryName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                } else if (GlobalsTable.publicTables.Contains(_TableName)) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                }

                else if (_TableName == "cust_sharing" && _isFromShared == true) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }
                }
                
                else if (_TableName == "cust_sharing" && _isFromShared == false) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {

                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);

                                _openDialog(_FileTitle, getBytes);

                            }
                        }
                    }

                } else if (GlobalsTable.publicTablesPs.Contains(_TableName)) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                closeRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }
                }

                closeRetrievalAlert();
            }

            catch (Exception) {
                new CustomAlert(title: "An error occurred","Failed to download this file.");
                closeRetrievalAlert();
            }
        }

        private static void closeRetrievalAlert() {
            Application.OpenForms
            .OfType<Form>()
            .Where(form => String.Equals(form.Name, "RetrievalAlert"))
            .ToList()
            .ForEach(form => form.Close());
        }
    }
}
