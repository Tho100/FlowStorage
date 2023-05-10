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
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {

    /// <summary>
    /// Download file class
    /// </summary>

    public partial class SaverModel {

        private static readonly MySqlConnection con = ConnectionModel.con;
        private static readonly MySqlCommand command = ConnectionModel.command;
        public static readonly SaverModel Instance = new SaverModel();
        public static bool stopFileRetrieval { get; set; } = false;
        private static string _getExt { get; set; }

        /// <summary>
        /// 
        /// Retrieve data (byte) from references and save them
        /// by opening SaveFileDialog
        /// 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_getBytes"></param>
        private static void _openDialog(String _FileTitle, Byte[] _getBytes) {

            var retrievalAlertForm = Application.OpenForms
                .OfType<Form>()
                .FirstOrDefault(form => form.Name == "RetrievalAlert");

            retrievalAlertForm?.Close();

            var dialog = new SaveFileDialog {
                Filter = $"|*.{_getExt}",
                FileName = _FileTitle
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(dialog.FileName, _getBytes);
            }

        }

        public static async void SaveSelectedFile(String _FileTitle, String _TableName, String _DirectoryName, bool _isFromShared = false) {

            _getExt = _FileTitle.Split('.').Last();

            try {

                List<String> _base64Encoded = new List<string>();

                if (_TableName == "upload_info_directory") {

                    var retrievalThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog());
                    retrievalThread.Start();

                    using (var command = new MySqlCommand("SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname", con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_DirectoryName));

                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            if (stopFileRetrieval) {

                                reader.Close();

                                Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());
                                
                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                var base64Encoded = reader.GetString(0);
                                var getBytes = Convert.FromBase64String(base64Encoded);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());

                } else if (_TableName == "folder_upload_info") {

                    var retrievalThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog());
                    retrievalThread.Start();

                    using (var command = new MySqlCommand($"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle", con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(_DirectoryName));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                var base64Encoded = reader.GetString(0);
                                var getBytes = Convert.FromBase64String(base64Encoded);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());

                } else if (_TableName != "folder_upload_info" && _TableName != "upload_info_directory" && _TableName != "cust_sharing") {

                    var retrievalThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog());
                    retrievalThread.Start();

                    using (var command = new MySqlCommand($"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename", con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                var base64Encoded = EncryptionModel.Decrypt(reader.GetString(0));
                                var getBytes = Convert.FromBase64String(base64Encoded);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                }

                else if (_TableName == "cust_sharing" && _isFromShared == true) {

                    var retrievalThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog());
                    retrievalThread.Start();

                    using (var command = new MySqlCommand("SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                var base64Encoded = reader.GetString(0);
                                var getBytes = Convert.FromBase64String(base64Encoded);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                }
                
                else if (_TableName == "cust_sharing") {

                    var retrievalThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.", "Saver").ShowDialog());
                    retrievalThread.Start();

                    using (var command = new MySqlCommand("SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileTitle));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {

                            if (stopFileRetrieval) {

                                reader.Close();

                                Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());

                                stopFileRetrieval = false;
                                return;
                            }

                            if (await reader.ReadAsync()) {
                                var base64Encoded = reader.GetString(0);
                                var getBytes = Convert.FromBase64String(base64Encoded);
                                _openDialog(_FileTitle, getBytes);
                            }
                        }
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                }
            }

            catch (Exception) {
                Application.OpenForms
                         .OfType<Form>()
                         .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                         .ToList()
                         .ForEach(form => form.Close());
            }
        }
    }
}
