using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Query;
using FlowstorageDesktop.Query.DataCaller;
using FlowstorageDesktop.Temporary;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class DirectoryForm : Form {

        public static DirectoryForm instance;

        readonly private Crud crud = new Crud();
        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();
        readonly private DirectoryDataCaller directoryDataCaller = new DirectoryDataCaller();

        readonly private MySqlConnection con = ConnectionModel.con;
        private string _todayDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public DirectoryForm(string directoryName) {
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

                string encryptedFileName = EncryptionModel.Encrypt(fileName);
                string encryptedDirectoryname = EncryptionModel.Encrypt(lblDirectoryName.Text);

                const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, DIR_NAME) VALUES (@file_name, @username, @date, @file_data, @thumbnail, @dir_name)";

                var param = new Dictionary<string, string>
                {
                    { "@username", tempDataUser.Username},
                    { "@file_name", encryptedFileName},
                    { "@dir_name", encryptedDirectoryname},
                    { "@date", _todayDate},
                    { "@file_data", fileBase64EncodedData},
                    { "@thumbnail", ""},
                };

                await crud.Insert(insertQuery, param);

                ClosePopupForm.CloseUploadingPopup();

            } catch (Exception) {
                OnUploadFailed();
            }

        }

        private async Task InsertFileDataVideo(string filePath, string fileBase64EncodedData) {

            string fileName = Path.GetFileName(filePath);

            var fileSizeInMb = Convert.FromBase64String(fileBase64EncodedData).Length / 1024 / 1024;

            StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

            string encryptedFileName = EncryptionModel.Encrypt(fileName);
            string encryptedDirectoryName = EncryptionModel.Encrypt(lblDirectoryName.Text);

            const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, DIR_NAME) VALUES (@file_name, @username, @date, @file_data, @thumbnail, @dir_name)";

            using (var command = new MySqlCommand(insertQuery, con)) {
                command.Parameters.AddWithValue("@file_name", encryptedFileName);
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
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

            await BuildFilePanelOnLoad();

            BuildRedundaneVisibility();
            lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";

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

        private async Task BuildFilePanelOnLoad() {

            var imageValues = new List<Image>();
            var onPressedEvent = new List<EventHandler>();
            var onMoreOptionButtonPressed = new List<EventHandler>();

            var base64EncodedImage = new List<string>();

            string directoryName = lblDirectoryName.Text;

            var filesInfo = await directoryDataCaller.GetFileMetadata(directoryName);

            var typeValues = filesInfo.Select(metadata => metadata.Item1.Split('.').Last()).ToList();

            int length = typeValues.Count;

            for (int i = 0; i < length; i++) {

                int accessIndex = i;
                string fileName = filesInfo[accessIndex].Item1;

                void moreOptionOnPressedEvent(object sender, EventArgs e) {
                    pnlFileOptions.Visible = true;
                    lblFileNameOnPanel.Text = filesInfo[accessIndex].Item1;
                    lblFilePanelName.Text = "dirParam" + accessIndex;
                }

                onMoreOptionButtonPressed.Add(moreOptionOnPressedEvent);

                if (Globals.imageTypes.Contains(typeValues[i])) {

                    if (base64EncodedImage.Count == 0) {
                        var images = await directoryDataCaller.GetImage(directoryName);
                        base64EncodedImage.AddRange(images);

                    }

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

                if (Globals.videoTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.VideoImage);

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

                if (Globals.textTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.TextImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        TextForm displayPic = new TextForm(GlobalsTable.directoryUploadTable, fileName, lblDirectoryName.Text, tempDataUser.Username);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (typeValues[i] == "exe") {

                    imageValues.Add(Globals.EXEImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ExeForm displayExe = new ExeForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        displayExe.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (Globals.excelTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.EXCELImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ExcelForm exlForm = new ExcelForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        exlForm.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (Globals.audioTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.AudioImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        AudioForm audioOnPressed = new AudioForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username);
                        audioOnPressed.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (typeValues[i] == "apk") {

                    imageValues.Add(Globals.APKImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        ApkForm displayPic = new ApkForm(fileName, tempDataUser.Username, GlobalsTable.directoryUploadTable, lblDirectoryName.Text);
                        displayPic.Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (typeValues[i] == "pdf") {

                    imageValues.Add(Globals.PDFImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PdfForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (Globals.ptxTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.PTXImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new PtxForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);
                }

                if (typeValues[i] == "msi") {

                    imageValues.Add(Globals.MSIImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new MsiForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);

                }

                if (Globals.wordTypes.Contains(typeValues[i])) {

                    imageValues.Add(Globals.DOCImage);

                    void videoOnPressed(object sender, EventArgs e) {
                        new WordsForm(fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).Show();
                    }

                    onPressedEvent.Add(videoOnPressed);

                }
            }

            PanelGenerator panelGenerator = new PanelGenerator();
            panelGenerator.GeneratePanel("dirParam", length, filesInfo, onPressedEvent, onMoreOptionButtonPressed, imageValues, isFromDirectory: true);

        }

        private void btnCloseDirectory_Click(object sender, EventArgs e) => this.Close();
    
        /// <summary>
        /// Initialize file panel variable (for increment)
        /// </summary>

        int paramCurr = 0;

        private async Task CreateFilePanel(string fileFullPath, string tableName, string fileData) {

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

            int picMain_Q_x = (panel.Width - panelImage.Width) / 2;

            panelImage.Location = new Point(picMain_Q_x, 10);

            Label dateLabel = new Label();
            panel.Controls.Add(dateLabel);
            dateLabel.Name = paramName;
            dateLabel.Font = GlobalStyle.DateLabelFont;
            dateLabel.ForeColor = GlobalStyle.DarkGrayColor;
            dateLabel.Location = GlobalStyle.DateLabelLoc;
            dateLabel.Text = _todayDate;

            Label titleLab = new Label();
            panel.Controls.Add(titleLab);
            titleLab.Name = paramName;
            titleLab.Font = GlobalStyle.TitleLabelFont;
            titleLab.ForeColor = GlobalStyle.GainsboroColor;
            titleLab.Location = GlobalStyle.TitleLabelLoc;
            titleLab.Width = 160;
            titleLab.Height = 20;
            titleLab.AutoEllipsis = true;
            titleLab.Text = fileName;

            Guna2Button moreOptionsButton = new Guna2Button();
            panel.Controls.Add(moreOptionsButton);
            moreOptionsButton.Name = paramName;
            moreOptionsButton.Width = 29;
            moreOptionsButton.Height = 26;
            moreOptionsButton.ImageOffset = new Point(2, 0);
            moreOptionsButton.FillColor = GlobalStyle.TransparentColor;
            moreOptionsButton.BorderRadius = 6;
            moreOptionsButton.BorderThickness = 1;
            moreOptionsButton.BorderColor = GlobalStyle.TransparentColor;
            moreOptionsButton.Image = GlobalStyle.GarbageImage;
            moreOptionsButton.Location = GlobalStyle.GarbageButtonLoc;

            moreOptionsButton.Click += (sender_tx, e_tx) => {
                pnlFileOptions.Visible = true;
                lblFileNameOnPanel.Text = titleLab.Text;
                lblFilePanelName.Text = panel.Name;
            };

            panelImage.MouseHover += (_senderM, _ev) => {
                panel.ShadowDecoration.Enabled = true;
                panel.ShadowDecoration.BorderRadius = 12;
            };

            panelImage.MouseLeave += (_senderQ, _evQ) => {
                panel.ShadowDecoration.Enabled = false;
            };

            if (tableName == GlobalsTable.homeImageTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = new Bitmap(fileFullPath);
                panelImage.Click += (sender_f, e_f) => {

                    var getImgName = (Guna2PictureBox)sender_f;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                    new PicForm(
                        defaultImage, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog(); 
                };

            }

            if (tableName == GlobalsTable.homeTextTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.TextImage;
                panelImage.Click += (sender_t, e_t) => {
                    new TextForm(
                        GlobalsTable.directoryUploadTable, fileName, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeExeTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.EXEImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExeForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeVideoTable) {

                await InsertFileDataVideo(fileFullPath, fileData);

                ShellFile shellFile = ShellFile.FromFilePath(fileFullPath);
                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                panelImage.Image = toBitMap;

                panelImage.Click += (sender_ex, e_ex) => {
                    var getImgName = (Guna2PictureBox)sender_ex;
                    var getWidth = getImgName.Image.Width;
                    var getHeight = getImgName.Image.Height;
                    Bitmap defaultImg = new Bitmap(getImgName.Image);

                    new VideoForm(
                        defaultImg, getWidth, getHeight, fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                    
                };

            }
            if (tableName == GlobalsTable.homeAudioTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.AudioImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new AudioForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeExcelTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.EXCELImage;
                panelImage.Click += (sender_ex, e_ex) => {
                    new ExcelForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeApkTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.APKImage;
                panelImage.Click += (sender_gi, e_gi) => {
                    new ApkForm(
                        fileName, tempDataUser.Username, GlobalsTable.directoryUploadTable, lblDirectoryName.Text).ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homePdfTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.PDFImage;
                panelImage.Click += (sender_pd, e_pd) => {
                    new PdfForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homePtxTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.PTXImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new PtxForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }
            if (tableName == GlobalsTable.homeMsiTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.MSIImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new MsiForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            if (tableName == GlobalsTable.homeWordTable) {

                await InsertFileData(fileName, fileData);

                panelImage.Image = Globals.DOCImage;
                panelImage.Click += (sender_ptx, e_ptx) => {
                    new WordsForm(
                        fileName, GlobalsTable.directoryUploadTable, lblDirectoryName.Text, tempDataUser.Username).ShowDialog();
                };
            }

            flwLayoutDirectory.Controls.Add(panel);

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
                    string fileType = selectedItems.Split('.').Last();

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
                            var getImg = new Bitmap(selectedItems);
                            var imgWidth = getImg.Width;
                            var imgHeight = getImg.Height;

                            string compressedImageBase64 = compressor.compressImageToBase64(selectedItems);
                            string encryptedImage = EncryptionModel.Encrypt(compressedImageBase64);
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeImageTable, encryptedImage);
                                
                        } else if (Globals.textTypes.Contains(fileType)) {
                            string nonLine = "";
                            using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) {
                                nonLine = ReadFileTxt.ReadToEnd();
                            }

                            byte[] getBytes = Encoding.UTF8.GetBytes(nonLine);
                            byte[] compressedTextBytes = compressor.compressFileData(getBytes);
                            string getEncoded = Convert.ToBase64String(compressedTextBytes);
                            string encryptEncodedText = EncryptionModel.Encrypt(getEncoded);

                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeTextTable, encryptEncodedText);

                        } else if (fileType == "exe") {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeExeTable, encryptBase64String);

                        } else if (Globals.videoTypes.Contains(fileType)) {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeVideoTable, encryptBase64String);

                        } else if (Globals.excelTypes.Contains(fileType)) {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeExcelTable, encryptBase64String);

                        } else if (Globals.audioTypes.Contains(fileType)) {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeAudioTable, encryptBase64String);

                        } else if (fileType == "apk") {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeApkTable, encryptBase64String);

                        } else if (fileType == "pdf") {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homePdfTable, encryptBase64String);

                        } else if (Globals.ptxTypes.Contains(fileType)) {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homePtxTable, encryptBase64String);

                        } else if (fileType == "msi") {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeMsiTable, encryptBase64String);

                        } else if (Globals.wordTypes.Contains(fileType)) {
                            await CreateFilePanel(
                                selectedItems, GlobalsTable.homeWordTable, encryptBase64String);

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
                OnUploadFailed();
            }

        }

        private async void btnDeleteFile_Click(object sender, EventArgs e) {

            string fileName = lblFileNameOnPanel.Text;
            string panelname = lblFilePanelName.Text;
            string directoryName = lblDirectoryName.Text;

            var verifyDialog = MessageBox.Show(
                $"Delete '{fileName}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (verifyDialog == DialogResult.Yes) {
                
                var deleteFileQuery = new DeleteFileDataQuery();
                await deleteFileQuery.DeleteFileData(
                    GlobalsTable.directoryUploadTable, fileName, directoryName);

                Control[] matches = this.Controls.Find(panelname, true);

                if (matches.Length > 0 && matches[0] is Guna2Panel) {
                    var filePanel = matches[0];
                    filePanel.Dispose();
                }

                BuildRedundaneVisibility();

                pnlFileOptions.Visible = false;
                lblFilesCount.Text = $"{flwLayoutDirectory.Controls.Count} File(s)";

            }
        }

        private void btnHideFileOptionsPnl_Click(object sender, EventArgs e) => pnlFileOptions.Visible = false;

        private void btnOpenRenameFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = GlobalsTable.directoryUploadTable;
            string panelName = lblFilePanelName.Text;
            string dirName = lblDirectoryName.Text;

            new RenameFileForm(titleFile, tableName, panelName, dirName).Show();

        }

        private void btnDownloadFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            SaverModel.SaveSelectedFile(titleFile, GlobalsTable.directoryUploadTable, dirName);

        }

        private void btnOpenShareFile_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblDirectoryName.Text;

            new ShareSelectedFileForm(
                titleFile, false, GlobalsTable.directoryUploadTable, dirName).Show();

        }

    }
}