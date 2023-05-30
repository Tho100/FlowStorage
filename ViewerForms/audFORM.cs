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
using Stripe.Terminal;
using System.Timers;
using System.Data.SqlClient;

namespace FlowSERVER1 {
    public partial class audFORM : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private MySqlCommand command = ConnectionModel.command;

        private String _TabName = "";
        private String _DirName = "";

        private static audFORM instance;
        private bool isFromShared;
        private bool IsFromSharing;

        private System.Windows.Forms.Timer _timer;
        private TimeSpan _elapsedTime;
        private Mp3FileReader _NReader;

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

                guna2Button7.Visible = true;
                guna2Button3.Visible = true;

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
                command.Parameters.AddWithValue("@username", HomePage.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
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
                command.Parameters.AddWithValue("@username", HomePage.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        /// <summary>
        /// 
        /// Play the audio from byte array based on the 
        /// file type (.wav, .mp3)
        /// 
        /// </summary>
        /// <param name="_audType"></param>
        /// <param name="_getByteAud"></param>
        private async Task setupPlayer(String _audType, Byte[] _getByteAud, int startPosition = 0) {

            label16.Text = $"{FileSize.fileSize(_getByteAud):F2}Mb";

            if (_audType == "wav") {
                using (MemoryStream ms = new MemoryStream(_getByteAud)) {
                    SoundPlayer player = new SoundPlayer(ms);
                    _getSoundPlayer = player;
                    player.Stream.Position = startPosition;
                    player.Play();
                }
            }
            else if (_audType == "mp3") {
                await Task.Run(() => mp3ToWav(_getByteAud));
            }
        }

        /// <summary>
        /// 
        /// Fetch audio byte array 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button5_Click(object sender, EventArgs e) {

            try {

                string audType = label1.Text.Substring(label1.Text.Length - 3);

                if (_mp3WaveOut != null) {
                    _mp3WaveOut.Resume();
                    pictureBox3.Enabled = true;

                } else {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving audio data.", "Loader").ShowDialog());
                    ShowAlert.Start();

                    pictureBox3.Enabled = true;

                    byte[] byteAud = LoaderModel.LoadFile(_TabName, _DirName, label1.Text, isFromShared);

                    await setupPlayer(audType, byteAud);

                    if (LoaderModel.stopFileRetrievalLoad) {
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

        /// <summary>
        /// 
        /// Convert .mp3 to .wav since the audio player only
        /// support .wav format
        /// 
        /// </summary>
        /// <param name="_mp3ByteIn"></param>
        private void mp3ToWav(Byte[] _mp3ByteIn) {

            Stream _setupStream = new MemoryStream(_mp3ByteIn);
            _NReader = new NAudio.Wave.Mp3FileReader(_setupStream);

            double getDurationSeconds = _NReader.TotalTime.TotalSeconds;
            int minutes = (int)(getDurationSeconds / 60);
            int seconds = (int)(getDurationSeconds % 60);
            label8.Text = $"{minutes}:{seconds}";

            var _setupWaveOut = new WaveOut();
            _setupWaveOut.Init(_NReader);
            _setupWaveOut.Play();
            _mp3WaveOut = _setupWaveOut;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000; 
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();

            // Show this form to start timer 
            AudioHelp breakFixedValue = new AudioHelp();
            breakFixedValue.ShowDialog();

            if(Application.OpenForms["AudioHelp"] != null) {
                Application.OpenForms["AudioHelp"].Close();
            }

            Application.OpenForms
            .OfType<Form>()
            .Where(form => String.Equals(form.Name, "AudioHelp"))
            .ToList()
            .ForEach(form => form.Close());
        }

        private void timer_Tick(object sender, EventArgs e) {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            var remainingTime = _NReader.TotalTime.Subtract(_elapsedTime);
            int minutes = remainingTime.Minutes;
            int seconds = remainingTime.Seconds;
            label4.Text = $"{minutes}:{seconds}";
        }


        /// <summary>
        /// 
        /// Pause audio if soundplayer is not null
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// When the user closed the form, stop the audio
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            this.Close();

            if(Application.OpenForms["AudioSubFORM"] != null) {
                Application.OpenForms["AudioSubFORM"].Close();
            }

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

        bool IsSubFormOpened = false;
        private void guna2Button8_Click(object sender, EventArgs e) {

            this.WindowState = FormWindowState.Minimized;
             
            if(_getSoundPlayer != null || _NReader != null) {

                if (!IsSubFormOpened) {

                    IsSubFormOpened = true;

                    AudioPlayingForm subForm = new AudioPlayingForm(label1.Text);
                    subForm.FormClosed += (s, args) => IsSubFormOpened = false;
                    subForm.Show();
                }
            }

            if (_TabName == "upload_info_directory") {

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => form.Name == "" || form.Name == "Form3")
                    .ToList()
                    .ForEach(form => form.Hide());

                this.TopMost = false;
            }
            else {

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => form.Name == "bgBlurForm")
                    .ToList()
                    .ForEach(form => form.Hide());
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            string getExtension = label1.Text.Substring(label1.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TabName, _DirName);
            _showSharingFileFORM.Show();
        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {
            //int position = guna2TrackBar1.Value;
            //_getSoundPlayer.Stop(); // Stop the current playing audio
            //setupPlayer(_audType, _getByteAud, position);
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private async Task saveChangesComment(String updatedComment) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using(var command = new MySqlCommand(query,con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", HomePage.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                await command.ExecuteNonQueryAsync();
            }
            
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button3.Visible = false;
            guna2Button7.Visible = true;
            label3.Visible = false;
            guna2TextBox4.Text = label3.Text;
        }

        private async void guna2Button7_Click_2(object sender, EventArgs e) {

            if(label3.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            } 

            label3.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label3.Text;
            guna2Button3.Visible = true;
            guna2Button7.Visible = false;
            guna2TextBox4.Visible = false;
            label3.Visible = true;
            label3.Refresh();

        }
    }
}
