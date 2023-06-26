using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading.Tasks;
using System.Threading;
using FlowSERVER1.AlertForms;
using FlowSERVER1.Helper;
using FlowSERVER1.Global;

namespace FlowSERVER1
{ 
    public partial class DirectoryForm : Form {

        public static DirectoryForm instance;

        readonly private Crud crud = new Crud();
        readonly private ImageCompressor compressor = new ImageCompressor();

        readonly private MySqlConnection con = ConnectionModel.con;

        private string _extName { get; set; }

        private string get_ex { get; set; }
        private string getName { get; set; }
        private string retrieved { get; set; }
        private string retrievedName { get; set; }
        private string varDate { get; set; }
        private long fileSizeInMB { get; set; }
        private object keyValMain { get; set; }

        /// <summary>
        /// 
        /// Initialize panel data
        /// 
        /// </summary>

        public DirectoryForm(String directoryName)
        {
            InitializeComponent();

            instance = this;

            this.Text = $"{directoryName} (Directory)";
            this.lblDirectoryName.Text = directoryName;

            Dictionary<string, (string, string)> fileExtensions = new Dictionary<string, (string, string)> {
                { ".png", ("imgFilePng", "file_info") },
                { ".jpg", ("imgFileJpg", "file_info") },
                { ".jpeg", ("imgFilePeg", "file_info") },
                { ".bmp", ("imgFileBmp", "file_info") },
                { ".txt", ("txtFile", "file_info_expand") },
                { ".js", ("txtFile", "file_info_expand") },
                { ".sql", ("txtFile", "file_info_expand") },
                { ".py", ("txtFile", "file_info_expand") },
                { ".html", ("txtFile", "file_info_expand") },
                { ".csv", ("txtFile", "file_info_expand") },
                { ".css", ("txtFile", "file_info_expand") },
                { ".exe", ("exeFile", "file_info_exe") },
                { ".mp4", ("vidFile", "file_info_vid") },
                { ".wav", ("vidFile", "file_info_vid") },
                { ".xlsx", ("exlFile", "file_info_excel") },
                { ".mp3", ("audiFile", "file_info_audi") },
                { ".apk", ("apkFile", "file_info_apk") },
                { ".pdf", ("pdfFile", "file_info_pdf") },
                { ".pptx", ("ptxFile", "file_info_ptx") },
                { ".msi", ("msiFile", "file_info_msi") },
                { ".docx", ("docFile", "file_info_word") },
            };

            foreach (string ext in fileExtensions.Keys) {
                int count = _countRow(ext);
                if (count > 0) {
                    _extName = ext;
                    string controlName = fileExtensions[ext].Item1;
                    string tableName = fileExtensions[ext].Item2;
                    buildFilePanelOnLoad(tableName, controlName, count);
                }
            }

            if(flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            } else {
                clearRedundane();
            }

            lblFilesCount.Text = $"{flowLayoutPanel1.Controls.Count} File(s)";

        }

        private int _countRow(string ext) {
            const string query = "SELECT COUNT(CUST_USERNAME) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                command.Parameters.AddWithValue("@ext", ext);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void clearRedundane() {
            guna2Button6.Visible = false;
            label8.Visible = false;
        }

        private void showRedundane() {
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

        int top = 275;
        int h_p = 100;

        private async void buildFilePanelOnLoad(String _tableName, String parameterName, int currItem) {

            List<(string, string)> filesInfo = new List<(string, string)>();

            const string selectFileDataDir = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (MySqlCommand command = new MySqlCommand(selectFileDataDir, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                command.Parameters.AddWithValue("@ext", _extName); 
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate));
                    }
                }
            }

            List<string> base64EncodedImage = new List<string>();

            if (Globals.imageTypes.Contains(_extName)) {

                if(base64EncodedImage.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", _extName);

                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64EncodedImage.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                            }
                        }
                    }
                }
            }

            List<string> base64EncodedThumbnail = new List<string>();

            if(Globals.videoTypes.Contains(_extName)) {

                if(base64EncodedThumbnail.Count == 0) {

                    const string retrieveImgQuery = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(lblDirectoryName.Text));
                        command.Parameters.AddWithValue("@ext", _extName);
                        using (var readBase64 = await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64EncodedThumbnail.Add(readBase64.GetString(0));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < currItem; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = parameterName + i,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = $"LabG{i}";
                dateLab.Font = GlobalStyle.DateLabelFont;
                dateLab.ForeColor = GlobalStyle.DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = GlobalStyle.DateLabelLoc;
                dateLab.Text = filesInfo[i].Item2;

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}";
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
                picMain_Q.Name = $"ImgG{i}";
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
                remBut.ImageOffset = new Point(2,0);
                remBut.FillColor = GlobalStyle.TransparentColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = GlobalStyle.TransparentColor;
                remBut.Image = GlobalStyle.GarbageImage;
                remBut.Visible = true;
                remBut.Location = GlobalStyle.GarbageButtonLoc;

                remBut.Click += (sender_im, e_im) => {

                    pnlFileOptions.Visible = true;
                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFilePanelName.Text = panelF.Name;

                };

                guna2Button6.Visible = false;
                label8.Visible = false;
                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == GlobalsTable.homeImageTable) {

                    if (base64EncodedImage.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64EncodedImage[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            picMain_Q.Image = Image.FromStream(toMs);
                        }
                    }

                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();

                    };
                }

                if (_tableName == GlobalsTable.homeVideoTable) {

                    if (base64EncodedThumbnail.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64EncodedThumbnail[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            picMain_Q.Image = Image.FromStream(toMs);
                        }
                    }

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        vidFormShow.Show();
                    };
                }


                if (_tableName == GlobalsTable.homeTextTable) {

                    string textTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    picMain_Q.Image = Globals.textTypeToImage[textTypes];
                   
                    picMain_Q.Click += (sender_t, e_t) => {

                        if (textTypes == ".csv" || textTypes == ".sql") {
                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                            _showRetrievalCsvAlert.Start();
                        }

                        txtFORM displayPic = new txtFORM("", "upload_info_directory", titleLab.Text, lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (_tableName == GlobalsTable.homeExeTable) {
                    img.Image = Globals.EXEImage;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayExe.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == GlobalsTable.homeExcelTable) {
                    picMain_Q.Image = Globals.EXCELImage;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        exlForm.Show();
                    };
                }
                if (_tableName == GlobalsTable.homeAudioTable) {
                    picMain_Q.Image = Globals.AudioImage;
                    picMain_Q.Click += (sender_Aud, e_Aud) => {
                        Form bgBlur = new Form();
                        audFORM displayPic = new audFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (_tableName == GlobalsTable.homeApkTable) {
                    picMain_Q.Image = Globals.APKImage;
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "upload_info_directory", lblDirectoryName.Text);
                        displayPic.Show();
                    };
                }

                if (_tableName == GlobalsTable.homePdfTable) {
                    picMain_Q.Image = Globals.PDFImage;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPdf.Show();
                    };
                }

                if (_tableName == GlobalsTable.homePtxTable) {
                    picMain_Q.Image = Globals.PTXImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayPtx.Show();
                    };
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = Globals.MSIImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayMsi.Show();
                    };
                }

                if (_tableName == GlobalsTable.homeWordTable) {
                    picMain_Q.Image = Globals.DOCImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                        displayMsi.Show();
                    };
                }
            }
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
                // Add File  
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
        private string _controlName;

        private async Task startSending(string setValue) {

            try {

                const string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)";
                var param = new Dictionary<string, string>
                {
                    { "@CUST_USERNAME", Globals.custUsername},
                    { "@CUST_FILE_PATH", EncryptionModel.Encrypt(getName)},
                    { "@UPLOAD_DATE", varDate},
                    { "@CUST_FILE", setValue},
                    { "@CUST_THUMB", ""},
                    { "@FILE_EXT", retrieved},
                    { "@DIR_NAME", EncryptionModel.Encrypt(lblDirectoryName.Text)}
                };

                await crud.Insert(insertQuery, param);

                Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

            }
            catch (Exception) {
                new CustomAlert("Something went wrong","Failed to upload this file.").Show();
            }
        }

        private async Task containThumbUpload(string filePath, object keyValMain) {

            using (var command = new MySqlCommand()) {
                command.Connection = con;
                command.CommandText = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)"; ;
                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getName));
                command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                command.Parameters.AddWithValue("@FILE_EXT", retrieved);
                command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
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

            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

        }

        private void buildFilePanel(int AccountType_, String _AccountTypeStr_) {

            var form1 = HomePage.instance;

            var open = new OpenFileDialog {
                Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp;|Video Files|*.mp4;*.webm;.mov;.wmv|Text Files|*.txt;*.md|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv",
                Multiselect = true
            };

            varDate = DateTime.Now.ToString("dd/MM/yyyy");
        
            int curFilesCount = flowLayoutPanel1.Controls.Count;

            if (open.ShowDialog() == DialogResult.OK) {

                List<string> _filValues = open.FileNames.Select(Path.GetFileName).ToList();

                if (open.FileNames.Length + curFilesCount > AccountType_) {
                    Form bgBlur = new Form();
                    using (UpgradeAccountAlert displayUpgrade = new UpgradeAccountAlert(_AccountTypeStr_)) {
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

                } else {

                    HashSet<string> existingLabels = new HashSet<string>(flowLayoutPanel1.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Select(label => label.Text.ToLower()));

                    foreach (var selectedItems in open.FileNames) {

                        if(existingLabels.Contains(Path.GetFileName(selectedItems).ToLower().Trim())) {
                            continue;
                        }

                        _filValues.Add(Path.GetFileName(selectedItems));

                        get_ex = open.FileName;
                        getName = Path.GetFileName(selectedItems);
                        retrieved = Path.GetExtension(selectedItems); 
                        retrievedName = Path.GetFileNameWithoutExtension(open.FileName);
                        fileSizeInMB = 0;

                        async void createPanelMain(string nameTable, string panName, int itemCurr, string keyVal) {

                            if (fileSizeInMB < 8000) {

                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = panName + itemCurr,
                                    Width = 200,
                                    Height = 222,
                                    BorderColor = GlobalStyle.BorderColor,
                                    BorderThickness = 1,
                                    BorderRadius = 8,
                                    BackColor = GlobalStyle.TransparentColor,
                                    Location = new Point(600, top)
                                };
                 
                                top += h_p;
                                flowLayoutPanel1.Controls.Add(panelTxt);
                                var mainPanelTxt = (Guna2Panel)panelTxt;
                                _controlName = panName + itemCurr;

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
                                dateLabTxt.Text = varDate;

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
                                titleLab.Text = getName;

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxtBut" + itemCurr;
                                remButTxt.Width = 29;
                                remButTxt.Height = 26;
                                remButTxt.ImageOffset = new Point(2,0);
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

                                new Thread(() => new UploadingAlert(getName, Globals.custUsername, "null", panName + itemCurr, "null", _fileSize: fileSizeInMB).ShowDialog())
                                .Start();

                                if (nameTable == GlobalsTable.homeImageTable) {

                                    await startSending(keyVal);

                                    textboxPic.Image = new Bitmap(selectedItems);
                                    textboxPic.Click += (sender_f, e_f) => {

                                        var getImgName = (Guna2PictureBox)sender_f;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                                        picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayPic.Show();
                                    };

                                    clearRedundane();
                                }

                                if (nameTable == "file_info_expand") {

                                    await startSending(keyVal);

                                    string textType = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                                    textboxPic.Image = Globals.textTypeToImage[textType];

                                    var filePath = getName;

                                    textboxPic.Click += (sender_t, e_t) => {

                                        if (textType == ".csv" || textType == ".sql") {
                                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                                            _showRetrievalCsvAlert.Start();
                                        }

                                        txtFORM txtFormShow = new txtFORM("", "upload_info_directory", titleLab.Text, lblDirectoryName.Text, Globals.custUsername);
                                        txtFormShow.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_exe") {

                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync(); 

                                    textboxPic.Image = Globals.EXEImage;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        Form bgBlur = new Form();
                                        exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayExe.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_vid") {

                                    await containThumbUpload(selectedItems,keyVal); 

                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        var getImgName = (Guna2PictureBox)sender_ex;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                                        vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        vidShow.Show();
                                    };

                                    clearRedundane();
                                }
                                if (nameTable == "file_info_audi") {

                                    await startSending(keyVal);

                                    textboxPic.Image = Globals.AudioImage;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        audFORM displayPic = new audFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_excel") {

                                    await startSending(keyVal);

                                    textboxPic.Image = Globals.EXCELImage;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_apk") {

                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = Globals.APKImage;
                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "upload_info_directory", lblDirectoryName.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_pdf") {

                                    await startSending(keyVal);

                                    textboxPic.Image = Globals.PDFImage;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_ptx") {

                                    await startSending(keyVal);

                                    textboxPic.Image = Globals.PTXImage;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayPtx.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_msi") {

                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = Globals.MSIImage;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername); 
                                        displayMsi.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_word") {
                                    await startSending(keyVal);
                                    textboxPic.Image = Globals.DOCImage;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "upload_info_directory", lblDirectoryName.Text, Globals.custUsername);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                remButTxt.Click += (sender_tx, e_tx) => {

                                    pnlFileOptions.Visible = true;
                                    lblFileNameOnPanel.Text = titleLab.Text;
                                    lblFilePanelName.Text = panelTxt.Name;
                                };

                            } else {
                                MessageBox.Show("File is too large, max file size is 8GB.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            foreach (var form in Application.OpenForms.OfType<Form>().Where(form => form.Name == "UploadAlrt").ToList()) {
                                form.Close();
                            }
                        }

                        try {

                            Byte[] _getBytesSelectedFiles = File.ReadAllBytes(selectedItems);
                            fileSizeInMB = (_getBytesSelectedFiles.Length / 1024) / 1024;

                            if (Globals.imageTypes.Contains(retrieved)) {

                                curr++;

                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                if (retrieved != ".ico") {

                                    string _compressedImageBase64 = compressor.compresImageToBase64(selectedItems);
                                    string _encryptedValue = EncryptionModel.Encrypt(_compressedImageBase64);
                                    createPanelMain("file_info", "PanImg", curr, _encryptedValue);

                                } else {

                                    Image retrieveIcon = Image.FromFile(selectedItems);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {

                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);

                                        dataIco = msIco.ToArray();
                                        string _tempToBase64 = Convert.ToBase64String(dataIco);
                                        string _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);

                                        createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                    }
                                }
                            }

                            else if (Globals.textTypes.Contains(retrieved)) {
                                txtCurr++;
                                String nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) { 
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }

                                byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                                String getEncoded = Convert.ToBase64String(getBytes);
                                String encryptText = EncryptionModel.Encrypt(getEncoded);

                                createPanelMain("file_info_expand", "PanTxt", txtCurr, encryptText);
                            }
                            else if (retrieved == ".exe") {
                                exeCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_exe", "PanExe", exeCurr, _encryptValue);

                            }
                            else if (Globals.videoTypes.Contains(retrieved)) {
                                vidCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_vid", "PanVid", vidCurr, _encryptValue);
                            }
                            else if (retrieved == ".xlsx" || retrieved == ".xls") {
                                exlCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_excel", "PanExl", exlCurr, _encryptValue);
                            }
                            else if (retrieved == ".mp3" || retrieved == ".wav") {
                                audCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_audi", "PanAud", audCurr, _encryptValue); 
                            }
                            else if (retrieved == ".apk") {
                                apkCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_apk", "PanApk", apkCurr, _encryptValue);
                            }
                            else if (retrieved == ".pdf") {
                                pdfCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_pdf", "PanPdf", pdfCurr, _encryptValue);
                            }
                            else if (retrieved == ".pptx" || retrieved == ".ppt") {
                                ptxCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_ptx", "PanPtx", ptxCurr, _encryptValue);
                            }
                            else if (retrieved == ".msi") {
                                msiCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_msi", "PanMsi", msiCurr, _encryptValue);
                            }
                            else if (retrieved == ".docx") {
                                docxCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_word", "PanDoc", docxCurr, _encryptValue);
                            }

                            CloseForm.closeForm("UploadAlrt");

                        } catch (Exception) {
                            CloseForm.closeForm("UploadAlrt");
                            new CustomAlert(title: "Some went wrong",subheader: "Failed to upload this file.").Show();
                        }

                    }
                }
            }

            lblFilesCount.Text = $"{flowLayoutPanel1.Controls.Count} File(s)";

        }

        public void DisplayError(String CurAcc) {
            Form bgBlur = new Form();
            using (UpgradeAccountAlert displayPic = new UpgradeAccountAlert(CurAcc)) {
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


        private async Task<int> countTotalFilesDirectory(String directoryName) {

            int count = 0;

            const string query = "SELECT COUNT(*) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
            using(MySqlCommand command = new MySqlCommand(query,con)) {
                command.Parameters.AddWithValue("@username",Globals.custUsername);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(directoryName));
                object results = await command.ExecuteScalarAsync();
                count = Convert.ToInt32(results);
            }

            return count;
        }
   
        private async void guna2Button2_Click_1(object sender, EventArgs e) {

            try {

                int currentUploadCount = await countTotalFilesDirectory(lblDirectoryName.Text);

                if (currentUploadCount != Globals.uploadFileLimit[Globals.accountType]) {
                    buildFilePanel(Globals.uploadFileLimit[Globals.accountType], Globals.accountType);
                }
                else {
                    DisplayError(Globals.accountType);
                }

           } catch (Exception) {

                CloseForm.closeForm("UploadAlrt");

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

                Control[] matches = this.Controls.Find(panelname,true);
                if(matches.Length > 0 && matches[0] is Panel) {
                    Panel myPanel = (Panel)matches[0];
                    myPanel.Dispose();
                }


                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }

                pnlFileOptions.Visible = false;

            }
        }

        private void guna2Button28_Click(object sender, EventArgs e) {
            pnlFileOptions.Visible = false;
        }

        private void guna2Button30_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = "file_info_directory";
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

            string fileExtensions = titleFile.Substring(titleFile.Length - 4);

            shareFileFORM sharingFileFORM = new shareFileFORM(titleFile, fileExtensions, false, "upload_info_directory", dirName);
            sharingFileFORM.Show();
        }
    }
}