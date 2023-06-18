using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using Microsoft.WindowsAPICodePack.Shell;
using FlowSERVER1.Sharing;
using FlowSERVER1.AlertForms;

namespace FlowSERVER1 {
    public partial class AskSharingAuthForm : Form {
        private String CustUsername {get; set; }
        private String _currentFileName {get; set; }
        private String _FileName { get; set; }
        private String _FilePath { get; set; }
        private String _retrieved { get; set; }

        readonly private MySqlConnection con = ConnectionModel.con;

        public AskSharingAuthForm(String _custUsername,String _fileName,String _fileExtension) {
            InitializeComponent();
            CustUsername = _custUsername;
            _currentFileName = _fileName;
            _FileName = _fileName;
            _retrieved = _fileExtension;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
            txtFieldShareToName.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            txtFieldShareToName.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// This function will retrieves user 
        /// account type 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int accountType(String _receiverUsername) {

            int allowedReturn = 20;

            const string getAccountTypeQuery = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(getAccountTypeQuery,con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        allowedReturn = Globals.uploadFileLimit[reader.GetString(0)];
                    }
                }
            }

            return allowedReturn;
        }

        /// <summary>
        /// This function will count the number of 
        /// files the user has been shared
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int countReceiverShared(String _receiverUsername) {

            const string countFileSharedQuery = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
            using(MySqlCommand command = new MySqlCommand(countFileSharedQuery,con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);
                return Convert.ToInt32(command.ExecuteScalar());
            }

        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>

        private async Task startSending(string encryptedFileValue, string thumbValue = "") {

            string varDate = DateTime.Now.ToString("dd/MM/yyyy");
            string encryptedFileName = EncryptionModel.Encrypt(_FileName);

            const string insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB)";
            using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@CUST_TO", CustUsername);
                command.Parameters.AddWithValue("@CUST_FROM", Globals.custUsername);
                command.Parameters.AddWithValue("@CUST_THUMB", thumbValue);
                command.Parameters.AddWithValue("@CUST_FILE_PATH", encryptedFileName);
                command.Parameters.AddWithValue("@FILE_EXT", _retrieved);
                command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                command.Parameters.AddWithValue("@CUST_FILE", encryptedFileValue);

                await command.ExecuteNonQueryAsync();
            }
        }

        String _controlName = null;
        private async Task startSharing() {

            int _accType = accountType(CustUsername);
            int _countReceiverFile = countReceiverShared(CustUsername);
            long fileSizeInMB = 0;

            if (_accType != _countReceiverFile) {

                _currentFileName = txtFieldShareToName.Text;

                byte[] _getBytes = File.ReadAllBytes(_FilePath);
                fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                new Thread(() => new SharingAlert(shareToName: CustUsername).ShowDialog()).Start();

                if (Globals.imageTypes.Contains(_retrieved)) {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".docx" || _retrieved == ".doc") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".pptx" || _retrieved == ".ppt") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".exe") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".mp3" || _retrieved == ".wav") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".pdf") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".apk") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (_retrieved == ".xlsx" || _retrieved == ".xls") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    await startSending(_toBase64);
                }
                else if (Globals.textTypes.Contains(_retrieved)) {

                    var nonLine = "";
                    using (StreamReader ReadFileTxt = new StreamReader(_FilePath)) { 
                        nonLine = ReadFileTxt.ReadToEnd();
                    }

                    var encryptValue = EncryptionModel.Encrypt(nonLine);
                    await startSending(encryptValue);
                }
                else if (Globals.videoTypes.Contains(_retrieved)) {

                    ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                    string toBase64StrThumb;
                    using (var stream = new MemoryStream()) {
                        toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        toBase64StrThumb = Convert.ToBase64String(stream.ToArray());
                    }

                    var _toBase64 = Convert.ToBase64String(File.ReadAllBytes(_FilePath));
                    await startSending(_toBase64,toBase64StrThumb);
                }

                CloseForm.closeForm("SharingAlert");
                new SucessSharedAlert(_FileName, CustUsername).Show();

                this.Close();
            }
            else {
                new CustomAlert(title: "Sharing failed",subheader: "The receiver has reached the limit amount of files they can received.").Show();
            }
        }

        private string getInformationSharing(string shareToName) {

            string _storeVal = "";
            const string _queryGet = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(_queryGet, con)) {
                command.Parameters.AddWithValue("@username", shareToName);

                using (MySqlDataReader _readPas = command.ExecuteReader()) {
                    while (_readPas.Read()) {
                        _storeVal = _readPas.GetString(0);
                    }
                }
            }

            return _storeVal;
        }
        /// <summary>
        /// This button will start sharing file if 
        /// the entered password is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            var _getInput = txtFieldShareToName.Text;
            var _decryptionOutput = EncryptionModel.computeAuthCase(getInformationSharing(CustUsername));
            
            if(EncryptionModel.computeAuthCase(_getInput) == _decryptionOutput) {
                await startSharing();
            } else {
                lblAlert.Visible = true;
            }

        }
    }
}
