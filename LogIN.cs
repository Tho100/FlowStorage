using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;

namespace FlowSERVER1 {
    public partial class LogIN : Form {
        public static LogIN instance;
        public LogIN() {
            InitializeComponent();
            instance = this;
        }

        public void setupAutoLogin(String _custPass, String _custUsername) {
            String setupDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            Directory.CreateDirectory(setupDir);
            using (StreamWriter _performWrite = File.CreateText(setupDir + "\\CUST_DATAS.txt")) {
                _performWrite.WriteLine(_custUsername);
                _performWrite.WriteLine(_custPass);
            }
        }
        public void loadUserData() {
            string server = "localhost";
            string db = "flowserver_db";
            string username = "root";
            string password = "nfreal-yt10";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            var form = Form1.instance;
            var flowlayout = form.flowLayoutPanel1;
            var but6 = form.guna2Button6;
            var lab8 = form.label8;
            var user = guna2TextBox1.Text;
            var pass = guna2TextBox2.Text;

            con.Open();

            String countRow = "SELECT COUNT(CUST_USERNAME) FROM information WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
            command = new MySqlCommand(countRow, con);
            command.Parameters.AddWithValue("@username", user);
            command.Parameters.AddWithValue("@password", pass);

            var totalRow = command.ExecuteScalar();
            var intRow = Convert.ToInt32(totalRow);
            if(intRow > 0) {
                flowlayout.Controls.Clear();
                form.label5.Text = user;
                form.label3.Text = pass;
                but6.Visible = false;
                lab8.Visible = false;
                label4.Visible = false;
                Form1.instance.guna2Panel7.Visible = false;
                setupTime();
                if (flowlayout.Controls.Count == 0) {
                    Form1.instance.label8.Visible = true;
                    Form1.instance.guna2Button6.Visible = true;
                }
                this.Close();
                try {

                    string countRowTxt = "SELECT COUNT(CUST_USERNAME) FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowTxt, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);

                    var totalRowTxt = command.ExecuteScalar();
                    int intTotalRowTxt = Convert.ToInt32(totalRowTxt);

                    string countRowExe = "SELECT COUNT(CUST_USERNAME) FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowExe, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);

                    var totalRowExe = command.ExecuteScalar();
                    int intTotalRowExe = Convert.ToInt32(totalRowExe);
                    label4.Text = intTotalRowExe.ToString();

                    string countRowVid = "SELECT COUNT(CUST_USERNAME) FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowVid, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);

                    var totalRowVid = command.ExecuteScalar();
                    int intTotalRowVid = Convert.ToInt32(totalRowVid);

                    String countRowImg = "SELECT COUNT(CUST_USERNAME) FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowImg, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                    var totalRowImg = command.ExecuteScalar();
                    var intRowImg = Convert.ToInt32(totalRowImg);
                    //Form1.instance.label6.Text = intRow.ToString();

                    string countRowExcel = "SELECT COUNT(CUST_USERNAME) FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowExcel, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                    var totalRowExcel = command.ExecuteScalar();
                    int intTotalRowExcel = Convert.ToInt32(totalRowExcel);

                    string countRowAudi = "SELECT COUNT(CUST_USERNAME) FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowAudi, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                    var totalRowAudi = command.ExecuteScalar();
                    int intTotalRowAudi = Convert.ToInt32(totalRowAudi);

                    var _form = Form1.instance;

                    void clearRedundane() {
                        _form.guna2Button6.Visible = false;
                        _form.label8.Visible = false;
                    }

                    void _generateUserFiles(String _tableName, String parameterName, int currItem) {
                        for (int i = 0; i < currItem; i++) {
                            int top = 275;
                            int h_p = 100;

                            _form.flowLayoutPanel1.Location = new Point(13, 10);
                            _form.flowLayoutPanel1.Size = new Size(1118, 579);

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
                            _form.flowLayoutPanel1.Controls.Add(panelPic_Q);

                            var panelF = ((Guna2Panel)_form.flowLayoutPanel1.Controls[parameterName + i]);

                            List<string> dateValues = new List<string>();
                            List<string> titleValues = new List<string>();

                            String getUpDate = "SELECT UPLOAD_DATE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                            command = new MySqlCommand(getUpDate, con);
                            command = con.CreateCommand();
                            command.CommandText = getUpDate;

                            command.Parameters.AddWithValue("@username", _form.label5.Text);
                            command.Parameters.AddWithValue("@password", _form.label3.Text);
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

                            String getTitleQue = "SELECT CUST_FILE_PATH FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                            command = new MySqlCommand(getTitleQue, con);
                            command = con.CreateCommand();
                            command.CommandText = getTitleQue;

                            command.Parameters.AddWithValue("@username", _form.label5.Text);
                            command.Parameters.AddWithValue("@password", _form.label3.Text);

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
                            remBut.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                            remBut.Visible = true;
                            remBut.Location = new Point(189, 218);

                            remBut.Click += (sender_im, e_im) => {
                                var titleFile = titleLab.Text;
                                DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (verifyDialog == DialogResult.Yes) {
                                    String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                    command = new MySqlCommand(noSafeUpdate, con);
                                    command.ExecuteNonQuery();

                                    String removeQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                                    command = new MySqlCommand(removeQuery, con);
                                    command.Parameters.AddWithValue("@username", _form.label5.Text);
                                    command.Parameters.AddWithValue("@password", label3.Text);
                                    command.Parameters.AddWithValue("@filename", titleFile);
                                    command.ExecuteNonQuery();

                                    panelPic_Q.Dispose();
                                    if (_form.flowLayoutPanel1.Controls.Count == 0) {
                                        _form.label8.Visible = true;
                                        _form.guna2Button6.Visible = true;
                                    }
                                }
                            };

                            _form.guna2Button6.Visible = false;
                            _form.label8.Visible = false;
                            var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                            if (_tableName == "file_info") {

                                String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = new MySqlCommand(retrieveImg, con);
                                command.Parameters.AddWithValue("@username", _form.label5.Text);
                                command.Parameters.AddWithValue("@password", _form.label3.Text);

                                MySqlDataAdapter da = new MySqlDataAdapter(command);
                                DataSet ds = new DataSet();

                                da.Fill(ds);
                                MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[i][0]);
                                img.Image = new Bitmap(ms);

                                picMain_Q.Click += (sender, e) => {
                                    var getImgName = (Guna2PictureBox)sender;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                                    picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text);
                                    displayPic.Show();

                                };
                                clearRedundane();
                            }

                            if (_tableName == "file_info_expand") {
                                img.Image = Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                                picMain_Q.Click += (sender_t, e_t) => {
                                    txtFORM txtFormShow = new txtFORM("LOLOL", titleLab.Text);
                                    txtFormShow.Show();
                                };
                                clearRedundane();
                            }

                            if (_tableName == "file_info_exe") {
                                img.Image = Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                                picMain_Q.Click += (sender_ex, e_ex) => {
                                    exeFORM exeFormShow = new exeFORM(titleLab.Text);
                                    exeFormShow.Show();
                                };
                                clearRedundane();
                            }

                            if (_tableName == "file_info_vid") {
                                
                                String getImgQue = "SELECT CUST_THUMB FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = new MySqlCommand(getImgQue, con);
                                command.Parameters.AddWithValue("@username", _form.label5.Text);
                                command.Parameters.AddWithValue("@password", _form.label3.Text);

                                MySqlDataAdapter da = new MySqlDataAdapter(command);
                                DataSet ds = new DataSet();

                                da.Fill(ds);
                                MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[i]["CUST_THUMB"]);
                                img.Image = new Bitmap(ms);

                                picMain_Q.Click += (sender_vq, e_vq) => {
                                    var getImgName = (Guna2PictureBox)sender_vq;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImage = new Bitmap(getImgName.Image);
                                    vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "DDD");
                                    vidFormShow.Show();
                                };
                                clearRedundane();
                            }

                            if (_tableName == "file_info_excel") {
                                picMain_Q.Image = Image.FromFile(@"C:\USERS\USER\Downloads\excelicon.png");
                                picMain_Q.Click += (sender_vq, e_vq) => {
                                    exlFORM exlForm = new exlFORM(titleLab.Text, "D");
                                    exlForm.Show();
                                };
                            }

                            if (_tableName == "file_info_audi") {
                                picMain_Q.Image = Image.FromFile(@"C:\users\USER\Downloads\icons8-audio-file-52.png");

                                picMain_Q.Click += (sender_Aud, e_Aud) => {
                                    audFORM audForm = new audFORM(titleLab.Text);
                                    audForm.Show();
                                };
                                clearRedundane();
                            }
                        }
                    }

                    // LOAD IMG
                    if (intRowImg > 0) {
                        _generateUserFiles("file_info", "imgFile", intRow);
                    }
                    // LOAD .TXT
                    if (intTotalRowTxt > 0) {
                        _generateUserFiles("file_info_expand", "txtFile", intTotalRowTxt);
                    }
                    // LOAD EXE
                    if (intTotalRowExe > 0) {
                        _generateUserFiles("file_info_exe", "exeFile", intTotalRowExe);
                    }
                    // LOAD VID
                    if (intTotalRowVid > 0) {
                        _generateUserFiles("file_info_vid", "vidFile", intTotalRowVid);
                    }
                    if (intTotalRowExcel > 0) {
                        _generateUserFiles("file_info_excel", "exlFile", intTotalRowExcel);
                    }
                    if (intTotalRowAudi > 0) {
                        _generateUserFiles("file_info_audi", "audiFile", intTotalRowAudi);
                    }

                    Form1.instance.label4.Text = (intTotalRowExcel + intTotalRowExe + intTotalRowTxt + intTotalRowVid + intRowImg).ToString();

                    if (guna2CheckBox2.Checked == true) {
                        setupAutoLogin(Form1.instance.label3.Text, Form1.instance.label5.Text);
                    }     
                }
                catch (Exception eq) {
                    MessageBox.Show(eq.Message);
                }
            } else {
                label4.Visible = true;
            }

            // AUTO-LOGIN SYSTEM

        }
        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            loadUserData();
        }
        public void setupTime() {
            var form = Form1.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;
            var picturebox2 = form.pictureBox2;
            var picturebox3 = form.pictureBox3;
            var picturebox1 = form.pictureBox1;
            try {
                string[] morningKeys = { "start your day with a coffee?", "" };
                var random = new Random();
                var getKeyRand = random.Next(0, 1);
                var getMorningKeys = morningKeys[getKeyRand];
                DateTime now = DateTime.Now;
                var hours = now.Hour;
                String greeting = null;
                if (hours >= 1 && hours <= 12) {
                    greeting = "Good Morning " + lab5.Text + " :) " + getMorningKeys;
                    picturebox2.Visible = true;
                    picturebox1.Visible = false;
                    picturebox3.Visible = false;
                }
                else if (hours >= 12 && hours <= 16) {
                    greeting = "Good Afternoon " + lab5.Text + " :)";
                    picturebox2.Visible = true;
                    picturebox1.Visible = false;
                    picturebox3.Visible = false;
                }
                else if (hours >= 16 && hours <= 21) {
                    greeting = "Good Evening " + lab5.Text + " :)";
                    picturebox3.Visible = true;
                    picturebox2.Visible = false;
                    picturebox1.Visible = false;
                }
                else if (hours >= 21 && hours <= 24) {
                    greeting = "Good Night " + lab5.Text + " :)";
                    picturebox1.Visible = true;
                    picturebox2.Visible = false;
                    picturebox3.Visible = false;
                }
                lab1.Text = greeting;
            }
            catch (Exception) {
                MessageBox.Show("Oh no! unable to retrieve the time :(( sooo sadd :CCCC");
            }
        }

        private void LogIN_Load(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e) {

        }
    }
}
