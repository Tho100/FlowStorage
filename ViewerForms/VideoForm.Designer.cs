namespace FlowSERVER1 {
    partial class VideoForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoForm));
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblUploaderName = new System.Windows.Forms.Label();
            this.videoViewer = new LibVLCSharp.WinForms.VideoView();
            this.lblUserComment = new System.Windows.Forms.Label();
            this.guna2VSeparator2 = new Guna.UI2.WinForms.Guna2VSeparator();
            this.guna2Separator2 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.guna2VSeparator1 = new Guna.UI2.WinForms.Guna2VSeparator();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.guna2TrackBar1 = new Guna.UI2.WinForms.Guna2TrackBar();
            this.guna2Separator3 = new Guna.UI2.WinForms.Guna2Separator();
            this.label15 = new System.Windows.Forms.Label();
            this.lblFileSize = new System.Windows.Forms.Label();
            this.txtFieldComment = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnEditComment = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button12 = new Guna.UI2.WinForms.Guna2Button();
            this.btnReplayVideo = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button9 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button8 = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnShareFile = new Guna.UI2.WinForms.Guna2Button();
            this.btnPauseVideo = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnPlayVideo = new Guna.UI2.WinForms.Guna2Button();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Button4 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button3 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.videoViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoEllipsis = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.ForeColor = System.Drawing.Color.White;
            this.lblFileName.Location = new System.Drawing.Point(10, 11);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(318, 87);
            this.lblFileName.TabIndex = 25;
            this.lblFileName.Text = "Video";
            this.lblFileName.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblUploaderName
            // 
            this.lblUploaderName.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblUploaderName.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblUploaderName.Location = new System.Drawing.Point(11, 189);
            this.lblUploaderName.Name = "lblUploaderName";
            this.lblUploaderName.Size = new System.Drawing.Size(266, 36);
            this.lblUploaderName.TabIndex = 29;
            this.lblUploaderName.Text = "Uploaded By urmom";
            // 
            // videoViewer
            // 
            this.videoViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoViewer.BackColor = System.Drawing.Color.Black;
            this.videoViewer.Location = new System.Drawing.Point(351, 76);
            this.videoViewer.MediaPlayer = null;
            this.videoViewer.Name = "videoViewer";
            this.videoViewer.Size = new System.Drawing.Size(774, 481);
            this.videoViewer.TabIndex = 38;
            this.videoViewer.Text = "videoView1";
            this.videoViewer.Visible = false;
            this.videoViewer.Click += new System.EventHandler(this.videoView1_Click_2);
            // 
            // lblUserComment
            // 
            this.lblUserComment.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserComment.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblUserComment.Location = new System.Drawing.Point(11, 297);
            this.lblUserComment.Name = "lblUserComment";
            this.lblUserComment.Size = new System.Drawing.Size(324, 302);
            this.lblUserComment.TabIndex = 39;
            this.lblUserComment.Text = "Uploaded By urmom";
            // 
            // guna2VSeparator2
            // 
            this.guna2VSeparator2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2VSeparator2.BackColor = System.Drawing.Color.Transparent;
            this.guna2VSeparator2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2VSeparator2.Location = new System.Drawing.Point(886, -1);
            this.guna2VSeparator2.Name = "guna2VSeparator2";
            this.guna2VSeparator2.Size = new System.Drawing.Size(10, 66);
            this.guna2VSeparator2.TabIndex = 73;
            // 
            // guna2Separator2
            // 
            this.guna2Separator2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2Separator2.Location = new System.Drawing.Point(-7, 230);
            this.guna2Separator2.Name = "guna2Separator2";
            this.guna2Separator2.Size = new System.Drawing.Size(349, 10);
            this.guna2Separator2.TabIndex = 72;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Separator1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Separator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2Separator1.Location = new System.Drawing.Point(343, 60);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(799, 10);
            this.guna2Separator1.TabIndex = 71;
            // 
            // guna2VSeparator1
            // 
            this.guna2VSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.guna2VSeparator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2VSeparator1.Location = new System.Drawing.Point(337, 1);
            this.guna2VSeparator1.Name = "guna2VSeparator1";
            this.guna2VSeparator1.Size = new System.Drawing.Size(10, 617);
            this.guna2VSeparator1.TabIndex = 70;
            // 
            // label5
            // 
            this.label5.AutoEllipsis = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(12, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 17);
            this.label5.TabIndex = 74;
            this.label5.Text = "Uploaded By";
            // 
            // label6
            // 
            this.label6.AutoEllipsis = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Silver;
            this.label6.Location = new System.Drawing.Point(11, 264);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 17);
            this.label6.TabIndex = 75;
            this.label6.Text = "Comment";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Silver;
            this.label7.Location = new System.Drawing.Point(408, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(195, 23);
            this.label7.TabIndex = 76;
            this.label7.Text = "Video File";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // guna2TrackBar1
            // 
            this.guna2TrackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2TrackBar1.Location = new System.Drawing.Point(363, 582);
            this.guna2TrackBar1.Name = "guna2TrackBar1";
            this.guna2TrackBar1.Size = new System.Drawing.Size(698, 23);
            this.guna2TrackBar1.TabIndex = 80;
            this.guna2TrackBar1.ThumbColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2TrackBar1.Value = 0;
            this.guna2TrackBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.guna2TrackBar1_Scroll);
            // 
            // guna2Separator3
            // 
            this.guna2Separator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Separator3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2Separator3.Location = new System.Drawing.Point(343, 562);
            this.guna2Separator3.Name = "guna2Separator3";
            this.guna2Separator3.Size = new System.Drawing.Size(799, 10);
            this.guna2Separator3.TabIndex = 81;
            // 
            // label15
            // 
            this.label15.AutoEllipsis = true;
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Silver;
            this.label15.Location = new System.Drawing.Point(13, 107);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 17);
            this.label15.TabIndex = 86;
            this.label15.Text = "File Size";
            // 
            // lblFileSize
            // 
            this.lblFileSize.AutoSize = true;
            this.lblFileSize.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileSize.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblFileSize.Location = new System.Drawing.Point(15, 131);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblFileSize.Size = new System.Drawing.Size(32, 17);
            this.lblFileSize.TabIndex = 85;
            this.lblFileSize.Text = "N/A";
            // 
            // txtFieldComment
            // 
            this.txtFieldComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtFieldComment.BackColor = System.Drawing.Color.Transparent;
            this.txtFieldComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldComment.BorderRadius = 8;
            this.txtFieldComment.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFieldComment.DefaultText = "";
            this.txtFieldComment.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFieldComment.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFieldComment.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldComment.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldComment.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldComment.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldComment.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtFieldComment.ForeColor = System.Drawing.Color.White;
            this.txtFieldComment.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldComment.Location = new System.Drawing.Point(11, 297);
            this.txtFieldComment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFieldComment.MaxLength = 295;
            this.txtFieldComment.Multiline = true;
            this.txtFieldComment.Name = "txtFieldComment";
            this.txtFieldComment.PasswordChar = '\0';
            this.txtFieldComment.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtFieldComment.PlaceholderText = "Add a comment for receiver to see";
            this.txtFieldComment.SelectedText = "";
            this.txtFieldComment.Size = new System.Drawing.Size(312, 292);
            this.txtFieldComment.TabIndex = 96;
            this.txtFieldComment.Visible = false;
            // 
            // btnEditComment
            // 
            this.btnEditComment.Animated = true;
            this.btnEditComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnEditComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnEditComment.BorderRadius = 6;
            this.btnEditComment.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnEditComment.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnEditComment.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEditComment.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnEditComment.FillColor = System.Drawing.Color.Transparent;
            this.btnEditComment.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnEditComment.ForeColor = System.Drawing.Color.White;
            this.btnEditComment.Image = global::FlowSERVER1.Properties.Resources.icons8_edit_48;
            this.btnEditComment.ImageSize = new System.Drawing.Size(21, 21);
            this.btnEditComment.Location = new System.Drawing.Point(294, 250);
            this.btnEditComment.Name = "btnEditComment";
            this.btnEditComment.Size = new System.Drawing.Size(38, 31);
            this.btnEditComment.TabIndex = 95;
            this.btnEditComment.Visible = false;
            this.btnEditComment.Click += new System.EventHandler(this.guna2Button11_Click);
            // 
            // guna2Button12
            // 
            this.guna2Button12.Animated = true;
            this.guna2Button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button12.BorderRadius = 6;
            this.guna2Button12.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button12.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button12.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button12.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button12.FillColor = System.Drawing.Color.Transparent;
            this.guna2Button12.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button12.ForeColor = System.Drawing.Color.White;
            this.guna2Button12.Image = global::FlowSERVER1.Properties.Resources.icons8_done_26;
            this.guna2Button12.ImageSize = new System.Drawing.Size(19, 19);
            this.guna2Button12.Location = new System.Drawing.Point(295, 250);
            this.guna2Button12.Name = "guna2Button12";
            this.guna2Button12.Size = new System.Drawing.Size(38, 31);
            this.guna2Button12.TabIndex = 97;
            this.guna2Button12.Visible = false;
            this.guna2Button12.Click += new System.EventHandler(this.guna2Button12_Click);
            // 
            // btnReplayVideo
            // 
            this.btnReplayVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReplayVideo.Animated = true;
            this.btnReplayVideo.BackColor = System.Drawing.Color.Transparent;
            this.btnReplayVideo.BorderColor = System.Drawing.Color.Empty;
            this.btnReplayVideo.BorderRadius = 6;
            this.btnReplayVideo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReplayVideo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReplayVideo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReplayVideo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReplayVideo.FillColor = System.Drawing.Color.Empty;
            this.btnReplayVideo.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnReplayVideo.ForeColor = System.Drawing.Color.White;
            this.btnReplayVideo.Image = global::FlowSERVER1.Properties.Resources.icons8_restart_500__1_;
            this.btnReplayVideo.ImageSize = new System.Drawing.Size(27, 27);
            this.btnReplayVideo.Location = new System.Drawing.Point(1071, 575);
            this.btnReplayVideo.Name = "btnReplayVideo";
            this.btnReplayVideo.Size = new System.Drawing.Size(46, 36);
            this.btnReplayVideo.TabIndex = 82;
            this.btnReplayVideo.Visible = false;
            this.btnReplayVideo.Click += new System.EventHandler(this.guna2Button10_Click);
            // 
            // guna2Button9
            // 
            this.guna2Button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button9.Animated = true;
            this.guna2Button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button9.BorderRadius = 6;
            this.guna2Button9.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button9.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button9.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button9.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button9.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button9.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button9.ForeColor = System.Drawing.Color.White;
            this.guna2Button9.Image = global::FlowSERVER1.Properties.Resources.icons8_subtract_30;
            this.guna2Button9.ImageOffset = new System.Drawing.Point(0, 1);
            this.guna2Button9.ImageSize = new System.Drawing.Size(22, 22);
            this.guna2Button9.Location = new System.Drawing.Point(998, 16);
            this.guna2Button9.Name = "guna2Button9";
            this.guna2Button9.Size = new System.Drawing.Size(38, 31);
            this.guna2Button9.TabIndex = 79;
            this.guna2Button9.Click += new System.EventHandler(this.guna2Button9_Click);
            // 
            // guna2Button8
            // 
            this.guna2Button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button8.Animated = true;
            this.guna2Button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button8.BorderRadius = 6;
            this.guna2Button8.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button8.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button8.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button8.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button8.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button8.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button8.ForeColor = System.Drawing.Color.White;
            this.guna2Button8.Image = global::FlowSERVER1.Properties.Resources.icons8_subtract_30;
            this.guna2Button8.ImageOffset = new System.Drawing.Point(0, 1);
            this.guna2Button8.ImageSize = new System.Drawing.Size(22, 22);
            this.guna2Button8.Location = new System.Drawing.Point(998, 16);
            this.guna2Button8.Name = "guna2Button8";
            this.guna2Button8.Size = new System.Drawing.Size(38, 0);
            this.guna2Button8.TabIndex = 78;
            this.guna2Button8.Click += new System.EventHandler(this.guna2Button8_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::FlowSERVER1.Properties.Resources.video_image;
            this.pictureBox2.Location = new System.Drawing.Point(362, 14);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(42, 38);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 77;
            this.pictureBox2.TabStop = false;
            // 
            // btnShareFile
            // 
            this.btnShareFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShareFile.Animated = true;
            this.btnShareFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnShareFile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnShareFile.BorderRadius = 6;
            this.btnShareFile.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnShareFile.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnShareFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnShareFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnShareFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.btnShareFile.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnShareFile.ForeColor = System.Drawing.Color.White;
            this.btnShareFile.Image = global::FlowSERVER1.Properties.Resources.icons8_share_26;
            this.btnShareFile.ImageSize = new System.Drawing.Size(17, 17);
            this.btnShareFile.Location = new System.Drawing.Point(910, 16);
            this.btnShareFile.Name = "btnShareFile";
            this.btnShareFile.Size = new System.Drawing.Size(38, 31);
            this.btnShareFile.TabIndex = 40;
            this.btnShareFile.Click += new System.EventHandler(this.guna2Button7_Click);
            // 
            // btnPauseVideo
            // 
            this.btnPauseVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPauseVideo.Animated = true;
            this.btnPauseVideo.BackColor = System.Drawing.Color.Transparent;
            this.btnPauseVideo.BorderColor = System.Drawing.Color.Empty;
            this.btnPauseVideo.BorderRadius = 6;
            this.btnPauseVideo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPauseVideo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPauseVideo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPauseVideo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPauseVideo.FillColor = System.Drawing.Color.Empty;
            this.btnPauseVideo.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnPauseVideo.ForeColor = System.Drawing.Color.White;
            this.btnPauseVideo.Image = ((System.Drawing.Image)(resources.GetObject("btnPauseVideo.Image")));
            this.btnPauseVideo.ImageSize = new System.Drawing.Size(31, 31);
            this.btnPauseVideo.Location = new System.Drawing.Point(1071, 575);
            this.btnPauseVideo.Name = "btnPauseVideo";
            this.btnPauseVideo.Size = new System.Drawing.Size(46, 36);
            this.btnPauseVideo.TabIndex = 37;
            this.btnPauseVideo.Visible = false;
            this.btnPauseVideo.Click += new System.EventHandler(this.guna2Button6_Click_1);
            // 
            // btnPlayVideo
            // 
            this.btnPlayVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlayVideo.Animated = true;
            this.btnPlayVideo.BackColor = System.Drawing.Color.Transparent;
            this.btnPlayVideo.BorderColor = System.Drawing.Color.Empty;
            this.btnPlayVideo.BorderRadius = 6;
            this.btnPlayVideo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPlayVideo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPlayVideo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPlayVideo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPlayVideo.FillColor = System.Drawing.Color.Empty;
            this.btnPlayVideo.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnPlayVideo.ForeColor = System.Drawing.Color.White;
            this.btnPlayVideo.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayVideo.Image")));
            this.btnPlayVideo.ImageSize = new System.Drawing.Size(34, 34);
            this.btnPlayVideo.Location = new System.Drawing.Point(1071, 575);
            this.btnPlayVideo.Name = "btnPlayVideo";
            this.btnPlayVideo.Size = new System.Drawing.Size(48, 36);
            this.btnPlayVideo.TabIndex = 27;
            this.btnPlayVideo.Click += new System.EventHandler(this.guna2Button5_Click);
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(351, 76);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(774, 479);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.guna2PictureBox1.TabIndex = 26;
            this.guna2PictureBox1.TabStop = false;
            this.guna2PictureBox1.Click += new System.EventHandler(this.guna2PictureBox1_Click);
            // 
            // guna2Button4
            // 
            this.guna2Button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button4.Animated = true;
            this.guna2Button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button4.BorderRadius = 6;
            this.guna2Button4.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button4.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button4.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button4.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button4.ForeColor = System.Drawing.Color.White;
            this.guna2Button4.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button4.Image")));
            this.guna2Button4.ImageSize = new System.Drawing.Size(22, 22);
            this.guna2Button4.Location = new System.Drawing.Point(954, 16);
            this.guna2Button4.Name = "guna2Button4";
            this.guna2Button4.Size = new System.Drawing.Size(38, 31);
            this.guna2Button4.TabIndex = 24;
            this.guna2Button4.Click += new System.EventHandler(this.guna2Button4_Click);
            // 
            // guna2Button3
            // 
            this.guna2Button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button3.Animated = true;
            this.guna2Button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button3.BorderRadius = 6;
            this.guna2Button3.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button3.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button3.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button3.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button3.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button3.ForeColor = System.Drawing.Color.White;
            this.guna2Button3.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button3.Image")));
            this.guna2Button3.ImageSize = new System.Drawing.Size(22, 22);
            this.guna2Button3.Location = new System.Drawing.Point(1042, 16);
            this.guna2Button3.Name = "guna2Button3";
            this.guna2Button3.Size = new System.Drawing.Size(38, 31);
            this.guna2Button3.TabIndex = 23;
            this.guna2Button3.Visible = false;
            this.guna2Button3.Click += new System.EventHandler(this.guna2Button3_Click);
            // 
            // guna2Button1
            // 
            this.guna2Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button1.Animated = true;
            this.guna2Button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button1.BorderRadius = 6;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button1.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button1.ForeColor = System.Drawing.Color.White;
            this.guna2Button1.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button1.Image")));
            this.guna2Button1.ImageSize = new System.Drawing.Size(22, 22);
            this.guna2Button1.Location = new System.Drawing.Point(1042, 16);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Size = new System.Drawing.Size(38, 31);
            this.guna2Button1.TabIndex = 22;
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // guna2Button2
            // 
            this.guna2Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button2.Animated = true;
            this.guna2Button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.guna2Button2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.guna2Button2.BorderRadius = 6;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button2.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button2.ForeColor = System.Drawing.Color.White;
            this.guna2Button2.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button2.Image")));
            this.guna2Button2.Location = new System.Drawing.Point(1086, 16);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Size = new System.Drawing.Size(38, 31);
            this.guna2Button2.TabIndex = 21;
            this.guna2Button2.Click += new System.EventHandler(this.guna2Button2_Click);
            // 
            // VideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.ClientSize = new System.Drawing.Size(1136, 617);
            this.Controls.Add(this.txtFieldComment);
            this.Controls.Add(this.btnEditComment);
            this.Controls.Add(this.guna2Button12);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lblFileSize);
            this.Controls.Add(this.btnReplayVideo);
            this.Controls.Add(this.guna2Separator3);
            this.Controls.Add(this.guna2TrackBar1);
            this.Controls.Add(this.guna2Button9);
            this.Controls.Add(this.guna2Button8);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.guna2VSeparator2);
            this.Controls.Add(this.guna2Separator2);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.guna2VSeparator1);
            this.Controls.Add(this.btnShareFile);
            this.Controls.Add(this.lblUserComment);
            this.Controls.Add(this.videoViewer);
            this.Controls.Add(this.btnPauseVideo);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblUploaderName);
            this.Controls.Add(this.btnPlayVideo);
            this.Controls.Add(this.guna2PictureBox1);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.guna2Button4);
            this.Controls.Add(this.guna2Button3);
            this.Controls.Add(this.guna2Button1);
            this.Controls.Add(this.guna2Button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VideoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video Viewer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.vidFORM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Button guna2Button4;
        private Guna.UI2.WinForms.Guna2Button guna2Button3;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private System.Windows.Forms.Label lblFileName;
        private Guna.UI2.WinForms.Guna2Button btnPlayVideo;
        private System.Windows.Forms.Label lblUploaderName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnPauseVideo;
        private LibVLCSharp.WinForms.VideoView videoViewer;
        private System.Windows.Forms.Label lblUserComment;
        private Guna.UI2.WinForms.Guna2Button btnShareFile;
        private Guna.UI2.WinForms.Guna2VSeparator guna2VSeparator2;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator2;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2VSeparator guna2VSeparator1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label7;
        private Guna.UI2.WinForms.Guna2Button guna2Button8;
        private Guna.UI2.WinForms.Guna2Button guna2Button9;
        private Guna.UI2.WinForms.Guna2TrackBar guna2TrackBar1;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator3;
        private Guna.UI2.WinForms.Guna2Button btnReplayVideo;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblFileSize;
        protected Guna.UI2.WinForms.Guna2TextBox txtFieldComment;
        private Guna.UI2.WinForms.Guna2Button btnEditComment;
        private Guna.UI2.WinForms.Guna2Button guna2Button12;
    }
}