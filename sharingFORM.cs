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

namespace FlowSERVER1 {
    public partial class sharingFORM : Form {
        private static String _FileName;
        private static String _FilePath;
        private static String _retrieved;
        private static String _getExt;
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static sharingFORM instance;
        public String _verifySetPas = "";
        public sharingFORM() {
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
            String countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(countUser,con);
            command.Parameters.AddWithValue("@username",_receiverUsername);
            var setupCount = command.ExecuteScalar();
            int ToInt = Convert.ToInt32(setupCount);
            return ToInt;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            OpenFileDialog _OpenDialog = new OpenFileDialog();
            _OpenDialog.Filter = "All Files|*.*";
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                var getEx = _OpenDialog.FileName;
                var getName = _OpenDialog.SafeFileName;
                var retrieved = Path.GetExtension(getEx);
                _FileName = getName;
                _FilePath = _OpenDialog.FileName;
                _getExt = getEx;
                _retrieved = retrieved;
                guna2TextBox2.Text = getName;
            }
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        private void startSharing() {

            int _accType = accountType(guna2TextBox1.Text);
            int _countReceiverFile = countReceiverShared(guna2TextBox1.Text);
            long fileSizeInMB = 0;

            if (_accType != _countReceiverFile) {
                if (_currentFileName != guna2TextBox2.Text) {

                    Byte[] _getBytes = File.ReadAllBytes(_FilePath);
                    fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                    Thread showUploadAlert = new Thread(() => new UploadAlrt(_FileName, Form1.instance.label5.Text, "cust_sharing", _controlName, guna2TextBox1.Text, _fileSize: fileSizeInMB).ShowDialog());
                    showUploadAlert.Start();

                    Application.DoEvents();

                    String varDate = DateTime.Now.ToString("dd/MM/yyyy");
                    String insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB,@CUST_COMMENT)";
                    command = new MySqlCommand(insertQuery, con);

                    command.Parameters.Add("@CUST_TO", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_FROM", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);
                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                    command.Parameters.Add("@FILE_EXT", MySqlDbType.Text);
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                    command.Parameters.Add("@CUST_COMMENT", MySqlDbType.LongText);

                    command.Parameters["@CUST_FROM"].Value = Form1.instance.label5.Text;
                    command.Parameters["@CUST_TO"].Value = guna2TextBox1.Text;
                    command.Parameters["@CUST_FILE_PATH"].Value = _FileName;
                    command.Parameters["@UPLOAD_DATE"].Value = varDate;
                    command.Parameters["@FILE_EXT"].Value = _retrieved;
                    command.Parameters["@CUST_COMMENT"].Value = guna2TextBox4.Text.Replace("\r\n","");

                    _currentFileName = guna2TextBox2.Text;

                    void startSending(Object setValue) {

                        command.Parameters["@CUST_FILE"].Value = setValue;
                        command.Prepare();
                        command.ExecuteNonQuery();
                        command.Dispose();

                        Application.OpenForms
                         .OfType<Form>()
                         .Where(form => String.Equals(form.Name, "UploadAlrt"))
                         .ToList()
                         .ForEach(form => form.Close());

                        sucessShare _showSuccessfullyTransaction = new sucessShare(guna2TextBox2.Text, guna2TextBox1.Text);
                        _showSuccessfullyTransaction.Show();

                        Application.OpenForms
                         .OfType<Form>()
                         .Where(form => String.Equals(form.Name, "UploadAlrt"))
                         .ToList()
                         .ForEach(form => form.Close());

                    }

                    if (_retrieved == ".png" || _retrieved == ".jpg" || _retrieved == ".jpeg" || _retrieved == ".bmp") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".docx" || _retrieved == ".doc") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".pptx" || _retrieved == ".ppt") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".exe") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.CommandTimeout = 12000;
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".msi") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.CommandTimeout = 12000;
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".mp3" || _retrieved == ".wav") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }

                    else if (_retrieved == ".pdf") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".apk") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".xlsx" || _retrieved == ".xls") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        startSending(_toBase64);
                    }
                    else if (_retrieved == ".gif") {
                        ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64String = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64String;
                        }
                        var _toBase64 = Convert.ToBase64String(File.ReadAllBytes(_FilePath));
                        startSending(_toBase64);

                    }
                    else if (_retrieved == ".txt" || _retrieved == ".html" || _retrieved == ".xml" || _retrieved == ".py" || _retrieved == ".css" || _retrieved == ".js" || _retrieved == ".sql" || _retrieved == ".csv") {
                        var nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_FilePath)) { //open.FileName
                            nonLine = ReadFileTxt.ReadToEnd();
                        }
                        //var encryptValue = EncryptionModel.EncryptText(nonLine);
                        startSending(nonLine);

                    }
                    else if (_retrieved == ".mp4" || _retrieved == ".mov" || _retrieved == ".webm" || _retrieved == ".avi" || _retrieved == ".wmv") {
                        ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64Str = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64Str;// To load: Bitmap -> Byte array
                        }
                        var _toBase64 = Convert.ToBase64String(File.ReadAllBytes(_FilePath));
                        startSending(_toBase64);
                    }

                    Application.DoEvents();

                }
                else {
                    MessageBox.Show("File is already sent.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else {
                MessageBox.Show("The receiver has reached the limit amount of files they can received.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// This function will retrieves user 
        /// account type 
        /// </summary>
        /// <param name="_receiverUsername">Receiver username who will received the file</param>
        /// <returns></returns>
        private int accountType(String _receiverUsername) {

            int _allowedReturn = 20;
            String _accType = "";

            String _getAccountTypeQue = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccountTypeQue,con);
            command.Parameters.AddWithValue("@username",_receiverUsername);
            
            MySqlDataReader _readAccType = command.ExecuteReader();
            if(_readAccType.Read()) {
                _accType = _readAccType.GetString(0);
            }
            _readAccType.Close();
            if(_accType == "Max") {
                _allowedReturn = 500;
            } else if (_accType == "Express") {
                _allowedReturn = 1000;
            } else if (_accType == "Supreme") {
                _allowedReturn = 2000;
            } else if (_accType == "Basic") {
                _allowedReturn = 20;
            }
            return _allowedReturn;
        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        private static string _hasPas = "DEF";
        private string hasPassword(String _custUsername) {
            String storeVal = "";
            String queryGet = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(queryGet, con);
            command.Parameters.AddWithValue("@username", _custUsername);

            object result = command.ExecuteScalar();
            if (result != null) {
                storeVal = result.ToString();
                if (storeVal == "DEF") {
                    storeVal = "";
                }
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
            string countFileSharedQuery = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
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
        private String retrieveDisabled(String _custUsername) {
            String querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            String isEnabled = "0";

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
            String queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";
            command = new MySqlCommand(queryRetrieveCount, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@receiver", _custUsername);
            command.Parameters.AddWithValue("@filename", _fileName);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count;

        }

        private static String _controlName = null;
        private static String _currentFileName = "";

        /// <summary>
        /// Button to start file sharing 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {
            try {

                if(guna2TextBox1.Text != Form1.instance.label5.Text) {
                    if(guna2TextBox1.Text != String.Empty) {
                        if(guna2TextBox2.Text != String.Empty) {
                            if(userIsExists(guna2TextBox1.Text) > 0) {

                                if(fileIsUploaded(guna2TextBox1.Text,guna2TextBox2.Text) > 0) {

                                    MessageBox.Show("This file is already shared.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);

                                } else {

                                    if(!(retrieveDisabled(guna2TextBox1.Text) == "0")) {
                                        MessageBox.Show("The user " + guna2TextBox1.Text + " disabled their file sharing.","Sharing Failed",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                    } else {

                                        if(hasPassword(guna2TextBox1.Text) != "") {
                                            AskPassSharing _askPassForm = new AskPassSharing(guna2TextBox1.Text,guna2TextBox2.Text,_FilePath,_retrieved);
                                            _askPassForm.Show();

                                        } else {

                                            startSharing();

                                            Application.OpenForms
                                           .OfType<Form>()
                                           .Where(form => String.Equals(form.Name, "UploadAlrt"))
                                           .ToList()
                                           .ForEach(form => form.Close());

                                        }
                                    }
                                }
                            }
                            else {
                                MessageBox.Show("User '" + guna2TextBox1.Text + "' not found.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                          }
                        }
                    } else {
                        MessageBox.Show("You can't share to yourself.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            } catch (Exception) {
                MessageBox.Show("An unknown error occurred.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            label5.Text = guna2TextBox4.Text.Length + "/52";
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
