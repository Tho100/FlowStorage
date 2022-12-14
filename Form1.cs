﻿using MySql.Data.MySqlClient;
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

namespace FlowSERVER1 {
    public partial class Form1 : Form {
        public static Form1 instance;
        public Label setupLabel;

        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;

        public Form1() {
            InitializeComponent();

            con.Open();

            instance = this;
            setupLabel = label5;

            String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            String _getAuth = _getPath + "\\CUST_DATAS.txt";
            if (File.Exists(_getAuth)) {
                String _UsernameFirst = File.ReadLines(_getAuth).First();
                String _PassSed = File.ReadLines(_getAuth).ElementAtOrDefault(1);
                if (new FileInfo(_getAuth).Length != 0) {
                    guna2Panel7.Visible = false;
                    label5.Text = _PassSed;
                    label3.Text = _UsernameFirst;

                    setupTime();

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
                    //label4.Text = intTotalRowExe.ToString();

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

                    string countRowExcel = "SELECT COUNT(CUST_USERNAME) FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowExcel,con);
                    command.Parameters.AddWithValue("@username",label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    var totalRowExcel = command.ExecuteScalar();
                    int intTotalRowExcel = Convert.ToInt32(totalRowExcel);

                    string countRowAudi = "SELECT COUNT(CUST_USERNAME) FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowAudi,con);
                    command.Parameters.AddWithValue("@username",label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    var totalRowAudi = command.ExecuteScalar();
                    int intTotalRowAudi = Convert.ToInt32(totalRowAudi);

                    string countRowGif = "SELECT COUNT(CUST_USERNAME) FROM file_info_gif WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowGif, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    var totalRowGif = command.ExecuteScalar();
                    int intTotalRowGif = Convert.ToInt32(totalRowGif);

                    string countRowPdf = "SELECT COUNT(CUST_USERNAME) FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowPdf, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    var totalRowPdf = command.ExecuteScalar();
                    int intTotalRowPdf = Convert.ToInt32(totalRowPdf);

                    string countRowDirectory = "SELECT COUNT(CUST_USERNAME) FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(countRowDirectory, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    var totalRowDir = command.ExecuteScalar();
                    int intTotalRowDir = Convert.ToInt32(totalRowDir);

                    //string countRowUploadDir = "SELECT COUNT"

                    // LOAD IMG
                    if (intRow > 0) {
                        _generateUserFiles("file_info", "imgAtLoad", intRow);
                    }
                    // LOAD .TXT
                    if (intTotalRowTxt > 0) {
                        _generateUserFiles("file_info_expand", "txtAtLoad", intTotalRowTxt);
                    }
                    // LOAD EXE
                    if (intTotalRowExe > 0) {
                        _generateUserFiles("file_info_exe", "exeAtLoad", intTotalRowExe);
                    }
                    // LOAD VID
                    if (intTotalRowVid > 0) {
                        _generateUserFiles("file_info_vid", "vidAtLoad", intTotalRowVid);
                    }
                    if (intTotalRowExcel > 0) {
                        _generateUserFiles("file_info_excel", "exlAtLoad", intTotalRowExcel);
                    }
                    if (intTotalRowAudi > 0) {
                        _generateUserFiles("file_info_audi", "audiAtLoad", intTotalRowAudi);
                    }
                    if(intTotalRowGif > 0) {
                        _generateUserFiles("file_info_gif","gifAtLoad",intTotalRowGif);
                    }
                    if(intTotalRowPdf > 0) {
                        _generateUserFiles("file_info_pdf","pdfAtLoad",intTotalRowPdf);
                    }
                    if(intTotalRowDir > 0) {
                        //_generateUserFiles("file_info_directory","dirFile",intTotalRowDir);
                    }
                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                    // @ ADD FOLDERS TO LISTBOX

                    listBox1.Items.Add("Home");
                    listBox1.SelectedIndex = 0;

                    List<String> titleValues = new List<String>();

                    String getTitles = "SELECT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(getTitles, ConnectionModel.con);
                    command = ConnectionModel.con.CreateCommand();
                    command.CommandText = getTitles;

                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
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
            }
        }

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

                String getUpDate = "SELECT UPLOAD_DATE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(getUpDate, con);
                command = con.CreateCommand();
                command.CommandText = getUpDate;

                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
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

                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

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

                        String removeQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);
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

                    String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);

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
                    } else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                    } else if (_extTypes == ".js") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    }
                    picMain_Q.Click += (sender_t, e_t) => {
                        Form bgBlur = new Form();
                        using (txtFORM displayPic = new txtFORM("IGNORETHIS","file_info_expand",titleLab.Text)) {
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

                if (_tableName == "file_info_exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        exeFORM exeFormShow = new exeFORM(titleLab.Text);
                        exeFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    String getImgQue = "SELECT CUST_THUMB FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(getImgQue, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);

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
                        exlFORM exlForm = new exlFORM(titleLab.Text);
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

                if(_tableName == "file_info_gif") {
                    String getImgQue = "SELECT CUST_THUMB FROM file_info_gif WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(getImgQue,con);
                    command.Parameters.AddWithValue("@username",label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);

                    MySqlDataAdapter da_Read = new MySqlDataAdapter(command);
                    DataSet ds_Read = new DataSet();
                    da_Read.Fill(ds_Read);
                    MemoryStream ms = new MemoryStream((byte[])ds_Read.Tables[0].Rows[i]["CUST_THUMB"]);
                    img.Image = new Bitmap(ms);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayPic = new gifFORM(titleLab.Text)) {
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
                
               if(_tableName == "file_info_apk") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        using (apkFORM displayPic = new apkFORM(titleLab.Text,label5.Text)) {
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

               if(_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        using (pdfFORM displayPdf = new pdfFORM(titleLab.Text,"file_info_pdf")) {
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

               if(_tableName == "file_info_ptx") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
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
                    clearRedundane();
                }
            }
        }
        public void _generateUserFold(List<String> _fileType,String _foldTitle, String parameterName, int currItem) {
            flowLayoutPanel1.Controls.Clear();
            List<String> typeValues = new List<String>(_fileType);
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

                String getUpDate = "SELECT UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(getUpDate, con);
                command = con.CreateCommand();
                command.CommandText = getUpDate;

                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                command.Parameters.AddWithValue("@foldername",_foldTitle);
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

                String getTitleQue = "SELECT CUST_FILE_PATH FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(getTitleQue, con);
                command = con.CreateCommand();
                command.CommandText = getTitleQue;

                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
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

                        String removeQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@password", label3.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.Parameters.AddWithValue("@foldername",_foldTitle);
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
                if (typeValues[i] == ".png" || typeValues[i] == ".jpeg" || typeValues[i] == ".jpg" || typeValues[i] == ".bmp") {
                    String retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);

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

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage,getWidth,getHeight,titleLab.Text)) {
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
                if(typeValues[i] == ".txt" || typeValues[i] == ".py" || typeValues[i] == ".html" || typeValues[i] == ".css" || typeValues[i] == ".js") {
                    String retrieveImg = "SELECT CONVERT(CUST_FILE USING utf8) FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@password", label3.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    command.Parameters.AddWithValue("@filename", titleLab.Text);

                    List<String> textValues_ = new List<String>();

                    MySqlDataReader _ReadTexts = command.ExecuteReader();
                    while(_ReadTexts.Read()) {
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

                    picMain_Q.Click += (sender_t, e_t) => {
                        Form bgBlur = new Form();
                        using (txtFORM displayPic = new txtFORM("","file_info_expand", titleLab.Text)) {
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

                if(typeValues[i] == ".xlsx") {
                    //
                }

                if(typeValues[i] == ".apk") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    img.Click += (sender_ap, e_ap) => {
                        Form bgBlur = new Form();
                        using (apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text)) {
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

                if(typeValues[i] == ".pdf") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    img.Click += (sender_pdf, e_pdf) => {
                        Form bgBlur = new Form();
                        using (pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info")) {
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
                string[] morningKeys = { "start your day with a coffee?", "" };
                var random = new Random();
                var getKeyRand = random.Next(0, 1);
                var getMorningKeys = morningKeys[getKeyRand];
                DateTime now = DateTime.Now;
                var hours = now.Hour;
                String greeting = null;
                if (hours >= 1 && hours <= 12) {
                    greeting = "Good Morning " + label5.Text + " :) " + getMorningKeys;
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
                    greeting = "Good Evening " + label5.Text + " :)";
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
        private void guna2Button2_Click(object sender, EventArgs e) {
                
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
                String setupPacketMax = "SET GLOBAL max_allowed_packet=2000000000000000000;"; // +5
                command = new MySqlCommand(setupPacketMax, con);
                command.ExecuteNonQuery();
            }

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp|Icon|*.ico|Video Files|*.mp4;*.webm;.mov|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;|Powerpoint Files|*.pptx;*.ppt|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|APK Files|*.apk|Acrobat Files|*.pdf";
            string varDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (open.ShowDialog() == DialogResult.OK) {

                void clearRedundane() {
                    label8.Visible = false;
                    guna2Button6.Visible = false;
                }

                void containThumbUpload(String nameTable, String getNamePath, Object keyValMain) {

                    String insertThumbQue = "INSERT INTO " + nameTable + "(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE,CUST_THUMB) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE,@CUST_THUMB)";
                    command = new MySqlCommand(insertThumbQue, con);

                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);

                    command.Parameters["@CUST_FILE_PATH"].Value = getNamePath;
                    command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                    command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                    command.Parameters["@UPLOAD_DATE"].Value = varDate;

                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                    command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);

                    command.Parameters["@CUST_FILE"].Value = keyValMain;

                    ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;

                    using (var stream = new MemoryStream()) {
                        toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        command.Parameters["@CUST_THUMB"].Value = stream.ToArray();// To load: Bitmap -> Byte array
                    }
                    command.ExecuteNonQuery();
                }
                        
                string get_ex = open.FileName;
                string getName = open.SafeFileName;
                string retrieved = System.IO.Path.GetExtension(get_ex);
                string retrievedName = System.IO.Path.GetFileNameWithoutExtension(open.FileName);

                void createPanelMain(String nameTable,String panName,int itemCurr,Object keyVal) {
                    String insertTxtQuery = "INSERT INTO " + nameTable + "(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE)";
                    command = new MySqlCommand(insertTxtQuery, con);

                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);

                    command.Parameters["@CUST_FILE_PATH"].Value = getName;
                    command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                    command.Parameters["@CUST_PASSWORD"].Value = label3.Text;//EncryptionModel.Encrypt(label3.Text,"ABHABH24");//label3.Text;
                    command.Parameters["@UPLOAD_DATE"].Value = varDate;

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

                    if (nameTable == "file_info") {
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();

                        label4.Text = (Convert.ToInt32(label4.Text) + 1).ToString();

                        textboxPic.Image = new Bitmap(open.FileName);
                        textboxPic.Click += (sender_f,e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            //picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName);
                            //displayPic.Show();

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage,getWidth,getHeight,getName)) {
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

                    if(nameTable == "file_info_expand") {
                        var encryptValue = EncryptionModel.Encrypt(keyVal.ToString(),"MAINKEY9999");
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = encryptValue;
                        command.ExecuteNonQuery();

                        var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                        if (_extTypes == ".py") {
                            textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                        } else if (_extTypes == ".txt") {
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

                        String nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(open.FileName)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }

                        var filePath = open.SafeFileName;

                        textboxPic.Click += (sender_t, e_t) => {
                            Form bgBlur = new Form();
                            using (txtFORM txtFormShow = new txtFORM("IGNORETHIS","file_info_expand",filePath)) {
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

                                txtFormShow.Owner = bgBlur;
                                txtFormShow.ShowDialog();

                                bgBlur.Dispose();
                            }

                        };
                        clearRedundane();
                    }

                    if(nameTable == "file_info_exe") {
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();

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
                        clearRedundane();
                    }

                    if (nameTable == "file_info_vid") {
                        containThumbUpload(nameTable,getName,keyVal);
                        ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        textboxPic.Image = toBitMap;

                        textboxPic.Click += (sender_ex, e_ex) => {
                            var getImgName = (Guna2PictureBox)sender_ex;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);

                            vidFORM vidShow = new vidFORM(defaultImg,getWidth,getHeight,titleLab.Text,open.FileName);
                            vidShow.Show();
                        };
                        clearRedundane();
                    }
                    if(nameTable == "file_info_audi") {
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();

                        textboxPic.Image = Image.FromFile(@"C:\users\USER\Downloads\icons8-audio-file-52.png");
                        textboxPic.Click += (sender_ex, e_ex) => {
                            audFORM vidShow = new audFORM(titleLab.Text);
                            vidShow.Show();
                        };
                        clearRedundane();
                    }
                    if(nameTable == "file_info_gif") {
                        containThumbUpload(nameTable,getName,keyVal);
                        ShellFile shellFile = ShellFile.FromFilePath(open.FileName);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        textboxPic.Image = toBitMap;

                        textboxPic.Click += (sender_gi, e_gi) => {
                            Form bgBlur = new Form();
                            using (gifFORM displayPic = new gifFORM(titleLab.Text)) {
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
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();

                        Byte[] _getApkBytes = File.ReadAllBytes(open.FileName);
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");

                        textboxPic.Click += (sender_gi, e_gi) => {
                            Form bgBlur = new Form();
                            using (apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text)) {
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
                    if(nameTable == "file_info_pdf") {
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();

                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        textboxPic.Click += (sender_pd, e_pd) => {
                            Form bgBlur = new Form();
                            using (pdfFORM displayPdf = new pdfFORM(titleLab.Text,"file_info_pdf")) {
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
                    if(nameTable == "file_info_ptx") {
                        command.Parameters.Add("@CUST_FILE",MySqlDbType.LongBlob);
                        command.Parameters["@CUST_FILE"].Value = keyVal;
                        command.ExecuteNonQuery();
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxPic.Click += (sender_ptx, e_ptx) => {
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
                        clearRedundane();
                    }

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

                    increaseSizeMethod();
                }

                if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico" || retrieved == ".bmp" || retrieved == ".svg") {
                    curr++;
                    var getImg = new Bitmap(open.FileName);
                    var imgWidth = getImg.Width;
                    var imgHeight = getImg.Height;
                    if (retrieved != ".ico") {
                        using(MemoryStream ms = new MemoryStream()) {
                            getImg.Save(ms,System.Drawing.Imaging.ImageFormat.Png);
                            var setupImg = ms.ToArray();
                            createPanelMain("file_info","PanImg",curr,setupImg);
                        }
                    }
                    else {
                        Image retrieveIcon = Image.FromFile(open.FileName);
                        byte[] dataIco;
                        using(MemoryStream msIco = new MemoryStream()) {
                            retrieveIcon.Save(msIco,System.Drawing.Imaging.ImageFormat.Png);
                            dataIco = msIco.ToArray();
                            createPanelMain("file_info","PanImg", curr, dataIco);
                        }
                    }
                } else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml" || retrieved == ".py" || retrieved == ".css" || retrieved == ".js") {
                    txtCurr++;
                    String nonLine = "";
                    using (StreamReader ReadFileTxt = new StreamReader(open.FileName)) {
                        nonLine = ReadFileTxt.ReadToEnd();
                    }
                    createPanelMain("file_info_expand","PanTxt",txtCurr,nonLine);
                } else if (retrieved == ".exe") {
                    exeCurr++;
                    Byte[] streamRead = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_exe","PanExe",exeCurr,streamRead);
                } else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi") { 
                    vidCurr++;
                    Byte[] streamReadVid = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_vid","PanVid",vidCurr,streamReadVid);
                } else if (retrieved == ".xlsx" || retrieved == ".csv") {
                    //String pathExl = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + open.FileName + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                    String pathExl = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + open.FileName + ";Extended Properties = \"Excel 12.0 Xml;HDR=YES\";";
                    OleDbConnection _conOledb = new OleDbConnection(pathExl);
                    _conOledb.Open();

                    DataTable Sheets = _conOledb.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        
                    List<string> sheetsValues = new List<string>();
                    List<string> fixedSheetValues = new List<string>();

                    /*
                        *                         for (int i = 0; i < Sheets.Rows.Count; i++) {
                        string worksheets = Sheets.Rows[i]["TABLE_NAME"].ToString();
                        string sqlQuery = String.Format("SELECT * FROM [{0}]", worksheets);
                        sheetsValues.Add(sqlQuery);
                    }

                    foreach (var item in sheetsValues) {
                        var output = String.Join(";", Regex.Matches(item, @"\[(.+?)\$")
                                                            .Cast<Match>()
                                                            .Select(m => m.Groups[1].Value));
                        fixedSheetValues.Add(output);
                    }
                        */

                    String insertXML = "INSERT INTO file_info_excel(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE,SHEET_NAME) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE,@SHEET_NAME)";
                    command = new MySqlCommand(insertXML, con);
                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongText);
                    command.Parameters.Add("@SHEET_NAME",MySqlDbType.LongText);

                    List<String> sheetNames_Values = new List<String>();
                    DataTable dt_ = _conOledb.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    for (int i = 0; i < dt_.Rows.Count; i++) {
                        String sheetName = dt_.Rows[i]["TABLE_NAME"].ToString();
                        sheetName = sheetName.Substring(0, sheetName.Length - 1);
                        sheetNames_Values.Add(sheetName);

                        sheetNames_Values[i] = sheetNames_Values[i].Replace("$","");

                        command.Parameters["@SHEET_NAME"].Value = sheetName;
                        command.Parameters["@CUST_FILE_PATH"].Value = getName;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;

                        /*OleDbCommand oleCmd = new OleDbCommand();
                        DataTable dtCfg = new DataTable{TableName = "ds" + i};
                        oleCmd.Connection = conExl;
                        oleCmd.CommandType = CommandType.Text;
                        oleCmd.CommandText = "SELECT * FROM [" + sheetNames_Values[i] + "$]";
                        OleDbDataAdapter oleDA = new OleDbDataAdapter(oleCmd);
                        oleDA.Fill(dtCfg);
                        conExl.Close();

                        StringWriter sm = new StringWriter();
                        dtCfg.WriteXml(sm);
                        string resultXML = sm.ToString();*/

                        //                            command.Parameters["@CUST_FILE"].Value = resultXML;

                        OleDbDataAdapter ole_Adapter = new OleDbDataAdapter("Select * from [" + sheetNames_Values[i] + "$]",_conOledb);
                        DataTable _dtOle = new DataTable();
                        ole_Adapter.Fill(_dtOle);

//                            command.ExecuteNonQuery();

                    }

                    /*for (int sheetI=0; sheetI<=fixedSheetValues.Count(); sheetI++) {
                        MessageBox.Show(fixedSheetValues.Count().ToString());
                        //var cmd = new OleDbCommand("select * from [" + fixedSheetValues[sheetI] + "$]", conExl);
                        var dtSchema = conExl.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                        var getSheetsNames = dtSchema.Rows[sheetI].Field<string>("TABLE_NAME");
                        MessageBox.Show(getSheetsNames);
                        /*var ds = new DataSet();
                        var da = new OleDbDataAdapter(dtSchema);
                        da.Fill(ds);

                        StringWriter sm = new StringWriter();
                        ds.WriteXml(sm);
                        string resultXML = sm.ToString();

                        command.Parameters["@CUST_FILE_PATH"].Value = getName;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;
                        command.Parameters["@CUST_FILE"].Value = resultXML;

                        if(command.ExecuteNonQuery() == 1) {
                            clearRedundane();
                        }
                }*/

                    exlCurr++;
                    int top = 275;
                    int h_p = 100;
                    var panelVid = new Guna2Panel() {
                        Name = "PanExl" + exlCurr,
                        Width = 240,
                        Height = 262,
                        BorderRadius = 8,
                        FillColor = ColorTranslator.FromHtml("#121212"),
                        BackColor = Color.Transparent,
                        Location = new Point(600, top)
                    };

                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelVid);
                    var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls["PanExl" + exlCurr]);

                    Label titleLab = new Label();
                    mainPanelTxt.Controls.Add(titleLab);
                    titleLab.Name = "LabExlUp" + exlCurr;//Segoe UI Semibold, 11.25pt, style=Bold
                    titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                    titleLab.ForeColor = Color.Gainsboro;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = new Point(12, 182);
                    titleLab.Width = 220;
                    titleLab.Height = 30;
                    titleLab.Text = getName;

                    // LOAD THUMBNAIL

                    var textboxExl = new Guna2PictureBox();
                    mainPanelTxt.Controls.Add(textboxExl);
                    textboxExl.Name = "ExeExl" + exlCurr;
                    textboxExl.Width = 240;
                    textboxExl.Height = 164;
                    textboxExl.FillColor = ColorTranslator.FromHtml("#232323");
                    textboxExl.Image = FlowSERVER1.Properties.Resources.excelIcon;//Image.FromFile(@"C:\Users\USER\Downloads\excelIcon.png");
                    textboxExl.SizeMode = PictureBoxSizeMode.CenterImage;
                    textboxExl.BorderRadius = 8;
                    textboxExl.Enabled = true;
                    textboxExl.Visible = true;

                    textboxExl.MouseHover += (_senderM, _ev) => {
                        mainPanelTxt.ShadowDecoration.Enabled = true;
                        mainPanelTxt.ShadowDecoration.BorderRadius = 8;
                    };

                    textboxExl.MouseLeave += (_senderQ, _evQ) => {
                        mainPanelTxt.ShadowDecoration.Enabled = false;
                    };

                    textboxExl.Click += (sender_eq, e_eq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text);
                        exlForm.Show();
                    };

                    Guna2Button remButExl = new Guna2Button();
                    mainPanelTxt.Controls.Add(remButExl);
                    remButExl.Name = "RemExlBut" + exlCurr;
                    remButExl.Width = 39;
                    remButExl.Height = 35;
                    remButExl.FillColor = ColorTranslator.FromHtml("#4713BF");
                    remButExl.BorderRadius = 6;
                    remButExl.BorderThickness = 1;
                    remButExl.BorderColor = ColorTranslator.FromHtml("#232323");
                    remButExl.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                    remButExl.Visible = true;
                    remButExl.Location = new Point(189, 218);

                    remButExl.Click += (sender_vid, e_vid) => {
                        var titleFile = titleLab.Text;
                        DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (verifyDialog == DialogResult.Yes) {
                            deletionMethod(titleFile, "file_info_excel");
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
                    dateLabExl.Name = "LabExlUp" + exlCurr;
                    dateLabExl.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                    dateLabExl.ForeColor = Color.DarkGray;
                    dateLabExl.Visible = true;
                    dateLabExl.Enabled = true;
                    dateLabExl.Location = new Point(12, 208);
                    dateLabExl.Width = 1000;
                    dateLabExl.Text = varDate;

                } else if (retrieved == ".mp3" || retrieved == ".mpeg" || retrieved == ".wav") {
                    audCurr++;
                    Byte[] toByte_ = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_audi","PanAud",audCurr,toByte_);

                }
                else if (retrieved == ".gif") {
                    gifCurr++;
                    Byte[] toByteGif_ = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_gif","PanGif",gifCurr,toByteGif_);
                }
                else if (retrieved == ".apk") {
                    apkCurr++;
                    Byte[] readApkBytes = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_apk","PanApk",apkCurr,readApkBytes);
                }
                else if (retrieved == ".pdf") {
                    pdfCurr++;
                    Byte[] readPdfBytes = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_pdf","PanPdf",pdfCurr,readPdfBytes);
                }
                else if (retrieved == ".pptx" || retrieved == ".ppt") {
                    ptxCurr++;
                    Byte[] readPtxBytes = File.ReadAllBytes(open.FileName);
                    createPanelMain("file_info_ptx","PanPtx",ptxCurr,readPtxBytes);
                }
                label4.Text = flowLayoutPanel1.Controls.Count.ToString();
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
            remAccFORM _RemAccShow = new remAccFORM(label5.Text);
            _RemAccShow.Show();
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

        }

        private void pictureBox2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void pictureBox3_Click(object sender, EventArgs e) {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            Control flowlayout = Form1.instance.flowLayoutPanel1;
            String _getUser = guna2TextBox1.Text;
            String _getPass = guna2TextBox2.Text;
            String _getEmail = guna2TextBox3.Text;

            String verfiyUserQue = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            command = con.CreateCommand();
            command.CommandText = verfiyUserQue;
            command.Parameters.AddWithValue("@username", _getUser);

            List<String> userExists = new List<String>();
            List<String> emailExists = new List<String>();

            MySqlDataReader userReader = command.ExecuteReader();
            while (userReader.Read()) {
                userExists.Add(userReader.GetString(0));
            }

            userReader.Close();

            String verifyEmailQue = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
            command = con.CreateCommand();
            command.CommandText = verifyEmailQue;
            command.Parameters.AddWithValue("@email",_getEmail);
            
            MySqlDataReader emailReader = command.ExecuteReader();
            while(emailReader.Read()) {
                emailExists.Add(emailReader.GetString(0));
            }
            emailReader.Close();

            if(emailExists.Count() >= 1 || userExists.Count() >= 1) {
                if(emailExists.Count() >= 1) {
                    label22.Visible = true;
                    label22.Text = "Email already exists.";
                }
                if(userExists.Count() >= 1) {
                    label11.Visible = true;
                    label11.Text = "Username is taken.";
                }
            }
            else {
                label22.Visible = false;
                label12.Visible = false;
                label11.Visible = false;
                if(_getUser.Length <= 15) {
                    if(_getPass.Length > 5) {
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

                                    var encryptPassVal = EncryptionModel.Encrypt(_getPass, "ABHABH24");//EncryptionModel.Encrypt(label3.Text,"ABHABH24");
                                    setupLabel.Text = _getUser;
                                    label3.Text = encryptPassVal;
                                    encryptedPassKey = encryptPassVal;

                                    String _getDate = DateTime.Now.ToString("MM/dd/yyyy");

                                    string query = "INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL) VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL)";
                                    using (var cmd = new MySqlCommand(query, con)) {
                                        cmd.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                        cmd.Parameters.AddWithValue("@CUST_PASSWORD", encryptPassVal);
                                        cmd.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                                        cmd.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                        cmd.ExecuteNonQuery();
                                    }

                                    label11.Visible = false;
                                    label12.Visible = false;
                                    guna2Panel7.Visible = false;
                                    guna2TextBox1.Text = String.Empty;
                                    guna2TextBox2.Text = String.Empty;
                                    guna2TextBox3.Text = String.Empty;
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
                        }
                    } else {
                        label12.Visible = true;
                        label12.Text = "Password must be longer than 5 characters.";
                    }
                } else {
                    label11.Visible = true;
                    label11.Text = "Username character length limit is 15.";
                }
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
        String encryptedPassKey;
        private void guna2Button12_Click(object sender, EventArgs e) {
            String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);
            try {

                void deletionFoldFile(String _Username, String _fileName, String _foldTitle) {
                    String _remQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(_remQue,con);
                    command.Parameters.AddWithValue("@username",_Username);
                    command.Parameters.AddWithValue("@foldtitle", _foldTitle);
                    command.Parameters.AddWithValue("@filename", _fileName);
                    if(command.ExecuteNonQuery() != 1) {
                        MessageBox.Show("There's an unknown error while attempting to delete this file.","Erorr");
                    } 
                }

                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                    var _getDirPath = dialog.FileName;
                    int _countFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.TopDirectoryOnly).Length;
                    var _getDirTitle = new DirectoryInfo(_getDirPath).Name;
                    listBox1.Items.Add(_getDirTitle);
                    flowLayoutPanel1.Controls.Clear();

                    String insertFoldQue_ = "INSERT INTO folder_upload_info(FOLDER_TITLE,CUST_USERNAME,CUST_PASSWORD,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH) VALUES (@FOLDER_TITLE,@CUST_USERNAME,@CUST_PASSWORD,@CUST_FILE,@FILE_TYPE,@UPLOAD_DATE,@CUST_FILE_PATH)";
                    command = new MySqlCommand(insertFoldQue_,con);

                    command.Parameters.Add("@FOLDER_TITLE", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                    command.Parameters.Add("@FILE_TYPE",MySqlDbType.VarChar,15);
                    command.Parameters.Add("@UPLOAD_DATE",MySqlDbType.Text);
                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);
                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);

                    String[] _TitleValues = Directory.GetFiles(_getDirPath,"*").Select(Path.GetFileName).ToArray();
                    
                    int _IntCurr = 0;
                    foreach (var _Files in Directory.EnumerateFiles(_getDirPath,"*")) {
                        String varDate = DateTime.Now.ToString("dd/MM/yyyy");
                        command.Parameters["@FOLDER_TITLE"].Value = _getDirTitle;
                        command.Parameters["@CUST_USERNAME"].Value = label5.Text;
                        command.Parameters["@CUST_PASSWORD"].Value = label3.Text;
                        command.Parameters["@CUST_FILE_PATH"].Value = Path.GetFileName(_Files);
                        command.Parameters["@FILE_TYPE"].Value = Path.GetExtension(_Files);
                        command.Parameters["@UPLOAD_DATE"].Value = varDate;
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
                                deletionFoldFile(label5.Text,titleLab.Text,label26.Text);
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

                        var _extTypes = Path.GetExtension(_Files);
                        if(_extTypes == ".png" || _extTypes == ".jpg" || _extTypes == ".jpeg" || _extTypes == ".bmp") {
                            var _imgContent = new Bitmap(_Files);
                            using(MemoryStream _ms = new MemoryStream()) {
                                _imgContent.Save(_ms,System.Drawing.Imaging.ImageFormat.Png);
                                var _setupImg = _ms.ToArray();
                                command.Parameters["@CUST_FILE"].Value = _setupImg;
                            }
                            textboxExl.Image = _imgContent;
                            textboxExl.Click += (sender_f, e_f) => {
                                var getImgName = (Guna2PictureBox)sender_f;
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
                            if (command.ExecuteNonQuery() == 1) {
                                clearRedundane();
                            } 
                        }
                        if(_extTypes == ".txt" || _extTypes == ".py" || _extTypes == ".html" || _extTypes == ".css" || _extTypes == ".js") {
                            // TXTCONTS = TEXT CONTENTS
                            if(_extTypes == ".py") {
                                textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                            } else if (_extTypes == ".txt") {
                                textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                            } else if (_extTypes == ".html") {
                                textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                            }
                            else if (_extTypes == ".css") {
                                textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                            } else if (_extTypes == ".js") {
                                textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                            }

                            var _encryptConts = EncryptionModel.Encrypt(File.ReadAllText(_Files),"TXTCONTS");
                            var _readText = File.ReadAllText(_Files);
                            textboxExl.Click += (sender_t, e_t) => {
                                Form bgBlur = new Form();
                                using (txtFORM displayPic = new txtFORM("","folder_upload_info",titleLab.Text)) {
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
                            command.Parameters["@CUST_FILE"].Value = _encryptConts; // Receive text
                            if (command.ExecuteNonQuery() == 1) {
                                clearRedundane();
                            }
                        }
                        if(_extTypes == ".apk") {
                            Byte[] _readApkBytes = File.ReadAllBytes(_Files);
                            command.Parameters["@CUST_FILE"].Value = _readApkBytes;
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                            textboxExl.Click += (sender_ap, e_ap) => {
                                Form bgBlur = new Form();
                                using (apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text)) {
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
                            if(command.ExecuteNonQuery() == 1) {
                                clearRedundane();
                            }
                        }
                        if(_extTypes == ".pdf") {
                            Byte[] readPdfBytes = File.ReadAllBytes(_Files);
                            command.Parameters["@CUST_FILE"].Value = readPdfBytes;
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                            textboxExl.Click += (sender_pdf, e_pdf) => {
                                Form bgBlur = new Form();
                                using (pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info")) {
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
                            if(command.ExecuteNonQuery() == 1) {
                                clearRedundane();
                            }
                        }
                    }
                    var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                    listBox1.SelectedIndex = _dirPosition;
                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                }
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
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

            if(_selectedFolder == "Home") {
                foldCurr++;
                guna2Button19.Visible = false;
                flowLayoutPanel1.Controls.Clear();

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
                //label4.Text = intTotalRowExe.ToString();

                string countRowVid = "SELECT COUNT(CUST_USERNAME) FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowVid, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

                var totalRowVid = command.ExecuteScalar();
                int intTotalRowVid = Convert.ToInt32(totalRowVid);

                String countRow = "SELECT COUNT(CUST_USERNAME) FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRow, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRow = command.ExecuteScalar();
                var intRow = Convert.ToInt32(totalRow);
                //label6.Text = intRow.ToString();

                string countRowExcel = "SELECT COUNT(CUST_USERNAME) FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowExcel, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowExcel = command.ExecuteScalar();
                int intTotalRowExcel = Convert.ToInt32(totalRowExcel);

                string countRowAudi = "SELECT COUNT(CUST_USERNAME) FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowAudi, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowAudi = command.ExecuteScalar();
                int intTotalRowAudi = Convert.ToInt32(totalRowAudi);

                string countRowGif = "SELECT COUNT(CUST_USERNAME) FROM file_info_gif WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowGif, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowGif = command.ExecuteScalar();
                int intTotalRowGif = Convert.ToInt32(totalRowGif);

                string countRowApk = "SELECT COUNT(CUST_USERNAME) FROM file_info_apk WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowApk, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowApk = command.ExecuteScalar();
                int intTotalRowApk = Convert.ToInt32(totalRowApk);

                string countRowDirectory = "SELECT COUNT(CUST_USERNAME) FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowDirectory, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowDir = command.ExecuteScalar();
                int intTotalRowDir = Convert.ToInt32(totalRowDir);

                string countRowPdf = "SELECT COUNT(CUST_USERNAME) FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowPdf, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowPdf = command.ExecuteScalar();
                int intTotalRowPdf = Convert.ToInt32(totalRowPdf);

                string countRowPtx = "SELECT COUNT(CUST_USERNAME) FROM file_info_ptx WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRowPtx, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);
                var totalRowPtx = command.ExecuteScalar();
                int intTotalRowPtx = Convert.ToInt32(totalRowPtx);

                if (intRow > 0) {
                    _generateUserFiles("file_info","imageFoldHome",intRow);
                }
                if (intTotalRowTxt > 0) {
                    _generateUserFiles("file_info_expand", "txtFoldHome", intTotalRowTxt);
                }
                if(intTotalRowGif > 0) {
                    _generateUserFiles("file_info_gif","gifFoldHome",intTotalRowGif);
                }
                if (intTotalRowVid > 0) {
                    _generateUserFiles("file_info_vid", "vidFoldHome", intTotalRowVid);
                }
                if(intTotalRowApk > 0) {
                    _generateUserFiles("file_info_apk","apkFoldHome",intTotalRowApk);
                }
                if(intTotalRowDir > 0) {
                    _generateUserDirectory(label5.Text,label3.Text,intTotalRowDir);
                }
                if(intTotalRowPdf > 0) {
                    _generateUserFiles("file_info_pdf","pdfFoldHome",intTotalRowPdf);
                }
                if(intTotalRowPtx > 0) {
                    _generateUserFiles("file_info_ptx","ptxFoldHome",intTotalRowPtx);
                }

                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                } else {
                    clearRedundane();
                }
                label4.Text = flowLayoutPanel1.Controls.Count.ToString();

            } else if(_selectedFolder != "Home") {
                mainFoldCurr++;
                guna2Button19.Visible = true;
                flowLayoutPanel1.Controls.Clear();

                String getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(getFileType,con);
                command = con.CreateCommand();
                command.CommandText = getFileType;
                command.Parameters.AddWithValue("@username",label5.Text);
                command.Parameters.AddWithValue("@password",label3.Text);
                command.Parameters.AddWithValue("@foldername", _selectedFolder);
                MySqlDataReader _readType = command.ExecuteReader();

                List<string> typesValues = new List<string>();

                while (_readType.Read()) {
                    typesValues.Add(_readType.GetString(0));// Append ToAr;
                }  
                _readType.Close();
                List<String> mainTypes = typesValues.Distinct().ToList();
                var currMainLength = typesValues.Count;
                _generateUserFold(typesValues,_selectedFolder,"TESTING",currMainLength); // fold_naem parname filetype curr
                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                } else {
                    clearRedundane();
                }
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        public void _removeFoldFunc(String foldName) {

            String removeFoldQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
            command = new MySqlCommand(removeFoldQue,con);
            command.Parameters.AddWithValue("@username",label5.Text);
            command.Parameters.AddWithValue("@foldername", foldName);
            command.ExecuteNonQuery();
                
            listBox1.Items.Remove(foldName);
            foreach(var _DupeItem in listBox1.Items) {
                if(_DupeItem.ToString() == foldName) {
                    listBox1.Items.Remove(_DupeItem.ToString());
                }
            }

            int indexSelected = listBox1.Items.IndexOf("Home");
            listBox1.SelectedIndex = indexSelected;
        }

        // REMOVE DIR BUT
        private void guna2Button19_Click(object sender, EventArgs e) {
            String _currentFold = listBox1.GetItemText(listBox1.SelectedItem);
            _removeFoldFunc(_currentFold);            
        }

        void _generateUserDirectory(String userName, String passUser, int rowLength) {
            for (int i = 0; i < rowLength-rowLength+1; i++) {
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

                String getTitleQue = "SELECT DIR_NAME FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(getTitleQue, con);
                command = con.CreateCommand();
                command.CommandText = getTitleQue;

                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@password", label3.Text);

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

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                    }
                };

                picMain_Q.Image = FlowSERVER1.Properties.Resources.icon1;
                picMain_Q.Click += (sender_dir, ev_dir) => {
                    Form bgBlur = new Form();
                    using (Form3 displayDirectory = new Form3(titleLab.Text)) {
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

                        displayDirectory.Owner = bgBlur;
                        displayDirectory.ShowDialog();

                        bgBlur.Dispose();
                    }
                };
            }
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
    }
}
