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

namespace FlowSERVER1 {
    public partial class AskPassSharing : Form {
        private static String CustUsername = "";
        private static String _currentFileName = "";
        private static String _FileName = "";
        private static String _FilePath = "";
        private static String _retrieved = "";
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        public AskPassSharing(String _custUsername,String _fileName,String _filePath,String _RETRIEVED) {
            InitializeComponent();
            CustUsername = _custUsername;
            _currentFileName = _fileName;
            _FileName = _fileName;
            _FilePath = _filePath;
            _retrieved = _RETRIEVED;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            guna2TextBox2.PasswordChar = '\0';
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

            int _allowedReturn = 12;
            String _accType = "";

            String _getAccountTypeQue = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccountTypeQue, con);
            command.Parameters.AddWithValue("@username", _receiverUsername);

            MySqlDataReader _readAccType = command.ExecuteReader();
            if (_readAccType.Read()) {
                _accType = _readAccType.GetString(0);
            }
            _readAccType.Close();
            if (_accType == "Max") {
                _allowedReturn = 120;
            }
            else if (_accType == "Express") {
                _allowedReturn = 500;
            }
            else if (_accType == "Supreme") {
                _allowedReturn = 1500;
            }
            else if (_accType == "Basic") {
                _allowedReturn = 20;
            }
            return _allowedReturn;
        }

        /// <summary>
        /// This function will count the number of 
        /// files the user has been shared
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int countReceiverShared(String _receiverUsername) {
            String _countFileShared = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
            command = new MySqlCommand(_countFileShared, con);
            command.Parameters.AddWithValue("@username", _receiverUsername);

            var _getValue = command.ExecuteScalar();
            int _toInt = Convert.ToInt32(_getValue);
            return _toInt;

        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        String _controlName = null;
        private void startSharing() {
            int _accType = accountType(CustUsername);
            int _countReceiverFile = countReceiverShared(CustUsername);
            long fileSizeInMB = 0;

            if (_accType != _countReceiverFile) {

                Byte[] _getBytes = File.ReadAllBytes(_FilePath);
                fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                Thread showUploadAlert = new Thread(() => new UploadAlrt(_FileName, Form1.instance.label5.Text, "cust_sharing", _controlName, CustUsername, _fileSize: fileSizeInMB).ShowDialog());
                showUploadAlert.Start();

                String varDate = DateTime.Now.ToString("dd/MM/yyyy");
                String insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB)";
                command = new MySqlCommand(insertQuery, con);

                command.Parameters.Add("@CUST_TO", MySqlDbType.Text);
                command.Parameters.Add("@CUST_FROM", MySqlDbType.Text);
                command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);
                command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                command.Parameters.Add("@FILE_EXT", MySqlDbType.Text);
                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);

                command.Parameters["@CUST_FROM"].Value = Form1.instance.label5.Text;
                command.Parameters["@CUST_TO"].Value = CustUsername;
                command.Parameters["@CUST_FILE_PATH"].Value = _FileName;
                command.Parameters["@UPLOAD_DATE"].Value = varDate;
                command.Parameters["@FILE_EXT"].Value = _retrieved;

                _currentFileName = guna2TextBox2.Text;

                if (_retrieved == ".png" || _retrieved == ".jpg" || _retrieved == ".jpeg" || _retrieved == ".bmp") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;//File.ReadAllBytes(_FilePath);
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".docx" || _retrieved == ".doc") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".pptx" || _retrieved == ".ppt") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".exe") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".mp3" || _retrieved == ".wav") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".pdf") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".apk") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".xlsx" || _retrieved == ".xls") {
                    var _toBase64 = Convert.ToBase64String(_getBytes);
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
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
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }
                else if (_retrieved == ".txt" || _retrieved == ".html" || _retrieved == ".xml" || _retrieved == ".py" || _retrieved == ".css" || _retrieved == ".js" || _retrieved == ".sql") {
                    var nonLine = "";
                    using (StreamReader ReadFileTxt = new StreamReader(_FilePath)) { //open.FileName
                        nonLine = ReadFileTxt.ReadToEnd();
                    }
                    var encryptValue = EncryptionModel.Encrypt(nonLine,EncryptionKey.KeyValue);
                    command.Parameters["@CUST_FILE"].Value = encryptValue;
                    command.ExecuteNonQuery();
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
                    command.Parameters["@CUST_FILE"].Value = _toBase64;
                    command.ExecuteNonQuery();
                }

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "UploadAlrt"))
                    .ToList()
                    .ForEach(form => form.Close());

                sucessShare _showSuccessfullyTransaction = new sucessShare(_FileName, CustUsername);
                _showSuccessfullyTransaction.Show();

                Application.OpenForms
                .OfType<Form>()
                .Where(form => String.Equals(form.Name, "UploadAlrt"))
                .ToList()
                .ForEach(form => form.Close());

                this.Close();
            }
            else {
                MessageBox.Show("The receiver has reached the limit amount of files they can received.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string getInformationSharing(String _custUsername) {
            String _storeVal = "";
            String _queryGet = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_queryGet, con);
            command.Parameters.AddWithValue("@username", _custUsername);

            MySqlDataReader _readPas = command.ExecuteReader();
            while (_readPas.Read()) {
                _storeVal = _readPas.GetString(0);
            }
            _readPas.Close();

            return _storeVal;
        }
        /// <summary>
        /// This button will start sharing file if 
        /// the entered password is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {
            var _getInput = guna2TextBox2.Text;
            var _decryptionOutput = EncryptionModel.Decrypt(getInformationSharing(CustUsername), "0123456789085746");
            if(_getInput == _decryptionOutput) {
                startSharing();
            } else {
                label4.Visible = true;
            }
        }
    }
}
