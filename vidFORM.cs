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

        public vidFORM(Image getThumb, int width, int height, String getTitle,String tableName, String dirName,String uploaderName) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
            label2.Text = "Uploaded By " + uploaderName;
            label3.Text = tableName;
            _TableName = tableName;
            _DirName = dirName;
            _UploaderName = uploaderName;
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

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving video data..","Loader").ShowDialog());
                    ShowAlert.Start();
                    Application.DoEvents();

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
                        setupPlayer(LoaderModel.LoadFile("cust_sharing", _DirName, label1.Text));
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
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName);
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
