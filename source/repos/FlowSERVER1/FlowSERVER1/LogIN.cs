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

namespace FlowstorageDesktop {
    public partial class LogIN : Form {
        public static LogIN instance;
        public LogIN() {
            InitializeComponent();
            instance = this;
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
                setupTime();

                String length = "SELECT COUNT(CUST_USERNAME) FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";

                command = new MySqlCommand(length,con);
                command.Parameters.AddWithValue("@username", user);
                command.Parameters.AddWithValue("@password", pass);

                if (Convert.ToInt32(command.ExecuteScalar()) > 0) {
                    try {
                        MessageBox.Show(form.label5.Text);
                        MessageBox.Show(form.label3.Text);
                        string countRowTxt = "SELECT COUNT(CUST_USERNAME) FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(countRowTxt, con);
                        command.Parameters.AddWithValue("@username", form.label5.Text);
                        command.Parameters.AddWithValue("@password", form.label3.Text);

                        var totalRowTxt = command.ExecuteScalar();
                        int intTotalRowTxt = Convert.ToInt32(totalRowTxt);

                        string countRowExe = "SELECT COUNT(CUST_USERNAME) FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(countRowExe, con);
                        command.Parameters.AddWithValue("@username", form.label5.Text);
                        command.Parameters.AddWithValue("@password", form.label3.Text);

                        var totalRowExe = command.ExecuteScalar();
                        int intTotalRowExe = Convert.ToInt32(totalRowExe);
                        label4.Text = intTotalRowExe.ToString();

                        string countRowVid = "SELECT COUNT(CUST_USERNAME) FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(countRowVid, con);
                        command.Parameters.AddWithValue("@username", form.label5.Text);
                        command.Parameters.AddWithValue("@password", form.label3.Text);

                        var totalRowVid = command.ExecuteScalar();
                        int intTotalRowVid = Convert.ToInt32(totalRowVid);

                        if (intRow > 0) {
                            for (int i = 0; i < intRow; i++) {
                                int top = 275;
                                int h_p = 100;

                                flowlayout.Location = new Point(13, 10);
                                flowlayout.Size = new Size(1118, 579);

                                var panelPic_Q = new Guna2Panel() {
                                    Name = "PanG" + i,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };
                                top += h_p;
                                flowlayout.Controls.Add(panelPic_Q);

                                var panelF = ((Guna2Panel)flowlayout.Controls["PanG" + i]);

                                List<string> dateValues = new List<string>();
                                List<string> titleValues = new List<string>();

                                String getUpDate = "SELECT UPLOAD_DATE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = new MySqlCommand(getUpDate, con);
                                command = con.CreateCommand();
                                command.CommandText = getUpDate;

                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);
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
                                dateLab.Location = new Point(12, 235);
                                dateLab.Text = dateValues[i];

                                String getTitleQue = "SELECT CUST_FILE_PATH FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = new MySqlCommand(getTitleQue, con);
                                command = con.CreateCommand();
                                command.CommandText = getTitleQue;

                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

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
                                titleLab.Width = 1000;
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

                                picMain_Q.Click += (sender, e) => {
                                    var getImgName = (Guna2PictureBox)sender;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImage = new Bitmap(getImgName.Image);

                                    picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text);
                                    displayPic.Show();

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

                                        String removeQuery = "DELETE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                                        command = new MySqlCommand(removeQuery, con);
                                        command.Parameters.AddWithValue("@username", form.label5.Text);
                                        command.Parameters.AddWithValue("@password", form.label3.Text);
                                        command.Parameters.AddWithValue("@filename", titleFile);
                                        command.ExecuteNonQuery();
                                    }
                                };

                                form.guna2Button6.Visible = false;
                                form.label8.Visible = false;

                                String retrieveImg = "SELECT CUST_FILE FROM file_info WHERE CUST_USERNAME = @username";
                                command = new MySqlCommand(retrieveImg, con);
                                command.Parameters.AddWithValue("@username", form.label5.Text);

                                MySqlDataAdapter da = new MySqlDataAdapter(command);
                                DataSet ds = new DataSet();

                                da.Fill(ds);
                                MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[i][0]);
                                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);
                                img.Image = new Bitmap(ms);
                            }
                        }

                        // LOAD .TXT

                        if (intTotalRowTxt > 0) {
                            for (int q = 0; q < intTotalRowTxt; q++) {
                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = "PanTxtF" + q,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowlayout.Controls.Add(panelTxt);
                                var mainPanelTxt = ((Guna2Panel)flowlayout.Controls["PanTxtF" + q]);

                                List<string> titlesValuesTxt = new List<string>();
                                List<string> dateValuesTxt = new List<string>();

                                String getTitleTxt = "SELECT CUST_FILE_TXT_NAME FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getTitleTxt;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader pathReaderTxt = command.ExecuteReader();
                                while (pathReaderTxt.Read()) {
                                    titlesValuesTxt.Add(pathReaderTxt.GetString(0));
                                }

                                pathReaderTxt.Close();

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabTxtUp" + q;//Segoe UI Semibold, 11.25pt, style=Bold
                                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                titleLab.ForeColor = Color.Gainsboro;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = new Point(12, 182);
                                titleLab.Width = 1000;
                                titleLab.Height = 30;
                                titleLab.Text = titlesValuesTxt[q];

                                String getDateTxt = "SELECT UPLOAD_DATE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getDateTxt;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader dateReaderTxt = command.ExecuteReader();
                                while (dateReaderTxt.Read()) {
                                    dateValuesTxt.Add(dateReaderTxt.GetString(0));
                                }

                                dateReaderTxt.Close();

                                var textboxPic = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxPic);
                                textboxPic.Name = "TxtBoxF" + q;
                                textboxPic.Width = 240;
                                textboxPic.Height = 164;
                                textboxPic.BorderRadius = 8;
                                textboxPic.Enabled = true;
                                textboxPic.Visible = true;
                                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxPic.Image = Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");

                                textboxPic.Click += (sender_t, e_t) => {
                                    txtFORM txtFormShow = new txtFORM("LOLOL", titleLab.Text);
                                    txtFormShow.Show();
                                };

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxt" + q;
                                remButTxt.Width = 39;
                                remButTxt.Height = 35;
                                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                                remButTxt.BorderRadius = 6;
                                remButTxt.BorderThickness = 1;
                                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                                remButTxt.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                                remButTxt.Visible = true;
                                remButTxt.Location = new Point(189, 218);

                                remButTxt.Click += (sender_rm, e_rm) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                        command = new MySqlCommand(noSafeUpdate, con);
                                        command.ExecuteNonQuery();

                                        String removeQuery = "DELETE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_TXT_NAME = @filename";
                                        command = new MySqlCommand(removeQuery, con);
                                        command.Parameters.AddWithValue("@username", form.label5.Text);
                                        command.Parameters.AddWithValue("@password", form.label3.Text);
                                        command.Parameters.AddWithValue("@filename", titleFile);
                                        command.ExecuteNonQuery();
                                    }
                                };

                                Label dateLabTxt = new Label();
                                mainPanelTxt.Controls.Add(dateLabTxt);
                                dateLabTxt.Name = "LabTxt" + q;//Segoe UI Semibold, 11.25pt, style=Bold
                                dateLabTxt.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                                dateLabTxt.ForeColor = Color.DarkGray;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = new Point(12, 235);
                                dateLabTxt.Width = 1000;
                                dateLabTxt.Text = dateValuesTxt[q];
                            }

                            form.label8.Visible = false;
                            form.guna2Button6.Visible = false;
                        }
                        if (intTotalRowExe > 0) {
                            for (int i = 0; i < intTotalRowExe; i++) {
                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = "PanExeF" + i,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowlayout.Controls.Add(panelTxt);
                                var mainPanelTxt = ((Guna2Panel)flowlayout.Controls["PanExeF" + i]);

                                List<string> titleValues = new List<string>();

                                String getPathQue = "SELECT CUST_FILE_PATH FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getPathQue;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader exePathReader = command.ExecuteReader();
                                while (exePathReader.Read()) {
                                    titleValues.Add(exePathReader.GetString(0));
                                }

                                exePathReader.Close();

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabExeUp" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                titleLab.ForeColor = Color.Gainsboro;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = new Point(12, 182);
                                titleLab.Width = 1000;
                                titleLab.Height = 30;
                                titleLab.Text = titleValues[i];

                                var textboxExe = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxExe);
                                textboxExe.Name = "ExeBoxF" + i;
                                textboxExe.Width = 240;
                                textboxExe.Height = 164;
                                textboxExe.FillColor = ColorTranslator.FromHtml("#232323");
                                textboxExe.Image = Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                                textboxExe.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxExe.BorderRadius = 8;
                                textboxExe.Enabled = true;
                                textboxExe.Visible = true;

                                textboxExe.Click += (sender_ex, e_ex) => {
                                    exeFORM exeFormShow = new exeFORM(titleLab.Text);
                                    exeFormShow.Show();
                                };

                                Guna2Button remButExe = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButExe);
                                remButExe.Name = "RemExeBut" + i;
                                remButExe.Width = 39;
                                remButExe.Height = 35;
                                remButExe.FillColor = ColorTranslator.FromHtml("#4713BF");
                                remButExe.BorderRadius = 6;
                                remButExe.BorderThickness = 1;
                                remButExe.BorderColor = ColorTranslator.FromHtml("#232323");
                                remButExe.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                                remButExe.Visible = true;
                                remButExe.Location = new Point(189, 218);

                                remButExe.Click += (sender_ex, e_ex) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                        command = new MySqlCommand(noSafeUpdate, con);
                                        command.ExecuteNonQuery();

                                        String removeQuery = "DELETE FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                                        command = new MySqlCommand(removeQuery, con);
                                        command.Parameters.AddWithValue("@username", form.label5.Text);
                                        command.Parameters.AddWithValue("@password", form.label3.Text);
                                        command.Parameters.AddWithValue("@filename", titleFile);
                                        command.ExecuteNonQuery();
                                    }
                                };

                                List<string> uploadDateValues = new List<string>();

                                String getDateQue = "SELECT UPLOAD_DATE FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getDateQue;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader exeDateReader = command.ExecuteReader();
                                while (exeDateReader.Read()) {
                                    uploadDateValues.Add(exeDateReader.GetString(0));
                                }

                                Label dateLabTxt = new Label();
                                mainPanelTxt.Controls.Add(dateLabTxt);
                                dateLabTxt.Name = "LabExeUp" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                                dateLabTxt.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                                dateLabTxt.ForeColor = Color.DarkGray;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = new Point(12, 235);
                                dateLabTxt.Text = uploadDateValues[i];

                                exeDateReader.Close();
                                form.label8.Visible = false;
                                form.guna2Button6.Visible = false;
                            }
                        }

                        if (intTotalRowVid > 0) {
                            for (int i = 0; i < intTotalRowVid; i++) {
                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = "PanVidF" + i,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowlayout.Controls.Add(panelTxt);
                                var mainPanelTxt = ((Guna2Panel)flowlayout.Controls["PanVidF" + i]);

                                List<string> titleValues = new List<string>();

                                String getPathQue = "SELECT CUST_FILE_PATH FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getPathQue;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader vidPathReader = command.ExecuteReader();
                                while (vidPathReader.Read()) {
                                    titleValues.Add(vidPathReader.GetString(0));
                                }

                                vidPathReader.Close();

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabVidUp" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                titleLab.ForeColor = Color.Gainsboro;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = new Point(12, 182);
                                titleLab.Width = 1000;
                                titleLab.Height = 30;
                                titleLab.Text = titleValues[i];

                                var textboxVid = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxVid);
                                textboxVid.Name = "VidBoxF" + i;
                                textboxVid.Width = 241;
                                textboxVid.Height = 164; // 144
                                textboxVid.FillColor = ColorTranslator.FromHtml("#232323");
                                textboxVid.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxVid.BorderRadius = 6;
                                textboxVid.Enabled = true;
                                textboxVid.Visible = true;

                                textboxVid.Click += (sender_vq, e_vq) => {
                                    var getImgName = (Guna2PictureBox)sender_vq;
                                    var getWidth = getImgName.Image.Width;
                                    var getHeight = getImgName.Image.Height;
                                    Bitmap defaultImage = new Bitmap(getImgName.Image);
                                    vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text);
                                    vidFormShow.Show();
                                };

                                String getImgQue = "SELECT CUST_THUMB FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = new MySqlCommand(getImgQue, con);
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataAdapter da = new MySqlDataAdapter(command);
                                DataSet ds = new DataSet();

                                da.Fill(ds);
                                MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[i][0]);
                                var img = ((Guna2PictureBox)mainPanelTxt.Controls["VidBoxF" + i]);
                                img.Image = new Bitmap(ms);

                                Guna2Button remButVid = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButVid);
                                remButVid.Name = "RemVidBut" + i;
                                remButVid.Width = 39;
                                remButVid.Height = 35;
                                remButVid.FillColor = ColorTranslator.FromHtml("#4713BF");
                                remButVid.BorderRadius = 6;
                                remButVid.BorderThickness = 1;
                                remButVid.BorderColor = ColorTranslator.FromHtml("#232323");
                                remButVid.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                                remButVid.Visible = true;
                                remButVid.Location = new Point(189, 218);

                                remButVid.Click += (sender_vid, e_vid) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                        command = new MySqlCommand(noSafeUpdate, con);
                                        command.ExecuteNonQuery();

                                        String removeQuery = "DELETE FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                                        command = new MySqlCommand(removeQuery, con);
                                        command.Parameters.AddWithValue("@username", form.label5.Text);
                                        command.Parameters.AddWithValue("@password", form.label3.Text);
                                        command.Parameters.AddWithValue("@filename", titleFile);
                                        command.ExecuteNonQuery();
                                    }
                                };

                                List<string> uploadDateValues = new List<string>();

                                String getDateQue = "SELECT UPLOAD_DATE FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                                command = con.CreateCommand();
                                command.CommandText = getDateQue;
                                command.Parameters.AddWithValue("@username", form.label5.Text);
                                command.Parameters.AddWithValue("@password", form.label3.Text);

                                MySqlDataReader exeDateReader = command.ExecuteReader();
                                while (exeDateReader.Read()) {
                                    uploadDateValues.Add(exeDateReader.GetString(0));
                                }

                                Label dateLabTxt = new Label();
                                mainPanelTxt.Controls.Add(dateLabTxt);
                                dateLabTxt.Name = "LabVidUp" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                                dateLabTxt.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                                dateLabTxt.ForeColor = Color.DarkGray;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = new Point(12, 235);
                                dateLabTxt.Text = uploadDateValues[i];
                                exeDateReader.Close();

                                form.label8.Visible = false;
                                form.guna2Button6.Visible = false;
                            }
                        }

                    }
                    catch (Exception eq) {
                        MessageBox.Show(eq.Message);
                    }

                    label4.Visible = false;
                } else {
                    lab8.Visible = true;
                    but6.Visible = true;
                }
            } else {
                label4.Visible = true;
  
            }
        }
        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            loadUserData();
        }
        int one = 0;
        int two = 0;
        public void setupTime() {
            var form = Form1.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;
            var picturebox2 = form.pictureBox2;
            var picturebox3 = form.pictureBox3;
            try {
                var time = DateTime.Now.ToString("hh:mm:ss tt");
                var theTime = Convert.ToInt32(time.Substring(0, 1));
                var getPeriod = time.Substring(time.Length - 2);
                if (theTime == 0) {
                    var theTimeOne = Convert.ToInt32(time.Substring(1, 1));
                    one = theTimeOne;
                    if (getPeriod == "PM" && one >= 1 && one <= 5) {
                        lab1.Text = "Good afternoon " + lab5.Text + " :)";
                        picturebox2.Visible = true;
                    }
                    else if (getPeriod == "PM" && one >= 6 && one <= 9) {
                        lab1.Text = "Good late-evening " + lab5.Text + " :)";
                        picturebox3.Visible = true;
                    }
                    else if (getPeriod == "AM" && one >= 1 && one <= 10) {
                        lab1.Text = "Good morning " + lab5.Text + " :)";
                        picturebox2.Visible = true;
                    }
                }
                else if (theTime != 0) {
                    var theTimeTwo = Convert.ToInt32(time.Substring(0, 2));
                    two = theTimeTwo;
                    if (getPeriod == "PM" && two >= 10 && two < 12) {
                        lab1.Text = "Good night " + lab5.Text + " :) shouldn't you be sleeping?";
                        picturebox2.Visible = false;
                    }
                }
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
    }
}
