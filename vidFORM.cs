using System;
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
using System.Threading;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class vidFORM : Form {
        public static vidFORM instance;
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;

        private LibVLC _libVLC;
        private MediaPlayer _mp;
        private static String _TableName;
        private static String _DirName;
        private static String _UploaderName;
        private static bool _IsFromShared;
        private static bool IsFromSharing;

        public vidFORM(Image getThumb, int width, int height, String getTitle,String tableName, String dirName,String uploaderName, bool _isFromShared = false,bool _isFromSharing = true) {
            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(uploaderName, @"^([\w\-]+)").Value == "Shared";

            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width, height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
            label3.Text = tableName;
            _TableName = tableName;
            _DirName = dirName;
            _UploaderName = uploaderName;
            _IsFromShared = _isFromShared;
            IsFromSharing = _isFromSharing;

            if (_isShared == true) {
                _getName = _UploaderName;
                guna2Button7.Visible = false;
                label4.Visible = true;
                label4.Text = getCommentSharedToOthers() != "" ? "Comment: '" + getCommentSharedToOthers() + "'" : "Comment: (No Comment)";
            }
            else {
                _getName = "Uploaded By " + _UploaderName;
                label4.Visible = true;
                label4.Text = getCommentSharedToMe() != "" ? "Comment: '" + getCommentSharedToMe() + "'" : "Comment: (No Comment)";
            }

            label2.Text = _getName;
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }

        private string getCommentSharedToOthers() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }
        public static Image resizeImage(Image userImg, Size size) {
            return (Image)(new Bitmap(userImg,size));
        }

        private void vidFORM_Load(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            if(_mp != null) {
                _mp.Stop();
            } 
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            label1.AutoSize = false;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            label1.AutoSize = true;
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

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving video data..", "Loader").ShowDialog());
                    ShowAlert.Start();
                        
                    guna2PictureBox1.Visible = false;
                    videoView1.Visible = true;
                    if (_TableName == "upload_info_directory") {
                        setupPlayer(LoaderModel.LoadFile("upload_info_directory",_DirName,label1.Text));
                    }
                     else if (_TableName == "file_info_vid"){
                        setupPlayer(LoaderModel.LoadFile("file_info_vid", _DirName, label1.Text));
                    } else if (_TableName == "folder_upload_info") {
                        setupPlayer(LoaderModel.LoadFile("folder_upload_info", _DirName, label1.Text));
                    } else if (_TableName == "cust_sharing") {
                        setupPlayer(LoaderModel.LoadFile("cust_sharing", _DirName, label1.Text,_IsFromShared));
                    }
                }

                guna2Button5.Visible = false;
                guna2Button6.Visible = true;

            } catch (Exception) {
                MessageBox.Show("Failed to play this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName,_IsFromShared);
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

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            string[] parts = label1.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirName);
            _showSharingFileFORM.Show();
        }
    }
}
