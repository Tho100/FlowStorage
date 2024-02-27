namespace FlowstorageDesktop {
    partial class UploadingAlert {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UploadingAlert));
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.lblFileName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancelUpload = new Guna.UI2.WinForms.Guna2Button();
            this.lblFileSize = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.loadingProgressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 25;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoEllipsis = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.ForeColor = System.Drawing.Color.LightGray;
            this.lblFileName.Location = new System.Drawing.Point(67, 22);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(317, 22);
            this.lblFileName.TabIndex = 57;
            this.lblFileName.Text = "dwkajidawd.mp4";
            this.lblFileName.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(90, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 58;
            this.label2.Text = "Uploading...";
            // 
            // btnCancelUpload
            // 
            this.btnCancelUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelUpload.Animated = true;
            this.btnCancelUpload.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.btnCancelUpload.BorderRadius = 6;
            this.btnCancelUpload.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelUpload.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelUpload.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCancelUpload.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCancelUpload.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnCancelUpload.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCancelUpload.ForeColor = System.Drawing.Color.White;
            this.btnCancelUpload.Location = new System.Drawing.Point(399, 39);
            this.btnCancelUpload.Name = "btnCancelUpload";
            this.btnCancelUpload.Size = new System.Drawing.Size(88, 32);
            this.btnCancelUpload.TabIndex = 59;
            this.btnCancelUpload.Text = "Cancel";
            this.btnCancelUpload.TextOffset = new System.Drawing.Point(0, -1);
            this.btnCancelUpload.Click += new System.EventHandler(this.btnCancelUpload_Click);
            // 
            // lblFileSize
            // 
            this.lblFileSize.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileSize.ForeColor = System.Drawing.Color.LightGray;
            this.lblFileSize.Location = new System.Drawing.Point(403, 12);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblFileSize.Size = new System.Drawing.Size(83, 19);
            this.lblFileSize.TabIndex = 67;
            this.lblFileSize.Text = "140MB";
            this.lblFileSize.Click += new System.EventHandler(this.label3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::FlowstorageDesktop.Properties.Resources.giphy__10_;
            this.pictureBox1.Location = new System.Drawing.Point(73, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(11, 10);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 65;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::FlowstorageDesktop.Properties.Resources.download_icon;
            this.pictureBox2.Location = new System.Drawing.Point(14, 22);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(46, 41);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 68;
            this.pictureBox2.TabStop = false;
            // 
            // loadingProgressBar
            // 
            this.loadingProgressBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.loadingProgressBar.Location = new System.Drawing.Point(1, 79);
            this.loadingProgressBar.Name = "loadingProgressBar";
            this.loadingProgressBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.loadingProgressBar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.loadingProgressBar.Size = new System.Drawing.Size(500, 5);
            this.loadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.loadingProgressBar.TabIndex = 69;
            this.loadingProgressBar.Text = "guna2ProgressBar1";
            this.loadingProgressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // UploadingAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(502, 86);
            this.Controls.Add(this.loadingProgressBar);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lblFileSize);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancelUpload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UploadingAlert";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Uploading File";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UploadAlrt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Label label2;
        public Guna.UI2.WinForms.Guna2Button btnCancelUpload;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblFileSize;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Guna.UI2.WinForms.Guna2ProgressBar loadingProgressBar;
        public System.Windows.Forms.Label lblFileName;
    }
}