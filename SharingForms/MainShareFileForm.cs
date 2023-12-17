using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;

using FlowSERVER1.Sharing;
using FlowSERVER1.AlertForms;
using FlowSERVER1.Helper;
using FlowSERVER1.AuthenticationQuery;
using FlowSERVER1.SharingQuery;

namespace FlowSERVER1 {
    public partial class MainShareFileForm : Form {

        public MainShareFileForm instance;

        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private Crud crud = new Crud();
        readonly private SharingOptionsQuery sharingQuery = new SharingOptionsQuery();

        private string _fileName{ get; set; }
        private string _fileFullPath { get; set; }
        private string _fileExtension { get; set; }
        private byte[] _fileBytes { get; set; }

        readonly private MySqlConnection con = ConnectionModel.con;

        public string _verifySetPas = "";

        public MainShareFileForm() {
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

        private void guna2Button1_Click(object sender, EventArgs e) {

            var selectFilesDialog = new OpenFileDialog();

            selectFilesDialog.Filter = Globals.filterFileType;

            if (selectFilesDialog.ShowDialog() == DialogResult.OK) {

                string fileExtension = selectFilesDialog.FileName;
                string fileName = selectFilesDialog.SafeFileName;
                string retrieved = Path.GetExtension(fileExtension);

                _fileName = fileName;
                _fileFullPath = selectFilesDialog.FileName;
                _fileExtension = retrieved;

                txtFieldFileName.Text = fileName;

                _fileBytes = File.ReadAllBytes(_fileFullPath);

                lblFileSize.Text = $"File Size: {GetFileSize.fileSize(_fileBytes):F2}Mb";
                lblFileSize.Visible = true;
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
        private async Task startSending(String fileDataBase64, String thumbnailBase64 = null) {

            try {
                
                string receiverUsername = txtFieldShareToName.Text;

                string encryptedFileName = EncryptionModel.Encrypt(_fileName);
                string encryptedComment = EncryptionModel.Encrypt(txtFieldComment.Text);

                string todayDate = DateTime.Now.ToString("dd/MM/yyyy");

                const string query = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@to,@from,@file_name,@date,@file_data,@file_type,@thumbnail,@comment)";

                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@to", receiverUsername);
                    command.Parameters.AddWithValue("@from", Globals.custUsername);
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@date", todayDate);
                    command.Parameters.AddWithValue("@file_data", fileDataBase64);
                    command.Parameters.AddWithValue("@file_type", _fileExtension);
                    command.Parameters.AddWithValue("@comment", encryptedComment);
                    command.Parameters.AddWithValue("@thumbnail", thumbnailBase64);
                    command.Prepare();

                    await command.ExecuteNonQueryAsync();
                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Sharing failed", subheader: "Something went wrong.").Show();

            }
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        private async Task startSharing() {

            string shareToName = txtFieldShareToName.Text;

            int receiverUploadLimit = Globals.uploadFileLimit[await crud.ReturnUserAccountType(txtFieldShareToName.Text)];
            int receiverFilesCount = countReceiverShared(shareToName);

            if (receiverUploadLimit != receiverFilesCount) {

                if (_currentFileName != txtFieldFileName.Text) {

                    _currentFileName = txtFieldFileName.Text;

                    StartPopupForm.StartSharingPopup(shareToName);

                    byte[] _compressedBytes = new GeneralCompressor().compressFileData(_fileBytes);
                    string _toBase64 = Convert.ToBase64String(_compressedBytes);

                    if (Globals.imageTypes.Contains(_fileExtension)) {
                        string compressedImageBase64 = compressor.compressImageToBase64(_fileFullPath);
                        string encryptText = EncryptionModel.Encrypt(compressedImageBase64);
                        await startSending(encryptText);

                    } else if (Globals.wordTypes.Contains(_fileExtension)) {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);      
                        
                    } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (_fileExtension == ".exe") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (_fileExtension == ".msi") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (Globals.audioTypes.Contains(_fileExtension)) {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (_fileExtension == ".pdf") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (_fileExtension == ".apk") {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (Globals.excelTypes.Contains(_fileExtension)) {
                        string encryptText = EncryptionModel.Encrypt(_toBase64);
                        await startSending(encryptText);

                    } else if (Globals.textTypes.Contains(_fileExtension)) {

                        var nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_fileFullPath)) { 
                            nonLine = ReadFileTxt.ReadToEnd();  
                        }

                        byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                        byte[] compressedTextBytes = new GeneralCompressor().compressFileData(getBytes);

                        string getEncoded = Convert.ToBase64String(compressedTextBytes);
                        string encryptText = EncryptionModel.Encrypt(getEncoded);
                        
                        await startSending(encryptText);

                    } else if (Globals.videoTypes.Contains(_fileExtension)) {

                        ShellFile shellFile = ShellFile.FromFilePath(_fileFullPath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                        string toBase64Thumbnail;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            toBase64Thumbnail = Convert.ToBase64String(stream.ToArray());
                        }

                        string encryptText = EncryptionModel.Encrypt(_toBase64);

                        await startSending(encryptText,toBase64Thumbnail);
                    }

                    ClosePopupForm.CloseSharingPopup();

                    new SucessSharedAlert(shareToName).Show();

                } else {
                    new CustomAlert(
                        title: "Sharing failed", subheader: "This file is already sent.").Show();

                }

            } else {
                new CustomAlert(
                    title: "Sharing failed", subheader: "The receiver has reached the limit amount of files they can received.").Show();

            }
        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        private string hasPassword(String username) {

            string sharingPassword = "";
            string sharingIsDisabled = "";

            const string queryGetAuth = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(queryGetAuth,con)) {
                command.Parameters.AddWithValue("@username", username);
                sharingPassword = command.ExecuteScalarAsync().ToString();
            }

            const string queryGetStatus = "SELECT PASSWORD_DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(queryGetStatus, con)) {
                command.Parameters.AddWithValue("@username", username);
                sharingIsDisabled = command.ExecuteScalarAsync().ToString();
            }

            if (sharingPassword == "DEF" || sharingIsDisabled == "1") {
                sharingPassword = "";

            } else {
                return sharingPassword;

            }

            return sharingPassword;

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
        private async Task<string> retrieveDisabled(String receiverUsername) {

            const string querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@username", receiverUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetString(0);
                    }
                }
            }

            return "0";
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

                int userExists = await sharingQuery.UserExistVerification(textBox1);

                if (userExists == 0) {
                    MessageBox.Show($"User '{textBox1}' not found.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int fileUploadCount = await sharingQuery.FileUploadCount(textBox1, textBox2);

                if (fileUploadCount > 0) {
                    MessageBox.Show("This file is already shared.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string disabled = await retrieveDisabled(textBox1);
                if (disabled != "0") {
                    MessageBox.Show($"The user {textBox1} disabled their file sharing.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string password = hasPassword(textBox1);

                if (!string.IsNullOrEmpty(password)) {
                    AskSharingAuthForm _askPassForm = new AskSharingAuthForm(textBox1, textBox2, _fileExtension);
                    _askPassForm.Show();

                } else {
                    await startSharing();

                }

            } catch (Exception) {
                MessageBox.Show("An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

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
