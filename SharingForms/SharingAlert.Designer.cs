namespace FlowstorageDesktop.Sharing {
    partial class SharingAlert {
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
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.btnCancelSharing = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblSharingTo = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 25;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.HasFormShadow = false;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // btnCancelSharing
            // 
            this.btnCancelSharing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelSharing.Animated = true;
            this.btnCancelSharing.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.btnCancelSharing.BorderRadius = 7;
            this.btnCancelSharing.BorderThickness = 2;
            this.btnCancelSharing.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelSharing.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCancelSharing.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCancelSharing.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCancelSharing.FillColor = System.Drawing.Color.Empty;
            this.btnCancelSharing.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelSharing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.btnCancelSharing.Location = new System.Drawing.Point(423, 109);
            this.btnCancelSharing.Name = "btnCancelSharing";
            this.btnCancelSharing.Size = new System.Drawing.Size(75, 25);
            this.btnCancelSharing.TabIndex = 62;
            this.btnCancelSharing.Text = "Cancel";
            this.btnCancelSharing.Click += new System.EventHandler(this.btnCancelSharing_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::FlowstorageDesktop.Properties.Resources.giphy__10_;
            this.pictureBox1.Location = new System.Drawing.Point(469, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(26, 26);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 61;
            this.pictureBox1.TabStop = false;
            // 
            // lblSharingTo
            // 
            this.lblSharingTo.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSharingTo.ForeColor = System.Drawing.Color.LightGray;
            this.lblSharingTo.Location = new System.Drawing.Point(24, 58);
            this.lblSharingTo.Name = "lblSharingTo";
            this.lblSharingTo.Size = new System.Drawing.Size(352, 20);
            this.lblSharingTo.TabIndex = 64;
            this.lblSharingTo.Text = "Sharing to danishisglesbian";
            this.lblSharingTo.Click += new System.EventHandler(this.lblSharingTo_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(20, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 25);
            this.label9.TabIndex = 63;
            this.label9.Text = "Sharing...";
            // 
            // SharingAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(508, 149);
            this.Controls.Add(this.lblSharingTo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnCancelSharing);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SharingAlert";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SharingAlert_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        public Guna.UI2.WinForms.Guna2Button btnCancelSharing;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblSharingTo;
        private System.Windows.Forms.Label label9;
    }
}