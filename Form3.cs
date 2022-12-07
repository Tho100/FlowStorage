using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
using Guna.UI2.WinForms;

namespace FlowSERVER1
{

    public partial class Form3 : Form
    {
        public Form3(String sendTitle_)
        {
            InitializeComponent();
            this.Icon = new Icon(@"C:\Users\USER\Documents\FlowStorage4.ico");
            label1.Text = sendTitle_;

            var form1 = Form1.instance;

            try {
                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 10616;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                String getFilesType = "SELECT FILE_EXT FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @dirname";
                command = new MySqlCommand(getFilesType,con);
                command = con.CreateCommand();
                command.CommandText = getFilesType;
                command.Parameters.AddWithValue("@username",form1.label5.Text);
                command.Parameters.AddWithValue("@password", form1.label3.Text);
                command.Parameters.AddWithValue("@dirname", label1.Text);

                MySqlDataReader _readType = command.ExecuteReader();

                List<string> typesValues = new List<string>();

                while (_readType.Read()) {
                    typesValues.Add(_readType.GetString(0));// Append ToAr;
                }
                _readType.Close();

                List<String> mainTypes = typesValues.Distinct().ToList();
                var currMainLength = typesValues.Count;

                generateUserDirectory(typesValues,label1.Text,"DIRPAR",typesValues.Count);

                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }
                else {
                    label8.Visible = false;
                    guna2Button6.Visible = false;
                }

            } catch (Exception eq) {
                //
            }

        }

        public void generateUserDirectory(List<String> _extTypes, String _dirTitle, String parameterName, int itemCurr) {

            string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
            string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
            string username = "root"; // epiz_33067528 | root
            string password = "nfreal-yt10";
            int mainPort_ = 10616;
            string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            con.Open();

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
                    String retrieveImg = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @foldername";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);
                    command.Parameters.AddWithValue("@password", form1.label3.Text);
                    command.Parameters.AddWithValue("@foldername", _dirTitle);

                    MySqlDataAdapter da = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();

                    da.Fill(ds);
                    MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[q][0]);
                    textboxPic.Image = new Bitmap(ms); 
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text)) {
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

                        var _theContents = EncryptionModel.Decrypt(_contentValues[0], "TXTCONTS");

                        Form bgBlur = new Form();
                        using (txtFORM displayPic = new txtFORM(_theContents, "upload_info_directory", titleLab.Text)) {
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
                        using (apkFORM displayPic = new apkFORM(titleLab.Text, form1.label5.Text)) {
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
                        using (exeFORM displayExe = new exeFORM(titleLab.Text)) {
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
                        using (pdfFORM displayPdf = new pdfFORM(titleLab.Text,"upload_info_directory")) {
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

                if(typeValues[q] == ".pptx" || typeValues[q] == ".ppt") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        using (ptxFORM displayPtx = new ptxFORM(titleLab.Text)) {
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

        private void guna2Button6_Click(object sender, EventArgs e) {

        }
        public static int currImg = 0;
        public static int currTxt = 0;
        public static int currApk = 0;
        public static int currExe = 0;
        public static int currPdf = 0;
        public static int currPtx = 0;
        private void guna2Button2_Click_1(object sender, EventArgs e) {
            try {

                String varDate = DateTime.Now.ToString("dd/MM/yyyy");

                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 10616;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();
                var form1 = Form1.instance;

                String _insertValues = "INSERT INTO upload_info_directory(DIR_NAME,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE,CUST_FILE_PATH,FILE_EXT) VALUES (@DIR_NAME,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE,@CUST_FILE_PATH,@FILE_EXT)";
                command = new MySqlCommand(_insertValues, con);
                command.Parameters.Add("@DIR_NAME", MySqlDbType.Text);
                command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar,255);
                command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                command.Parameters.Add("@FILE_EXT",MySqlDbType.Text);

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp|Icon(*.ico)|*.ico|Video files(*.mp4;*.webm;*.mov)|*.mp4;*.webm;.mov|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php|Markup Languages|*.html;*.css;*.xml|APK Files|*.apk";
                if (open.ShowDialog() == DialogResult.OK) {
                    string get_ex = open.FileName;
                    string getName = open.SafeFileName;
                    string retrieved = System.IO.Path.GetExtension(get_ex);
                    string retrievedName = System.IO.Path.GetFileNameWithoutExtension(open.FileName);

                    void clearRedundane() {
                        guna2Button6.Visible = false;
                        label8.Visible = false;
                    }

                    void createPanelMain(String type_, String parameterName,  int itemCurr) {
                        command.Parameters["@DIR_NAME"].Value = label1.Text;
                        command.Parameters["@CUST_USERNAME"].Value = form1.label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = form1.label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;
                        command.Parameters["@CUST_FILE_PATH"].Value = getName;
                        command.Parameters["@FILE_EXT"].Value = retrieved;

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

                        if(type_ == "Image") {
                            textboxPic.Image = new Bitmap(open.FileName);
                            textboxPic.Click += (sender_im, e_im) => {
                                var getImgName = (Guna2PictureBox)sender_im;
                                var getWidth = getImgName.Image.Width;
                                var getHeight = getImgName.Image.Height;
                                Bitmap defaultImage = new Bitmap(getImgName.Image);

                                Form bgBlur = new Form();
                                using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName)) {
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

                        if(type_ == "Texts") {
                            if (retrieved == ".py") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                            }
                            else if (retrieved == ".txt") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                            }
                            else if (retrieved== ".html") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                            }
                            else if (retrieved == ".css") {
                                textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                            }
                            textboxPic.Click += (sender_t, e_t) => {
                                String retrieveContents = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND DIR_NAME = @foldername AND CUST_FILE_PATH = @filename";
                                command = new MySqlCommand(retrieveContents,con);
                                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                                command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                                command.Parameters.AddWithValue("@filename", titleLab.Text);
                                command.Parameters.AddWithValue("@foldername", label1.Text);

                                List<String> _contentValues = new List<String>();
                                MySqlDataReader _readContents = command.ExecuteReader();
                                while(_readContents.Read()) {
                                    _contentValues.Add(_readContents.GetString(0));
                                }
                                _readContents.Close();

                                var _theContents = EncryptionModel.Decrypt(_contentValues[0],"TXTCONTS");

                                Form bgBlur = new Form();
                                using (txtFORM displayPic = new txtFORM(_theContents,"upload_info_directory", titleLab.Text)) {
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

                        if(type_ == "Apk") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                            textboxPic.Click += (sender_ap, e_ap) => {
                                Form bgBlur = new Form();
                                using (apkFORM displayPic = new apkFORM(titleLab.Text, form1.label5.Text)) {
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

                        if(type_ == "Exe") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                            textboxPic.Click += (sender_ex, e_ex) => {
                                Form bgBlur = new Form();
                                using (exeFORM displayExe = new exeFORM(titleLab.Text)) {
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

                        if(type_ == "Pdf") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                            textboxPic.Click += (sender_pd, e_pd) => {
                                Form bgBlur = new Form();
                                using (pdfFORM displayPdf = new pdfFORM(titleLab.Text,"upload_info_directory")) {
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

                        if(type_ == "Ptx") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                            textboxPic.Click += (sender_pt, e_pt) => {
                                Form bgBlur = new Form();
                                using (ptxFORM displayPtx = new ptxFORM(titleLab.Text)) {
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
                    }

                    if(retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".webm") {
                        currImg++;
                        var getImg = new Bitmap(open.FileName);
                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;
                        if (retrieved != ".ico") {
                            using (MemoryStream ms = new MemoryStream()) {
                                getImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                var setupImg = ms.ToArray();
                                command.Parameters["@CUST_FILE"].Value = setupImg;
                                createPanelMain("Image","ImagePar", currImg);
                            }
                        }
                        else {
                            Image retrieveIcon = Image.FromFile(open.FileName);
                            byte[] dataIco;
                            using (MemoryStream msIco = new MemoryStream()) {
                                retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                dataIco = msIco.ToArray();
                                command.Parameters["@CUST_FILE"].Value = dataIco;
                                createPanelMain("Image","IcoPar", currImg);
                            }
                        }
                    }

                    if (retrieved == ".txt" || retrieved == ".py" || retrieved == ".html" || retrieved == ".css") {
                        currTxt++;
                        var _encryptConts = EncryptionModel.Encrypt(File.ReadAllText(open.FileName), "TXTCONTS");
                        var _readText = File.ReadAllText(open.FileName);
                        command.Parameters["@CUST_FILE"].Value = _encryptConts; // Receive text
                        createPanelMain("Texts","TextPar",currTxt);
                    }

                    if(retrieved == ".apk") {
                        currApk++;
                        Byte[] _readApkBytes = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE"].Value = _readApkBytes;
                        createPanelMain("Apk","ApkPar",currApk);
                    }

                    if(retrieved == ".exe") {
                        currExe++;
                        Byte[] _readExeBytes = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE"].Value = _readExeBytes;
                        createPanelMain("Exe","ExePar",currExe);
                    }
                    if(retrieved == ".pdf") {
                        currPdf++;
                        Byte[] _readPdfBytes = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE"].Value = _readPdfBytes;
                        createPanelMain("Pdf","PdfPar",currPdf);
                    }
                    if(retrieved == ".pptx" || retrieved == ".ppt") {
                        currPtx++;
                        Byte[] _readPtxBytes = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE"].Value = _readPtxBytes;
                        createPanelMain("Ptx","PtxPar",currPtx);
                    }
                    if(command.ExecuteNonQuery() == 1) {
                        clearRedundane();
                    }
                }
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }
    }
}