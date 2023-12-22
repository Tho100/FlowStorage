using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.SharingQuery;
using FlowstorageDesktop.Temporary;
using Microsoft.WindowsAPICodePack.Shell;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class MainShareFileForm : Form {

        public MainShareFileForm instance;

        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private Crud crud = new Crud();

        readonly private UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();
        readonly private SharingOptionsQuery sharingOptions = new SharingOptionsQuery();
        readonly private ShareFileQuery shareFile = new ShareFileQuery();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string _fileName{ get; set; }
        private string _fileFullPath { get; set; }
        private string _fileExtension { get; set; }
        private string _currentFileName { get; set; }
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
        private async Task InsertFileData(String fileDataBase64, String thumbnailBase64 = null) {

            try {
                
                string fileType = Path.GetExtension(_fileName);

                string receiverUsername = txtFieldShareToName.Text;

                string encryptedFileName = EncryptionModel.Encrypt(_fileName);
                string encryptedComment = EncryptionModel.Encrypt(txtFieldComment.Text);

                string todayDate = DateTime.Now.ToString("dd/MM/yyyy");

                string compressedFileData = UniqueFile.IgnoreEncryption(fileType)
                    ? fileDataBase64
                    : EncryptionModel.Encrypt(fileDataBase64);

                const string query = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB,CUST_COMMENT) VALUES (@to,@from,@file_name,@date,@file_data,@file_type,@thumbnail,@comment)";

                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@to", receiverUsername);
                    command.Parameters.AddWithValue("@from", tempDataUser.Username);
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@date", todayDate);
                    command.Parameters.AddWithValue("@file_data", compressedFileData);
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
        private async Task StartSharingFile() {

            string shareToName = txtFieldShareToName.Text;

            int receiverUploadLimit = await userAuthQuery.GetUploadLimit(shareToName);
            int receiverCurrentTotalUploaded = await shareFile.CountReceiverTotalShared(txtFieldShareToName.Text);

            if (receiverUploadLimit != receiverCurrentTotalUploaded) {

                if (_currentFileName != txtFieldFileName.Text) {

                    _currentFileName = txtFieldFileName.Text;

                    StartPopupForm.StartSharingPopup(shareToName);

                    byte[] compressedBytes = new GeneralCompressor().compressFileData(_fileBytes);
                    string fileBase64Data = Convert.ToBase64String(compressedBytes);

                    if (Globals.imageTypes.Contains(_fileExtension)) {
                        string compressedImageBase64 = compressor.compressImageToBase64(_fileFullPath);
                        await InsertFileData(compressedImageBase64);

                    } else if (Globals.wordTypes.Contains(_fileExtension)) {
                        await InsertFileData(fileBase64Data);      
                        
                    } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                        await InsertFileData(fileBase64Data);

                    } else if (_fileExtension == ".exe") {
                        await InsertFileData(fileBase64Data);

                    } else if (_fileExtension == ".msi") {
                        await InsertFileData(fileBase64Data);

                    } else if (Globals.audioTypes.Contains(_fileExtension)) {
                        await InsertFileData(fileBase64Data);

                    } else if (_fileExtension == ".pdf") {
                        await InsertFileData(fileBase64Data);

                    } else if (_fileExtension == ".apk") {
                        await InsertFileData(fileBase64Data);

                    } else if (Globals.excelTypes.Contains(_fileExtension)) {
                        await InsertFileData(fileBase64Data);

                    } else if (Globals.textTypes.Contains(_fileExtension)) {

                        var nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_fileFullPath)) { 
                            nonLine = ReadFileTxt.ReadToEnd();  
                        }

                        byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                        byte[] compressedTextBytes = new GeneralCompressor().compressFileData(getBytes);

                        string encodedBase64 = Convert.ToBase64String(compressedTextBytes);
                        
                        await InsertFileData(encodedBase64);

                    } else if (Globals.videoTypes.Contains(_fileExtension)) {

                        ShellFile shellFile = ShellFile.FromFilePath(_fileFullPath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                        string toBase64Thumbnail;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            toBase64Thumbnail = Convert.ToBase64String(stream.ToArray());
                        }

                        await InsertFileData(fileBase64Data, toBase64Thumbnail);

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
        /// Button to start file sharing 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                string receiverUsername = txtFieldShareToName.Text;
                string fileName = txtFieldFileName.Text;

                if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(receiverUsername)) {
                    return;
                }

                if (receiverUsername == tempDataUser.Username) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: "You can't share to yourself.").Show();
                    return;
                }

                if (await sharingOptions.UserExistVerification(receiverUsername) == 0) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: $"The user {receiverUsername} does not exist.").Show();
                    return;
                }

                if (await shareFile.FileIsUploadedVerification(receiverUsername, fileName) > 0) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: "This file is already shared.").Show();
                    return;
                }

                if (!(await sharingOptions.RetrieveIsSharingDisabled(receiverUsername) == "0")) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: $"The user {receiverUsername} disabled their file sharing.").Show();
                    return;
                }

                if (await sharingOptions.ReceiverHasAuthVerification(receiverUsername) != "") {
                    new AskSharingAuthForm(
                        receiverUsername, txtFieldComment.Text, fileName).Show();
                    return;
                }

                await StartSharingFile();

            } catch (Exception) {
                MessageBox.Show(
                    "An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
