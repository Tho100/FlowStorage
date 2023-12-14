using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;

using FlowSERVER1.Sharing;
using FlowSERVER1.Helper;
using FlowSERVER1.Global;

namespace FlowSERVER1 {
    public partial class AskSharingAuthForm : Form {
        private string _receiverUsername { get; set; }
        private string _fileName { get; set; }
        private string _fileExtension { get; set; }
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private string _userComment { get; set; }

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private Crud crud = new Crud();
        readonly private GeneralCompressor compressor = new GeneralCompressor();

        public AskSharingAuthForm(String receiverUsername, String comment, String fileName) {
            InitializeComponent();
            this._receiverUsername = receiverUsername;
            this._fileName = fileName;
            this._userComment = comment;
            this._fileExtension = Path.GetExtension(fileName);
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
            txtFieldReceiverAuth.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            txtFieldReceiverAuth.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
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

        private async Task<string> GetFileMetadata(String fileName, String tableName) {

            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        private async Task<string> GetFileMetadataSharedToMe(String fileName) {

            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        private async Task<string> GetFileMetadataSharedToOthers(String fileName) {

            const string queryGetFileData = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename LIMIT 1;";

            using (MySqlCommand command = new MySqlCommand(queryGetFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        private async Task<string> GetFileMetadataExtra(String tableName, String columnName, String directoryName, String fileName) {

            string base64String = "";

            string queryGetFileByte = $"SELECT CUST_FILE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        base64String = readData.GetString(0);
                    }
                }
            }

            return base64String;
        }

        private async Task<string> RetrieveThumbnails(String tableName, String fileName) {

            string GetBase64String = "";

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
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

        private async Task<string> RetrieveThumbnailsExtra(String tableName, String columnName, String directoryName, String fileName) {

            string GetBase64String = "";

            string queryGetFileByte = $"SELECT CUST_THUMB FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND {columnName} = @dirname";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        GetBase64String = readData.GetString(0);
                    }
                }
            }

            return GetBase64String;
        }

        private async Task<string> RetrieveThumbnailShared(String fileName, String columnName) {

            string queryGetFileByte = $"SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = {columnName} AND CUST_FILE_PATH = @filename LIMIT 1;";
            using (MySqlCommand command = new MySqlCommand(queryGetFileByte, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                using (MySqlDataReader readData = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await readData.ReadAsync()) {
                        return readData.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>

        private async Task InsertFileData(string fileBase64EncodedData, string thumbnailBase64 = null) {

            string fileType = Path.GetExtension(_fileName);

            string encryptedFileName = EncryptionModel.Encrypt(_fileName);
            string encryptedComment = string.IsNullOrEmpty(_userComment)
                ? String.Empty : EncryptionModel.Encrypt(_userComment);

            string thumbnail = thumbnailBase64 ?? String.Empty;

            string todayDate = DateTime.Now.ToString("dd/MM/yyyy");

            byte[] toBytes = Convert.FromBase64String(fileBase64EncodedData);

            string compressedUpdatedData = Globals.imageTypes.Contains(fileType)
                ? fileBase64EncodedData
                : Convert.ToBase64String(compressor.compressFileData(toBytes));

            const string insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@receiver, @from, @file_name, @date, @file_data, @file_type, @thumbnail, @comment)";

            using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@from", Globals.custUsername);
                command.Parameters.AddWithValue("@receiver", _receiverUsername);
                command.Parameters.AddWithValue("@file_name", encryptedFileName);
                command.Parameters.AddWithValue("@date", todayDate);
                command.Parameters.AddWithValue("@file_data", compressedUpdatedData);
                command.Parameters.AddWithValue("@file_type", fileType);
                command.Parameters.AddWithValue("@comment", encryptedComment);
                command.Parameters.AddWithValue("@thumbnail", thumbnail);

                await command.ExecuteNonQueryAsync();
            }

        }

        private async Task StartSharingFile() {

            int receiverUploadLimit = Globals.uploadFileLimit[await crud.ReturnUserAccountType(_receiverUsername)];
            int receiverCurrentTotalUploaded = CountReceiverTotalShared(_receiverUsername);

            if (receiverUploadLimit != receiverCurrentTotalUploaded) {

                new Thread(() => new SharingAlert(shareToName: _receiverUsername).ShowDialog()).Start();

                if (_isFromShared == false && _tableName == GlobalsTable.sharingTable) {
                    string getThumbnails = await RetrieveThumbnailShared(_fileName, "CUST_TO");
                    await InsertFileData(await GetFileMetadataSharedToMe(_fileName), getThumbnails);
                    
                } else if (_tableName != GlobalsTable.directoryUploadTable && _tableName != GlobalsTable.folderUploadTable) {

                    if (Globals.imageTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psImage
                            ? GlobalsTable.psImage : GlobalsTable.homeImageTable;

                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (Globals.videoTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psVideo
                            ? GlobalsTable.psVideo : GlobalsTable.homeVideoTable;
                        string getThumbnails = await RetrieveThumbnails(finalTable, _fileName);

                        await InsertFileData(await GetFileMetadata(_fileName, finalTable), getThumbnails);

                    } else if (Globals.textTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psText
                            ? GlobalsTable.psText : GlobalsTable.homeTextTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (Globals.excelTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psExcel
                            ? GlobalsTable.psExcel : GlobalsTable.homeExcelTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    }
                    else if (_fileExtension == "pdf") {
                        string finalTable = _tableName == GlobalsTable.psPdf
                            ? GlobalsTable.psPdf : GlobalsTable.homePdfTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psPtx
                            ? GlobalsTable.psPtx : GlobalsTable.homePtxTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (Globals.wordTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psWord
                            ? GlobalsTable.psWord : GlobalsTable.homeWordTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (Globals.audioTypes.Contains(_fileExtension)) {
                        string finalTable = _tableName == GlobalsTable.psAudio
                            ? GlobalsTable.psAudio : GlobalsTable.homeAudioTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (_fileExtension == "exe") {
                        string finalTable = _tableName == GlobalsTable.psExe
                            ? GlobalsTable.psExe : GlobalsTable.homeExeTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    } else if (_fileExtension == "apk") {
                        string finalTable = _tableName == GlobalsTable.psApk
                            ? GlobalsTable.psApk : GlobalsTable.homeApkTable;
                        await InsertFileData(await GetFileMetadata(_fileName, finalTable));

                    }

                } else if (_isFromShared == true && _tableName == GlobalsTable.sharingTable) {
                    string getThumbnails = await RetrieveThumbnailShared(_fileName, "CUST_FROM");
                    await InsertFileData(await GetFileMetadataSharedToOthers(_fileName), getThumbnails);

                } else if (_tableName == GlobalsTable.directoryUploadTable) {
                    string getThumbnails = await RetrieveThumbnailsExtra(
                        GlobalsTable.directoryUploadTable, "DIR_NAME", _directoryName, _fileName);

                    await InsertFileData(await GetFileMetadataExtra(
                        GlobalsTable.directoryUploadTable, "DIR_NAME", _directoryName, _fileName), getThumbnails);

                } else if (_tableName == GlobalsTable.folderUploadTable) {
                    string getThumbnails = await RetrieveThumbnailsExtra(
                        GlobalsTable.folderUploadTable, "FOLDER_TITLE", _directoryName, _fileName);

                    await InsertFileData(await GetFileMetadataExtra(
                        GlobalsTable.folderUploadTable, "FOLDER_TITLE", _directoryName, _fileName), getThumbnails);
                }

                CloseForm.closeForm("SharingAlert");

                new SucessSharedAlert(_fileName, _receiverUsername).Show();

            }

        }

        private async Task<string> GetReceiverAuth() {

            const string query = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", _receiverUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }
        /// <summary>
        /// This button will start sharing file if 
        /// the entered password is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            var receiverAuthInput = txtFieldReceiverAuth.Text;
            var actualReceiverAuth = EncryptionModel.computeAuthCase(await GetReceiverAuth());
            
            if(EncryptionModel.computeAuthCase(receiverAuthInput) == actualReceiverAuth) {
                await StartSharingFile();

            } else {
                lblAlert.Visible = true;

            }

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
