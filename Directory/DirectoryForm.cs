using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using FlowSERVER1.Temporary;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Shell;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class DirectoryForm : Form {

        public static DirectoryForm instance;

        readonly private Crud crud = new Crud();
        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        readonly private MySqlConnection con = ConnectionModel.con;
        private string _todayDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public DirectoryForm(String directoryName) {
            InitializeComponent();

            instance = this;

            this.Text = $"{directoryName} (Directory)";
            this.lblDirectoryName.Text = directoryName;

            InitializeDirectoryFiles();

        }

        private async Task InsertFileData(string fileName, string fileBase64EncodedData) {

            try {

                var fileSizeInMb = Convert.FromBase64String(fileBase64EncodedData).Length / 1024 / 1024;

                StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

                string fileType = Path.GetExtension(fileName);

                string encryptedFileName = EncryptionModel.Encrypt(fileName);
                string encryptedDirectoryname = EncryptionModel.Encrypt(lblDirectoryName.Text);

                const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@file_name, @username, @date, @file_data, @thumbnail, @file_type, @dir_name)";

                var param = new Dictionary<string, string>
                {
                    { "@username", tempDataUser.Username},
                    { "@file_name", encryptedFileName},
                    { "@file_type", fileType},
                    { "@dir_name", encryptedDirectoryname},
                    { "@date", _todayDate},
                    { "@file_data", fileBase64EncodedData},
                    { "@thumbnail", ""},
                };

                await crud.Insert(insertQuery, param);

                ClosePopupForm.CloseUploadingPopup();

            } catch (Exception) {
                new CustomAlert(
                    title: "Something went wrong", subheader: "Failed to upload this file.").Show();

            }
        }

        private async Task InsertFileDataVideo(string filePath, string fileBase64EncodedData) {

            string fileName = Path.GetFileName(filePath);
            string fileType = Path.GetExtension(fileName);

            var fileSizeInMb = Convert.FromBase64String(fileBase64EncodedData).Length / 1024 / 1024;

            StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

            string encryptedFileName = EncryptionModel.Encrypt(fileName);
            string encryptedDirectoryName = EncryptionModel.Encrypt(lblDirectoryName.Text);

            const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@file_name, @username, @date, @file_data, @thumbnail, @file_type, @dir_name)";

            using (var command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@file_name", encryptedFileName);
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@file_type", fileType);
                command.Parameters.AddWithValue("@date", _todayDate);
                command.Parameters.AddWithValue("@dir_name", encryptedDirectoryName);
                command.Parameters.AddWithValue("@file_data", fileBase64EncodedData);

                using (var shellFile = ShellFile.FromFilePath(filePath))
                using (var stream = new MemoryStream()) {
                    shellFile.Thumbnail.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    string base64Thumbnail = Convert.ToBase64String(stream.ToArray());
                    string compressedThumbnail = compressor.compressBase64Image(base64Thumbnail);

                    command.Parameters.AddWithValue("@thumbnail", compressedThumbnail);
                }

                await command.ExecuteNonQueryAsync();
            }

            ClosePopupForm.CloseUploadingPopup();

        }

        private async void InitializeDirectoryFiles() {

            Dictionary<string, (string, string)> fileExtensions = new Dictionary<string, (string, string)> {
                { ".png", ("imgFilePng", GlobalsTable.homeImageTable) },
                { ".jpg", ("imgFileJpg", GlobalsTable.homeImageTable) },
                { ".jpeg", ("imgFilePeg", GlobalsTable.homeImageTable) },
                { ".txt", ("txtFile", GlobalsTable.homeTextTable) },
                { ".js", ("txtFile", GlobalsTable.homeTextTable) },
                { ".sql", ("txtFile", GlobalsTable.homeTextTable) },
                { ".py", ("txtFile", GlobalsTable.homeTextTable) },
                { ".html", ("txtFile", GlobalsTable.homeTextTable) },
                { ".csv", ("txtFile", GlobalsTable.homeTextTable) },
                { ".css", ("txtFile", GlobalsTable.homeTextTable) },
                { ".exe", ("exeFile", GlobalsTable.homeExeTable) },
                { ".mp4", ("vidFile", GlobalsTable.homeVideoTable) },
                { ".wav", ("vidFile", GlobalsTable.homeVideoTable) },
                { ".xlsx", ("exlFile", GlobalsTable.homeExcelTable) },
                { ".mp3", ("audiFile", GlobalsTable.homeAudioTable) },
                { ".apk", ("apkFile", GlobalsTable.homeApkTable) },
                { ".pdf", ("pdfFile", GlobalsTable.homePdfTable) },
                { ".pptx", ("ptxFile", GlobalsTable.homePtxTable) },
                { ".msi", ("msiFile", GlobalsTable.homeMsiTable) },
                { ".docx", ("docFile", GlobalsTable.homeWordTable) },
            };

            foreach (string fileType in fileExtensions.Keys) {

                int count = CountFilesInDirectory(fileType);

                if (count > 0) {
                    string controlName = fileExtensions[fileType].Item1;
                    string tableName = fileExtensions[fileType].Item2;
                    await BuildFilePanelOnLoad(fileType, tableName, controlName, count);

                }

            }

            BuildRedundaneVisibility();
            lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";

        }

        private int CountFilesInDirectory(string fileType) {

            string encryptedDirectoryName = EncryptionModel.Encrypt(lblDirectoryName.Text);

            const string query = "SELECT COUNT(*) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", encryptedDirectoryName);
                command.Parameters.AddWithValue("@ext", fileType);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void BuildRedundaneVisibility() {
            if (flwLayoutDirectory.Controls.Count == 0) {
                ShowRedundane();

            } else {
                ClearRedundane();

            }
        }

        private void ClearRedundane() {
            guna2Button6.Visible = false;
            label8.Visible = false;
        }

        private void ShowRedundane() {
            guna2Button6.Visible = true;
            label8.Visible = true;
        }

        /// <summary>
        /// Function for file generation on load.
        /// Control will be generated on the flowlayout panel
        /// based on the count of files
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>

        private async Task BuildFilePanelOnLoad(string fileType, string tableName, string parameterName, int currItem) {

            var imageValues = new List<Image>();
            var onPressedEvent = new List<EventHandler>();
            var onMoreOptionButtonPressed = new List<EventHandler>();

            var filesInfo = new List<(string, string, string)>();

            const string selectFileDataDir = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (MySqlCommand command = new MySqlCommand(selectFileDataDir, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                command.Parameters.AddWithValue("@ext", fileType);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate, String.Empty));
                    }
                }
            }

            var base64EncodedImage = new List<string>();
            var base64EncodedThumbnail = new List<string>();

            if (Globals.imageTypes.Contains(fileType)) {

                if (base64EncodedImage.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", tempDataUser.Username);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", fileType);

                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64EncodedImage.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                            }
                        }
                    }
                }
            }

            if (Globals.videoTypes.Contains(fileType)) {

                if (base64EncodedThumbnail.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", tempDataUser.Username);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", fileType);
                        using (var readBase64 = await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64EncodedThumbnail.Add(readBase64.GetString(0));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < currItem; i++) {

                int accessIndex = i;
                string fileName = filesInfo[accessIndex].Item1;

                void moreOptionOnPressedEvent(object sender, EventArgs e) {
                    pnlFileOptions.Visible = true;
                    lblFileNameOnPanel.Text = filesInfo[accessIndex].Item1;
                    lblFilePanelName.Text = parameterName + accessIndex;
                }

                onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                if (tableName == GlobalsTable.homeImageTable) {

                    if (base64EncodedImage.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64EncodedImage[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            imageValues.Add(Image.FromStream(toMs));
                        }
                    }

                    void imageOnPressed(object sender, EventArgs e) {

                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(imageOnPressed);

                }

                if (tableName == GlobalsTable.homeVideoTable) {

                    if (base64EncodedThumbnail.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64EncodedThumbnail[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            imageValues.Add(Image.FromStream(toMs));
                        }
                    }

                    void videoOnPressed(object sender, EventArgs e) {

                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;

                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        vidFormShow.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }


                if (tableName == GlobalsTable.homeTextTable) {

                    string textTypes = fileName.Substring(fileName.LastIndexOf('.')).TrimStart();
                    imageValues.Add(Globals.textTypeToImage[textTypes]);

                    void videoOnPressed(object sender, EventArgs e) {
                        TextForm displayPic = new TextForm(GlobalsTable.directoryUploadTable, fileName, lblDirectoryName.Text, tempDataUser.Username);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeExeTable) {

                    imageValues.Add(Globals.EXEImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        exeFORM displayExe = new exeFORM(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        displayExe.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeExcelTable) {

                    imageValues.Add(Globals.EXCELImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ExcelForm exlForm = new ExcelForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        exlForm.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeAudioTable) {

                    imageValues.Add(Globals.AudioImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        AudioForm audioOnPressed = new AudioForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        audioOnPressed.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeApkTable) {

                    imageValues.Add(Globals.APKImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ApkForm displayPic = new ApkForm(fileName, tempDataUser.Username, GlobalsTable.directoryUploadTable, lblDirectoryName.Text);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homePdfTable) {

                    imageValues.Add(Globals.PDFImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PdfForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homePtxTable) {

                    imageValues.Add(Globals.PTXImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PtxForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeMsiTable) {

                    imageValues.Add(Globals.MSIImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new MsiForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);

                }

                if (tableName == GlobalsTable.homeWordTable) {

                    imageValues.Add(Globals.DOCImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new WordDocForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);

                }
            }

            PanelGenerator panelGenerator = new PanelGenerator();
            panelGenerator.GeneratePanel(parameterName, currItem, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues, isFromDirectory: true);
        }

        private void Form3_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// Initialize file panel variable (for increment)
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

        private async Task CreateFilePanel(string fileFullPath, string tableName, string panName, int itemCurr, string keyVal) {

            string fileName = Path.GetFileName(fileFullPath);

            var panelTxt = new Guna2Panel() {
                Name = panName + itemCurr,
                Width = 200,
                Height = 222,
                BorderColor = GlobalStyle.BorderColor,
                BorderThickness = 1,
                BorderRadius = 8,
                BackColor = GlobalStyle.TransparentColor,
                Location = new Point(600, Globals.PANEL_GAP_TOP)
            };

            Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

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

            int picMain_Q_x = (mainPanelTxt.Width - textboxPic.Width) / 2;

            textboxPic.Location = new Point(picMain_Q_x, 10);

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
            titleLab.Text = fileName;

            Guna2Button remButTxt = new Guna2Button();
            mainPanelTxt.Controls.Add(remButTxt);
            remButTxt.Name = "RemTxtBut" + itemCurr;
            remButTxt.Width = 29;
            remButTxt.Height = 26;
            remButTxt.ImageOffset = new Point(2, 0);
            remButTxt.FillColor = GlobalStyle.TransparentColor;
            remButTxt.BorderRadius = 6;
            remButTxt.BorderThickness = 1;
            remButTxt.BorderColor = GlobalStyle.TransparentColor;
            remButTxt.Image = GlobalStyle.GarbageImage;
            remButTxt.Visible = true;
            remButTxt.Location = GlobalStyle.GarbageButtonLoc;
            remButTxt.BringToFront();

            remButTxt.Click += (sender_tx, e_tx) => {

                pnlFileOptions.Visible = true;
                lblFileNameOnPanel.Text = titleLab.Text;
                lblFilePanelName.Text = panelTxt.Name;
            };

            textboxPic.MouseHover += (_senderM, _ev) => {
                panelTxt.ShadowDecoration.Enabled = true;
                panelTxt.ShadowDecoration.BorderRadius = 8;
            };

            textboxPic.MouseLeave += (_senderQ, _evQ) => {
                panelTxt.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.homeImageTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = new Bitmap(fileFullPath);
                textboxPic.Click += (sender_f, e_f) => {

                    var getImgName = (Guna2PictureBox)sender_f;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                    PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayPic.Show();
                };

            }

            if (tableName == GlobalsTable.homeTextTable) {

                await InsertFileData(fileName, keyVal);

                string textType = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                textboxPic.Image = Globals.textTypeToImage[textType];

                textboxPic.Click += (sender_t, e_t) => {
                    TextForm txtFormShow = new TextForm(GlobalsTable.directoryUploadTable, fileName, lblDirectoryName.Text, tempDataUser.Username);
                    txtFormShow.Show();
                };
            }

            if (tableName == GlobalsTable.homeExeTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.EXEImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    new exeFORM(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                };
            }

            if (tableName == GlobalsTable.homeVideoTable) {

                await InsertFileDataVideo(fileFullPath, keyVal);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                textboxPic.Image = toBitMap;

                textboxPic.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImg = new Bitmap(getImgName.Image);

                    VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    vidShow.Show();
                };

            }
            if (tableName == GlobalsTable.homeAudioTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.AudioImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    AudioForm displayPic = new AudioForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.homeExcelTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.EXCELImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    ExcelForm displayPic = new ExcelForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.homeApkTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.APKImage;
                textboxPic.Click += (sender_gi, e_gi) => {
                    ApkForm displayPic = new ApkForm(fileName, tempDataUser.Username, GlobalsTable.directoryUploadTable, lblDirectoryName.Text);
                    displayPic.Show();
                };
            }
            if (tableName == GlobalsTable.homePdfTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.PDFImage;
                textboxPic.Click += (sender_pd, e_pd) => {
                    PdfForm displayPdf = new PdfForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayPdf.ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homePtxTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.PTXImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    PtxForm displayPtx = new PtxForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayPtx.ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homeMsiTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.MSIImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    MsiForm displayMsi = new MsiForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayMsi.Show();
                };
            }

            if (tableName == GlobalsTable.homeWordTable) {

                await InsertFileData(fileName, keyVal);

                textboxPic.Image = Globals.DOCImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    WordDocForm displayWord = new WordDocForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                    displayWord.ShowDialog();
                };
            }

            flwLayoutDirectory.Controls.Add(panelTxt);

            ClosePopupForm.CloseUploadingPopup();

        }

        private void ShowUpgradeDialog() {
            Form bgBlur = new Form();
            using (UpgradeAccountAlert displayUpgrade = new UpgradeAccountAlert()) {
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

                displayUpgrade.Owner = bgBlur;
                displayUpgrade.ShowDialog();

                bgBlur.Dispose();
            };
        }

        private async void OpenDialogUpload() {

            var open = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = true
            };

            int curFilesCount = flwLayoutDirectory.Controls.Count;

            if (open.ShowDialog() == DialogResult.OK) {

                if (open.FileNames.Length + curFilesCount > Globals.uploadFileLimit[tempDataUser.AccountType]) {
                    ShowUpgradeDialog();
                    return;
                } 

                var filesName = new HashSet<string>(flwLayoutDirectory.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                foreach (var selectedItems in open.FileNames) {

                    string fileName = Path.GetFileName(selectedItems);
                    string fileType = Path.GetExtension(selectedItems);

                    if (filesName.Contains(Path.GetFileName(selectedItems).ToLower().Trim())) {
                        continue;
                    }

                    try {

                        byte[] getBytesSelectedFiles = File.ReadAllBytes(selectedItems);
                        byte[] compressedBytes = new GeneralCompressor().compressFileData(getBytesSelectedFiles);

                        string toBase64String = Convert.ToBase64String(compressedBytes);
                        string encryptBase64String = UniqueFile.IgnoreEncryption(fileType) 
                            ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                        if (Globals.imageTypes.Contains(fileType)) {
                            curr++;

                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImageBase64 = compressor.compressImageToBase64(selectedItems);
                            string encryptedImage = EncryptionModel.Encrypt(compressedImageBase64);
                            await CreateFilePanel
                                (selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, encryptedImage);
                                
                        } else if (Globals.textTypes.Contains(fileType)) {
                            txtCurr++;

                            string nonLine = "";
                            using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                nonLine = ReadFileTxt.ReadToEnd();
                            }

                            byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                            byte[] compressedTextBytes = compressor.compressFileData(getBytes);
                            string getEncoded = Convert.ToBase64String(compressedTextBytes);
                            string encryptEncodedText = EncryptionModel.Encrypt(getEncoded);

                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeTextTable, "PanTxt", txtCurr, encryptEncodedText);

                        } else if (fileType == ".exe") {
                            exeCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeExeTable, "PanExe", exeCurr, encryptBase64String);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            vidCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptBase64String);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            exlCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptBase64String);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            audCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptBase64String);

                        } else if (fileType == ".apk") {
                            apkCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptBase64String);

                        } else if (fileType == ".pdf") {
                            pdfCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptBase64String);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            ptxCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptBase64String);

                        } else if (fileType == ".msi") {
                            msiCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptBase64String);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            docxCurr++;
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptBase64String);

                        } else {
                            new CustomAlert(
                                title: "Upload Failed", subheader: "File type is not supported.").Show();

                        }

                        ClosePopupForm.CloseUploadingPopup();

                    } catch (Exception) {
                        OnUploadFailed();

                    }
                }
            }

            lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";
            BuildRedundaneVisibility();

        }

        private void OnUploadFailed() {
            ClosePopupForm.CloseUploadingPopup();
            new CustomAlert(
                title: "Some went wrong", subheader: "Failed to upload this file.").Show();
        }

        private void DisplayError() {
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

        private void btnUploadFile_Click(object sender, EventArgs e) {

            try {

                int currentUploadCount = flwLayoutDirectory.Controls.Count;

                if (currentUploadCount != Globals.uploadFileLimit[tempDataUser.AccountType]) {
                    OpenDialogUpload();

                } else {
                    DisplayError();

                }

            } catch (Exception) {
                ClosePopupForm.CloseUploadingPopup();
                new CustomAlert(
                    title: "An error occurred", "Something went wrong while trying to upload files.").Show();
            }
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }

        private void guna2Button26_Click(object sender, EventArgs e) {

            string fileName = lblFileNameOnPanel.Text;
            string panelname = lblFilePanelName.Text;

            DialogResult verifyDialog = MessageBox.Show($"Delete '{fileName}'?", "Flowstorage", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (verifyDialog == DialogResult.Yes) {
                using (var command = con.CreateCommand()) {
                    const string noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                    command.CommandText = noSafeUpdate;
                    command.ExecuteNonQuery();
                }

                using (var command = con.CreateCommand()) {
                    const string removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command.CommandText = removeQuery;
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                    command.ExecuteNonQuery();
                }

                Control[] matches = this.Controls.Find(panelname, true);

                if (matches.Length > 0 && matches[0] is Guna2Panel) {
                    Guna2Panel myPanel = (Guna2Panel)matches[0];
                    myPanel.Dispose();
                }

                BuildRedundaneVisibility();

                pnlFileOptions.Visible = false;
                lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";

            }
        }

        private void guna2Button28_Click(object sender, EventArgs e) {
            pnlFileOptions.Visible = false;
        }

        private void guna2Button30_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = GlobalsTable.directoryUploadTable;
            string panelName = lblFilePanelName.Text;
            string dirName = lblDirectoryName.Text;

            RenameFileForm renameFileFORM = new RenameFileForm(titleFile, tableName, panelName, dirName);
            renameFileFORM.Show();
        }

        private void guna2Button32_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            SaverModel.SaveSelectedFile(titleFile, GlobalsTable.directoryUploadTable, dirName);

        }

        private void guna2Button29_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            new shareFileFORM(
                titleFile, false, GlobalsTable.directoryUploadTable, dirName).Show();

        }
    }
}