using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Authentication;
using FlowstorageDesktop.ExtraForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Query;
using FlowstorageDesktop.Temporary;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using DiscordRPC;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowstorageDesktop.Query.DataCaller;

namespace FlowstorageDesktop {

    public partial class HomePage : Form {

        readonly private Crud crud = new Crud();

        readonly private InsertFileDataQuery insertFileData = new InsertFileDataQuery();

        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private CurrencyConverter currencyConverter = new CurrencyConverter();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        readonly private HomeDataCaller homeDataCaller = new HomeDataCaller();
        readonly private PsDataCaller psDataCaller = new PsDataCaller();
        readonly private SharedFilesDataCaller sharedFilesDataCaller = new SharedFilesDataCaller();
        readonly private SharedToMeDataCaller sharedToMeDataCaller = new SharedToMeDataCaller();
        readonly private FolderDataCaller folderDataCaller = new FolderDataCaller();
        readonly private DirectoryDataCaller directoryDataCaller = new DirectoryDataCaller();

        public static HomePage instance { get; set; } = new HomePage();
        public bool CallInitialStartupData { get; set; } = false;
        public string PublicStorageUserComment { get; set; } = null;
        public string PublicStorageUserTitle { get; set; } = null;
        public string PublicStorageUserTag { get; set; } = null;
        public bool PublicStorageClosed { get; set; } = false;
        private bool _isMyPublicStorageSelected { get; set; }
        private string _todayDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        private string previousSelectedItem = null;

        public HomePage() {

            InitializeComponent();

            instance = this;

            this.AllowDrop = true;

            this.DragEnter += new DragEventHandler(HomePage_DragEnter);
            this.DragOver += new DragEventHandler(HomePage_DragOver);
            this.DragDrop += new DragEventHandler(HomePage_DragDrop);
            this.DragLeave += new EventHandler(HomePage_DragLeave);

            this.flwLayoutHome.HorizontalScroll.Maximum = 0;
            this.flwLayoutHome.VerticalScroll.Maximum = 0;
            this.flwLayoutHome.AutoScrollMinSize = new Size(0, 0);

            this.flwLayoutHome.BorderStyle = BorderStyle.None;

            this.flwLayoutHome.AutoScroll = true;
            this.flwLayoutHome.HorizontalScroll.Visible = false;
            this.flwLayoutHome.VerticalScroll.Visible = false;

            this.TopMost = false;

        }

        private void HomePage_Load(object sender, EventArgs e) {
            InitializeHomeFiles();
            InitializeDiscordRPC();
        }

        private void InitializeDiscordRPC() {

            string clientId = ConfigurationManager.ConnectionStrings["discRp"].ConnectionString;

            var client = new DiscordRpcClient(clientId);
            client.Initialize();

            client.SetPresence(new RichPresence() {
                State = "Searching for lost files...",
                Assets = new Assets() {
                    LargeImageKey = "group_15_1_"
                }
            });

        }

        private void btnCreateDirectory_Click(object sender, EventArgs e) => new CreateDirectoryForm().Show();
        private void btnOpenMainShare_Click(object sender, EventArgs e) => new MainShareFileForm().Show();
        private void btnOpenRenameFolderFile_Click(object sender, EventArgs e)
            => new RenameFolderFileForm(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem)).Show();
        private void BuildShowAlert(string title, string subheader)
            => new CustomAlert(title: title, subheader: subheader).Show();

        private void UpdateProgressBarValue() {

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            int getCurrentCount = int.Parse(lblItemCountText.Text);
            int getLimitedValue = int.Parse(lblLimitUploadText.Text);
            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
            lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

            progressBarUsageStorage.Value = calculatePercentageUsage;

        }

        private async void InitializeHomeFiles() {

            BuildButtonsOnHomePageSelected();

            if(CallInitialStartupData) {
                await BuildHomeFiles();
                UpdateProgressBarValue();
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

        int paramCurr = 0;

        #region Home section

        /// <summary>
        /// 
        /// Generate user Home files panel on startup
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>
        /// <returns></returns>
        private async Task BuildFilePanelHome(string tableName, string parameterName, int currItem) {

            var imageValues = new List<Image>();
            var onPressedEvent = new List<EventHandler>();
            var onMoreOptionButtonPressed = new List<EventHandler>();

            try {

                List<(string, string, string)> filesInfo = await homeDataCaller.GetFileMetadata(tableName);

                if (tableName == GlobalsTable.homeImageTable) {
                    if (GlobalsData.base64EncodedImageHome.Count == 0) {
                        await homeDataCaller.AddImageCaching();

                    }
                }

                if (tableName == GlobalsTable.homeVideoTable) {
                    if (GlobalsData.base64EncodedThumbnailHome.Count == 0) {
                        await homeDataCaller.AddVideoThumbnailCaching();

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

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.homeImageTable, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (tableName == GlobalsTable.homeTextTable) {

                        imageValues.Add(Globals.TextImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.homeTextTable, filesInfo[accessIndex].Item1, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.homeExeTable) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            ExeForm displayExe = new ExeForm(filesInfo[accessIndex].Item1, GlobalsTable.homeExeTable, string.Empty, tempDataUser.Username);
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
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.homeVideoTable, string.Empty, tempDataUser.Username);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (tableName == GlobalsTable.homeExcelTable) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfo[accessIndex].Item1, GlobalsTable.homeExcelTable, string.Empty, tempDataUser.Username);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (tableName == GlobalsTable.homeAudioTable) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfo[accessIndex].Item1, GlobalsTable.homeAudioTable, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (tableName == GlobalsTable.homeApkTable) {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfo[accessIndex].Item1, tempDataUser.Username, GlobalsTable.homeApkTable, string.Empty);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (tableName == GlobalsTable.homePdfTable) {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfo[accessIndex].Item1, GlobalsTable.homePdfTable, string.Empty, tempDataUser.Username);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (tableName == GlobalsTable.homePtxTable) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfo[accessIndex].Item1, GlobalsTable.homePtxTable, string.Empty, tempDataUser.Username);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (tableName == GlobalsTable.homeMsiTable) {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfo[accessIndex].Item1, GlobalsTable.homeMsiTable, string.Empty, tempDataUser.Username);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (tableName == GlobalsTable.homeWordTable) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordsForm displayMsi = new WordsForm(filesInfo[accessIndex].Item1, GlobalsTable.homeWordTable, string.Empty, tempDataUser.Username);
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
                BuildShowAlert(
                    title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");
            }

        }

        private async Task CreateFilePanelHome(string fileFullPath, string tableName, string fileData) {

            string fileName = Path.GetFileName(fileFullPath);

            string paramName = paramCurr.ToString();

            var panel = new Guna2Panel() {
                Name = paramName,
                Width = 200,
                Height = 222,
                BorderColor = GlobalStyle.BorderColor,
                BorderThickness = 1,
                BorderRadius = 12,
                BackColor = GlobalStyle.TransparentColor,
                Location = new Point(600, Globals.PANEL_GAP_TOP)
            };

            Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

            var panelImage = new Guna2PictureBox();
            panel.Controls.Add(panelImage);
            panelImage.Name = paramName;
            panelImage.BorderRadius = 12;
            panelImage.Width = 190;
            panelImage.Height = 145;
            panelImage.SizeMode = PictureBoxSizeMode.CenterImage;

            panelImage.Anchor = AnchorStyles.None;

            int panelImageLoc_x = (panel.Width - panelImage.Width) / 2;

            panelImage.Location = new Point(panelImageLoc_x, 10);

            Label dateLabel = new Label();
            panel.Controls.Add(dateLabel);
            dateLabel.Name = paramName;
            dateLabel.Font = GlobalStyle.DateLabelFont;
            dateLabel.ForeColor = GlobalStyle.DarkGrayColor;
            dateLabel.Location = GlobalStyle.DateLabelLoc;
            dateLabel.Text = _todayDate;

            Label titleLabel = new Label();
            panel.Controls.Add(titleLabel);
            titleLabel.Name = paramName;
            titleLabel.Font = GlobalStyle.TitleLabelFont;
            titleLabel.ForeColor = GlobalStyle.GainsboroColor;
            titleLabel.Location = GlobalStyle.TitleLabelLoc;
            titleLabel.Width = 160;
            titleLabel.Height = 20;
            titleLabel.Text = fileName;
            titleLabel.AutoEllipsis = true;

            Guna2Button moreOptionsButton = new Guna2Button();
            panel.Controls.Add(moreOptionsButton);
            moreOptionsButton.Name = paramName;
            moreOptionsButton.Width = 29;
            moreOptionsButton.Height = 26;
            moreOptionsButton.ImageOffset = GlobalStyle.GarbageOffset;
            moreOptionsButton.FillColor = GlobalStyle.TransparentColor;
            moreOptionsButton.BorderRadius = 6;
            moreOptionsButton.BorderThickness = 1;
            moreOptionsButton.BorderColor = GlobalStyle.TransparentColor;
            moreOptionsButton.Image = GlobalStyle.GarbageImage;
            moreOptionsButton.Location = GlobalStyle.GarbageButtonLoc;

            moreOptionsButton.Click += (sender_tx, e_tx) => {
                lblFileNameOnPanel.Text = titleLabel.Text;
                lblFileTableName.Text = tableName;
                lblFilePanelName.Text = panel.Name;
                pnlFileOptions.Visible = true;
            };

            panelImage.MouseHover += (_senderM, _ev) => {
                panel.ShadowDecoration.Enabled = true;
                panel.ShadowDecoration.BorderRadius = 12;
            };

            panelImage.MouseLeave += (_senderQ, _evQ) => {
                panel.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.homeImageTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                GlobalsData.base64EncodedImageHome.Add(EncryptionModel.Decrypt(fileData));

                panelImage.Image = new Bitmap(fileFullPath);
                panelImage.Click += (sender_f, e_f) => {

                    var imageName = (Guna2PictureBox)sender_f;
                    var imageWidth = imageName.Image.Width;
                    var imageHeight = imageName.Image.Height;

                    var defaultImage = new Bitmap(imageName.Image);

                    new PicForm(
                        defaultImage, imageWidth, imageHeight, fileName, GlobalsTable.homeImageTable, string.Empty, tempDataUser.Username).ShowDialog();
                };

            }

            if (tableName == GlobalsTable.homeTextTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.TextImage;
                panelImage.Click += (sender_t, e_t) => {
                    new TextForm(
                        GlobalsTable.homeTextTable, fileName, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeExeTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.EXEImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExeForm(
                        titleLabel.Text, GlobalsTable.homeExeTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeVideoTable) {

                await insertFileData.InsertFileDataVideo(fileFullPath, tableName, fileName, fileData);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                panelImage.Image = toBitMap;

                panelImage.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    var defaultImg = new Bitmap(getImgName.Image);

                    new VideoForm(
                        defaultImg, getWidth, getHeight, titleLabel.Text, GlobalsTable.homeVideoTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homeAudioTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.AudioImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new AudioForm(
                        titleLabel.Text, GlobalsTable.homeAudioTable, string.Empty, tempDataUser.Username).ShowDialog();
                };

            }

            if (tableName == GlobalsTable.homeExcelTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.EXCELImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExcelForm(
                        titleLabel.Text, GlobalsTable.homeExcelTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeApkTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.APKImage;
                panelImage.Click += (sender_gi, e_gi) => {
                    new ApkForm(
                        titleLabel.Text, tempDataUser.Username, GlobalsTable.homeApkTable, string.Empty).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homePdfTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.PDFImage;
                panelImage.Click += (sender_pd, e_pd) => {
                    new PdfForm(
                        titleLabel.Text, GlobalsTable.homePdfTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homePtxTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.PTXImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new PtxForm(
                        titleLabel.Text, GlobalsTable.homePtxTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeMsiTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.MSIImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new MsiForm(
                        titleLabel.Text, GlobalsTable.homeMsiTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeWordTable) {

                await insertFileData.InsertFileData(fileName, fileData, tableName);

                panelImage.Image = Globals.DOCImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new WordsForm(
                        titleLabel.Text, GlobalsTable.homeWordTable, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            flwLayoutHome.Controls.Add(panel);

        }

        private async void OpenDialogHomeFile() {

            var selectFilesDialog = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = true
            };

            int currentFilesCount = flwLayoutHome.Controls.Count;

            if (selectFilesDialog.ShowDialog() == DialogResult.OK) {

                if (selectFilesDialog.FileNames.Length + currentFilesCount > Globals.uploadFileLimit[tempDataUser.AccountType]) {
                    DisplayUpgradeAccountDialog();
                    return;
                }

                var fileNameLabels = new HashSet<string>(flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                GlobalsData.filesMetadataCacheHome.Clear();

                foreach (var selectedItems in selectFilesDialog.FileNames) {

                    string selectedFileName = Path.GetFileName(selectedItems);
                    string fileType = selectedFileName.Split('.').Last();

                    if (fileNameLabels.Contains(selectedFileName.ToLower().Trim())) {
                        continue;
                    }

                    try {

                        byte[] originalRetrieveBytes = File.ReadAllBytes(selectedItems);
                        byte[] compressedBytes = new GeneralCompressor().compressFileData(originalRetrieveBytes);

                        string convertToBase64 = Convert.ToBase64String(compressedBytes);
                        string encryptText = UniqueFile.IgnoreEncryption(fileType) 
                            ? convertToBase64 : EncryptionModel.Encrypt(convertToBase64);

                        paramCurr++;

                        if (Globals.imageTypes.Contains(fileType)) {
                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImage = compressor.compressImageToBase64(selectedItems);
                            string encryptedValue = EncryptionModel.Encrypt(compressedImage);

                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeImageTable, encryptedValue);
                                
                        } else if (Globals.textTypes.Contains(fileType)) {
                            string nonLine = "";
                            using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                nonLine = ReadFileTxt.ReadToEnd();
                            }

                            byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                            byte[] compressedTextBytes = compressor.compressFileData(getBytes);
                            string getEncoded = Convert.ToBase64String(compressedTextBytes);
                            string encryptEncodedText = EncryptionModel.Encrypt(getEncoded);

                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeTextTable, encryptEncodedText);

                        } else if (fileType == "exe") {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeExeTable, encryptText);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeVideoTable, encryptText);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeExcelTable, encryptText);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeAudioTable, encryptText);

                        } else if (fileType == "apk") {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeApkTable, encryptText);

                        } else if (fileType == "pdf") {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homePdfTable, encryptText);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homePtxTable, encryptText);

                        } else if (fileType == "msi") {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeMsiTable, encryptText);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeWordTable, encryptText);

                        } else {
                            BuildShowAlert(title: "Upload Failed","File type is not supported.");

                        }

                        ClosePopupForm.CloseUploadingPopup();

                    } catch (Exception) {
                        ClosePopupForm.CloseUploadingPopup();

                    }

                    lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                }
            }

            UpdateProgressBarValue();
            BuildRedundaneVisibility();

            ClosePopupForm.CloseUploadingPopup();

        }

        private async Task BuildHomeFiles() {

            var directoriesName = await homeDataCaller.GetDirectories();

            foreach (string tableName in GlobalsTable.publicTables) {
                if (GlobalsTable.tableToFileType.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileType[tableName];
                    if (fileType != null) {
                        ClearRedundane();
                        await BuildFilePanelHome(tableName, fileType, await crud.CountUserTableRow(tableName));

                    } else {
                        BuildDirectoryPanel(directoriesName, await crud.CountUserTableRow(tableName));

                    }
                }
            }

            UpdateProgressBarValue();
            BuildRedundaneVisibility();

        }

        private async Task RefreshHomePanels() {

            btnDeleteFolder.Visible = false;

            GlobalsData.filesMetadataCacheHome.Clear();

            var directoriesName = await homeDataCaller.GetDirectories();

            foreach (string tableName in GlobalsTable.publicTables) {
                if (GlobalsTable.tableToFileType.ContainsKey(tableName)) {
                    string fileType = GlobalsTable.tableToFileType[tableName];
                    if (fileType != null) {
                        ClearRedundane();
                        await BuildFilePanelHome(tableName, fileType, await crud.CountUserTableRow(tableName));

                    } else {
                        BuildDirectoryPanel(directoriesName, await crud.CountUserTableRow(tableName));

                    }
                }
            }

            BuildRedundaneVisibility();
            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        #endregion END - Home section

        #region Public Storage section

        private async Task BuildFilePanelPublicStorage(string tableName, string parameterName, int currItem, bool isFromMyPs = false) {

            var imageValues = new List<Image>();
            var onPressedEvent = new List<EventHandler>();
            var onMoreOptionButtonPressed = new List<EventHandler>();

            try {

                List<(string, string, string, string)> filesInfo;

                if (!isFromMyPs) {
                    filesInfo = await psDataCaller.GetFileMetadata(tableName);

                } else {
                    filesInfo = new List<(string, string, string, string)>();

                }

                if (isFromMyPs) {
                    GlobalsData.base64EncodedImagePs.Clear();
                    GlobalsData.base64EncodedThumbnailPs.Clear();

                }

                if (isFromMyPs) {
                    filesInfo = await psDataCaller.GetFileMetadataMyPs(tableName);
                    
                }

                var usernameList = new List<string>();

                if (!isFromMyPs) {
                    var uploaderName = await psDataCaller.GetUploaderName(tableName);
                    usernameList.AddRange(uploaderName);

                } else {
                    var uploaderName = await psDataCaller.GetUploaderNameMyPs(tableName);
                    usernameList.AddRange(uploaderName);

                }

                if (tableName == GlobalsTable.psImage) {
                    if (GlobalsData.base64EncodedImagePs.Count == 0) {
                        await psDataCaller.AddImageCaching(isFromMyPs);

                    }
                }

                if (tableName == GlobalsTable.psVideo) {
                    if (GlobalsData.base64EncodedThumbnailPs.Count == 0) {
                        await psDataCaller.AddVideoThumbnailCaching(isFromMyPs);
                        
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

                            new PicForm(
                                defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psImage, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (tableName == GlobalsTable.psText) {

                        imageValues.Add(Globals.TextImage);

                        void textOnPressed(object sender, EventArgs e) {
                            new TextForm(
                                GlobalsTable.psText, filesInfo[accessIndex].Item1, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.psExe) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            new ExeForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psExe, string.Empty, uploaderName).ShowDialog();
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
                            var defaultImage = new Bitmap(getImgName.Image);

                            new VideoForm(
                                defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psVideo, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (tableName == GlobalsTable.psExcel) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            new ExcelForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psExcel, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (tableName == GlobalsTable.psAudio) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            new AudioForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psAudio, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (tableName == GlobalsTable.psApk) {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            new ApkForm(
                                filesInfo[accessIndex].Item1, uploaderName, GlobalsTable.psApk, string.Empty).ShowDialog();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (tableName == GlobalsTable.psPdf) {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            new PdfForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psPdf, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (tableName == GlobalsTable.psPtx) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            new PtxForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psPtx, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (tableName == GlobalsTable.psMsi) {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            new MsiForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psMsi, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (tableName == GlobalsTable.psWord) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            new WordsForm(
                                filesInfo[accessIndex].Item1, GlobalsTable.psWord, string.Empty, uploaderName).ShowDialog();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePublicStoragePanel(parameterName, currItem, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues, usernameList, moreButtonVisible: isFromMyPs);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");

            }            

        }

        private async Task CreateFilePanelPublicStorage(string fileFullPath, string tableName, string parameterName, string keyVal) {

            string fileName = Path.GetFileName(fileFullPath);

            var panel = new Guna2Panel() {
                Name = parameterName + paramCurr,
                Width = 280,
                Height = 268,
                BorderColor = GlobalStyle.BorderColor,
                BorderThickness = 1,
                BorderRadius = 12,
                BackColor = GlobalStyle.TransparentColor,
                Location = new Point(600, Globals.PANEL_GAP_TOP)
            };

            Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

            var panelImage = new Guna2PictureBox();
            panel.Controls.Add(panelImage);
            panelImage.Name = "pnlImage" + paramCurr;
            panelImage.BorderRadius = 12;
            panelImage.Width = 270;
            panelImage.Height = 165;
            panelImage.SizeMode = PictureBoxSizeMode.CenterImage;

            panelImage.Anchor = AnchorStyles.None;

            int textboxPic_x = (panel.Width - panelImage.Width) / 2;

            panelImage.Location = new Point(textboxPic_x, 10);

            Label dateLabel = new Label();
            panel.Controls.Add(dateLabel);
            dateLabel.Name = "dateLbl" + paramCurr;
            dateLabel.BackColor = GlobalStyle.TransparentColor;
            dateLabel.Font = GlobalStyle.DateLabelFont;
            dateLabel.ForeColor = GlobalStyle.DarkGrayColor;
            dateLabel.Location = new Point(12, 241);
            dateLabel.Text = _todayDate;

            Guna2Panel tagBackground = new Guna2Panel();
            panel.Controls.Add(tagBackground);
            tagBackground.BorderRadius = 11;
            tagBackground.Location = new Point(12, 188);
            tagBackground.Size = new Size(108, 24);
            tagBackground.FillColor = GlobalStyle.psBackgroundColorTag[PublicStorageUserTag];
            tagBackground.BringToFront();

            Label tagLabel = new Label();
            panel.Controls.Add(tagLabel);
            tagLabel.Name = $"ButTag{paramCurr}";
            tagLabel.Font = GlobalStyle.PsLabelTagFont;
            tagLabel.Height = 15;
            tagLabel.Width = 85;
            tagLabel.BackColor = GlobalStyle.psBackgroundColorTag[PublicStorageUserTag];
            tagLabel.ForeColor = GlobalStyle.GainsboroColor;
            tagLabel.TextAlign = ContentAlignment.MiddleCenter;

            int centerX = (tagBackground.Width - tagLabel.Width) / 2;
            tagLabel.Location = new Point(centerX + 15, GlobalStyle.PsLabelTagLoc.Y+1);

            tagLabel.Text = PublicStorageUserTag;
            tagLabel.BringToFront();

            Label titleLabel = new Label();
            panel.Controls.Add(titleLabel);
            titleLabel.Name = "titleLbl" + paramCurr;
            titleLabel.Font = GlobalStyle.TitleLabelFont;
            titleLabel.ForeColor = GlobalStyle.GainsboroColor;
            titleLabel.Location = new Point(12, 218);
            titleLabel.Width = 200;
            titleLabel.Height = 20;
            titleLabel.AutoEllipsis = true;
            titleLabel.Text = fileName;

            panelImage.MouseHover += (_senderM, _ev) => {
                panel.ShadowDecoration.Enabled = true;
                panel.ShadowDecoration.BorderRadius = 12;
            };

            panelImage.MouseLeave += (_senderQ, _evQ) => {
                panel.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.psImage) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                GlobalsData.base64EncodedImagePs.Add(EncryptionModel.Decrypt(keyVal));

                panelImage.Image = new Bitmap(fileFullPath);
                panelImage.Click += (sender_f, e_f) => {

                    var getImgName = (Guna2PictureBox)sender_f;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    var defaultImage = new Bitmap(getImgName.Image);

                    new PicForm(
                        defaultImage, getWidth, getHeight, fileName, GlobalsTable.psImage, string.Empty, tempDataUser.Username).ShowDialog();
                };

            }

            if (tableName == GlobalsTable.psText) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.TextImage;
                panelImage.Click += (sender_t, e_t) => {
                    new TextForm(
                        GlobalsTable.psText, fileName, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psExe) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.EXEImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExeForm(
                        titleLabel.Text, GlobalsTable.psExe, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psVideo) {

                await insertFileData.InsertFileVideoDataPublic(fileFullPath, fileName, keyVal);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                panelImage.Image = toBitMap;

                panelImage.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    var defaultImg = new Bitmap(getImgName.Image);

                    new VideoForm(
                        defaultImg, getWidth, getHeight, titleLabel.Text, GlobalsTable.psVideo, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psAudio) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.AudioImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new AudioForm(
                        titleLabel.Text, GlobalsTable.psAudio, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psExcel) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.EXCELImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExcelForm(
                        titleLabel.Text, GlobalsTable.psExcel, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psApk) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.APKImage;
                panelImage.Click += (sender_gi, e_gi) => {
                    new ApkForm(
                        titleLabel.Text, tempDataUser.Username, GlobalsTable.psApk, string.Empty).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psPdf) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.PDFImage;
                panelImage.Click += (sender_pd, e_pd) => {
                    new PdfForm(
                        titleLabel.Text, GlobalsTable.psPdf, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psPtx) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.PTXImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new PtxForm(
                        titleLabel.Text, GlobalsTable.psPtx, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }
            if (tableName == GlobalsTable.psMsi) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.MSIImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new MsiForm(
                        titleLabel.Text, GlobalsTable.psMsi, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psWord) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                panelImage.Image = Globals.DOCImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new WordsForm(
                        titleLabel.Text, GlobalsTable.psWord, string.Empty, tempDataUser.Username).ShowDialog();
                };
            }

            flwLayoutHome.Controls.Add(panel);

        }

        private async void OpenDialogPublicStorage() {

            var selectFilesDialog = new OpenFileDialog {
                Filter = Globals.filterFileType,
                Multiselect = false
            };

            int currentFilesCount = flwLayoutHome.Controls.Count;

            if (selectFilesDialog.ShowDialog() == DialogResult.OK) {

                if (selectFilesDialog.FileNames.Length + currentFilesCount > Globals.uploadFileLimit[tempDataUser.AccountType]) {
                    DisplayUpgradeAccountDialog();
                    return;
                } 

                var fileNameLabels = new HashSet<string>(flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                string selectedItems = selectFilesDialog.FileName;

                string selectedFileName = Path.GetFileName(selectedItems);
                string fileType = Path.GetExtension(selectedItems);

                if (fileNameLabels.Contains(selectedFileName.ToLower().Trim())) {
                    BuildShowAlert(
                        title: "Upload Failed", $"A file with the same name is already uploaded to Public Storage. File name: {selectedFileName}");
                    return;
                }

                GlobalsData.filesMetadataCachePs.Clear();

                try {

                    new PublishPublicStorage(fileName: selectedFileName).ShowDialog();

                    if (!PublicStorageClosed) {

                        byte[] retrieveBytes = File.ReadAllBytes(selectedItems);
                        byte[] compressedBytes = new GeneralCompressor().compressFileData(retrieveBytes);

                        string toBase64String = Convert.ToBase64String(compressedBytes);
                        string encryptText = UniqueFile.IgnoreEncryption(fileType) 
                            ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                        paramCurr++;

                        if (Globals.imageTypes.Contains(fileType)) {
                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImage = compressor.compressImageToBase64(selectedItems);
                            string encryptedImage = EncryptionModel.Encrypt(compressedImage);
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psImage, "PanImg", encryptedImage);
       
                        } else if (Globals.textTypes.Contains(fileType)) {
                            string nonLine = "";
                            using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                nonLine = ReadFileTxt.ReadToEnd();
                            }

                            byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                            string getEncoded = Convert.ToBase64String(getBytes);
                            string encryptTextValues = EncryptionModel.Encrypt(getEncoded);
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psText, "PanTxt", encryptTextValues);

                        } else if (fileType == "exe") {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psExe, "PanExe", encryptText);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psVideo, "PanVid", encryptText);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psExcel, "PanExl", encryptText);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psAudio, "PanAud", encryptText);

                        } else if (fileType == "apk") {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psApk, "PanApk", encryptText);

                        } else if (fileType == "pdf") {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psPdf, "PanPdf", encryptText);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psPtx, "PanPtx", encryptText);

                        } else if (fileType == "msi") {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psMsi, "PanMsi", encryptText);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psWord, "PanDoc", encryptText);

                        } else {
                            BuildShowAlert(title: "Upload Failed", "File type is not supported.");

                        }

                        ClosePopupForm.CloseUploadingPopup();

                    }

                } catch (Exception) {
                    ClosePopupForm.CloseUploadingPopup();

                }

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();
               
            }

            PublicStorageClosed = false;

            UpdateProgressBarValue();
            BuildRedundaneVisibility();

            ClosePopupForm.CloseUploadingPopup();

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

            lblIPsILimitedText.Text = Globals.uploadFileLimit[tempDataUser.AccountType].ToString();

            var username = new List<string>(flwLayoutHome.Controls
                .OfType<Guna2Panel>()
                .SelectMany(panel => panel.Controls.OfType<Label>())
                .Where(label => label.Text.Contains(tempDataUser.Username))
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

        List<(string, string, string)> filesInfoSharedOthers = new List<(string, string, string)>();
        private async Task CallFilesInformationOthers() {
            
            filesInfoSharedOthers.Clear();

            var filesInfo = await sharedFilesDataCaller.GetFileMetadata();
            filesInfoSharedOthers.AddRange(filesInfo);

        }

        private async Task BuildFilePanelSharedToOthers(string parameterName) {

            var imageValues = new List<Image>();
            var onPressedEvent = new List<EventHandler>();
            var onMoreOptionButtonPressed = new List<EventHandler>();

            var typeValues = filesInfoSharedOthers.Select(metadata => metadata.Item1.Split('.').Last()).ToList();

            var uploadToNameList = await sharedFilesDataCaller.GetSharedToUsername();

            int length = typeValues.Count;

            if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {
                if (GlobalsData.base64EncodedImageSharedOthers.Count == 0) {
                    await sharedFilesDataCaller.AddImageCaching();
                    
                }
            }

            for (int i = 0; i < length; i++) {

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
                        var defaultImage = new Bitmap(getImgName.Image);

                        new PicForm(
                            defaultImage, getWidth, getHeight, filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }


                    onPressedEvent.Add(imageOnPressed);

                }

                if (Globals.textTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.TextImage);

                    void textOnPressed(object sender, EventArgs e) {
                        new TextForm(
                            GlobalsTable.sharingTable, filesInfoSharedOthers[accessIndex].Item1, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(textOnPressed);
                }

                if (Globals.videoTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.VideoImage);

                    void videoOnPressed(object sender, EventArgs e) {

                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        var defaultImage = new Bitmap(getImgName.Image);

                        new VideoForm(
                            defaultImage, getWidth, getHeight, filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (Globals.excelTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.EXCELImage);

                    void excelOnPressed(object sender, EventArgs e) {
                        new ExcelForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(excelOnPressed);
                }

                if (Globals.audioTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.AudioImage);

                    void audioOnPressed(object sender, EventArgs e) {
                        new AudioForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(audioOnPressed);
                }

                if (typeValues[i] == "exe") {

                    imageValues.Add(Globals.EXEImage);

                    void exeOnPressed(object sender, EventArgs e) {
                        new ExeForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(exeOnPressed);
                }

                if (typeValues[i] == "apk") {

                    imageValues.Add(Globals.APKImage);

                    void apkOnPressed(object sender, EventArgs e) {
                        new ApkForm(
                            filesInfoSharedOthers[accessIndex].Item1, uploadToName, GlobalsTable.sharingTable, lblGreetingText.Text, true).ShowDialog();
                    }

                    onPressedEvent.Add(apkOnPressed);
                }

                if (typeValues[i] == "pdf") {

                    imageValues.Add(Globals.PDFImage);

                    void pdfOnPressed(object sender, EventArgs e) {
                        new PdfForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(pdfOnPressed);
                }

                if (Globals.ptxTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.PTXImage);

                    void ptxOnPressed(object sender, EventArgs e) {
                        new PtxForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(ptxOnPressed);
                }

                if (typeValues[i] == "msi") {

                    imageValues.Add(Globals.MSIImage);

                    void msiOnPressed(object sender, EventArgs e) {
                        new MsiForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(msiOnPressed);

                }

                if (Globals.wordTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.DOCImage);

                    void wordOnPressed(object sender, EventArgs e) {
                        new WordsForm(
                            filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploadToName, true).ShowDialog();
                    }

                    onPressedEvent.Add(wordOnPressed);
                }
            }

            PanelGenerator panelGenerator = new PanelGenerator();
            panelGenerator.GeneratePanel(parameterName, length, filesInfoSharedOthers, onPressedEvent, onMoreOptionButtonPressed, imageValues);

            BuildRedundaneVisibility();

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        private async Task BuildSharedToOthers() {

            await BuildFilePanelSharedToOthers("DirParOther");

            UpdateProgressBarValue();
            BuildRedundaneVisibility();

        }

        /// <summary>
        /// 
        /// Refresh Shared To Others panel
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="typeValuesOthersCache"></param>
        /// <param name="dirName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task RefreshGenerateUserSharedOthers(string dirName) {

            GlobalsData.base64EncodedImageSharedOthers.Clear();

            await CallFilesInformationOthers();
            await BuildFilePanelSharedToOthers(dirName);

        }

        #endregion END - Shared to others

        #region Shared to me section

        List<(string, string, string)> filesInfoSharedToMe = new List<(string, string, string)>();
        private async Task CallFilesInformationSharedToMe() {

            filesInfoSharedToMe.Clear();

            var filesInfo = await sharedToMeDataCaller.GetFileMetadata();
            filesInfoSharedToMe.AddRange(filesInfo);
            
        }

        private async Task BuildFilePanelSharedToMe(string parameterName) {

            try {

                var imageValues = new List<Image>();
                var onPressedEvent = new List<EventHandler>();
                var onMoreOptionButtonPressed = new List<EventHandler>();

                string uploaderUsername = await sharedToMeDataCaller.SharedToMeUploaderName();
                
                var typeValues = filesInfoSharedToMe.Select(metadata => metadata.Item1.Split('.').Last()).ToList();

                int length = typeValues.Count;

                if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {
                    if (GlobalsData.base64EncodedImageSharedToMe.Count == 0) {
                        await sharedToMeDataCaller.AddImageCaching();

                    }
                }

                for (int i = 0; i < length; i++) {

                    int accessIndex = i;

                    void moreOptionOnPressedEvent(object sender, EventArgs e) {
                        lblFileNameOnPanel.Text = filesInfoSharedToMe[accessIndex].Item1;
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

                            new PicForm(
                                defaultImage, getWidth, getHeight, filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.TextImage);

                        void textOnPressed(object sender, EventArgs e) {
                            new TextForm(
                                GlobalsTable.sharingTable, filesInfoSharedToMe[accessIndex].Item1, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.VideoImage);

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            var defaultImage = new Bitmap(getImgName.Image);

                            new VideoForm(
                                defaultImage, getWidth, getHeight, filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog(); 
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            new ExcelForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }

                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            new AudioForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == "exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            new ExeForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (typeValues[i] == "apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            new ApkForm(
                                filesInfoSharedToMe[accessIndex].Item1, uploaderUsername, GlobalsTable.sharingTable, lblGreetingText.Text, false).ShowDialog();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == "pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            new PdfForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            new PtxForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == "msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            new MsiForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            new WordsForm(
                                filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false).ShowDialog();
                        }
                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel(parameterName, length, filesInfoSharedToMe, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");

            }

        }

        private async Task BuildSharedToMe() {

            await BuildFilePanelSharedToMe("DirParMe");

            UpdateProgressBarValue();
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
        private async Task RefreshGenerateUserSharedToMe(string dirName) {

            GlobalsData.base64EncodedImageSharedToMe.Clear();

            await CallFilesInformationSharedToMe();
            await BuildFilePanelSharedToMe(dirName);

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

        private void OpenFolderDialog() {

            var dialog = new CommonOpenFileDialog {
                InitialDirectory = "",
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {

                var getFolderPath = dialog.FileName;
                var getFolderName = new DirectoryInfo(getFolderPath).Name;

                if (!lstFoldersPage.Items.Contains(getFolderName)) {

                    string[] folderFilesName = Directory.GetFiles(getFolderPath, "*")
                                                    .Select(Path.GetFileName).ToArray();

                    int numberOfFiles = Directory.GetFiles(getFolderPath, "*", SearchOption.AllDirectories).Length;

                    if (numberOfFiles <= Globals.uploadFileLimit[tempDataUser.AccountType]) {

                        flwLayoutHome.Controls.Clear();
                        lstFoldersPage.Items.Add(getFolderName);

                        CreateFilePanelFolder(getFolderPath, getFolderName, folderFilesName);
                        var folderListboxPosition = lstFoldersPage.Items.IndexOf(getFolderName);

                        lstFoldersPage.SelectedIndex = folderListboxPosition;

                    } else {
                        DisplayErrorFolder(tempDataUser.AccountType);
                        lstFoldersPage.SelectedItem = "Home";

                    }

                } else {
                    MessageBox.Show(
                        "Folder already exists", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }

        private void OpenFolderDownloadDialog(string folderTitle, List<(string fileName, byte[] fileBytes)> files) {

            ClosePopupForm.CloseRetrievalPopup();

            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), EncryptionModel.Decrypt(folderTitle));
            Directory.CreateDirectory(folderPath);

            foreach (var (fileName, fileBytes) in files) {
                var filePath = Path.Combine(folderPath, $"{fileName}");
                File.WriteAllBytes(filePath, fileBytes);
            }

            Process.Start(folderPath);

        }

        private async Task DownloadUserFolder(string folderName) {

            var filesData = await folderDataCaller.GetDownloadFolderData(folderName);
            OpenFolderDownloadDialog(folderName, filesData);

        }

        private async Task RefreshFolder() {

            GlobalsData.base64EncodedImageFolder.Clear();

            string selectedFolderName = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            await BuildFilePanelFolder(selectedFolderName);

        }

        private async Task BuildFilePanelFolder(string folderName) {

            ClearRedundane();

            StartPopupForm.StartRetrievalPopup();

            flwLayoutHome.Controls.Clear();

            try {

                var imageValues = new List<Image>();
                var onPressedEvent = new List<EventHandler>();
                var onMoreOptionButtonPressed = new List<EventHandler>();

                var filesInfo = await folderDataCaller.GetFileMetadata(folderName);

                var typeValues = filesInfo.Select(metadata => metadata.Item1.Split('.').Last()).ToList();

                int length = typeValues.Count;

                if (typeValues.Any(tv => Globals.imageTypes.Contains(tv))) {

                    if (lstFoldersPage.SelectedItems.Count > 0) {

                        string selectedItem = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                        if (!(selectedItem == previousSelectedItem)) {

                            previousSelectedItem = selectedItem;

                            GlobalsData.base64EncodedImageFolder.Clear();
                        }
                    }

                    if (GlobalsData.base64EncodedImageFolder.Count == 0) {
                        await folderDataCaller.AddImageCaching(folderName);
                        
                    }
                }

                for (int i = 0; i < length; i++) {

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

                            new PicForm(
                                defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, string.Empty, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.TextImage);

                        void textOnPressed(object sender, EventArgs e) {
                            new TextForm(
                                GlobalsTable.folderUploadTable, fileName, string.Empty, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (typeValues[i] == "exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            new ExeForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, string.Empty).ShowDialog();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.VideoImage);

                        void videoOnPressed(object sender, EventArgs e) {

                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            var defaultImage = new Bitmap(getImgName.Image);

                            new VideoForm(
                                defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            new ExcelForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }

                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            new AudioForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == "apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            new ApkForm(
                                fileName, tempDataUser.Username, GlobalsTable.folderUploadTable, folderName).ShowDialog();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == "pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            new PdfForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            new PtxForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == "msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            new MsiForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            new WordsForm(
                                fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).ShowDialog();
                        }

                        onPressedEvent.Add(wordOnPressed);
                    }
                }

                PanelGenerator panelGenerator = new PanelGenerator();
                panelGenerator.GeneratePanel("folderParameter", length, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues);

                UpdateProgressBarValue();
                BuildRedundaneVisibility();

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

                ClosePopupForm.CloseRetrievalPopup();

            } catch (Exception) {
                ClosePopupForm.CloseRetrievalPopup();
                BuildShowAlert(
                    title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");

            }
        }

        private async void CreateFilePanelFolder(string folderPath, string folderName, string[] filesName) {

            string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            int curr = 0;

            StartPopupForm.StartUploadingFolderPopup(folderName);

            GlobalsData.base64EncodedImageFolder.Clear();

            foreach (var filesFullPath in Directory.EnumerateFiles(folderPath, "*")) {

                curr++;

                var panel = new Guna2Panel() {
                    Name = $"PanExlFold{curr}",
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 12,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

                Label titleLabel = new Label();
                panel.Controls.Add(titleLabel);
                titleLabel.Name = $"titleLbl{curr}";
                titleLabel.Font = GlobalStyle.TitleLabelFont;
                titleLabel.ForeColor = GlobalStyle.GainsboroColor;
                titleLabel.Location = GlobalStyle.TitleLabelLoc;
                titleLabel.AutoEllipsis = true;
                titleLabel.Width = 160;
                titleLabel.Height = 20;
                titleLabel.Text = filesName[curr - 1];

                var panelImage = new Guna2PictureBox();
                panel.Controls.Add(panelImage);
                panelImage.Name = $"imagePnl{curr}";
                panelImage.Width = 190;
                panelImage.Height = 145;
                panelImage.SizeMode = PictureBoxSizeMode.CenterImage;
                panelImage.BorderRadius = 12;

                panelImage.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panel.Width - panelImage.Width) / 2;

                panelImage.Location = new Point(picMain_Q_x, 10);

                panelImage.MouseHover += (_senderM, _ev) => {
                    panel.ShadowDecoration.Enabled = true;
                    panel.ShadowDecoration.BorderRadius = 12;
                };

                panelImage.MouseLeave += (_senderQ, _evQ) => {
                    panel.ShadowDecoration.Enabled = false;
                };

                Guna2Button moreOptionsButton = new Guna2Button();
                panel.Controls.Add(moreOptionsButton);
                moreOptionsButton.Name = $"RemExlButFold{curr}";
                moreOptionsButton.Width = 29;
                moreOptionsButton.Height = 26;
                moreOptionsButton.ImageOffset = GlobalStyle.GarbageOffset;
                moreOptionsButton.BorderColor = GlobalStyle.TransparentColor;
                moreOptionsButton.FillColor = GlobalStyle.TransparentColor;
                moreOptionsButton.BorderRadius = 6;
                moreOptionsButton.BorderThickness = 1;
                moreOptionsButton.BorderColor = GlobalStyle.BorderColor2;
                moreOptionsButton.Image = GlobalStyle.GarbageImage;
                moreOptionsButton.Location = GlobalStyle.GarbageButtonLoc;

                moreOptionsButton.Click += (sender_vid, e_vid) => {
                    lblFileNameOnPanel.Text = titleLabel.Text;
                    lblFileTableName.Text = GlobalsTable.folderUploadTable;
                    lblFilePanelName.Text = panel.Name;
                    lblSelectedDirName.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                    pnlFileOptions.Visible = true;
                };

                Label dateLabExl = new Label();
                panel.Controls.Add(dateLabExl);
                dateLabExl.Name = $"dateLbl{curr}";
                dateLabExl.Font = GlobalStyle.DateLabelFont;
                dateLabExl.ForeColor = GlobalStyle.DarkGrayColor;
                dateLabExl.Location = GlobalStyle.DateLabelLoc;
                dateLabExl.Text = _todayDate;

                lblEmptyHere.Visible = false;
                btnGarbageImage.Visible = false;

                try {

                    string fileType = filesFullPath.Split('.').Last();

                    byte[] retrieveBytes = File.ReadAllBytes(filesFullPath);
                    byte[] compressedBytes = new GeneralCompressor().compressFileData(retrieveBytes);

                    string toBase64String = Convert.ToBase64String(compressedBytes);
                    string encryptValues = UniqueFile.IgnoreEncryption(fileType) 
                        ? toBase64String : EncryptionModel.Encrypt(toBase64String);

                    if (Globals.imageTypes.Contains(fileType)) {

                        var image = new Bitmap(filesFullPath);

                        string compressImage = compressor.compressImageToBase64(filesFullPath);
                        string compressedImageToBase64 = EncryptionModel.Encrypt(compressImage);

                        await insertFileData.InsertFileDataFolder(
                            filesFullPath, folderName, compressedImageToBase64);

                        panelImage.Image = image;
                        panelImage.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            var defaultImage = new Bitmap(getImgName.Image);
                            
                            new PicForm(
                                defaultImage, getWidth, getHeight, titleLabel.Text, GlobalsTable.folderUploadTable, string.Empty, tempDataUser.Username).ShowDialog();

                        };
                    }

                    if (Globals.textTypes.Contains(fileType)) {

                        string nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(filesFullPath)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptEncoded = EncryptionModel.Encrypt(getEncoded);

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptEncoded);

                        panelImage.Image = Globals.TextImage;
                        panelImage.Click += (sender_t, e_t) => {
                            new TextForm(
                                GlobalsTable.folderUploadTable, titleLabel.Text, string.Empty, tempDataUser.Username).ShowDialog();
                        };

                    }

                    if (fileType == "apk") {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.APKImage;
                        panelImage.Click += (sender_ap, e_ap) => {
                            new ApkForm(
                                titleLabel.Text, tempDataUser.Username, GlobalsTable.folderUploadTable, string.Empty).ShowDialog();
                        };
                    }

                    if (Globals.videoTypes.Contains(fileType)) {

                        ShellFile shellFile = ShellFile.FromFilePath(filesFullPath);

                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        string compressedThumbnail;

                        using (var stream = new MemoryStream()) {
                            shellFile.Thumbnail.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            string toBase64BitmapThumbnail = Convert.ToBase64String(stream.ToArray());
                            compressedThumbnail = compressor.compressBase64Image(toBase64BitmapThumbnail);
                        }

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues, compressedThumbnail);

                        panelImage.Image = toBitMap;
                        panelImage.Click += (sender_vid, e_vid) => {

                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            var defaultImage = new Bitmap(getImgName.Image);

                            new VideoForm(
                                defaultImage, getWidth, getHeight, titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }

                    if (fileType == "pdf") {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.PDFImage;
                        panelImage.Click += (sender_pdf, e_pdf) => {
                            new PdfForm(
                                titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }

                    if (Globals.wordTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.DOCImage;
                        panelImage.Click += (sender_pdf, e_pdf) => {
                            new WordsForm(
                                titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }

                    if (Globals.excelTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.DOCImage;
                        panelImage.Click += (sender_pdf, e_pdf) => {
                            new ExcelForm(
                                titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }


                    if (Globals.ptxTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.PTXImage;
                        panelImage.Click += (sender_pdf, e_pdf) => {
                            new PtxForm(
                                titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }

                    if (Globals.audioTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        panelImage.Image = Globals.AudioImage;
                        panelImage.Click += (sender_pdf, e_pdf) => {
                            new AudioForm(
                                titleLabel.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username).ShowDialog();
                        };
                    }

                    flwLayoutHome.Controls.Add(panel);

                } catch (Exception) { }

            }

            ClosePopupForm.CloseUploadingPopup();

            UpdateProgressBarValue();

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
        private async void RemoveAndDeleteFolder(string folderName) {

            DialogResult verifyDeletion = MessageBox.Show(
                $"Delete {folderName} folder?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (verifyDeletion == DialogResult.Yes) {

                await folderDataCaller.DeleteFolder(folderName);

                lstFoldersPage.Items.Remove(folderName);

                BuildButtonsOnHomePageSelected();

                await BuildHomeFiles();

                lblCurrentPageText.Text = "Home";
                lstFoldersPage.SelectedIndex = -1;

                ClosePopupForm.CloseRetrievalPopup();

            }
        }

        /// <summary>
        /// Function to display alert form if the 
        /// number of user folder files exceeding the amount of file 
        /// they can upload
        /// </summary>
        /// <param name="CurAcc"></param>
        public void DisplayErrorFolder(string CurAcc) {
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

                string origin = lblCurrentPageText.Text;

                if (origin == "Home") {

                    int currentUploadCount = Convert.ToInt32(lblItemCountText.Text);

                    if (currentUploadCount != Globals.uploadFileLimit[tempDataUser.AccountType]) {
                        OpenDialogHomeFile();

                    } else {
                        DisplayUpgradeAccountDialog();

                    }

                } else {
                    MessageBox.Show(
                        "You can only upload files on Home.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            } catch (Exception) {
                BuildShowAlert(
                    title: "Something went wrong", subheader: "Something went wrong while trying to upload files.");

            }

        }

        private async void btnUploadPs_Click_1(object sender, EventArgs e) {

            try {

                var returnedCountValue = new List<int>();

                foreach (string tableName in GlobalsTable.publicTablesPs) {
                    int count = await crud.CountUserTableRow(tableName);
                    returnedCountValue.Add(count);
                }

                int currentUploadCount = returnedCountValue.Sum();

                if (currentUploadCount != Globals.uploadFileLimit[tempDataUser.AccountType]) {
                    OpenDialogPublicStorage();

                } else {
                    DisplayUpgradeAccountDialog();

                }

                returnedCountValue.Clear();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Something went wrong", subheader: "Something went wrong while trying to upload files.");

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

                var folderUploadFailed = new LimitedFolderAlert(
                    tempDataUser.AccountType, "it looks like you've reached the max \r\namount of folder you can upload", true);

                var countTotalFolders = lstFoldersPage.Items.Cast<string>().ToList().Count();

                if (Globals.uploadFolderLimit[tempDataUser.AccountType] == countTotalFolders) {
                    folderUploadFailed.Show();
                    return;
                }

                OpenFolderDialog();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Something went wrong", subheader: "Something went wrong while trying to upload this folder.");

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

        private async Task FolderOnSelected(string folderName) {

            BuildButtonsOnFolderNameSelected();

            await BuildFilePanelFolder(folderName);
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

                await FolderOnSelected(selectedFolderTab);

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            } catch (Exception) {
                flwLayoutHome.Controls.Clear();

                BuildRedundaneVisibility();
                BuildShowAlert(
                    title: "Something went wrong", subheader: "Try to restart Flowstorage.");

            }
        }

        private void btnDeleteFolder_Click(object sender, EventArgs e) {

            ClosePopupForm.CloseRetrievalPopup();

            string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            RemoveAndDeleteFolder(selectedFolder);

        }


        /// <summary>
        /// Generate user directory from Home folder
        /// </summary>
        /// <param name="userName">Username of user</param>
        /// <param name="customParameter">Custom parameter for panel</param>
        /// <param name="rowLength"></param>

        #region Build directory panel section

        private void BuildDirectoryPanel(List<string> directoriesName, int directoryCount) {

            for (int i = 0; i < directoryCount; i++) {

                var panel = new Guna2Panel() {
                    Name = "directoryPar" + i,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 12,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

                flwLayoutHome.Controls.Add(panel);

                Label directoryLab = new Label();
                panel.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + i;
                directoryLab.Font = GlobalStyle.DateLabelFont;
                directoryLab.ForeColor = GlobalStyle.DarkGrayColor;
                directoryLab.Location = GlobalStyle.DateLabelLoc;
                directoryLab.Text = "Directory";

                Label titleLabel = new Label();
                panel.Controls.Add(titleLabel);
                titleLabel.Name = "titleLab" + i;
                titleLabel.Font = GlobalStyle.TitleLabelFont;
                titleLabel.ForeColor = GlobalStyle.GainsboroColor;
                titleLabel.Location = GlobalStyle.TitleLabelLoc;
                titleLabel.Width = 160;
                titleLabel.Height = 20;
                titleLabel.AutoEllipsis = true;
                titleLabel.Text = directoriesName[i];

                Guna2PictureBox panelImage = new Guna2PictureBox();
                panel.Controls.Add(panelImage);
                panelImage.Name = "imagePnl" + i;
                panelImage.SizeMode = PictureBoxSizeMode.CenterImage;
                panelImage.BorderRadius = 12;
                panelImage.Width = 190;
                panelImage.Height = 145;

                panelImage.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panel.Width - panelImage.Width) / 2;
                panelImage.Location = new Point(picMain_Q_x, 10);

                panelImage.MouseHover += (_senderM, _ev) => {
                    panel.ShadowDecoration.Enabled = true;
                    panel.ShadowDecoration.BorderRadius = 12;
                };

                panelImage.MouseLeave += (_senderQ, _evQ) => {
                    panel.ShadowDecoration.Enabled = false;
                };

                Guna2Button deleteDirectoryButton = new Guna2Button();
                panel.Controls.Add(deleteDirectoryButton);
                deleteDirectoryButton.Name = "Rem" + i;
                deleteDirectoryButton.Width = 29;
                deleteDirectoryButton.Height = 26;
                deleteDirectoryButton.ImageOffset = GlobalStyle.GarbageOffset;
                deleteDirectoryButton.FillColor = GlobalStyle.TransparentColor;
                deleteDirectoryButton.BorderRadius = 6;
                deleteDirectoryButton.BorderThickness = 1;
                deleteDirectoryButton.BorderColor = GlobalStyle.TransparentColor;
                deleteDirectoryButton.Image = Globals.DirectoryGarbageImage;
                deleteDirectoryButton.Location = GlobalStyle.GarbageButtonLoc;

                deleteDirectoryButton.Click += async (sender_im, e_im) => {

                    string directoryName = titleLabel.Text; 

                    DialogResult verifyDialog = MessageBox.Show(
                        $"Delete {directoryName} directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (verifyDialog == DialogResult.Yes) {
                        
                        await directoryDataCaller.DeleteDirectory(directoryName);

                        panel.Dispose();

                        BuildRedundaneVisibility();
                        UpdateProgressBarValue();

                    }
                };

                panelImage.Image = Globals.DIRIcon;
                panelImage.Click += (sender_dir, ev_dir) => {

                    StartPopupForm.StartRetrievalPopup();

                    new DirectoryForm(titleLabel.Text).Show();

                    ClosePopupForm.CloseRetrievalPopup();

                };
            }

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

        }

        #endregion END - Build directory panel section

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

        private async void btnRefreshFiles_Click(object sender, EventArgs e) {

            flwLayoutHome.Controls.Clear();

            if (lblCurrentPageText.Text == "Shared To Me") {
                await RefreshGenerateUserSharedToMe("DirParMe");

            } else if (lblCurrentPageText.Text == "Shared Files") {
                await RefreshGenerateUserSharedOthers("DirParOther");

            } else if (lblCurrentPageText.Text == "Home") {
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

            var disposedPanels = new List<Guna2Panel>();

            for (int i = flwLayoutHome.Controls.Count - 1; i >= 0; i--) {

                var control = flwLayoutHome.Controls[i];

                if (control is Guna2Panel panel) {
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

                flwLayoutHome.Controls.Clear();

                switch(origin) {
                    case "Home":
                        await BuildHomeFiles();
                        break;

                    case "Shared To Me":
                        await RefreshGenerateUserSharedToMe("DirParMe");
                        break;

                    case "Shared Files":
                        await RefreshGenerateUserSharedOthers("DirParOther");
                        break;

                    case "Public Storage":
                        GlobalsData.base64EncodedImagePs.Clear();
                        GlobalsData.base64EncodedThumbnailPs.Clear();
                        await BuildPublicStorageFiles();
                        break;

                    default:
                        await RefreshFolder();
                        break;

                }

            }

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();
            BuildRedundaneVisibility();

        }

        /// <summary>
        /// Go to Home button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnGoHomePage_Click(object sender, EventArgs e) {

            BuildButtonsOnHomePageSelected();

            btnRefreshFiles.Visible = true;
            pnlSubPanelDetails.Visible = true;
            btnLogout.Visible = true;
            pnlMain.Visible = true;
            pnlPublicStorage.Visible = false;

            pnlShared.Visible = false;
            pnlFilterType.Visible = false;

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

            if (lblCurrentPageText.Text == "Home") {
                return;
            }

            lblCurrentPageText.Text = "Home";

            flwLayoutHome.Controls.Clear();

            await BuildHomeFiles();

        }

        /// <summary>
        /// Go to Folders button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoFolderPage_Click(object sender, EventArgs e) {

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

        private async void btnOpenUpgradePage_Click(object sender, EventArgs e) {

            new SettingsForm().Show();

            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];

            await currencyConverter.ConvertToLocalCurrency();

        }

        private void RefreshAllOnLogut() {

            pnlMain.SendToBack();

            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";

            if(Directory.Exists(directoryPath)) {
                Directory.Delete(directoryPath, true);
            }

            GlobalsData.base64EncodedImageHome.Clear();
            GlobalsData.base64EncodedThumbnailHome.Clear();

            GlobalsData.base64EncodedImageSharedOthers.Clear();

            GlobalsData.base64EncodedImageSharedToMe.Clear();

            GlobalsData.base64EncodedImagePs.Clear();
            GlobalsData.base64EncodedThumbnailPs.Clear();

            instance.lstFoldersPage.Items.Clear();

            Hide();

            new SignUpForm().ShowDialog();

        }

        private void btnLogout_Click(object sender, EventArgs e) {

            try {

                DialogResult confirmation = MessageBox.Show(
                    "Logout your account?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmation== DialogResult.Yes) {
                    RefreshAllOnLogut();

                }

            } catch (Exception) {
                MessageBox.Show(
                    "There's a problem while attempting to logout your account. Please try again.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        #region Drag and drop upload section

        private void HomePage_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void HomePage_DragOver(object sender, DragEventArgs e) {
            pnlDragAndDropUpload.Visible = true;
            e.Effect = DragDropEffects.Copy;
        }

        private void HomePage_DragLeave(object sender, EventArgs e) => pnlDragAndDropUpload.Visible = false;


        /// <summary>
        /// 
        /// Drag-drop upload feature is implemented here
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void HomePage_DragDrop(object sender, DragEventArgs e) {

            pnlDragAndDropUpload.Visible = false;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length + flwLayoutHome.Controls.Count > Globals.uploadFileLimit[tempDataUser.AccountType]) {
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

            var filePathList = new List<string>(files);

            foreach (var selectedItems in filePathList) {

                string fileName = Path.GetFileName(selectedItems);
                string fileType = fileName.Split('.').Last();

                try {

                    byte[] readFileBytes = File.ReadAllBytes(selectedItems);
                    string toBase64String = Convert.ToBase64String(readFileBytes);
                    string encryptText = EncryptionModel.Encrypt(toBase64String);

                    paramCurr++;

                    if (Globals.imageTypes.Contains(fileType)) {
                        var getImg = new Bitmap(selectedItems);
                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;

                        string tempToBase64 = Convert.ToBase64String(readFileBytes);
                        string encryptedValue = EncryptionModel.Encrypt(tempToBase64);
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeImageTable, encryptedValue);
                        
                    } else if (Globals.textTypes.Contains(fileType)) {
                        string nonLine = "";

                        using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptTextValue = EncryptionModel.Encrypt(getEncoded);
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeTextTable, encryptTextValue);

                    } else if (fileType == "exe") {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExeTable, encryptText);

                    } else if (Globals.videoTypes.Contains(fileType)) {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeVideoTable, encryptText);

                    } else if (Globals.excelTypes.Contains(fileType)) {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExcelTable, encryptText);

                    } else if (Globals.audioTypes.Contains(fileType)) {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeAudioTable, encryptText);

                    } else if (fileType == "apk") {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeApkTable, encryptText);

                    } else if (fileType == "pdf") {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePdfTable, encryptText);

                    } else if (Globals.ptxTypes.Contains(fileType)) {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePtxTable, encryptText);

                    } else if (fileType == "msi") {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeMsiTable, encryptText);

                    } else if (Globals.wordTypes.Contains(fileType)) {
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeWordTable, encryptText);

                    }

                    ClosePopupForm.CloseUploadingPopup();

                } catch (Exception) {
                    ClosePopupForm.CloseUploadingPopup();

                }

                lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            }
        }

        #endregion END - Drag and upload section

        #region Filter type section

        private void btnFilterImages_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".png,.jpeg,.jpg";
        }

        private void btnFilterText_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".txt,.html,.md,.sql,.css,.js,.csv";
        }

        private void btnFilterDocuments_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".doc,.docx";
        }

        private void btnFilterAudio_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".mp3,.wav";
        }

        private void btnFilterExcel_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".xlsx,xls";
        }

        private void btnFilterVideos_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".mp4,.avi,.mov,wmv";
        }

        private void btnFilterPdf_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".pdf";
        }

        private void btnClearFilter_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
        }

        #endregion END - Filter type section

        /// <summary>
        /// 
        /// Download folder button.
        /// Only user with paid plan has this feature enabled, else
        /// alert the user to upgrade their plan if their account is Basic plan
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadFolder_Click(object sender, EventArgs e) {

            if (tempDataUser.AccountType == "Max" || tempDataUser.AccountType == "Express" || tempDataUser.AccountType == "Supreme") {
                string folderTitleGet = EncryptionModel.Encrypt(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem));
                await DownloadUserFolder(folderTitleGet);

            } else {
                new LimitedFolderAlert(
                    tempDataUser.AccountType, 
                    "Please upgrade your account \r\nplan to download folder.", false).Show();
            }
        }

        private void btnClosePnlFileOptions_Click(object sender, EventArgs e) => pnlFileOptions.Visible = false;

        /// <summary>
        /// 
        /// Delete file, including folder, home, sharing, directory
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDeleteFile_Click(object sender, EventArgs e) {

            string fileName = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string panelName = lblFilePanelName.Text;
            string sharedToName = lblSharedToName.Text;
            string dirName = lblSelectedDirName.Text;

            var verifyDialog = MessageBox.Show(
                $"Delete '{fileName}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (verifyDialog == DialogResult.Yes) {

                var deleteFileQuery = new DeleteFileDataQuery();
                await deleteFileQuery.DeleteFileData(tableName, fileName, dirName, sharedToName);

                var matches = this.Controls.Find(panelName, true);

                if (matches.Length > 0 && matches[0] is Guna2Panel myPanel) {
                    flwLayoutHome.Controls.Remove(myPanel);
                    myPanel.Dispose();
                }

                UpdateProgressBarValue();
                BuildRedundaneVisibility();

                pnlFileOptions.Visible = false;

            }
        }

        private void btnRenameFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string panelName = lblFilePanelName.Text;
            string sharedToName = lblSharedToName.Text;
            string dirName = lblSelectedDirName.Text;

            pnlFileOptions.Visible = false;

            new RenameFileForm(titleFile, tableName, panelName, dirName, sharedToName).Show();

        }

        private void btnDownloadFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string dirName = lblSelectedDirName.Text;

            pnlFileOptions.Visible = false;

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

        private void btnOpenShareFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblSelectedDirName.Text;

            string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            bool fromSharedFiles = selectedFolder == "Shared Files";

            pnlFileOptions.Visible = false;

            new ShareSelectedFileForm(titleFile, fromSharedFiles, tempDataUser.Username, dirName).Show();

        }

        /// <summary>
        /// 
        /// Public storage upload file
        /// 
        /// </summary>g
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void btnGoPsPage_Click(object sender, EventArgs e) {

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

        private void btnCloseHomePage_Click(object sender, EventArgs e) => this.Close();

        private async void btnOpenUpgradePage2_Click(object sender, EventArgs e) {

            new SettingsForm().Show();

            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];

            await currencyConverter.ConvertToLocalCurrency();

        }

        private void btnShowFilterTypePnl_Click(object sender, EventArgs e) {
            pnlShared.Visible = false;
            pnlFilterType.Visible = !pnlFilterType.Visible;
        }

        private void btnShowSharedPnl_Click(object sender, EventArgs e) {
            pnlFilterType.Visible = false;
            pnlShared.Visible = !pnlShared.Visible;
        }

        private async void btnGoSharedFilesPage_Click(object sender, EventArgs e) {

            flwLayoutHome.Controls.Clear();

            BuildButtonsOnSharedToMeSelected();
            ClearRedundane();

            await CallFilesInformationOthers();
            await BuildSharedToOthers();

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            lblCurrentPageText.Text = "Shared Files";
            lblCurrentPageText.Visible = true;

            pnlSubPanelDetails.Visible = true;
            btnDeleteFolder.Visible = false;

            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;

            pnlShared.Visible = false;

        }

        private async void btnGoSharedToMePage_Click(object sender, EventArgs e) {

            flwLayoutHome.Controls.Clear();

            BuildButtonOnSharedFilesSelected();
            ClearRedundane();

            await CallFilesInformationSharedToMe();
            await BuildSharedToMe();

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            lblCurrentPageText.Text = "Shared To Me";
            lblCurrentPageText.Visible = true;

            pnlSubPanelDetails.Visible = false;
            btnDeleteFolder.Visible = false;

            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;

            pnlShared.Visible = false;

        }

    }
}