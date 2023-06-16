using FlowSERVER1.AlertForms;
using FlowSERVER1.Sharing;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Shell;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class shareFileFORM : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;

        public static string _FileName { get; set; }
        public static string _FileExt {get; set ;}
        public static string _IsFromTable {get; set;}
        public static string _DirectoryName { get; set; }
        public static bool _IsFromShared { get; set; }

        public shareFileFORM(String FileName,String FileExtension,bool IsFromShared, String IsFrom,String DirectoryName) {
            InitializeComponent();
            _FileName = FileName;
            _FileExt = FileExtension;
            _IsFromShared = IsFromShared;
            _IsFromTable = IsFrom;
            _DirectoryName = DirectoryName;
            label3.Text = FileName;
        }

        /// <summary>
        /// This function will retrieves user 
        /// account type 
        /// </summary>
        /// <param name="_receiverUsername">Receiver username who will received the file</param>
        /// <returns></returns>
        private int accountType(String _receiverUsername) {

            string _accType = "";

            string _getAccountTypeQue = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(_getAccountTypeQue, con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);

                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        _accType = reader.GetString(0);
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
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int userIsExists(String _receiverUsername) {

            const string  countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";

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

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";

            command = new MySqlCommand(queryRetrieveCount, con);
            command.Parameters.AddWithValue("@username", Globals.custUsername);
            command.Parameters.AddWithValue("@receiver", _custUsername);
            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName, EncryptionKey.KeyValue));

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count;

        }

        private async Task<string> getFileMetadata(String _fileName,String _TableName) {

            string GetBase64String = "";
            string queryGetFileByte = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";

            using(MySqlCommand command = new MySqlCommand(queryGetFileByte,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                using(MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataShared(String _custUsername, String _fileName) {

            string GetBase64String = "";
            const string queryGetFileByte = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";

            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataExtra(String _tableName,String _columnName, String _dirName, String _custUsername,String _fileName) {

            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {_columnName} = @dirname";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename",_fileName); 
                command.Parameters.AddWithValue("@dirname", _dirName);
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> retrieveThumbnails(String _tableName, String _custUsername, String _fileName) {

            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }
        private async Task<string> retrieveThumbnailsExtra(String _tableName, String _columnName, String _dirName, String _custUsername, String _fileName) {

            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {_columnName} = @dirname";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", _fileName);
                command.Parameters.AddWithValue("@dirname", _dirName);
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>

        private async Task startSending(Object setValue, Object thumbnailValue = null) {

            using (MySqlCommand command = new MySqlCommand("INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB,@CUST_COMMENT)", con)) {
                command.Parameters.AddWithValue("@CUST_TO", btnShareToName.Text);
                command.Parameters.AddWithValue("@CUST_FROM", Globals.custUsername);
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(_FileName, EncryptionKey.KeyValue));
                command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@CUST_FILE", setValue);
                command.Parameters.AddWithValue("@FILE_EXT", _FileExt);
                command.Parameters.AddWithValue("@CUST_COMMENT", EncryptionModel.Encrypt(guna2TextBox4.Text));
                command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);

                await command.ExecuteNonQueryAsync();
            }

        }

        private async void startSharing() {

            int _accType = accountType(btnShareToName.Text);
            int _countReceiverFile = countReceiverShared(btnShareToName.Text);
            string shareToName = btnShareToName.Text;

            if (_accType != _countReceiverFile) {

                new Thread(() => new SharingAlert(fileName: _FileName, shareToName: shareToName).ShowDialog()).Start();

                if (_IsFromShared == false && _IsFromTable == "cust_sharing") {
                    string getThumbnails = await retrieveThumbnails("cust_sharing",Globals.custUsername, EncryptionModel.Encrypt(_FileName, EncryptionKey.KeyValue));
                    await startSending(getFileMetadataShared(Globals.custUsername, EncryptionModel.Encrypt(_FileName)),getThumbnails);
                } else if (_IsFromTable != "upload_info_directory" && _IsFromTable != "folder_upload_info") {

                    if (Globals.imageTypes.Contains(_FileExt)) {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName),"file_info"));  
                    } 
                    
                    else if (_FileExt == ".xlsx" || _FileExt == ".xls") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_excel"));
                    }
                    
                    else if (Globals.textTypes.Contains(_FileExt)) {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_expand"));
                    } 
                    
                    else if (_FileExt == ".pdf") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_pdf"));
                    } 
                    
                    else if (_FileExt == ".pptx" || _FileExt == ".ppt") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_ptx"));
                    } 
                    
                    else if (_FileExt == ".docx" || _FileExt == ".doc") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_word"));
                    } 
                    
                    else if (_FileExt == ".wav" || _FileExt == ".mp3") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_audi"));
                    } 
                    
                    else if (Globals.videoTypes.Contains(_FileExt)) {
                        string getThumbnails = await retrieveThumbnails("file_info_vid",Globals.custUsername,EncryptionModel.Encrypt(_FileName));
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_vid"),getThumbnails);
                    }
                    
                    else if (_FileExt == ".exe") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_exe"));
                    }
                    
                    else if (_FileExt == ".apk") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_apk"));
                    }

                } 
                
                else if (_IsFromTable == "upload_info_directory") {
                    string getThumbnails = await retrieveThumbnailsExtra("upload_info_directory", "DIR_NAME", _DirectoryName, Globals.custUsername, EncryptionModel.Encrypt(_FileName));
                    await startSending(await getFileMetadataExtra("upload_info_directory","DIR_NAME",_DirectoryName,Globals.custUsername,EncryptionModel.Encrypt(_FileName)));
                } 
                
                else if (_IsFromTable == "folder_upload_info") {
                    string getThumbnails = await retrieveThumbnailsExtra("folder_upload_info", "FOLDER_TITLE", _DirectoryName, Globals.custUsername, EncryptionModel.Encrypt(_FileName));
                    await startSending(await getFileMetadataExtra("folder_upload_info", "FOLDER_TITLE", _DirectoryName, Globals.custUsername, EncryptionModel.Encrypt(_FileName)));
                }

                CloseForm.closeForm("SharingAlert");
                new SucessSharedAlert(_FileName, btnShareToName.Text).Show();

            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                if (btnShareToName.Text == Globals.custUsername) {
                    new CustomAlert(title: "Sharing failed", subheader: "You can't share to yourself.").Show();
                    return;
                }
                if (btnShareToName.Text == String.Empty) {
                    return;
                }
                if (userIsExists(btnShareToName.Text) == 0) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {btnShareToName.Text} does not exist.").Show();
                    return;
                }
                if (fileIsUploaded(btnShareToName.Text, _FileName) > 0) {
                    new CustomAlert(title: "Sharing failed", subheader: "This file is already shared.").Show();
                    return;
                }

                if (!(retrieveDisabled(btnShareToName.Text) == "0")) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {btnShareToName.Text} disabled their file sharing.").Show();
                    return;
                }
        


                //if (hasPassword(guna2TextBox1.Text) != "") {
                // AskPassSharing _askPassForm = new AskPassSharing(guna2TextBox1.Text, _FileName, _FilePath, _retrieved);
                // _askPassForm.Show();

            //}
            //else {

                startSharing();


            //}
                               
 
            }
            catch (Exception eq) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            label5.Text = guna2TextBox4.Text.Length.ToString() + "/295";
        }

        private void shareFileFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnShareToName_TextChanged(object sender, EventArgs e) {

        }
    }
}
