using FlowSERVER1.AlertForms;
using FlowSERVER1.Authentication;
using FlowSERVER1.ExtraForms;
using FlowSERVER1.Global;
using FlowSERVER1.Helper;

using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {

    public partial class HomePage : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private Crud crud = new Crud();
        readonly private GeneralCompressor compressor = new GeneralCompressor();

        public static HomePage instance { get; set; } = new HomePage();
        public string PublicStorageUserComment { get; set; } = null;
        public string PublicStorageUserTitle { get; set; } = null;
        public string PublicStorageUserTag { get; set; } = null;
        public bool PublicStorageClosed { get; set; } = false;
        public string CurrentLang { get; set; }
        private string _fileName { get; set; }
        private string _fileExtension { get; set; }
        private long _fileSizeInMB { get; set; }
        private bool _isMyPublicStorageSelected { get; set; }
        private string _todayDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        private string previousSelectedItem = null;

        public HomePage() {

            InitializeComponent();

            instance = this;

            this.AllowDrop = true;

            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragOver += new DragEventHandler(Form1_DragOver);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.DragLeave += new EventHandler(Form1_DragLeave);

            var form4Instances = Application.OpenForms.OfType<Form>().Where(form => form.Name == "Form4").ToList();
            form4Instances.ForEach(form => form.Close());

            this.flwLayoutHome.HorizontalScroll.Maximum = 0;
            this.flwLayoutHome.VerticalScroll.Maximum = 0;
            this.flwLayoutHome.AutoScrollMinSize = new Size(0, 0);

            this.flwLayoutHome.BorderStyle = BorderStyle.None;

            this.flwLayoutHome.AutoScroll = true;
            this.flwLayoutHome.HorizontalScroll.Visible = false;
            this.flwLayoutHome.VerticalScroll.Visible = false;

            this.TopMost = false;

        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void guna2Button1_Click(object sender, EventArgs e) => new CreateDirectoryForm().Show();
        private void guna2Button7_Click(object sender, EventArgs e) => new MainShareFileForm().Show();
        private void guna2Button3_Click_1(object sender, EventArgs e)
            => new RenameFolderFileForm(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem)).Show();
        private void BuildShowAlert(String title, String subheader)
            => new CustomAlert(title: title, subheader: subheader).Show();

        private void UpdateProgressBarValue() {

            int getCurrentCount = int.Parse(lblItemCountText.Text);
            int getLimitedValue = int.Parse(lblLimitUploadText.Text);
            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
            lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

            progressBarUsageStorage.Value = calculatePercentageUsage;

        }

        private async Task InsertFileDataVideo(string selectedItems, string nameTable, string getNamePath, object keyValMain) {

            try {

                string encryptedFileName = EncryptionModel.Encrypt(getNamePath);
                string thumbnailCompressedBase64 = "";

                string insertQuery = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB) VALUES (@file_name, @username, @date, @file_value, @thumbnail_value)";
                using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@date", _todayDate);
                    command.Parameters.AddWithValue("@file_value", keyValMain);

                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                        var toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            string toBase64 = Convert.ToBase64String(stream.ToArray());
                            thumbnailCompressedBase64 = compressor.compressBase64Image(toBase64);
                            command.Parameters.AddWithValue("@thumbnail_value", thumbnailCompressedBase64);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

                if (lblCurrentPageText.Text == "Home") {
                    GlobalsData.base64EncodedThumbnailHome.Add(thumbnailCompressedBase64);

                }
                else if (lblCurrentPageText.Text == "Public Storage") {
                    GlobalsData.base64EncodedThumbnailPs.Add(thumbnailCompressedBase64);

                }

                CloseUploadAlert();
                UpdateProgressBarValue();

            } catch (Exception) {
                BuildShowAlert(title: "Upload failed", subheader: $"Failed to upload {_fileName}");
            }

        }

        private async Task InsertFileData(string fileDataBase64Encoded, string nameTable) {

            try {

                string encryptedFileName = EncryptionModel.Encrypt(_fileName);

                string insertQuery = $"INSERT INTO {nameTable} (CUST_USERNAME,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE) VALUES (@username, @file_name, @date, @file_value)";
                var param = new Dictionary<string, string>
                {
                    { "@username", Globals.custUsername},
                    { "@file_name", encryptedFileName},
                    { "@date", _todayDate},
                    { "@file_value", fileDataBase64Encoded}
                };

                await crud.Insert(insertQuery, param);

                CloseUploadAlert();
                UpdateProgressBarValue();

            } catch (Exception) {
                BuildShowAlert(title: "Upload failed", subheader: $"Failed to upload {_fileName}");
            }

        }

        private async Task InsertFileVideoDataPublic(string selectedItems, string fileName, object keyValMain) {

            try {

                string encryptedFileName = EncryptionModel.Encrypt(fileName);
                string encryptedComment = EncryptionModel.Encrypt(PublicStorageUserComment);

                string thumbnailCompressedBase64 = "";

                string insertQuery = $"INSERT INTO ps_info_video (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, CUST_TITLE, CUST_TAG) VALUES (@file_name, @username, @date, @file_value, @thumbnail_value, @title, @tag)";
                using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@date", _todayDate);
                    command.Parameters.AddWithValue("@file_value", keyValMain);
                    command.Parameters.AddWithValue("@title", PublicStorageUserTitle);
                    command.Parameters.AddWithValue("@tag", PublicStorageUserTag);

                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                        var toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            thumbnailCompressedBase64 = Convert.ToBase64String(stream.ToArray());

                            command.Parameters.AddWithValue("@thumbnail_value", thumbnailCompressedBase64);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

                string insertQueryComment = $"INSERT INTO ps_info_comment (CUST_FILE_NAME, CUST_COMMENT) VALUES (@file_name, @comment)";
                var paramComment = new Dictionary<string, string>
                {
                    { "@file_name", encryptedFileName},
                    { "@comment", encryptedComment},
                };

                await crud.Insert(insertQueryComment, paramComment);

                GlobalsData.base64EncodedThumbnailPs.Add(thumbnailCompressedBase64);

                CloseUploadAlert();
                UpdateProgressBarValue();

                PublicStorageUserComment = null;

            } catch (Exception) {
                BuildShowAlert(title: "Upload failed", subheader: $"Failed to upload {_fileName}");
            }

        }

        public async Task InsertFileDataPublic(string fileBase64Value, string nameTable) {

            try {

                string encryptedComment = EncryptionModel.Encrypt(PublicStorageUserComment);
                string encryptedFileName = EncryptionModel.Encrypt(_fileName);

                string insertQuery = $"INSERT INTO {nameTable} (CUST_USERNAME,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE, CUST_TITLE, CUST_TAG) VALUES (@username, @file_name, @date, @file_value, @title, @tag)";
                var param = new Dictionary<string, string>
                {
                    { "@username", Globals.custUsername},
                    { "@file_name", encryptedFileName},
                    { "@date", _todayDate},
                    { "@file_value", fileBase64Value},
                    { "@title", PublicStorageUserTitle},
                    { "@tag", PublicStorageUserTag},
                };

                await crud.Insert(insertQuery, param);

                string insertQueryComment = $"INSERT INTO ps_info_comment (CUST_FILE_NAME, CUST_COMMENT) VALUES (@file_name, @comment)";
                var paramComment = new Dictionary<string, string>
                {
                    { "@file_name", encryptedFileName},
                    { "@comment", encryptedComment},
                };

                await crud.Insert(insertQueryComment, paramComment);

                CloseUploadAlert();
                UpdateProgressBarValue();

                PublicStorageUserComment = null;

            } catch (Exception) {
                BuildShowAlert(title: "Upload failed", subheader: $"Failed to upload {_fileName}");
            }

        }

        private void BuildButtonsOnHomePageSelected() {
            btnDownloadFolder.Visible = false;
            btnDeleteFolder.Visible = false;
            btnOpenRenameFolderPage.Visible = false;
        }

        private void BuildButtonOnSharedFilesSelected() {
            btnRefreshFiles.Visible = true;
            btnOpenRenameFolderPage.Visible = false;
            btnDeleteFolder.Visible = false;
            btnDownloadFolder.Visible = false;
        }

        private void BuildButtonsOnSharedToMeSelected() {
            btnRefreshFiles.Visible = true;
            btnDownloadFolder.Visible = false;
            btnDeleteFolder.Visible = false;
            btnOpenRenameFolderPage.Visible = false;
        }

        private void BuildButtonsOnFolderNameSelected() {
            btnDeleteFolder.Visible = true;
            btnRefreshFiles.Visible = false;
            btnOpenRenameFolderPage.Visible = true;
            pnlSubPanelDetails.Visible = false;
            btnDownloadFolder.Visible = true;
        }

        private void CloseRetrievalAlert() {
            var retrievalAlertForm = Application.OpenForms
            .OfType<Form>()
            .FirstOrDefault(form => form.Name == "RetrievalAlert");
            retrievalAlertForm?.Close();
        }

        private void CloseUploadAlert() {
            Application.OpenForms
            .OfType<Form>()
            .Where(form => String.Equals(form.Name, "UploadingAlert"))
            .ToList()
            .ForEach(form => form.Close());
        }

        private void BuildRedundaneVisibility() {
            if (flwLayoutHome.Controls.Count == 0) {
                ShowRedundane();

            } else {
                ClearRedundane();

            }
        }

        /// <summary>
        /// (Un-Displayed when flowlayout is not empty
        /// </summary>
        private void ClearRedundane() {
            btnGarbageImage.Visible = false;
            lblEmptyHere.Visible = false;
        }

        /// <summary>
        /// (Displayed when flowlayout is empty)
        /// Show "It's empty here" text and garbage image
        /// </summary>
        private void ShowRedundane() {
            btnGarbageImage.Visible = true;
            lblEmptyHere.Visible = true;
        }

        /// <summary>
        /// This function will shows alert form that tells the user to upgrade 
        /// their account when the total amount of files upload is exceeding
        /// the amount of file they can upload 
        /// </summary>
        /// <param name="CurAcc"></param>
        public void DisplayUpgradeAccountDialog() {
            Form bgBlur = new Form();
            using (UpgradeAccountAlert displayPic = new UpgradeAccountAlert()) {
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.FormBorderStyle = FormBorderStyle.None;
                bgBlur.Opacity = .24d;
                bgBlur.BackColor = Color.Black;
                bgBlur.WindowState = FormWindowState.Maximized;
                bgBlur.TopMost = true;
                bgBlur.Location = this.Location;
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.ShowInTaskbar = false;
                bgBlur.Show();

                displayPic.Owner = bgBlur;
                displayPic.ShowDialog();

                bgBlur.Dispose();
            }
        }

        /// <summary>
        /// 
        /// This function will opens a file dialog and 
        /// generate panel for selected file
        /// 
        /// </summary>

        int curr = 0;
        int txtCurr = 0;
        int exeCurr = 0;
        int vidCurr = 0;
        int exlCurr = 0;
        int audCurr = 0;
        int apkCurr = 0;
        int pdfCurr = 0;
        int ptxCurr = 0;
        int msiCurr = 0;
        int docxCurr = 0;

        #region Home section

        /// <summary>
        /// 
        /// Get user Home files metadata: file name, upload date
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private async Task<List<(string, string, string)>> GetFileMetadataHome(string tableName) {

            if (GlobalsData.filesMetadataCacheHome.ContainsKey(tableName)) {
                return GlobalsData.filesMetadataCacheHome[tableName];

            } else {
                string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM {tableName} WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        List<(string, string, string)> filesInfo = new List<(string, string, string)>();
                        while (await reader.ReadAsync()) {
                            string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);
                            filesInfo.Add((fileName, uploadDate, string.Empty));
                        }

                        GlobalsData.filesMetadataCacheHome[tableName] = filesInfo;
                        return filesInfo;
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// Generate user Home files panel on startup
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>
        /// <returns></returns>
        private async Task BuildFilePanelHome(String tableName, String parameterName, int currItem) {

            List<Image> imageValues = new List<Image>();
            List<EventHandler> onPressedEvent = new List<EventHandler>();
            List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

            try {

                List<(string, string, string)> filesInfo = await GetFileMetadataHome(tableName);

                if (tableName == GlobalsTable.homeImageTable) {

                    if (GlobalsData.base64EncodedImageHome.Count == 0) {

                        const string retrieveImgQuery = "SELECT CUST_FILE FROM file_info_image WHERE CUST_USERNAME = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                                    GlobalsData.base64EncodedImageHome.Add(base64String);
                                }
                            }
                        }
                    }
                }

                if (tableName == GlobalsTable.homeVideoTable) {

                    if (GlobalsData.base64EncodedThumbnailHome.Count == 0) {

                        const string retrieveImgQuery = "SELECT CUST_THUMB FROM file_info_video WHERE CUST_USERNAME = @username";
                        using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);

                            using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    GlobalsData.base64EncodedThumbnailHome.Add(readBase64.GetString(0));
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < currItem; i++) {

                    int accessIndex = i;

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {
                        lblFileNameOnPanel.Text = filesInfo[accessIndex].Item1;
                        lblFileTableName.Text = tableName;
                        lblFilePanelName.Text = parameterName + accessIndex;
                        pnlFileOptions.Visible = true;
                    }

                    onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                    if (tableName == GlobalsTable.homeImageTable) {

                        if (GlobalsData.base64EncodedImageHome.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedImageHome[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image setImageStream = Image.FromStream(toMs);
                                imageValues.Add(setImageStream);
                            }
                        }

                        void imageOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.homeImageTable, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }


                    if (tableName == GlobalsTable.homeTextTable) {

                        var getExtension = filesInfo[i].Item1.Substring(filesInfo[i].Item1.LastIndexOf('.')).TrimStart();
                        var textTypeToImage = Globals.textTypeToImage[getExtension];
                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {

                            TextForm displayPic = new TextForm(GlobalsTable.homeTextTable, filesInfo[accessIndex].Item1, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.homeExeTable) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfo[accessIndex].Item1, GlobalsTable.homeExeTable, String.Empty, Globals.custUsername);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (tableName == GlobalsTable.homeVideoTable) {

                        if (GlobalsData.base64EncodedThumbnailHome.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedThumbnailHome[i]);
                            using (var toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }
                        }

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.homeVideoTable, String.Empty, Globals.custUsername);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (tableName == GlobalsTable.homeExcelTable) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfo[accessIndex].Item1, GlobalsTable.homeExcelTable, String.Empty, Globals.custUsername);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (tableName == GlobalsTable.homeAudioTable) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfo[accessIndex].Item1, GlobalsTable.homeAudioTable, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (tableName == GlobalsTable.homeApkTable) {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfo[accessIndex].Item1, Globals.custUsername, GlobalsTable.homeApkTable, String.Empty);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (tableName == GlobalsTable.homePdfTable) {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfo[accessIndex].Item1, GlobalsTable.homePdfTable, String.Empty, Globals.custUsername);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (tableName == GlobalsTable.homePtxTable) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfo[accessIndex].Item1, GlobalsTable.homePtxTable, String.Empty, Globals.custUsername);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (tableName == GlobalsTable.homeMsiTable) {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfo[accessIndex].Item1, GlobalsTable.homeMsiTable, String.Empty, Globals.custUsername);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (tableName == GlobalsTable.homeWordTable) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfo[accessIndex].Item1, GlobalsTable.homeWordTable, String.Empty, Globals.custUsername);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel(parameterName, currItem, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                if(int.Parse(lblUsagePercentage.Text.Replace("%", "")) >= 70 && int.Parse(lblUsagePercentage.Text.Replace("%", "")) < 75) {
                    pnlExceedStorage.Visible = true;

                } else {
                    pnlExceedStorage.Visible = false;

                }

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }

        }

        private async Task CreateFilePanelHome(string fileFullPath, string tableName, string parameterName, int itemCurr, string keyVal) {

            if (_fileSizeInMB < 1500) {

                var panelTxt = new Guna2Panel() {
                    Name = parameterName + itemCurr,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;
                flwLayoutHome.Controls.Add(panelTxt);
                var mainPanelTxt = panelTxt;

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = "TxtBox" + itemCurr;
                textboxPic.BorderRadius = 8;
                textboxPic.Width = 190;
                textboxPic.Height = 145;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                textboxPic.Anchor = AnchorStyles.None;

                int textboxPic_x = (mainPanelTxt.Width - textboxPic.Width) / 2;
                textboxPic.Location = new Point(textboxPic_x, 10);

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = "LabTxtUp" + itemCurr;
                dateLabTxt.Font = GlobalStyle.DateLabelFont;
                dateLabTxt.ForeColor = GlobalStyle.DarkGrayColor;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = GlobalStyle.DateLabelLoc;
                dateLabTxt.Text = _todayDate;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = "LabVidUp" + itemCurr;
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.Width = 160;
                titleLab.Height = 20;
                titleLab.AutoEllipsis = true;
                titleLab.Text = _fileName;

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = "RemTxtBut" + itemCurr;
                remButTxt.Width = 29;
                remButTxt.Height = 26;
                remButTxt.ImageOffset = GlobalStyle.GarbageOffset;
                remButTxt.FillColor = GlobalStyle.TransparentColor;
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = GlobalStyle.TransparentColor;
                remButTxt.Image = GlobalStyle.GarbageImage;
                remButTxt.Visible = true;
                remButTxt.Location = GlobalStyle.GarbageButtonLoc;
                remButTxt.BringToFront();

                remButTxt.Click += (sender_tx, e_tx) => {
                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFileTableName.Text = tableName;
                    lblFilePanelName.Text = mainPanelTxt.Name;
                    pnlFileOptions.Visible = true;
                };

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                new Thread(() => new UploadingAlert(_fileName, String.Empty, parameterName + itemCurr, String.Empty, fileSize: _fileSizeInMB).ShowDialog())
                    .Start();

                if (tableName == GlobalsTable.homeImageTable) {

                    await InsertFileData(keyVal, tableName);

                    GlobalsData.base64EncodedImageHome.Add(EncryptionModel.Decrypt(keyVal));

                    textboxPic.Image = new Bitmap(fileFullPath);
                    textboxPic.Click += (sender_f, e_f) => {

                        var getImgName = (Guna2PictureBox)sender_f;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;

                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, _fileName, GlobalsTable.homeImageTable, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };

                }

                if (tableName == GlobalsTable.homeTextTable) {

                    string textType = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    textboxPic.Image = Globals.textTypeToImage[textType];

                    await InsertFileData(keyVal, tableName);

                    var filePath = _fileName;

                    textboxPic.Click += (sender_t, e_t) => {

                        TextForm txtFormShow = new TextForm(GlobalsTable.homeTextTable, filePath, String.Empty, Globals.custUsername);
                        txtFormShow.Show();
                    };
                }

                if (tableName == GlobalsTable.homeExeTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.EXEImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        exeFORM displayExe = new exeFORM(titleLab.Text, GlobalsTable.homeExeTable, String.Empty, Globals.custUsername);
                        displayExe.Show();
                    };
                }

                if (tableName == GlobalsTable.homeVideoTable) {

                    await InsertFileDataVideo(fileFullPath, tableName, _fileName, keyVal);

                    ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                    textboxPic.Image = toBitMap;

                    textboxPic.Click += (sender_ex, e_ex) => {
                        var getImgName = (Guna2PictureBox)sender_ex;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                        VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, titleLab.Text, GlobalsTable.homeVideoTable, String.Empty, Globals.custUsername);
                        vidShow.Show();
                    };
                }
                if (tableName == GlobalsTable.homeAudioTable) {

                    await InsertFileData(keyVal, tableName);

                    var _getWidth = this.Width;
                    var _getHeight = this.Height;
                    textboxPic.Image = Globals.AudioImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.homeAudioTable, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.homeExcelTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.EXCELImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.homeExcelTable, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.homeApkTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.APKImage;
                    textboxPic.Click += (sender_gi, e_gi) => {
                        ApkForm displayPic = new ApkForm(titleLab.Text, Globals.custUsername, GlobalsTable.homeApkTable, String.Empty);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.homePdfTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.PDFImage;
                    textboxPic.Click += (sender_pd, e_pd) => {
                        PdfForm displayPdf = new PdfForm(titleLab.Text, GlobalsTable.homePdfTable, String.Empty, Globals.custUsername);
                        displayPdf.ShowDialog();
                    };
                }

                if (tableName == GlobalsTable.homePtxTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.PTXImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        PtxForm displayPtx = new PtxForm(titleLab.Text, GlobalsTable.homePtxTable, String.Empty, Globals.custUsername);
                        displayPtx.ShowDialog();
                    };
                }
                if (tableName == GlobalsTable.homeMsiTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.MSIImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        MsiForm displayMsi = new MsiForm(titleLab.Text, GlobalsTable.homeMsiTable, String.Empty, Globals.custUsername);
                        displayMsi.Show();
                    };
                }

                if (tableName == GlobalsTable.homeWordTable) {

                    await InsertFileData(keyVal, tableName);

                    textboxPic.Image = Globals.DOCImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        WordDocForm displayWord = new WordDocForm(titleLab.Text, GlobalsTable.homeWordTable, String.Empty, Globals.custUsername);
                        displayWord.ShowDialog();
                    };
                }

            } else {
                MessageBox.Show("File is too large, max file size is 1.5GB.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private async void OpenDialogHomeFile() {

            var open = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = true
            };

            int curFilesCount = flwLayoutHome.Controls.Count;
            if (open.ShowDialog() == DialogResult.OK) {

                if (open.FileNames.Length + curFilesCount > Globals.uploadFileLimit[Globals.accountType]) {
                    DisplayUpgradeAccountDialog();

                } else {

                    HashSet<string> fileNameLabels = new HashSet<string>(flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                    GlobalsData.filesMetadataCacheHome.Clear();

                    foreach (var selectedItems in open.FileNames) {

                        string selectedFileName = Path.GetFileName(selectedItems);
                        string fileType = selectedFileName.Split('.').Last();

                        if (fileNameLabels.Contains(selectedFileName.ToLower().Trim())) {
                            continue;
                        }

                        _fileName = Path.GetFileName(selectedItems);
                        _fileExtension = Path.GetExtension(selectedItems);
                        _fileSizeInMB = 0;

                        try {

                            byte[] originalRetrieveBytes = File.ReadAllBytes(selectedItems);
                            byte[] compressedBytes = new GeneralCompressor().compressFileData(originalRetrieveBytes);

                            string convertToBase64 = Convert.ToBase64String(compressedBytes);
                            string encryptText = UniqueFile.IgnoreEncryption(_fileExtension) ? convertToBase64 : EncryptionModel.Encrypt(convertToBase64);

                            _fileSizeInMB = (originalRetrieveBytes.Length / 1024) / 1024;

                            if (Globals.imageTypes.Contains(_fileExtension)) {
                                curr++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                string compressedImage = compressor.compressImageToBase64(selectedItems);
                                string encryptedValue = EncryptionModel.Encrypt(compressedImage);

                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, encryptedValue);
                                
                            } else if (Globals.textTypes.Contains(_fileExtension)) {
                                txtCurr++;

                                string nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }

                                byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                                byte[] compressedTextBytes = compressor.compressFileData(getBytes);
                                string getEncoded = Convert.ToBase64String(compressedTextBytes);
                                string encryptEncodedText = EncryptionModel.Encrypt(getEncoded);

                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeTextTable, "PanTxt", txtCurr, encryptEncodedText);

                            } else if (_fileExtension == ".exe") {
                                exeCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeExeTable, "PanExe", exeCurr, encryptText);

                            } else if (Globals.videoTypes.Contains(_fileExtension)) {
                                vidCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptText);

                            } else if (Globals.excelTypes.Contains(_fileExtension)) {
                                exlCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptText);

                            } else if (Globals.audioTypes.Contains(_fileExtension)) {
                                audCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptText);

                            } else if (_fileExtension == ".apk") {
                                apkCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptText);

                            } else if (_fileExtension == ".pdf") {
                                pdfCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptText);

                            } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                                ptxCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptText);

                            } else if (_fileExtension == ".msi") {
                                msiCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptText);

                            } else if (Globals.wordTypes.Contains(_fileExtension)) {
                                docxCurr++;
                                await CreateFilePanelHome(
                                    selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptText);

                            } else {
                                UnknownTypeAlert unsupportedFileFormartForm = new UnknownTypeAlert(_fileName);
                                unsupportedFileFormartForm.Show();

                            }

                            CloseUploadAlert();

                        } catch (Exception) {
                            CloseUploadAlert();
                        }

                        lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();
                    }
                }

            }

            BuildRedundaneVisibility();
            CloseUploadAlert();

        }

        private async Task BuildHomeFiles() {

            foreach (string tableName in GlobalsTable.publicTables) {
                if (GlobalsTable.tableToFileType.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileType[tableName];
                    if (fileType != null) {
                        ClearRedundane();
                        await BuildFilePanelHome(tableName, fileType, await crud.CountUserTableRow(tableName));

                    } else {
                        await buildDirectoryPanel(await crud.CountUserTableRow(tableName));

                    }
                }
            }

            BuildRedundaneVisibility();

        }

        private async Task RefreshHomePanels() {

            btnDeleteFolder.Visible = false;

            GlobalsData.filesMetadataCacheHome.Clear();

            foreach (string tableName in GlobalsTable.publicTables) {
                if (GlobalsTable.tableToFileType.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileType[tableName];
                    if (fileType != null) {
                        ClearRedundane();
                        await BuildFilePanelHome(tableName, fileType, await crud.CountUserTableRow(tableName));

                    } else {
                        await buildDirectoryPanel(await crud.CountUserTableRow(tableName));

                    }
                }
            }

            BuildRedundaneVisibility();
            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        #endregion END - Home section

        #region Public Storage section
        private async Task<List<(string, string, string, string)>> GetFileMetadataPublicStorage(string tableName) {

            if (GlobalsData.filesMetadataCachePs.ContainsKey(tableName)) {
                return GlobalsData.filesMetadataCachePs[tableName];

            } else {

                string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE, CUST_TAG, CUST_TITLE FROM {tableName}";
                using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        List<(string, string, string, string)> filesInfo = new List<(string, string, string, string)>();
                        while (await reader.ReadAsync()) {
                            string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);
                            string tagValue = reader.GetString(2);
                            string titleValue = reader.GetString(3);
                            filesInfo.Add((fileName, uploadDate, tagValue, titleValue));
                        }

                        GlobalsData.filesMetadataCachePs[tableName] = filesInfo;
                        return filesInfo;
                    }
                }
            }

        }

        private async Task BuildFilePanelPublicStorage(String tableName, String parameterName, int currItem, bool isFromMyPs = false) {

            List<Image> imageValues = new List<Image>();
            List<EventHandler> onPressedEvent = new List<EventHandler>();
            List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

            try {

                List<(string, string, string, string)> filesInfo;

                if (isFromMyPs == false) {
                    filesInfo = await GetFileMetadataPublicStorage(tableName);

                } else {
                    filesInfo = new List<(string, string, string, string)>();
                }

                if (isFromMyPs == true) {
                    GlobalsData.base64EncodedImagePs.Clear();
                    GlobalsData.base64EncodedThumbnailPs.Clear();

                }

                if (isFromMyPs) {

                    string selectFileDataQuery = $"SELECT CUST_FILE_PATH, UPLOAD_DATE, CUST_TAG, CUST_TITLE FROM {tableName} WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(selectFileDataQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            List<(string, string, string, string)> tuplesList = new List<(string, string, string, string)>();
                            while (await reader.ReadAsync()) {
                                string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                                string uploadDate = reader.GetString(1);
                                string tagValue = reader.GetString(2);
                                string titleValue = reader.GetString(3);
                                tuplesList.Add((fileName, uploadDate, tagValue, titleValue));
                            }
                            filesInfo.AddRange(tuplesList);
                        }
                    }
                }

                List<string> usernameList = new List<string>();

                string selectUploaderNameQuery = null;

                if (isFromMyPs == false) {

                    selectUploaderNameQuery = $"SELECT CUST_USERNAME FROM {tableName}";
                    using (MySqlCommand command = new MySqlCommand(selectUploaderNameQuery, con)) {
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                usernameList.Add(reader.GetString(0));
                            }
                        }
                    }

                } else {

                    selectUploaderNameQuery = $"SELECT CUST_USERNAME FROM {tableName} WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(selectUploaderNameQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                usernameList.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                if (tableName == GlobalsTable.psImage) {

                    if (GlobalsData.base64EncodedImagePs.Count == 0) {

                        if (isFromMyPs == false) {

                            const string retrieveImagesQuery = "SELECT CUST_FILE FROM ps_info_image";
                            using (MySqlCommand command = new MySqlCommand(retrieveImagesQuery, con)) {
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                                        GlobalsData.base64EncodedImagePs.Add(base64String);
                                    }
                                }
                            }

                        } else {

                            const string retrieveImagesQuery = "SELECT CUST_FILE FROM ps_info_image WHERE CUST_USERNAME = @username";
                            using (MySqlCommand command = new MySqlCommand(retrieveImagesQuery, con)) {
                                command.Parameters.AddWithValue("@username", Globals.custUsername);
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                                        GlobalsData.base64EncodedImagePs.Add(base64String);
                                    }
                                }
                            }

                        }
                    }
                }

                if (tableName == GlobalsTable.psVideo) {

                    if (GlobalsData.base64EncodedThumbnailPs.Count == 0) {

                        if (isFromMyPs == false) {

                            const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM ps_info_video";
                            using (var command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        GlobalsData.base64EncodedThumbnailPs.Add(readBase64.GetString(0));
                                    }
                                }
                            }

                        } else {

                            const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM ps_info_video WHERE CUST_USERNAME = @username";
                            using (var command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                                command.Parameters.AddWithValue("@username", Globals.custUsername);
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        GlobalsData.base64EncodedThumbnailPs.Add(readBase64.GetString(0));
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < currItem; i++) {

                    int accessIndex = i;
                    string uploaderName = usernameList[accessIndex];

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {
                        lblFileNameOnPanel.Text = filesInfo[accessIndex].Item1;
                        lblFileTableName.Text = tableName;
                        lblFilePanelName.Text = parameterName + accessIndex;
                        pnlFileOptions.Visible = true;
                    }

                    onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                    if (tableName == GlobalsTable.psImage) {

                        if (GlobalsData.base64EncodedImagePs.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedImagePs[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image setImageStream = Image.FromStream(toMs);
                                imageValues.Add(setImageStream);
                            }
                        }

                        void imageOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psImage, String.Empty, uploaderName);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }


                    if (tableName == GlobalsTable.psText) {

                        var getExtension = filesInfo[i].Item1.Substring(filesInfo[i].Item1.LastIndexOf('.')).TrimStart();
                        var textTypeToImage = Globals.textTypeToImage[getExtension];
                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.psText, filesInfo[accessIndex].Item1, String.Empty, uploaderName);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.psExe) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfo[accessIndex].Item1, GlobalsTable.psExe, String.Empty, uploaderName);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (tableName == GlobalsTable.psVideo) {

                        if (GlobalsData.base64EncodedThumbnailPs.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedThumbnailPs[i]);
                            using (var toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }
                        }

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psVideo, String.Empty, uploaderName);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (tableName == GlobalsTable.psExcel) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfo[accessIndex].Item1, GlobalsTable.psExcel, String.Empty, uploaderName);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (tableName == GlobalsTable.psAudio) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfo[accessIndex].Item1, GlobalsTable.psAudio, String.Empty, uploaderName);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (tableName == GlobalsTable.psApk) {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfo[accessIndex].Item1, uploaderName, GlobalsTable.psApk, String.Empty);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (tableName == GlobalsTable.psPdf) {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfo[accessIndex].Item1, GlobalsTable.psPdf, String.Empty, uploaderName);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (tableName == GlobalsTable.psPtx) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfo[accessIndex].Item1, GlobalsTable.psPtx, String.Empty, uploaderName);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (tableName == GlobalsTable.psMsi) {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfo[accessIndex].Item1, GlobalsTable.psMsi, String.Empty, uploaderName);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (tableName == GlobalsTable.psWord) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfo[accessIndex].Item1, GlobalsTable.psWord, String.Empty, uploaderName);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePublicStoragePanel(parameterName, currItem, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues, usernameList, moreButtonVisible: isFromMyPs);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }            

        }

        private async Task CreateFilePanelPublicStorage(string fileFullPath, string tableName, string parameterName, int itemCurr, string keyVal) {

            if (_fileSizeInMB < 1500) {

                var panelTxt = new Guna2Panel() {
                    Name = parameterName + itemCurr,
                    Width = 280,
                    Height = 268,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;
                flwLayoutHome.Controls.Add(panelTxt);
                var mainPanelTxt = panelTxt;

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = "TxtBox" + itemCurr;
                textboxPic.BorderRadius = 8;
                textboxPic.Width = 270;
                textboxPic.Height = 165;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                textboxPic.Anchor = AnchorStyles.None;

                int textboxPic_x = (mainPanelTxt.Width - textboxPic.Width) / 2;
                textboxPic.Location = new Point(textboxPic_x, 10);

                Label dateLab = new Label();
                mainPanelTxt.Controls.Add(dateLab);
                dateLab.Name = "LabTxtUp" + itemCurr;
                dateLab.BackColor = GlobalStyle.TransparentColor;
                dateLab.Font = GlobalStyle.DateLabelFont;
                dateLab.ForeColor = GlobalStyle.DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = new Point(12, 241);
                dateLab.Text = _todayDate;

                Guna2Panel tagBackground = new Guna2Panel();
                mainPanelTxt.Controls.Add(tagBackground);
                tagBackground.BorderRadius = 11;
                tagBackground.Location = new Point(12, 188);
                tagBackground.Size = new Size(108, 24);
                tagBackground.FillColor = GlobalStyle.psBackgroundColorTag[PublicStorageUserTag];
                tagBackground.BringToFront();

                Label tagLabel = new Label();
                mainPanelTxt.Controls.Add(tagLabel);
                tagLabel.Name = $"ButTag{itemCurr}";
                tagLabel.Font = GlobalStyle.PsLabelTagFont;
                tagLabel.Height = 15;
                tagLabel.Width = 85;
                tagLabel.BackColor = GlobalStyle.psBackgroundColorTag[PublicStorageUserTag];
                tagLabel.ForeColor = GlobalStyle.GainsboroColor;
                tagLabel.Visible = true;
                tagLabel.TextAlign = ContentAlignment.MiddleCenter;

                int centerX = (tagBackground.Width - tagLabel.Width) / 2;
                tagLabel.Location = new Point(centerX + 15, GlobalStyle.PsLabelTagLoc.Y+1);

                tagLabel.Text = PublicStorageUserTag;
                tagLabel.BringToFront();

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = "LabVidUp" + itemCurr;
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 218);
                titleLab.Width = 200;
                titleLab.Height = 20;
                titleLab.AutoEllipsis = true;
                titleLab.Text = _fileName;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                new Thread(() => new UploadingAlert(_fileName, String.Empty, parameterName + itemCurr, String.Empty, fileSize: _fileSizeInMB).ShowDialog())
                .Start();

                if (tableName == GlobalsTable.psImage) {

                    await InsertFileDataPublic(keyVal, tableName);

                    GlobalsData.base64EncodedImagePs.Add(EncryptionModel.Decrypt(keyVal));

                    textboxPic.Image = new Bitmap(fileFullPath);
                    textboxPic.Click += (sender_f, e_f) => {

                        var getImgName = (Guna2PictureBox)sender_f;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;

                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, _fileName, GlobalsTable.psImage, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };

                }

                if (tableName == GlobalsTable.psText) {

                    string textType = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    textboxPic.Image = Globals.textTypeToImage[textType];

                    await InsertFileDataPublic(keyVal, tableName);

                    var filePath = _fileName;

                    textboxPic.Click += (sender_t, e_t) => {

                        TextForm txtFormShow = new TextForm(GlobalsTable.psText, filePath, String.Empty, Globals.custUsername);
                        txtFormShow.Show();
                    };
                }

                if (tableName == GlobalsTable.psExe) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.EXEImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        exeFORM displayExe = new exeFORM(titleLab.Text, GlobalsTable.psExe, String.Empty, Globals.custUsername);
                        displayExe.Show();
                    };
                }

                if (tableName == GlobalsTable.psVideo) {

                    await InsertFileVideoDataPublic(fileFullPath, _fileName, keyVal);

                    ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                    textboxPic.Image = toBitMap;

                    textboxPic.Click += (sender_ex, e_ex) => {
                        var getImgName = (Guna2PictureBox)sender_ex;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                        VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, titleLab.Text, GlobalsTable.psVideo, String.Empty, Globals.custUsername);
                        vidShow.Show();
                    };
                }

                if (tableName == GlobalsTable.psAudio) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.AudioImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.psAudio, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.psExcel) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.EXCELImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.psExcel, String.Empty, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.psApk) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.APKImage;
                    textboxPic.Click += (sender_gi, e_gi) => {
                        ApkForm displayPic = new ApkForm(titleLab.Text, Globals.custUsername, GlobalsTable.psApk, String.Empty);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.psPdf) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.PDFImage;
                    textboxPic.Click += (sender_pd, e_pd) => {
                        PdfForm displayPdf = new PdfForm(titleLab.Text, GlobalsTable.psPdf, String.Empty, Globals.custUsername);
                        displayPdf.ShowDialog();
                    };
                }

                if (tableName == GlobalsTable.psPtx) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.PTXImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        PtxForm displayPtx = new PtxForm(titleLab.Text, GlobalsTable.psPtx, String.Empty, Globals.custUsername);
                        displayPtx.ShowDialog();
                    };
                }
                if (tableName == GlobalsTable.psMsi) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.MSIImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        MsiForm displayMsi = new MsiForm(titleLab.Text, GlobalsTable.psMsi, String.Empty, Globals.custUsername);
                        displayMsi.Show();
                    };
                }

                if (tableName == GlobalsTable.psWord) {

                    await InsertFileDataPublic(keyVal, tableName);

                    textboxPic.Image = Globals.DOCImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        WordDocForm displayWord = new WordDocForm(titleLab.Text, GlobalsTable.psWord, String.Empty, Globals.custUsername);
                        displayWord.ShowDialog();
                    };
                }

            } else {
                MessageBox.Show("File is too large, max file size is 1.5GB.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private async void OpenDialogPublicStorage() {

            var open = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = false
            };

            int curFilesCount = flwLayoutHome.Controls.Count;
            if (open.ShowDialog() == DialogResult.OK) {

                if (open.FileNames.Length + curFilesCount > Globals.uploadFileLimit[Globals.accountType]) {
                    DisplayUpgradeAccountDialog();

                } else {

                    HashSet<string> fileNameLabels = new HashSet<string>(flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                    string selectedItems = open.FileName;
                    string selectedFileName = Path.GetFileName(selectedItems);

                    if (fileNameLabels.Contains(selectedFileName.ToLower().Trim())) {
                        BuildShowAlert(title: "Upload Failed", $"A file with the same name is already uploaded to Public Storage. File name: {selectedFileName}");
                        return;
                    }

                    GlobalsData.filesMetadataCachePs.Clear();

                    _fileName = Path.GetFileName(selectedItems);
                    _fileExtension = Path.GetExtension(selectedItems);
                    _fileSizeInMB = 0;

                    try {

                        new PublishPublicStorage(fileName: _fileName).ShowDialog();

                        if (PublicStorageClosed == false) {

                            byte[] retrieveBytes = File.ReadAllBytes(selectedItems);
                            byte[] compressedBytes = new GeneralCompressor().compressFileData(retrieveBytes);

                            string toBase64String = Convert.ToBase64String(compressedBytes);
                            string encryptText = UniqueFile.IgnoreEncryption(_fileExtension) ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                            _fileSizeInMB = (retrieveBytes.Length / 1024) / 1024;

                            if (Globals.imageTypes.Contains(_fileExtension)) {
                                curr++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                string compressedImage = compressor.compressImageToBase64(selectedItems);
                                string encryptedImage = EncryptionModel.Encrypt(compressedImage);
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psImage, "PanImg", curr, encryptedImage);
       
                            } else if (Globals.textTypes.Contains(_fileExtension)) {
                                txtCurr++;

                                string nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }

                                byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                                string getEncoded = Convert.ToBase64String(getBytes);
                                string encryptTextValues = EncryptionModel.Encrypt(getEncoded);
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psText, "PanTxt", txtCurr, encryptTextValues);

                            } else if (_fileExtension == ".exe") {
                                exeCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psExe, "PanExe", exeCurr, encryptText);

                            } else if (Globals.videoTypes.Contains(_fileExtension)) {
                                vidCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psVideo, "PanVid", vidCurr, encryptText);

                            } else if (Globals.excelTypes.Contains(_fileExtension)) {
                                exlCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psExcel, "PanExl", exlCurr, encryptText);

                            } else if (Globals.audioTypes.Contains(_fileExtension)) {
                                audCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psAudio, "PanAud", audCurr, encryptText);

                            } else if (_fileExtension == ".apk") {
                                apkCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psApk, "PanApk", apkCurr, encryptText);

                            } else if (_fileExtension == ".pdf") {
                                pdfCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psPdf, "PanPdf", pdfCurr, encryptText);

                            } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                                ptxCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psPtx, "PanPtx", ptxCurr, encryptText);

                            } else if (_fileExtension == ".msi") {
                                msiCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psMsi, "PanMsi", msiCurr, encryptText);

                            } else if (Globals.wordTypes.Contains(_fileExtension)) {
                                docxCurr++;
                                await CreateFilePanelPublicStorage(
                                    selectedItems, GlobalsTable.psWord, "PanDoc", docxCurr, encryptText);

                            } else {
                                UnknownTypeAlert unsupportedFileFormartForm = new UnknownTypeAlert(_fileName);
                                unsupportedFileFormartForm.Show();

                            }

                            CloseUploadAlert();

                        }

                    } catch (Exception) {
                        CloseUploadAlert();
                    }

                    lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                }
            }

            PublicStorageClosed = false;

            BuildRedundaneVisibility();
            CloseUploadAlert();

        }

        private async Task BuildPublicStorageFiles() {

            foreach (string tableName in GlobalsTable.publicTablesPs) {
                if (GlobalsTable.tableToFileTypePs.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileTypePs[tableName];
                    if (fileType != null) {

                        ClearRedundane();

                        await BuildFilePanelPublicStorage(tableName, fileType, await crud.CountTableRow(tableName), isFromMyPs: false);
                    }
                }
            }

            lblIPsILimitedText.Text = Globals.uploadFileLimit[Globals.accountType].ToString();

            List<string> username = new List<string>(flwLayoutHome.Controls
                .OfType<Guna2Panel>()
                .SelectMany(panel => panel.Controls.OfType<Label>())
                .Where(label => label.Text.Contains(Globals.custUsername))
                .Select(label => label.Text.Split('·')[0].Trim().ToLower()));

            lblIPsItemCountText.Text = username.Count.ToString();

            int getCurrentCount = int.Parse(lblIPsItemCountText.Text);
            int getLimitedValue = int.Parse(lblIPsILimitedText.Text);
            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
            lblUsagePercentagePs.Text = calculatePercentageUsage.ToString() + "%";

            progressBarPs.Value = calculatePercentageUsage;

            BuildRedundaneVisibility();

        }
        private async Task BuildMyPublicStorageFiles() {

            GlobalsData.base64EncodedImagePs.Clear();
            GlobalsData.base64EncodedThumbnailPs.Clear();

            foreach (string tableName in GlobalsTable.publicTablesPs) {
                if (GlobalsTable.tableToFileTypePs.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileTypePs[tableName];
                    if (fileType != null) {

                        ClearRedundane();

                        await BuildFilePanelPublicStorage(tableName, fileType, await crud.CountUserTableRow(tableName), isFromMyPs: true);
                    }
                }
            }

            BuildRedundaneVisibility();

        }

        private async Task RefreshPublicStoragePanels() {

            GlobalsData.filesMetadataCachePs.Clear();

            foreach (string tableName in GlobalsTable.publicTablesPs) {
                if (GlobalsTable.tableToFileTypePs.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileTypePs[tableName];
                    if (fileType != null) {

                        ClearRedundane();

                        await BuildFilePanelPublicStorage(tableName, fileType, await crud.CountTableRow(tableName), isFromMyPs: false);
                    }
                }
            }

            _isMyPublicStorageSelected = true;
            BuildRedundaneVisibility();

        }

        #endregion END - Public Storage section 

        #region Shared to others section

        /// <summary>
        /// Retrieve username of file that has been shared to
        /// </summary>
        /// <returns></returns>
        private string UploaderName() {

            const string selectUploaderName = "SELECT CUST_FROM FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(selectUploaderName, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        List<(string, string, string)> filesInfoSharedOthers = new List<(string, string, string)>();
        private async Task CallFilesInformationOthers() {

            filesInfoSharedOthers.Clear();
            const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_FROM = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfoSharedOthers.Add((fileName, uploadDate, String.Empty));
                    }
                }
            }

        }

        private async Task BuildFilePanelSharedToOthers(List<String> fileTypes, String parameterName, int itemCurr) {

            try {

                List<Image> imageValues = new List<Image>();
                List<EventHandler> onPressedEvent = new List<EventHandler>();
                List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

                List<string> typeValues = new List<string>(fileTypes);
                List<string> uploadToNameList = new List<string>();

                const string selectUploadToName = "SELECT CUST_TO FROM cust_sharing WHERE CUST_FROM = @username";
                using (MySqlCommand command = new MySqlCommand(selectUploadToName, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            uploadToNameList.Add(reader.GetString(0));
                        }
                    }
                }

                if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {

                    if (GlobalsData.base64EncodedImageSharedOthers.Count == 0) {

                        const string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);

                            using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    GlobalsData.base64EncodedImageSharedOthers.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                                }
                                readBase64.Close();
                            }
                        }
                    }
                }

                for (int i = 0; i < itemCurr; i++) {

                    int accessIndex = i;
                    string uploadToName = uploadToNameList[accessIndex];

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {

                        lblFileNameOnPanel.Text = filesInfoSharedOthers[accessIndex].Item1;
                        lblFileTableName.Text = GlobalsTable.sharingTable;
                        lblSharedToName.Text = uploadToName;
                        lblFilePanelName.Text = parameterName + accessIndex;
                        pnlFileOptions.Visible = true;
                    }

                    onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);


                    if (Globals.imageTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedImageSharedOthers.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedImageSharedOthers[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }
                        }

                        void imageOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.textTypeToImage[typeValues[i]]);

                        void textOnPressed(object sender, EventArgs e) {

                            TextForm displayTxt = new TextForm(GlobalsTable.sharingTable, filesInfoSharedOthers[accessIndex].Item1, lblGreetingText.Text, uploadToName, true);
                            displayTxt.Show();

                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (typeValues[i] == ".exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.homeExeTable, lblGreetingText.Text, uploadToName, true);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedThumbnailSharedOthers.Count == 0) {

                            const string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_FROM = @username AND FILE_EXT = @ext";
                            using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                                command.Parameters.AddWithValue("@username", Globals.custUsername);
                                command.Parameters.AddWithValue("@ext", typeValues[i]);

                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        GlobalsData.base64EncodedThumbnailSharedOthers.Add(readBase64.GetString(0));
                                    }
                                    readBase64.Close();
                                }
                            }
                        }

                        if (GlobalsData.base64EncodedThumbnailSharedOthers.Count > 0) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedThumbnailSharedOthers[0]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }

                        }

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == ".apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfoSharedOthers[accessIndex].Item1, uploadToName, GlobalsTable.sharingTable, lblGreetingText.Text, true);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == ".pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == ".msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel(parameterName, itemCurr, filesInfoSharedOthers, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }

        }

        private async Task BuildSharedToOthers() {

            if (!GlobalsData.fileTypeValuesSharedToOthers.Any()) {
                const string getFilesTypeOthers = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
                using (var command = new MySqlCommand(getFilesTypeOthers, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (var readTypeOthers = await command.ExecuteReaderAsync()) {
                        while (await readTypeOthers.ReadAsync()) {
                            GlobalsData.fileTypeValuesSharedToOthers.Add(readTypeOthers.GetString(0));
                        }
                    }
                }
            }

            await BuildFilePanelSharedToOthers(GlobalsData.fileTypeValuesSharedToOthers, "DirParOther", GlobalsData.fileTypeValuesSharedToOthers.Count);
            BuildRedundaneVisibility();

        }

        /// <summary>
        /// 
        /// Refresh Shared To Others panel
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="typeValuesOthers"></param>
        /// <param name="dirName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task RefreshGenerateUserSharedOthers(List<string> typeValuesOthers, string dirName) {

            GlobalsData.base64EncodedImageSharedOthers.Clear();
            GlobalsData.base64EncodedThumbnailSharedOthers.Clear();

            if (typeValuesOthers.Count == 0) {
                using (MySqlCommand command = con.CreateCommand()) {
                    command.CommandText = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    using (MySqlDataReader _readType = command.ExecuteReader()) {
                        while (_readType.Read()) {
                            typeValuesOthers.Add(_readType.GetString(0));
                        }
                    }
                }
            }

            await CallFilesInformationOthers();
            await BuildFilePanelSharedToOthers(typeValuesOthers, dirName, typeValuesOthers.Count);

        }

        #endregion END - Shared to others

        #region Shared to me section

        List<(string, string, string)> filesInfoShared = new List<(string, string, string)>();
        private async Task CallFilesInformationShared() {

            filesInfoShared.Clear();

            const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfoShared.Add((fileName, uploadDate, String.Empty));
                    }
                }
            }

        }

        private async Task BuildFilePanelSharedToMe(List<String> extTypes, String parameterName, int itemCurr) {

            try {

                List<Image> imageValues = new List<Image>();
                List<EventHandler> onPressedEvent = new List<EventHandler>();
                List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

                var UploaderUsername = UploaderName();

                List<string> typeValues = new List<string>(extTypes);

                if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {

                    if (GlobalsData.base64EncodedImageSharedToMe.Count == 0) {

                        const string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    GlobalsData.base64EncodedImageSharedToMe.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                                }
                                readBase64.Close();
                            }
                        }
                    }
                }

                for (int i = 0; i < itemCurr; i++) {

                    int accessIndex = i;

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {

                        lblFileNameOnPanel.Text = filesInfoShared[accessIndex].Item1;
                        lblFileTableName.Text = GlobalsTable.sharingTable;
                        lblFilePanelName.Text = parameterName + accessIndex;
                        pnlFileOptions.Visible = true;
                    }

                    onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                    if (Globals.imageTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedImageSharedToMe.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedImageSharedToMe[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }
                        }

                        void imageOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.textTypeToImage[typeValues[i]]);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.sharingTable, filesInfoShared[accessIndex].Item1, lblGreetingText.Text, UploaderUsername, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (typeValues[i] == ".exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedThumbnailSharedToMe.Count == 0) {

                            const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                            using (MySqlCommand command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                                command.Parameters.AddWithValue("@username", Globals.custUsername);
                                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(filesInfoShared[i].Item1));

                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        GlobalsData.base64EncodedThumbnailSharedToMe.Add(readBase64.GetString(0));
                                    }
                                }
                            }
                        }

                        if (GlobalsData.base64EncodedThumbnailSharedToMe.Count > 0) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedThumbnailSharedToMe[0]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }
                        }

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == ".apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfoShared[accessIndex].Item1, UploaderUsername, GlobalsTable.sharingTable, lblGreetingText.Text, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == ".pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == ".msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfoShared[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, UploaderUsername, false);
                            displayMsi.Show();
                        }
                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel(parameterName, itemCurr, filesInfoShared, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }
        }

        private async Task BuildSharedToMe() {

            if (!GlobalsData.fileTypeValuesSharedToMe.Any()) {
                const string getFilesTypeQuery = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
                using (MySqlCommand command = new MySqlCommand(getFilesTypeQuery, ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            GlobalsData.fileTypeValuesSharedToMe.Add(reader.GetString(0));
                        }
                    }
                }
                await BuildFilePanelSharedToMe(GlobalsData.fileTypeValuesSharedToMe, "DirParMe", GlobalsData.fileTypeValuesSharedToMe.Count);

            } else {
                await BuildFilePanelSharedToMe(GlobalsData.fileTypeValuesSharedToMe, "DirParMe", GlobalsData.fileTypeValuesSharedToMe.Count);

            }

            BuildRedundaneVisibility();

        }

        /// <summary>
        /// 
        /// Refresh Shared To Me panel
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="typeValues"></param>
        /// <param name="dirName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task RefreshGenerateUserShared(List<string> typeValues, string dirName) {

            GlobalsData.base64EncodedImageSharedToMe.Clear();
            GlobalsData.base64EncodedThumbnailSharedToMe.Clear();

            if (typeValues.Count == 0) {
                using (MySqlCommand command = con.CreateCommand()) {
                    command.CommandText = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    using (MySqlDataReader _readType = command.ExecuteReader()) {
                        while (_readType.Read()) {
                            typeValues.Add(_readType.GetString(0));
                        }
                    }
                }
            }

            await CallFilesInformationShared();

            await BuildFilePanelSharedToMe(typeValues, dirName, typeValues.Count);
        }

        #endregion END - Shared to me

        #region Folder section

        /// <summary>
        /// 
        /// Generate user folders files on folder selection 
        /// 
        /// </summary>
        /// <param name="_fileType">File type of the files</param>
        /// <param name="_foldTitle">Folder title</param>
        /// <param name="parameterName">Custom parameter name for panel</param>
        /// <param name="currItem"></param>

        private async Task InsertFileDataFolder(String filesFullPath, String folderName, String fileDataBase64, String thumbnailValue = null) {

            const string insertFoldQue = "INSERT INTO folder_upload_info(FOLDER_TITLE,CUST_USERNAME,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH,CUST_THUMB) VALUES (@FOLDER_TITLE,@CUST_USERNAME,@CUST_FILE,@FILE_TYPE,@UPLOAD_DATE,@CUST_FILE_PATH,@CUST_THUMB)";
            using (var command = new MySqlCommand(insertFoldQue, con)) {
                command.Parameters.AddWithValue("@FOLDER_TITLE", EncryptionModel.Encrypt(folderName));
                command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                command.Parameters.AddWithValue("@FILE_TYPE", filesFullPath.Substring(filesFullPath.LastIndexOf('.') + 1));
                command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(Path.GetFileName(filesFullPath)));
                command.Parameters.AddWithValue("@CUST_FILE", fileDataBase64);
                command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);

                await command.ExecuteNonQueryAsync();
            }

            foreach (var form in Application.OpenForms.OfType<Form>().Where(form => form.Name == "UploadingAlert")) {
                form.Close();
            }

        }

        private void OpenFolderDialog() {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {

                var getFolderPath = dialog.FileName;
                var getFolderName = new DirectoryInfo(getFolderPath).Name;

                if (!lstFoldersPage.Items.Contains(getFolderName)) {

                    string[] folderFilesName = Directory.GetFiles(getFolderPath, "*").Select(Path.GetFileName).ToArray();
                    int numberOfFiles = Directory.GetFiles(getFolderPath, "*", SearchOption.AllDirectories).Length;

                    if (numberOfFiles <= Globals.uploadFileLimit[Globals.accountType]) {

                        flwLayoutHome.Controls.Clear();
                        lstFoldersPage.Items.Add(getFolderName);

                        CreateFilePanelFolder(getFolderPath, getFolderName, folderFilesName);
                        var folderListboxPosition = lstFoldersPage.Items.IndexOf(getFolderName);

                        lstFoldersPage.SelectedIndex = folderListboxPosition;

                    } else {
                        DisplayErrorFolder(Globals.accountType);
                        lstFoldersPage.SelectedItem = "Home";
                    }

                } else {
                    MessageBox.Show("Folder already exists", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void OpenFolderDownloadDialog(string folderTitle, List<(string fileName, byte[] fileBytes)> files) {

            CloseRetrievalAlert();

            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), EncryptionModel.Decrypt(folderTitle));
            Directory.CreateDirectory(folderPath);

            foreach (var (fileName, fileBytes) in files) {
                var filePath = Path.Combine(folderPath, $"{fileName}");
                File.WriteAllBytes(filePath, fileBytes);
            }

            Process.Start(folderPath);
        }

        private async Task DownloadUserFolder(string folderTitle) {

            var files = new List<(string fileName, byte[] fileBytes)>();

            using (var command = new MySqlCommand($"SELECT CUST_FILE_PATH, CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle", con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@foldtitle", folderTitle);
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        var fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        var base64Encoded = EncryptionModel.Decrypt(reader.GetString(1));
                        var fileBytes = Convert.FromBase64String(base64Encoded);
                        files.Add((fileName, fileBytes));
                    }
                }
            }

            OpenFolderDownloadDialog(folderTitle, files);
        }

        private async Task RefreshFolder() {

            GlobalsData.base64EncodedImageFolder.Clear();

            string _selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            var typesValues = new List<string>();

            const string getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
            using (var command = new MySqlCommand(getFileType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(_selectedFolder));
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        typesValues.Add(reader.GetString(0));
                    }
                }
            }

            var currMainLength = typesValues.Count;
            await BuildFilePanelFolder(typesValues, _selectedFolder, currMainLength);

        }

        private async Task BuildFilePanelFolder(List<String> fileType, String foldTitle, int currItem) {

            ClearRedundane();
            CloseUploadAlert();

            new Thread(() => new RetrievalAlert("Flowstorage is retrieving your folder files.", "Loader").ShowDialog()).Start();

            flwLayoutHome.Controls.Clear();

            try {

                List<string> originalTypeData = new List<string>(fileType);
                List<string> typeValues = originalTypeData.Select(f => "." + f).ToList();

                List<Image> imageValues = new List<Image>();
                List<EventHandler> onPressedEvent = new List<EventHandler>();
                List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

                List<(string, string, string)> filesInfo = new List<(string, string, string)>();
                HashSet<string> filePaths = new HashSet<string>();

                const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldname";
                using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@foldname", EncryptionModel.Encrypt(foldTitle));
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {

                            string filePath = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);

                            if (filePaths.Contains(filePath)) {
                                continue;
                            }

                            filePaths.Add(filePath);
                            filesInfo.Add((filePath, uploadDate, String.Empty));
                        }
                        reader.Close();
                    }
                }

                if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {

                    if (lstFoldersPage.SelectedItems.Count > 0) {

                        string selectedItem = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                        if (!(selectedItem == previousSelectedItem)) {

                            previousSelectedItem = selectedItem;

                            GlobalsData.base64EncodedImageFolder.Clear();
                            GlobalsData.base64EncodedThumbnailFolder.Clear();
                        }
                    }

                    if (GlobalsData.base64EncodedImageFolder.Count == 0) {

                        const string retrieveImgQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(foldTitle));
                            using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                                    GlobalsData.base64EncodedImageFolder.Add(base64String);
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < currItem; i++) {

                    int accessIndex = i;
                    string fileName = filesInfo[accessIndex].Item1;

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {
                        lblFileNameOnPanel.Text = fileName;
                        lblFileTableName.Text = GlobalsTable.folderUploadTable;
                        lblSelectedDirName.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                        lblFilePanelName.Text = "folderParameter" + accessIndex;
                        pnlFileOptions.Visible = true;
                    }

                    onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                    if (Globals.imageTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedImageFolder.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedImageFolder[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image setImageStream = Image.FromStream(toMs);
                                imageValues.Add(setImageStream);
                            }
                        }

                        void imageOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }


                    if (Globals.textTypes.Contains(typeValues[i])) {

                        var getExtension = fileName.Substring(fileName.LastIndexOf('.')).TrimStart();
                        var textTypeToImage = Globals.textTypeToImageFolder[getExtension];
                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.folderUploadTable, fileName, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (typeValues[i] == "exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(fileName, GlobalsTable.folderUploadTable, foldTitle, String.Empty);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedThumbnailFolder.Count == 0) {

                            const string retrieveThumbnailQuery = "SELECT CUST_THUMB FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND FILE_TYPE = @ext";
                            using (MySqlCommand command = new MySqlCommand(retrieveThumbnailQuery, con)) {
                                command.Parameters.AddWithValue("@username", Globals.custUsername);
                                command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(foldTitle));
                                command.Parameters.AddWithValue("@ext", originalTypeData[i]);
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        GlobalsData.base64EncodedThumbnailFolder.Add(readBase64.GetString(0));
                                    }
                                }
                            }
                        }

                        if (GlobalsData.base64EncodedThumbnailFolder.Count > 0) {

                            byte[] getBytes = Convert.FromBase64String(GlobalsData.base64EncodedThumbnailFolder[0]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                imageValues.Add(Image.FromStream(toMs));
                            }

                        }

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm displayVid = new VideoForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername);
                            displayVid.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            new ExcelForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            new AudioForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == "apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            new ApkForm(fileName, Globals.custUsername, GlobalsTable.folderUploadTable, foldTitle);
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == "pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            new PdfForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            new WordDocForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == "msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            new MsiForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            new WordDocForm(fileName, GlobalsTable.folderUploadTable, foldTitle, Globals.custUsername).Show();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel("folderParameter", currItem, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                CloseForm.closeForm("RetrievalAlert");

            } catch (Exception) {
                CloseForm.closeForm("RetrievalAlert");
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }
        }

        private async void CreateFilePanelFolder(String getFolderPath, String getFolderName, String[] filesName) {

            string _selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            int _IntCurr = 0;
            long _fileSizeInMB = 0;

            new Thread(() => new UploadingAlert(getFolderName, GlobalsTable.folderUploadTable, "PanExlFold" + _IntCurr, getFolderName, fileSize: _fileSizeInMB)
            .ShowDialog()).Start();

            GlobalsData.base64EncodedImageFolder.Clear();
            GlobalsData.base64EncodedThumbnailFolder.Clear();

            foreach (var filesFullPath in Directory.EnumerateFiles(getFolderPath, "*")) {

                _IntCurr++;

                var panelVid = new Guna2Panel() {
                    Name = $"PanExlFold{_IntCurr}",
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;
                flwLayoutHome.Controls.Add(panelVid);
                var mainPanelTxt = panelVid;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{_IntCurr}";
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.AutoEllipsis = true;
                titleLab.Width = 160;
                titleLab.Height = 20;
                titleLab.Text = filesName[_IntCurr - 1];

                var textboxExl = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxExl);
                textboxExl.Name = $"ExeExlFold{_IntCurr}";
                textboxExl.Width = 190;
                textboxExl.Height = 145;
                textboxExl.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxExl.BorderRadius = 8;
                textboxExl.Enabled = true;
                textboxExl.Visible = true;

                textboxExl.Anchor = AnchorStyles.None;

                int picMain_Q_x = (mainPanelTxt.Width - textboxExl.Width) / 2;

                textboxExl.Location = new Point(picMain_Q_x, 10);

                textboxExl.Click += (sender_w, ev_w) => {

                };

                textboxExl.MouseHover += (_senderM, _ev) => {
                    mainPanelTxt.ShadowDecoration.Enabled = true;
                    mainPanelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxExl.MouseLeave += (_senderQ, _evQ) => {
                    mainPanelTxt.ShadowDecoration.Enabled = false;
                };

                textboxExl.Click += (sender_eq, e_eq) => {

                };

                Guna2Button remButExl = new Guna2Button();
                mainPanelTxt.Controls.Add(remButExl);
                remButExl.Name = $"RemExlButFold{_IntCurr}";
                remButExl.Width = 29;
                remButExl.Height = 26;
                remButExl.ImageOffset = GlobalStyle.GarbageOffset;
                remButExl.BorderColor = GlobalStyle.TransparentColor;
                remButExl.FillColor = GlobalStyle.TransparentColor;
                remButExl.BorderRadius = 6;
                remButExl.BorderThickness = 1;
                remButExl.BorderColor = GlobalStyle.BorderColor2;
                remButExl.Image = GlobalStyle.GarbageImage;
                remButExl.Visible = true;
                remButExl.Location = GlobalStyle.GarbageButtonLoc;

                remButExl.Click += (sender_vid, e_vid) => {

                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFileTableName.Text = GlobalsTable.folderUploadTable;
                    lblFilePanelName.Text = mainPanelTxt.Name;
                    lblSelectedDirName.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                    pnlFileOptions.Visible = true;

                };

                Label dateLabExl = new Label();
                mainPanelTxt.Controls.Add(dateLabExl);
                dateLabExl.Name = $"LabExlUpFold{_IntCurr}";
                dateLabExl.Font = GlobalStyle.DateLabelFont;
                dateLabExl.ForeColor = GlobalStyle.DarkGrayColor;
                dateLabExl.Visible = true;
                dateLabExl.Enabled = true;
                dateLabExl.Location = GlobalStyle.DateLabelLoc;
                dateLabExl.Text = _todayDate;

                lblEmptyHere.Visible = false;
                btnGarbageImage.Visible = false;

                var _extTypes = Path.GetExtension(filesFullPath);

                try {

                    byte[] retrieveBytes = File.ReadAllBytes(filesFullPath);
                    byte[] compressedBytes = new GeneralCompressor().compressFileData(retrieveBytes);

                    string toBase64String = Convert.ToBase64String(compressedBytes);
                    string encryptValues = UniqueFile.IgnoreEncryption(_extTypes) ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                    _fileSizeInMB = (retrieveBytes.Length / 1024) / 1024;

                    if (Globals.imageTypes.Contains(_extTypes)) {

                        var image = new Bitmap(filesFullPath);

                        string compressImage = compressor.compressImageToBase64(filesFullPath);
                        string compressedImageToBase64 = EncryptionModel.Encrypt(compressImage);
                        await InsertFileDataFolder(filesFullPath, getFolderName, compressedImageToBase64);

                        textboxExl.Image = image;
                        textboxExl.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, titleLab.Text, GlobalsTable.folderUploadTable, String.Empty, Globals.custUsername);
                            displayPic.Show();

                        };
                    }

                    if (Globals.textTypes.Contains(_extTypes)) {

                        textboxExl.Image = Globals.textTypeToImageFolder[_extTypes];

                        string nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(filesFullPath)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptEncoded = EncryptionModel.Encrypt(getEncoded);

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptEncoded);

                        textboxExl.Click += (sender_t, e_t) => {

                            TextForm displayPic = new TextForm(GlobalsTable.folderUploadTable, titleLab.Text, String.Empty, Globals.custUsername);
                            displayPic.Show();
                        };

                    }

                    if (_extTypes == ".apk") {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.APKImage;
                        textboxExl.Click += (sender_ap, e_ap) => {
                            ApkForm displayPic = new ApkForm(titleLab.Text, Globals.custUsername, GlobalsTable.folderUploadTable, String.Empty);
                            displayPic.ShowDialog();
                        };
                    }
                    if (Globals.videoTypes.Contains(_extTypes)) {

                        ShellFile shellFile = ShellFile.FromFilePath(filesFullPath);

                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        string compressedThumbnail;

                        using (var stream = new MemoryStream()) {
                            shellFile.Thumbnail.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            string toBase64BitmapThumbnail = Convert.ToBase64String(stream.ToArray());
                            compressedThumbnail = compressor.compressBase64Image(toBase64BitmapThumbnail);
                        }

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues, compressedThumbnail);

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {

                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm displayVid = new VideoForm(defaultImage, getWidth, getHeight, titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".pdf") {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.PDFImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            PdfForm displayPic = new PdfForm(titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.wordTypes.Contains(_fileExtension)) {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            WordDocForm displayPic = new WordDocForm(titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.excelTypes.Contains(_fileExtension)) {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }


                    if (Globals.ptxTypes.Contains(_fileExtension)) {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.PTXImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            PtxForm displayPic = new PtxForm(titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.audioTypes.Contains(_fileExtension)) {

                        await InsertFileDataFolder(filesFullPath, getFolderName, encryptValues);

                        textboxExl.Image = Globals.AudioImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.folderUploadTable, _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                } catch (Exception) { }

            }

            var uploadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadingAlert");
            uploadAlertForm?.Close();

            btnShowFolderPage.FillColor = Color.FromArgb(255, 71, 19, 191);
            btnGoHomePage.FillColor = Color.Transparent;
            pnlMain.SendToBack();
            pnlFolders.BringToFront();
            lblMyFolders.Visible = true;
            lstFoldersPage.Visible = true;

            ClearRedundane();
            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        /// This function will delete user folder if 
        /// Garbage (delete folder) button is clicked
        /// </summary>
        /// <param name="foldName"></param>
        private async void RemoveAndDeleteFolder(String foldName) {

            DialogResult verifyDeletion = MessageBox.Show($"Delete {foldName} folder?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (verifyDeletion == DialogResult.Yes) {

                const string removeFoldQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                using (MySqlCommand command = new MySqlCommand(removeFoldQue, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(foldName));
                    await command.ExecuteNonQueryAsync();
                }

                lstFoldersPage.Items.Remove(foldName);

                int indexSelected = lstFoldersPage.Items.IndexOf("Home");
                lstFoldersPage.SelectedIndex = indexSelected;

                CloseRetrievalAlert();

            }
        }

        /// <summary>
        /// Function to display alert form if the 
        /// number of user folder files exceeding the amount of file 
        /// they can upload
        /// </summary>
        /// <param name="CurAcc"></param>
        public void DisplayErrorFolder(String CurAcc) {
            Form bgBlur = new Form();
            using (LimitedFolderFileAlert displayPic = new LimitedFolderFileAlert(CurAcc)) {
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.FormBorderStyle = FormBorderStyle.None;
                bgBlur.Opacity = .24d;
                bgBlur.BackColor = Color.Black;
                bgBlur.WindowState = FormWindowState.Maximized;
                bgBlur.TopMost = true;
                bgBlur.Location = this.Location;
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.ShowInTaskbar = false;
                bgBlur.Show();

                displayPic.Owner = bgBlur;
                displayPic.ShowDialog();

                bgBlur.Dispose();
            }
        }

        #endregion END - Folder section

        /// <summary>
        /// 
        /// Mini Garbage button which should remove 
        /// user folder based on the currently selected item in 
        /// listBox
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region Open dialog section (Home, Public Storage, Folder)

        /// <summary>
        /// Select user account type and show file dialog to upload file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUploadFileHome_Click(object sender, EventArgs e) {

            try {

                string selectedPageTab = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                if (selectedPageTab == "Home") {

                    int currentUploadCount = Convert.ToInt32(lblItemCountText.Text);

                    if (currentUploadCount != Globals.uploadFileLimit[Globals.accountType]) {
                        OpenDialogHomeFile();

                    } else {
                        DisplayUpgradeAccountDialog();

                    }

                } else {
                    MessageBox.Show("You can only upload a file on Home folder.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", subheader: "Something went wrong while trying to upload files.");
            }

        }

        private async void btnUploadPs_Click_1(object sender, EventArgs e) {

            try {

                List<int> returnedCountValue = new List<int>();

                foreach (string tableName in GlobalsTable.publicTablesPs) {
                    int count = await crud.CountUserTableRow(tableName);
                    returnedCountValue.Add(count);
                }

                int currentUploadCount = returnedCountValue.Sum();

                if (currentUploadCount != Globals.uploadFileLimit[Globals.accountType]) {
                    OpenDialogPublicStorage();

                } else {
                    DisplayUpgradeAccountDialog();

                }

                returnedCountValue.Clear();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", subheader: "Something went wrong while trying to upload files.");
            }

        }

        /// <summary>
        /// 
        /// Open FileDialog and ask user
        /// to select file and send those file 
        /// metadata into DB and generate panel file on folderDialog.
        /// 
        /// If condition is met then alert user to upgrade their account.
        /// The condition; Trying to upload more than the number of 
        /// folder they can upload according to their account plan
        /// 
        /// </summary>

        private void btnUploadFolder_Click(object sender, EventArgs e) {

            try {

                LimitedFolderAlert folderUploadFailed = new LimitedFolderAlert(Globals.accountType, "it looks like you've reached the max \r\namount of folder you can upload", true);

                List<string> foldersItems = lstFoldersPage.Items.Cast<string>().ToList();
                List<string> execludedStringsItem = new List<string> { "Home", "Shared To Me", "Shared Files" };
                int countTotalFolders = foldersItems.Count(item => !execludedStringsItem.Contains(item));

                if (Globals.uploadFolderLimit[Globals.accountType] == countTotalFolders) {
                    folderUploadFailed.Show();
                    return;
                }

                OpenFolderDialog();

            } catch (Exception) {
                BuildShowAlert(title: "Something went wrong", subheader: "Something went wrong while trying to upload this folder.");
            }
        }

        #endregion END - Open dialog section (Home, Public Storage, Folder)

        /// <summary>
        /// This button will show settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSettings_Click(object sender, EventArgs e) {

            Task.Run(() => new SettingsLoadingAlert().ShowDialog());

            var remAccShow = new SettingsForm();
            remAccShow.Show();

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private async Task FolderOnSelected(String folderName) {

            BuildButtonsOnFolderNameSelected();

            List<string> fileTypes = new List<string>();

            const string getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
            using (var command = new MySqlCommand(getFileType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(folderName));
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        fileTypes.Add(reader.GetString(0));
                    }
                }
            }

            var currMainLength = fileTypes.Count;

            await BuildFilePanelFolder(fileTypes, folderName, currMainLength);
            BuildRedundaneVisibility();

        }

        /// <summary>
        /// Select folder from listBox and start showing
        /// the files from selected folder
        /// </summary>

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

            pnlFileOptions.Visible = false;

            imgDiscover.Visible = false;
            lblDiscover.Visible = false;
            dotDiscover.Visible = false;

            try {

                int selectedTabIndex = lstFoldersPage.SelectedIndex;
                string selectedFolderTab = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                lblCurrentPageText.Text = selectedFolderTab;
                lblCurrentPageText.Visible = true;
                btnDeleteFolder.Visible = true;

                flwLayoutHome.Controls.Clear();

                if (selectedFolderTab == "Home") {
                    BuildButtonsOnHomePageSelected();
                    await BuildHomeFiles();

                } else if (selectedFolderTab != "Home" && selectedFolderTab != "Shared To Me" && selectedFolderTab != "Shared Files") {
                    await FolderOnSelected(selectedFolderTab);

                } else if (selectedTabIndex == 1) {
                    BuildButtonOnSharedFilesSelected();
                    ClearRedundane();

                    await CallFilesInformationShared();
                    await BuildSharedToMe();

                } else if (selectedTabIndex == 2) {
                    BuildButtonsOnSharedToMeSelected();
                    ClearRedundane();

                    await CallFilesInformationOthers();
                    await BuildSharedToOthers();

                }

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                flwLayoutHome.Controls.Clear();

                BuildRedundaneVisibility();
                BuildShowAlert(title: "Something went wrong", subheader: "Try to restart Flowstorage.");

            }
        }

        private void guna2Button19_Click(object sender, EventArgs e) {

            CloseRetrievalAlert();

            string _currentFold = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            RemoveAndDeleteFolder(_currentFold);

        }


        /// <summary>
        /// Generate user directory from Home folder
        /// </summary>
        /// <param name="userName">Username of user</param>
        /// <param name="customParameter">Custom parameter for panel</param>
        /// <param name="rowLength"></param>

        #region Build directory panel section

        private async Task buildDirectoryPanel(int rowLength) {

            List<Tuple<string>> filesInfo = new List<Tuple<string>>();

            const string selectFileData = "SELECT DIR_NAME FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        filesInfo.Add(new Tuple<string>(fileName));
                    }
                }
            }

            for (int i = 0; i < rowLength; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = "ABC02" + i,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };
                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;
                flwLayoutHome.Controls.Add(panelPic_Q);

                var panelF = panelPic_Q;

                Label directoryLab = new Label();
                panelF.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + i;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = GlobalStyle.DateLabelFont;
                directoryLab.ForeColor = GlobalStyle.DarkGrayColor;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Location = GlobalStyle.DateLabelLoc;
                directoryLab.Text = "Directory";

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.Width = 160;
                titleLab.Height = 20;
                titleLab.AutoEllipsis = true;
                titleLab.Text = filesInfo[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 8;
                picMain_Q.Width = 190;
                picMain_Q.Height = 145;
                picMain_Q.Visible = true;

                picMain_Q.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panelF.Width - picMain_Q.Width) / 2;
                picMain_Q.Location = new Point(picMain_Q_x, 10);

                picMain_Q.MouseHover += (_senderM, _ev) => {
                    panelF.ShadowDecoration.Enabled = true;
                    panelF.ShadowDecoration.BorderRadius = 8;
                };

                picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                    panelF.ShadowDecoration.Enabled = false;
                };

                Guna2Button remBut = new Guna2Button();
                panelF.Controls.Add(remBut);
                remBut.Name = "Rem" + i;
                remBut.Width = 29;
                remBut.Height = 26;
                remBut.ImageOffset = GlobalStyle.GarbageOffset;
                remBut.FillColor = GlobalStyle.TransparentColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = GlobalStyle.TransparentColor;
                remBut.Image = Globals.DirectoryGarbageImage;
                remBut.Visible = true;
                remBut.Location = GlobalStyle.GarbageButtonLoc;

                remBut.Click += (sender_im, e_im) => {

                    var titleFile = titleLab.Text;

                    DialogResult verifyDialog = MessageBox.Show($"Delete '{titleFile}' directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (verifyDialog == DialogResult.Yes) {

                        using (var command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                            command.ExecuteNonQuery();
                        }

                        using (var command = new MySqlCommand("DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleLab.Text));
                            command.ExecuteNonQuery();
                        }

                        using (var command = new MySqlCommand("DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleLab.Text));
                            command.ExecuteNonQuery();
                        }

                        panelPic_Q.Dispose();

                        lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                        BuildRedundaneVisibility();
                        UpdateProgressBarValue();
                    }
                };

                picMain_Q.Image = Globals.DIRIcon;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader").ShowDialog());
                    ShowAlert.Start();

                    DirectoryForm displayDirectory = new DirectoryForm(titleLab.Text);
                    displayDirectory.Show();

                    CloseRetrievalAlert();
                };
            }

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        #endregion END - Build directory panel section

        private void label10_Click(object sender, EventArgs e) {

        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e) {
            this.Invalidate();
            base.OnScroll(e);
        }

        /// ------------- TESTING ---------------

        /// <summary>
        /// Start encrypting connection 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button1_Click_1(object sender, EventArgs e) {
            richTextBox1.Text = EncryptConnection("0afe74-gksuwpe8r", richTextBox1.Text);
        }

        public static string EncryptConnection(string key, string plainInput) {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Refresh Shared To Me/Shared To Others panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void guna2Button4_Click(object sender, EventArgs e) {

            int selectedIndex = lstFoldersPage.SelectedIndex;

            flwLayoutHome.Controls.Clear();

            if (selectedIndex == 1 && lblCurrentPageText.Text == "Shared To Me") {
                GlobalsData.fileTypeValuesSharedToMe.Clear();
                await RefreshGenerateUserShared(GlobalsData.fileTypeValuesSharedToMe, "DirParMe");

            } else if (selectedIndex == 2 && lblCurrentPageText.Text == "Shared Files") {
                GlobalsData.fileTypeValuesSharedToOthers.Clear();
                await RefreshGenerateUserSharedOthers(GlobalsData.fileTypeValuesSharedToOthers, "DirParOther");

            } else if (selectedIndex == 0 && lblCurrentPageText.Text == "Home") {
                GlobalsData.base64EncodedImageHome.Clear();
                GlobalsData.base64EncodedThumbnailHome.Clear();
                await RefreshHomePanels();

            } else if (lblCurrentPageText.Text == "Public Storage") {
                GlobalsData.base64EncodedImagePs.Clear();
                GlobalsData.base64EncodedThumbnailPs.Clear();
                await RefreshPublicStoragePanels();

            }

            BuildRedundaneVisibility();
            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        private IEnumerable<Control> GetAllControls(Control parent) {
            var controls = parent.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControls(c)).Concat(controls);
        }

        /// <summary>
        /// 
        /// Detect for text input and search
        /// file based on the input
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void txtBoxSearchFile_TextChanged(object sender, EventArgs e) {

            string searchText = txtBoxSearchFile.Text.Trim().ToLower();

            string[] searchTerms = searchText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<Guna2Panel> disposedPanels = new List<Guna2Panel>();

            for (int i = flwLayoutHome.Controls.Count - 1; i >= 0; i--) {
                Control ctrl = flwLayoutHome.Controls[i];
                if (ctrl is Guna2Panel panel) {
                    bool matchesSearchTerms = false;
                    foreach (string term in searchTerms) {
                        if (panel.Controls.OfType<Label>().Any(l => l.Text.ToLower().Contains(term.Trim()))) {
                            matchesSearchTerms = true;
                            break;
                        }
                    }

                    if (matchesSearchTerms) {
                        panel.BackColor = Color.Transparent;
                        if (disposedPanels.Contains(panel)) {
                            disposedPanels.Remove(panel);
                        }

                    } else {
                        if (!disposedPanels.Contains(panel)) {
                            disposedPanels.Add(panel);
                        }

                        flwLayoutHome.Controls.RemoveAt(i);
                        panel.Dispose();

                    }
                }
            }

            if (string.IsNullOrEmpty(searchText)) {

                string origin = lblCurrentPageText.Text;
                string _selectedFolderSearch = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                flwLayoutHome.Controls.Clear();

                if (origin == "Home") {
                    await BuildHomeFiles();

                } else if (origin == "Shared To Me") {
                    GlobalsData.fileTypeValuesSharedToMe.Clear();
                    await RefreshGenerateUserShared(GlobalsData.fileTypeValuesSharedToMe, "DirParMe");

                } else if (origin == "Shared Files") {
                    GlobalsData.fileTypeValuesSharedToOthers.Clear();
                    await RefreshGenerateUserSharedOthers(GlobalsData.fileTypeValuesSharedToOthers, "DirParOther");

                } else if (_selectedFolderSearch != "Shared Files" || _selectedFolderSearch != "Shared To Me" || _selectedFolderSearch != "Home") {
                    await RefreshFolder();

                }

                if (origin == "Public Storage") {
                    GlobalsData.base64EncodedImagePs.Clear();
                    GlobalsData.base64EncodedThumbnailPs.Clear();
                    await BuildPublicStorageFiles();

                }

            }

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();
            BuildRedundaneVisibility();

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        /// <summary>
        /// Go to Home button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button9_Click_1(object sender, EventArgs e) {

            if (lblCurrentPageText.Text == "Public Storage") {
                flwLayoutHome.Controls.Clear();
                await BuildHomeFiles();
            }

            lblCurrentPageText.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            btnRefreshFiles.Visible = true;
            pnlSubPanelDetails.Visible = true;
            btnLogout.Visible = true;
            pnlMain.Visible = true;
            pnlPublicStorage.Visible = false;

            imgDiscover.Visible = false;
            lblDiscover.Visible = false;
            dotDiscover.Visible = false;

            lblMyFolders.Visible = false;
            lstFoldersPage.Visible = false;

            btnGoHomePage.FillColor = GlobalStyle.DarkPurpleColor;

            btnShowFolderPage.FillColor = GlobalStyle.TransparentColor;
            btnShowPs.FillColor = GlobalStyle.TransparentColor;

            pnlMain.BringToFront();
            pnlFolders.SendToBack();
            pnlPublicStorage.SendToBack();

        }

        /// <summary>
        /// Go to Folders button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button13_Click(object sender, EventArgs e) {

            pnlSubPanelDetails.Visible = false;
            btnLogout.Visible = false;
            pnlPublicStorage.Visible = false;

            btnShowFolderPage.FillColor = GlobalStyle.DarkPurpleColor;

            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;
            btnShowPs.FillColor = GlobalStyle.TransparentColor;

            lblMyFolders.Visible = true;
            lstFoldersPage.Visible = true;

            pnlFolders.BringToFront();
            pnlPublicStorage.SendToBack();
            pnlMain.SendToBack();
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {
        }

        private void guna2Button14_Click(object sender, EventArgs e) {
            new SettingsForm().Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e) {

        }

        private void RefreshAllOnLogut() {

            pnlMain.SendToBack();

            HomePage.instance.label2.Text = "Item Count";
            HomePage.instance.lblUpload.Text = "Upload";
            HomePage.instance.btnUploadFile.Text = "Upload File";
            HomePage.instance.btnUploadFolder.Text = "Upload Folder";
            HomePage.instance.btnCreateDirectory.Text = "Create Directory";
            HomePage.instance.btnFileSharing.Text = "File Sharing";
            HomePage.instance.btnFileSharing.Size = new Size(125, 47);
            HomePage.instance.lblEssentials.Text = "Essentials";

            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";

            if(Directory.Exists(directoryPath)) {
                Directory.Delete(directoryPath, true);
            }

            GlobalsData.base64EncodedImageHome.Clear();
            GlobalsData.base64EncodedThumbnailHome.Clear();

            GlobalsData.base64EncodedThumbnailSharedOthers.Clear();
            GlobalsData.base64EncodedImageSharedOthers.Clear();

            GlobalsData.base64EncodedThumbnailSharedToMe.Clear();
            GlobalsData.base64EncodedImageSharedToMe.Clear();

            GlobalsData.base64EncodedImagePs.Clear();
            GlobalsData.base64EncodedThumbnailPs.Clear();

            HomePage.instance.lstFoldersPage.Items.Clear();

            Hide();

            SignUpForm signUpForm = new SignUpForm();
            signUpForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e) {

            try {

                DialogResult _confirmation = MessageBox.Show("Logout your account?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (_confirmation == DialogResult.Yes) {
                    RefreshAllOnLogut();

                }

            } catch (Exception) {
                MessageBox.Show("There's a problem while attempting to logout your account. Please try again.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        #region Drag and drop upload section

        private void Form1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e) {
            pnlDragAndDropUpload.Visible = true;
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragLeave(object sender, EventArgs e) => pnlDragAndDropUpload.Visible = false;


        /// <summary>
        /// 
        /// Drag-drop upload feature is implemented here
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void Form1_DragDrop(object sender, DragEventArgs e) {

            pnlDragAndDropUpload.Visible = false;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length + flwLayoutHome.Controls.Count > Globals.uploadFileLimit[Globals.accountType]) {
                using (Form bgBlur = new Form()) {
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.FormBorderStyle = FormBorderStyle.None;
                    bgBlur.Opacity = .24d;
                    bgBlur.BackColor = Color.Black;
                    bgBlur.Name = "bgBlurForm";
                    bgBlur.WindowState = FormWindowState.Maximized;
                    bgBlur.TopMost = true;
                    bgBlur.Location = this.Location;
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.ShowInTaskbar = false;
                    bgBlur.Show();

                    using (UpgradeAccountAlert displayUpgrade = new UpgradeAccountAlert()) {
                        displayUpgrade.Owner = bgBlur;
                        displayUpgrade.ShowDialog();
                    }
                };

            } else {
                await DragDropHandleFiles(files);

            }

        }

        private async Task DragDropHandleFiles(string[] files) {

            List<string> filePathList = new List<string>(files);

            foreach (var selectedItems in filePathList) {

                _fileName = Path.GetFileName(selectedItems);
                _fileExtension = Path.GetExtension(selectedItems);
                _fileSizeInMB = 0;

                try {

                    byte[] readFileBytes = File.ReadAllBytes(selectedItems);
                    string toBase64String = Convert.ToBase64String(readFileBytes);
                    string encryptText = EncryptionModel.Encrypt(toBase64String);

                    _fileSizeInMB = (readFileBytes.Length / 1024) / 1024;

                    if (Globals.imageTypes.Contains(_fileExtension)) {
                        curr++;
                        var getImg = new Bitmap(selectedItems);
                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;

                        string tempToBase64 = Convert.ToBase64String(readFileBytes);
                        string encryptedValue = EncryptionModel.Encrypt(tempToBase64);
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, encryptedValue);
                        
                    } else if (Globals.textTypes.Contains(_fileExtension)) {
                        txtCurr++;

                        string nonLine = "";

                        using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptTextValue = EncryptionModel.Encrypt(getEncoded);
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeTextTable, "PanTxt", txtCurr, encryptTextValue);

                    } else if (_fileExtension == ".exe") {
                        exeCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExeTable, "PanExe", exeCurr, encryptText);

                    } else if (Globals.videoTypes.Contains(_fileExtension)) {
                        vidCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptText);

                    } else if (Globals.excelTypes.Contains(_fileExtension)) {
                        exlCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptText);

                    } else if (Globals.audioTypes.Contains(_fileExtension)) {
                        audCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptText);

                    } else if (_fileExtension == ".apk") {
                        apkCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptText);

                    } else if (_fileExtension == ".pdf") {
                        pdfCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptText);

                    } else if (Globals.ptxTypes.Contains(_fileExtension)) {
                        ptxCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptText);

                    } else if (_fileExtension == ".msi") {
                        msiCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptText);

                    } else if (Globals.wordTypes.Contains(_fileExtension)) {
                        docxCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptText);

                    }

                    CloseUploadAlert();

                } catch (Exception) {
                    CloseUploadAlert();
                }

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            }

        }

        #endregion END - Drag and upload section

        #region Filter type section

        bool filterTypePanelVisible = false;
        private void guna2Button16_Click(object sender, EventArgs e) {
            filterTypePanelVisible = !filterTypePanelVisible;
            pnlFilterType.Visible = filterTypePanelVisible;
        }

        private void guna2Button18_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".png,.jpeg,.jpg";
        }

        private void guna2Button17_Click_2(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".txt,.html,.md,.sql,.css,.js,.csv";
        }

        private void guna2Button22_Click_1(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".doc,.docx";

        }

        private void guna2Button20_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".mp3,.wav";

        }

        private void guna2Button21_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".xlsx,xls";
        }

        private void guna2Button23_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".mp4,.avi,.mov,wmv";

        }

        private void guna2Button24_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".pdf";
        }

        private void guna2Button25_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
        }

        #endregion END - Filter type section

        private void guna2Panel2_Paint_1(object sender, PaintEventArgs e) {

        }

        private void label26_Click(object sender, EventArgs e) {

        }


        /// <summary>
        /// 
        /// Download folder button.
        /// Only user with paid plan has this feature enabled, else
        /// alert the user to upgrade their plan if their account is Basic plan
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button27_Click(object sender, EventArgs e) {

            if (Globals.accountType == "Max" || Globals.accountType == "Express" || Globals.accountType == "Supreme") {
                string folderTitleGet = EncryptionModel.Encrypt(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem));
                await DownloadUserFolder(folderTitleGet);

            } else {
                new LimitedFolderAlert(
                    Globals.accountType, 
                    "Please upgrade your account \r\nplan to download folder.", false).Show();
            }
        }

        private void guna2Panel3_Paint_1(object sender, PaintEventArgs e) {

        }

        private void guna2Button28_Click(object sender, EventArgs e) => pnlFileOptions.Visible = false;

        /// <summary>
        /// 
        /// Delete file, including folder, home, sharing, directory
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteFile_Click_1(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string panelName = lblFilePanelName.Text;
            string sharedToName = lblSharedToName.Text;
            string dirName = lblSelectedDirName.Text;

            DialogResult verifyDialog = MessageBox.Show($"Delete '{titleFile}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (verifyDialog == DialogResult.Yes) {

                using (MySqlCommand command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                    command.ExecuteNonQuery();
                }

                if (GlobalsTable.publicTables.Contains(tableName) || GlobalsTable.publicTablesPs.Contains(tableName)) {

                    string removeQuery = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.ExecuteNonQuery();
                    }

                    GlobalsData.filesMetadataCacheHome.Clear();

                } else if (tableName == GlobalsTable.folderUploadTable) {

                    const string removeQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername";
                    using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(dirName));
                        command.ExecuteNonQuery();
                    }

                } else if (tableName == GlobalsTable.sharingTable && sharedToName != "sharedToName") {

                    const string removeQuery = "DELETE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                    using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                        cmd.Parameters.AddWithValue("@username", Globals.custUsername);
                        cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        cmd.Parameters.AddWithValue("@sharedname", sharedToName);

                        cmd.ExecuteNonQuery();
                    }

                } else if (tableName == GlobalsTable.sharingTable && sharedToName == "sharedToName") {

                    const string removeQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    using (var command = new MySqlCommand(removeQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.ExecuteNonQuery();
                    }

                }

                Control[] matches = this.Controls.Find(panelName, true);
                if (matches.Length > 0 && matches[0] is Guna2Panel) {
                    Guna2Panel myPanel = (Guna2Panel)matches[0];
                    flwLayoutHome.Controls.Remove(myPanel);
                    myPanel.Dispose();
                }

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                BuildRedundaneVisibility();

                int getCurrentCount = int.Parse(lblItemCountText.Text);
                int getLimitedValue = int.Parse(lblLimitUploadText.Text);
                int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
                lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                progressBarUsageStorage.Value = calculatePercentageUsage;

                pnlFileOptions.Visible = false;

            }
        }

        private void guna2Button30_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string panelName = lblFilePanelName.Text;
            string sharedToName = lblSharedToName.Text;
            string dirName = lblSelectedDirName.Text;

            RenameFileForm renameFileFORM = new RenameFileForm(titleFile, tableName, panelName, dirName, sharedToName);
            renameFileFORM.Show();
        }

        private void guna2Button32_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string dirName = lblSelectedDirName.Text;

            if (tableName == GlobalsTable.folderUploadTable) {
                SaverModel.SaveSelectedFile(titleFile, GlobalsTable.folderUploadTable, dirName);

            } else if (tableName == GlobalsTable.homeVideoTable) {
                SaverModel.SaveSelectedFile(titleFile, GlobalsTable.homeVideoTable, dirName);

            } else if (tableName == GlobalsTable.sharingTable) {
                string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                bool fromSharedFiles = selectedFolder == "Shared Files";

                SaverModel.SaveSelectedFile(titleFile, GlobalsTable.sharingTable, dirName, fromSharedFiles);

            } else if (tableName != GlobalsTable.sharingTable && tableName != GlobalsTable.folderUploadTable && tableName != GlobalsTable.directoryUploadTable) {
                SaverModel.SaveSelectedFile(titleFile, tableName, dirName, isFromMyPs: _isMyPublicStorageSelected);

            }

        }

        private void guna2Button29_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblSelectedDirName.Text;

            string fileExtensions = titleFile.Split('.').Last();

            string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            bool fromSharedFiles = selectedFolder == "Shared Files";

            shareFileFORM sharingFileFORM = new shareFileFORM(titleFile, fileExtensions, fromSharedFiles, Globals.custUsername, dirName);
            sharingFileFORM.Show();

        }

        private void label25_Click(object sender, EventArgs e) {


        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        /// <summary>
        /// 
        /// Public storage upload file
        /// 
        /// </summary>g
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void guna2Button4_Click_1(object sender, EventArgs e) {

            btnShowPs.FillColor = GlobalStyle.DarkPurpleColor;

            btnShowFolderPage.FillColor = GlobalStyle.TransparentColor;
            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;

            btnRefreshFiles.Visible = true;
            pnlSubPanelDetails.Visible = true;
            btnLogout.Visible = true;
            pnlPublicStorage.Visible = true;

            lblMyFolders.Visible = false;
            lstFoldersPage.Visible = false;

            pnlPublicStorage.BringToFront();
            pnlFolders.SendToBack();
            pnlMain.SendToBack();

            imgDiscover.Visible = true;
            lblDiscover.Visible = true;
            dotDiscover.Visible = true;

            _isMyPublicStorageSelected = false;

            if (lblCurrentPageText.Text == "Public Storage") {
                return;
            }

            lblCurrentPageText.Text = "Public Storage";

            flwLayoutHome.Controls.Clear();

            await BuildPublicStorageFiles();

            BuildRedundaneVisibility();

        }

        private async void btnMyPsFiles_Click(object sender, EventArgs e) {
            flwLayoutHome.Controls.Clear();
            await BuildMyPublicStorageFiles();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e) {

        }

        private void HomePage_FormClosing(object sender, FormClosingEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            new SettingsForm().Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];
        }

        private void guna2Button1_Click_1(object sender, EventArgs e) {
            pnlFilterType.Visible = !pnlFilterType.Visible;
        }

        private void lblItemCountText_Click(object sender, EventArgs e) {

        }

        private void pnlPsSubDetails_Paint(object sender, PaintEventArgs e) {

        }

        private void pnlPublicStorage_Paint(object sender, PaintEventArgs e) {

        }
    }
}