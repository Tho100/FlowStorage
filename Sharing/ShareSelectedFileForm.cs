using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using FlowSERVER1.Sharing;

namespace FlowSERVER1 {
    public partial class shareFileFORM : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

        private string _fileName { get; set; }
        private string _fileExtension {get; set ;}
        private string _tableName {get; set;}
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }

        public shareFileFORM(String fileName,String fileExtension,bool isFromShared, String tableName,String directoryName) {
            InitializeComponent();

            this._fileName = fileName;
            this._fileExtension = fileExtension;
            this._isFromShared = isFromShared;
            this._tableName = tableName;
            this._directoryName = directoryName;

            lblFileName.Text = fileName;
        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        private string ReceiverHasAuthVerificaiton(String _custUsername) {

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
        private int CountReceiverTotalShared(String username) {

            const string countFileSharedQuery = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @receiver";
            int fileCount = 0;

            using (MySqlCommand command = new MySqlCommand(countFileSharedQuery, con)) {
                command.Parameters.AddWithValue("@receiver", username);

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
        private string RetrieveIsSharingDisabled(String username) {

            string isEnabled = "0";
            const string querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @receiver";

            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@receiver", username);
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
        private int UserExistVerification(String _receiverUsername) {

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
        private int FileIsUploadedVerification(String username, String fileName) {

            int count = 0;

            const string queryRetrieveCount = "SELECT COUNT(CUST_TO) FROM cust_sharing WHERE CUST_FROM = @sender AND CUST_FILE_PATH = @filename AND CUST_TO = @receiver";
            using(MySqlCommand command = new MySqlCommand(queryRetrieveCount,con)) {
                command.Parameters.AddWithValue("@sender", Globals.custUsername);
                command.Parameters.AddWithValue("@receiver", username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                count = Convert.ToInt32(command.ExecuteScalar());
            }
            
            return count;

        }

        private async Task<string> getFileMetadata(String fileName,String tableName) {

            string GetBase64String = "";
            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using(MySqlCommand command = new MySqlCommand(queryGetFileByte,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", fileName);
                using(MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataSharedToMe(String fileName) {

            string GetBase64String = "";
            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> getFileMetadataSharedToOthers(String fileName) {

            string GetBase64String = "";
            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
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

        private async Task<string> getFileMetadataExtra(String tableName, String columnName, String directoryName, String fileName) {

            string base64String = "";

            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename",EncryptionModel.Encrypt(fileName)); 
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        base64String = readData.GetString(0);
                    }
                }
            }

            return base64String;
        }

        private async Task<string> retrieveThumbnails(String tableName, String fileName) {

            string GetBase64String = "";

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> retrieveThumbnailsExtra(String tableName, String columnName, String directoryName, String fileName) {

            string GetBase64String = "";

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
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
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(_fileName));
                command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@CUST_FILE", setValue);
                command.Parameters.AddWithValue("@FILE_EXT", "." + _fileExtension);
                command.Parameters.AddWithValue("@CUST_COMMENT", EncryptionModel.Encrypt(txtFieldComment.Text));
                command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);

                await command.ExecuteNonQueryAsync();
            }

        }

        private async void StartSharingFile() {

            int receiverUploadLimit = Globals.uploadFileLimit[await crud.ReturnUserAccountType(txtFieldShareToName.Text)];
            int receiverCurrentTotalUploaded = CountReceiverTotalShared(txtFieldShareToName.Text);
            string shareToName = txtFieldShareToName.Text;

            if (receiverUploadLimit != receiverCurrentTotalUploaded) {

                new Thread(() => new SharingAlert(shareToName: shareToName).ShowDialog()).Start();

                if (_isFromShared == false && _tableName == "cust_sharing") {

                    string getThumbnails = await retrieveThumbnailShared(_fileName, "CUST_TO");
                    await startSending(getFileMetadataSharedToMe(_fileName), getThumbnails);

                } else if (_tableName != GlobalsTable.directoryUploadTable && _tableName != GlobalsTable.folderUploadTable) {

                    if (Globals.imageTypesFolder.Contains(_fileExtension)) {
                        string finalTable = _tableName == "ps_info_image" ? "ps_info_image" : GlobalsTable.homeImageTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));  
                    }

                    else if (Globals.videoTypesFolder.Contains(_fileExtension)) {

                        string finalTable = _tableName == "ps_info_video" ? "ps_info_video" : GlobalsTable.homeVideoTable;

                        string getThumbnails = await retrieveThumbnails(finalTable, _fileName);

                        await startSending(await getFileMetadata(
                            EncryptionModel.Encrypt(_fileName), finalTable), getThumbnails);
                    }

                    else if (Globals.textTypesFolder.Contains(_fileExtension)) {
                        string finalTable = _tableName == "ps_info_text" ? "ps_info_text" : GlobalsTable.homeTextTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    }

                    else if (_fileExtension == "xlsx" || _fileExtension == "xls") {
                        string finalTable = _tableName == "ps_info_excel" ? "ps_info_excel" : GlobalsTable.homeExcelTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    }
                    
                    else if (_fileExtension == "pdf") {
                        string finalTable = _tableName == "ps_info_pdf" ? "ps_info_pdf" : GlobalsTable.homePdfTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    } 
                    
                    else if (_fileExtension == "pptx" || _fileExtension == "ppt") {
                        string finalTable = _tableName == "ps_info_ptx" ? "ps_info_ptx" : GlobalsTable.homePtxTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    } 
                    
                    else if (_fileExtension == "docx" || _fileExtension == "doc") {
                        string finalTable = _tableName == "ps_info_word" ? "ps_info_word" : GlobalsTable.homeWordTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    } 
                    
                    else if (_fileExtension == "wav" || _fileExtension == "mp3") {
                        string finalTable = _tableName == "ps_info_audio" ? "ps_info_audio" : GlobalsTable.homeAudioTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    } 
                    
                    else if (_fileExtension == "exe") {
                        string finalTable = _tableName == "ps_info_exe" ? "ps_info_exe" : GlobalsTable.homeExeTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    }
                    
                    else if (_fileExtension == "apk") {
                        string finalTable = _tableName == "ps_info_apk" ? "ps_info_apk" : GlobalsTable.homeApkTable;
                        await startSending(await getFileMetadata(EncryptionModel.Encrypt(_fileName), finalTable));
                    }

                } else if (_isFromShared == true && _tableName == GlobalsTable.sharingTable) {
                    string getThumbnails = await retrieveThumbnailShared(_fileName, "CUST_FROM");
                    await startSending(getFileMetadataSharedToOthers(_fileName), getThumbnails);
                } 
                
                else if (_tableName == GlobalsTable.directoryUploadTable) {

                    string getThumbnails = await retrieveThumbnailsExtra(
                        GlobalsTable.directoryUploadTable, "DIR_NAME", _directoryName, _fileName);

                    await startSending(await getFileMetadataExtra(
                        GlobalsTable.directoryUploadTable,"DIR_NAME",_directoryName, _fileName), getThumbnails);
                } 
                
                else if (_tableName == GlobalsTable.folderUploadTable) {

                    string getThumbnails = await retrieveThumbnailsExtra(
                        GlobalsTable.folderUploadTable, "FOLDER_TITLE", _directoryName, _fileName);

                    await startSending(await getFileMetadataExtra(
                        GlobalsTable.folderUploadTable, "FOLDER_TITLE", _directoryName, _fileName), getThumbnails);
                }

                CloseForm.closeForm("SharingAlert");
                new SucessSharedAlert(_fileName, txtFieldShareToName.Text).Show();

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
                if (UserExistVerification(txtFieldShareToName.Text) == 0) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} does not exist.").Show();
                    return;
                }
                if (FileIsUploadedVerification(txtFieldShareToName.Text, _fileName) > 0) {
                    new CustomAlert(title: "Sharing failed", subheader: "This file is already shared.").Show();
                    return;
                }

                if (!(RetrieveIsSharingDisabled(txtFieldShareToName.Text) == "0")) {
                    new CustomAlert(title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} disabled their file sharing.").Show();
                    return;
                }

                if (ReceiverHasAuthVerificaiton(txtFieldShareToName.Text) != "") {
                    new AskSharingAuthForm(txtFieldShareToName.Text, _fileName, _fileName.Substring(_fileName.Length-4)).Show();
                    return;
                }

                StartSharingFile();
                               
            }
            catch (Exception) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
