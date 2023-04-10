using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Media;
using NAudio.Wave;
using System.Linq;
using System.Drawing;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class audFORM : Form {
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String _TabName = "";
        private static String _DirName = "";
        private static audFORM instance;
        private static bool isFromShared;
        private static bool IsFromSharing; 

        public audFORM(String titleName,String _TableName,String _DirectoryName,String _UploaderName, bool _isFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            label1.Text = titleName;
            _TabName = _TableName;
            _DirName = _DirectoryName;
            isFromShared = _isFromShared;
            IsFromSharing = _isFromSharing;

            if (_isShared == true) {
                _getName = _UploaderName.Replace("Shared", "");
                label5.Text = "Shared To";
                guna2Button1.Visible = false;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _UploaderName;
                label5.Text = "Uploaded By";
                label3.Visible = true;
                label3.Text = getCommentSharedToMe() != "" ? getCommentSharedToMe() : "(No Comment)";
            }

            label2.Text = _getName;

            pictureBox3.Enabled = false;
            instance = this;
        }

        SoundPlayer _getSoundPlayer = null;
        WaveOut _mp3WaveOut = null;

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text,EncryptionKey.KeyValue));
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text,EncryptionKey.KeyValue));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }

        private void setupPlayer(String _audType, Byte[] _getByteAud) {
            if (_audType == "wav") {
                using (MemoryStream _ms = new MemoryStream(_getByteAud)) {
                    SoundPlayer player = new SoundPlayer(_ms);
                    _getSoundPlayer = player;
                    player.Play();
                }
            }
            else if (_audType == "mp3") {
                mp3ToWav(_getByteAud);
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e) {                
            try {
                String _audType = label1.Text.Substring(label1.Text.Length-3);
                if(_mp3WaveOut != null) {
                    _mp3WaveOut.Resume();
                    pictureBox3.Enabled = true;
                }
                else {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving audio data.","Loader").ShowDialog());
                    ShowAlert.Start();

                    if (_TabName == "file_info_audi") {
                        pictureBox3.Enabled = true;
                        setupPlayer(_audType,LoaderModel.LoadFile("file_info_audi",_DirName,label1.Text));
                    } else if (_TabName == "upload_info_directory") {
                        pictureBox3.Enabled = true;
                        setupPlayer(_audType, LoaderModel.LoadFile("upload_info_directory", _DirName, label1.Text));
                    }
                    else if (_TabName == "folder_upload_info") {
                        pictureBox3.Enabled = true;
                        setupPlayer(_audType, LoaderModel.LoadFile("folder_upload_info", _DirName, label1.Text));
                    } else if (_TabName == "cust_sharing") {
                        pictureBox3.Enabled = true;
                        setupPlayer(_audType, LoaderModel.LoadFile("cust_sharing", _DirName, label1.Text,isFromShared));
                    }

                    if (LoaderModel.stopFileRetrievalLoad == true) {
                        pictureBox3.Enabled = false;
                    }
                }
            }
            catch (Exception) {
                pictureBox3.Enabled = false;
                guna2Button6.Visible = false;
                guna2Button5.Visible = true;
            }

            guna2Button6.Visible = true;
            guna2Button5.Visible = false;
        }

        private void mp3ToWav(Byte[] _mp3ByteIn) {
            Stream _setupStream = new MemoryStream(_mp3ByteIn);
            var _NReader = new NAudio.Wave.Mp3FileReader(_setupStream);
            var _setupWaveOut = new WaveOut();
            _setupWaveOut.Init(_NReader);
            _setupWaveOut.Play();
            _mp3WaveOut = _setupWaveOut;
        }


        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
            pictureBox3.Enabled = false;
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
            }
            if (_mp3WaveOut != null) {
                _mp3WaveOut.Pause();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
            }
            if (_mp3WaveOut != null) {
                _mp3WaveOut.Stop();
            }
        }

        private void audFORM_Load(object sender, EventArgs e) {
                
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if (_TabName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
            }
            else if (_TabName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
            }
            else if (_TabName == "file_info_audi") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_audi", _DirName);
            }
            else if (_TabName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName);
            }
            this.TopMost = true;
        }

        private void zedGraphControl1_Load(object sender, EventArgs e) {

        }

        private void waveViewer1_Load(object sender, EventArgs e) {

        }

        private void chart1_Click(object sender, EventArgs e) {

        }
        private void guna2Button7_Click(object sender, EventArgs e) {

        }

        private void guna2Button7_Click_1(object sender, EventArgs e) {

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            if(_TabName == "upload_info_directory") {
                this.WindowState = FormWindowState.Minimized;
                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, ""))
                    .ToList()
                    .ForEach(form => form.Hide());
                Application.OpenForms
                  .OfType<Form>()
                  .Where(form => String.Equals(form.Name, "Form3"))
                  .ToList()
                  .ForEach(form => form.Hide());
                this.TopMost = false;
            }
            else {
                this.WindowState = FormWindowState.Minimized;
                Application.OpenForms
                  .OfType<Form>()
                  .Where(form => String.Equals(form.Name, "bgBlurForm"))
                  .ToList()
                  .ForEach(form => form.Hide());
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            string[] parts = label1.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TabName, _DirName);
            _showSharingFileFORM.Show();
        }

        private void label7_Click(object sender, EventArgs e) {

        }
    }
}
