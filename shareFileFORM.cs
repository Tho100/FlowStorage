using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class shareFileFORM : Form {
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        public static String _FileName { get; set; }
        public static String _FileExt {get; set ;}
        public static bool _IsFromShared { get; set; }
        public shareFileFORM(String FileName,String FileExtension,bool IsFromShared) {
            InitializeComponent();
            _FileName = FileName;
            _FileExt = FileExtension;
            _IsFromShared = IsFromShared;
            label3.Text = FileName;
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
            command = new MySqlCommand(_getAccountTypeQue, con);
            command.Parameters.AddWithValue("@username", _receiverUsername);

            MySqlDataReader _readAccType = command.ExecuteReader();
            if (_readAccType.Read()) {
                _accType = _readAccType.GetString(0);
            }
            _readAccType.Close();
            if (_accType == "Max") {
                _allowedReturn = 500;
            }
            else if (_accType == "Express") {
                _allowedReturn = 1000;
            }
            else if (_accType == "Supreme") {
                _allowedReturn = 2000;
            }
            else if (_accType == "Basic") {
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
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int userIsExists(String _receiverUsername) {
            String countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(countUser, con);
            command.Parameters.AddWithValue("@username", _receiverUsername);
            var setupCount = command.ExecuteScalar();
            int ToInt = Convert.ToInt32(setupCount);
            return ToInt;
        }

        /// <summary>
        /// Verify if file is already shared
        /// </summary>
        /// <param name="_custUsername">Username of receiver</param>
        /// <param name="_fileName">File name to be send</param>
        /// <returns></returns>
        private int fileIsUploaded(String _custUsername, String _fileName) {
            String queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";
            command = new MySqlCommand(queryRetrieveCount, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@receiver", _custUsername);
            command.Parameters.AddWithValue("@filename", _fileName);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count;

        }

        private string getFileMetadata(String _fileName,String _TableName) {
            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using(MySqlCommand command = new MySqlCommand(queryGetFileByte,con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", _fileName);
                using(MySqlDataReader readData = (MySqlDataReader) command.ExecuteReader()) {
                    while (readData.Read()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private string getFileMetadataShared(String _custUsername, String _fileName) {
            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                using (MySqlDataReader readData = (MySqlDataReader)command.ExecuteReader()) {
                    while (readData.Read()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        private static String _controlName = null;
        private static String _currentFileName = "";
        private void startSharing() {

            int _accType = accountType(guna2TextBox1.Text);
            int _countReceiverFile = countReceiverShared(guna2TextBox1.Text);
            long fileSizeInMB = 0;

            if (_accType != _countReceiverFile) {

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
                command.Parameters["@FILE_EXT"].Value = _FileExt;
                command.Parameters["@CUST_COMMENT"].Value = guna2TextBox4.Text.Replace("\r\n","");

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

                    sucessShare _showSuccessfullyTransaction = new sucessShare(_FileName, guna2TextBox1.Text);
                    _showSuccessfullyTransaction.Show();

                    Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "UploadAlrt"))
                        .ToList()
                        .ForEach(form => form.Close());
                }

                if (_IsFromShared == false) {
                    startSending(getFileMetadataShared(Form1.instance.label5.Text, _FileName));
                } else {

                    if (_FileExt == ".png" || _FileExt == ".jpg" || _FileExt == ".jpeg" || _FileExt == ".bmp") {
                        startSending(getFileMetadata(_FileName,"file_info"));
                    } else if (_FileExt == ".xlsx" || _FileExt == ".xls") {
                        startSending(getFileMetadata(_FileName, "file_info_excel"));
                    } else if (_FileExt == ".txt" || _FileExt == ".html" || _FileExt == ".sql" || _FileExt == ".csv" || _FileExt == ".css" || _FileExt == ".js") {
                        startSending(getFileMetadata(_FileName, "file_info_expand"));
                    } else if (_FileExt == ".pdf") {
                        startSending(getFileMetadata(_FileName, "file_info_pdf"));
                    } else if (_FileExt == ".pptx" || _FileExt == ".ppt") {
                        startSending(getFileMetadata(_FileName, "file_info_ptx"));
                    } else if (_FileExt == ".docx" || _FileExt == ".doc") {
                        startSending(getFileMetadata(_FileName, "file_info_doc"));
                    } else if (_FileExt == ".wav" || _FileExt == ".mp3") {
                        startSending(getFileMetadata(_FileName, "file_info_audi"));
                    } else if (_FileExt == ".mp4" || _FileExt == ".mov" || _FileExt == ".avi" || _FileExt == ".webm" || _FileExt == ".wmv") {
                        startSending(getFileMetadata(_FileName, "file_info_vid"));
                    } else if (_FileExt == ".exe") {
                        startSending(getFileMetadata(_FileName, "file_info_exe"));
                    } else if (_FileExt == ".apk") {
                        startSending(getFileMetadata(_FileName, "file_info_apk"));
                    }

                }

            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                if (guna2TextBox1.Text != Form1.instance.label5.Text) {
                    if (guna2TextBox1.Text != String.Empty) {
                        if (userIsExists(guna2TextBox1.Text) > 0) {
                            if (fileIsUploaded(guna2TextBox1.Text, _FileName) > 0) {
                                MessageBox.Show("This file is already shared.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            } else {

                                if (!(retrieveDisabled(guna2TextBox1.Text) == "0")) {
                                    MessageBox.Show("The user " + guna2TextBox1.Text + " disabled their file sharing.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else {

                                    //if (hasPassword(guna2TextBox1.Text) != "") {
                                       // AskPassSharing _askPassForm = new AskPassSharing(guna2TextBox1.Text, _FileName, _FilePath, _retrieved);
                                       // _askPassForm.Show();

                                    //}
                                    //else {

                                        startSharing();

                                        Application.OpenForms
                                        .OfType<Form>()
                                        .Where(form => String.Equals(form.Name, "UploadAlrt"))
                                        .ToList()
                                        .ForEach(form => form.Close());

                                    //}
                                }
                            }
                        }
                        else {
                            MessageBox.Show("User '" + guna2TextBox1.Text + "' not found.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
                    }
                }
                else {
                    MessageBox.Show("You can't share to yourself.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            label5.Text = guna2TextBox4.Text.Length.ToString() + "/52";
        }
    }
}
