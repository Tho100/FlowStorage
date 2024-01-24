using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class AudioForm : Form {

        private string _tableName { get; set; }
        private string _directoryName { get; set; }

        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }

        private byte[] _audioBytes { get; set; }
        private Timer _elapsedTickTimer { get; set; }
        private TimeSpan _elapsedTime { get; set; }
        private Mp3FileReader _NReader { get; set; }
        private SoundPlayer _wavAudioPlayer { get; set; }
        private WaveOut _mp3WaveOut { get; set; }

        public AudioForm(string fileName, string tableName,string directoryName,string uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            label5.Text = isFromShared ? "Shared To" : "Uploaded By";

            lblUserComment.Visible = true;

            if (isFromShared) {
                string comment = GetComment.getCommentSharedToOthers(fileName: fileName);
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
                btnEditComment.Visible = true;
                btnShareFile.Visible = false;

            } else {

                if (GlobalsTable.publicTables.Contains(tableName) || tableName == GlobalsTable.directoryUploadTable || tableName == GlobalsTable.folderUploadTable) {
                    lblUserComment.Text = "(No Comment)";

                } else if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                    string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                    lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

                } else {
                    string comment = GetComment.getCommentSharedToMe(fileName: fileName);
                    lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

                }
            }

            lblUploaderUsername.Text = uploaderName;
            pictureBox3.Enabled = false;

        }

        /// <summary>
        /// 
        /// Fetch data/play/pause/resume audio
        /// based on it's audio type (.wav, .mp3)
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button5_Click(object sender, EventArgs e) {

            try {

                string audioType = lblFileName.Text.Substring(lblFileName.Text.Length - 3);

                if(_wavAudioPlayer != null && audioType == "wav") {
                    _wavAudioPlayer.Play();
                    btnPauseAudio.Visible = true;
                    btnPlayAudio.Visible = false;
                    pictureBox3.Enabled = true;
                    return;
                }

                if (_mp3WaveOut != null && audioType == "mp3") {
                    _elapsedTickTimer.Start();
                    _mp3WaveOut.Resume();
                    pictureBox3.Enabled = true;

                } else {

                    StartPopupForm.StartRetrievalPopup();

                    pictureBox3.Enabled = true;

                    byte[] audioBytes = LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text, _isFromShared);
                    _audioBytes = audioBytes;

                    await StartPlayAudio(audioType, audioBytes);

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
        /// Play the audio from byte array based on the 
        /// file type (.wav, .mp3)
        /// 
        /// </summary>
        /// <param name="_audType"></param>
        /// <param name="_getByteAud"></param>
        private async Task StartPlayAudio(string audioType, byte[] audioBytes) {

            lblFileSize.Text = $"{GetFileSize.fileSize(audioBytes):F2}Mb";

            btnPauseAudio.Visible = true;
            btnPlayAudio.Visible = false;

            btnPlayAudio.SendToBack();
            btnPauseAudio.BringToFront();

            if (audioType == "wav") {
                await Task.Run(() => PlayAudioWave(audioBytes));

            } else if (audioType == "mp3") {
                await Task.Run(() => PlayAudioMp3(audioBytes));

            }

        }

        /// <summary>
        /// 
        /// Convert .mp3 to .wav since the audio player only
        /// support .wav format
        /// 
        /// </summary>
        /// <param name="_mp3ByteIn"></param>
        private void PlayAudioMp3(byte[] mp3Bytes) {

            var stream = new MemoryStream(mp3Bytes);
            _NReader = new Mp3FileReader(stream);

            double getDurationSeconds = _NReader.TotalTime.TotalSeconds;
            int minutes = (int)(getDurationSeconds / 60);
            int seconds = (int)(getDurationSeconds % 60);
            lblDuration.Text = $"{minutes}:{seconds:D2}";

            var _setupWaveOut = new WaveOut();
            _setupWaveOut.Init(_NReader);
            _setupWaveOut.Play();
            _mp3WaveOut = _setupWaveOut;

            _elapsedTickTimer = new Timer();
            _elapsedTickTimer.Interval = 1000;
            _elapsedTickTimer.Tick += new EventHandler(timer_Tick);
            _elapsedTickTimer.Start();

            new AudioHelp().ShowDialog();

            Application.OpenForms["AudioHelp"]?.Close();

            ClosePopupForm.CloseCustomPopup("AudioHelp");

        }

        private void PlayAudioWave(byte[] waveBytes) {

            using (MemoryStream ms = new MemoryStream(waveBytes)) {
                SoundPlayer player = new SoundPlayer(ms);
                _wavAudioPlayer = player;
                player.Stream.Position = 0;
                player.Play();
            }
        }

        private void timer_Tick(object sender, EventArgs e) {

            if (_elapsedTime >= _NReader.TotalTime) {

                lblDurationLeft.Text = "0:00";

                btnReplayAudio.Visible = true;
                btnPauseAudio.Visible = false;
                btnReplayAudio.BringToFront();
                _elapsedTickTimer.Stop();

            } else {

                _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));
                var remainingTime = _NReader.TotalTime.Subtract(_elapsedTime);
                int minutes = remainingTime.Minutes;
                int seconds = remainingTime.Seconds;
                lblDurationLeft.Text = $"{minutes}:{seconds}";

            }
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
            btnReplayAudio.Visible = false;
            btnPlayAudio.Visible = true;
            pictureBox3.Enabled = false;

            if(_elapsedTickTimer != null) {
                _elapsedTickTimer.Stop();
            }

            if(_wavAudioPlayer != null) {
                _wavAudioPlayer.Stop();
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

            if(_wavAudioPlayer != null) {
                _wavAudioPlayer.Stop();
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
             
            if(_wavAudioPlayer != null || _NReader != null) {

                if (!IsSubFormOpened) {

                    IsSubFormOpened = true;

                    AudioPlayingForm subForm = new AudioPlayingForm(lblFileName.Text);
                    subForm.FormClosed += (s, args) => IsSubFormOpened = false;
                    subForm.Show();
                }
            }

            if (_tableName == GlobalsTable.directoryUploadTable) {

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => form.Name == "" || form.Name == "Form3")
                    .ToList()
                    .ForEach(form => form.Hide());

                this.TopMost = false;

            } else {

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => form.Name == "bgBlurForm")
                    .ToList()
                    .ForEach(form => form.Hide());
            }

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            new ShareSelectedFileForm(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();
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
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != string.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button7.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();

        }

        private async void btnReplayAudio_Click(object sender, EventArgs e) {

            btnPauseAudio.Visible = true;
            string audioType = lblFileName.Text.Substring(lblFileName.Text.Length - 3);

            _elapsedTickTimer.Stop();
            _elapsedTime = TimeSpan.Zero;

            await StartPlayAudio(audioType, _audioBytes);

            _elapsedTickTimer.Start();
        }

    }
}
