using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Diagnostics;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;
using FlowSERVER1.Sharing;
using FlowSERVER1.AlertForms;

namespace FlowSERVER1 {
    public partial class MainSharingForm : Form {

        private string _FileName;
        private string _FilePath;
        private string _retrieved;
        private string _getExt;

        readonly private MySqlConnection con = ConnectionModel.con;

        public MainSharingForm instance;

        public string _verifySetPas = "";

        public MainSharingForm() {
            InitializeComponent();
            instance = this;
        }

        private void sharingFORM_Load(object sender, EventArgs e) {

            
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int userIsExists(String _receiverUsername) {

            const string countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(countUser,con)) {
                command.Parameters.AddWithValue("@username",_receiverUsername);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            OpenFileDialog _OpenDialog = new OpenFileDialog();
            _OpenDialog.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp;|Video Files|*.mp4;*.webm;.mov;.wmv|Text Files|*.txt;|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv";
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                var getEx = _OpenDialog.FileName;
                var getName = _OpenDialog.SafeFileName;
                var retrieved = Path.GetExtension(getEx);
                _FileName = getName;
                _FilePath = _OpenDialog.FileName;
                _getExt = getEx;
                _retrieved = retrieved;
                txtFieldFileName.Text = getName;
            }
        }

        /// <summary>
        /// 
        /// Start inserting values
        /// 
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="thumbnailValue"></param>
        /// <returns></returns>
        private async Task startSending(Object setValue, Object thumbnailValue = null) {

            const string query = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB,@CUST_COMMENT)";

            try {

                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@CUST_TO", txtFieldShareToName.Text);
                    command.Parameters.AddWithValue("@CUST_FROM", Globals.custUsername);
                    command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(_FileName));
                    command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                    command.Parameters.AddWithValue("@CUST_FILE", setValue);
                    command.Parameters.AddWithValue("@FILE_EXT", _retrieved);
                    command.Parameters.AddWithValue("@CUST_COMMENT", EncryptionModel.Encrypt(txtFieldComment.Text));
                    command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);
                    command.Prepare();

                    await command.ExecuteNonQueryAsync();
                }

            } catch (Exception) {
                MessageBox.Show("Failed to share this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        private async Task startSharing() {

            string shareToName = txtFieldShareToName.Text;

            int _accType = accountType(shareToName);
            int _countReceiverFile = countReceiverShared(shareToName);

            if (_accType != _countReceiverFile) {

                if (_currentFileName != txtFieldFileName.Text) {

                    _currentFileName = txtFieldFileName.Text;

                    new Thread(() => new SharingAlert(shareToName: shareToName).ShowDialog()).Start();

                    byte[] _getBytes = File.ReadAllBytes(_FilePath); 
                    string _toBase64 = Convert.ToBase64String(_getBytes);

                    if (Globals.imageTypes.Contains(_retrieved)) {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".docx" || _retrieved == ".doc") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);                    
                    }
                    else if (_retrieved == ".pptx" || _retrieved == ".ppt") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".exe") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".msi") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".mp3" || _retrieved == ".wav") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }

                    else if (_retrieved == ".pdf") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".apk") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (_retrieved == ".xlsx" || _retrieved == ".xls") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);
                    }
                    else if (Globals.textTypes.Contains(_retrieved)) {

                        var nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_FilePath)) { 
                            nonLine = ReadFileTxt.ReadToEnd();  
                        }

                        byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptText = EncryptionModel.Encrypt(getEncoded);
                        
                        await startSending(encryptText);

                    } else if (Globals.videoTypes.Contains(_retrieved)) {

                        ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                        string toBase64Thumbnail;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            toBase64Thumbnail = Convert.ToBase64String(stream.ToArray());
                        }

                        string encryptText = EncryptionModel.Encrypt(_toBase64);

                        await startSending(encryptText,toBase64Thumbnail);
                    }

                    CloseForm.closeForm("SharingAlert");
                    new SucessSharedAlert(_currentFileName, shareToName).Show();

                }
                else {
                    new CustomAlert(title: "Sharing failed", subheader: "This file is already sent.").Show();
                }
            }
            else {
                new CustomAlert(title: "Sharing failed", subheader: "The receiver has reached the limit amount of files they can received.").Show();
            }
        }

        /// <summary>
        /// This function will retrieves user 
        /// account type 
        /// </summary>
        /// <param name="_receiverUsername">Receiver username who will received the file</param>
        /// <returns></returns>
        private int accountType(String _receiverUsername) {

            string _accType = "";

            const string _getAccountTypeQue = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(_getAccountTypeQue,con)) {
                command.Parameters.AddWithValue("@username",_receiverUsername);
                using(MySqlDataReader read = command.ExecuteReader()) {
                    if(read.Read()) {
                        _accType = read.GetString(0);
                    }
                }
            }

            return Globals.uploadFileLimit[_accType];

        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        private static string _hasPas = "DEF";
        private string hasPassword(String _custUsername) {

            string storeVal = "";
            const string queryGetAuth = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(queryGetAuth,con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                storeVal = command.ExecuteScalar().ToString();
            }

            if(storeVal == "DEF") {
                storeVal = "";
            } else {
                return storeVal;
            }

            return storeVal;

        }

        /// <summary>
        /// This function will count the number of 
        /// files the user has been shared
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int countReceiverShared(String _receiverUsername) {

            const string countFileSharedQuery = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
            int fileCount = 0;

            using (MySqlCommand command = new MySqlCommand(countFileSharedQuery, con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value) {
                    fileCount = Convert.ToInt32(result);
                }
            }

            return fileCount;

        }

        /// <summary>
        /// This function will retrieve user current 
        /// file sharing status (enabeled or disabled)
        /// </summary>
        private string retrieveDisabled(String _custUsername) {

            string isEnabled = "0";
            const string querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        isEnabled = reader.GetString(0);
                    }
                }
            }

            return isEnabled;
        }
        
        /// <summary>
        /// Verify if file is already shared
        /// </summary>
        /// <param name="_custUsername">Username of receiver</param>
        /// <param name="_fileName">File name to be send</param>
        /// <returns></returns>
        private int fileIsUploaded(String _custUsername,String _fileName) {

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";

            using(MySqlCommand command = new MySqlCommand(queryRetrieveCount,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@receiver", _custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                return Convert.ToInt32(command.ExecuteScalar());
            }

        }

        private string _currentFileName = "";

        /// <summary>
        /// Button to start file sharing 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                string textBox1 = txtFieldShareToName.Text;
                string textBox2 = txtFieldFileName.Text;

                if (textBox1 == Globals.custUsername) {
                    MessageBox.Show("You can't share to yourself.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (string.IsNullOrEmpty(textBox1) || string.IsNullOrEmpty(textBox2)) {
                    MessageBox.Show("Please enter valid input in the text boxes.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int userExists = userIsExists(textBox1);

                if (userExists == 0) {
                    MessageBox.Show($"User '{textBox1}' not found.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (fileIsUploaded(textBox1, textBox2) > 0) {
                    MessageBox.Show("This file is already shared.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string disabled = retrieveDisabled(textBox1);
                if (disabled != "0") {
                    MessageBox.Show($"The user {textBox1} disabled their file sharing.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string password = hasPassword(textBox1);
                if (!string.IsNullOrEmpty(password)) {
                    AskSharingAuthForm _askPassForm = new AskSharingAuthForm(textBox1, textBox2, _retrieved);
                    _askPassForm.Show();
                }
                else {

                    await startSharing();
                }
            }
            catch (Exception) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            label5.Text = txtFieldComment.Text.Length + "/295";
        }

        private void guna2Panel3_Paint_1(object sender, PaintEventArgs e) {

        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {

        }
    }
}
