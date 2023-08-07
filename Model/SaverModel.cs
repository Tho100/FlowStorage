using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using Guna.UI2.WinForms;
using Xamarin.Forms.Internals;

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
        private static void OpenSaveFileDialog(String fileName, Byte[] fileBytes) {

            CloseRetrievalAlert();

            var dialog = new SaveFileDialog {
                Filter = $"|*.{fileExtension}",
                FileName = fileName
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(dialog.FileName, fileBytes);
            }

        }

        public static async void SaveSelectedFile(String fileName, String tableName, String directoryName, bool isFromShared = false) {

            fileExtension = fileName.Split('.').Last();

            try {

                HashSet<string> filesName = new HashSet<string>(HomePage.instance.flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower())
                    .Where(text => text.Contains(".")));

                int indexOfImage = filesName.IndexOf(fileName.ToLower());

                if (Globals.imageTypesFolder.Contains(fileExtension) 
                && (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName))) {

                    string imageBase64Encoded = null;

                    if(GlobalsTable.publicTables.Contains(tableName)) {
                        imageBase64Encoded = GlobalsData.base64EncodedImageHome[indexOfImage];
                    } else if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                        imageBase64Encoded = GlobalsData.base64EncodedImagePs[indexOfImage];
                    }

                    var imageBytes = Convert.FromBase64String(imageBase64Encoded);

                    OpenSaveFileDialog(fileName, imageBytes);
                    return;
                }

                new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver")
                    .ShowDialog()).Start();

                if (tableName == GlobalsTable.directoryUploadTable) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));

                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();
                                
                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                OpenSaveFileDialog(fileName, getBytes);
                            }
                        }
                    }

                } else if (tableName == GlobalsTable.folderUploadTable) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(directoryName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                OpenSaveFileDialog(fileName, getBytes);
                            }
                        }
                    }

                } else if (GlobalsTable.publicTables.Contains(tableName)) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                OpenSaveFileDialog(fileName, getBytes);
                            }
                        }
                    }

                }

                else if (tableName == "cust_sharing" && isFromShared == true) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                OpenSaveFileDialog(fileName, getBytes);
                            }
                        }
                    }
                }
                
                else if (tableName == "cust_sharing" && isFromShared == false) {

                    const string selectFileDataQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {

                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);

                                OpenSaveFileDialog(fileName, getBytes);

                            }
                        }
                    }

                } else if (GlobalsTable.publicTablesPs.Contains(tableName)) {

                    string selectFileDataQuery = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                        using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                CloseRetrievalAlert();

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                string encryptedBase64 = reader.GetString(0);
                                string decryptedBase64 = EncryptionModel.Decrypt(encryptedBase64);
                                var getBytes = Convert.FromBase64String(decryptedBase64);
                                OpenSaveFileDialog(fileName, getBytes);
                            }
                        }
                    }
                }

                CloseRetrievalAlert();
            }

            catch (Exception) {
                CloseRetrievalAlert();
                new CustomAlert(title: "An error occurred","Failed to download this file.");
            }
        }

        private static void CloseRetrievalAlert() {
            Application.OpenForms
            .OfType<Form>()
            .Where(form => String.Equals(form.Name, "RetrievalAlert"))
            .ToList()
            .ForEach(form => form.Close());
        }
    }
}
