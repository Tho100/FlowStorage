using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using LibVLCSharp.Shared;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class VideoForm : Form {

        public readonly VideoForm instance;
        private MediaPlayer _videoMediaPlayer { get; set; }
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }
        private bool _isEndReached { get; set; }

        public VideoForm(Image getThumb, int width, int height, string fileName, string tableName, string directoryName, string uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            instance = this;

            guna2PictureBox1.Image = ResizeImage(getThumb, new Size(width, height));

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

            lblUploaderName.Text = uploaderName;

        }

        public static Image ResizeImage(Image userImg, Size size) {
            return new Bitmap(userImg, size);
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            if (_videoMediaPlayer != null) {
                _videoMediaPlayer.Stop();
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
        private void StartPlayVideo(byte[] _retrieveBytesValue) {

            lblFileSize.Text = $"{GetFileSize.fileSize(_retrieveBytesValue):F2}Mb";

            var _toStream = new MemoryStream(_retrieveBytesValue);

            LibVLC _setLibVLC = new LibVLC();
            var _setMedia = new Media(_setLibVLC, new StreamMediaInput(_toStream));

            _videoMediaPlayer?.Dispose();
            _videoMediaPlayer = new MediaPlayer(_setMedia);

            videoViewer.MediaPlayer?.Dispose();
            videoViewer.MediaPlayer = _videoMediaPlayer;

            _videoMediaPlayer.Play();

            _videoMediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            _videoMediaPlayer.EndReached += MediaPlayer_EndReached;

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

                if (_videoMediaPlayer != null) {

                    videoViewer.Visible = true;

                    if (_isEndReached) {

                        _videoMediaPlayer.Position = 0;
                        _isEndReached = false;

                        StartPlayVideo(LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text));

                    }

                    _videoMediaPlayer.Play();

                } else {

                    StartPopupForm.StartRetrievalPopup();

                    guna2PictureBox1.Visible = false;
                    videoViewer.Visible = true;

                    StartPlayVideo(LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text, _isFromShared));

                }

                btnPlayVideo.Visible = false;
                btnPauseVideo.Visible = true;

            } catch (Exception) {
                new CustomAlert(
                    title: "An error occurred", subheader: "Failed to play this video. It may be corrupted or in unsupported format.").Show();

            }

        }

        /// <summary>
        /// 
        /// Save video 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
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
            if (_videoMediaPlayer != null) {
                _videoMediaPlayer.Pause();
                btnPlayVideo.Visible = true;
                btnPauseVideo.Visible = false;
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            new shareFileFORM(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();
        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }


        /// <summary>
        /// 
        /// Apply seekbar function for video
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {
            if (_videoMediaPlayer != null && _videoMediaPlayer.IsPlaying) {
                long newPosition = (long)(_videoMediaPlayer.Length * guna2TrackBar1.Value / 100.0);
                _videoMediaPlayer.Time = newPosition;
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
                guna2TrackBar1.Value = (int)(_videoMediaPlayer.Position * 100);
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
            btnPlayVideo.Visible = true;
            btnPauseVideo.Visible = false;
            _isEndReached = true;
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            guna2TrackBar1.Value = 0;
            _videoMediaPlayer.Stop();
            _videoMediaPlayer.Play();
            btnReplayVideo.Visible = false;
            btnPauseVideo.Visible = true;
        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button12.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button12_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != string.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button12.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

    }
}
