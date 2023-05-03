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
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;
using System.Text;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Caching;

namespace FlowSERVER1
{
    /// <summary>
    /// Directory form class
    /// </summary>
    public partial class Form3 : Form {

        public static Form3 instance;
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static string _extName { get; set; }

        private string get_ex { get; set; }
        private string getName { get; set; }
        private string retrieved { get; set; }
        private string retrievedName { get; set; }
        private string tableName { get; set; }
        private string varDate { get; set; }
        private long fileSizeInMB { get; set; }
        private object keyValMain { get; set; }

        /// <summary>
        /// Initialize panel data
        /// </summary>

        // Date label
        private const string DateLabelFontName = "Segoe UI Semibold";
        private const float DateLabelFontSize = 10f;
        private static readonly Font DateLabelFont = new Font(DateLabelFontName, DateLabelFontSize, FontStyle.Bold);

        // Title label
        private const string TitleLabelFontName = "Segoe UI Semibold";
        private const float TitleLabelFontSize = 12f;
        private static readonly Font TitleLabelFont = new Font(TitleLabelFontName, TitleLabelFontSize, FontStyle.Bold);

        // Panel
        private static readonly Color BorderColor = ColorTranslator.FromHtml("#212121");
        private static readonly Color DarkGrayColor = Color.DarkGray;
        private static readonly Color GainsboroColor = Color.Gainsboro;
        private static readonly Color TransparentColor = Color.Transparent;
        private static readonly Point TitleLabelLoc = new Point(12, 182);
        private static readonly Point DateLabelLoc = new Point(12, 208);

        // Garbage button
        private static readonly Color BorderColor2 = ColorTranslator.FromHtml("#232323");
        private static readonly Color FillColor = ColorTranslator.FromHtml("#4713BF");
        private static readonly Image GarbageImage = FlowSERVER1.Properties.Resources.icons8_garbage_66;
        private static readonly Point GarbageButtonLoc = new Point(189, 218);

        public Form3(String sendTitle_)
        {
            InitializeComponent();
            instance = this;
            this.Text = $"{sendTitle_} (Directory)";
            label1.Text = sendTitle_;

            var form1 = Form1.instance;

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
                { ".gif", ("gifFile", "file_info_gif") },
                { ".apk", ("apkFile", "file_info_apk") },
                { ".pdf", ("pdfFile", "file_info_pdf") },
                { ".pptx", ("ptxFile", "file_info_ptx") },
                { ".msi", ("msiFile", "file_info_msi") },
                { ".docx", ("docFile", "file_info_word") },
            };

            string username = Form1.instance.label5.Text;
            string dirname = label1.Text;

            foreach (string ext in fileExtensions.Keys) {
                int count = _countRow(ext);
                if (count > 0) {
                    _extName = ext;
                    string controlName = fileExtensions[ext].Item1;
                    string tableName = fileExtensions[ext].Item2;
                    _generateUserFiles(tableName, controlName, count);
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();    
            }

            int _countRow(string ext) {
                string query = "SELECT COUNT(CUST_USERNAME) FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                using (var command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(dirname,EncryptionKey.KeyValue));
                    command.Parameters.AddWithValue("@ext", ext);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                label8.Visible = true;
                guna2Button6.Visible = true;
            }
            else {
                label8.Visible = false;
                guna2Button6.Visible = false;
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
        private async void _generateUserFiles(String _tableName, String parameterName, int currItem) {

            List<(string, string)> filesInfo = new List<(string, string)>();
            string selectFileDataDir = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
            using (MySqlCommand command = new MySqlCommand(selectFileDataDir, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                command.Parameters.AddWithValue("@ext", _extName); 
                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0));
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate));
                    }
                }
            }

            for (int i = 0; i < currItem; i++) {
                int top = 275;
                int h_p = 100;

                var panelPic_Q = new Guna2Panel() {
                    Name = parameterName + i,
                    Width = 240,
                    Height = 262,
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
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfo[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = $"ImgG{i}";
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 8;
                picMain_Q.Width = 226;
                picMain_Q.Height = 165;
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
                remBut.Width = 39;
                remBut.Height = 35;
                remBut.FillColor = FillColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = BorderColor2;
                remBut.Image = GarbageImage;
                remBut.Visible = true;
                remBut.Location = GarbageButtonLoc;

                remBut.Click += (sender_im, e_im) => {

                    var titleFile = titleLab.Text;

                    DialogResult verifyDialog = MessageBox.Show($"Delete '{titleLab.Text}'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        using (var command = con.CreateCommand()) {
                            String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                            command.CommandText = noSafeUpdate;
                            command.ExecuteNonQuery();
                        }

                        using (var command = con.CreateCommand()) {
                            String removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                            command.CommandText = removeQuery;
                            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile, EncryptionKey.KeyValue));
                            command.ExecuteNonQuery();
                        }

                        panelPic_Q.Dispose();

                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }

                    }
                };

                guna2Button6.Visible = false;
                label8.Visible = false;
                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == "file_info") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                        command.Parameters.AddWithValue("@ext", _extName);

                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Decrypt(readBase64.GetString(0), "0123456789085746"));
                            }
                        }
                    }

                    string base64String = base64Encoded.ElementAtOrDefault(i);
                    if (base64String != null) {
                        byte[] getBytes = Convert.FromBase64String(base64String);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = new Bitmap(toMs);
                        }
                    }

                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text)) {
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
                    else if (_extTypes == ".txt") {
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
                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                            _showRetrievalCsvAlert.Start();
                        }

                        txtFORM displayPic = new txtFORM("", "upload_info_directory", titleLab.Text, label1.Text, Form1.instance.label5.Text);
                        displayPic.Show();
                    };
                }

                if (_tableName == "file_info_exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayExe.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    var base64Encoded = new List<string>();
                    string retrieveImgQuery = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext AND CUST_FILE_PATH = @filename";
                    using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                        command.Parameters.AddWithValue("@ext", _extName);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));
                        using (var readBase64 = await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    byte[] getBytes = Convert.FromBase64String(base64Encoded?.FirstOrDefault());
                    using (var toMs = new MemoryStream(getBytes)) {
                        img.Image = new Bitmap(toMs);
                    }
                
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        vidFormShow.Show();
                    };
                }

                if (_tableName == "file_info_excel") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        exlForm.Show();
                    };
                }
                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    picMain_Q.Click += (sender_Aud, e_Aud) => {
                        Form bgBlur = new Form();
                        audFORM displayPic = new audFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayPic.Show();
                    };
                }

                if (_tableName == "file_info_gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                    command.Parameters.AddWithValue("@ext",_extName);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.Image = new Bitmap(_toMs);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text)) {
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
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, Form1.instance.label5.Text, "upload_info_directory", label1.Text);
                        displayPic.Show();
                    };
                }

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayPdf.Show();
                    };
                }

                if (_tableName == "file_info_ptx") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayPtx.Show();
                    };
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayMsi.Show();
                    };
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayMsi.Show();
                    };
                }
            }
        }

        public void label3_Click(object sender, EventArgs e)
        {

        }
        
        private void Form3_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }

        private void panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

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
        int gifCurr = 0;
        int apkCurr = 0;
        int pdfCurr = 0;
        int ptxCurr = 0;
        int msiCurr = 0;
        int docxCurr = 0;
        private string _controlName;
        private async void _mainFileGenerator(int AccountType_, String _AccountTypeStr_) {

            var form1 = Form1.instance;

            void deletionMethod(string fileName) {
                string query = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";

                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName, EncryptionKey.KeyValue));
                    command.ExecuteNonQuery();
                }

                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }
            }

            var open = new OpenFileDialog {
                Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp;|Video Files|*.mp4;*.webm;.mov;.wmv|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv",
                Multiselect = true
            };

            varDate = DateTime.Now.ToString("dd/MM/yyyy");
        
            int curFilesCount = flowLayoutPanel1.Controls.Count;

            if (open.ShowDialog() == DialogResult.OK) {

                List<string> _filValues = open.FileNames.Select(Path.GetFileName).ToList();

                if (open.FileNames.Length + curFilesCount > AccountType_) {
                    Form bgBlur = new Form();
                    using (upgradeFORM displayUpgrade = new upgradeFORM(_AccountTypeStr_)) {
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

                    foreach (var selectedItems in open.FileNames) {

                        _filValues.Add(Path.GetFileName(selectedItems));

                        void clearRedundane() {
                            label8.Visible = false;
                            guna2Button6.Visible = false;
                        }

                        get_ex = open.FileName;
                        getName = Path.GetFileName(selectedItems);
                        retrieved = Path.GetExtension(selectedItems); 
                        retrievedName = Path.GetFileNameWithoutExtension(open.FileName);
                        fileSizeInMB = 0;

                        async Task containThumbUpload(object keyValMain) {
                            using (var command = new MySqlCommand()) {
                                command.Connection = con;
                                command.CommandText = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)"; ;
                                command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getName, EncryptionKey.KeyValue));
                                command.Parameters.AddWithValue("@CUST_USERNAME", form1.label5.Text);
                                command.Parameters.AddWithValue("@FILE_EXT", retrieved);
                                command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                                command.Parameters.AddWithValue("@DIR_NAME", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                                command.Parameters.AddWithValue("@CUST_FILE", keyValMain);

                                using (var shellFile = ShellFile.FromFilePath(selectedItems))
                                using (var stream = new MemoryStream()) {
                                    shellFile.Thumbnail.Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    string base64Thumb = Convert.ToBase64String(stream.ToArray());
                                    command.Parameters.AddWithValue("@CUST_THUMB", base64Thumb);
                                }

                                await command.ExecuteNonQueryAsync();
                            }

                            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                        }

                        async void createPanelMain(string nameTable, string panName, int itemCurr, string keyVal) {

                            if (fileSizeInMB < 8000) {

                                async Task startSending(string setValue) {

                                    string insertQuery = "INSERT INTO upload_info_directory (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, FILE_EXT, DIR_NAME) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB, @FILE_EXT, @DIR_NAME)";

                                    try {

                                        using (var command = new MySqlCommand(insertQuery, con)) {
                                            command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getName, EncryptionKey.KeyValue));
                                            command.Parameters.AddWithValue("@CUST_USERNAME", form1.label5.Text);
                                            command.Parameters.AddWithValue("@FILE_EXT", retrieved);
                                            command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                                            command.Parameters.AddWithValue("@DIR_NAME", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                                            command.Parameters.AddWithValue("@CUST_FILE", setValue);
                                            command.Parameters.AddWithValue("@CUST_THUMB", "null");

                                            await command.ExecuteNonQueryAsync();

                                            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                                        }

                                        Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                                    } catch (Exception) {
                                        MessageBox.Show("Hey there's an error uploading this file! :(","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                    }
                                }

                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = panName + itemCurr,
                                    Width = 240,
                                    Height = 262,
                                    BorderColor = BorderColor,
                                    BorderThickness = 1,
                                    BorderRadius = 8,
                                    BackColor = TransparentColor,
                                    Location = new Point(600, top)
                                };
                 
                                top += h_p;
                                flowLayoutPanel1.Controls.Add(panelTxt);
                                var mainPanelTxt = (Guna2Panel)panelTxt;
                                _controlName = panName + itemCurr;

                                var textboxPic = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxPic);
                                textboxPic.Name = "TxtBox" + itemCurr;
                                textboxPic.Width = 226;
                                textboxPic.Height = 165;
                                textboxPic.BorderRadius = 8;
                                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxPic.Enabled = true;
                                textboxPic.Visible = true;

                                textboxPic.Anchor = AnchorStyles.None;

                                int picMain_Q_x = (mainPanelTxt.Width - textboxPic.Width) / 2;

                                textboxPic.Location = new Point(picMain_Q_x, 10);

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabVidUp" + itemCurr;
                                titleLab.Font = TitleLabelFont;
                                titleLab.ForeColor = GainsboroColor;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = TitleLabelLoc;
                                titleLab.Width = 220;
                                titleLab.Height = 30;
                                titleLab.Text = getName;

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxtBut" + itemCurr;
                                remButTxt.Width = 39;
                                remButTxt.Height = 35;
                                remButTxt.FillColor = FillColor;
                                remButTxt.BorderRadius = 6;
                                remButTxt.BorderThickness = 1;
                                remButTxt.BorderColor = BorderColor2;
                                remButTxt.Image = GarbageImage;
                                remButTxt.Visible = true;
                                remButTxt.Location = GarbageButtonLoc;
                                remButTxt.BringToFront();

                                textboxPic.MouseHover += (_senderM, _ev) => {
                                    panelTxt.ShadowDecoration.Enabled = true;
                                    panelTxt.ShadowDecoration.BorderRadius = 8;
                                };

                                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                                    panelTxt.ShadowDecoration.Enabled = false;
                                };

                                if (nameTable == "file_info") {
                                    await startSending(keyVal);
                                    textboxPic.Image = new Bitmap(selectedItems);
                                    textboxPic.Click += (sender_f, e_f) => {
                                        var getImgName = (Guna2PictureBox)sender_f;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                                        Form bgBlur = new Form();
                                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "upload_info_directory", label1.Text, form1.label5.Text)) {
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
                                    await startSending(keyVal);

                                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                                    if (_extTypes == ".py") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                                    }
                                    else if (_extTypes == ".txt") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                                    }
                                    else if (_extTypes == ".html") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                                    }
                                    else if (_extTypes == ".css") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
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
                                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                                            _showRetrievalCsvAlert.Start();
                                        }

                                        txtFORM txtFormShow = new txtFORM("", "upload_info_directory", titleLab.Text, label1.Text, form1.label5.Text);
                                        txtFormShow.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_exe") {
                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync(); 

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        Form bgBlur = new Form();
                                        exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayExe.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_vid") {
                                    await containThumbUpload(keyVal); // nameTable, getName, keyVal
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        var getImgName = (Guna2PictureBox)sender_ex;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                                        vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
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
                                        audFORM displayPic = new audFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_excel") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_gif") {
                                    await startSending(keyVal);
                                    textboxPic.Image = new Bitmap(selectedItems);

                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        gifFORM displayPic = new gifFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_apk") {
                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        apkFORM displayPic = new apkFORM(titleLab.Text, form1.label5.Text, "upload_info_directory", label1.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_pdf") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_ptx") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPtx.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_msi") {
                                    keyValMain = keyVal;
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        Form bgBlur = new Form();
                                        using (msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text)) {
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

                                            displayMsi.Owner = bgBlur;
                                            displayMsi.ShowDialog();

                                            bgBlur.Dispose();
                                        }
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_word") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                remButTxt.Click += (sender_tx, e_tx) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        deletionMethod(titleFile);
                                        panelTxt.Dispose();
                                    }

                                    if (flowLayoutPanel1.Controls.Count == 0) {
                                        label8.Visible = true;
                                        guna2Button6.Visible = true;
                                    }
                                };

                                Label dateLabTxt = new Label();
                                mainPanelTxt.Controls.Add(dateLabTxt);
                                dateLabTxt.Name = "LabTxtUp" + itemCurr;
                                dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                dateLabTxt.ForeColor = Color.DarkGray;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = new Point(12, 208);
                                dateLabTxt.Width = 1000;
                                dateLabTxt.Text = varDate;

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

                            if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico" || retrieved == ".bmp" || retrieved == ".svg") {
                                curr++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;
                                if (retrieved != ".ico") {
                                    String _tempToBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                    String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
                                    createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                }
                                else {
                                    Image retrieveIcon = Image.FromFile(selectedItems);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {
                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                        dataIco = msIco.ToArray();
                                        String _tempToBase64 = Convert.ToBase64String(dataIco);
                                        String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64);
                                        createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                    }
                                }
                            }
                            else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml" || retrieved == ".py" || retrieved == ".css" || retrieved == ".js" || retrieved == ".sql" || retrieved == ".csv") {
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
                            else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi" || retrieved == ".wmv") {
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
                            else if (retrieved == ".gif") {
                                gifCurr++;
                                var _toBase64 = Convert.ToBase64String(_getBytesSelectedFiles);
                                var _encryptValue = EncryptionModel.Encrypt(_toBase64);
                                createPanelMain("file_info_gif", "PanGif", gifCurr, _encryptValue);
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

                            Application.OpenForms.Cast<Form>().Where(f => f.Name == "UploadAlrt").ToList().ForEach(f => f.Close());

                        }

                        catch (Exception ) {
                            // IGNORE
                        }

                        Application.OpenForms.Cast<Form>().Where(f => f.Name == "UploadAlrt").ToList().ForEach(f => f.Close());

                    }
                }
            }
        }

        public void DisplayError(String CurAcc) {
            Form bgBlur = new Form();
            using (upgradeFORM displayPic = new upgradeFORM(CurAcc)) {
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
   
        private void guna2Button2_Click_1(object sender, EventArgs e) {

            try {

                int CurrentUploadCount = 0;
                String _accType = "";
                String getAccTypeQuery = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
                using (var command = new MySqlCommand(getAccTypeQuery, con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    List<String> types = new List<String>();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            types.Add(reader.GetString(0));
                        }
                    }
                    _accType = types.FirstOrDefault();
                    CurrentUploadCount = flowLayoutPanel1.Controls.Count;
                }
                

                if (_accType == "Basic") {
                    if (CurrentUploadCount != 20) {
                        _mainFileGenerator(20,"Basic");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Max") {
                    if (CurrentUploadCount != 500) {

                        _mainFileGenerator(500,"Max");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Express") {
                    if (CurrentUploadCount != 1000) {
                        _mainFileGenerator(1000, "Express");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Supreme") {
                    if (CurrentUploadCount != 2000) {
                        _mainFileGenerator(2000, "Supreme");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }
           } catch (Exception) {

                Application.OpenForms
                     .OfType<Form>()
                     .Where(form => String.Equals(form.Name, "UploadAlrt"))
                     .ToList()
                     .ForEach(form => form.Close());

                Form bgBlur = new Form();
                using (waitFORM displayWait = new waitFORM()) {
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

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }

        private void guna2Button12_Click(object sender, EventArgs   e) {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            String insertTxtQuery = "INSERT INTO " + "upload_info_directory" + "(CUST_FILE_PATH,CUST_USERNAME,UPLOAD_DATE,CUST_FILE,CUST_THUMB,FILE_EXT,DIR_NAME) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@UPLOAD_DATE,@CUST_FILE,@CUST_THUMB,@FILE_EXT,@DIR_NAME)";
            command = new MySqlCommand(insertTxtQuery, con);

            command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
            command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
            command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
            command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
            command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);
            command.Parameters.Add("@FILE_EXT", MySqlDbType.LongText);
            command.Parameters.Add("@DIR_NAME", MySqlDbType.Text);

            command.Parameters["@CUST_FILE_PATH"].Value = EncryptionModel.Encrypt(getName, EncryptionKey.KeyValue);
            command.Parameters["@CUST_USERNAME"].Value = Form1.instance.label5.Text;
            command.Parameters["@UPLOAD_DATE"].Value = varDate;

            command.Parameters["@FILE_EXT"].Value = retrieved;
            command.Parameters["@DIR_NAME"].Value = label1.Text;
            command.Parameters["@CUST_THUMB"].Value = "null";

            command.CommandTimeout = 15000;
            command.Prepare();

            if(command.ExecuteNonQuery() == 1) {
                Application.OpenForms
                .OfType<Form>()
                .Where(form => String.Equals(form.Name, "UploadAlrt"))
                .ToList()
                .ForEach(form => form.Close());
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }
    }
}