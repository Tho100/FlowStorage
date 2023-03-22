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

namespace FlowSERVER1
{
    /// <summary>
    /// Directory form class
    /// </summary>
    public partial class Form3 : Form {

        public static Form3 instance;
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String _extName = null;

        private string get_ex;
        private string getName;
        private string retrieved;
        private string retrievedName;
        private string tableName;
        private string varDate;
        private long fileSizeInMB;
        private object keyValMain;

        public Form3(String sendTitle_)
        {
            InitializeComponent();
            instance = this;
            this.Text = "Directory: " + sendTitle_;
            label1.Text = sendTitle_;

            var form1 = Form1.instance;

            try {

                int _countRow(String _fileExt) {
                    String _countRowTable = "SELECT COUNT(CUST_USERNAME) FROM " + "upload_info_directory" + " WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    command = new MySqlCommand(_countRowTable, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", label1.Text);
                    command.Parameters.AddWithValue("@ext", _fileExt);

                    var _totalRow = command.ExecuteScalar();
                    int totalRowInt = Convert.ToInt32(_totalRow);
                    return totalRowInt;
                }

                    flowLayoutPanel1.Controls.Clear();

                    if (_countRow(".png") > 0) {
                        _extName = ".png";
                        _generateUserFiles("file_info","imgFilePng",_countRow(".png"));
                    }

                    if (_countRow(".jpg") > 0) {
                        _extName = ".jpg";
                        _generateUserFiles("file_info", "imgFileJpg", _countRow(".jpg"));
                    }

                    if (_countRow(".jpeg") > 0) {
                        _extName = ".jpeg";
                        _generateUserFiles("file_info", "imgFilePeg", _countRow(".jpeg"));
                    }

                    if (_countRow(".bmp") > 0) {
                        _extName = ".bmp";
                        _generateUserFiles("file_info", "imgFileBmp", _countRow(".bmp"));
                    }

                    //
                    // LOAD TEXT
                    //

                    if (_countRow(".txt") > 0) {
                        _extName = ".txt";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".txt"));
                    }

                    if (_countRow(".js") > 0) {
                        _extName = ".js";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".js"));
                    }
                    if (_countRow(".sql") > 0) {
                        _extName = ".sql";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".sql"));
                    }
                    if (_countRow(".py") > 0) {
                        _extName = ".py";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".py"));
                    }
                    if (_countRow(".html") > 0) {
                        _extName = ".html";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".html"));
                    }
                    if (_countRow(".csv") > 0) {
                        _extName = ".csv";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".csv"));
                    }
                    if (_countRow(".css") > 0) {
                        _extName = ".css";
                        _generateUserFiles("file_info_expand", "txtFile", _countRow(".css"));
                    }
                    ////
                // LOAD EXE
                    if (_countRow(".exe") > 0) {
                        _extName = ".exe";
                        _generateUserFiles("file_info_exe", "exeFile", _countRow(".exe"));
                    }
                    // LOAD VID
                    if (_countRow(".mp4") > 0) {
                        _extName = ".mp4";
                        _generateUserFiles("file_info_vid", "vidFile", _countRow(".mp4"));
                    }
                    if (_countRow(".xlsx") > 0) {
                        _extName = ".xlsx";
                        _generateUserFiles("file_info_excel", "exlFile", _countRow(".xlsx"));
                    }
                    if (_countRow(".mp3") > 0) {
                        _extName = ".mp3";
                        _generateUserFiles("file_info_audi", "audiFile", _countRow(".mp3"));
                    }
                    if (_countRow(".gif") > 0) {
                        _extName = ".gif";
                        _generateUserFiles("file_info_gif", "gifFile", _countRow(".gif"));
                    }
                    if (_countRow(".apk") > 0) {
                        _extName = ".apk";
                        _generateUserFiles("file_info_apk", "apkFile", _countRow(".apk"));
                    }
                    if (_countRow(".pdf") > 0) {
                        _extName = ".pdf";
                        _generateUserFiles("file_info_pdf", "pdfFile", _countRow(".pdf"));
                    }
                    if (_countRow(".pptx") > 0) {
                        _extName = ".pptx";
                        _generateUserFiles("file_info_ptx", "ptxFile", _countRow(".pptx"));
                    }
                    if (_countRow(".msi") > 0) {
                        _extName = ".msi";
                        _generateUserFiles("file_info_msi", "msiFile", _countRow(".msi"));
                    }
                    if (_countRow(".docx") > 0) {
                        _extName = ".docx";
                        _generateUserFiles("file_info_word", "docFile", _countRow(".docx"));
                    }

                    if (flowLayoutPanel1.Controls.Count == 0) {
                        showRedundane();
                    }
                    else {
                   //   clearRedundane();
                    }
                } catch (Exception) {
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
        public void _generateUserFiles(String _tableName, String parameterName, int currItem) {

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

                String getUpDate = "SELECT UPLOAD_DATE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                command = new MySqlCommand(getUpDate, con);
                command = con.CreateCommand();
                command.CommandText = getUpDate;

                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);//Form1.instance.
                command.Parameters.AddWithValue("@dirname", label1.Text);
                command.Parameters.AddWithValue("@ext", _extName);

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

                String getTitleQue = "SELECT CUST_FILE_PATH FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                command = new MySqlCommand(getTitleQue, con);
                command = con.CreateCommand();
                command.CommandText = getTitleQue;

                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@dirname", label1.Text);
                command.Parameters.AddWithValue("@ext", _extName);

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

                        String removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", label1.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

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
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", label1.Text);
                    command.Parameters.AddWithValue("@ext", _extName);

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
                    //clearRedundane();
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
                    //clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayExe.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", label1.Text);
                    command.Parameters.AddWithValue("@ext", _extName);

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
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        vidFormShow.Show();
                    };
                    //clearRedundane();
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
                   // clearRedundane();
                }

                if (_tableName == "file_info_gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND FILE_EXT = @ext";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", label1.Text);
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
                 //   clearRedundane();
                }

                if (_tableName == "file_info_apk") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, Form1.instance.label5.Text, "upload_info_directory", label1.Text);
                        displayPic.Show();
                    };
                 //   clearRedundane();
                }

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayPdf.Show();
                    };
                //    clearRedundane();
                }

                if (_tableName == "file_info_ptx") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayPtx.Show();
                    };
                 //   clearRedundane();
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayMsi.Show();
                    };
                  //  clearRedundane();
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "upload_info_directory", label1.Text, Form1.instance.label5.Text);
                        displayMsi.Show();
                    };
                  //  clearRedundane();
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
        int progressionUpload = 0;
        private string _controlName;
        private void _mainFileGenerator(int AccountType_, String _AccountTypeStr_) {
            var form1 = Form1.instance;
            void deletionMethod(String fileName, String getDB) {
                String offSqlUpdates = "SET SQL_SAFE_UPDATES = 0";
                command = new MySqlCommand(offSqlUpdates, con);
                command.ExecuteNonQuery();

                String removeQuery = "DELETE FROM " + "upload_info_directory" + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                command = new MySqlCommand(removeQuery, con);
                command.Parameters.AddWithValue("@username", form1.label5.Text);
                command.Parameters.AddWithValue("@filename", fileName);
                command.Parameters.AddWithValue("@dirname", label1.Text);

                command.ExecuteNonQuery();
                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }
            }

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp|Video Files|*.mp4;*.webm;.mov;.wmv|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;*.csv|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv";
            open.Multiselect = true;
            varDate = DateTime.Now.ToString("dd/MM/yyyy");
        
            List<String> _filValues = new List<String>();
            int curFilesCount = flowLayoutPanel1.Controls.Count;
            if (open.ShowDialog() == DialogResult.OK) {
                foreach(var selectedFileIterate in open.FileNames) {
                    _filValues.Add(Path.GetFileName(selectedFileIterate));
                }

                if (_filValues.Count() + curFilesCount > AccountType_) {
                    Form bgBlur = new Form();
                    using (upgradeFORM displayUpgrade = new upgradeFORM(_AccountTypeStr_)) {
                        bgBlur.StartPosition = FormStartPosition.Manual;
                        bgBlur.FormBorderStyle = FormBorderStyle.None;
                        bgBlur.Opacity = .24d;
                        bgBlur.BackColor = Color.Black;
                        //bgBlur.Size = new Size(_getWidth,_getHeight);
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
                        getName = Path.GetFileName(selectedItems);//open.SafeFileName;//selectedItems;//open.SafeFileName;
                        retrieved = Path.GetExtension(selectedItems); //Path.GetExtension(get_ex);
                        retrievedName = Path.GetFileNameWithoutExtension(open.FileName);//Path.GetFileNameWithoutExtension(selectedItems);
                        fileSizeInMB = 0;

                        void containThumbUpload(String nameTable, String getNamePath, Object keyValMain) {

                            Application.OpenForms
                              .OfType<Form>()
                              .Where(form => String.Equals(form.Name, "UploadAlrt"))
                              .ToList()
                              .ForEach(form => form.Close());

                            String insertThumbQue = "INSERT INTO " + "upload_info_directory" + "(CUST_FILE_PATH,CUST_USERNAME,UPLOAD_DATE,CUST_FILE,CUST_THUMB,FILE_EXT,DIR_NAME) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@UPLOAD_DATE,@CUST_FILE,@CUST_THUMB,@FILE_EXT,@DIR_NAME)";
                            command = new MySqlCommand(insertThumbQue, con);

                            command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                            command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                            command.Parameters.Add("@FILE_EXT", MySqlDbType.LongText);
                            command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                            command.Parameters.Add("@DIR_NAME", MySqlDbType.Text);

                            command.Parameters["@CUST_FILE_PATH"].Value = getNamePath;
                            command.Parameters["@CUST_USERNAME"].Value = form1.label5.Text;
                            command.Parameters["@FILE_EXT"].Value = retrieved;
                            command.Parameters["@UPLOAD_DATE"].Value = varDate;
                            command.Parameters["@DIR_NAME"].Value = label1.Text;

                            command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                            command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                            command.Parameters["@CUST_FILE"].Value = keyValMain;

                            ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                            Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                            using (var stream = new MemoryStream()) {
                                toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                var tobase64 = Convert.ToBase64String(stream.ToArray());
                                command.Parameters["@CUST_THUMB"].Value = tobase64;
                            }
                            Application.OpenForms
                              .OfType<Form>()
                              .Where(form => String.Equals(form.Name, "UploadAlrt"))
                              .ToList()
                              .ForEach(form => form.Close());

                            command.ExecuteNonQuery();

                            Application.OpenForms
                                 .OfType<Form>()
                                 .Where(form => String.Equals(form.Name, "UploadAlrt"))
                                 .ToList()
                                 .ForEach(form => form.Close());
                        }


                        void createPanelMain(String nameTable, String panName, int itemCurr, Object keyVal) {

                            if(fileSizeInMB < 8000) {

                                String insertTxtQuery = "INSERT INTO " + "upload_info_directory" + "(CUST_FILE_PATH,CUST_USERNAME,UPLOAD_DATE,CUST_FILE,CUST_THUMB,FILE_EXT,DIR_NAME) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@UPLOAD_DATE,@CUST_FILE,@CUST_THUMB,@FILE_EXT,@DIR_NAME)";
                                command = new MySqlCommand(insertTxtQuery, con);

                                command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                                command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                                command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);
                                command.Parameters.Add("@FILE_EXT", MySqlDbType.LongText);
                                command.Parameters.Add("@DIR_NAME", MySqlDbType.Text);

                                command.Parameters["@CUST_FILE_PATH"].Value = getName;
                                command.Parameters["@CUST_USERNAME"].Value = form1.label5.Text;
                                command.Parameters["@UPLOAD_DATE"].Value = varDate;

                                command.Parameters["@FILE_EXT"].Value = retrieved;
                                command.Parameters["@DIR_NAME"].Value = label1.Text;
                                command.Parameters["@CUST_THUMB"].Value = "null";

                                void startSending(Object setValue) {

                                    command.Parameters["@CUST_FILE"].Value = setValue;
                                    command.Prepare();
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
                                _controlName = panName + itemCurr;

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

                                var _setupUploadAlertThread = new Thread(() => new UploadAlrt(getName, form1.label5.Text, "upload_info_directory", _controlName, label1.Text, _fileSize: fileSizeInMB).ShowDialog());
                                _setupUploadAlertThread.Start();

                                Application.DoEvents();

                                if (nameTable == "file_info") {
                                    startSending(keyVal);
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
                                    var encryptValue = EncryptionModel.EncryptText(keyVal.ToString());
                                    startSending(encryptValue);

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
                                    containThumbUpload(nameTable, getName, keyVal);
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
                                    startSending(keyVal);
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
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_gif") {
                                    startSending(keyVal);
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
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_ptx") {
                                    startSending(keyVal);
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
                                    startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "upload_info_directory", label1.Text, form1.label5.Text);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                ////////////////// WON'T INSERT IF THESE TWO CODES REPLACED TO ANOTHER PLACE //////////////////
                                remButTxt.Click += (sender_tx, e_tx) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        deletionMethod(titleFile, nameTable);
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
                        }

                        try {

                            if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico" || retrieved == ".bmp" || retrieved == ".svg") {
                                curr++;
                                var getImg = new Bitmap(selectedItems);//new Bitmap(open.FileName);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;
                                if (retrieved != ".ico") {
                                    Byte[] _getByteImg = File.ReadAllBytes(selectedItems);
                                    //fileSizeInMB = (_getByteImg.Length / 1024) / 1024;
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
                            else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml" || retrieved == ".py" || retrieved == ".css" || retrieved == ".js" || retrieved == ".sql" || retrieved == ".csv") {
                                txtCurr++;
                                String nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) { //open.FileName
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }
                                createPanelMain("file_info_expand", "PanTxt", txtCurr, nonLine);
                            }
                            else if (retrieved == ".exe") {
                                exeCurr++;
                                Byte[] streamRead = File.ReadAllBytes(selectedItems);
                                fileSizeInMB = (streamRead.Length / 1024) / 1024;
                                createPanelMain("file_info_exe", "PanExe", exeCurr, streamRead);

                            }
                            else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi" || retrieved == ".wmv") {
                                vidCurr++;
                                Byte[] streamReadVid = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(streamReadVid);
                                fileSizeInMB = (streamReadVid.Length / 1024) / 1024;
                                createPanelMain("file_info_vid", "PanVid", vidCurr, _toBase64);
                            }
                            else if (retrieved == ".xlsx" || retrieved == ".xls") {
                                exlCurr++;
                                Byte[] _toByte = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                fileSizeInMB = (_toByte.Length / 1024) / 1024;
                                createPanelMain("file_info_excel", "PanExl", exlCurr, _toBase64);
                            }
                            else if (retrieved == ".mp3" || retrieved == ".wav") {
                                audCurr++;
                                Byte[] toByte_ = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(toByte_);
                                fileSizeInMB = (toByte_.Length / 1024) / 1024;
                                createPanelMain("file_info_audi", "PanAud", audCurr, _toBase64); // ReadFile(open.FileName)
                            }
                            else if (retrieved == ".gif") {
                                gifCurr++;
                                Byte[] toByteGif_ = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(toByteGif_);
                                createPanelMain("file_info_gif", "PanGif", gifCurr, _toBase64);
                            }
                            else if (retrieved == ".apk") {
                                apkCurr++;
                                Byte[] readApkBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readApkBytes);
                                fileSizeInMB = (readApkBytes.Length / 1024) / 1024;
                                createPanelMain("file_info_apk", "PanApk", apkCurr, _toBase64);
                            }
                            else if (retrieved == ".pdf") {
                                pdfCurr++;
                                Byte[] readPdfBytes = File.ReadAllBytes(selectedItems);//File.ReadAllBytes(open.FileName);
                                var _toBase64 = Convert.ToBase64String(readPdfBytes);
                                fileSizeInMB = (readPdfBytes.Length / 1024) / 1024;
                                createPanelMain("file_info_pdf", "PanPdf", pdfCurr, _toBase64);
                            }
                            else if (retrieved == ".pptx" || retrieved == ".ppt") {
                                ptxCurr++;
                                Byte[] readPtxBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readPtxBytes);
                                fileSizeInMB = (readPtxBytes.Length / 1024) / 1024;
                                createPanelMain("file_info_ptx", "PanPtx", ptxCurr, _toBase64);
                            }
                            else if (retrieved == ".msi") {
                                msiCurr++;
                                Byte[] readMsiBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readMsiBytes);
                                fileSizeInMB = (readMsiBytes.Length / 1024) / 1024;
                                createPanelMain("file_info_msi", "PanMsi", msiCurr, readMsiBytes);
                            }
                            else if (retrieved == ".docx") {
                                docxCurr++;
                                Byte[] readDocxBytes = File.ReadAllBytes(selectedItems);
                                var _toBase64 = Convert.ToBase64String(readDocxBytes);
                                fileSizeInMB = (readDocxBytes.Length / 1024) / 1024;
                                createPanelMain("file_info_word", "PanDoc", docxCurr, _toBase64);
                            }

                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "UploadAlrt"))
                            .ToList()
                            .ForEach(form => form.Close());
                        }

                        catch (Exception ) {
                            //
                        }

                        Application.OpenForms
                         .OfType<Form>()
                         .Where(form => String.Equals(form.Name, "UploadAlrt"))
                         .ToList()
                         .ForEach(form => form.Close());
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
        public static byte[] ReadFile(String filePath) {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally {
                fileStream.Close();
            }
            return buffer;
        }
        private void guna2Button2_Click_1(object sender, EventArgs e) {
            try {

                String _getAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(_getAccType, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                List<String> _types = new List<String>();
                MySqlDataReader _readType = command.ExecuteReader();
                while (_readType.Read()) {
                    _types.Add(_readType.GetString(0));
                }
                _readType.Close();

                String _accType = _types[0];
                int CurrentUploadCount = flowLayoutPanel1.Controls.Count;
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

        private void guna2Button12_Click(object sender, EventArgs e) {

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

            command.Parameters["@CUST_FILE_PATH"].Value = getName;
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