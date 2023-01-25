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

namespace FlowSERVER1
{

    public partial class Form3 : Form {

        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static Form3 instance;
        public Form3(String sendTitle_)
        {
            InitializeComponent();
            instance = this;

            label1.Text = sendTitle_;

            var form1 = Form1.instance;

            try {

                String getFilesType = "SELECT FILE_EXT FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @dirname";
                command = new MySqlCommand(getFilesType,con);
                command = con.CreateCommand();
                command.CommandText = getFilesType;
                command.Parameters.AddWithValue("@username",form1.label5.Text);
                command.Parameters.AddWithValue("@password", form1.label3.Text);
                command.Parameters.AddWithValue("@dirname", label1.Text);

                List<String> _TypeValues = new List<String>();

                Application.DoEvents();
                MySqlDataReader _readType = command.ExecuteReader();
                while (_readType.Read()) {
                    _TypeValues.Add(_readType.GetString(0));// Append ToAr;
                }
                _readType.Close();
                List<String> mainTypes = _TypeValues.Distinct().ToList();
                var currMainLength = _TypeValues.Count;
                generateUserDirectory(_TypeValues, label1.Text, "DIRPAR", _TypeValues.Count);
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

        public void generateUserDirectory(List<String> _extTypes, String _dirTitle, String parameterName, int itemCurr) {

            var form1 = Form1.instance;
            String varDate = DateTime.Now.ToString("dd/MM/yyyy");
            flowLayoutPanel1.Controls.Clear();
            List<String> typeValues = new List<String>(_extTypes);
            for(int q=0; q<itemCurr; q++) {
                int top = 275;
                int h_p = 100;
                var panelTxt = new Guna2Panel() {
                    Name = parameterName + q,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelTxt);
                var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls[parameterName + q]);

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = "TxtBox" + q;
                textboxPic.Width = 240;
                textboxPic.Height = 164;
                textboxPic.BorderRadius = 8;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                List<String> filesNames = new List<string>();

                String _selectFileName = "SELECT CUST_FILE_PATH FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @dirname";
                command = new MySqlCommand(_selectFileName,con);
                command = con.CreateCommand();
                command.CommandText = _selectFileName;
                command.Parameters.AddWithValue("@username",form1.label5.Text);
                command.Parameters.AddWithValue("@password", form1.label3.Text);
                command.Parameters.AddWithValue("@dirname", label1.Text);

                MySqlDataReader _readFileNames = command.ExecuteReader();
                while(_readFileNames.Read()) {
                    filesNames.Add(_readFileNames.GetString(0));
                }
                _readFileNames.Close();

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = "LabVidUp" + q;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesNames[q];

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = "RemTxtBut" + q;
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

                remButTxt.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@password", form1.label3.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

                        mainPanelTxt.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                    }
                };

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = "LabTxtUp" + q;
                dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                dateLabTxt.ForeColor = Color.DarkGray;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = new Point(12, 208);
                dateLabTxt.Width = 1000;
                dateLabTxt.Text = varDate;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                if (typeValues[q] == ".png" || typeValues[q] == ".jpeg" || typeValues[q] == ".jpg" || typeValues[q] == ".bmp")  {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", _dirTitle);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[q]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    textboxPic.Image = new Bitmap(_toMs);
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "upload_info_directory", label1.Text,form1.label5.Text)) {
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

                if(typeValues[q] == ".txt" || typeValues[q] == ".html" || typeValues[q] == ".css" || typeValues[q] == ".py") {
                    if (typeValues[q] == ".py") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                    }
                    else if (typeValues[q] == ".txt") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                    }
                    else if (typeValues[q] == ".html") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                    }
                    else if (typeValues[q] == ".css") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                    }
                    textboxPic.Click += (sender_t, e_t) => {
                        String retrieveContents = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @foldername AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(retrieveContents, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", titleLab.Text);
                        command.Parameters.AddWithValue("@foldername", label1.Text);

                        List<String> _contentValues = new List<String>();
                        MySqlDataReader _readContents = command.ExecuteReader();
                        while (_readContents.Read()) {
                            _contentValues.Add(_readContents.GetString(0));
                        }
                        _readContents.Close();

                        //var _theContents = EncryptionModel.Decrypt(_contentValues[0], "TXTCONTS01947265");
                        //_theContents, "upload_info_directory", titleLab.Text
                        Form bgBlur = new Form();
                        using (txtFORM displayPic = new txtFORM("","upload_info_directory", titleLab.Text,label1.Text,form1.label5.Text)) {
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

                if(typeValues[q] == ".apk") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    textboxPic.Click += (sender_ap, e_ap) => {
                        Form bgBlur = new Form();
                        using (apkFORM displayPic = new apkFORM(titleLab.Text, form1.label5.Text,"upload_info_directory","null")) {
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

                if(typeValues[q] == ".exe") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                    textboxPic.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        using (exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", label1.Text,form1.label5.Text)) {
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
                }

                if(typeValues[q] == ".pdf") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        using (pdfFORM displayPdf = new pdfFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            displayPdf.Owner = bgBlur;
                            displayPdf.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if(typeValues[q] == ".pptx") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        using (ptxFORM displayPtx = new ptxFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            displayPtx.Owner = bgBlur;
                            displayPtx.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if(typeValues[q] == ".gif") {
                    String getGifThumb = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(getGifThumb, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname", label1.Text);
                    command.Parameters.AddWithValue("@filename",titleLab.Text);
                    MySqlDataAdapter da_Read = new MySqlDataAdapter(command);
                    DataSet ds_Read = new DataSet();
                    da_Read.Fill(ds_Read);
                    MemoryStream ms = new MemoryStream((byte[])ds_Read.Tables[0].Rows[0]["CUST_THUMB"]);
                    textboxPic.Image = new Bitmap(ms);
                    textboxPic.Click += (sender_gif, e_gif) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                if (typeValues[q] == ".mp4" || typeValues[q] == ".mov" || typeValues[q] == ".webm" || typeValues[q] == ".avi" || typeValues[q] == ".wmv") {
                    String getImgQue = "SELECT CUST_THUMB FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(getImgQue, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@dirname",label1.Text);
                    command.Parameters.AddWithValue("@filename",titleLab.Text);

                    MySqlDataAdapter da = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();

                    da.Fill(ds);
                    MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[0]["CUST_THUMB"]);
                    textboxPic.Image = new Bitmap(ms);
                    textboxPic.Click += (sender_vid, e_vid) => {
                        var getImgName = (Guna2PictureBox)sender_vid;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        Form bgBlur = new Form();
                        using (vidFORM displayVid = new vidFORM(defaultImage, getWidth,getHeight,titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            displayVid.Owner = bgBlur;
                            displayVid.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".msi") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    textboxPic.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        using (msiFORM displayMsi = new msiFORM(titleLab.Text, "upload_info_directory", label1.Text)) {
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
                }

                if (typeValues[q] == ".docx") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        using (wordFORM displayDocx = new wordFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            displayDocx.Owner = bgBlur;
                            displayDocx.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if(typeValues[q] == ".mp3" || typeValues[q] == ".wav") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        using (audFORM displayAudio = new audFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            displayAudio.Owner = bgBlur;
                            displayAudio.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }
            }
        }

        public void label3_Click(object sender, EventArgs e)
        {

        }
        
        private void Form3_Load(object sender, EventArgs e) {
            //Form4 get_dir_title = new Form4();
            //string dir_title = get_dir_title.guna2TextBox1.Text;
            //label3.Text = dir_title;
            //this.Text = "Directory: " + label3.Text;

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            string server = "localhost";
            string db = "flowserver_db";
            string username = "root";
            string password = "nfreal-yt10";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK) {
                guna2PictureBox1.Image = new Bitmap(open.FileName);
                guna2TextBox1.Text = open.FileName;

                guna2TextBox1.Visible = true;
                guna2PictureBox1.Visible = true;

                MemoryStream ms = new MemoryStream();
                guna2PictureBox1.Image.Save(ms,guna2PictureBox1.Image.RawFormat);
                byte[] img = ms.ToArray(); 

                String query = "INSERT INTO file_info(CUST_FILE_PATH,CUST_FILE) VALUES (@CUST_FILE_PATH,@CUST_FILE)";

                con.Open();

                command = new MySqlCommand(query,con);

                command.Parameters.Add("@CUST_FILE_PATH",MySqlDbType.VarChar,255);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.Blob);

                command.Parameters["@CUST_FILE_PATH"].Value = open.FileName;
                command.Parameters["@CUST_FILE"].Value = img;

                if(command.ExecuteNonQuery() == 1) {
                    MessageBox.Show("INSERTED");
                } else {
                    MessageBox.Show("FAILED");
                }

            }
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


        private int getUserType() {
            String _typeAcc = "";
            int _intAllowed = 0;
            List<String> _emailValues = new List<string>();
            List<String> _typeValues = new List<string>();

            String _queSelectEm = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_queSelectEm, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
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
                _intAllowed = 10;
            }
            else if (_typeAcc == "Max") {
                _intAllowed = 25;
            }
            else if (_typeAcc == "Express") {
                _intAllowed = 40;
            }
            else if (_typeAcc == "Supreme") {
                _intAllowed = 95;
            }
            return _intAllowed;
        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }
        public static int currImg = 0;
        public static int currTxt = 0;
        public static int currApk = 0;
        public static int currAud = 0;
        public static int currExe = 0;
        public static int currPdf = 0;
        public static int currPtx = 0;
        public static int currGif = 0;
        public static int currMsi = 0;
        public static int currDoc = 0;
        public static int currVid = 0;
        public void _mainFileGenerator(String _AccountTypeStr_) {

            String varDate = DateTime.Now.ToString("dd/MM/yyyy");
            var form1 = Form1.instance;

            // INSERT VALUE ORINNGANLLY HERE

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp|Video Files|*.mp4;*.webm;.mov;.avi;.wmv|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf";
            open.Multiselect = true;
            List<String> _filValues = new List<string>();
            int curFilesCount = flowLayoutPanel1.Controls.Count;
            int AccountType_ = getUserType();
            if (open.ShowDialog() == DialogResult.OK) {
                foreach(var selectedItems in open.FileNames) {
                    _filValues.Add(Path.GetFileName(selectedItems));
                    void clearRedundane() {
                        guna2Button6.Visible = false;
                        label8.Visible = false;
                    }

                    if (_filValues.Count() + curFilesCount > AccountType_) {
                        //MessageBox.Show("YOUVE REACHED THE LIKMIT");
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
                        //clearRedundane();
                    } else {

                        String _insertValues = "INSERT INTO upload_info_directory(DIR_NAME,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE,CUST_FILE_PATH,FILE_EXT,CUST_THUMB) VALUES (@DIR_NAME,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE,@CUST_FILE_PATH,@FILE_EXT,@CUST_THUMB)";
                        command = new MySqlCommand(_insertValues, con);
                        command.Parameters.Add("@DIR_NAME", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                        command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                        command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                        command.Parameters.Add("@FILE_EXT", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                        string getName = Path.GetFileName(selectedItems);//open.SafeFileName;
                        string retrieved = Path.GetExtension(selectedItems);
                        string retrievedName = Path.GetFileNameWithoutExtension(open.FileName);

                        void createPanelMain(String type_, String parameterName, int itemCurr) {

                            int top = 275;
                            int h_p = 100;
                            var panelTxt = new Guna2Panel() {
                                Name = parameterName + itemCurr,
                                Width = 240,
                                Height = 262,
                                BorderRadius = 8,
                                FillColor = ColorTranslator.FromHtml("#121212"),
                                BackColor = Color.Transparent,
                                Location = new Point(600, top)
                            };

                            top += h_p;
                            flowLayoutPanel1.Controls.Add(panelTxt);
                            var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls[parameterName + itemCurr]);

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

                            remButTxt.Click += (sender_im, e_im) => {
                                var titleFile = titleLab.Text;
                                DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (verifyDialog == DialogResult.Yes) {
                                    String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                    command = new MySqlCommand(noSafeUpdate, con);
                                    command.ExecuteNonQuery();

                                    String removeQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                                    command = new MySqlCommand(removeQuery, con);
                                    command.Parameters.AddWithValue("@username", form1.label5.Text);
                                    command.Parameters.AddWithValue("@password", form1.label3.Text);
                                    command.Parameters.AddWithValue("@filename", titleFile);
                                    command.ExecuteNonQuery();

                                    mainPanelTxt.Dispose();
                                    if (flowLayoutPanel1.Controls.Count == 0) {
                                        label8.Visible = true;
                                        guna2Button6.Visible = true;
                                    }
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

                            textboxPic.MouseHover += (_senderM, _ev) => {
                                panelTxt.ShadowDecoration.Enabled = true;
                                panelTxt.ShadowDecoration.BorderRadius = 8;
                            };

                            textboxPic.MouseLeave += (_senderQ, _evQ) => {
                                panelTxt.ShadowDecoration.Enabled = false;
                            };

                            if (type_ == "Image") {
                                textboxPic.Image = new Bitmap(selectedItems);
                                textboxPic.Click += (sender_im, e_im) => {
                                    var getImgName = (Guna2PictureBox)sender_im;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                                    Form bgBlur = new Form();
                                    using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "upload_info_directory", label1.Text,form1.label5.Text)) {
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

                            if (type_ == "Texts") {
                                if (retrieved == ".py") {
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                                }
                                else if (retrieved == ".txt") {
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                                }
                                else if (retrieved == ".html") {
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                                }
                                else if (retrieved == ".css") {
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                                }
                                else if (retrieved == ".js") {
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                                }
                                textboxPic.Click += (sender_t, e_t) => {
                                    String retrieveContents = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @foldername AND CUST_FILE_PATH = @filename";
                                    command = new MySqlCommand(retrieveContents, con);
                                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                                    command.Parameters.AddWithValue("@filename", titleLab.Text);
                                    command.Parameters.AddWithValue("@foldername", label1.Text);

                                    List<String> _contentValues = new List<String>();
                                    MySqlDataReader _readContents = command.ExecuteReader();
                                    while (_readContents.Read()) {
                                        _contentValues.Add(_readContents.GetString(0));
                                    }
                                    _readContents.Close();

                                  //  var _theContents = EncryptionModel.Decrypt(_contentValues[0], "TXTCONTS01947265");

                                    Form bgBlur = new Form();
                                    using (txtFORM displayPic = new txtFORM("","upload_info_directory", titleLab.Text,label1.Text,form1.label5.Text)) {
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

                            if (type_ == "Apk") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                                textboxPic.Click += (sender_ap, e_ap) => {
                                    Form bgBlur = new Form();
                                    using (apkFORM displayPic = new apkFORM(titleLab.Text, form1.label5.Text,"upload_info_directory",label1.Text)) {
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

                            if (type_ == "Exe") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                                textboxPic.Click += (sender_ex, e_ex) => {
                                    Form bgBlur = new Form();
                                    using (exeFORM displayExe = new exeFORM(titleLab.Text, "upload_info_directory", label1.Text,form1.label5.Text)) {
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
                            }

                            if (type_ == "Pdf") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                textboxPic.Click += (sender_pd, e_pd) => {
                                    Form bgBlur = new Form();
                                    using (pdfFORM displayPdf = new pdfFORM(titleLab.Text, "upload_info_directory",label1.Text,form1.label5.Text)) {
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

                                        displayPdf.Owner = bgBlur;
                                        displayPdf.ShowDialog();

                                        bgBlur.Dispose();
                                    }
                                };
                                clearRedundane();
                            }

                            if (type_ == "Ptx") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                                textboxPic.Click += (sender_pt, e_pt) => {
                                    Form bgBlur = new Form();
                                    using (ptxFORM displayPtx = new ptxFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                                        displayPtx.Owner = bgBlur;
                                        displayPtx.ShowDialog();

                                        bgBlur.Dispose();
                                    }
                                };
                            }

                            if (type_ == "Gif") {
                                ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                textboxPic.Image = toBitMap;
                                textboxPic.Click += (sender_pt, e_pt) => {
                                    Form bgBlur = new Form();
                                    using (gifFORM displayGif = new gifFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                            if (type_ == "Msi") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                                textboxPic.Click += (sender_pt, e_pt) => {
                                    Form bgBlur = new Form();
                                    using (msiFORM displayMsi = new msiFORM(titleLab.Text,"upload_info_directory",label1.Text)) {
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
                            }

                            if (type_ == "Docx") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                textboxPic.Click += (sender_pt, e_pt) => {
                                    Form bgBlur = new Form();
                                    using (wordFORM displayDocx = new wordFORM(titleLab.Text,"upload_info_directory",label1.Text,form1.label5.Text)) {
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

                                        displayDocx.Owner = bgBlur;
                                        displayDocx.ShowDialog();

                                        bgBlur.Dispose();
                                    }
                                };
                            }

                            if (type_ == "Audio") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                                textboxPic.Click += (sender_Aud, e_Aud) => {
                                   audFORM displayPic = new audFORM(titleLab.Text, "upload_info_directory", label1.Text,form1.label5.Text);
                                    displayPic.Show();
                                };
                            }

                            if (type_ == "Vid") {
                                ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                textboxPic.Image = toBitMap;
                                textboxPic.Click += (sender_vid, e_vid) => {
                                    var getImgName = (Guna2PictureBox)sender_vid;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImg = new Bitmap(getImgName.Image);
                                    Form bgBlur = new Form();
                                    using (vidFORM displayPic = new vidFORM(defaultImg,getWidth,getHeight,titleLab.Text,open.FileName,label1.Text,form1.label5.Text)) {
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
                        }


                        try {

                            command.Parameters["@DIR_NAME"].Value = label1.Text;
                            command.Parameters["@CUST_USERNAME"].Value = form1.label5.Text;
                            command.Parameters["@CUST_PASSWORD"].Value = form1.label3.Text;
                            command.Parameters["@UPLOAD_DATE"].Value = varDate;
                            command.Parameters["@CUST_FILE_PATH"].Value = getName;
                            command.Parameters["@FILE_EXT"].Value = retrieved;

                            UploadAlrt ShowUploadAlert = new UploadAlrt(getName);
                            ShowUploadAlert.Show();
                            Application.DoEvents();

                            if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".webm") {
                                currImg++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;
                                if (retrieved != ".ico") {
                                    Byte[] _getByteImg = File.ReadAllBytes(selectedItems);
                                    String _tempToBase64 = Convert.ToBase64String(_getByteImg);
                                    Application.DoEvents();

                                    command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                                    command.ExecuteNonQuery();
                                    command.Dispose();

                                    clearRedundane();
                                    createPanelMain("Image", "ImagePar", currImg);
                                }
                                else {
                                    Image retrieveIcon = Image.FromFile(open.FileName);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {
                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                        dataIco = msIco.ToArray();
                                        String _tempToBase64 = Convert.ToBase64String(dataIco);

                                        Application.DoEvents();
                                        command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                                        command.ExecuteNonQuery();
                                        command.Dispose();

                                        clearRedundane();
                                        createPanelMain("Image", "IcoPar", currImg);
                                    }
                                }
                            }

                            if (retrieved == ".txt" || retrieved == ".py" || retrieved == ".html" || retrieved == ".css") {
                                currTxt++;

                                var _encryptConts = EncryptionModel.EncryptText(File.ReadAllText(selectedItems));
                                var _readText = File.ReadAllText(selectedItems);
                                Application.DoEvents();
                                command.Parameters["@CUST_FILE"].Value = _encryptConts;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Texts", "TextPar", currTxt);
                            }

                            if (retrieved == ".apk") {
                                currApk++;

                                Application.DoEvents();
                                Byte[] _readApkBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readApkBytes; 
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Apk", "ApkPar", currApk);
                            }

                            if (retrieved == ".exe") {
                                currExe++;

                                Application.DoEvents();
                                Byte[] _readExeBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readExeBytes;//ReadFile(open.FileName);
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Exe", "ExePar", currExe);
                            }
                            if (retrieved == ".pdf") {
                                currPdf++;

                                Application.DoEvents();
                                Byte[] _readPdfBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readPdfBytes;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Pdf", "PdfPar", currPdf);
                            }
                            if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi" || retrieved == ".wmv") {
                                currVid++;

                                Application.DoEvents();
                                Byte[] _readVidBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readVidBytes;
                                ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                using (var stream = new MemoryStream()) {
                                    toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    command.Parameters["@CUST_THUMB"].Value = stream.ToArray();// To load: Bitmap -> Byte array
                                }
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Vid", "VidPar", currVid);
                            }
                            if (retrieved == ".pptx" || retrieved == ".ppt") {
                                currPtx++;

                                Application.DoEvents();
                                Byte[] _readPtxBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readPtxBytes;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Ptx", "PtxPar", currPtx);
                            }
                            if (retrieved == ".gif") {
                                currGif++;

                                Application.DoEvents();
                                Byte[] _readGifBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readGifBytes;

                                ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                using (var stream = new MemoryStream()) {
                                    toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                    command.Parameters["@CUST_THUMB"].Value = stream.ToArray();// To load: Bitmap -> Byte array
                                }
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Gif", "GifPar", currGif);
                            }
                            if (retrieved == ".msi") {
                                currMsi++;

                                Application.DoEvents();
                                Byte[] _readMsiBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readMsiBytes;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Msi", "MsiPar", currMsi);
                            }
                            if (retrieved == ".docx") {
                                currDoc++;

                                Application.DoEvents();
                                Byte[] _readDocxBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readDocxBytes;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Docx", "DocPar", currDoc);
                            }
                            if(retrieved == ".mp3" || retrieved == ".wav") {
                                currAud++;

                                Application.DoEvents();
                                Byte[] _readAudiBytes = File.ReadAllBytes(selectedItems);
                                command.Parameters["@CUST_FILE"].Value = _readAudiBytes;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                clearRedundane();
                                createPanelMain("Audio","AudPar",currAud);
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
                            Form bgBlur = new Form();
                            using (errorLoad displayErr = new errorLoad()) {
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

                                displayErr.Owner = bgBlur;
                                displayErr.ShowDialog();

                                bgBlur.Dispose();
                            }
                        }
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

                String _getAccType = "SELECT ACC_TYPE FROM CUST_TYPE WHERE CUST_USERNAME = @username";
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
                    if (CurrentUploadCount != 25) {
                        _mainFileGenerator("Basic");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Max") {
                    if (CurrentUploadCount != 50) {
                        _mainFileGenerator("Max");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Express") {
                    if (CurrentUploadCount != 85) {
                        _mainFileGenerator("Express");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }

                if (_accType == "Supreme") {
                    if (CurrentUploadCount != 170) {
                        _mainFileGenerator("Supreme");
                    }
                    else {
                        DisplayError(_accType);
                    }
                }
           } catch (Exception eq) {
                MessageBox.Show(eq.Message);
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
    }
}