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
using System.Web;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace FlowSERVER1 {
    public partial class Form1 : Form {
        public static Form1 instance;
        public Label setupLabel;
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public bool stopFileTransaction = false;
        public Form1() {
            InitializeComponent();
            instance = this;

            // @ Enable double buffering for Form

            this.DoubleBuffered = true;
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // @ Load user data  

            try {

                con.Open();

                setupLabel = label5;

                String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                String _getAuth = _getPath + "\\CUST_DATAS.txt";
                if (File.Exists(_getAuth)) {
                    String _UsernameFirst = File.ReadLines(_getAuth).First();
                    String _PassSed = File.ReadLines(_getAuth).ElementAtOrDefault(1);
                    if (new FileInfo(_getAuth).Length != 0) {
                        Thread showLoadingForm = new Thread(() => new LoadAlertFORM().ShowDialog());
                        showLoadingForm.IsBackground = false;
                        showLoadingForm.Start();

                        guna2Panel7.Visible = false;
                        label5.Text = _PassSed;
                        label3.Text = _UsernameFirst;

                        setupTime();
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                        // @ ADD FOLDERS TO LISTBOX

                        listBox1.Items.Add("Home");
                        listBox1.SelectedIndex = 0;

                        List<String> titleValues = new List<String>();

                        String getTitles = "SELECT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username";
                        command = new MySqlCommand(getTitles, ConnectionModel.con);
                        command = ConnectionModel.con.CreateCommand();
                        command.CommandText = getTitles;

                        command.Parameters.AddWithValue("@username", label5.Text);
                        MySqlDataReader fold_Reader = command.ExecuteReader();
                        while (fold_Reader.Read()) {
                            titleValues.Add(fold_Reader.GetString(0));
                        }

                        fold_Reader.Close();

                        List<String> updatesTitle = titleValues.Distinct().ToList();
                        for (int iterateTitles = 0; iterateTitles < updatesTitle.Count; iterateTitles++) {
                            listBox1.Items.Add(updatesTitle[iterateTitles]);
                        }
                    }
                    Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                        .ToList()
                        .ForEach(form => form.Close());
                }
            }
         
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void _generateUserFiles(String _tableName, String parameterName, int currItem) {
            
            Application.DoEvents();

            for (int i = 0; i < currItem; i++) {
                int top = 275;
                int h_p = 100;

                flowLayoutPanel1.Location = new Point(13, 10);
                flowLayoutPanel1.Size = new Size(1118, 579);

                var panelPic_Q = new Guna2Panel() {
                    Name = parameterName + i,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = ((Guna2Panel)flowLayoutPanel1.Controls[parameterName + i]);

                List<string> dateValues = new List<string>();
                List<string> titleValues = new List<string>();

                String getUpDate = "SELECT UPLOAD_DATE FROM " + _tableName + " WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(getUpDate, con);
                command = con.CreateCommand();
                command.CommandText = getUpDate;

                command.Parameters.AddWithValue("@username", label5.Text);
                MySqlDataReader readerDate = command.ExecuteReader();

                while (readerDate.Read()) {
                    dateValues.Add(readerDate.GetString(0));
                }
                readerDate.Close();

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = "LabG" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                dateLab.ForeColor = Color.DarkGray;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = new Point(12, 208);
                dateLab.Text = dateValues[i];

                String getTitleQue = "SELECT CUST_FILE_PATH FROM " + _tableName + " WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(getTitleQue, con);
                command = con.CreateCommand();
                command.CommandText = getTitleQue;

                command.Parameters.AddWithValue("@username", label5.Text);

                MySqlDataReader titleReader = command.ExecuteReader();
                while (titleReader.Read()) {
                    titleValues.Add(titleReader.GetString(0));
                }
                titleReader.Close();

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = titleValues[i];

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 6;
                picMain_Q.Width = 241;
                picMain_Q.Height = 165;
                picMain_Q.Visible = true;

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
                remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remBut.Visible = true;
                remBut.Location = new Point(189, 218);

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true; 
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                guna2Button6.Visible = false;
                label8.Visible = false;
                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == "file_info") {

                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    
                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while(_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.Image = new Bitmap(_toMs);
                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"file_info","null",label5.Text)) {
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

                if (_tableName == "file_info_expand") {
                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    if (_extTypes == ".py") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                    }
                    else if (_extTypes == ".txt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                    }
                    else if (_extTypes == ".html") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                    }
                    else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                    }
                    else if (_extTypes == ".js") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    } else if (_extTypes == ".sql") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    picMain_Q.Click += (sender_t, e_t) => {
                        Form bgBlur = new Form();
                        using (txtFORM displayPic = new txtFORM("IGNORETHIS", "file_info_expand", titleLab.Text,"null",label5.Text)) {
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

                            displayPic.Owner = bgBlur;
                            displayPic.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",label5.Text);
                        displayExe.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_THUMB FROM  " + _tableName + " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.Image = new Bitmap(_toMs);

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid","null",label5.Text);
                        vidFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_excel") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text,"file_info_excel","null",label5.Text);
                        exlForm.Show();
                    };
                }
                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    picMain_Q.Click += (sender_Aud, e_Aud) => {
                        Form bgBlur = new Form();
                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi","null",label5.Text);
                        displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);

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
                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",label5.Text)) {
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

                if (_tableName == "file_info_apk") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk","null");
                        displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",label5.Text);
                        displayPdf.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_ptx") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",label5.Text);
                        displayPtx.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text,"file_info_msi","null");
                        displayMsi.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word","null",label5.Text);
                        displayMsi.Show();
                    };
                    clearRedundane();
                }
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();

            Application.DoEvents();
        }
        public void _generateUserFold(List<String> _fileType, String _foldTitle, String parameterName, int currItem) {

            try {

                flowLayoutPanel1.Controls.Clear();
                List<String> typeValues = new List<String>(_fileType);

                var _setupRetrievalAlertThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your folder files.","Loader").ShowDialog());
                _setupRetrievalAlertThread.Start();
                Application.DoEvents();

                for (int i = 0; i < currItem; i++) {
                    int top = 275;
                    int h_p = 100;
                    flowLayoutPanel1.Location = new Point(13, 10);
                    flowLayoutPanel1.Size = new Size(1118, 579);

                    var panelPic_Q = new Guna2Panel() {
                        Name = "panelf" + i,
                        Width = 240,
                        Height = 262,
                        BorderRadius = 8,
                        FillColor = ColorTranslator.FromHtml("#121212"),
                        BackColor = Color.Transparent,
                        Location = new Point(600, top)
                    };
                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelPic_Q);

                    var panelF = ((Guna2Panel)flowLayoutPanel1.Controls["panelf" + i]);

                    List<string> dateValues = new List<string>();
                    List<string> titleValues = new List<string>();

                    String getUpDate = "SELECT UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                    command = new MySqlCommand(getUpDate, con);
                    command = con.CreateCommand();
                    command.CommandText = getUpDate;

                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    MySqlDataReader readerDate = command.ExecuteReader();

                    while (readerDate.Read()) {
                        dateValues.Add(readerDate.GetString(0));
                    }
                    readerDate.Close();

                    Label dateLab = new Label();
                    panelF.Controls.Add(dateLab);
                    dateLab.Name = "datef" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                    dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                    dateLab.ForeColor = Color.DarkGray;
                    dateLab.Visible = true;
                    dateLab.Enabled = true;
                    dateLab.Location = new Point(12, 208);
                    dateLab.Text = dateValues[i];

                    String getTitleQue = "SELECT CUST_FILE_PATH FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                    command = new MySqlCommand(getTitleQue, con);
                    command = con.CreateCommand();
                    command.CommandText = getTitleQue;

                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);

                    MySqlDataReader titleReader = command.ExecuteReader();
                    while (titleReader.Read()) {
                        titleValues.Add(titleReader.GetString(0));
                    }
                    titleReader.Close();
                    Label titleLab = new Label();
                    panelF.Controls.Add(titleLab);
                    titleLab.Name = "titlef" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                    titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                    titleLab.ForeColor = Color.Gainsboro;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = new Point(12, 182);
                    titleLab.Width = 220;
                    titleLab.Height = 30;
                    titleLab.Text = titleValues[i];

                    Guna2PictureBox picMain_Q = new Guna2PictureBox();
                    panelF.Controls.Add(picMain_Q);
                    picMain_Q.Name = "imgf" + i;
                    picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                    picMain_Q.BorderRadius = 6;
                    picMain_Q.Width = 241;
                    picMain_Q.Height = 165;
                    picMain_Q.Visible = true;

                    picMain_Q.MouseHover += (_senderM, _ev) => {
                        panelF.ShadowDecoration.Enabled = true;
                        panelF.ShadowDecoration.BorderRadius = 8;
                    };

                    picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                        panelF.ShadowDecoration.Enabled = false;
                    };

                    Guna2Button remBut = new Guna2Button();
                    panelF.Controls.Add(remBut);
                    remBut.Name = "remf" + i;
                    remBut.Width = 39;
                    remBut.Height = 35;
                    remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                    remBut.BorderRadius = 6;
                    remBut.BorderThickness = 1;
                    remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                    remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                    remBut.Visible = true;
                    remBut.Location = new Point(189, 218);

                    remBut.Click += (sender_im, e_im) => {
                        var titleFile = titleLab.Text;
                        DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (verifyDialog == DialogResult.Yes) {
                            String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                            command = new MySqlCommand(noSafeUpdate, con);
                            command.ExecuteNonQuery();

                            String removeQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername";
                            command = new MySqlCommand(removeQuery, con);
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@filename", titleFile);
                            command.Parameters.AddWithValue("@foldername", _foldTitle);
                            command.ExecuteNonQuery();

                            panelPic_Q.Dispose();
                            if (flowLayoutPanel1.Controls.Count == 0) {
                                label8.Visible = true;
                                guna2Button6.Visible = true;
                            }
                            label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                        }
                    };

                    guna2Button6.Visible = false;
                    label8.Visible = false;

                    var img = ((Guna2PictureBox)panelF.Controls["imgf" + i]);
                    Application.DoEvents();

                    if (typeValues[i] == ".png" || typeValues[i] == ".jpg" || typeValues[i] == ".jpeg") {

                        List<String> _base64Encoded = new List<string>();
                        String retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);

                        MySqlDataReader _readBase64 = command.ExecuteReader();
                        while (_readBase64.Read()) {
                            _base64Encoded.Add(_readBase64.GetString(0));
                        }
                        _readBase64.Close();

                        var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                        MemoryStream _toMs = new MemoryStream(_getBytes);
                        img.Image = new Bitmap(_toMs);

                        picMain_Q.Click += (sender, e) => {
                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"folder_upload_info","null",label5.Text)) {
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


                    if (typeValues[i] == ".txt" || typeValues[i] == ".py" || typeValues[i] == ".html" || typeValues[i] == ".css" || typeValues[i] == ".js" || typeValues[i] == ".sql") {
                        String retrieveImg = "SELECT CONVERT(CUST_FILE USING utf8) FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.Parameters.AddWithValue("@filename", titleLab.Text);

                        List<String> textValues_ = new List<String>();

                        MySqlDataReader _ReadTexts = command.ExecuteReader();
                        while (_ReadTexts.Read()) {
                            textValues_.Add(_ReadTexts.GetString(0));
                        }
                        _ReadTexts.Close();
                        var getMainText = textValues_[0];

                        var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                        if (typeValues[i] == ".py") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                        }
                        else if (typeValues[i] == ".txt") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                        }
                        else if (_extTypes == ".html") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".css") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".js") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }

                        picMain_Q.Click += (sender_t, e_t) => {
                            Form bgBlur = new Form();
                            using (txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text,"null",label5.Text)) { // orignally: file_info_expand;
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

                                displayPic.Owner = bgBlur;
                                displayPic.ShowDialog();

                                bgBlur.Dispose();
                            }
                        };
                        clearRedundane();
                    }

                    if(typeValues[i] == ".mp4" || typeValues[i] == ".mov" || typeValues[i] == ".webm" || typeValues[i] == ".avi" || typeValues[i] == ".wmv") {
                        List<String> _base64Encoded = new List<string>();
                        String retrieveImg = "SELECT CUST_THUMB FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.Parameters.AddWithValue("@filename", titleLab.Text);

                        MySqlDataReader _readBase64 = command.ExecuteReader();
                        while(_readBase64.Read()) {
                            _base64Encoded.Add(_readBase64.GetString(0));
                        }
                        _readBase64.Close();
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        MemoryStream _toMs = new MemoryStream(_getBytes);
                        img.Image = new Bitmap(_toMs);

                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImg,getWidth,getHeight,titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayVid.Show();
                        };
                    }

                    if (typeValues[i] == ".gif") {
                        List<String> _base64Encoded = new List<string>();
                        String retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.Parameters.AddWithValue("@filename", titleLab.Text);

                        MySqlDataReader _readBase64 = command.ExecuteReader();
                        while (_readBase64.Read()) {
                            _base64Encoded.Add(_readBase64.GetString(0));
                        }
                        _readBase64.Close();

                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        MemoryStream _toMs = new MemoryStream(_getBytes);

                        img.Image = new Bitmap(_toMs);
                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            using(gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text)) {
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

                    if (typeValues[i] == ".xlsx" || typeValues[i] == ".xls" || typeValues[i] == ".csv") {
                        img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                        img.Click += (sender_aud, e_aud) => {
                            Form bgBlur = new Form();
                            exlFORM displayExl = new exlFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                            displayExl.Show();
                        };
                    }

                    if (typeValues[i] == ".wav" || typeValues[i] == ".mp3") {
                        //Application.DoEvents();
                        var _getWidth = this.Width;
                        var _getHeight = this.Height;
                        img.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                        img.Click += (sender_aud, e_aud) => {
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".apk") {
                        Application.DoEvents();
                        img.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk",_foldTitle);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".exe") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_exe_96;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            exeFORM displayPic = new exeFORM(titleLab.Text, "file_info_exe", _foldTitle,"null");
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".pdf") {
                    //    Application.DoEvents();
                        img.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info",_foldTitle,label5.Text);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".docx" || typeValues[i] == ".doc") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".pptx" || typeValues[i] == ".ppt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".msi") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            msiFORM displayMsi = new msiFORM(titleLab.Text, "folder_upload_info", _foldTitle);
                            displayMsi.Show();
                        };
                    }
                    Application.DoEvents();
                }
                Application.OpenForms
                  .OfType<Form>()
                  .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                  .ToList()
                  .ForEach(form => form.Close());

            } catch (Exception) {
                // @ Ignore exception after the user cancelled
                // folder file retrieval
            }
        }

        public void clearRedundane() {
            guna2Button6.Visible = false;
            label8.Visible = false;
        }

        public void showRedundane() {
            guna2Button6.Visible = true;
            label8.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e) {
            setupTime();
        }
        public void setupTime() {
            try {
                DateTime now = DateTime.Now;
                var hours = now.Hour;
                String greeting = null;
                if (hours >= 1 && hours <= 12) {
                    greeting = "Good Morning " + label5.Text + " :) ";
                    pictureBox2.Visible = true;
                    pictureBox1.Visible = false;
                    pictureBox3.Visible = false;
                }
                else if (hours >= 12 && hours <= 16) {
                    greeting = "Good Afternoon " + label5.Text + " :)";
                    pictureBox2.Visible = true;
                    pictureBox1.Visible = false;
                    pictureBox3.Visible = false;
                }
                else if (hours >= 16 && hours <= 21) {
                    if(hours == 20 || hours == 21) {
                        greeting = "Good Late Evening " + label5.Text + " :)";
                    } else {
                        greeting = "Good Evening " + label5.Text + " :)";
                    }
                    pictureBox3.Visible = true;
                    pictureBox2.Visible = false;
                    pictureBox1.Visible = false;
                }
                else if (hours >= 21 && hours <= 24) {
                    greeting = "Good Night " + label5.Text + " :)";
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = false;
                    pictureBox3.Visible = false;
                }
                label1.Text = greeting;
            }
            catch (Exception) {
                MessageBox.Show("Oh no! unable to retrieve the time :(( sooo sadd :CCCC");
            }
        }

        // Dir
        private void guna2Button1_Click(object sender, EventArgs e) {
            Form4 create_dir = new Form4();
            create_dir.Show();
        }

        private int getUserDirCount() {
            String _typeAcc = "";
            int _intAllowed = 0;
            List<String> _emailValues = new List<string>();
            List<String> _typeValues = new List<string>();

            String _queSelectEm = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_queSelectEm, con);
            command.Parameters.AddWithValue("@username", label5.Text);
            MySqlDataReader _readEmail = command.ExecuteReader();
            if (_readEmail.Read()) {
                _emailValues.Add(_readEmail.GetString(0));
            }
            _readEmail.Close();

            String _queSelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_EMAIL = @email";
            command = new MySqlCommand(_queSelectType, con);
            command.Parameters.AddWithValue("@email", _emailValues[0]);
            MySqlDataReader _ReadType = command.ExecuteReader();
            if (_ReadType.Read()) {
                _typeValues.Add(_ReadType.GetString(0));
            }
            _ReadType.Close();
            _typeAcc = _typeValues[0];

            if (_typeAcc == "Basic") {
                _intAllowed = 12;
            }
            else if (_typeAcc == "Max") {
                _intAllowed = 30;
            }
            else if (_typeAcc == "Express") {
                _intAllowed = 85;
            }
            else if (_typeAcc == "Supreme") {
                _intAllowed = 170;
            }
            return _intAllowed;
        }

        private static IEnumerable<byte[]> ReadChunks(string path) {
            var lengthBytes = new byte[sizeof(int)];

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                int n = fs.Read(lengthBytes, 0, sizeof(int));  // Read block size.

                if (n == 0)      // End of file.
                    yield break;

                if (n != sizeof(int))
                    throw new InvalidOperationException("Invalid header");

                int blockLength = BitConverter.ToInt32(lengthBytes, 0);
                var buffer = new byte[blockLength];
                n = fs.Read(buffer, 0, blockLength);

                if (n != blockLength)
                    throw new InvalidOperationException("Missing data");

                yield return buffer;
            }
        }

        private Byte[] convertFileToByte(String _filePath) {

            Byte[] fileData = null;

            using (FileStream fs = File.OpenRead(_filePath)) {
                using (BinaryReader binaryReader = new BinaryReader(fs)) {
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            return fileData;
        }

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
        private async void _mainFileGenerator(int AccountType_, String _AccountTypeStr_) {
            
            void deletionMethod(String fileName, String getDB) {
                String offSqlUpdates = "SET SQL_SAFE_UPDATES = 0";
                command = new MySqlCommand(offSqlUpdates, con);
                command.ExecuteNonQuery();

                String removeQuery = "DELETE FROM " + getDB + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(removeQuery, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@filename", fileName);

                command.ExecuteNonQuery();
                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }
            }

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp|Video Files|*.mp4;*.webm;.mov;.wmv|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;*.csv|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf";
            open.Multiselect = true;
            string varDate = DateTime.Now.ToString("dd/MM/yyyy");
          
            List<String> _filValues = new List<String>();
            int curFilesCount = flowLayoutPanel1.Controls.Count;
            if (open.ShowDialog() == DialogResult.OK) {
                foreach (var selectedItems in open.FileNames) {
                    _filValues.Add(Path.GetFileName(selectedItems));
                    void clearRedundane() {
                        label8.Visible = false;
                        guna2Button6.Visible = false;
                    }
                    
                    if (_filValues.Count() + curFilesCount > AccountType_) {
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
                    }
                    else {

                        Application.DoEvents();

                        string get_ex = open.FileName;
                        string getName = Path.GetFileName(selectedItems);//open.SafeFileName;//selectedItems;//open.SafeFileName;
                        string retrieved = Path.GetExtension(selectedItems); //Path.GetExtension(get_ex);
                        string retrievedName = Path.GetFileNameWithoutExtension(open.FileName);//Path.GetFileNameWithoutExtension(selectedItems);
                        long fileSizeInMB = 0;

                        void containThumbUpload(String nameTable, String getNamePath, Object keyValMain) {

                            String insertThumbQue = "INSERT INTO " + nameTable + "(CUST_FILE_PATH,CUST_USERNAME,UPLOAD_DATE,CUST_FILE,CUST_THUMB) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@UPLOAD_DATE,@CUST_FILE,@CUST_THUMB)";
                            command = new MySqlCommand(insertThumbQue, con);

                            command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                            command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                            command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);

                            command.Parameters["@CUST_FILE_PATH"].Value = getNamePath;
                            command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                            command.Parameters["@UPLOAD_DATE"].Value = varDate;

                            command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                            command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                            command.Parameters["@CUST_FILE"].Value = keyValMain;

                            ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                            Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                            using (var stream = new MemoryStream()) {
                                toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                var toBase64 = Convert.ToBase64String(stream.ToArray());
                                command.Parameters["@CUST_THUMB"].Value = toBase64;// To load: Bitmap -> Byte array
                            }
                            command.ExecuteNonQuery();
                        }

                        void createPanelMain(String nameTable, String panName, int itemCurr, Object keyVal) {

                            Application.DoEvents();

                            if(fileSizeInMB < 8000) {

                                String insertTxtQuery = "INSERT INTO " + nameTable + "(CUST_FILE_PATH,CUST_USERNAME,UPLOAD_DATE,CUST_FILE) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@UPLOAD_DATE,@CUST_FILE)";
                                command = new MySqlCommand(insertTxtQuery, con);

                                command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                                command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);

                                command.Parameters["@CUST_FILE_PATH"].Value = getName;
                                command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                                command.Parameters["@UPLOAD_DATE"].Value = varDate;

                                void startSending(Object setValue) {
                                    Application.DoEvents();                           
                                    command.Parameters["@CUST_FILE"].Value = setValue;
                                    command.ExecuteNonQuery();
                                    command.Dispose();

                                    Application.OpenForms
                                     .OfType<Form>()
                                     .Where(form => String.Equals(form.Name, "UploadAlrt"))
                                     .ToList()
                                     .ForEach(form => form.Close());
                                }

                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = panName + itemCurr,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowLayoutPanel1.Controls.Add(panelTxt);
                                var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls[panName + itemCurr]);

                                var textboxPic = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxPic);
                                textboxPic.Name = "TxtBox" + itemCurr;
                                textboxPic.Width = 240;
                                textboxPic.Height = 164;
                                textboxPic.BorderRadius = 8;
                                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxPic.Enabled = true;
                                textboxPic.Visible = true;

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabVidUp" + itemCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                titleLab.ForeColor = Color.Gainsboro;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = new Point(12, 182);
                                titleLab.Width = 220;
                                titleLab.Height = 30;
                                titleLab.Text = getName;

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxtBut" + itemCurr;
                                remButTxt.Width = 39;
                                remButTxt.Height = 35;
                                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                                remButTxt.BorderRadius = 6;
                                remButTxt.BorderThickness = 1;
                                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                                remButTxt.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                                remButTxt.Visible = true;
                                remButTxt.Location = new Point(189, 218);
                                remButTxt.BringToFront();

                                textboxPic.MouseHover += (_senderM, _ev) => {
                                    panelTxt.ShadowDecoration.Enabled = true;
                                    panelTxt.ShadowDecoration.BorderRadius = 8;
                                };

                                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                                    panelTxt.ShadowDecoration.Enabled = false;
                                };

                                var _setupUploadAlertThread = new Thread(() => new UploadAlrt(getName,label5.Text,"null", panName + itemCurr,"null",_fileSize: fileSizeInMB).ShowDialog());
                                _setupUploadAlertThread.Start();

                                if (nameTable == "file_info") {
                                    startSending(keyVal);

                                    textboxPic.Image = new Bitmap(selectedItems);
                                    textboxPic.Click += (sender_f, e_f) => {
                                        var getImgName = (Guna2PictureBox)sender_f;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                                        Form bgBlur = new Form();
                                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "file_info", "null",label5.Text)) {
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
                                    var encryptValue = EncryptionModel.EncryptText(keyVal.ToString());
                                    startSending(encryptValue);
                                    /*command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                                    command.Parameters["@CUST_FILE"].Value = encryptValue;
                                    command.ExecuteNonQuery();
                                    command.Dispose();*/

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

                                    String nonLine = "";
                                    // using (StreamReader ReadFileTxt = new StreamReader(open.FileName)) {
                                    //     nonLine = ReadFileTxt.ReadToEnd();
                                    // }

                                    //var filePath = open.SafeFileName;
                                    var filePath = getName;

                                    textboxPic.Click += (sender_t, e_t) => {
                                        Form bgBlur = new Form();
                                        using (txtFORM txtFormShow = new txtFORM("IGNORETHIS", "file_info_expand", filePath,"null",label5.Text)) {
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

                                            txtFormShow.Owner = bgBlur;
                                            txtFormShow.ShowDialog();

                                            bgBlur.Dispose();
                                        }

                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_exe") {
                                    startSending(keyVal);

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        Form bgBlur = new Form();
                                        using (exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",label5.Text)) {
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

                                            displayExe.Owner = bgBlur;
                                            displayExe.ShowDialog();

                                            bgBlur.Dispose();
                                        }
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_vid") {
                                    containThumbUpload(nameTable, getName, keyVal);
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        var getImgName = (Guna2PictureBox)sender_ex;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                                        vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "file_info_vid","null",label5.Text);
                                        vidShow.Show();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_audi") {
                                    startSending(keyVal);
                                    var _getWidth = this.Width;
                                    var _getHeight = this.Height;
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null",label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_excel") {
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "file_info_excel", "null", label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_gif") {
                                    containThumbUpload(nameTable, getName, keyVal);
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems); //open.FileName
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",label5.Text)) {
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
                                    startSending(keyVal);

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        using (apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk","null")) {
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
                                if (nameTable == "file_info_pdf") {
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",label5.Text);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_ptx") {
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",label5.Text);
                                        displayPtx.ShowDialog();                                       
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_msi") {
                                   /* command.Parameters["@CUST_FILE"].Value = keyVal;
                                    command.CommandTimeout = 3;
                                    command.ExecuteNonQuery();
                                    command.Dispose();*/
                                   startSending(keyVal);
                           
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        Form bgBlur = new Form();
                                        using (msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null")) {
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
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "file_info_word","null",label5.Text);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                ////////

                                ////////////////// WON'T INSERT IF THESE TWO CODES REPLACED TO ANOTHER PLACE //////////////////
                                remButTxt.Click += (sender_tx, e_tx) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        deletionMethod(titleFile, nameTable);
                                        panelTxt.Dispose();
                                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();

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
                                MessageBox.Show("File is too large, max file size is 8GB.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            }
                        }

                        try {

                            if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico" || retrieved == ".bmp" || retrieved == ".svg") {
                                curr++;
                                var getImg = new Bitmap(selectedItems);//new Bitmap(open.FileName);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;
                                if (retrieved != ".ico") {
                                    Application.DoEvents();
                                    Byte[] _getByteImg = File.ReadAllBytes(selectedItems);
                                    fileSizeInMB = (_getByteImg.Length / 1024) / 1024;
                                    String _tempToBase64 = Convert.ToBase64String(_getByteImg); 
                                    createPanelMain("file_info", "PanImg", curr, _tempToBase64);
                                }
                                else {
                                    Image retrieveIcon = Image.FromFile(selectedItems);//Image.FromFile(open.FileName);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {
                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                        dataIco = msIco.ToArray();
                                        String _tempToBase64 = Convert.ToBase64String(dataIco);
                                        createPanelMain("file_info", "PanImg", curr, _tempToBase64);
                                    }
                                }
                            }
                            else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml" || retrieved == ".py" || retrieved == ".css" || retrieved == ".js" || retrieved == ".sql") {
                                txtCurr++;
                                String nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) { //open.FileName
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }
                                Application.DoEvents();
                                createPanelMain("file_info_expand", "PanTxt", txtCurr, nonLine);
                            }
                            else if (retrieved == ".exe") {
                                exeCurr++;
                                /*Byte[] streamRead = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(streamRead);
                                fileSizeInMB = (streamRead.Length / 1024) / 1024;*/
                                var _toBase64 = Convert.ToBase64String(convertFileToByte(selectedItems));
                                fileSizeInMB = (convertFileToByte(selectedItems).Length/1024)/1024;
                                //var _readFile = ReadChunks(selectedItems);

                                Application.DoEvents();
                                createPanelMain("file_info_exe", "PanExe", exeCurr, _toBase64);

                            }
                            else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi" || retrieved == ".wmv") {
                                vidCurr++;
                                Byte[] streamReadVid = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(streamReadVid);
                                fileSizeInMB = (streamReadVid.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_vid", "PanVid", vidCurr, _toBase64);
                            }
                            else if (retrieved == ".xlsx" || retrieved == ".xls" || retrieved == ".csv") {
                                exlCurr++;
                                Byte[] _toByte = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                fileSizeInMB = (_toByte.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_excel","PanExl",exlCurr,_toBase64);
                            }
                            else if (retrieved == ".mp3" || retrieved == ".wav") {
                                audCurr++;
                                Byte[] toByte_ = File.ReadAllBytes(selectedItems);
                                fileSizeInMB = (toByte_.Length/1024)/1024;
                                var _toBase64 = Convert.ToBase64String(toByte_);
                                Application.DoEvents();
                                createPanelMain("file_info_audi", "PanAud", audCurr, _toBase64); // ReadFile(open.FileName)
                                
                            }
                            else if (retrieved == ".gif") {
                                gifCurr++;
                                Byte[] toByteGif_ = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(toByteGif_);
                                Application.DoEvents();
                                createPanelMain("file_info_gif", "PanGif", gifCurr, _toBase64);
                            }
                            else if (retrieved == ".apk") {
                                apkCurr++;
                                Byte[] readApkBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readApkBytes);
                                fileSizeInMB = (readApkBytes.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_apk", "PanApk", apkCurr, _toBase64);
                            }
                            else if (retrieved == ".pdf") {
                                pdfCurr++;
                                Byte[] readPdfBytes = File.ReadAllBytes(selectedItems);//File.ReadAllBytes(open.FileName);
                                var _toBase64 = Convert.ToBase64String(readPdfBytes);
                                fileSizeInMB = (readPdfBytes.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_pdf", "PanPdf", pdfCurr, _toBase64);
                            }
                            else if (retrieved == ".pptx" || retrieved == ".ppt") {
                                ptxCurr++;
                                Byte[] readPtxBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readPtxBytes);
                                fileSizeInMB = (readPtxBytes.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_ptx", "PanPtx", ptxCurr, _toBase64);
                            }
                            else if (retrieved == ".msi") {
                                msiCurr++;
                                /*Byte[] readMsiBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readMsiBytes);
                                fileSizeInMB = (readMsiBytes.Length / 1024) / 1024;*/
                                var _toBase64 = Convert.ToBase64String(convertFileToByte(selectedItems));
                                fileSizeInMB = (convertFileToByte(selectedItems).Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_msi", "PanMsi", msiCurr, _toBase64);
                            }
                            else if (retrieved == ".docx") {
                                docxCurr++;
                                Byte[] readDocxBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readDocxBytes);
                                fileSizeInMB = (readDocxBytes.Length / 1024) / 1024;
                                Application.DoEvents();
                                createPanelMain("file_info_word", "PanDoc", docxCurr, _toBase64);
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

                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
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
        private void guna2Button2_Click(object sender, EventArgs e) {
            try {
                List<String> _types = new List<String>();
                String _getAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(_getAccType, con);
                command.Parameters.AddWithValue("@username", label5.Text);

                MySqlDataReader readAccType = command.ExecuteReader();
                while (readAccType.Read()) {
                    _types.Add(readAccType.GetString(0));
                }
                readAccType.Close();
                String _accType = _types[0];
                int CurrentUploadCount = Convert.ToInt32(label4.Text);
                if (_accType == "Basic") {
                    if (CurrentUploadCount != 12) {
                        _mainFileGenerator(12, _accType);
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Max") {
                    if (CurrentUploadCount != 30) {
                        _mainFileGenerator(30, _accType);
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Express") {
                    if (CurrentUploadCount != 85) {
                        _mainFileGenerator(85, _accType);
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Supreme") {
                    if (CurrentUploadCount != 170) {
                        _mainFileGenerator(170, _accType);
                    }
                    else {
                        DisplayError(_accType);
                    }
                }
            }
            catch (Exception eq) {
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

        private void panel6_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            Form2 login_page = new Form2();
            login_page.Show();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {

            Thread _showRetrievalAlert = new Thread(() => new settingsAlert().ShowDialog());
            _showRetrievalAlert.Start();

            remAccFORM _RemAccShow = new remAccFORM(label5.Text);
            _RemAccShow.Show();

            Application.OpenForms
            .OfType<Form>()
            .Where(form => String.Equals(form.Name, "settingsAlert"))
            .ToList()
            .ForEach(form => form.Close());

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

        private int countSharing() {
            String queryCountShared = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
            command = new MySqlCommand(queryCountShared,con);
            command.Parameters.AddWithValue("@username",label5.Text);
            var _getValueInt = command.ExecuteScalar();
            int _convertValInt = Convert.ToInt32(_getValueInt);
            return _convertValInt;
        }

        private void guna2Button7_Click(object sender, EventArgs e) {

            if(countSharing() > 0) {
                Thread _showRetrieval = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your shared files...", "Loader").ShowDialog());
                _showRetrieval.Start();
            }

            sharingFORM _ShowSharing = new sharingFORM();
            _ShowSharing.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void pictureBox3_Click(object sender, EventArgs e) {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e) {

        }

        NetworkCredential _login;
        SmtpClient _client;
        MailMessage _Message;
        private void setupEmailSender() {
            using (MailMessage mail = new MailMessage()) {
                mail.From = new MailAddress("nfrealyt@gmail.com");
                mail.To.Add("urmomacc123@gmail.com");
                mail.Subject = "Hello World";
                mail.Body = "<h1>Hello</h1>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) {
                    smtp.Credentials = new NetworkCredential("nfrealyt@gmail.com", "mainpassword-3141");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

        }
        private void guna2Button11_Click(object sender, EventArgs e) {

            try {

                Control flowlayout = flowLayoutPanel1;
                String _getUser = guna2TextBox1.Text;
                String _getPass = guna2TextBox2.Text;
                String _getEmail = guna2TextBox3.Text;
                String _getPin = guna2TextBox4.Text;

                String verfiyUserQue = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
                command = con.CreateCommand();
                command.CommandText = verfiyUserQue;
                command.Parameters.AddWithValue("@username", _getUser);

                List<String> userExists = new List<String>();
                List<String> emailExists = new List<String>();
                List<String> accTypeExists = new List<String>();

                MySqlDataReader userReader = command.ExecuteReader();
                while (userReader.Read()) {
                    userExists.Add(userReader.GetString(0));
                }

                userReader.Close();

                String verifyEmailQue = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
                command = con.CreateCommand();
                command.CommandText = verifyEmailQue;
                command.Parameters.AddWithValue("@email", _getEmail);

                MySqlDataReader emailReader = command.ExecuteReader();
                while (emailReader.Read()) {
                    emailExists.Add(emailReader.GetString(0));
                }
                emailReader.Close();

                String verifyAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
                command = con.CreateCommand();
                command.CommandText = verifyAccType;
                command.Parameters.AddWithValue("@username", _getUser);

                MySqlDataReader accTypeReader = command.ExecuteReader();
                while (accTypeReader.Read()) {
                    accTypeExists.Add(accTypeReader.GetString(0));
                }
                accTypeReader.Close();

                if (emailExists.Count() >= 1 || userExists.Count() >= 1 || accTypeExists.Count() >= 1) {
                    if (emailExists.Count() >= 1) {
                        label22.Visible = true;
                        label22.Text = "Email already exists.";
                    }
                    if (userExists.Count() >= 1) {
                        label11.Visible = true;
                        label11.Text = "Username is taken.";
                    }
                }
                else {
                    label22.Visible = false;
                    label12.Visible = false;
                    label11.Visible = false;
                    if(!String.IsNullOrEmpty(_getPin)) {
                    if(_getPin.Length == 3) {
                        if (_getEmail.Contains("@gmail.com") || _getEmail.Contains("@email.com")) {
                            if (_getUser.Length <= 20) {
                                if (_getPass.Length > 5) {
                                    if (!String.IsNullOrEmpty(_getEmail)) {
                                        if (!String.IsNullOrEmpty(_getPass)) {
                                            if (!String.IsNullOrEmpty(_getUser)) {
                                                flowlayout.Controls.Clear();
                                                if (flowlayout.Controls.Count == 0) {
                                                    Form1.instance.label8.Visible = true;
                                                    Form1.instance.guna2Button6.Visible = true;
                                                }
                                                if (Form1.instance.setupLabel.Text.Length > 14) {
                                                    var label = Form1.instance.setupLabel;
                                                    label.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                                                    label.Location = new Point(3, 27);
                                                }

                                                var _encryptPassVal = EncryptionModel.Encrypt(_getPass, "0123456789085746");
                                                label3.Text = _encryptPassVal;
                                                label5.Text = _getUser;
                                                label24.Text = _getEmail;

                                                var _encryptPinVal = EncryptionModel.Encrypt(_getPin, "0123456789085746");

                                                String _getDate = DateTime.Now.ToString("MM/dd/yyyy");

                                                String _InsertUser = "INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN) VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL,@CUST_PIN)";
                                                command = new MySqlCommand(_InsertUser, con);
                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                command.Parameters.AddWithValue("@CUST_PASSWORD", _encryptPassVal);
                                                command.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                                                command.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                                command.Parameters.AddWithValue("@CUST_PIN", _encryptPinVal);
                                                command.ExecuteNonQuery();

                                                String _InsertType = "INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE) VALUES(@CUST_USERNAME,@CUST_EMAIL,@ACC_TYPE)";
                                                command = new MySqlCommand(_InsertType, con);
                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                command.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                                command.Parameters.AddWithValue("@ACC_TYPE", "Basic");
                                                command.ExecuteNonQuery();

                                           //     setupEmailSender();

                                                label11.Visible = false;
                                                label12.Visible = false;
                                                label30.Visible = false;
                                                guna2Panel7.Visible = false;
                                                guna2TextBox1.Text = String.Empty;
                                                guna2TextBox2.Text = String.Empty;
                                                guna2TextBox3.Text = String.Empty;
                                                guna2TextBox4.Text = String.Empty;
                                                setupTime();

                                                listBox1.Items.Add("Home");
                                                listBox1.SelectedIndex = 0;
                                            }
                                            else {
                                                label11.Visible = true;
                                            }
                                        }
                                        else {
                                            label12.Visible = true;
                                        }
                                    }
                                    else {
                                        label22.Visible = true;
                                        label22.Text = "Please add your email";
                                    }

                                }
                                else {
                                    label12.Visible = true;
                                    label12.Text = "Password must be longer than 5 characters.";
                                }
                            }
                            else {
                                label11.Visible = true;
                                label11.Text = "Username character length limit is 20.";
                            }
                        }
                        else {
                            label22.Visible = true;
                            label22.Text = "Entered email is not valid.";
                        }
                    } else {
                        label30.Visible = true;
                        label30.Text = "PIN Number must have 3 digits.";
                        }
                    } else {
                        label30.Visible = true;
                        label30.Text = "Please add a PIN number.";
                    }
                }
            }
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button17_Click(object sender, EventArgs e) {

        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            LogIN showLogin = new LogIN();
            showLogin.Show();
        }


        private void guna2Button22_Click(object sender, EventArgs e) {

        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e) {

        }

        private void folderDialog(String _getDirPath,String _getDirTitle,String[] _TitleValues) {

            String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);

            void deletionFoldFile(String _Username, String _fileName, String _foldTitle) {
                String _remQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(_remQue, con);
                command.Parameters.AddWithValue("@username", _Username);
                command.Parameters.AddWithValue("@foldtitle", _foldTitle);
                command.Parameters.AddWithValue("@filename", _fileName);
                if (command.ExecuteNonQuery() != 1) {
                    MessageBox.Show("There's an unknown error while attempting to delete this file.", "Erorr");
                }
            }

            int _IntCurr = 0;
            long fileSizeInMB = 0;

            foreach (var _Files in Directory.EnumerateFiles(_getDirPath, "*")) {

                String insertFoldQue_ = "INSERT INTO folder_upload_info(FOLDER_TITLE,CUST_USERNAME,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH,CUST_THUMB) VALUES (@FOLDER_TITLE,@CUST_USERNAME,@CUST_FILE,@FILE_TYPE,@UPLOAD_DATE,@CUST_FILE_PATH,@CUST_THUMB)";
                command = new MySqlCommand(insertFoldQue_, con);

                command.Parameters.Add("@FOLDER_TITLE", MySqlDbType.Text);
                command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                command.Parameters.Add("@FILE_TYPE", MySqlDbType.VarChar, 15);
                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.Text);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                String varDate = DateTime.Now.ToString("dd/MM/yyyy");
                command.Parameters["@FOLDER_TITLE"].Value = _getDirTitle;
                command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                command.Parameters["@CUST_FILE_PATH"].Value = Path.GetFileName(_Files);
                command.Parameters["@FILE_TYPE"].Value = Path.GetExtension(_Files);
                command.Parameters["@UPLOAD_DATE"].Value = varDate;

                void setupUpload(Byte[] _byteValues) {
                    Application.DoEvents();
                    String _tempToBase64 = Convert.ToBase64String(_byteValues);
                    command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                    command.ExecuteNonQuery();
                    command.Dispose();
                }

                _IntCurr++;
                int top = 275;
                int h_p = 100;

                var panelVid = new Guna2Panel() {
                    Name = "PanExlFold" + _IntCurr,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelVid);
                var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanExlFold" + _IntCurr]);
                _controlName = "PanExlFold" + _IntCurr;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = "LabExlUpFold" + _IntCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = _TitleValues[_IntCurr - 1]; // [_IntCurr - 1];

                // LOAD THUMBNAIL

                var textboxExl = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxExl);
                textboxExl.Name = "ExeExlFold" + _IntCurr;
                textboxExl.Width = 240;
                textboxExl.Height = 164;
                textboxExl.FillColor = ColorTranslator.FromHtml("#232323");
                textboxExl.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxExl.BorderRadius = 8;
                textboxExl.Enabled = true;
                textboxExl.Visible = true;

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
                remButExl.Name = "RemExlButFold" + _IntCurr;
                remButExl.Width = 39;
                remButExl.Height = 35;
                remButExl.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButExl.BorderRadius = 6;
                remButExl.BorderThickness = 1;
                remButExl.BorderColor = ColorTranslator.FromHtml("#232323");
                remButExl.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;///Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remButExl.Visible = true;
                remButExl.Location = new Point(189, 218);

                remButExl.Click += (sender_vid, e_vid) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        deletionFoldFile(label5.Text, titleLab.Text, label26.Text);
                        panelVid.Dispose();
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }

                    if (flowLayoutPanel1.Controls.Count == 0) {
                        label8.Visible = true;
                        guna2Button6.Visible = true;
                    }
                };

                Label dateLabExl = new Label();
                mainPanelTxt.Controls.Add(dateLabExl);
                dateLabExl.Name = "LabExlUpFold" + _IntCurr;
                dateLabExl.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                dateLabExl.ForeColor = Color.DarkGray;
                dateLabExl.Visible = true;
                dateLabExl.Enabled = true;
                dateLabExl.Location = new Point(12, 208);
                dateLabExl.Width = 1000;
                dateLabExl.Text = varDate;

                label8.Visible = false;
                guna2Button6.Visible = false;

                _titleText = titleLab.Text;

                var _setupUploadAlertThread = new Thread(() => new UploadAlrt(titleLab.Text, label5.Text, "folder_upload_info", "PanExlFold" + _IntCurr, _getDirTitle,_fileSize: 0101).ShowDialog());
                _setupUploadAlertThread.Start();

                var _extTypes = Path.GetExtension(_Files);

                try {

                    if (_extTypes == ".png" || _extTypes == ".jpg" || _extTypes == ".jpeg" || _extTypes == ".bmp") {
                        var _imgContent = new Bitmap(_Files);
                        Byte[] _getByteImg = File.ReadAllBytes(_Files);
                        setupUpload(_getByteImg);

                        textboxExl.Image = _imgContent;
                        textboxExl.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", label5.Text)) {
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
                    if (_extTypes == ".txt" || _extTypes == ".py" || _extTypes == ".html" || _extTypes == ".css" || _extTypes == ".js" || _extTypes == ".sql") {
                        if (_extTypes == ".py") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                        }
                        else if (_extTypes == ".txt") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                        }
                        else if (_extTypes == ".html") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".css") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".js") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }

                        var _encryptConts = EncryptionModel.Encrypt(File.ReadAllText(_Files), "TXTCONTS01947265");
                        var _readText = File.ReadAllText(_Files);
                        textboxExl.Click += (sender_t, e_t) => {
                            Form bgBlur = new Form();
                            using (txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text, "null", label5.Text)) {
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

                                displayPic.Owner = bgBlur;
                                displayPic.ShowDialog();

                                bgBlur.Dispose();
                            }

                        };
                        Application.DoEvents();
                        command.Parameters["@CUST_FILE"].Value = _encryptConts; // Receive text
                        command.ExecuteNonQuery();
                        command.Dispose();
                    }
                    if (_extTypes == ".apk") {
                        Byte[] _readApkBytes = File.ReadAllBytes(_Files);
                        setupUpload(_readApkBytes);
                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        textboxExl.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk", "null");
                            displayPic.ShowDialog();
                        };
                    }
                    if (_extTypes == ".mp4" || _extTypes == ".mov" || _extTypes == ".webm" || _extTypes == ".avi" || _extTypes == ".wmv") {
                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64;// To load: Bitmap -> Byte array
                        }

                        Byte[] _readVidBytes = File.ReadAllBytes(_Files);
                        Application.DoEvents();
                        String _tempToBase64 = Convert.ToBase64String(_readVidBytes);
                        command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                        command.ExecuteNonQuery();
                        command.Dispose();

                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".gif") {
                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64;// To load: Bitmap -> Byte array
                        }
                        Byte[] _readGifBytes = File.ReadAllBytes(_Files);
                        Application.DoEvents();
                        String _tempToBase64 = Convert.ToBase64String(_readGifBytes);
                        command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                        command.ExecuteNonQuery();
                        command.Dispose();

                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".pdf") {
                        Byte[] readPdfBytes = File.ReadAllBytes(_Files);
                        setupUpload(readPdfBytes);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".docx" || _extTypes == ".doc") {
                        Byte[] readWordBytes = File.ReadAllBytes(_Files);
                        setupUpload(readWordBytes);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayPic = new wordFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".xlsx" || _extTypes == ".xls") {
                        Byte[] readXlsBytes = File.ReadAllBytes(_Files);
                        setupUpload(readXlsBytes);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            exlFORM displayPic = new exlFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }


                    if (_extTypes == ".pptx" || _extTypes == ".ppt") {
                        Byte[] readPtxBytes = File.ReadAllBytes(_Files);
                        setupUpload(readPtxBytes);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            ptxFORM displayPic = new ptxFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".mp3" || _extTypes == ".wav") {
                        Byte[] readAudBytes = File.ReadAllBytes(_Files);
                        setupUpload(readAudBytes);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "UploadAlrt"))
                    .ToList()
                    .ForEach(form => form.Close());

                }
                catch (Exception) {
                    MessageBox.Show("An error ocurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                clearRedundane();
                label4.Text = flowLayoutPanel1.Controls.Count.ToString();
            }
        
        }
        private String getAccountType() {
            List<String> _types = new List<String>();
            String _getAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccType, con);
            command.Parameters.AddWithValue("@username", label5.Text);

            MySqlDataReader readAccType = command.ExecuteReader();
            while (readAccType.Read()) {
                _types.Add(readAccType.GetString(0));
            }
            readAccType.Close();
            String _accType = _types[0];
            return _accType;
        }

        // FOLDER
        private String _titleText;
        private String _controlName;
        private void guna2Button12_Click(object sender, EventArgs e) {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {

                var _getDirPath = dialog.FileName;
                var _accType = getAccountType();
                int _countFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.TopDirectoryOnly).Length;
                var _getDirTitle = new DirectoryInfo(_getDirPath).Name;

                if(!(listBox1.Items.Contains(_getDirTitle))) {

                    flowLayoutPanel1.Controls.Clear();

                    String[] _TitleValues = Directory.GetFiles(_getDirPath, "*").Select(Path.GetFileName).ToArray();
                    int _numberOfFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.AllDirectories).Length;

                    Application.DoEvents();

                    if (_accType == "Basic") {
                        if (_numberOfFiles <= 12) {
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath,_getDirTitle,_TitleValues);
                            var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                            listBox1.SelectedIndex = _dirPosition;
                        }
                        else {
                            DisplayErrorFolder(_accType);
                        }
                    }

                    if (_accType == "Max") {
                        if (_numberOfFiles <= 30) {
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                        }
                        else {
                            DisplayErrorFolder(_accType);
                        }
                    }

                    if (_accType == "Express") {
                        if (_numberOfFiles <= 85) {
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                        }
                        else {
                            DisplayErrorFolder(_accType);
                        }
                    }

                    if (_accType == "Supreme") {
                        if (_numberOfFiles <= 170) {
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                            var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                            listBox1.SelectedIndex = _dirPosition;
                        }
                        else {
                            DisplayErrorFolder(_accType);
                        }
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
            using (UpgradeFormFold displayPic = new UpgradeFormFold(CurAcc)) {
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

        private void label25_Click(object sender, EventArgs e) {

        }
        int foldCurr = 0;
        int mainFoldCurr = 0;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);
            guna2Panel17.Visible = true;
            label27.Visible = true;
            label26.Visible = true;
            guna2Button19.Visible = true;
            label26.Text = _selectedFolder;

            int _countRow(String _tableName) {
                String _countRowTable = "SELECT COUNT(CUST_USERNAME) FROM " + _tableName + " WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(_countRowTable, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                var _totalRow = command.ExecuteScalar();
                int totalRowInt = Convert.ToInt32(_totalRow);
                return totalRowInt;
            }

            if (_selectedFolder == "Home") {
                foldCurr++;
                guna2Button19.Visible = false;
                flowLayoutPanel1.Controls.Clear();

                if (_countRow("file_info") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info", "imgFile", _countRow("file_info"));
                }
                // LOAD .TXT
                if (_countRow("file_info_expand") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_expand", "txtFile", _countRow("file_info_expand"));
                }
                // LOAD EXE
                if (_countRow("file_info_exe") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_exe", "exeFile", _countRow("file_info_exe"));
                }
                // LOAD VID
                if (_countRow("file_info_vid") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_vid", "vidFile", _countRow("file_info_vid"));
                }
                if (_countRow("file_info_excel") > 0) {
                    _generateUserFiles("file_info_excel", "exlFile", _countRow("file_info_excel"));
                }
                if (_countRow("file_info_audi") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_audi", "audiFile", _countRow("file_info_audi"));
                }
                if (_countRow("file_info_gif") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_gif", "gifFile", _countRow("file_info_gif"));
                }
                if (_countRow("file_info_apk") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_apk", "apkFile", _countRow("file_info_apk"));
                }
                if (_countRow("file_info_pdf") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_pdf", "pdfFile", _countRow("file_info_pdf"));
                }
                if (_countRow("file_info_ptx") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_ptx", "ptxFile", _countRow("file_info_ptx"));
                }
                if (_countRow("file_info_msi") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_msi", "msiFile", _countRow("file_info_msi"));
                }
                if (_countRow("file_info_word") > 0) {
                    Application.DoEvents();
                    _generateUserFiles("file_info_word", "docFile", _countRow("file_info_word"));
                }
                if (_countRow("file_info_directory") > 0) {
                    _generateUserDirectory("file_info_directory", "dirFile", _countRow("file_info_directory"));
                }

                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                }
                else {
                    clearRedundane();
                }
                label4.Text = flowLayoutPanel1.Controls.Count.ToString();

            }
            else if (_selectedFolder != "Home") {
                mainFoldCurr++;
                guna2Button19.Visible = true;
                flowLayoutPanel1.Controls.Clear();

                List<string> typesValues = new List<string>();
                String getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(getFileType, con);
                command = con.CreateCommand();
                command.CommandText = getFileType;
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@foldername", _selectedFolder);
                MySqlDataReader _readType = command.ExecuteReader();
                while (_readType.Read()) {
                    typesValues.Add(_readType.GetString(0));// Append ToAr;
                }
                _readType.Close();

                List<String> mainTypes = typesValues.Distinct().ToList();
                var currMainLength = typesValues.Count;
                Application.DoEvents();
                _generateUserFold(typesValues, _selectedFolder, "TESTING", currMainLength); 
                
                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                }
                else {
                    clearRedundane();
                }
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }
        /// <summary>
        /// This function will delete user folder if 
        /// Garbage (delete folder) button is clicked
        /// </summary>
        /// <param name="foldName"></param>
        public void _removeFoldFunc(String foldName) {

            DialogResult verifyDeletion = MessageBox.Show("Delete " + foldName + " folder?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (verifyDeletion == DialogResult.Yes) {

                String removeFoldQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(removeFoldQue, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@foldername", foldName);
                command.ExecuteNonQuery();

                listBox1.Items.Remove(foldName);
                foreach (var _DupeItem in listBox1.Items) {
                    if (_DupeItem.ToString() == foldName) {
                        listBox1.Items.Remove(_DupeItem.ToString());
                    }
                }

                int indexSelected = listBox1.Items.IndexOf("Home");
                listBox1.SelectedIndex = indexSelected;
                Application.OpenForms
                     .OfType<Form>()
                     .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                     .ToList()
                     .ForEach(form => form.Close());
            }
        }

        // REMOVE DIR BUT
        private void guna2Button19_Click(object sender, EventArgs e) {
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "RetrievalAlert"))
              .ToList()
              .ForEach(form => form.Close());
            String _currentFold = listBox1.GetItemText(listBox1.SelectedItem);
            _removeFoldFunc(_currentFold);
        }

        void _generateUserDirectory(String userName, String passUser, int rowLength) {
            for (int i = 0; i < rowLength; i++) {
                int top = 275;
                int h_p = 100;

                flowLayoutPanel1.Location = new Point(13, 10);
                flowLayoutPanel1.Size = new Size(1118, 579);

                var panelPic_Q = new Guna2Panel() {
                    Name = "ABC02" + i,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = ((Guna2Panel)flowLayoutPanel1.Controls["ABC02" + i]);

                List<string> dateValues = new List<string>();
                List<string> titleValues = new List<string>();

                Label directoryLab = new Label();
                panelF.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + i;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                directoryLab.ForeColor = Color.DarkGray;
                directoryLab.Location = new Point(12, 208);
                directoryLab.BackColor = Color.Transparent;
                directoryLab.Width = 75;
                directoryLab.Text = "Directory";

                String getTitleQue = "SELECT DIR_NAME FROM file_info_directory WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(getTitleQue, con);
                command = con.CreateCommand();
                command.CommandText = getTitleQue;
                command.Parameters.AddWithValue("@username", label5.Text);

                MySqlDataReader titleReader = command.ExecuteReader();
                while (titleReader.Read()) {
                    titleValues.Add(titleReader.GetString(0));
                }
                titleReader.Close();

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = titleValues[i];

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 6;
                picMain_Q.Width = 241;
                picMain_Q.Height = 165;
                picMain_Q.Visible = true;

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
                remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remBut.Visible = true;
                remBut.Location = new Point(189, 218);

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' Directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String _removeDirQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@dirname", titleLab.Text);
                        command.ExecuteNonQuery();

                        String _removeDirUploadQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirUploadQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@dirname", titleLab.Text);
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                picMain_Q.Image = FlowSERVER1.Properties.Resources.icon1;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader").ShowDialog());
                    ShowAlert.Start();

                    Form3 displayDirectory = new Form3(titleLab.Text);
                    displayDirectory.Show();

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                };
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
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

        private void label22_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox4.Text, "[^0-9]")) {
                guna2TextBox4.Text = guna2TextBox4.Text.Remove(guna2TextBox4.Text.Length - 1);
            }
        }

        private void guna2TextBox2_TextChanged_1(object sender, EventArgs e) {

        }

        private void label11_Click(object sender, EventArgs e) {

        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e) {

        }

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e) {

        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e) {
            this.Invalidate();
            base.OnScroll(e);
        }

        private void label19_Click(object sender, EventArgs e) {

        }
    }
}