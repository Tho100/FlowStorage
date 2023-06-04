using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;
using System.Runtime.Caching;
using System.Xml;
using FlowSERVER1.Authentication;

namespace FlowSERVER1 {

    public partial class HomePage : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;

        private Crud crudClass = new Crud();

        public static HomePage instance { get; set;} = new HomePage();
        public Label setupLabel { get; set; }
        public string CurrentLang { get; set; }
        public string nameTableInsert { get; private set; }

        // Initialize file (caching)

        private List<string> base64EncodedImageSharedOthers = new List<string>();
        private List<string> base64EncodedImageHome = new List<string>();
        private List<string> base64EncodedThumbnailHome = new List<string>();

        private string get_ex;
        private string getName;
        private string retrieved;
        private string retrievedName;
        private object keyValMain;
        private long fileSizeInMB;
        private string varDate;
        private string tableName;

        /// <summary>
        /// 
        /// Initialize panel data
        /// 
        /// </summary>
        
        // Date label
        private const string DateLabelFontName = "Segoe UI Semibold";
        private const float DateLabelFontSize = 9f;
        private readonly Font DateLabelFont = new Font(DateLabelFontName, DateLabelFontSize, FontStyle.Bold);

        // Title label
        private const string TitleLabelFontName = "Segoe UI Semibold";
        private const float TitleLabelFontSize = 11f; 
        private readonly Font TitleLabelFont = new Font(TitleLabelFontName, TitleLabelFontSize, FontStyle.Bold);

        // Panel
        private readonly Color BorderColor = ColorTranslator.FromHtml("#212121");
        private readonly Color DarkGrayColor = Color.DarkGray;
        private readonly Color GainsboroColor = Color.Gainsboro;
        private readonly Color TransparentColor = Color.Transparent;
        private readonly Point TitleLabelLoc = new Point(12,166); 
        private readonly Point DateLabelLoc = new Point(12,192);

        // Garbage button
        private readonly Color BorderColor2 = ColorTranslator.FromHtml("#232323");
        private readonly Color FillColor = ColorTranslator.FromHtml("#4713BF");
        private readonly Image GarbageImage = FlowSERVER1.Properties.Resources.icons8_menu_vertical_30;
        private readonly Point GarbageButtonLoc = new Point(165, 188);
        private readonly Point GarbageOffset = new Point(2, 0); 

        private readonly Image DirectoryGarbageImage = FlowSERVER1.Properties.Resources.icons8_garbage_66__1_;

        // File Images
        private readonly Image TextImage = FlowSERVER1.Properties.Resources.icons8_txt_48;
        private readonly Image PDFImage = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
        private readonly Image AudioImage = FlowSERVER1.Properties.Resources.icons8_audio_file_60;

        private readonly Image DOCImage = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
        private readonly Image PTXImage = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
        private readonly Image APKImage = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;

        private readonly Image EXCELImage = FlowSERVER1.Properties.Resources.excelIcon;
        private readonly Image CSVImage = FlowSERVER1.Properties.Resources.icons8_csv_48;
        private readonly Image MSIImage = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
        private readonly Image EXEImage = FlowSERVER1.Properties.Resources.icons8_exe_48;

        private int top = 275; private int h_p = 100;

        public HomePage() {

            InitializeComponent();

            this.AllowDrop = true;

            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragOver += new DragEventHandler(Form1_DragOver);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            this.DragLeave += new EventHandler(Form1_DragLeave);

            instance = this;

            var form4Instances = Application.OpenForms.OfType<Form>().Where(form => form.Name == "Form4").ToList();
            form4Instances.ForEach(form => form.Close());

            this.flowLayoutPanel1.HorizontalScroll.Maximum = 0;
            this.flowLayoutPanel1.VerticalScroll.Maximum = 0;
            this.flowLayoutPanel1.AutoScrollMinSize = new Size(0, 0);

            this.flowLayoutPanel1.BorderStyle = BorderStyle.None;

            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.HorizontalScroll.Visible = false;
            this.flowLayoutPanel1.VerticalScroll.Visible = false;

            this.TopMost = false;
        }

        /// <summary>
        /// 
        /// Generate user files on startup
        /// 
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>

        public async Task _generateUserFiles(String _tableName, String parameterName, int currItem) {

            List<(string, string)> filesInfo = new List<(string, string)>();
            string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM {_tableName} WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    List<(string, string)> tuplesList = new List<(string, string)>();
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        tuplesList.Add((fileName, uploadDate));
                    }
                    filesInfo.AddRange(tuplesList);
                }
            }

            btnGarbageImage.Visible = false;
            lblEmptyHere.Visible = false;

            for (int i = 0; i < currItem; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = $"{parameterName + i}",
                    Width = 200, 
                    Height = 222, 
                    BorderColor = BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = $"LabG{i}";
                dateLab.Font = DateLabelFont;
                dateLab.ForeColor = DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = DateLabelLoc;
                dateLab.Text = filesInfo[i].Item2;

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}"; 
                titleLab.Font = TitleLabelFont;
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
                titleLab.AutoEllipsis = true;
                titleLab.Width = 160; 
                titleLab.Height = 20; 
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
                remBut.ImageOffset = GarbageOffset;
                remBut.FillColor = TransparentColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = TransparentColor;
                remBut.Image = GarbageImage; 
                remBut.Visible = true;
                remBut.Location = GarbageButtonLoc; 

                remBut.Click += (sender_im, e_im) => {

                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFileTableName.Text = _tableName;
                    lblFilePanelName.Text = panelPic_Q.Name;
                    pnlFileOptions.Visible = true;

                };

                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == "file_info") {

                   if(base64EncodedImageHome.Count == 0) {

                        string retrieveImgQuery = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    string base64String = EncryptionModel.Decrypt(readBase64.GetString(0));
                                    base64EncodedImageHome.Add(base64String);
                                }
                            }
                        }

                        if (base64EncodedImageHome.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64EncodedImageHome[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image setImageStream = Image.FromStream(toMs);
                                picMain_Q.Image = setImageStream;
                            }
                        }

                    } else {

                        if (base64EncodedImageHome.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64EncodedImageHome[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image setImageStream = Image.FromStream(toMs);
                                picMain_Q.Image = setImageStream;
                            }
                        }

                    }
                    

                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"file_info","null",Globals.custUsername)) {
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

                    };
                }

                if (_tableName == "file_info_expand") {
                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    if (_extTypes == ".py") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (_extTypes == ".txt" || _extTypes == ".md") {
                        img.Image = TextImage;
                    }
                    else if (_extTypes == ".html") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                    }
                    else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                    }
                    else if (_extTypes == ".js") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    } else if (_extTypes == ".sql") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (_extTypes == ".csv") {
                        img.Image = CSVImage;
                    }

                    picMain_Q.Click += (sender_t, e_t) => {

                        if (_extTypes == ".csv" || _extTypes == ".sql") {
                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                            _showRetrievalCsvAlert.Start();
                        }

                        txtFORM displayPic = new txtFORM("IGNORETHIS", "file_info_expand", titleLab.Text,"null",Globals.custUsername);
                         displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    img.Image = EXEImage;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",Globals.custUsername);
                        displayExe.Show();
                    };
                }

                if (_tableName == "file_info_vid") {

                    if(base64EncodedThumbnailHome.Count == 0) {

                        string retrieveImgQuery = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username";
                        using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);

                            using (var readBase64 = await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    base64EncodedThumbnailHome.Add(readBase64.GetString(0));
                                }
                            }
                        }

                        if (base64EncodedThumbnailHome.Count > i && !string.IsNullOrEmpty(base64EncodedThumbnailHome[i])) {
                            byte[] getBytes = Convert.FromBase64String(base64EncodedThumbnailHome[i]);
                            using (var toMs = new MemoryStream(getBytes)) {
                                img.Image = Image.FromStream(toMs);
                            }
                        }

                    } else {

                        if (base64EncodedThumbnailHome.Count > i && !string.IsNullOrEmpty(base64EncodedThumbnailHome[i])) {
                            byte[] getBytes = Convert.FromBase64String(base64EncodedThumbnailHome[i]);
                            using (var toMs = new MemoryStream(getBytes)) {
                                img.Image = Image.FromStream(toMs);
                            }
                        }

                    }

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid","null",Globals.custUsername);
                        vidFormShow.Show();
                    };
                }

                if (_tableName == "file_info_excel") {
                    picMain_Q.Image = EXCELImage;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text,"file_info_excel","null",Globals.custUsername);
                        exlForm.Show();
                    };
                }
                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = AudioImage;
                    picMain_Q.Click += (sender_Aud, e_Aud) => {
                        Form bgBlur = new Form();
                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi","null",Globals.custUsername);
                        displayPic.Show();
                    };
                }

                if (_tableName == "file_info_gif") {

                    List<String> _base64Encoded = new List<string>();
                    string retrieveGifQuery = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(retrieveGifQuery, con);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.WaitOnLoad = false;
                    img.Image = new Bitmap(_toMs);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",Globals.custUsername)) {
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
                    };
                }

                if (_tableName == "file_info_apk") {
                    picMain_Q.Image = APKImage;
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk","null");
                        displayPic.Show();
                    };
                }

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = PDFImage;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",Globals.custUsername);
                        displayPdf.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_ptx") {
                    picMain_Q.Image = PTXImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",Globals.custUsername);
                        displayPtx.Show();
                    };
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = MSIImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text,"file_info_msi","null",Globals.custUsername);
                        displayMsi.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = DOCImage;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word","null",Globals.custUsername);
                        displayMsi.Show();
                    };
                }
            }

            if(flowLayoutPanel1.Controls.Count > 0) {
                clearRedundane();
            }

            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        /// <summary>
        /// 
        /// Generate user folders files on folder selection 
        /// 
        /// </summary>
        /// <param name="_fileType">File type of the files</param>
        /// <param name="_foldTitle">Folder title</param>
        /// <param name="parameterName">Custom parameter name for panel</param>
        /// <param name="currItem"></param>
        private async Task _generateUserFold(List<String> _fileType, String _foldTitle, int currItem) {

            btnGarbageImage.Visible = false;
            lblEmptyHere.Visible = false;

            var uploadAlertFormSucceeded = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
            uploadAlertFormSucceeded?.Close();

            var _setupRetrievalAlertThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your folder files.", "Loader").ShowDialog());
            _setupRetrievalAlertThread.Start();

            flowLayoutPanel1.Controls.Clear();

            try {

                List<String> typeValues = new List<String>(_fileType);

                List<(string, string)> filesInfo = new List<(string, string)>();
                HashSet<string> filePaths = new HashSet<string>();

                const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldname";
                using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@foldname", EncryptionModel.Encrypt(_foldTitle));
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {

                            string filePath = EncryptionModel.Decrypt(reader.GetString(0));
                            string uploadDate = reader.GetString(1);

                            if (filePaths.Contains(filePath)) {
                                continue;
                            }

                            filePaths.Add(filePath);
                            filesInfo.Add((filePath, uploadDate));
                        }
                    }
                }

                for (int i = 0; i < currItem; i++) {

                    var panelPic_Q = new Guna2Panel() {
                        Name = $"panelf{i}",
                        Width = 200,
                        Height = 222,
                        BorderColor = BorderColor,
                        BorderThickness = 1,
                        BorderRadius = 8,
                        BackColor = TransparentColor,
                        Location = new Point(600, top)
                    };
                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelPic_Q);

                    var panelF = (Guna2Panel)panelPic_Q;

                    Label dateLab = new Label();
                    panelF.Controls.Add(dateLab);
                    dateLab.Name = $"datef{i}";
                    dateLab.Font = DateLabelFont;
                    dateLab.ForeColor = DarkGrayColor;
                    dateLab.Visible = true;
                    dateLab.Enabled = true;
                    dateLab.Location = DateLabelLoc;
                    dateLab.Text = filesInfo[i].Item2;

                    Label titleLab = new Label();
                    panelF.Controls.Add(titleLab);
                    titleLab.Name = $"titleImgL{i}";
                    titleLab.Font = TitleLabelFont;
                    titleLab.ForeColor = GainsboroColor;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = TitleLabelLoc;
                    titleLab.Width = 160;
                    titleLab.Height = 20;
                    titleLab.AutoEllipsis = true;
                    titleLab.Text = filesInfo[i].Item1;

                    Guna2PictureBox picMain_Q = new Guna2PictureBox();
                    panelF.Controls.Add(picMain_Q);
                    picMain_Q.Name = "imgf" + i;
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
                    remBut.Name = $"remf{i}";
                    remBut.Width = 29;
                    remBut.Height = 26;
                    remBut.ImageOffset = GarbageOffset;
                    remBut.FillColor = TransparentColor;
                    remBut.BorderRadius = 6;
                    remBut.BorderThickness = 1;
                    remBut.BorderColor = TransparentColor;
                    remBut.Image = GarbageImage;
                    remBut.Visible = true;
                    remBut.Location = GarbageButtonLoc;

                    remBut.Click += (sender_im, e_im) => {

                        lblFileNameOnPanel.Text = titleLab.Text;
                        lblFileTableName.Text = "folder_upload_info";
                        lblSelectedDirName.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                        lblFilePanelName.Text = panelPic_Q.Name;
                        pnlFileOptions.Visible = true;

                    };

                    var img = ((Guna2PictureBox)panelF.Controls["imgf" + i]);

                    if (Globals.imageTypes.Contains(typeValues[i])) {

                        List<string> base64Encoded = new List<string>();

                        const string retrieveImgQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@foldtitle", EncryptionModel.Encrypt(_foldTitle));

                            string cacheKey = $"folder_{_foldTitle}_images";
                            MemoryCache cache = MemoryCache.Default;
                            if (cache.Contains(cacheKey)) {

                                base64Encoded = (List<string>)cache.Get(cacheKey);

                            } else {

                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        string encryptedString = readBase64.GetString(0);
                                        string decryptedString = EncryptionModel.Decrypt(encryptedString);
                                        base64Encoded.Add(decryptedString);
                                    }
                                }
                                CacheItemPolicy cachePolicy = new CacheItemPolicy {
                                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                                };

                                cache.Add(cacheKey, base64Encoded, cachePolicy);
                            }
                        }

                        if (base64Encoded.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                Image newImage = new Bitmap(toMs);
                                img.Image = newImage;
                            }
                        }



                        picMain_Q.Click += (sender, e) => {
                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"folder_upload_info","null",Globals.custUsername)) {
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

                        };
                        clearRedundane();
                    }


                    if (Globals.textTypes.Contains(typeValues[i])) {

                        var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                        if (typeValues[i] == ".py") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                        }
                        else if (typeValues[i] == ".txt" || typeValues[i] == ".md") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                        }
                        else if (_extTypes == ".html") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                        }
                        else if (_extTypes == ".css") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                        }
                        else if (_extTypes == ".js") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (_extTypes == ".csv") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        picMain_Q.Click += (sender_t, e_t) => {

                            if (_extTypes == ".csv" || _extTypes == ".sql") {
                                Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                                _showRetrievalCsvAlert.Start();
                            }

                            txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text,"null",Globals.custUsername);
                            displayPic.Show();
                        };
                        clearRedundane();
                    }

                    if(Globals.videoTypes.Contains(typeValues[i])) {

                        List<string> base64Encoded = new List<string>();

                        const string retrieveImgQuery = "SELECT CUST_THUMB FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(_foldTitle));
                            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text));

                            string cacheKey = $"folder_{_foldTitle}_thumb";
                            MemoryCache cache = MemoryCache.Default;
                            if (cache.Contains(cacheKey)) {
                                base64Encoded = (List<string>)cache.Get(cacheKey);
                            } else {
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        base64Encoded.Add(readBase64.GetString(0));
                                    }
                                }
                                CacheItemPolicy cachePolicy = new CacheItemPolicy {
                                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                                };
                                cache.Add(cacheKey, base64Encoded, cachePolicy);
                            }
                        }

                        byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = new Bitmap(toMs);
                        }


                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImg,getWidth,getHeight,titleLab.Text, "folder_upload_info", _foldTitle,Globals.custUsername);
                            displayVid.Show();
                        };
                    }

                    if (typeValues[i] == ".gif") {

                        List<String> _base64Encoded = new List<string>();

                        const string retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        using (MySqlCommand command = new MySqlCommand(retrieveImg, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(_foldTitle));
                            command.Parameters.AddWithValue("@filename", titleLab.Text);

                            using (MySqlDataReader _readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await _readBase64.ReadAsync()) {
                                    _base64Encoded.Add(_readBase64.GetString(0));
                                }
                            }
                        }

                        var _getBytes = Convert.FromBase64String(EncryptionModel.Decrypt(_base64Encoded[0]));
                        MemoryStream _toMs = new MemoryStream(_getBytes);

                        img.Image = new Bitmap(_toMs);
                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            using(gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _foldTitle, Globals.custUsername)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                                bgBlur.FormBorderStyle = FormBorderStyle.None;
                                bgBlur.Opacity = .24d;
                                bgBlur.BackColor = Color.Black;
                                bgBlur.WindowState = FormWindowState.Maximized;
                                bgBlur.Name = "bgBlurForm";
                                bgBlur.TopMost = true;
                                bgBlur.Location = this.Location;
                                bgBlur.StartPosition = FormStartPosition.Manual;
                                bgBlur.ShowInTaskbar = false;
                                bgBlur.Show();

                                displayVid.Owner = bgBlur;
                                displayVid.ShowDialog();

                                bgBlur.Dispose();
                            }
                        };
                    }

                    if (typeValues[i] == ".xlsx" || typeValues[i] == ".xls") {
                        img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                        img.Click += (sender_aud, e_aud) => {
                            Form bgBlur = new Form();
                            exlFORM displayExl = new exlFORM(titleLab.Text, "folder_upload_info", _foldTitle, Globals.custUsername);
                            displayExl.Show();
                        };
                    }

                    if (typeValues[i] == ".wav" || typeValues[i] == ".mp3") {
                        var _getWidth = this.Width;
                        var _getHeight = this.Height;
                        img.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                        img.Click += (sender_aud, e_aud) => {
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _foldTitle,Globals.custUsername);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".apk") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk",_foldTitle);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".exe") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_exe_96;
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            exeFORM displayPic = new exeFORM(titleLab.Text, "file_info_exe", _foldTitle,"null");
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".pdf") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info",_foldTitle,Globals.custUsername);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".docx" || typeValues[i] == ".doc") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,Globals.custUsername);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".pptx" || typeValues[i] == ".ppt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,Globals.custUsername);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".msi") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            msiFORM displayMsi = new msiFORM(titleLab.Text, "folder_upload_info", _foldTitle,Globals.custUsername);
                            displayMsi.Show();
                        };
                    }

                    var loadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "LoadAlertFORM");
                    loadAlertForm?.Close();

                    var retrievalAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "RetrievalAlert");
                    retrievalAlertForm?.Close();

                    var uploadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
                    uploadAlertForm?.Close();
                }

            } catch (Exception) {
                // @ Ignore exception after the user cancelled
                // folder file retrieval
            }
        }

        private void clearRedundane() {
            btnGarbageImage.Visible = false;
            lblEmptyHere.Visible = false;
        }

        private void showRedundane() {
            btnGarbageImage.Visible = true;
            lblEmptyHere.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e) {
            buildGreetingLabel();
        }

        /// <summary>
        /// 
        /// Setup greeting label based on current time and translate 
        /// them into user default language (English on registration).
        /// 
        /// </summary>
        
        private void buildGreetingLabel() {

            var form = HomePage.instance;
            var lab1 = form.lblGreetingText;
            var lab5 = Globals.custUsername;

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = $"Good Night {Globals.custUsername}";

            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おはよう " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buen día " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bonjour " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Bom dia " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "早上好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Доброе утро " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemorgen " + Globals.custUsername + " :)";
                }

            }

            else if (hours >= 12 && hours <= 16) {
                if (CurrentLang == "US") {
                    greeting = "Good Afternoon, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Petang, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Tag, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "こんにちは " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas tardes " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bon après-midi " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa tarde " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "下午好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Добрый день " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemiddag " + Globals.custUsername + " :)";
                }
            }
            else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (CurrentLang == "US") {
                        greeting = "Good Late Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten späten Abend " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "buenas tardes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый день " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }

                }
                else {
                    if (CurrentLang == "US") {
                        greeting = "Good Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "Buenas terdes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый вечер " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (CurrentLang == "US") {
                    greeting = "Good Night, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Malam, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Nacth, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おやすみ " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas noches " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "bonne nuit " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa noite " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "晚安 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Спокойной ночи " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Welterusten " + Globals.custUsername + " :)";
                }

            }

            lab1.Text = greeting;
        }

        /// <summary>
        /// 
        /// Show create directory form
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button1_Click(object sender, EventArgs e) {
            CreateDirectoryForm create_dir = new CreateDirectoryForm();
            create_dir.Show();
        }


        /// <summary>
        /// 
        /// This function will opens a file dialog and 
        /// generate panel for selected file
        /// 
        /// </summary>

        // Add File  
        int curr = 0;
        int txtCurr = 0;
        int exeCurr = 0;
        int vidCurr = 0;
        int exlCurr = 0;
        int audCurr = 0;
        int gifCurr = 0;
        int apkCurr = 0;
        int pdfCurr = 0;
        int ptxCurr = 0;
        int msiCurr = 0;
        int docxCurr = 0;

        private int searchCurr = 0;
        private string searchPan = "";

        private async Task containThumbUpload(string selectedItems,string nameTable, string getNamePath, object keyValMain) {

            int getCurrentCount = int.Parse(lblItemCountText.Text);
            int getLimitedValue = int.Parse(lblLimitUploadText.Text);
            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) + 5;
            lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

            progressBarUsageStorage.Value = calculatePercentageUsage;

            try {

                using (var command = new MySqlCommand()) {

                    command.Connection = con;
                    command.CommandText = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB)";
                    command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getNamePath, "0123456789085746"));
                    command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                    command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                    command.Parameters.AddWithValue("@CUST_FILE", keyValMain);

                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                        var toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            base64EncodedThumbnailHome.Add(toBase64);
                            command.Parameters.AddWithValue("@CUST_THUMB", toBase64);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

            }
            catch (Exception) {
                /* TODO: User has cancelled the insert operation
                 * then ignore 'object reference was not set to an object'
                 * exception
                 */
            }

            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

        }
        private async Task startSending(string setValue,string nameTable) {

            int getCurrentCount = int.Parse(lblItemCountText.Text);
            int getLimitedValue = int.Parse(lblLimitUploadText.Text);
            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) + 5;
            lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

            progressBarUsageStorage.Value = calculatePercentageUsage;

            string insertQuery = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE)";

            try {

                using (var command = new MySqlCommand(insertQuery, con)) {
                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text).Value = EncryptionModel.Encrypt(getName);
                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text).Value = Globals.custUsername;
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255).Value = varDate;
                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob).Value = setValue;

                    await command.ExecuteNonQueryAsync();
                }

                Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

            }
            catch (Exception) {
                MessageBox.Show("Failed to upload this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void _mainFileGenerator(int AccountType_, String _AccountTypeStr_) {

            var open = new OpenFileDialog {
                Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp;.webp;|Video Files|*.mp4;*.webm;.mov;.wmv|Gif Files|*.gif|Text Files|*.txt;*.md|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv",
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

                        string selectedFileName = Path.GetFileName(selectedItems);
                        if (existingLabels.Contains(selectedFileName.ToLower().Trim())) {
                            continue;
                        }

                        _filValues.Add(Path.GetFileName(selectedItems));

                        get_ex = open.FileName;
                        getName = Path.GetFileName(selectedItems);
                        retrieved = Path.GetExtension(selectedItems); 
                        retrievedName = Path.GetFileNameWithoutExtension(open.FileName);
                        fileSizeInMB = 0;

                        async void createPanelMain(String nameTable, String panName, int itemCurr, String keyVal) {

                            searchPan = panName;
                            nameTableInsert = nameTable;

                            if (fileSizeInMB < 1500) {

                                var panelTxt = new Guna2Panel() {
                                    Name = panName + itemCurr,
                                    Width = 200,
                                    Height = 222,
                                    BorderColor = BorderColor,
                                    BorderThickness = 1,
                                    BorderRadius = 8,
                                    BackColor = TransparentColor,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowLayoutPanel1.Controls.Add(panelTxt);
                                var mainPanelTxt = (Guna2Panel)panelTxt;

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
                                dateLabTxt.Font = DateLabelFont;
                                dateLabTxt.ForeColor = DarkGrayColor;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = DateLabelLoc;
                                dateLabTxt.Text = varDate;

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabVidUp" + itemCurr;
                                titleLab.Font = TitleLabelFont;
                                titleLab.ForeColor = GainsboroColor;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = TitleLabelLoc;
                                titleLab.Width = 160;
                                titleLab.Height = 20;
                                titleLab.AutoEllipsis = true;
                                titleLab.Text = getName;

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxtBut" + itemCurr;
                                remButTxt.Width = 29;
                                remButTxt.Height = 26;
                                remButTxt.ImageOffset = GarbageOffset;
                                remButTxt.FillColor = TransparentColor;
                                remButTxt.BorderRadius = 6;
                                remButTxt.BorderThickness = 1;
                                remButTxt.BorderColor = TransparentColor;
                                remButTxt.Image = GarbageImage;
                                remButTxt.Visible = true;
                                remButTxt.Location = GarbageButtonLoc;
                                remButTxt.BringToFront();

                                remButTxt.Click += (sender_tx, e_tx) => {
                                    lblFileNameOnPanel.Text = titleLab.Text;
                                    lblFileTableName.Text = nameTable;
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

                                var _setupUploadAlertThread = new Thread(() => new UploadingAlert(getName, Globals.custUsername, "null", panName + itemCurr, "null", _fileSize: fileSizeInMB).ShowDialog());
                                _setupUploadAlertThread.Start();

                                if (nameTable == "file_info") {

                                    base64EncodedImageHome.Add(EncryptionModel.Decrypt(keyVal));
                                    await startSending(keyVal, nameTable);

                                    textboxPic.Image = new Bitmap(selectedItems);
                                    textboxPic.Click += (sender_f, e_f) => {
                                        var getImgName = (Guna2PictureBox)sender_f;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                                        Form bgBlur = new Form();
                                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "file_info", "null",Globals.custUsername)) {
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
                                    };

                                    clearRedundane();
                                }

                                if (nameTable == "file_info_expand") {
                                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                                    await startSending(keyVal,nameTable);
                                    
                                    if (_extTypes == ".py") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                                    }
                                    else if (_extTypes == ".txt" || _extTypes == ".md") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                                    }
                                    else if (_extTypes == ".html") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                                    }
                                    else if (_extTypes == ".css") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                                    }
                                    else if (_extTypes == ".js") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                                    }
                                    else if (_extTypes == ".sql") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                                    }
                                    else if (_extTypes == ".csv") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                                    }

                                    var filePath = getName;

                                    textboxPic.Click += (sender_t, e_t) => {

                                        if(_extTypes == ".csv" || _extTypes == ".sql") {
                                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                                            _showRetrievalCsvAlert.Start();
                                        }

                                        txtFORM txtFormShow = new txtFORM("IGNORETHIS", "file_info_expand", filePath,"null",Globals.custUsername);
                                        txtFormShow.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_exe") {

                                    await startSending(keyVal, nameTable);

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        Form bgBlur = new Form();
                                        exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",Globals.custUsername);
                                        displayExe.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_vid") {

                                    await containThumbUpload(selectedItems, nameTable, getName, keyVal);

                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        var getImgName = (Guna2PictureBox)sender_ex;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                                        vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "file_info_vid","null",Globals.custUsername);
                                        vidShow.Show();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_audi") {
                                   await startSending(keyVal, nameTable);

                                    var _getWidth = this.Width;
                                    var _getHeight = this.Height;
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null",Globals.custUsername);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_excel") {
                                    await startSending(keyVal, nameTable);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "file_info_excel", "null", Globals.custUsername);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_gif") {
                                    await containThumbUpload(selectedItems, nameTable, getName, keyVal);
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems); 
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",Globals.custUsername)) {
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
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_apk") {
                                    await startSending(keyVal, nameTable);
      
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk","null");
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_pdf") {
                                    await startSending(keyVal, nameTable);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",Globals.custUsername);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_ptx") {
                                    await startSending(keyVal, nameTable);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",Globals.custUsername);
                                        displayPtx.ShowDialog();                                       
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_msi") {

                                    await startSending(keyVal, nameTable);
                           
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        Form bgBlur = new Form();
                                        msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null",Globals.custUsername);
                                        displayMsi.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_word") {
                                    await startSending(keyVal, nameTable);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "file_info_word","null",Globals.custUsername);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                            } else {
                                MessageBox.Show("File is too large, max file size is 1.5GB.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            }
                        }

                        try {

                            Byte[] _toByte = File.ReadAllBytes(selectedItems);
                            fileSizeInMB = (_toByte.Length/1024)/1024;

                            if (Globals.imageTypes.Contains(retrieved)) {
                                curr++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                if (retrieved != ".ico") {
                                    String _tempToBase64 = Convert.ToBase64String(_toByte);
                                    String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
                                    createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                }
                                else {
                                    Image retrieveIcon = Image.FromFile(selectedItems);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {
                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                        dataIco = msIco.ToArray();
                                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(dataIco));
                                        String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
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
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_exe", "PanExe", exeCurr, encryptText);

                            }
                            else if (Globals.videoTypes.Contains(retrieved)) {
                                vidCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_vid", "PanVid", vidCurr, encryptText);
                            }

                            else if (retrieved == ".xlsx" || retrieved == ".xls") {
                                exlCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_excel","PanExl",exlCurr,encryptText);
                            }

                            else if (retrieved == ".mp3" || retrieved == ".wav") {
                                audCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_audi", "PanAud", audCurr, encryptText); 
                                
                            }

                            else if (retrieved == ".gif") {
                                gifCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_gif", "PanGif", gifCurr, encryptText);
                            }

                            else if (retrieved == ".apk") {
                                apkCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_apk", "PanApk", apkCurr, encryptText);
                            }

                            else if (retrieved == ".pdf") {
                                pdfCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_pdf", "PanPdf", pdfCurr, encryptText);
                            }

                            else if (retrieved == ".pptx" || retrieved == ".ppt") {
                                ptxCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_ptx", "PanPtx", ptxCurr, encryptText);
                            }
                            else if (retrieved == ".msi") {
                                msiCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_msi", "PanMsi", msiCurr, encryptText);
                            }
                            else if (retrieved == ".docx") {
                                docxCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_word", "PanDoc", docxCurr, encryptText);

                            } else {

                                UnknownTypeAlert unsupportedFileFormartForm = new UnknownTypeAlert(getName);
                                unsupportedFileFormartForm.Show();
                            }

                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "UploadAlrt"))
                            .ToList()
                            .ForEach(form => form.Close());
                           
                        } catch (Exception) {
                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "UploadAlrt"))
                            .ToList()
                            .ForEach(form => form.Close());
                        }

                        searchCurr = curr;

                        lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                }
            }
            Application.OpenForms
             .OfType<Form>()
             .Where(form => String.Equals(form.Name, "UploadAlrt"))
             .ToList()
             .ForEach(form => form.Close());
        }
  
        /// <summary>
        /// This function will shows alert form that tells the user to upgrade 
        /// their account when the total amount of files upload is exceeding
        /// the amount of file they can upload 
        /// </summary>
        /// <param name="CurAcc"></param>
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

        /// <summary>
        /// Select user account type and show file dialog to upload file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                string _currentFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                if (_currentFolder == "Home") {

                    int CurrentUploadCount = Convert.ToInt32(lblItemCountText.Text);

                    if (Globals.accountType == "Basic") {
                        if (CurrentUploadCount != 20) {
                            _mainFileGenerator(20, Globals.accountType);
                        }
                        else {
                            DisplayError(Globals.accountType);
                        }
                    }

                    if (Globals.accountType == "Max") {
                        if (CurrentUploadCount != 500) {
                            _mainFileGenerator(500, Globals.accountType);
                        }
                        else {
                            DisplayError(Globals.accountType);
                        }
                    }

                    if (Globals.accountType == "Express") {
                        if (CurrentUploadCount != 1000) {
                            _mainFileGenerator(1000, Globals.accountType);
                        }
                        else {
                            DisplayError(Globals.accountType);
                        }
                    }

                    if (Globals.accountType == "Supreme") {
                        if (CurrentUploadCount != 2000) {
                            _mainFileGenerator(2000, Globals.accountType);
                        }
                        else {
                            DisplayError(Globals.accountType);
                        }
                    }
                } else {
                    MessageBox.Show("You can only upload a file on Home folder.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
            catch (Exception) {
                Form bgBlur = new Form();
                using (WaitAlert displayWait = new WaitAlert()) {
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

                    displayWait.Owner = bgBlur;
                    displayWait.ShowDialog();

                    bgBlur.Dispose();
                }

            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e) {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// This button will show settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button5_Click(object sender, EventArgs e) {

            Task.Run(() => new SettingsLoadingAlert().ShowDialog());

            var remAccShow = new SettingsForm(Globals.custEmail);
            remAccShow.Show();

            var settingsAlertForms = Application.OpenForms
                .OfType<Form>()
                .Where(form => form.Name == "settingsAlert")
                .ToList();

            foreach (var form in settingsAlertForms) {
                form.Close();
            }
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e) {


        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button7_Click(object sender, EventArgs e) {

            MainSharingForm _ShowSharing = new MainSharingForm();
            _ShowSharing.Show();
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private async void folderDialog(String _getDirPath,String _getDirTitle,String[] _TitleValues) {

            string _selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

            int _IntCurr = 0;
            long fileSizeInMB = 0;

            var _setupUploadAlertThread = new Thread(() => new UploadingAlert(_getDirTitle, Globals.custUsername, "folder_upload_info", "PanExlFold" + _IntCurr, _getDirTitle, _fileSize: fileSizeInMB).ShowDialog());
            _setupUploadAlertThread.Start();

            foreach (var _Files in Directory.EnumerateFiles(_getDirPath, "*")) {

                async Task setupUpload(String _tempToBase64,String thumbnailValue = null) {

                    const string insertFoldQue = "INSERT INTO folder_upload_info(FOLDER_TITLE,CUST_USERNAME,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH,CUST_THUMB) VALUES (@FOLDER_TITLE,@CUST_USERNAME,@CUST_FILE,@FILE_TYPE,@UPLOAD_DATE,@CUST_FILE_PATH,@CUST_THUMB)";
                    using (var command = new MySqlCommand(insertFoldQue, con)) {
                        command.Parameters.AddWithValue("@FOLDER_TITLE", EncryptionModel.Encrypt(_getDirTitle));
                        command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                        command.Parameters.AddWithValue("@FILE_TYPE", Path.GetExtension(_Files));
                        command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(Path.GetFileName(_Files)));
                        command.Parameters.AddWithValue("@CUST_FILE", _tempToBase64);
                        command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);

                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (var form in Application.OpenForms.OfType<Form>().Where(form => form.Name == "UploadAlrt")) {
                        form.Close();
                    }

                }

                _IntCurr++;

                var panelVid = new Guna2Panel() {
                    Name = $"PanExlFold{_IntCurr}",
                    Width = 200, 
                    Height = 222, 
                    BorderColor = BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelVid);
                var mainPanelTxt = (Guna2Panel)panelVid;
                _controlName = "PanExlFold" + _IntCurr;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{_IntCurr}";
                titleLab.Font = TitleLabelFont; 
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
                titleLab.AutoEllipsis = true;
                titleLab.Width = 160; 
                titleLab.Height = 20; 
                titleLab.Text = _TitleValues[_IntCurr - 1]; 

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
                remButExl.ImageOffset = GarbageOffset;
                remButExl.BorderColor = TransparentColor;
                remButExl.FillColor = TransparentColor;
                remButExl.BorderRadius = 6;
                remButExl.BorderThickness = 1;
                remButExl.BorderColor = BorderColor2;
                remButExl.Image = GarbageImage;
                remButExl.Visible = true;
                remButExl.Location = GarbageButtonLoc;

                remButExl.Click += (sender_vid, e_vid) => {

                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFileTableName.Text = "folder_upload_info";
                    lblFilePanelName.Text = mainPanelTxt.Name;
                    lblSelectedDirName.Text = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                    pnlFileOptions.Visible = true;

                };

                Label dateLabExl = new Label();
                mainPanelTxt.Controls.Add(dateLabExl);
                dateLabExl.Name = $"LabExlUpFold{_IntCurr}";
                dateLabExl.Font = DateLabelFont;
                dateLabExl.ForeColor = DarkGrayColor; 
                dateLabExl.Visible = true;
                dateLabExl.Enabled = true;
                dateLabExl.Location = DateLabelLoc;
                dateLabExl.Text = varDate;

                lblEmptyHere.Visible = false;
                btnGarbageImage.Visible = false;

                _titleText = titleLab.Text;

                var _extTypes = Path.GetExtension(_Files);

                try {

                    Byte[] _getBytes = File.ReadAllBytes(_Files);
                    fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                    if (Globals.imageTypes.Contains(_extTypes)) {

                        var _imgContent = new Bitmap(_Files);

                        String _tobase64 = EncryptionModel.Encrypt(Convert.ToBase64String(_getBytes));
                        await setupUpload(_tobase64);

                        textboxExl.Image = _imgContent;
                        textboxExl.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", Globals.custUsername)) {
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

                        };
                    }

                    if (Globals.textTypes.Contains(_extTypes)) {

                        if (_extTypes == ".py") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                        }
                        else if (_extTypes == ".txt" || _extTypes == ".md") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                        }
                        else if (_extTypes == ".html") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                        }
                        else if (_extTypes == ".css") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                        }
                        else if (_extTypes == ".js") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (_extTypes == ".csv") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        String nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_Files)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                        String getEncoded = Convert.ToBase64String(getBytes);
                        String encryptEncoded = EncryptionModel.Encrypt(getEncoded);

                        await setupUpload(encryptEncoded);

                        textboxExl.Click += (sender_t, e_t) => {

                            if (_extTypes == ".csv" || _extTypes == ".sql") {
                                Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                                _showRetrievalCsvAlert.Start();
                            }

                            txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text, "null", Globals.custUsername);
                            displayPic.Show();
                        };

                    }

                    if (_extTypes == ".apk") {

                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);

                        textboxExl.Image = APKImage;
                        textboxExl.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk", "null");
                            displayPic.ShowDialog();
                        };
                    }
                    if (Globals.videoTypes.Contains(_extTypes)) {

                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        String toBase64BitmapThumbnail;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            toBase64BitmapThumbnail = toBase64;
                        }

                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(_getBytes));
                        await setupUpload(_tempToBase64, toBase64BitmapThumbnail);

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".gif") {
                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64;
                        }

                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(_getBytes), "0123456789085746");
                        command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                        command.ExecuteNonQuery();
                        command.Dispose();

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".pdf") {

                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);

                        textboxExl.Image = PDFImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".docx" || _extTypes == ".doc") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);

                        textboxExl.Image = DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayPic = new wordFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".xlsx" || _extTypes == ".xls") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);
                        
                        textboxExl.Image = DOCImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            exlFORM displayPic = new exlFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }


                    if (_extTypes == ".pptx" || _extTypes == ".ppt") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);
                        
                        textboxExl.Image = PTXImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            ptxFORM displayPic = new ptxFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".mp3" || _extTypes == ".wav") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64);
                        await setupUpload(encryptValues);

                        textboxExl.Image = AudioImage;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _selectedFolder, Globals.custUsername);
                            displayPic.ShowDialog();
                        };
                    }

                }
                catch (Exception) {
                    //MessageBox.Show("An error ocurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

            var uploadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
            uploadAlertForm?.Close();

            btnShowFolderPage.FillColor = Color.FromArgb(255, 71, 19, 191);
            btnGoHomePage.FillColor = Color.Transparent;
            panel1.SendToBack();
            panel3.BringToFront();
            label9.Visible = true;
            lstFoldersPage.Visible = true;

            clearRedundane();
            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
            
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
        /// 

        private String _titleText;
        private String _controlName;
        private void guna2Button12_Click(object sender, EventArgs e) {

            LimitedFolderAlert folderUploadFailed = new LimitedFolderAlert(Globals.accountType, "it looks like you've reached the max \r\namount of folder you can upload",true);

            List<string> foldersItems = lstFoldersPage.Items.Cast<string>().ToList();
            List<string> execludedStringsItem = new List<string> { "Home", "Shared To Me", "Shared Files" };
            int countTotalFolders = foldersItems.Count(item => !execludedStringsItem.Contains(item));

            if(Globals.uploadFolderLimit[Globals.accountType] == countTotalFolders) {
                folderUploadFailed.Show();
                return;
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {

                var _getDirPath = dialog.FileName;
                int _countFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.TopDirectoryOnly).Length;
                var _getDirTitle = new DirectoryInfo(_getDirPath).Name;

                if(!lstFoldersPage.Items.Contains(_getDirTitle)) {

                    string[] _TitleValues = Directory.GetFiles(_getDirPath, "*").Select(Path.GetFileName).ToArray();
                    int _numberOfFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.AllDirectories).Length;

                    if(_numberOfFiles <= Globals.uploadFileLimit[Globals.accountType]) {

                        flowLayoutPanel1.Controls.Clear();
                        lstFoldersPage.Items.Add(_getDirTitle);

                        folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                        var _dirPosition = lstFoldersPage.Items.IndexOf(_getDirTitle);

                        lstFoldersPage.SelectedIndex = _dirPosition;

                    } else {
                        DisplayErrorFolder(Globals.accountType);
                        lstFoldersPage.SelectedItem = "Home";
                    }

                } else {
                    MessageBox.Show("Folder already exists","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
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

        private async void buildHomeFiles() {

            foreach (string tableName in Globals.publicTables) {
                if (Globals.tableToFileType.ContainsKey(tableName)) {
                    string fileType = Globals.tableToFileType[tableName];
                    if (fileType != null) {
                        await _generateUserFiles(tableName, fileType, await crudClass.countRow(tableName));
                    }
                    else {
                        await _generateUserDirectory(await crudClass.countRow(tableName));
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

        }

        private async void buildSharedToMe() {

            if (!_TypeValues.Any()) {
                const string getFilesTypeQuery = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
                using (MySqlCommand command = new MySqlCommand(getFilesTypeQuery, ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            _TypeValues.Add(reader.GetString(0));
                        }
                    }
                }
                await generateUserShared(_TypeValues, "DirParMe", _TypeValues.Count);
            }
            else {
                await generateUserShared(_TypeValues, "DirParMe", _TypeValues.Count);
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

        }

        private async void buildSharedToOthers() {

            if (!_TypeValuesOthers.Any()) {
                const string getFilesTypeOthers = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
                using (var command = new MySqlCommand(getFilesTypeOthers, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    using (var readTypeOthers = await command.ExecuteReaderAsync()) {
                        while (await readTypeOthers.ReadAsync()) {
                            _TypeValuesOthers.Add(readTypeOthers.GetString(0));
                        }
                    }
                }
            }

           
            await generateUserSharedOthers(_TypeValuesOthers, "DirParOther", _TypeValuesOthers.Count);

            

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }
        }

        /// <summary>
        /// Select folder from listBox and start showing
        /// the files from selected folder
        /// </summary>

        public List<String> _TypeValuesOthers = new List<String>();
        public List<String> _TypeValues = new List<String>();

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

            pnlFileOptions.Visible = false;

            try {

                int _selectedIndex = lstFoldersPage.SelectedIndex;
                string _selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

                lblCurrentPageText.Text = _selectedFolder;
                lblCurrentPageText.Visible = true;
                btnDeleteFolder.Visible = true;

                if (_selectedFolder == "Home") {

                    btnDownloadFolder.Visible = false;
                    btnDeleteFolder.Visible = false;
                    btnRefreshSharedFiles.Visible = false;
                    btnOpenRenameFolderPage.Visible = false;
                    btnRefreshFiles.Visible = true;
                    flowLayoutPanel1.WrapContents = true;
                    flowLayoutPanel1.Controls.Clear();

                    buildHomeFiles();

                    lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

                }
                else if (_selectedFolder != "Home" && _selectedFolder != "Shared To Me" && _selectedFolder != "Shared Files") {

                    btnDeleteFolder.Visible = true;
                    btnOpenRenameFolderPage.Visible = true;
                    btnRefreshFiles.Visible = false;
                    guna2Panel4.Visible = false;
                    btnDownloadFolder.Visible = true;
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.WrapContents = true;

                    var typesValues = new List<string>();
                    const string getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                    using (var command = new MySqlCommand(getFileType, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(_selectedFolder));
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                typesValues.Add(reader.GetString(0));
                            }
                        }
                    }

                    var mainTypes = typesValues.Distinct().ToList();
                    var currMainLength = typesValues.Count;
                    await _generateUserFold(typesValues, _selectedFolder, currMainLength);

                    lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
                    if (flowLayoutPanel1.Controls.Count == 0) {
                        showRedundane();
                    }
                    else {
                        clearRedundane();
                    }

                } else if (_selectedIndex == 1) {


                    btnRefreshSharedFiles.Visible = true;
                    btnOpenRenameFolderPage.Visible = false;
                    btnRefreshFiles.Visible = false;
                    btnDeleteFolder.Visible = false;
                    btnDownloadFolder.Visible = false;
                    flowLayoutPanel1.Controls.Clear();

                    clearRedundane();

                    _callFilesInformationShared();

                    buildSharedToMe();

                    lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
                    

                }
                else if (_selectedIndex == 2) {

                    btnRefreshSharedFiles.Visible = true;
                    btnRefreshFiles.Visible = false;
                    btnDownloadFolder.Visible = false;

                    btnDeleteFolder.Visible = false;
                    btnOpenRenameFolderPage.Visible = false;
                    flowLayoutPanel1.Controls.Clear();

                    clearRedundane();

                    _callFilesInformationOthers();

                    buildSharedToOthers();

                    lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
                    

                }

            } catch (Exception) {

                flowLayoutPanel1.Controls.Clear();

                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                }
                else {
                    clearRedundane();
                }

                MessageBox.Show("Hmm... something is wrong. Restarting Flowstorage may fix the problem.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// This function will delete user folder if 
        /// Garbage (delete folder) button is clicked
        /// </summary>
        /// <param name="foldName"></param>
        private async void _removeFoldFunc(String foldName) {

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

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
            }
        }

        /// <summary>
        /// Retrieve username of file that has been shared to
        /// </summary>
        /// <returns></returns>
        private string uploaderName() {

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

        /// <summary>
        /// Retrieve username of user you shared file to
        /// </summary>
        /// <returns></returns>
        String getUploaderNameShared = "";
        private string sharedToName() {

            const string selectUploaderName = "SELECT CUST_TO FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(selectUploaderName, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(getUploaderNameShared, "0123456789085746"));
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        /// <summary>
        /// Generate files that has been shared to other
        /// </summary>
        /// <param name="_extTypes"></param>
        /// <param name="parameterName"></param>
        /// <param name="itemCurr"></param>
        /// 

        List<(string, string)> filesInfoSharedOthers = new List<(string, string)>();
        private async void _callFilesInformationOthers() {

            filesInfoSharedOthers.Clear();
            const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_FROM = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfoSharedOthers.Add((fileName, uploadDate));
                    }
                }
            }

        }


        private async Task generateUserSharedOthers(List<String> _extTypes, String parameterName, int itemCurr) {

            var form1 = HomePage.instance;

            List<String> typeValues = new List<String>(_extTypes);

            try {

                for (int q = 0; q < itemCurr; q++) {

                    var panelTxt = new Guna2Panel() {
                        Name = $"{parameterName}{q}",
                        Width = 200,
                        Height = 222,
                        BorderColor = BorderColor,
                        BorderThickness = 1,
                        BorderRadius = 8,
                        BackColor = TransparentColor,
                        Location = new Point(600, top)
                    };

                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelTxt);
                    var mainPanelTxt = (Guna2Panel)panelTxt;  
                    _controlName = $"{parameterName}{q}";

                    var textboxPic = new Guna2PictureBox();
                    mainPanelTxt.Controls.Add(textboxPic);
                    textboxPic.Name = $"TxtBox{q}";
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
                    dateLabTxt.Name = $"LabTxtUp{q}";
                    dateLabTxt.Font = DateLabelFont;
                    dateLabTxt.ForeColor = DarkGrayColor;
                    dateLabTxt.Visible = true;
                    dateLabTxt.Enabled = true;
                    dateLabTxt.Location = DateLabelLoc;
                    dateLabTxt.Text = filesInfoSharedOthers[q].Item2;

                    Label titleLab = new Label();
                    mainPanelTxt.Controls.Add(titleLab);
                    titleLab.Name = $"LabVidUp{q}";
                    titleLab.Font = TitleLabelFont;
                    titleLab.ForeColor = GainsboroColor;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = TitleLabelLoc;
                    titleLab.Width = 160;
                    titleLab.Height = 20;
                    titleLab.AutoEllipsis = true;
                    titleLab.Text = filesInfoSharedOthers[q].Item1;

                    getUploaderNameShared = titleLab.Text;
                    var setupSharedUsername = sharedToName();
                    var SharedToName = getUploaderNameShared;

                    Guna2Button remButTxt = new Guna2Button();
                    mainPanelTxt.Controls.Add(remButTxt);
                    remButTxt.Name = $"RemTxtBut{q}";
                    remButTxt.Width = 29;
                    remButTxt.Height = 26;
                    remButTxt.ImageOffset = GarbageOffset;
                    remButTxt.FillColor = TransparentColor;
                    remButTxt.BorderRadius = 6;
                    remButTxt.BorderThickness = 1;
                    remButTxt.BorderColor = TransparentColor;
                    remButTxt.Image = GarbageImage;
                    remButTxt.Visible = true;
                    remButTxt.Location = GarbageButtonLoc;
                    remButTxt.BringToFront();

                    remButTxt.Click += (sender_im, e_im) => {

                        lblFileNameOnPanel.Text = titleLab.Text;
                        lblFileTableName.Text = "cust_sharing";
                        lblSharedToName.Text = setupSharedUsername;
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

                    if (Globals.imageTypes.Contains(typeValues[q])) {

                        if(base64EncodedImageSharedOthers.Count == 0) {

                            const string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                            using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                                command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;

                                using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        base64EncodedImageSharedOthers.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                                    }
                                }
                            }

                            if (base64EncodedImageSharedOthers.Count > q) {
                                byte[] getBytes = Convert.FromBase64String(base64EncodedImageSharedOthers[q]);
                                using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                    textboxPic.Image = new Bitmap(toMs);
                                }
                            }

                        } else {

                            if (base64EncodedImageSharedOthers.Count > q) {
                                byte[] getBytes = Convert.FromBase64String(base64EncodedImageSharedOthers[q]);
                                using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                    textboxPic.Image = new Bitmap(toMs);
                                }
                            }

                        }

                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared "  + sharedToName(),true)) {
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
                        };
                    }

                    if (typeValues[q] == ".pptx" || typeValues[q] == ".pptx") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, sharedToName(),true);
                            displayPtx.Show();
                        };
                    }

                    if (typeValues[q] == ".pdf") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayPtx.Show();
                        };
                    }

                    if (typeValues[q] == ".apk") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            apkFORM displayApk = new apkFORM(titleLab.Text, "Shared To " + sharedToName(), "cust_sharing", lblGreetingText.Text,true);
                            displayApk.Show();
                        };
                    }

                    if (typeValues[q] == ".msi") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            msiFORM displayMsi = new msiFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayMsi.Show();
                        };
                    }

                    if (typeValues[q] == ".docx" || typeValues[q] == ".doc") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                        textboxPic.Click += (sender_im, e_im) => {
                            exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayXls.Show();
                        };
                    }

                    if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                        textboxPic.Click += (sender_im, e_im) => {
                            audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayAud.Show();
                        };
                    }

                    if (Globals.videoTypes.Contains(typeValues[q])) {

                        List<string> base64Encoded = new List<string>();

                        const string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text));

                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    base64Encoded.Add(readBase64.GetString(0));
                                }
                            }
                        }

                        if (base64Encoded.Count > 0) {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                textboxPic.Image = new Bitmap(toMs);
                            }
                        }

                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            vidFORM displayAud = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared " + sharedToName(),true);
                            displayAud.Show();
                        };
                    }

                    if (typeValues[q] == ".exe") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                        textboxPic.Click += (sender_im, e_im) => {
                            var getImgName = (Guna2PictureBox)sender_im;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared To " + sharedToName(),true);
                            displayExe.Show();
                        };
                    }

                    if (Globals.textTypes.Contains(typeValues[q])) {

                        if (typeValues[q] == ".py") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                        }
                        else if (typeValues[q] == ".txt" || typeValues[q] == ".md") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                        }
                        else if (typeValues[q] == ".html") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                        }
                        else if (typeValues[q] == ".css") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                        }
                        else if (typeValues[q] == ".js") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (typeValues[q] == ".sql") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (typeValues[q] == ".csv") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        textboxPic.Click += (sender_im, e_im) => {

                            txtFORM displayTxt = new txtFORM("", "cust_sharing", titleLab.Text, lblGreetingText.Text, "Shared " +  sharedToName());
                            displayTxt.Show();
                        };
                    }

                    if (typeValues[q] == ".gif") {

                        List<String> _base64Encoded = new List<string>();

                        const string retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", Globals.custUsername);

                        MySqlDataReader _readBase64 = command.ExecuteReader();
                        while (_readBase64.Read()) {
                            _base64Encoded.Add(_readBase64.GetString(0));
                        }
                        _readBase64.Close();

                        var _getBytes = Convert.FromBase64String(_base64Encoded[q]);
                        MemoryStream _toMs = new MemoryStream(_getBytes);

                        textboxPic.Image = new Bitmap(_toMs);
                        textboxPic.Click += (sender_im, e_im) => {
                            Form bgBlur = new Form();
                            using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, "Shared To " + sharedToName(),true)) {
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

                                displayGif.Owner = bgBlur;
                                displayGif.ShowDialog();

                                bgBlur.Dispose();
                            }
                        };
                    }

                    lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

                    if (flowLayoutPanel1.Controls.Count > 0) {
                        lblEmptyHere.Visible = false;
                        btnGarbageImage.Visible = false;
                    }
                } 

            } catch (Exception) {
                // TODO: Ignore;
            }
        }

        /// <summary>
        /// Generate panel for user shared file
        /// </summary>
        /// <param name="_extTypes"></param>
        /// <param name="parameterName"></param>
        /// <param name="itemCurr"></param>
        /// 

        List<(string, string)> filesInfoShared = new List<(string, string)>();
        private async void _callFilesInformationShared() {

            filesInfoShared.Clear();
            const string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfoShared.Add((fileName, uploadDate));
                    }
                }
            }

        }

        private async Task generateUserShared(List<String> _extTypes, String parameterName, int itemCurr) {

            var form1 = HomePage.instance;

            var UploaderUsername = uploaderName();

            List<String> typeValues = new List<String>(_extTypes);

            for (int q = 0; q < itemCurr; q++) {
                var panelTxt = new Guna2Panel() {
                    Name = $"parameterName{q}",
                    Width = 200,
                    Height = 222,
                    BorderColor = BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelTxt);
                var mainPanelTxt = (Guna2Panel)panelTxt; 
                _controlName = parameterName + q;

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = $"TxtBox{q}";
                textboxPic.BorderRadius = 8;
                textboxPic.Width = 190;
                textboxPic.Height = 145;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                textboxPic.Anchor = AnchorStyles.None;

                int textboxPic_x = (mainPanelTxt.Width - textboxPic.Width) / 2;

                textboxPic.Location = new Point(textboxPic_x, 10);

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"LabVidUp{q}";
                titleLab.Font = TitleLabelFont;
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
                titleLab.Width = 160;
                titleLab.Height = 20;
                titleLab.AutoEllipsis = true;
                titleLab.Text = filesInfoShared[q].Item1;

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = $"RemTxtBut{q}";
                remButTxt.Width = 29;
                remButTxt.Height = 26;
                remButTxt.ImageOffset = GarbageOffset;
                remButTxt.FillColor = TransparentColor;
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = TransparentColor;
                remButTxt.Image = GarbageImage;
                remButTxt.Visible = true;
                remButTxt.Location = GarbageButtonLoc;
                remButTxt.BringToFront();

                remButTxt.Click += (sender_im, e_im) => {

                    lblFileNameOnPanel.Text = titleLab.Text;
                    lblFileTableName.Text = "cust_sharing";
                    lblFilePanelName.Text = mainPanelTxt.Name;
                    pnlFileOptions.Visible = true;

                };

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = $"LabTxtUp{q}";
                dateLabTxt.Font = DateLabelFont;
                dateLabTxt.ForeColor = DarkGrayColor;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = DateLabelLoc;
                dateLabTxt.Text = filesInfoShared[q].Item2;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                if (Globals.imageTypes.Contains(typeValues[q])) {

                    List<string> base64Encoded = new List<string>();

                    const string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Decrypt(readBase64.GetString(0)));
                            }
                        }
                    }

                    if (base64Encoded.Count > q) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername,false)) {
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
                    };
                }

                if (typeValues[q] == ".pptx" || typeValues[q] == ".ppt") {

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername,false);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".pdf") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername,false);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".apk") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (apkFORM displayApk = new apkFORM(titleLab.Text, UploaderUsername, "cust_sharing", lblGreetingText.Text,true)) {
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

                            displayApk.Owner = bgBlur;
                            displayApk.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".docx" || typeValues[q] == ".doc") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername, false);
                        displayDoc.Show();
                    };
                }

                if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    textboxPic.Click += (sender_im, e_im) => {
                        exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername, false);
                        displayXls.Show();
                    };
                }

                if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername, false);
                        displayAud.Show();
                    };
                }

                if (Globals.videoTypes.Contains(typeValues[q])) {

                    List<string> base64Encoded = new List<string>();

                    const string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text));

                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > 0) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        vidFORM displayAud = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername,false);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".exe") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername);
                        displayExe.Show();
                    };
                }

                if (Globals.textTypes.Contains(typeValues[q])) {

                    if (typeValues[q] == ".py") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (typeValues[q] == ".txt" || typeValues[q] == ".md") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                    }
                    else if (typeValues[q] == ".html") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                    }
                    else if (typeValues[q] == ".css") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                    }
                    else if (typeValues[q] == ".js") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    }
                    else if (typeValues[q] == ".sql") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (typeValues[q] == ".csv") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        txtFORM displayTxt = new txtFORM("", "cust_sharing", titleLab.Text, lblGreetingText.Text, UploaderUsername, false);
                        displayTxt.Show();
                    };
                }

                if (typeValues[q] == ".gif") {

                    List<String> _base64Encoded = new List<string>();

                    const string retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(EncryptionModel.Decrypt(_base64Encoded[q]));
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    textboxPic.Image = new Bitmap(_toMs);
                    textboxPic.Click += (sender_im, e_im) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", lblGreetingText.Text, UploaderUsername, false)) {
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

                            displayGif.Owner = bgBlur;
                            displayGif.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                Application.OpenForms
                          .OfType<Form>()
                          .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                          .ToList()
                          .ForEach(form => form.Close());


                lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

                if (flowLayoutPanel1.Controls.Count > 0) {
                    lblEmptyHere.Visible = false;
                    btnGarbageImage.Visible = false;
                }
            }
        }

        /// <summary>
        /// 
        /// Mini Garbage button which should remove 
        /// user folder based on the currently selected item in 
        /// listBox
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button19_Click(object sender, EventArgs e) {

            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "RetrievalAlert"))
              .ToList()
              .ForEach(form => form.Close());

            String _currentFold = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
            _removeFoldFunc(_currentFold);
        }


        /// <summary>
        /// Generate user directory from Home folder
        /// </summary>
        /// <param name="userName">Username of user</param>
        /// <param name="customParameter">Custom parameter for panel</param>
        /// <param name="rowLength"></param>
        private async Task _generateUserDirectory(int rowLength) {

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
                    BorderColor = BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label directoryLab = new Label();
                panelF.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + i;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = DateLabelFont;
                directoryLab.ForeColor = DarkGrayColor;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Location = DateLabelLoc;
                directoryLab.Text = "Directory";

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = TitleLabelFont;
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
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
                remBut.ImageOffset = GarbageOffset;
                remBut.FillColor = TransparentColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = TransparentColor;
                remBut.Image = DirectoryGarbageImage;
                remBut.Visible = true;
                remBut.Location = GarbageButtonLoc;

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

                        if (flowLayoutPanel1.Controls.Count == 0) {
                            lblEmptyHere.Visible = true;
                            btnGarbageImage.Visible = true;
                        }

                        lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                picMain_Q.Image = FlowSERVER1.Properties.Resources.DirIcon;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader").ShowDialog());
                    ShowAlert.Start();

                    DirectoryForm displayDirectory = new DirectoryForm(titleLab.Text);
                    displayDirectory.Show();

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                };
            }
            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private void guna2Panel16_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void label10_Click(object sender, EventArgs e) {

        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button9_Click(object sender, EventArgs e) {

        }

        private void label17_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox1_TextChanged_1(object sender, EventArgs e) {
           
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e) {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void label16_Click(object sender, EventArgs e) {

        }

        private void guna2GradientPanel1_Paint_1(object sender, PaintEventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {

        }


        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e) {
            this.Invalidate();
            base.OnScroll(e);
        }

        private void label19_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint_1(object sender, PaintEventArgs e) {

        }

        private void backgroundWorker1_DoWork_2(object sender, DoWorkEventArgs e) {

        }

        private void backgroundWorker1_ProgressChanged_2(object sender, ProgressChangedEventArgs e) {
            
        }

        private void backgroundWorker1_RunWorkerCompleted_2(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {

        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e) {
            Process.Start("https://apps.microsoft.com/store/detail/flowstorage/9PKQW5LQLBT5");
        }

        private async void backgroundWorker1_DoWork_3(object sender, DoWorkEventArgs e) {
            await Task.Run(async () =>
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {tableName} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE)";
                    command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getName));
                    command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                    command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                    command.Parameters.AddWithValue("@CUST_FILE", keyValMain);
                    command.CommandTimeout = 15000;

                    await command.ExecuteNonQueryAsync();

                    var form = Application.OpenForms.OfType<Form>().FirstOrDefault(f => f.Name == "UploadAlrt");
                    form?.Close();
                    
                }
            });
        }

        private void backgroundWorker1_ProgressChanged_3(object sender, ProgressChangedEventArgs e) {
        }

        private void backgroundWorker1_RunWorkerCompleted_3(object sender, RunWorkerCompletedEventArgs e) {

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
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream)) {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private void pictureBox4_Click(object sender, EventArgs e) {

        }

        private void pictureBox6_Click(object sender, EventArgs e) {

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

            _callFilesInformationShared();

            await generateUserShared(typeValues, dirName, typeValues.Count);
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

            _callFilesInformationOthers();

            await generateUserSharedOthers(typeValuesOthers, dirName, typeValuesOthers.Count);
        }

        /// <summary>
        /// Refresh Shared To Me/Shared To Others panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void guna2Button4_Click(object sender, EventArgs e) {

            int selectedIndex = lstFoldersPage.SelectedIndex;

            _TypeValues.Clear();
            _TypeValuesOthers.Clear();
            flowLayoutPanel1.Controls.Clear();

            if (selectedIndex == 1) {
                await RefreshGenerateUserShared(_TypeValues, "DirParMe");
            }
            else if (selectedIndex == 2) {
                await RefreshGenerateUserSharedOthers(_TypeValuesOthers, "DirParOther");
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

        }

        private IEnumerable<Control> GetAllControls(Control parent) {
            var controls = parent.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControls(c)).Concat(controls);
        }

        private async Task refreshHomePanels() {

            btnDeleteFolder.Visible = false;
            btnRefreshSharedFiles.Visible = false;
            flowLayoutPanel1.Controls.Clear();

            foreach (string tableName in Globals.publicTables) {
                if (Globals.tableToFileType.ContainsKey(tableName)) {
                    string fileType = Globals.tableToFileType[tableName];
                    if (fileType != null) {
                        await _generateUserFiles(tableName, fileType, await crudClass.countRow(tableName));
                    }
                    else {
                        await _generateUserDirectory(await crudClass.countRow(tableName));
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private async Task RefreshFolder() {

            String _selectedFolder = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);

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
            await _generateUserFold(typesValues, _selectedFolder, currMainLength);
        }

        /// <summary>
        /// 
        /// Detect for text input and search
        /// file based on the input
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool _isSearched = false;
        private async void guna2TextBox5_TextChanged(object sender, EventArgs e) {

            string searchText = txtBoxSearchFile.Text.Trim().ToLower();

            string[] searchTerms = searchText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            List<Guna2Panel> disposedPanels = new List<Guna2Panel>();

            for (int i = flowLayoutPanel1.Controls.Count - 1; i >= 0; i--) {
                Control ctrl = flowLayoutPanel1.Controls[i];
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
                    }
                    else {
                        if (!disposedPanels.Contains(panel)) {
                            disposedPanels.Add(panel);
                        }
                        flowLayoutPanel1.Controls.RemoveAt(i);
                        panel.Dispose();
                    }
                }
            }


            if (string.IsNullOrEmpty(searchText)) {

                string _selectedFolderSearch = lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem);
                if (_selectedFolderSearch == "Home") {
                    await refreshHomePanels();
                } else if (_selectedFolderSearch == "Shared To Me") {
                    flowLayoutPanel1.Controls.Clear();
                    _TypeValues.Clear();
                    await RefreshGenerateUserShared(_TypeValues, "DirParMe");
                } else if (_selectedFolderSearch == "Shared Files") {
                    flowLayoutPanel1.Controls.Clear();
                    _TypeValuesOthers.Clear();
                    await RefreshGenerateUserSharedOthers(_TypeValuesOthers, "DirParOther");
                } else if (_selectedFolderSearch != "Shared Files" || _selectedFolderSearch != "Shared To Me" || _selectedFolderSearch != "Home") {
                    flowLayoutPanel1.Controls.Clear();
                    await RefreshFolder();
                }
            }

            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

            if (lblItemCountText.Text == "0") {
                showRedundane();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private async void guna2Button8_Click(object sender, EventArgs e) {

            btnDeleteFolder.Visible = false;
            btnRefreshSharedFiles.Visible = false;
            flowLayoutPanel1.Controls.Clear();

            foreach (string tableName in Globals.publicTables) {
                if (Globals.tableToFileType.ContainsKey(tableName)) {
                    string fileType = Globals.tableToFileType[tableName];
                    if (fileType != null) {
                        await _generateUserFiles(tableName, fileType, await crudClass.countRow(tableName));
                    }
                    else {
                        await _generateUserDirectory(await crudClass.countRow(tableName));
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }


            lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private void guna2Button9_Click_1(object sender, EventArgs e) {
            guna2Panel4.Visible = true;
            btnGoHomePage.FillColor = Color.FromArgb(255,71, 19, 191);
            btnShowFolderPage.FillColor = Color.Transparent;
            panel3.SendToBack();
            panel1.BringToFront();
            label9.Visible = false;
            lstFoldersPage.Visible = false;
            btnLogout.Visible = true;
            guna2VSeparator1.BringToFront();
        }

        private void guna2Button13_Click(object sender, EventArgs e) {
            guna2Panel4.Visible = false;
            btnShowFolderPage.FillColor = Color.FromArgb(255, 71, 19, 191);
            btnGoHomePage.FillColor = Color.Transparent;
            panel1.SendToBack();
            panel3.BringToFront();
            label9.Visible = true;
            lstFoldersPage.Visible = true;
            btnLogout.Visible = false;
            guna2VSeparator1.BringToFront();
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {
        }

        private void guna2Button14_Click(object sender, EventArgs e) {

            var remAccShow = new SettingsForm(_emailAddr: Globals.custEmail);
            remAccShow.Show();

            SettingsForm.instance.guna2TabControl1.SelectedTab = SettingsForm.instance.guna2TabControl1.TabPages["tabPage3"];
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e) {

        }

        private void guna2Button15_Click(object sender, EventArgs e) {

            try {

                DialogResult _confirmation = MessageBox.Show("Logout your account?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (_confirmation == DialogResult.Yes) {

                    panel1.SendToBack();

                    HomePage.instance.label2.Text = "Item Count";
                    HomePage.instance.lblUpload.Text = "Upload";
                    HomePage.instance.btnUploadFile.Text = "Upload File";
                    HomePage.instance.btnUploadFolder.Text = "Upload Folder";
                    HomePage.instance.btnCreateDirectory.Text = "Create Directory";
                    HomePage.instance.btnFileSharing.Text = "File Sharing";
                    HomePage.instance.btnFileSharing.Size = new Size(125, 47);
                    HomePage.instance.lblEssentials.Text = "Essentials";

                    String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                    String _getAuth = _getPath + "\\CUST_DATAS.txt";

                    if (File.Exists(_getAuth)) {
                        if (Directory.Exists(_getPath)) {
                            Directory.Delete(_getPath, true);
                        }
                    }

                    base64EncodedImageHome.Clear();
                    base64EncodedThumbnailHome.Clear();
                    base64EncodedImageSharedOthers.Clear();
                    HomePage.instance.lstFoldersPage.Items.Clear();

                    Hide();

                    SignUpForm signUpForm = new SignUpForm();
                    signUpForm.ShowDialog();

                }
            }
            catch (Exception) {
                MessageBox.Show("There's a problem while attempting to logout your account. Please try again.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void Form1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy; 
            }
        }

        private void Form1_DragOver(object sender, DragEventArgs e) {
            pnlDragAndDropUpload.Visible = true;
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragLeave(object sender, EventArgs e) {
            pnlDragAndDropUpload.Visible = false;
        }

        /// <summary>
        /// 
        /// Drag-drop upload feature is implemented here
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Form1_DragDrop(object sender, DragEventArgs e) {

            pnlDragAndDropUpload.Visible = false;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return; 

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length + flowLayoutPanel1.Controls.Count > Globals.uploadFileLimit[Globals.accountType]) {
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

                    using (UpgradeAccountAlert displayUpgrade = new UpgradeAccountAlert(Globals.accountType)) {
                        displayUpgrade.Owner = bgBlur;
                        displayUpgrade.ShowDialog();
                    }
                };
            }
            else {
                DragDropHandleFiles(files);
            }
        }

        private void DragDropHandleFiles(string[] files) {

            List<string> filePathList = new List<string>(files);

            varDate = DateTime.Now.ToString("dd/MM/yyyy");

            foreach (var selectedItems in filePathList) {

                void clearRedundane() {
                    lblEmptyHere.Visible = false;
                    btnGarbageImage.Visible = false;
                }
                    
                getName = Path.GetFileName(selectedItems);
                retrieved = Path.GetExtension(selectedItems);
                fileSizeInMB = 0;

                async Task containThumbUpload(string nameTable, string getNamePath, object keyValMain) {

                    int getCurrentCount = int.Parse(lblItemCountText.Text);
                    int getLimitedValue = int.Parse(lblLimitUploadText.Text);
                    int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) + 5;
                    lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                    progressBarUsageStorage.Value = calculatePercentageUsage;

                    try {

                        using (var command = new MySqlCommand()) {

                            command.Connection = con;
                            command.CommandText = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB)";
                            command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getNamePath, "0123456789085746"));
                            command.Parameters.AddWithValue("@CUST_USERNAME", Globals.custUsername);
                            command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                            command.Parameters.AddWithValue("@CUST_FILE", keyValMain);

                            using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                                var toBitMap = shellFile.Thumbnail.Bitmap;
                                using (var stream = new MemoryStream()) {
                                    toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    var toBase64 = Convert.ToBase64String(stream.ToArray());
                                    command.Parameters.AddWithValue("@CUST_THUMB", toBase64);
                                }
                            }

                            await command.ExecuteNonQueryAsync();
                        }

                    }
                    catch (Exception) {
                        /* TODO: User has cancelled the insert operation
                            * then ignore 'object reference was not set to an object'
                            * exception
                            */
                    }

                    Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                }

                async void createPanelMain(String nameTable, String panName, int itemCurr, String keyVal) {
                    searchPan = panName;
                    nameTableInsert = nameTable;

                    if (fileSizeInMB < 1500) {

                        async Task startSending(string setValue) {

                            int getCurrentCount = int.Parse(lblItemCountText.Text);
                            int getLimitedValue = int.Parse(lblLimitUploadText.Text);
                            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) + 5;
                            lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                            progressBarUsageStorage.Value = calculatePercentageUsage;

                            string insertQuery = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE)";

                            try {

                                using (var command = new MySqlCommand(insertQuery, con)) {
                                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text).Value = EncryptionModel.Encrypt(getName);
                                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text).Value = Globals.custUsername;
                                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255).Value = varDate;
                                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob).Value = setValue;

                                    await command.ExecuteNonQueryAsync();
                                }

                                Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                            }
                            catch (Exception) {
                                MessageBox.Show("Failed to upload this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }

                        var panelTxt = new Guna2Panel() {
                            Name = panName + itemCurr,
                            Width = 200,
                            Height = 222,
                            BorderColor = BorderColor,
                            BorderThickness = 1,
                            BorderRadius = 8,
                            BackColor = TransparentColor,
                            Location = new Point(600, top)
                        };

                        top += h_p;
                        flowLayoutPanel1.Controls.Add(panelTxt);
                        var mainPanelTxt = (Guna2Panel)panelTxt;

                        Label dateLabTxt = new Label();
                        mainPanelTxt.Controls.Add(dateLabTxt);
                        dateLabTxt.Name = "LabTxtUp" + itemCurr;
                        dateLabTxt.Font = DateLabelFont;
                        dateLabTxt.ForeColor = DarkGrayColor;
                        dateLabTxt.Visible = true;
                        dateLabTxt.Enabled = true;
                        dateLabTxt.Location = DateLabelLoc;
                        dateLabTxt.Text = varDate;

                        Label titleLab = new Label();
                        mainPanelTxt.Controls.Add(titleLab);
                        titleLab.Name = "LabVidUp" + itemCurr;
                        titleLab.Font = TitleLabelFont;
                        titleLab.ForeColor = GainsboroColor;
                        titleLab.Visible = true;
                        titleLab.Enabled = true;
                        titleLab.Location = TitleLabelLoc;
                        titleLab.AutoEllipsis = true;
                        titleLab.Width = 160;
                        titleLab.Height = 20;
                        titleLab.Text = getName;

                        var textboxPic = new Guna2PictureBox();
                        mainPanelTxt.Controls.Add(textboxPic);
                        textboxPic.Name = "TxtBox" + itemCurr;
                        textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                        textboxPic.BorderRadius = 8;
                        textboxPic.Width = 190;
                        textboxPic.Height = 145;
                        textboxPic.Visible = true;

                        textboxPic.Anchor = AnchorStyles.None;

                        int picMain_Q_x = (panelTxt.Width - textboxPic.Width) / 2;

                        textboxPic.Location = new Point(picMain_Q_x, 10);

                        Guna2Button remButTxt = new Guna2Button();
                        mainPanelTxt.Controls.Add(remButTxt);
                        remButTxt.Name = "RemTxtBut" + itemCurr;
                        remButTxt.Width = 29;
                        remButTxt.Height = 26;
                        remButTxt.ImageOffset = GarbageOffset;
                        remButTxt.FillColor = TransparentColor;
                        remButTxt.BorderRadius = 6;
                        remButTxt.BorderThickness = 1;
                        remButTxt.BorderColor = TransparentColor;
                        remButTxt.Image = GarbageImage;
                        remButTxt.Visible = true;
                        remButTxt.Location = GarbageButtonLoc;

                        remButTxt.Click += (sender_im, e_im) => {

                            lblFileNameOnPanel.Text = titleLab.Text;
                            lblFileTableName.Text = nameTable;
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

                        var _setupUploadAlertThread = new Thread(() => new UploadingAlert(getName, Globals.custUsername, "null", panName + itemCurr, "null", _fileSize: fileSizeInMB).ShowDialog());
                        _setupUploadAlertThread.Start();

                        if (nameTable == "file_info") {
                            await startSending(keyVal);

                            textboxPic.Image = new Bitmap(selectedItems);
                            textboxPic.Click += (sender_f, e_f) => {
                                var getImgName = (Guna2PictureBox)sender_f;
                                var getWidth = getImgName.Image.Width;
                                var getHeight = getImgName.Image.Height;
                                Bitmap defaultImage = new Bitmap(getImgName.Image);

                                Form bgBlur = new Form();
                                using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "file_info", "null", Globals.custUsername)) {
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
                            };

                            clearRedundane();
                        }

                        if (nameTable == "file_info_expand") {
                            var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                            await startSending(keyVal);

                            if (_extTypes == ".py") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                            }
                            else if (_extTypes == ".txt" || _extTypes == ".md") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                            }
                            else if (_extTypes == ".html") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                            }
                            else if (_extTypes == ".css") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                            }
                            else if (_extTypes == ".js") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                            }
                            else if (_extTypes == ".sql") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                            }
                            else if (_extTypes == ".csv") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                            }

                            var filePath = getName;

                            textboxPic.Click += (sender_t, e_t) => {

                                if (_extTypes == ".csv" || _extTypes == ".sql") {
                                    Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrievalAlert().ShowDialog());
                                    _showRetrievalCsvAlert.Start();
                                }

                                txtFORM txtFormShow = new txtFORM("IGNORETHIS", "file_info_expand", filePath, "null", Globals.custUsername);
                                txtFormShow.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_exe") {

                            keyValMain = keyVal;
                            tableName = "file_info_exe";
                            backgroundWorker1.RunWorkerAsync();

                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                            textboxPic.Click += (sender_ex, e_ex) => {
                                Form bgBlur = new Form();
                                exeFORM displayExe = new exeFORM(titleLab.Text, "file_info_exe", "null", Globals.custUsername);
                                displayExe.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_vid") {
                            await containThumbUpload(nameTable, getName, keyVal);
                            ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                            Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                            textboxPic.Image = toBitMap;

                            textboxPic.Click += (sender_ex, e_ex) => {
                                var getImgName = (Guna2PictureBox)sender_ex;
                                var getWidth = getImgName.Image.Width;
                                var getHeight = getImgName.Image.Height;
                                Bitmap defaultImg = new Bitmap(getImgName.Image);

                                vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "file_info_vid", "null", Globals.custUsername);
                                vidShow.Show();
                            };
                            clearRedundane();
                        }
                        if (nameTable == "file_info_audi") {
                            await startSending(keyVal);

                            var _getWidth = this.Width;
                            var _getHeight = this.Height;
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                            textboxPic.Click += (sender_ex, e_ex) => {
                                audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null", Globals.custUsername);
                                displayPic.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_excel") {
                            await startSending(keyVal);
                            textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                            textboxPic.Click += (sender_ex, e_ex) => {
                                exlFORM displayPic = new exlFORM(titleLab.Text, "file_info_excel", "null", Globals.custUsername);
                                displayPic.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_gif") {
                            await containThumbUpload(nameTable, getName, keyVal);
                            ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                            Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                            textboxPic.Image = toBitMap;

                            textboxPic.Click += (sender_gi, e_gi) => {
                                Form bgBlur = new Form();
                                using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif", "null", Globals.custUsername)) {
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
                            };
                            clearRedundane();
                        }
                        if (nameTable == "file_info_apk") {
                            keyValMain = keyVal;
                            tableName = "file_info_apk";
                            backgroundWorker1.RunWorkerAsync();

                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                            textboxPic.Click += (sender_gi, e_gi) => {
                                Form bgBlur = new Form();
                                apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk", "null");
                                displayPic.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_pdf") {
                            await startSending(keyVal);
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                            textboxPic.Click += (sender_pd, e_pd) => {
                                pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf", "null", Globals.custUsername);
                                displayPdf.ShowDialog();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_ptx") {
                            await startSending(keyVal);
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                            textboxPic.Click += (sender_ptx, e_ptx) => {
                                ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx", "null", Globals.custUsername);
                                displayPtx.ShowDialog();
                            };
                            clearRedundane();
                        }
                        if (nameTable == "file_info_msi") {
                            keyValMain = keyVal;
                            tableName = "file_info_msi";
                            backgroundWorker1.RunWorkerAsync();

                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                            textboxPic.Click += (sender_ptx, e_ptx) => {
                                Form bgBlur = new Form();
                                msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null", Globals.custUsername);
                                displayMsi.Show();
                            };
                            clearRedundane();
                        }

                        if (nameTable == "file_info_word") {
                            await startSending(keyVal);
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                            textboxPic.Click += (sender_ptx, e_ptx) => {
                                wordFORM displayWord = new wordFORM(titleLab.Text, "file_info_word", "null", Globals.custUsername);
                                displayWord.ShowDialog();
                            };
                            clearRedundane();
                        }

                    } else {
                        MessageBox.Show("File is too large, max file size is 1.5GB.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                try {

                    Byte[] _toByte = File.ReadAllBytes(selectedItems);
                    fileSizeInMB = (_toByte.Length / 1024) / 1024;

                    if (Globals.imageTypes.Contains(retrieved)) {
                        curr++;
                        var getImg = new Bitmap(selectedItems);
                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;

                        if (retrieved != ".ico") {
                            String _tempToBase64 = Convert.ToBase64String(_toByte);
                            String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
                            createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                        }
                        else {
                            Image retrieveIcon = Image.FromFile(selectedItems);
                            byte[] dataIco;
                            using (MemoryStream msIco = new MemoryStream()) {
                                retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                dataIco = msIco.ToArray();
                                String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(dataIco));
                                String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
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
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_exe", "PanExe", exeCurr, encryptText);

                    }
                    else if (Globals.videoTypes.Contains(retrieved)) {
                        vidCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_vid", "PanVid", vidCurr, encryptText);
                    }

                    else if (retrieved == ".xlsx" || retrieved == ".xls") {
                        exlCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_excel", "PanExl", exlCurr, encryptText);
                    }

                    else if (retrieved == ".mp3" || retrieved == ".wav") {
                        audCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_audi", "PanAud", audCurr, encryptText);

                    }

                    else if (retrieved == ".gif") {
                        gifCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_gif", "PanGif", gifCurr, encryptText);
                    }

                    else if (retrieved == ".apk") {
                        apkCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_apk", "PanApk", apkCurr, encryptText);
                    }

                    else if (retrieved == ".pdf") {
                        pdfCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_pdf", "PanPdf", pdfCurr, encryptText);
                    }

                    else if (retrieved == ".pptx" || retrieved == ".ppt") {
                        ptxCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_ptx", "PanPtx", ptxCurr, encryptText);
                    }
                    else if (retrieved == ".msi") {
                        msiCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_msi", "PanMsi", msiCurr, encryptText);
                    }
                    else if (retrieved == ".docx") {
                        docxCurr++;
                        var _toBase64 = Convert.ToBase64String(_toByte);
                        String encryptText = EncryptionModel.Encrypt(_toBase64);
                        createPanelMain("file_info_word", "PanDoc", docxCurr, encryptText);
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "UploadAlrt"))
                    .ToList()
                    .ForEach(form => form.Close());

                }
                catch (Exception) {
                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "UploadAlrt"))
                    .ToList()
                    .ForEach(form => form.Close());
                }

                searchCurr = curr;

                lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

            }
            
        }

        private void guna2Button3_Click_1(object sender, EventArgs e) {

            RenameFolderFileForm renameFolderForm = new RenameFolderFileForm(lstFoldersPage.GetItemText(lstFoldersPage.SelectedItem));
            renameFolderForm.Show();
        }

        private void guna2Panel17_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button17_Click_1(object sender, EventArgs e) {

        }

        bool filterTypePanelVisible = false;
        private void guna2Button16_Click(object sender, EventArgs e) {
            filterTypePanelVisible = !filterTypePanelVisible;
            guna2Panel1.Visible = filterTypePanelVisible;
        }

        private void guna2Button18_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".png,.jpeg,.jpg";
        }

        private void guna2Button17_Click_2(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".txt,.html,.md,.sql,.css,.js";
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
            txtBoxSearchFile.Text = ".csv";

        }

        private void guna2Button24_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
            txtBoxSearchFile.Text = ".pdf";
        }

        private void guna2Button25_Click(object sender, EventArgs e) {
            txtBoxSearchFile.Text = String.Empty;
        }

        private void guna2Panel2_Paint_1(object sender, PaintEventArgs e) {

        }

        private void label26_Click(object sender, EventArgs e) {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e) {
            ApiPageForm apiFORMShow = new ApiPageForm();
            apiFORMShow.Show();
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e) {
            MobilePageForm mobileFORMShow = new MobilePageForm();
            mobileFORMShow.Show();
        }

        private void guna2Button26_Click(object sender, EventArgs e) {

        }

        private void _openFolderDownloadDialog(string folderTitle, List<(string fileName, byte[] fileBytes)> files) {

            var retrievalAlertForm = Application.OpenForms
                .OfType<Form>()
                .FirstOrDefault(form => form.Name == "RetrievalAlert");

            retrievalAlertForm?.Close();

            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), EncryptionModel.Decrypt(folderTitle));
            Directory.CreateDirectory(folderPath);

            foreach (var (fileName, fileBytes) in files) {
                var filePath = Path.Combine(folderPath, $"{fileName}");
                File.WriteAllBytes(filePath, fileBytes);
            }

            Process.Start(folderPath);
        }

        private async Task _downloadUserFolder(string folderTitle) {

            var files = new List<(string fileName, byte[] fileBytes)>();

            using (var command = new MySqlCommand($"SELECT CUST_FILE_PATH, CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle", con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@foldtitle", folderTitle);
                using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        var fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        var base64Encoded = EncryptionModel.Decrypt(reader.GetString(1));
                        var fileBytes = Convert.FromBase64String(base64Encoded);
                        files.Add((fileName, fileBytes));
                    }
                }
            }

            _openFolderDownloadDialog(folderTitle, files);
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
                await _downloadUserFolder(folderTitleGet);
            }
            else {
                LimitedFolderAlert upgradeAccountFolderFORM = new LimitedFolderAlert(Globals.accountType, "Please upgrade your account \r\nplan to download folder.", false);
                upgradeAccountFolderFORM.Show();
            }
        }


        private void guna2Panel3_Paint_1(object sender, PaintEventArgs e) {

        }

        private void guna2Button28_Click(object sender, EventArgs e) {
            pnlFileOptions.Visible = false;
        }

        /// <summary>
        /// 
        /// Delete file, including folder, home, sharing, directory
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button26_Click_1(object sender, EventArgs e) {

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

                if (tableName != "cust_sharing" && tableName != "folder_upload_info" && tableName != "file_info_directory") {

                    string removeQuery = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.ExecuteNonQuery();
                    }

                } else if (tableName == "folder_upload_info") {

                    using (MySqlCommand command = new MySqlCommand("DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername", con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(dirName));
                        command.ExecuteNonQuery();
                    }

                } else if (tableName == "cust_sharing" && sharedToName != "sharedToName") {

                    const string removeQuery = "DELETE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                    using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                        cmd.Parameters.AddWithValue("@username", Globals.custUsername);
                        cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        cmd.Parameters.AddWithValue("@sharedname", sharedToName);

                        cmd.ExecuteNonQuery();
                    }

                } else if (tableName == "cust_sharing" && sharedToName == "sharedToName") {

                    const string removeQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    using (var command = new MySqlCommand(removeQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                        command.ExecuteNonQuery();
                    }
                }

                Control[] matches = this.Controls.Find(panelName, true);
                if (matches.Length > 0 && matches[0] is Panel) {
                    Panel myPanel = (Panel)matches[0];
                    myPanel.Dispose();
                }

                lblItemCountText.Text = flowLayoutPanel1.Controls.Count.ToString();

                if (flowLayoutPanel1.Controls.Count == 0) {
                    lblEmptyHere.Visible = true;
                    btnGarbageImage.Visible = true;
                }

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

            RenameFileForm renameFileFORM = new RenameFileForm(titleFile,tableName,panelName, dirName,sharedToName);
            renameFileFORM.Show();
        }

        private void guna2Button32_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string tableName = lblFileTableName.Text;
            string dirName = lblSelectedDirName.Text;

            if (tableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(titleFile, "folder_upload_info", dirName);
            }
            else if (tableName == "file_info_vid") {
                SaverModel.SaveSelectedFile(titleFile, "file_info_vid", dirName);
            }
            else if (tableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(titleFile, "cust_sharing", dirName, false);
            } else if (tableName != "cust_sharing" && tableName != "folder_upload_info" && tableName != "upload_info_directory") {
                SaverModel.SaveSelectedFile(titleFile, tableName, dirName);
            }

        }

        private void guna2Button29_Click(object sender, EventArgs e) {

            string titleFile = lblFileNameOnPanel.Text;
            string dirName = lblSelectedDirName.Text;

            string fileExtensions = titleFile.Substring(titleFile.Length-4);

            shareFileFORM sharingFileFORM = new shareFileFORM(titleFile,fileExtensions,false,Globals.custUsername,dirName);
            sharingFileFORM.Show();
        }

        private void label25_Click(object sender, EventArgs e) {

        }
    }
}