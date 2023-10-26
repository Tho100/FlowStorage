using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;
using FlowSERVER1.Helper;

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

        readonly private MySqlConnection con = ConnectionModel.con;

        private string _loadedExtensionType { get; set; }
        private string _uploadedExtensionType { get; set; }
        private string _fileName { get; set; }
        private string _todayDate { get; set; }
        private long _fileSizeInMB { get; set; }

        /// <summary>
        /// 
        /// Initialize panel data
        /// 
        /// </summary>

        public DirectoryForm(String directoryName) {
            InitializeComponent();

            instance = this;

            this.Text = $"{directoryName} (Directory)";
            this.lblDirectoryName.Text = directoryName;

            Dictionary<string, (string, string)> fileExtensions = new Dictionary<string, (string, string)> {
                { ".png", ("imgFilePng", "file_info_image") },
                { ".jpg", ("imgFileJpg", "file_info_image") },
                { ".jpeg", ("imgFilePeg", "file_info_image") },
                { ".bmp", ("imgFileBmp", "file_info_image") },
                { ".txt", ("txtFile", "file_info_text") },
                { ".js", ("txtFile", "file_info_text") },
                { ".sql", ("txtFile", "file_info_text") },
                { ".py", ("txtFile", "file_info_text") },
                { ".html", ("txtFile", "file_info_text") },
                { ".csv", ("txtFile", "file_info_text") },
                { ".css", ("txtFile", "file_info_text") },
                { ".exe", ("exeFile", "file_info_exe") },
                { ".mp4", ("vidFile", "file_info_video") },
                { ".wav", ("vidFile", "file_info_video") },
                { ".xlsx", ("exlFile", "file_info_excel") },
                { ".mp3", ("audiFile", "file_info_audio") },
                { ".apk", ("apkFile", "file_info_apk") },
                { ".pdf", ("pdfFile", "file_info_pdf") },
                { ".pptx", ("ptxFile", "file_info_ptx") },
                { ".msi", ("msiFile", "file_info_msi") },
                { ".docx", ("docFile", "file_info_word") },
            };

            foreach (string ext in fileExtensions.Keys) {
                int count = CountFilesInDirectory(ext);
                if (count > 0) {
                    _loadedExtensionType = ext;
                    string controlName = fileExtensions[ext].Item1;
                    string tableName = fileExtensions[ext].Item2;
                    BuildFilePanelOnLoad(tableName, controlName, count);
                }
            }

            BuildRedundaneVisibility();

            lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";

        }

        private int CountFilesInDirectory(string fileType) {

            string encryptedDirectoryName = EncryptionModel.Encrypt(lblDirectoryName.Text);

            const string query = "SELECT COUNT(*) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@dirname", encryptedDirectoryName);
                command.Parameters.AddWithValue("@ext", fileType);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void BuildRedundaneVisibility() {
            if (flwLayoutDirectory.Controls.Count == 0) {
                ShowRedundane();
            }
            else {
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

        private async void BuildFilePanelOnLoad(String tableName, String parameterName, int currItem) {

            List<Image> imageValues = new List<Image>();
            List<EventHandler> onPressedEvent = new List<EventHandler>();
            List<EventHandler> onMoreOptionButtonPressed = new List<EventHandler>();

            List<(string, string, string)> filesInfo = new List<(string, string, string)>();

            const string selectFileDataDir = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (MySqlCommand command = new MySqlCommand(selectFileDataDir, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                command.Parameters.AddWithValue("@ext", _loadedExtensionType);
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate, String.Empty));
                    }
                }
            }

            List<string> base64EncodedImage = new List<string>();

            if (Globals.imageTypes.Contains(_loadedExtensionType)) {

                if (base64EncodedImage.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", _loadedExtensionType);

                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64EncodedImage.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                            }
                        }
                    }
                }
            }

            List<string> base64EncodedThumbnail = new List<string>();

            if (Globals.videoTypes.Contains(_loadedExtensionType)) {

                if (base64EncodedThumbnail.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", _loadedExtensionType);
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

                        PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername);
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
                        VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername);
                        vidFormShow.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }


                if (tableName == GlobalsTable.homeTextTable) {

                    string textTypes = fileName.Substring(fileName.LastIndexOf('.')).TrimStart();
                    imageValues.Add(Globals.textTypeToImage[textTypes]);

                    void videoOnPressed(object sender, EventArgs e) {
                        TextForm displayPic = new TextForm("", GlobalsTable.directoryUploadTable, fileName, lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeExeTable) {

                    imageValues.Add(Globals.EXEImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        exeFORM displayExe = new exeFORM(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername);
                        displayExe.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeExcelTable) {

                    imageValues.Add(Globals.EXCELImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ExcelForm exlForm = new ExcelForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername);
                        exlForm.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeAudioTable) {

                    imageValues.Add(Globals.AudioImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        AudioForm audioOnPressed = new AudioForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername);
                        audioOnPressed.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeApkTable) {

                    imageValues.Add(Globals.APKImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ApkForm displayPic = new ApkForm(fileName, Globals.custUsername, GlobalsTable.directoryUploadTable, lblDirectoryName.Text);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homePdfTable) {

                    imageValues.Add(Globals.PDFImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PdfForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homePtxTable) {

                    imageValues.Add(Globals.PTXImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PtxForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (tableName == GlobalsTable.homeMsiTable) {

                    imageValues.Add(Globals.MSIImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new MsiForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);

                }

                if (tableName == GlobalsTable.homeWordTable) {

                    imageValues.Add(Globals.DOCImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new WordDocForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, Globals.custUsername).Show();
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

        private async Task startSending(string setValue) {

            try {

                const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)";
                var param = new Dictionary<string, string>
                {
                    { "@CUST_USERNAME", Globals.custUsername},
                    { "@CUST_FILE_PATH", EncryptionModel.Encrypt(_fileName)},
                    { "@UPLOAD_DATE", _todayDate},
                    { "@CUST_FILE", setValue},
                    { "@CUST_THUMB", ""},
                    { "@FILE_EXT", _uploadedExtensionType},
                    { "@DIR_NAME", EncryptionModel.Encrypt(lblDirectoryName.Text)}
                };

                await crud.Insert(insertQuery, param);

                Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadingAlert")).ToList().ForEach(form => form.Close());

            }
            catch (Exception) {
                new CustomAlert("Something went wrong", "Failed to upload this file.").Show();
            }
        }

        private async Task containThumbUpload(string filePath, object keyValMain) {

            const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)";
            using (var command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(_fileName));
                command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                command.Parameters.AddWithValue("@FILE_EXT", _uploadedExtensionType);
                command.Parameters.AddWithValue("@UPLOAD_DATE", _todayDate);
                command.Parameters.AddWithValue("@DIR_NAME", EncryptionModel.Encrypt(lblDirectoryName.Text));
                command.Parameters.AddWithValue("@CUST_FILE", keyValMain);

                using (var shellFile = ShellFile.FromFilePath(filePath))
                using (var stream = new MemoryStream()) {
                    shellFile.Thumbnail.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    string base64Thumb = Convert.ToBase64String(stream.ToArray());
                    command.Parameters.AddWithValue("@CUST_THUMB", base64Thumb);
                }

                await command.ExecuteNonQueryAsync();
            }

            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadingAlert")).ToList().ForEach(form => form.Close());

        }

        private async void CreateFilePanel(string fileFullPath, string tableName, string panName, int itemCurr, string keyVal) {

            if (_fileSizeInMB < 8000) {

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

                flwLayoutDirectory.Controls.Add(panelTxt);

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
                titleLab.Text = _fileName;

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

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                new Thread(() => new UploadingAlert(_fileName, "null", panName + itemCurr, "null", fileSize: _fileSizeInMB).ShowDialog())
                .Start();

                if (tableName == GlobalsTable.homeImageTable) {

                    await startSending(keyVal);

                    textboxPic.Image = new Bitmap(fileFullPath);
                    textboxPic.Click += (sender_f, e_f) => {

                        var getImgName = (Guna2PictureBox)sender_f;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, _fileName, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    };

                }

                if (tableName == GlobalsTable.homeTextTable) {

                    await startSending(keyVal);

                    string textType = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    textboxPic.Image = Globals.textTypeToImage[textType];

                    var filePath = _fileName;

                    textboxPic.Click += (sender_t, e_t) => {
                        TextForm txtFormShow = new TextForm("", "upload_info_directory", titleLab.Text, lblDirectoryName.Text, Globals.custUsername);
                        txtFormShow.Show();
                    };
                }

                if (tableName == GlobalsTable.homeExeTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.EXEImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        new exeFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername).Show();
                    };
                }

                if (tableName == GlobalsTable.homeVideoTable) {

                    await containThumbUpload(fileFullPath, keyVal);

                    ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                    textboxPic.Image = toBitMap;

                    textboxPic.Click += (sender_ex, e_ex) => {
                        var getImgName = (Guna2PictureBox)sender_ex;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                        VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        vidShow.Show();
                    };

                }
                if (tableName == GlobalsTable.homeAudioTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.AudioImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        AudioForm displayPic = new AudioForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.homeExcelTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.EXCELImage;
                    textboxPic.Click += (sender_ex, e_ex) => {
                        ExcelForm displayPic = new ExcelForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (tableName == GlobalsTable.homeApkTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.APKImage;
                    textboxPic.Click += (sender_gi, e_gi) => {
                        ApkForm displayPic = new ApkForm(titleLab.Text, Globals.custUsername, "upload_info_directory", lblDirectoryName.Text);
                        displayPic.Show();
                    };
                }
                if (tableName == GlobalsTable.homePdfTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.PDFImage;
                    textboxPic.Click += (sender_pd, e_pd) => {
                        PdfForm displayPdf = new PdfForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPdf.ShowDialog();
                    };
                }
                if (tableName == GlobalsTable.homePtxTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.PTXImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        PtxForm displayPtx = new PtxForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPtx.ShowDialog();
                    };
                }
                if (tableName == GlobalsTable.homeMsiTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.MSIImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        MsiForm displayMsi = new MsiForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayMsi.Show();
                    };
                }

                if (tableName == GlobalsTable.homeWordTable) {

                    await startSending(keyVal);

                    textboxPic.Image = Globals.DOCImage;
                    textboxPic.Click += (sender_ptx, e_ptx) => {
                        WordDocForm displayWord = new WordDocForm(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayWord.ShowDialog();
                    };
                }

                remButTxt.Click += (sender_tx, e_tx) => {

                    pnlFileOptions.Visible = true;
                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFilePanelName.Text = panelTxt.Name;
                };

            }
            else {
                MessageBox.Show("File is too large, max file size is 8GB.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            foreach (var form in Application.OpenForms.OfType<Form>().Where(form => form.Name == "UploadingAlert").ToList()) {
                form.Close();
            }
        }


        private void OpenDialogUpload() {

            var form1 = HomePage.instance;

            var open = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = true
            };

            _todayDate = DateTime.Now.ToString("dd/MM/yyyy");

            int curFilesCount = flwLayoutDirectory.Controls.Count;

            if (open.ShowDialog() == DialogResult.OK) {

                List<string> _filValues = open.FileNames.Select(Path.GetFileName).ToList();

                if (open.FileNames.Length + curFilesCount > Globals.uploadFileLimit[Globals.accountType]) {
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

                    _filValues.Clear();

                }
                else {

                    HashSet<string> existingLabels = new HashSet<string>(flwLayoutDirectory.Controls
                        .OfType<Guna2Panel>()
                        .SelectMany(panel => panel.Controls.OfType<Label>())
                        .Select(label => label.Text.ToLower()));

                    foreach (var selectedItems in open.FileNames) {

                        if (existingLabels.Contains(Path.GetFileName(selectedItems).ToLower().Trim())) {
                            continue;
                        }

                        _filValues.Add(Path.GetFileName(selectedItems));

                        _fileName = Path.GetFileName(selectedItems);
                        _uploadedExtensionType = Path.GetExtension(selectedItems);
                        _fileSizeInMB = 0;

                        try {

                            byte[] getBytesSelectedFiles = File.ReadAllBytes(selectedItems);
                            byte[] compressedBytes = new GeneralCompressor().compressFileData(getBytesSelectedFiles);

                            string toBase64String = Convert.ToBase64String(compressedBytes);
                            string encryptBase64String = UniqueFile.IgnoreEncryptionFolder(_uploadedExtensionType) 
                                            ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                            _fileSizeInMB = (getBytesSelectedFiles.Length / 1024) / 1024;

                            if (Globals.imageTypes.Contains(_uploadedExtensionType)) {
                                curr++;

                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                string _compressedImageBase64 = compressor.compresImageToBase64(selectedItems);
                                string _encryptedValue = EncryptionModel.Encrypt(_compressedImageBase64);
                                CreateFilePanel(selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, _encryptedValue);
                                
                            }
                            else if (Globals.textTypes.Contains(_uploadedExtensionType)) {
                                txtCurr++;

                                string nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }

                                byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                                byte[] compressedTextBytes = new GeneralCompressor().compressFileData(getBytes);
                                string getEncoded = Convert.ToBase64String(compressedTextBytes);
                                string encryptEncodedText = EncryptionModel.Encrypt(getEncoded);

                                CreateFilePanel(selectedItems, GlobalsTable.homeTextTable, "PanTxt", txtCurr, encryptEncodedText);

                            }
                            else if (_uploadedExtensionType == ".exe") {
                                exeCurr++;
                                CreateFilePanel(selectedItems, "file_info_exe", "PanExe", exeCurr, encryptBase64String);

                            }
                            else if (Globals.videoTypes.Contains(_uploadedExtensionType)) {
                                vidCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".xlsx" || _uploadedExtensionType == ".xls") {
                                exlCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".mp3" || _uploadedExtensionType == ".wav") {
                                audCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".apk") {
                                apkCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".pdf") {
                                pdfCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".pptx" || _uploadedExtensionType == ".ppt") {
                                ptxCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".msi") {
                                msiCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptBase64String);
                            }
                            else if (_uploadedExtensionType == ".docx") {
                                docxCurr++;
                                CreateFilePanel(selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptBase64String);
                            }

                            CloseForm.closeForm("UploadingAlert");

                        }
                        catch (Exception) {
                            CloseForm.closeForm("UploadingAlert");
                            new CustomAlert(title: "Some went wrong", subheader: "Failed to upload this file.").Show();
                        }

                    }
                }
            }

            lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";
            BuildRedundaneVisibility();

        }

        public void DisplayError() {
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

                if (currentUploadCount != Globals.uploadFileLimit[Globals.accountType]) {
                    OpenDialogUpload();
                }
                else {
                    DisplayError();
                }

            }
            catch (Exception) {

                CloseForm.closeForm("UploadingAlert");

                new CustomAlert(title: "An error occurred", "Something went wrong while trying to upload files.").Show();

            }
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }

        private void guna2Button26_Click(object sender, EventArgs e) {

            string fileName = lblFileNameOnPanel.Text;
            string panelname = lblFilePanelName.Text;

            DialogResult verifyDialog = MessageBox.Show($"Delete '{fileName}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (verifyDialog == DialogResult.Yes) {
                using (var command = con.CreateCommand()) {
                    const string noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                    command.CommandText = noSafeUpdate;
                    command.ExecuteNonQuery();
                }

                using (var command = con.CreateCommand()) {
                    const string removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command.CommandText = removeQuery;
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
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

            }
        }

        private void guna2Button28_Click(object sender, EventArgs e) {
            pnlFileOptions.Visible = false;
        }

        private void guna2Button30_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = "upload_info_directory";
            string panelName = lblFilePanelName.Text;
            string dirName = lblDirectoryName.Text;

            RenameFileForm renameFileFORM = new RenameFileForm(titleFile, tableName, panelName, dirName);
            renameFileFORM.Show();
        }

        private void guna2Button32_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            SaverModel.SaveSelectedFile(titleFile, "upload_info_directory", dirName);

        }

        private void guna2Button29_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            string fileExtensions = titleFile.Split('.').Last();

            shareFileFORM sharingFileFORM = new shareFileFORM(titleFile, fileExtensions, false, GlobalsTable.directoryUploadTable, dirName);
            sharingFileFORM.Show();
        }
    }
}