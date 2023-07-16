using System;
using System.Windows.Forms;
using System.IO;
using System.Media;
using NAudio.Wave;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using FlowSERVER1.Helper; 
using FlowSERVER1.Global;
using FlowSERVER1.AlertForms;

namespace FlowSERVER1 {
    public partial class AudioForm : Form {

        private string _tableName { get; set; }
        private string _directoryName { get; set; }

        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }

        private System.Windows.Forms.Timer _elapsedTickTimer { get; set; }
        private TimeSpan _elapsedTime { get; set; }
        private Mp3FileReader _NReader { get; set; }

        private SoundPlayer _getSoundPlayer { get; set; }
        private WaveOut _mp3WaveOut { get; set; }

        public AudioForm(String fileName, String tableName,String directoryName,String uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            if (isFromShared == true) {

                guna2Button7.Visible = true;
                btnEditComment.Visible = true;

                label5.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
            } else {
                label5.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";
            }

            if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                label5.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(tableName: tableName, fileName: fileName, uploaderName: uploaderName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
            }

            lblUploaderUsername.Text = uploaderName;
            pictureBox3.Enabled = false;

        }

        /// <summary>
        /// 
        /// Play the audio from byte array based on the 
        /// file type (.wav, .mp3)
        /// 
        /// </summary>
        /// <param name="_audType"></param>
        /// <param name="_getByteAud"></param>
        private async Task StartPlayAudio(String _audType, Byte[] _getByteAud, int startPosition = 0) {

            lblFileSize.Text = $"{FileSize.fileSize(_getByteAud):F2}Mb";

            btnPauseAudio.Visible = true;
            btnPlayAudio.Visible = false;

            btnPlayAudio.SendToBack();
            btnPauseAudio.BringToFront();

            if (_audType == "wav") {
                using (MemoryStream ms = new MemoryStream(_getByteAud)) {
                    SoundPlayer player = new SoundPlayer(ms);
                    _getSoundPlayer = player;
                    player.Stream.Position = startPosition;
                    player.Play();
                }
            }
            else if (_audType == "mp3") {
                await Task.Run(() => ConvertMP3ToWav(_getByteAud));
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

                string audType = lblFileName.Text.Substring(lblFileName.Text.Length - 3);

                if (_mp3WaveOut != null) {
                    _mp3WaveOut.Resume();
                    pictureBox3.Enabled = true;

                } else {

                    new Thread(() => new RetrievalAlert("Flowstorage is retrieving audio data.", "Loader").ShowDialog()).Start();

                    pictureBox3.Enabled = true;

                    byte[] byteAud = LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text, _isFromShared);

                    await StartPlayAudio(audType, byteAud);

                    if (LoaderModel.stopFileRetrievalLoad) {
                        pictureBox3.Enabled = false;
                    }
                }

                btnPauseAudio.Visible = true;
                btnPlayAudio.Visible = false;

                btnPlayAudio.SendToBack();
                btnPauseAudio.BringToFront();

            } catch (Exception) {
                new CustomAlert(title: "An error occurred","Failed to play this audio.").Show();
            }
        }

        /// <summary>
        /// 
        /// Convert .mp3 to .wav since the audio player only
        /// support .wav format
        /// 
        /// </summary>
        /// <param name="_mp3ByteIn"></param>
        private void ConvertMP3ToWav(Byte[] _mp3ByteIn) {

            Stream _setupStream = new MemoryStream(_mp3ByteIn);
            _NReader = new NAudio.Wave.Mp3FileReader(_setupStream);

            double getDurationSeconds = _NReader.TotalTime.TotalSeconds;
            int minutes = (int)(getDurationSeconds / 60);
            int seconds = (int)(getDurationSeconds % 60);
            lblDuration.Text = $"{minutes}:{seconds}";

            var _setupWaveOut = new WaveOut();
            _setupWaveOut.Init(_NReader);
            _setupWaveOut.Play();
            _mp3WaveOut = _setupWaveOut;

            _elapsedTickTimer = new System.Windows.Forms.Timer();
            _elapsedTickTimer.Interval = 1000;
            _elapsedTickTimer.Tick += new EventHandler(timer_Tick);
            _elapsedTickTimer.Start();

            AudioHelp breakFixedValue = new AudioHelp();
            breakFixedValue.ShowDialog();

            if(Application.OpenForms["AudioHelp"] != null) {
                Application.OpenForms["AudioHelp"].Close();
            }

            CloseForm.closeForm("AudioHelp");
        }

        private void timer_Tick(object sender, EventArgs e) {
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
            var remainingTime = _NReader.TotalTime.Subtract(_elapsedTime);
            int minutes = remainingTime.Minutes;
            int seconds = remainingTime.Seconds;
            lblDurationLeft.Text = $"{minutes}:{seconds}";
        }


        /// <summary>
        /// 
        /// Pause audio if soundplayer is not null
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button6_Click(object sender, EventArgs e) {
            btnPauseAudio.Visible = false;
            btnPlayAudio.Visible = true;
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
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
            this.TopMost = true;
        }


        bool IsSubFormOpened = false;
        private void guna2Button8_Click(object sender, EventArgs e) {

            this.WindowState = FormWindowState.Minimized;
             
            if(_getSoundPlayer != null || _NReader != null) {

                if (!IsSubFormOpened) {

                    IsSubFormOpened = true;

                    AudioPlayingForm subForm = new AudioPlayingForm(lblFileName.Text);
                    subForm.FormClosed += (s, args) => IsSubFormOpened = false;
                    subForm.Show();
                }
            }

            if (_tableName == "upload_info_directory") {

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
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            new shareFileFORM(lblFileName.Text, getExtension, 
                _isFromSharing, _tableName, _directoryName).Show();
        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button7.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button7_Click_2(object sender, EventArgs e) {

            if(lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().saveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button7.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();

        }
    }
}
