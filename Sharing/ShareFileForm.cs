using FlowSERVER1.AlertForms;
using FlowSERVER1.Helper;
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
            lblFileName.Text = FileName;
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
        private string hasPassword(String _custUsername) {

            string storeVal = "";
            const string queryGetSharingAuth = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";

            using(MySqlCommand command = new MySqlCommand(queryGetSharingAuth,con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                using(MySqlDataReader read = command.ExecuteReader()) {
                    if(read.Read()) {
                        if(read.GetString(0) == "DEF") {
                            storeVal = "";
                        } else {
                            storeVal = read.GetString(0);
                        }
                    }
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
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int userIsExists(String _receiverUsername) {

            int count = 0;

            const string countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(countUser,con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);
                count = Convert.ToInt32(command.ExecuteScalar());
            }

            return count;
        }

        /// <summary>
        /// Verify if file is already shared
        /// </summary>
        /// <param name="_custUsername">Username of receiver</param>
        /// <param name="_fileName">File name to be send</param>
        /// <returns></returns>
        private int fileIsUploaded(String _custUsername, String _fileName) {

            int count = 0;

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";
            using(MySqlCommand command = new MySqlCommand(queryRetrieveCount,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@receiver", _custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                count = Convert.ToInt32(command.ExecuteScalar());
            }
            
            return count;

        }

        private async Task<string> getFileMetadata(String _fileName,String _TableName) {

            string GetBase64String = "";
            string queryGetFileByte = $"SELECT CUST_FILE FROM {_TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

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

        private async Task<string> getFileMetadataSharedToMe(String _custUsername, String _fileName) {

            string GetBase64String = "";
            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataSharedToOthers(String _custUsername, String _fileName) {

            string GetBase64String = "";
            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataExtra(String _tableName,String _columnName, String _dirName, String _custUsername,String _fileName) {

            string base64String = "";

            string queryGetFileByte = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {_columnName} = @dirname LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename",EncryptionModel.Encrypt(_fileName)); 
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_dirName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        base64String = readData.GetString(0);
                    }
                }
            }

            return base64String;
        }

        private async Task<string> retrieveThumbnails(String _tableName, String _custUsername, String _fileName) {

            String GetBase64String = "";
            String queryGetFileByte = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_dirName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> retrieveThumbnailShared(String fileName,String columnName) {

            string GetBase64String = "";
            string queryGetFileByte = $"SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = {columnName} AND CUST_FILE_PATH = @filename LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
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
                command.Parameters.AddWithValue("@CUST_TO", txtFieldShareToName.Text);
                command.Parameters.AddWithValue("@CUST_FROM", Globals.custUsername);
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(_FileName));
                command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@CUST_FILE", setValue);
                command.Parameters.AddWithValue("@FILE_EXT", _FileExt);
                command.Parameters.AddWithValue("@CUST_COMMENT", EncryptionModel.Encrypt(txtFieldComment.Text));
                command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);

                await command.ExecuteNonQueryAsync();
            }

        }

        private async void startSharing() {

            int _accType = accountType(txtFieldShareToName.Text);
            int _countReceiverFile = countReceiverShared(txtFieldShareToName.Text);
            string shareToName = txtFieldShareToName.Text;

            if (_accType != _countReceiverFile) {

                new Thread(() => new SharingAlert(shareToName: shareToName).ShowDialog()).Start();

                if (_IsFromShared == false && _IsFromTable == "cust_sharing") {
                    string getThumbnails = await retrieveThumbnailShared(_FileName,"CUST_TO");
                    await startSending(getFileMetadataSharedToMe(Globals.custUsername, _FileName), getThumbnails);
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
                        string getThumbnails = await retrieveThumbnails("file_info_vid",Globals.custUsername,_FileName);
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_vid"),getThumbnails);
                    }
                    
                    else if (_FileExt == ".exe") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_exe"));
                    }
                    
                    else if (_FileExt == ".apk") {
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_FileName), "file_info_apk"));
                    }

                } else if (_IsFromShared == true && _IsFromTable == "cust_sharing") {
                    string getThumbnails = await retrieveThumbnailShared(_FileName, "CUST_FROM");
                    await startSending(getFileMetadataSharedToOthers(Globals.custUsername, _FileName), getThumbnails);
                } 
                
                else if (_IsFromTable == "upload_info_directory") {
                    string getThumbnails = await retrieveThumbnailsExtra("upload_info_directory", "DIR_NAME", _DirectoryName, Globals.custUsername, _FileName);
                    await startSending(await getFileMetadataExtra("upload_info_directory","DIR_NAME",_DirectoryName,Globals.custUsername,_FileName));
                } 
                
                else if (_IsFromTable == "folder_upload_info") {
                    string getThumbnails = await retrieveThumbnailsExtra("folder_upload_info", "FOLDER_TITLE", _DirectoryName, Globals.custUsername, _FileName);
                    await startSending(await getFileMetadataExtra("folder_upload_info", "FOLDER_TITLE", _DirectoryName, Globals.custUsername, _FileName));
                }

                CloseForm.closeForm("SharingAlert");
                new SucessSharedAlert(_FileName, txtFieldShareToName.Text).Show();

            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                if (txtFieldShareToName.Text == Globals.custUsername) {
                    new CustomAlert(title: "Sharing failed", subheader: "You can't share to yourself.").Show();
                    return;
                }
                if (txtFieldShareToName.Text == String.Empty) {
                    return;
                }
                if (userIsExists(txtFieldShareToName.Text) == 0) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} does not exist.").Show();
                    return;
                }
                if (fileIsUploaded(txtFieldShareToName.Text, _FileName) > 0) {
                    new CustomAlert(title: "Sharing failed", subheader: "This file is already shared.").Show();
                    return;
                }

                if (!(retrieveDisabled(txtFieldShareToName.Text) == "0")) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} disabled their file sharing.").Show();
                    return;
                }

                if (hasPassword(txtFieldShareToName.Text) != "") {
                    new AskSharingAuthForm(txtFieldShareToName.Text, _FileName, _FileName.Substring(_FileName.Length-4)).Show();
                    return;
                }

                startSharing();
                               
            }
            catch (Exception) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            lblCountCharComment.Text = txtFieldComment.Text.Length.ToString() + "/295";
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
