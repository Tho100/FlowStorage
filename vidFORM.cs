﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using LibVLCSharp.Shared.MediaPlayerElement;
using LibVLCSharp.WinForms;
using LibVLCSharp.Shared;

namespace FlowSERVER1 {
    public partial class vidFORM : Form {
        public static vidFORM instance;
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;

        public LibVLC _libVLC;
        public MediaPlayer _mp;
        public static String _TableName;
        public static String _DirName;

        public vidFORM(Image getThumb, int width, int height, String getTitle,String tableName, String dirName) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            label3.Text = tableName;
            _TableName = tableName;
            _DirName = dirName;
        }

        public static Image resizeImage(Image userImg, Size size) {
            return (Image)(new Bitmap(userImg,size));
        }

        private void vidFORM_Load(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
            if(_mp != null) {
                _mp.Stop();
            } 
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
        }
        private void setupPlayer(Byte[] _retrieveBytesValue) {
            Stream _toStream = new MemoryStream(_retrieveBytesValue);
            _libVLC = new LibVLC();
            var media = new Media(_libVLC, new StreamMediaInput(_toStream));
            _mp = new MediaPlayer(media);
            videoView1.MediaPlayer = _mp;
            _mp.Play();
        }
        // Play
        private void guna2Button5_Click(object sender, EventArgs e) {
            try {
                if(_mp != null) {
                    videoView1.MediaPlayer = _mp;
                    _mp.Play();
                } else {
                    RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving video data..");
                    ShowAlert.Show();
                    Application.DoEvents();
                    if (_TableName == "upload_info_directory") {
                        String Select_VidByte = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(Select_VidByte, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname",_DirName);
                        command.Parameters.AddWithValue("@filename",label1.Text);

                        guna2PictureBox1.Visible = false;
                        videoView1.Visible = true;
                        List<Byte> _ByteValues = new List<Byte>();
                        MySqlDataReader _ReadBytes = command.ExecuteReader();
                        if (_ReadBytes.Read()) {
                            Application.OpenForms
                          .OfType<Form>()
                          .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                          .ToList()
                          .ForEach(form => form.Close());
                            var _retrieveBytesValue = (byte[])_ReadBytes["CUST_FILE"];
                            setupPlayer(_retrieveBytesValue);

                        }
                        _ReadBytes.Close();
                    } else if (_TableName == "file_info_vid"){
                        String Select_VidByte = "SELECT CUST_FILE FROM file_info_vid WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(Select_VidByte, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename",label1.Text);

                        guna2PictureBox1.Visible = false;
                        videoView1.Visible = true;
                        List<Byte> _ByteValues = new List<Byte>();
                        MySqlDataReader _ReadBytes = command.ExecuteReader();
                        if (_ReadBytes.Read()) {
                            Application.OpenForms
                           .OfType<Form>()
                           .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                           .ToList()
                           .ForEach(form => form.Close());
                            var _retrieveBytesValue = (byte[])_ReadBytes["CUST_FILE"];
                            setupPlayer(_retrieveBytesValue);
                        }
                        _ReadBytes.Close();
                    }
                    else if (_TableName == "folder_upload_info") {
                        String Select_VidByte = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";
                        command = new MySqlCommand(Select_VidByte, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", label1.Text);
                        command.Parameters.AddWithValue("@foldtitle", _DirName);

                        guna2PictureBox1.Visible = false;
                        videoView1.Visible = true;
                        List<Byte> _ByteValues = new List<Byte>();
                        MySqlDataReader _ReadBytes = command.ExecuteReader();
                        if (_ReadBytes.Read()) {
                            Application.OpenForms
                           .OfType<Form>()
                           .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                           .ToList()
                           .ForEach(form => form.Close());
                            var _retrieveBytesValue = (byte[])_ReadBytes["CUST_FILE"];
                            setupPlayer(_retrieveBytesValue);
                        }
                        _ReadBytes.Close();
                    }

                }

                guna2Button5.Visible = false;
                guna2Button6.Visible = true;
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
                //MessageBox.Show("Failed to play this file.","Flowstorage");
            }
        }

        private void pictureBox1_Click_2(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
            }
            else if (_TableName == "file_info_vid") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_vid", _DirName);
            }
        }

        private void videoView1_Click(object sender, EventArgs e) {

        }

        private void vlcControl1_Click(object sender, EventArgs e) {

        }

        private void videoView1_Click_1(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click_1(object sender, EventArgs e) {
            if(_mp != null) {
                _mp.Pause();
                guna2Button5.Visible = true;
                guna2Button6.Visible = false;
            }
        }
    }
}
