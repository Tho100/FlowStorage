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
using MySql.Data.MySqlClient;
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

        private void guna2Button1_Click(object sender, EventArgs e) => new CreateDirectoryForm().Show();
        private void guna2Button7_Click(object sender, EventArgs e) => new MainShareFileForm().Show();
        private void guna2Button3_Click_1(object sender, EventArgs e)
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

            //try {

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

                        var getExtension = filesInfo[i].Item1.Split('.').Last();
                        var textTypeToImage = Globals.textTypeToImage[getExtension];
                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.homeTextTable, filesInfo[accessIndex].Item1, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.homeExeTable) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfo[accessIndex].Item1, GlobalsTable.homeExeTable, string.Empty, tempDataUser.Username);
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
                            WordDocForm displayMsi = new WordDocForm(filesInfo[accessIndex].Item1, GlobalsTable.homeWordTable, string.Empty, tempDataUser.Username);
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

            /*} catch (Exception) {
                BuildShowAlert(title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");

            */

        }

        private async Task CreateFilePanelHome(string fileFullPath, string tableName, string parameterName, int itemCurr, string keyVal) {

            string fileName = Path.GetFileName(fileFullPath);

            var filePanel = new Guna2Panel() {
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

            var mainPanelTxt = filePanel;

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
            titleLab.Text = fileName;

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
                filePanel.ShadowDecoration.Enabled = true;
                filePanel.ShadowDecoration.BorderRadius = 8;
            };

            textboxPic.MouseLeave += (_senderQ, _evQ) => {
                filePanel.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.homeImageTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                GlobalsData.base64EncodedImageHome.Add(EncryptionModel.Decrypt(keyVal));

                textboxPic.Image = new Bitmap(fileFullPath);
                textboxPic.Click += (sender_f, e_f) => {

                    var imageName = (Guna2PictureBox)sender_f;
                    var imageWidth = imageName.Image.Width;
                    var imageHeight = imageName.Image.Height;

                    Bitmap defaultImage = new Bitmap(imageName.Image);

                    PicForm displayPic = new PicForm(defaultImage, imageWidth, imageHeight, fileName, GlobalsTable.homeImageTable, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };

            }

            if (tableName == GlobalsTable.homeTextTable) {

                string textType = titleLab.Text.Split('.').Last();
                textboxPic.Image = Globals.textTypeToImage[textType];

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Click += (sender_t, e_t) => {

                    TextForm txtFormShow = new TextForm(GlobalsTable.homeTextTable, fileName, string.Empty, tempDataUser.Username);
                    txtFormShow.Show();
                };
            }

            if (tableName == GlobalsTable.homeExeTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.EXEImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    exeFORM displayExe = new exeFORM(titleLab.Text, GlobalsTable.homeExeTable, string.Empty, tempDataUser.Username);
                    displayExe.Show();
                };
            }

            if (tableName == GlobalsTable.homeVideoTable) {

                await insertFileData.InsertFileDataVideo(fileFullPath, tableName, fileName, keyVal);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                textboxPic.Image = toBitMap;

                textboxPic.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImg = new Bitmap(getImgName.Image);

                    VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, titleLab.Text, GlobalsTable.homeVideoTable, string.Empty, tempDataUser.Username);
                    vidShow.Show();
                };
            }
            if (tableName == GlobalsTable.homeAudioTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.AudioImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.homeAudioTable, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };

            }

            if (tableName == GlobalsTable.homeExcelTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.EXCELImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.homeExcelTable, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.homeApkTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.APKImage;
                textboxPic.Click += (sender_gi, e_gi) => {
                    ApkForm displayPic = new ApkForm(titleLab.Text, tempDataUser.Username, GlobalsTable.homeApkTable, string.Empty);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.homePdfTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.PDFImage;
                textboxPic.Click += (sender_pd, e_pd) => {
                    PdfForm displayPdf = new PdfForm(titleLab.Text, GlobalsTable.homePdfTable, string.Empty, tempDataUser.Username);
                    displayPdf.ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homePtxTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.PTXImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    PtxForm displayPtx = new PtxForm(titleLab.Text, GlobalsTable.homePtxTable, string.Empty, tempDataUser.Username);
                    displayPtx.ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeMsiTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.MSIImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    MsiForm displayMsi = new MsiForm(titleLab.Text, GlobalsTable.homeMsiTable, string.Empty, tempDataUser.Username);
                    displayMsi.Show();
                };
            }

            if (tableName == GlobalsTable.homeWordTable) {

                await insertFileData.InsertFileData(fileName, keyVal, tableName);

                textboxPic.Image = Globals.DOCImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    WordDocForm displayWord = new WordDocForm(titleLab.Text, GlobalsTable.homeWordTable, string.Empty, tempDataUser.Username);
                    displayWord.ShowDialog();
                };
            }

            flwLayoutHome.Controls.Add(filePanel);

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
                    string fileType = Path.GetExtension(selectedItems);

                    if (fileNameLabels.Contains(selectedFileName.ToLower().Trim())) {
                        continue;
                    }

                    try {

                        byte[] originalRetrieveBytes = File.ReadAllBytes(selectedItems);
                        byte[] compressedBytes = new GeneralCompressor().compressFileData(originalRetrieveBytes);

                        string convertToBase64 = Convert.ToBase64String(compressedBytes);
                        string encryptText = UniqueFile.IgnoreEncryption(fileType) 
                            ? convertToBase64 : EncryptionModel.Encrypt(convertToBase64);

                        if (Globals.imageTypes.Contains(fileType)) {
                            curr++;
                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImage = compressor.compressImageToBase64(selectedItems);
                            string encryptedValue = EncryptionModel.Encrypt(compressedImage);

                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, encryptedValue);
                                
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

                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeTextTable, "PanTxt", txtCurr, encryptEncodedText);

                        } else if (fileType == ".exe") {
                            exeCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeExeTable, "PanExe", exeCurr, encryptText);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            vidCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptText);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            exlCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptText);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            audCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptText);

                        } else if (fileType == ".apk") {
                            apkCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptText);

                        } else if (fileType == ".pdf") {
                            pdfCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptText);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            ptxCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptText);

                        } else if (fileType == ".msi") {
                            msiCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptText);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            docxCurr++;
                            await CreateFilePanelHome(
                                selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptText);

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

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psImage, string.Empty, uploaderName);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }


                    if (tableName == GlobalsTable.psText) {

                        var getExtension = filesInfo[i].Item1.Split('.').Last();
                        var textTypeToImage = Globals.textTypeToImage[getExtension];

                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.psText, filesInfo[accessIndex].Item1, string.Empty, uploaderName);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (tableName == GlobalsTable.psExe) {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfo[accessIndex].Item1, GlobalsTable.psExe, string.Empty, uploaderName);
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
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfo[accessIndex].Item1, GlobalsTable.psVideo, string.Empty, uploaderName);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (tableName == GlobalsTable.psExcel) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfo[accessIndex].Item1, GlobalsTable.psExcel, string.Empty, uploaderName);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (tableName == GlobalsTable.psAudio) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfo[accessIndex].Item1, GlobalsTable.psAudio, string.Empty, uploaderName);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (tableName == GlobalsTable.psApk) {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfo[accessIndex].Item1, uploaderName, GlobalsTable.psApk, string.Empty);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (tableName == GlobalsTable.psPdf) {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfo[accessIndex].Item1, GlobalsTable.psPdf, string.Empty, uploaderName);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (tableName == GlobalsTable.psPtx) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfo[accessIndex].Item1, GlobalsTable.psPtx, string.Empty, uploaderName);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (tableName == GlobalsTable.psMsi) {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfo[accessIndex].Item1, GlobalsTable.psMsi, string.Empty, uploaderName);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (tableName == GlobalsTable.psWord) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfo[accessIndex].Item1, GlobalsTable.psWord, string.Empty, uploaderName);
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
                BuildShowAlert(
                    title: "Something went wrong", "Failed to load your files. Try to hit the refresh button.");

            }            

        }

        private async Task CreateFilePanelPublicStorage(string fileFullPath, string tableName, string parameterName, int itemCurr, string keyVal) {

            string fileName = Path.GetFileName(fileFullPath);

            var filePanel = new Guna2Panel() {
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

            var mainPanelTxt = filePanel;

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
            titleLab.Text = fileName;

            textboxPic.MouseHover += (_senderM, _ev) => {
                filePanel.ShadowDecoration.Enabled = true;
                filePanel.ShadowDecoration.BorderRadius = 8;
            };

            textboxPic.MouseLeave += (_senderQ, _evQ) => {
                filePanel.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.psImage) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                GlobalsData.base64EncodedImagePs.Add(EncryptionModel.Decrypt(keyVal));

                textboxPic.Image = new Bitmap(fileFullPath);
                textboxPic.Click += (sender_f, e_f) => {

                    var getImgName = (Guna2PictureBox)sender_f;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;

                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                    PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.psImage, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };

            }

            if (tableName == GlobalsTable.psText) {

                string textType = titleLab.Text.Split('.').Last();
                textboxPic.Image = Globals.textTypeToImage[textType];

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Click += (sender_t, e_t) => {
                    TextForm txtFormShow = new TextForm(GlobalsTable.psText, fileName, string.Empty, tempDataUser.Username);
                    txtFormShow.Show();
                };
            }

            if (tableName == GlobalsTable.psExe) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.EXEImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    exeFORM displayExe = new exeFORM(titleLab.Text, GlobalsTable.psExe, string.Empty, tempDataUser.Username);
                    displayExe.Show();
                };
            }

            if (tableName == GlobalsTable.psVideo) {

                await insertFileData.InsertFileVideoDataPublic(fileFullPath, fileName, keyVal);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                textboxPic.Image = toBitMap;

                textboxPic.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImg = new Bitmap(getImgName.Image);

                    VideoForm vidShow = new VideoForm(defaultImg, getWidth, getHeight, titleLab.Text, GlobalsTable.psVideo, string.Empty, tempDataUser.Username);
                    vidShow.Show();
                };
            }

            if (tableName == GlobalsTable.psAudio) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.AudioImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.psAudio, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.psExcel) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.EXCELImage;
                textboxPic.Click += (sender_ex, e_ex) => {
                    ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.psExcel, string.Empty, tempDataUser.Username);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.psApk) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.APKImage;
                textboxPic.Click += (sender_gi, e_gi) => {
                    ApkForm displayPic = new ApkForm(titleLab.Text, tempDataUser.Username, GlobalsTable.psApk, string.Empty);
                    displayPic.Show();
                };
            }

            if (tableName == GlobalsTable.psPdf) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.PDFImage;
                textboxPic.Click += (sender_pd, e_pd) => {
                    PdfForm displayPdf = new PdfForm(titleLab.Text, GlobalsTable.psPdf, string.Empty, tempDataUser.Username);
                    displayPdf.ShowDialog();
                };
            }

            if (tableName == GlobalsTable.psPtx) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.PTXImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    PtxForm displayPtx = new PtxForm(titleLab.Text, GlobalsTable.psPtx, string.Empty, tempDataUser.Username);
                    displayPtx.ShowDialog();
                };
            }
            if (tableName == GlobalsTable.psMsi) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.MSIImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    MsiForm displayMsi = new MsiForm(titleLab.Text, GlobalsTable.psMsi, string.Empty, tempDataUser.Username);
                    displayMsi.Show();
                };
            }

            if (tableName == GlobalsTable.psWord) {

                await insertFileData.InsertFileDataPublic(fileName, keyVal, tableName);

                textboxPic.Image = Globals.DOCImage;
                textboxPic.Click += (sender_ptx, e_ptx) => {
                    WordDocForm displayWord = new WordDocForm(titleLab.Text, GlobalsTable.psWord, string.Empty, tempDataUser.Username);
                    displayWord.ShowDialog();
                };
            }

            flwLayoutHome.Controls.Add(filePanel);

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

                        if (Globals.imageTypes.Contains(fileType)) {
                            curr++;
                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImage = compressor.compressImageToBase64(selectedItems);
                            string encryptedImage = EncryptionModel.Encrypt(compressedImage);
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psImage, "PanImg", curr, encryptedImage);
       
                        } else if (Globals.textTypes.Contains(fileType)) {
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

                        } else if (fileType == ".exe") {
                            exeCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psExe, "PanExe", exeCurr, encryptText);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            vidCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psVideo, "PanVid", vidCurr, encryptText);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            exlCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psExcel, "PanExl", exlCurr, encryptText);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            audCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psAudio, "PanAud", audCurr, encryptText);

                        } else if (fileType == ".apk") {
                            apkCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psApk, "PanApk", apkCurr, encryptText);

                        } else if (fileType == ".pdf") {
                            pdfCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psPdf, "PanPdf", pdfCurr, encryptText);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            ptxCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psPtx, "PanPtx", ptxCurr, encryptText);

                        } else if (fileType == ".msi") {
                            msiCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psMsi, "PanMsi", msiCurr, encryptText);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            docxCurr++;
                            await CreateFilePanelPublicStorage(
                                selectedItems, GlobalsTable.psWord, "PanDoc", docxCurr, encryptText);

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

                if (Globals.videoTypes.Contains(typeValues[i])) {

                    if (GlobalsData.base64EncodedThumbnailSharedOthers.Count == 0) {
                        await sharedFilesDataCaller.AddVideoThumbnailCaching(filesInfoSharedOthers[i].Item1);

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

                if (typeValues[i] == "exe") {

                    imageValues.Add(Globals.EXEImage);

                    void exeOnPressed(object sender, EventArgs e) {
                        exeFORM displayExe = new exeFORM(filesInfoSharedOthers[accessIndex].Item1, GlobalsTable.homeExeTable, lblGreetingText.Text, uploadToName, true);
                        displayExe.Show();
                    }

                    onPressedEvent.Add(exeOnPressed);
                }

                if (typeValues[i] == "apk") {

                    imageValues.Add(Globals.APKImage);

                    void apkOnPressed(object sender, EventArgs e) {
                        ApkForm displayPic = new ApkForm(filesInfoSharedOthers[accessIndex].Item1, uploadToName, GlobalsTable.sharingTable, lblGreetingText.Text, true);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(apkOnPressed);
                }

                if (typeValues[i] == "pdf") {

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

                if (typeValues[i] == "msi") {

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
            GlobalsData.base64EncodedThumbnailSharedOthers.Clear();

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

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.textTypeToImage[typeValues[i]]);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.sharingTable, filesInfoSharedToMe[accessIndex].Item1, lblGreetingText.Text, uploaderUsername, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedThumbnailSharedToMe.Count == 0) {
                            MessageBox.Show(filesInfoSharedToMe[i].Item1);
                            await sharedToMeDataCaller.AddVideoThumbnailCaching(filesInfoSharedToMe[i].Item1);

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
                            VideoForm vidFormShow = new VideoForm(defaultImage, getWidth, getHeight, filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            vidFormShow.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            ExcelForm exlForm = new ExcelForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            exlForm.Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }

                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            AudioForm displayPic = new AudioForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == "exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (typeValues[i] == "apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            ApkForm displayPic = new ApkForm(filesInfoSharedToMe[accessIndex].Item1, uploaderUsername, GlobalsTable.sharingTable, lblGreetingText.Text, false);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == "pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            PdfForm displayPdf = new PdfForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayPdf.Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            PtxForm displayPtx = new PtxForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayPtx.Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == "msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            MsiForm displayMsi = new MsiForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayMsi.Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            WordDocForm displayMsi = new WordDocForm(filesInfoSharedToMe[accessIndex].Item1, GlobalsTable.sharingTable, lblGreetingText.Text, uploaderUsername, false);
                            displayMsi.Show();
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
            GlobalsData.base64EncodedThumbnailSharedToMe.Clear();

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

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;

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
                            GlobalsData.base64EncodedThumbnailFolder.Clear();
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

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }


                        onPressedEvent.Add(imageOnPressed);

                    }

                    if (Globals.textTypes.Contains(typeValues[i])) {

                        var getExtension = fileName.Split('.').Last();
                        var textTypeToImage = Globals.textTypeToImageFolder[getExtension];
                        imageValues.Add(textTypeToImage);

                        void textOnPressed(object sender, EventArgs e) {
                            TextForm displayPic = new TextForm(GlobalsTable.folderUploadTable, fileName, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        }

                        onPressedEvent.Add(textOnPressed);
                    }

                    if (typeValues[i] == "exe") {

                        imageValues.Add(Globals.EXEImage);

                        void exeOnPressed(object sender, EventArgs e) {
                            exeFORM displayExe = new exeFORM(fileName, GlobalsTable.folderUploadTable, folderName, string.Empty);
                            displayExe.Show();
                        }

                        onPressedEvent.Add(exeOnPressed);
                    }

                    if (Globals.videoTypes.Contains(typeValues[i])) {

                        if (GlobalsData.base64EncodedThumbnailFolder.Count == 0) {
                            await folderDataCaller.AddVideoThumbnailCaching(folderName, filesInfo[i].Item1);
                            
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
                            VideoForm displayVid = new VideoForm(defaultImage, getWidth, getHeight, fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username);
                            displayVid.Show();
                        }

                        onPressedEvent.Add(videoOnPressed);
                    }

                    if (Globals.excelTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.EXCELImage);

                        void excelOnPressed(object sender, EventArgs e) {
                            new ExcelForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
                        }

                        onPressedEvent.Add(excelOnPressed);
                    }
                    if (Globals.audioTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.AudioImage);

                        void audioOnPressed(object sender, EventArgs e) {
                            new AudioForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
                        }

                        onPressedEvent.Add(audioOnPressed);
                    }

                    if (typeValues[i] == "apk") {

                        imageValues.Add(Globals.APKImage);

                        void apkOnPressed(object sender, EventArgs e) {
                            new ApkForm(fileName, tempDataUser.Username, GlobalsTable.folderUploadTable, folderName);
                        }

                        onPressedEvent.Add(apkOnPressed);
                    }

                    if (typeValues[i] == "pdf") {

                        imageValues.Add(Globals.PDFImage);

                        void pdfOnPressed(object sender, EventArgs e) {
                            new PdfForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
                        }

                        onPressedEvent.Add(pdfOnPressed);
                    }

                    if (Globals.ptxTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.PTXImage);

                        void ptxOnPressed(object sender, EventArgs e) {
                            new WordDocForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
                        }

                        onPressedEvent.Add(ptxOnPressed);
                    }

                    if (typeValues[i] == "msi") {

                        imageValues.Add(Globals.MSIImage);

                        void msiOnPressed(object sender, EventArgs e) {
                            new MsiForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
                        }

                        onPressedEvent.Add(msiOnPressed);

                    }

                    if (Globals.wordTypes.Contains(typeValues[i])) {

                        imageValues.Add(Globals.DOCImage);

                        void wordOnPressed(object sender, EventArgs e) {
                            new WordDocForm(fileName, GlobalsTable.folderUploadTable, folderName, tempDataUser.Username).Show();
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

            int _IntCurr = 0;

            StartPopupForm.StartUploadingFolderPopup(folderName);

            GlobalsData.base64EncodedImageFolder.Clear();
            GlobalsData.base64EncodedThumbnailFolder.Clear();

            foreach (var filesFullPath in Directory.EnumerateFiles(folderPath, "*")) {

                _IntCurr++;

                var filePanel = new Guna2Panel() {
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

                var mainPanelTxt = filePanel;

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

                try {

                    string fileType = Path.GetExtension(filesFullPath);

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

                        textboxExl.Image = image;
                        textboxExl.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            PicForm displayPic = new PicForm(defaultImage, getWidth, getHeight, titleLab.Text, GlobalsTable.folderUploadTable, string.Empty, tempDataUser.Username);
                            displayPic.Show();

                        };
                    }

                    if (Globals.textTypes.Contains(fileType)) {

                        textboxExl.Image = Globals.textTypeToImageFolder[fileType];

                        string nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(filesFullPath)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                        string getEncoded = Convert.ToBase64String(getBytes);
                        string encryptEncoded = EncryptionModel.Encrypt(getEncoded);

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptEncoded);

                        textboxExl.Click += (sender_t, e_t) => {

                            TextForm displayPic = new TextForm(GlobalsTable.folderUploadTable, titleLab.Text, string.Empty, tempDataUser.Username);
                            displayPic.Show();
                        };

                    }

                    if (fileType == ".apk") {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.APKImage;
                        textboxExl.Click += (sender_ap, e_ap) => {
                            ApkForm displayPic = new ApkForm(titleLab.Text, tempDataUser.Username, GlobalsTable.folderUploadTable, string.Empty);
                            displayPic.ShowDialog();
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

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {

                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;

                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            VideoForm displayVid = new VideoForm(defaultImage, getWidth, getHeight, titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayVid.ShowDialog();
                        };
                    }

                    if (fileType == ".pdf") {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.PDFImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            PdfForm displayPic = new PdfForm(titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.wordTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            WordDocForm displayPic = new WordDocForm(titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.excelTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            ExcelForm displayPic = new ExcelForm(titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayPic.ShowDialog();
                        };
                    }


                    if (Globals.ptxTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.PTXImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            PtxForm displayPic = new PtxForm(titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayPic.ShowDialog();
                        };
                    }

                    if (Globals.audioTypes.Contains(fileType)) {

                        await insertFileData.InsertFileDataFolder(filesFullPath, folderName, encryptValues);

                        textboxExl.Image = Globals.AudioImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            AudioForm displayPic = new AudioForm(titleLab.Text, GlobalsTable.folderUploadTable, selectedFolder, tempDataUser.Username);
                            displayPic.ShowDialog();
                        };
                    }

                    flwLayoutHome.Controls.Add(filePanel);

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

        private void guna2Button19_Click(object sender, EventArgs e) {

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

                var mainPanel = new Guna2Panel() {
                    Name = "directoryPar" + i,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

                flwLayoutHome.Controls.Add(mainPanel);

                Label directoryLab = new Label();
                mainPanel.Controls.Add(directoryLab);
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
                mainPanel.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.Width = 160;
                titleLab.Height = 20;
                titleLab.AutoEllipsis = true;
                titleLab.Text = directoriesName[i];

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                mainPanel.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 8;
                picMain_Q.Width = 190;
                picMain_Q.Height = 145;
                picMain_Q.Visible = true;

                picMain_Q.Anchor = AnchorStyles.None;

                int picMain_Q_x = (mainPanel.Width - picMain_Q.Width) / 2;
                picMain_Q.Location = new Point(picMain_Q_x, 10);

                picMain_Q.MouseHover += (_senderM, _ev) => {
                    mainPanel.ShadowDecoration.Enabled = true;
                    mainPanel.ShadowDecoration.BorderRadius = 8;
                };

                picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                    mainPanel.ShadowDecoration.Enabled = false;
                };

                Guna2Button remBut = new Guna2Button();
                mainPanel.Controls.Add(remBut);
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

                remBut.Click += async (sender_im, e_im) => {

                    string directoryName = titleLab.Text; 

                    DialogResult verifyDialog = MessageBox.Show(
                        $"Delete {directoryName} directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (verifyDialog == DialogResult.Yes) {
                        
                        await directoryDataCaller.DeleteDirectory(directoryName);

                        mainPanel.Dispose();

                        BuildRedundaneVisibility();
                        UpdateProgressBarValue();

                    }
                };

                picMain_Q.Image = Globals.DIRIcon;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    StartPopupForm.StartRetrievalPopup();

                    new DirectoryForm(titleLab.Text).Show();

                    ClosePopupForm.CloseRetrievalPopup();

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

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        /// <summary>
        /// Go to Home button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button9_Click_1(object sender, EventArgs e) {

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

        private async void guna2Button14_Click(object sender, EventArgs e) {

            new SettingsForm().Show();

            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];

            await currencyConverter.ConvertToLocalCurrency();

        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e) {

        }

        private void RefreshAllOnLogut() {

            pnlMain.SendToBack();

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

                string fileType = Path.GetExtension(selectedItems);

                try {

                    byte[] readFileBytes = File.ReadAllBytes(selectedItems);
                    string toBase64String = Convert.ToBase64String(readFileBytes);
                    string encryptText = EncryptionModel.Encrypt(toBase64String);

                    if (Globals.imageTypes.Contains(fileType)) {
                        curr++;
                        var getImg = new Bitmap(selectedItems);
                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;

                        string tempToBase64 = Convert.ToBase64String(readFileBytes);
                        string encryptedValue = EncryptionModel.Encrypt(tempToBase64);
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeImageTable, "PanImg", curr, encryptedValue);
                        
                    } else if (Globals.textTypes.Contains(fileType)) {
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

                    } else if (fileType == ".exe") {
                        exeCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExeTable, "PanExe", exeCurr, encryptText);

                    } else if (Globals.videoTypes.Contains(fileType)) {
                        vidCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeVideoTable, "PanVid", vidCurr, encryptText);

                    } else if (Globals.excelTypes.Contains(fileType)) {
                        exlCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeExcelTable, "PanExl", exlCurr, encryptText);

                    } else if (Globals.audioTypes.Contains(fileType)) {
                        audCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeAudioTable, "PanAud", audCurr, encryptText);

                    } else if (fileType == ".apk") {
                        apkCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeApkTable, "PanApk", apkCurr, encryptText);

                    } else if (fileType == ".pdf") {
                        pdfCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePdfTable, "PanPdf", pdfCurr, encryptText);

                    } else if (Globals.ptxTypes.Contains(fileType)) {
                        ptxCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homePtxTable, "PanPtx", ptxCurr, encryptText);

                    } else if (fileType == ".msi") {
                        msiCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeMsiTable, "PanMsi", msiCurr, encryptText);

                    } else if (Globals.wordTypes.Contains(fileType)) {
                        docxCurr++;
                        await CreateFilePanelHome(
                            selectedItems, GlobalsTable.homeWordTable, "PanDoc", docxCurr, encryptText);

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

        bool filterTypePanelVisible = false;
        private void guna2Button16_Click(object sender, EventArgs e) {
            filterTypePanelVisible = !filterTypePanelVisible;
            pnlFilterType.Visible = filterTypePanelVisible;
        }

        private void guna2Button18_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".png,.jpeg,.jpg";
        }

        private void guna2Button17_Click_2(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".txt,.html,.md,.sql,.css,.js,.csv";
        }

        private void guna2Button22_Click_1(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".doc,.docx";

        }

        private void guna2Button20_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".mp3,.wav";

        }

        private void guna2Button21_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".xlsx,xls";
        }

        private void guna2Button23_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".mp4,.avi,.mov,wmv";

        }

        private void guna2Button24_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
            txtBoxSearchFile.Text = ".pdf";
        }

        private void guna2Button25_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = string.Empty;
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

            if (tempDataUser.AccountType == "Max" || tempDataUser.AccountType == "Express" || tempDataUser.AccountType == "Supreme") {
                string folderTitleGet = EncryptionModel.Encrypt(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem));
                await DownloadUserFolder(folderTitleGet);

            } else {
                new LimitedFolderAlert(
                    tempDataUser.AccountType, 
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
        private async void btnDeleteFile_Click_1(object sender, EventArgs e) {

            string fileName = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string panelName = lblFilePanelName.Text;
            string sharedToName = lblSharedToName.Text;
            string dirName = lblSelectedDirName.Text;

            DialogResult verifyDialog = MessageBox.Show(
                $"Delete '{fileName}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (verifyDialog == DialogResult.Yes) {

                var deleteFileQuery = new DeleteFileDataQuery();
                await deleteFileQuery.DeleteFileData(tableName, fileName, dirName, sharedToName);

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

            string selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            bool fromSharedFiles = selectedFolder == "Shared Files";

            new shareFileFORM(
                titleFile, fromSharedFiles, tempDataUser.Username, dirName).Show();

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

        private void guna2Button3_Click(object sender, EventArgs e) => this.Close();

        private async void guna2Button2_Click(object sender, EventArgs e) {

            new SettingsForm().Show();

            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];

            await currencyConverter.ConvertToLocalCurrency();

        }

        private void guna2Button1_Click_1(object sender, EventArgs e) {
            pnlShared.Visible = false;
            pnlFilterType.Visible = !pnlFilterType.Visible;
        }

        private void lblItemCountText_Click(object sender, EventArgs e) {

        }

        private void pnlPsSubDetails_Paint(object sender, PaintEventArgs e) {

        }

        private void pnlPublicStorage_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            pnlFilterType.Visible = false;
            pnlShared.Visible = !pnlShared.Visible;
        }

        private async void guna2Button7_Click_1(object sender, EventArgs e) {

            flwLayoutHome.Controls.Clear();

            BuildButtonsOnSharedToMeSelected();
            ClearRedundane();

            await CallFilesInformationOthers();
            await BuildSharedToOthers();

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            lblCurrentPageText.Text = "Shared Files";
            lblCurrentPageText.Visible = true;

            btnDeleteFolder.Visible = false;
            pnlSubPanelDetails.Visible = false;

            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;

            pnlShared.Visible = false;

        }

        private async void guna2Button6_Click_1(object sender, EventArgs e) {

            flwLayoutHome.Controls.Clear();

            BuildButtonOnSharedFilesSelected();
            ClearRedundane();

            await CallFilesInformationSharedToMe();
            await BuildSharedToMe();

            lblItemCountText.Text = flwLayoutHome.Controls.Count.ToString();

            lblCurrentPageText.Text = "Shared To Me";
            lblCurrentPageText.Visible = true;

            pnlSubPanelDetails.Visible = true;
            btnDeleteFolder.Visible = false;

            btnGoHomePage.FillColor = GlobalStyle.TransparentColor;

            pnlShared.Visible = false;

        }

    }
}