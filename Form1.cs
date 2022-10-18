using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Resources;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack;

namespace FlowSERVER1 {
    public partial class Form1 : Form {
        public static Form1 instance;
        public Label setupLabel;

        public Form1() {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Icon = new Icon(@"C:\Users\USER\Documents\FlowStorage4.ico");

            randomizeUser();

            string server = "localhost";
            string db = "flowserver_db";
            string username = "root";
            string password = "nfreal-yt10";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            instance = this;
            setupLabel = label5;

            con.Open();

            String query = "INSERT IGNORE INTO file_info(CUST_FILE_PATH,CUST_FILE,CUST_USERNAME,UPLOAD_DATE) VALUES (@CUST_FILE_PATH,@CUST_FILE,@CUST_USERNAME,@UPLOAD_DATE)";

            command = new MySqlCommand(query, con);
            command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.VarChar, 255);

            try {

                string countRowTxt = "SELECT COUNT(CUST_USERNAME) FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowTxt, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

                var totalRowTxt = command.ExecuteScalar();
                int intTotalRowTxt = Convert.ToInt32(totalRowTxt);

                string countRowExe = "SELECT COUNT(CUST_USERNAME) FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowExe, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

                var totalRowExe = command.ExecuteScalar();
                int intTotalRowExe = Convert.ToInt32(totalRowExe);
                label4.Text = intTotalRowExe.ToString();

                string countRowVid = "SELECT COUNT(CUST_USERNAME) FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowVid, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

                var totalRowVid = command.ExecuteScalar();
                int intTotalRowVid = Convert.ToInt32(totalRowVid);

                String countRow = "SELECT COUNT(CUST_USERNAME) FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRow, con);
                command.Parameters.AddWithValue("@username",label5.Text);
                command.Parameters.AddWithValue("@password",label3.Text);
                var totalRow = command.ExecuteScalar();
                var intRow = Convert.ToInt32(totalRow);
                label6.Text = intRow.ToString();

                if (intRow > 0) {
                    for(int i=0; i<intRow; i++) {
                        int top = 275;
                        int h_p = 100;

                        flowLayoutPanel1.Location = new Point(13, 10);
                        flowLayoutPanel1.Size = new Size(1118, 579);

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
                        flowLayoutPanel1.Controls.Add(panelPic_Q);

                        var panelF = ((Guna2Panel)flowLayoutPanel1.Controls["PanG" + i]);

                        List<string> dateValues = new List<string>();
                        List<string> titleValues = new List<string>();

                        String getUpDate = "SELECT UPLOAD_DATE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(getUpDate,con);
                        command = con.CreateCommand();
                        command.CommandText = getUpDate;

                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);
                        MySqlDataReader readerDate = command.ExecuteReader();

                        while(readerDate.Read()) {
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

                        String getTitleQue = "SELECT CUST_FILE_PATH FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(getTitleQue,con);
                        command = con.CreateCommand();
                        command.CommandText = getTitleQue;

                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataReader titleReader = command.ExecuteReader();
                        while(titleReader.Read()) {
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
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@password", label3.Text);
                                command.Parameters.AddWithValue("@filename", titleFile);
                                command.ExecuteNonQuery();

                                panelPic_Q.Dispose();
                                if(flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            }
                        };

                        guna2Button6.Visible = false;
                        label8.Visible = false;
                        var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                        String retrieveImg = "SELECT CUST_FILE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataAdapter da = new MySqlDataAdapter(command);
                        DataSet ds = new DataSet();

                        da.Fill(ds);
                        MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[i][0]);
                        img.Image = new Bitmap(ms);
                    }
                }

                // LOAD .TXT

                if(intTotalRowTxt > 0) {
                    for(int q=0; q<intTotalRowTxt; q++) {
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
                        flowLayoutPanel1.Controls.Add(panelTxt);
                        var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanTxtF" + q]);

                        List<string> titlesValuesTxt = new List<string>();
                        List<string> dateValuesTxt = new List<string>();

                        String getTitleTxt = "SELECT CUST_FILE_TXT_NAME FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getTitleTxt;
                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataReader pathReaderTxt = command.ExecuteReader();
                        while(pathReaderTxt.Read()) {
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
                        titleLab.Width = 220;
                        titleLab.Height = 30;
                        titleLab.Text = titlesValuesTxt[q];

                        String getDateTxt = "SELECT UPLOAD_DATE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getDateTxt;
                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataReader dateReaderTxt = command.ExecuteReader();
                        while(dateReaderTxt.Read()) {
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
                            txtFORM txtFormShow = new txtFORM("LOLOL",titleLab.Text);
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
                            DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                            if(verifyDialog == DialogResult.Yes) {
                                String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                                command = new MySqlCommand(noSafeUpdate,con);
                                command.ExecuteNonQuery();

                                String removeQuery = "DELETE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_TXT_NAME = @filename";
                                command = new MySqlCommand(removeQuery,con);
                                command.Parameters.AddWithValue("@username",label5.Text);
                                command.Parameters.AddWithValue("@password", label3.Text);
                                command.Parameters.AddWithValue("@filename", titleFile);
                                command.ExecuteNonQuery();

                                panelTxt.Dispose();
                                if (flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            }
                        };

                        Label dateLabTxt = new Label();
                        mainPanelTxt.Controls.Add(dateLabTxt);
                        dateLabTxt.Name = "LabTxt" + q;//Segoe UI Semibold, 11.25pt, style=Bold
                        dateLabTxt.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                        dateLabTxt.ForeColor = Color.DarkGray;
                        dateLabTxt.Visible = true;
                        dateLabTxt.Enabled = true;
                        dateLabTxt.Location = new Point(12, 208);
                        dateLabTxt.Width = 1000;
                        dateLabTxt.Text = dateValuesTxt[q];
                    }
                    
                    label8.Visible = false;
                    guna2Button6.Visible = false;
                }
                if(intTotalRowExe > 0) {
                    for(int i=0; i<intTotalRowExe; i++) {
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
                        flowLayoutPanel1.Controls.Add(panelTxt);
                        var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanExeF" + i]);

                        List<string> titleValues = new List<string>();

                        String getPathQue = "SELECT CUST_FILE_PATH FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getPathQue;
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

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
                        titleLab.Width = 220;
                        titleLab.Height = 30;
                        titleLab.Text = titleValues[i];

                        var textboxExe = new Guna2PictureBox();
                        mainPanelTxt.Controls.Add(textboxExe);
                        textboxExe.Name = "ExeBoxF" + i;
                        textboxExe.Width = 240;
                        textboxExe.Height = 164;
                        textboxExe.FillColor = ColorTranslator.FromHtml("#232323");
                        textboxExe.SizeMode = PictureBoxSizeMode.CenterImage;
                        textboxExe.BorderRadius = 8;
                        textboxExe.Enabled = true;
                        textboxExe.Visible = true;

                        var imgExe = ((Guna2PictureBox)mainPanelTxt.Controls["ExeBoxF" + i]);
                        /*
                        String retrieveImgExe = "SELECT CUST_THUMB FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(retrieveImgExe,con);
                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataAdapter da_exe = new MySqlDataAdapter(command);
                        DataSet ds_exe = new DataSet();

                        da_exe.Fill(ds_exe);
                        MemoryStream ms_exe = new MemoryStream((byte[])ds_exe.Tables[0].Rows[i][0]);
                        imgExe.Image = new Bitmap(ms_exe);*/

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
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@password", label3.Text);
                                command.Parameters.AddWithValue("@filename", titleFile);
                                command.ExecuteNonQuery();

                                panelTxt.Dispose();
                                if (flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            }
                        };

                        List<string> uploadDateValues = new List<string>();

                        String getDateQue = "SELECT UPLOAD_DATE FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getDateQue;
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

                        MySqlDataReader exeDateReader = command.ExecuteReader();
                        while(exeDateReader.Read()) {
                            uploadDateValues.Add(exeDateReader.GetString(0));
                        }

                        Label dateLabTxt = new Label();
                        mainPanelTxt.Controls.Add(dateLabTxt);
                        dateLabTxt.Name = "LabExeUp" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                        dateLabTxt.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                        dateLabTxt.ForeColor = Color.DarkGray;
                        dateLabTxt.Visible = true;
                        dateLabTxt.Enabled = true;
                        dateLabTxt.Location = new Point(12, 208);
                        dateLabTxt.Text = uploadDateValues[i];

                        exeDateReader.Close();
                        label8.Visible = false;
                        guna2Button6.Visible = false;
                    }
                }

                if(intTotalRowVid > 0) {
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
                        flowLayoutPanel1.Controls.Add(panelTxt);
                        var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanVidF" + i]);

                        List<string> titleValues = new List<string>();

                        String getPathQue = "SELECT CUST_FILE_PATH FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getPathQue;
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

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
                        titleLab.Width = 220;
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
                            var getImgName = (Guna2PictureBox) sender_vq;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            vidFORM vidFormShow = new vidFORM(defaultImage,getWidth,getHeight,titleLab.Text);
                            vidFormShow.Show();
                        };

                        String getImgQue = "SELECT CUST_THUMB FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = new MySqlCommand(getImgQue, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

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
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@password", label3.Text);
                                command.Parameters.AddWithValue("@filename", titleFile);
                                command.ExecuteNonQuery();

                                panelTxt.Dispose();
                                if (flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            }
                        };

                        List<string> uploadDateValues = new List<string>();

                        String getDateQue = "SELECT UPLOAD_DATE FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                        command = con.CreateCommand();
                        command.CommandText = getDateQue;
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);

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
                        dateLabTxt.Location = new Point(12, 208);
                        dateLabTxt.Text =  uploadDateValues[i];
                        exeDateReader.Close();

                        label8.Visible = false;
                        guna2Button6.Visible = false;
                    }
                }

            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }

            // COUNT TOTAL FILE
            /*
            command = new MySqlCommand("SELECT COUNT(*) FROM file_info WHERE CUST_USERNAME = @username" AND CUST_PASSWORD = @password, con);
	    command.Parameters.AddWithValue("@password",label3.Text);
            command.Parameters.AddWithValue("@username",label5.Text);
            var totalFilesCount = command.ExecuteScalar();
            var totalFileInt = Convert.ToInt32(totalFilesCount);
            label6.Text = totalFileInt.ToString();*/
        }

        private void Form1_Load(object sender, EventArgs e) {
            setupTime();
        }
        int one = 0;
        int two = 0;
        public void setupTime() {
            try {
                var time = DateTime.Now.ToString("hh:mm:ss tt");
                var theTime = Convert.ToInt32(time.Substring(0, 1));
                var getPeriod = time.Substring(time.Length - 2);
                if(theTime == 0) {
                    var theTimeOne = Convert.ToInt32(time.Substring(1, 1));
                    one = theTimeOne;
                    if(getPeriod == "PM" && one >= 1 && one <= 5) {
                        label1.Text = "Good afternoon " + label5.Text + " :)";
                        pictureBox2.Visible = true;
                    } else if(getPeriod == "PM" && one >= 6 && one <= 9) {
                        label1.Text = "Good late-evening " + label5.Text + " :)";
                        pictureBox3.Visible = true;
                    }
                    else if(getPeriod == "AM" && one >= 1 && one <= 10) {
                        label1.Text = "Good morning " + label5.Text + " :)";
                        pictureBox2.Visible = true;
                    }
                } else if (theTime != 0) {
                    var theTimeTwo = Convert.ToInt32(time.Substring(0, 2));
                    two = theTimeTwo;
                    if(getPeriod == "PM" && two >= 10 && two < 12) {
                        label1.Text = "Good night " + label5.Text + " :) shouldn't you be sleeping?";
                        pictureBox2.Visible = false;
                    } 
                }
            }
            catch (Exception) {
                MessageBox.Show("Oh no! unable to retrieve the time :(( sooo sadd :CCCC");
            }
        }

        // GENERATE USERNAME
        public void randomizeUser() {
            Random setupRand = new Random();
	    int setupTotalRand = setupRand.Next(0,300);
            int randInt1 = setupRand.Next(0,setupTotalRand);
            int randInt2 = setupRand.Next(1, 300);
            int randInt3 = setupRand.Next(0, 300);
            int randInt4 = setupRand.Next(0, 9);
            var usernameSet = "Guest" + randInt1 + randInt2 + randInt3 + randInt4;
            var setupPath = @"C:\FLOWSTORAGEINFO\cust_username.txt";
            Directory.CreateDirectory(@"C:\FLOWSTORAGEINFO\"); 
            if(Directory.Exists(@"C:\FLOWSTORAGEINFO\")) {
                using (StreamWriter sw = File.AppendText(setupPath)) {
                    sw.WriteLine(usernameSet);
                    sw.Close();
                }
            }

            String retrieveFirstLine = File.ReadLines(@"C:\FLOWSTORAGEINFO\cust_username.txt").First();
            label5.Text = retrieveFirstLine;

            var ReadFile = new List<string>(File.ReadAllLines(setupPath));
            ReadFile.RemoveAt(1);
            File.WriteAllLines(setupPath,ReadFile.ToArray());
            setupUserDate();
        }

        public void setupUserDate() {
            var setupPath = @"C:\FLOWSTORAGEINFO\";
            if(Directory.Exists(setupPath)) {
                FileInfo getCustfile = new FileInfo(setupPath + "cust_username.txt");
                DateTime getCreationTime = getCustfile.CreationTime;
                label10.Text = getCreationTime.ToString().Substring(0,10);
            }
        }
        // Dir
        private void guna2Button1_Click(object sender, EventArgs e) {
            Form4 create_dir = new Form4();
            create_dir.Show();
        }
        // Add File  
        int curr = 0;
        int txtCurr = 0;
        int exeCurr = 0;
        int vidCurr = 0;
        private void guna2Button2_Click(object sender, EventArgs e) {
            try {
                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                void deletionMethod(String fileName, String getDB) {
                    String offSqlUpdates = "SET SQL_SAFE_UPDATES = 0";
                    command = new MySqlCommand(offSqlUpdates,con);
                    command.ExecuteNonQuery();

                    String removeQuery = "DELETE FROM " + getDB + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(removeQuery, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    command.Parameters.AddWithValue("@filename", fileName);

                    command.ExecuteNonQuery();
                    if (flowLayoutPanel1.Controls.Count == 0) {
                        label8.Visible = true;
                        guna2Button6.Visible = true;
                    }
                }

                void increaseSizeMethod() {
                    String setupPacketMax = "SET GLOBAL max_allowed_packet=10000000000000;";
                    command = new MySqlCommand(setupPacketMax, con);
                    command.ExecuteNonQuery();
                }

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "All Files(*.*)|*.*|Images(*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;.bmp|Icon(*.ico)|*.ico|Video files(*.mp4;*.webm;*.mov)|*.mp4;*.webm;.mov|Text files(*.txt;)|*.txt;|HTML files(*.html;)|*.html;|Exe Files(*.exe)|*.exe|FlowDB Records(*.fldb)|.*fldb;";
                string varDate = DateTime.Now.ToString("dd/MM/yyyy");
                if (open.ShowDialog() == DialogResult.OK) {
                    string get_ex = open.FileName;
                    string getName = open.SafeFileName;
                    string retrieved = System.IO.Path.GetExtension(get_ex);
                    string retrievedName = System.IO.Path.GetFileNameWithoutExtension(open.FileName);
                    if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico") {
                        curr++;
                        var getImg = new Bitmap(open.FileName);

                        var imgWidth = getImg.Width;
                        var imgHeight = getImg.Height;
                        int top = 275;
                        int h_p = 100;
                        var panelPic = new Guna2Panel() {
                            Name = "ImgPan" + curr,
                            Width = 240,
                            Height = 262,
                            BorderRadius = 8,
                            FillColor = ColorTranslator.FromHtml("#121212"),
                            BackColor = Color.Transparent,
                            Location = new Point(600, top)
                        };

                        top += h_p;
                        flowLayoutPanel1.Controls.Add(panelPic);
           
                        var panel = ((Guna2Panel)flowLayoutPanel1.Controls["ImgPan" + curr]);
                        
                        Label titleLab = new Label();
                        panel.Controls.Add(titleLab);
                        titleLab.Name = "ImgTitle" + curr;
                        titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                        titleLab.ForeColor = Color.Gainsboro;
                        titleLab.Visible = true;
                        titleLab.Enabled = true;
                        titleLab.Location = new Point(12, 182);
                        titleLab.Width = 220;
                        titleLab.Height = 30;
                        titleLab.Text = getName;

                        Guna2PictureBox picMain = new Guna2PictureBox();
                        panel.Controls.Add(picMain);
                        picMain.Name = "Img" + curr;
                        picMain.SizeMode = PictureBoxSizeMode.CenterImage;
                        picMain.BorderRadius = 6;
                        picMain.Width = 241;
                        picMain.Height = 164;
                        picMain.Visible = true;

                        picMain.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName);
                            displayPic.Show();
                        };

                        Label dateLab = new Label();
                        panel.Controls.Add(dateLab);
                        dateLab.Name = "ImgDate" + curr;
                        dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                        dateLab.ForeColor = Color.DarkGray;
                        dateLab.Visible = true;
                        dateLab.Enabled = true;
                        dateLab.Location = new Point(12, 208);
                        dateLab.Text = varDate;

                        Guna2Button remBut = new Guna2Button();
                        panel.Controls.Add(remBut);
                        remBut.Name = "remImg" + curr;
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
                                panelPic.Dispose();
                                deletionMethod(titleFile,"file_info");
                            }

                            if (flowLayoutPanel1.Controls.Count == 0) {
                                label8.Visible = true;
                                guna2Button6.Visible = true;
                            }
                        };

                        label8.Visible = false;
                        guna2Button6.Visible = false;

                        for (int i=0; i<flowLayoutPanel1.Controls.Count; i++) {
                            var img = ((Guna2PictureBox)panel.Controls["Img" + curr]);
                            img.Image = new Bitmap(open.FileName);
                        }

                        con.Open();
                        increaseSizeMethod();

                        string selectUsername = "SELECT CUST_USERNAME from information WHERE CUST_USERNAME = @username";
                        String query = "INSERT IGNORE INTO file_info(CUST_FILE_PATH,CUST_FILE,CUST_USERNAME,UPLOAD_DATE,CUST_PASSWORD) VALUES (@CUST_FILE_PATH,@CUST_FILE,@CUST_USERNAME,@UPLOAD_DATE,@CUST_PASSWORD)";
                        
                        command = new MySqlCommand(query, con);
                        command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.VarChar, 255);
                        command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                        command.Parameters.Add(@"CUST_USERNAME",MySqlDbType.Text);
                        command.Parameters.Add(@"CUST_PASSWORD", MySqlDbType.Text);
                        command.Parameters.Add(@"UPLOAD_DATE", MySqlDbType.VarChar,255);
                        command.Parameters.AddWithValue("@username",label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);
                        
                        if(!(retrieved == ".ico")) {

                            MemoryStream ms = new MemoryStream();
                            var imgBox = ((Guna2PictureBox)panel.Controls["Img" + curr]);
                            imgBox.Image.Save(ms, imgBox.Image.RawFormat);
                        
                            byte[] imgSetup = ms.ToArray();
 
                            command.Parameters["@CUST_FILE_PATH"].Value = getName;
                            command.Parameters["@CUST_FILE"].Value = imgSetup;
                            command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                            command.Parameters[@"UPLOAD_DATE"].Value = varDate;
                            command.Parameters[@"CUST_PASSWORD"].Value = label3.Text;
                            if(command.ExecuteNonQuery() == 1) {
                                //
                            } 
                        } else {
                            Image retrieveIcon = Image.FromFile(open.FileName);
                            byte[] dataIco;
                            using(MemoryStream msIco = new MemoryStream()) {
                                retrieveIcon.Save(msIco,System.Drawing.Imaging.ImageFormat.Bmp);
                                dataIco = msIco.ToArray();
                            }
                            command.Parameters["@CUST_FILE_PATH"].Value = getName;
                            command.Parameters["@CUST_FILE"].Value = dataIco;
                            command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                            command.Parameters[@"UPLOAD_DATE"].Value = varDate;
                            command.Parameters[@"CUST_PASSWORD"].Value = label3.Text;
                            if (command.ExecuteNonQuery() == 1) {
                                //
                            }
                        }

                        command = new MySqlCommand("SELECT COUNT(CUST_FILE) FROM file_info WHERE CUST_USERNAME = @username",con);
                        command.Parameters.AddWithValue("@username",label5.Text);
                        var totalFilesCount = command.ExecuteScalar();
                        var totalFileInt = Convert.ToInt32(totalFilesCount);
                        label6.Text = totalFileInt.ToString();
                    } else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml") {
                        txtCurr++;
                        int top = 275;
                        int h_p = 100;
                        var panelTxt = new Guna2Panel() {
                            Name = "PanTxt" + txtCurr,
                            Width = 240,
                            Height = 262,
                            BorderRadius = 8,
                            FillColor = ColorTranslator.FromHtml("#121212"),
                            BackColor = Color.Transparent,
                            Location = new Point(600, top)
                        };

                        top += h_p;
                        flowLayoutPanel1.Controls.Add(panelTxt);
                        var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanTxt" + txtCurr]);

                        var textboxPic = new Guna2PictureBox();
                        mainPanelTxt.Controls.Add(textboxPic);
                        textboxPic.Name = "TxtBox" + txtCurr;
                        textboxPic.Width = 240;
                        textboxPic.Height = 164;
                        textboxPic.BorderRadius = 8;
                        textboxPic.SizeMode = PictureBoxSizeMode.CenterImage; 
                        textboxPic.Image = Image.FromFile(@"C:\users\USER\Downloads\Gallery\icons8-txt-48.png");
                        textboxPic.Enabled = true;
                        textboxPic.Visible = true;

                        String nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(open.FileName)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }
                        var filePath = open.SafeFileName;

                        textboxPic.Click += (sender_t, e_t) => {
                            txtFORM txtFormShow = new txtFORM(nonLine,filePath);
                            txtFormShow.Show();
                        };

                        Label titleLab = new Label();
                        mainPanelTxt.Controls.Add(titleLab);
                        titleLab.Name = "LabVidUp" + txtCurr;//Segoe UI Semibold, 11.25pt, style=Bold
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
                        remButTxt.Name = "RemTxtBut" + txtCurr;
                        remButTxt.Width = 39;
                        remButTxt.Height = 35;
                        remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                        remButTxt.BorderRadius = 6;
                        remButTxt.BorderThickness = 1;
                        remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                        remButTxt.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                        remButTxt.Visible = true;
                        remButTxt.Location = new Point(189, 218);
                        remButTxt.BringToFront();

                        remButTxt.Click += (sender_tx, e_tx) => {
                            var titleFile = titleLab.Text;
                            DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (verifyDialog == DialogResult.Yes) {
                                deletionMethod(titleFile,"file_info_expand");
                                panelTxt.Dispose();
                            }

                            if (flowLayoutPanel1.Controls.Count == 0) {
                                label8.Visible = true;
                                guna2Button6.Visible = true;
                            }
                        };

                        Label dateLabTxt = new Label();
                        mainPanelTxt.Controls.Add(dateLabTxt);
                        dateLabTxt.Name = "LabTxtUp" + txtCurr;
                        dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                        dateLabTxt.ForeColor = Color.DarkGray;
                        dateLabTxt.Visible = true;
                        dateLabTxt.Enabled = true;
                        dateLabTxt.Location = new Point(12, 208);
                        dateLabTxt.Width = 1000;
                        dateLabTxt.Text = varDate;

                        con.Open();
                        increaseSizeMethod();

                        String insertTxtQuery = "INSERT INTO file_info_expand(CUST_FILE_TXT_NAME,CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE_TXT) VALUE (@CUST_FILE_TXT_NAME,@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE_TXT)";
                        command = new MySqlCommand(insertTxtQuery,con);
                        command.Parameters.Add("@CUST_FILE_TXT_NAME",MySqlDbType.Text);
                        command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.VarChar,255);
                        command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                        command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar,255);
                        command.Parameters.Add("@CUST_FILE_TXT", MySqlDbType.LongText);

                        command.Parameters["@CUST_FILE_TXT_NAME"].Value = getName;
                        command.Parameters["@CUST_FILE_PATH"].Value = filePath;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;
                        command.Parameters["@CUST_FILE_TXT"].Value = nonLine;
                        if(command.ExecuteNonQuery() == 1) {
                            guna2Button6.Visible = false;
		                    label8.Visible = false;
                        } else {
                            //
                        }

                    } else if (retrieved == ".exe") {
                        con.Open();
                        increaseSizeMethod();
                        //Process.Start(open.FileName);
                        String exeInsQuery = "INSERT INTO file_info_exe(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE_EXE,CUST_THUMB) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE_EXE,@CUST_THUMB)";
                        command = new MySqlCommand(exeInsQuery,con);
                        command.Parameters.Add("@CUST_FILE_PATH",MySqlDbType.Text);
                        command.Parameters.Add("@CUST_USERNAME",MySqlDbType.Text);
                        command.Parameters.Add("@CUST_PASSWORD",MySqlDbType.Text);
                        command.Parameters.Add("@UPLOAD_DATE",MySqlDbType.VarChar,255);
                        command.Parameters.Add("@CUST_FILE_EXE",MySqlDbType.LongBlob);
                        command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                        command.Parameters["@CUST_FILE_PATH"].Value = getName;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;

                        Byte[] streamRead = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE_EXE"].Value = streamRead;

                        Icon retrieveExeIco = Icon.ExtractAssociatedIcon(open.FileName);
                        command.Parameters["@CUST_THUMB"].Value = retrieveExeIco;
                        if(command.ExecuteNonQuery() == 1) {
                            exeCurr++;
                            int top = 275;
                            int h_p = 100;
                            var panelTxt = new Guna2Panel() {
                                Name = "PanExe" + exeCurr,
                                Width = 240,
                                Height = 262,
                                BorderRadius = 8,
                                FillColor = ColorTranslator.FromHtml("#121212"),
                                BackColor = Color.Transparent,
                                Location = new Point(600, top)
                            };

                            top += h_p;
                            flowLayoutPanel1.Controls.Add(panelTxt);
                            var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanExe" + exeCurr]);

                            Label titleLab = new Label();
                            mainPanelTxt.Controls.Add(titleLab);
                            titleLab.Name = "LabVidUp" + vidCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                            titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                            titleLab.ForeColor = Color.Gainsboro;
                            titleLab.Visible = true;
                            titleLab.Enabled = true;
                            titleLab.Location = new Point(12, 182);
                            titleLab.Width = 220;
                            titleLab.Height = 30;
                            titleLab.Text = getName;

                            var textboxExe = new Guna2PictureBox();
                            mainPanelTxt.Controls.Add(textboxExe);
                            textboxExe.Name = "ExeBox" + exeCurr;
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
                            remButExe.Name = "RemExeBut" + exeCurr;
                            remButExe.Width = 39;
                            remButExe.Height = 35;
                            remButExe.FillColor = ColorTranslator.FromHtml("#4713BF");
                            remButExe.BorderRadius = 6;
                            remButExe.BorderThickness = 1;
                            remButExe.BorderColor = ColorTranslator.FromHtml("#232323");
                            remButExe.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                            remButExe.Visible = true;
                            remButExe.Location = new Point(189, 218);

                            remButExe.Click += (sender_exe, e_exe) => {
                                var titleFile = titleLab.Text;
                                DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (verifyDialog == DialogResult.Yes) {
                                    deletionMethod(titleFile,"file_info_exe");
                                    panelTxt.Dispose();
                                }

                                if(flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            };

                            Label dateLabExe = new Label();
                            mainPanelTxt.Controls.Add(dateLabExe);
                            dateLabExe.Name = "LabExeUp" + exeCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                            dateLabExe.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                            dateLabExe.ForeColor = Color.DarkGray;
                            dateLabExe.Visible = true;
                            dateLabExe.Enabled = true;
                            dateLabExe.Location = new Point(12, 208);
                            dateLabExe.Width = 1000;
                            dateLabExe.Text = varDate;

                            String countExeQue = "SELECT COUNT(CUST_USERNAME) FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                            command = new MySqlCommand(countExeQue,con);
                            command.Parameters.AddWithValue("@username",label5.Text);
                            command.Parameters.AddWithValue("@password", label3.Text);
                            var totalRow = command.ExecuteScalar();
                            var intTotalExeRow = Convert.ToInt32(totalRow);
                            label4.Text = intTotalExeRow.ToString();

                            label8.Visible = false;
                            guna2Button6.Visible = false;
                        } else {
                            MessageBox.Show("Adding file failed");
                        }
                    } else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi") {
                        con.Open();
                        increaseSizeMethod();    

                        String insertVidQue = "INSERT INTO file_info_vid(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE_VID,CUST_THUMB) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE_VID,@CUST_THUMB)";
                        command = new MySqlCommand(insertVidQue,con);
                        command.Parameters.Add("@CUST_FILE_PATH",MySqlDbType.Text);
                        command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                        command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                        command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                        command.Parameters.Add("@CUST_FILE_VID", MySqlDbType.LongBlob);
                        command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                        command.Parameters["@CUST_FILE_PATH"].Value = getName;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;

                        Byte[] streamReadVid = File.ReadAllBytes(open.FileName);
                        command.Parameters["@CUST_FILE_VID"].Value = streamReadVid;

                        ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                        Bitmap getThumbNail = shellFile.Thumbnail.Bitmap;
                        var setupThumb = ImageToByte(getThumbNail);
                        command.Parameters["@CUST_THUMB"].Value = setupThumb;

                        if (command.ExecuteNonQuery() == 1) {
                            vidCurr++;
                            int top = 275;
                            int h_p = 100;
                            var panelVid = new Guna2Panel() {
                                Name = "PanVid" + vidCurr,
                                Width = 240,
                                Height = 262,
                                BorderRadius = 8,
                                FillColor = ColorTranslator.FromHtml("#121212"),
                                BackColor = Color.Transparent,
                                Location = new Point(600, top)
                            };

                            top += h_p;
                            flowLayoutPanel1.Controls.Add(panelVid);
                            var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanVid" + vidCurr]);

                            Label titleLab = new Label();
                            mainPanelTxt.Controls.Add(titleLab);
                            titleLab.Name = "LabVidUp" + vidCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                            titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                            titleLab.ForeColor = Color.Gainsboro;
                            titleLab.Visible = true;
                            titleLab.Enabled = true;
                            titleLab.Location = new Point(12, 182);
                            titleLab.Width = 220;
                            titleLab.Height = 30;
                            titleLab.Text = getName;

                            // LOAD THUMBNAIL

                            var textboxVid = new Guna2PictureBox();
                            mainPanelTxt.Controls.Add(textboxVid);
                            textboxVid.Name = "ExeVid" + vidCurr;
                            textboxVid.Width = 240;
                            textboxVid.Height = 164;
                            textboxVid.FillColor = ColorTranslator.FromHtml("#232323");
                            textboxVid.Image = toBitMap;
                            textboxVid.SizeMode = PictureBoxSizeMode.CenterImage;
                            textboxVid.BorderRadius = 8;
                            textboxVid.Enabled = true;
                            textboxVid.Visible = true;

                            textboxVid.Click += (sender_vq, e_vq) => {
                                var getImgName = (Guna2PictureBox)sender_vq;
                                var getWidth = getImgName.Image.Width;
                                var getHeight = getImgName.Image.Height;
                                Bitmap defaultImage = new Bitmap(getImgName.Image);
                                vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight,titleLab.Text);
                                vidFormShow.Show();
                            };

                            Guna2Button remButVid = new Guna2Button();
                            mainPanelTxt.Controls.Add(remButVid);
                            remButVid.Name = "RemVidBut" + vidCurr;
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
                                    deletionMethod(titleFile,"file_info_vid");
                                    panelVid.Dispose();
                                }

                                if (flowLayoutPanel1.Controls.Count == 0) {
                                    label8.Visible = true;
                                    guna2Button6.Visible = true;
                                }
                            };

                            Label dateLabVid = new Label();
                            mainPanelTxt.Controls.Add(dateLabVid);
                            dateLabVid.Name = "LabVidUp" + vidCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                            dateLabVid.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                            dateLabVid.ForeColor = Color.DarkGray;
                            dateLabVid.Visible = true;
                            dateLabVid.Enabled = true;
                            dateLabVid.Location = new Point(12, 208);
                            dateLabVid.Width = 1000;
                            dateLabVid.Text = varDate;

                            label8.Visible = false;
                            guna2Button6.Visible = false;
                        }
                    }
                } 
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }

        public byte[] ImageToByte(Image imgIn) {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(imgIn, typeof(byte[]));
            return xByte;
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

        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {

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

        }

        private void pictureBox2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void pictureBox3_Click(object sender, EventArgs e) {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e) {

        }
    }
}
