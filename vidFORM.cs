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

        readonly private static MySqlConnection con = ConnectionModel.con;

        private MediaPlayer _mp;
        private string _TableName;
        private string _DirName;
        private string _UploaderName;
        private bool _IsFromShared;
        private bool IsFromSharing;
        private bool _IsEndReached;

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

                guna2Button11.Visible = true;
                guna2Button12.Visible = true;

                _getName = _UploaderName.Replace("Shared", "");
                label5.Text = "Shared To";
                guna2Button7.Visible = false;
                label4.Visible = true;
                label4.Text = getCommentSharedToOthers() != "" ? getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _UploaderName;
                label5.Text = "Uploaded By";
                label4.Visible = true;
                label4.Text = getCommentSharedToMe() != "" ? getCommentSharedToMe() : "(No Comment)";
            }

            label2.Text = _getName;
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        private string getCommentSharedToOthers() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
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

        /// <summary>
        /// 
        /// Change form state size to normal
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        /// <summary>
        /// 
        /// Maximized form state 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
        }


        /// <summary>
        /// 
        /// Play the video from byte array
        /// 
        /// </summary>
        /// <param name="_retrieveBytesValue"></param>
        private void setupPlayer(byte[] _retrieveBytesValue) {

            label16.Text = $"{FileSize.fileSize(_retrieveBytesValue):F2}Mb";

            var _toStream = new MemoryStream(_retrieveBytesValue);

            LibVLC _setLibVLC = new LibVLC();
            var _setMedia = new Media(_setLibVLC, new StreamMediaInput(_toStream));

            _mp?.Dispose();
            _mp = new MediaPlayer(_setMedia);

            videoView1.MediaPlayer?.Dispose();
            videoView1.MediaPlayer = _mp;

            _mp.Play();

            _mp.PositionChanged += MediaPlayer_PositionChanged;
            _mp.EndReached += MediaPlayer_EndReached;

            _setLibVLC.Dispose();           

        }

        /// <summary>
        /// 
        /// Check if _mp is null, if not null then play that current _mp value
        /// else retrieve the values and assign it to _mp and play the video
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button5_Click(object sender, EventArgs e) {

            try {

                if(_mp != null) {

                    videoView1.Visible = true;

                    if (_IsEndReached) {

                        _mp.Position = 0;
                        _IsEndReached = false;

                        if (_TableName == "upload_info_directory") {
                            setupPlayer(LoaderModel.LoadFile("upload_info_directory", _DirName, label1.Text));
                        }
                        else if (_TableName == "file_info_vid") {
                            setupPlayer(LoaderModel.LoadFile("file_info_vid", _DirName, label1.Text));
                        }
                        else if (_TableName == "folder_upload_info") {
                            setupPlayer(LoaderModel.LoadFile("folder_upload_info", _DirName, label1.Text));
                        }
                        else if (_TableName == "cust_sharing") {
                            setupPlayer(LoaderModel.LoadFile("cust_sharing", _DirName, label1.Text, _IsFromShared));
                        }
                    }

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

        /// <summary>
        /// 
        /// Save video 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// Pause the video if _mp is not null and set play
        /// button visibility to true, and if play button is pressed
        /// then change pause button visibility to true
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            string getExtension = label1.Text.Substring(label1.Text.Length-4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirName);
            _showSharingFileFORM.Show();
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {

        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void videoView1_Click_2(object sender, EventArgs e) {

        }


        /// <summary>
        /// 
        /// Apply seekbar function for video
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {
            if (_mp != null && _mp.IsPlaying) {
                long newPosition = (long)(_mp.Length * guna2TrackBar1.Value / 100.0);
                _mp.Time = newPosition;
            }
        }

        /// <summary>
        ///
        /// Update trackbar value to make it sync with 
        /// the video 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e) {
            guna2TrackBar1.Invoke((MethodInvoker)delegate {
                guna2TrackBar1.Value = (int)(_mp.Position * 100);
            });
        }

        /// <summary>
        /// 
        /// Set trackbar value to 100 when 
        /// the video has ended and re-show the play button and clear
        /// video media player to allow re-play
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayer_EndReached(object sender, EventArgs e) {
            guna2TrackBar1.Value = 100;
            //guna2Button10.Visible = true;
            guna2Button5.Visible = true;
            guna2Button6.Visible = false;
            _IsEndReached = true;
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            guna2TrackBar1.Value = 0;
            _mp.Stop();
            _mp.Play();
            guna2Button10.Visible = false;
            guna2Button6.Visible = true;
        }

        private async Task saveChangesComment(String updatedComment) {

            string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button11.Visible = false;
            guna2Button12.Visible = true;
            label4.Visible = false;
            guna2TextBox4.Text = label4.Text;
        }

        private async void guna2Button12_Click(object sender, EventArgs e) {
            if (label4.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            label4.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label4.Text;
            guna2Button11.Visible = true;
            guna2Button12.Visible = false;
            guna2TextBox4.Visible = false;
            label4.Visible = true;
            label4.Refresh();
        }

        private void label7_Click(object sender, EventArgs e) {

        }
    }
}
