namespace FlowSERVER1
{
    partial class DirectoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectoryForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.guna2Button6 = new Guna.UI2.WinForms.Guna2Button();
            this.flwLayoutDirectory = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUploadFile = new Guna.UI2.WinForms.Guna2Button();
            this.guna2VSeparator1 = new Guna.UI2.WinForms.Guna2VSeparator();
            this.btnCloseDirectory = new Guna.UI2.WinForms.Guna2Button();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblDirectoryName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.pnlFileOptions = new Guna.UI2.WinForms.Guna2Panel();
            this.lblSharedToName = new System.Windows.Forms.Label();
            this.lblSelectedDirName = new System.Windows.Forms.Label();
            this.lblFileTableName = new System.Windows.Forms.Label();
            this.lblFilePanelName = new System.Windows.Forms.Label();
            this.btnDownload = new Guna.UI2.WinForms.Guna2Button();
            this.btnShareFile = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.btnRenameFile = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button28 = new Guna.UI2.WinForms.Guna2Button();
            this.lblFileNameOnPanel = new System.Windows.Forms.Label();
            this.guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.lblFilesCount = new System.Windows.Forms.Label();
            this.guna2Panel3.SuspendLayout();
            this.pnlFileOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2Panel3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.guna2Panel3.BorderRadius = 8;
            this.guna2Panel3.BorderThickness = 1;
            this.guna2Panel3.Controls.Add(this.label8);
            this.guna2Panel3.Controls.Add(this.guna2Button6);
            this.guna2Panel3.Controls.Add(this.flwLayoutDirectory);
            this.guna2Panel3.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.guna2Panel3.Location = new System.Drawing.Point(12, 80);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(1104, 568);
            this.guna2Panel3.TabIndex = 20;
            this.guna2Panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.guna2Panel3_Paint);
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Silver;
            this.label8.Location = new System.Drawing.Point(479, 260);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(214, 32);
            this.label8.TabIndex = 28;
            this.label8.Text = "Directory is empty";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // guna2Button6
            // 
            this.guna2Button6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.guna2Button6.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button6.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button6.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button6.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button6.FillColor = System.Drawing.Color.Empty;
            this.guna2Button6.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2Button6.ForeColor = System.Drawing.Color.White;
            this.guna2Button6.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button6.Image")));
            this.guna2Button6.ImageSize = new System.Drawing.Size(65, 65);
            this.guna2Button6.Location = new System.Drawing.Point(539, 175);
            this.guna2Button6.Name = "guna2Button6";
            this.guna2Button6.Size = new System.Drawing.Size(76, 92);
            this.guna2Button6.TabIndex = 27;
            this.guna2Button6.Click += new System.EventHandler(this.guna2Button6_Click);
            // 
            // flwLayoutDirectory
            // 
            this.flwLayoutDirectory.AutoScroll = true;
            this.flwLayoutDirectory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.flwLayoutDirectory.Location = new System.Drawing.Point(16, 11);
            this.flwLayoutDirectory.Name = "flwLayoutDirectory";
            this.flwLayoutDirectory.Size = new System.Drawing.Size(1075, 551);
            this.flwLayoutDirectory.TabIndex = 16;
            this.flwLayoutDirectory.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // btnUploadFile
            // 
            this.btnUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUploadFile.Animated = true;
            this.btnUploadFile.BorderRadius = 10;
            this.btnUploadFile.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnUploadFile.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnUploadFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnUploadFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnUploadFile.FillColor = System.Drawing.Color.Empty;
            this.btnUploadFile.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.btnUploadFile.ForeColor = System.Drawing.Color.LightGray;
            this.btnUploadFile.Image = ((System.Drawing.Image)(resources.GetObject("btnUploadFile.Image")));
            this.btnUploadFile.ImageSize = new System.Drawing.Size(26, 26);
            this.btnUploadFile.Location = new System.Drawing.Point(882, 15);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(127, 47);
            this.btnUploadFile.TabIndex = 30;
            this.btnUploadFile.Text = "Upload File";
            this.btnUploadFile.TextOffset = new System.Drawing.Point(4, 0);
            this.btnUploadFile.Click += new System.EventHandler(this.guna2Button2_Click_1);
            // 
            // guna2VSeparator1
            // 
            this.guna2VSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2VSeparator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.guna2VSeparator1.Location = new System.Drawing.Point(841, 16);
            this.guna2VSeparator1.Name = "guna2VSeparator1";
            this.guna2VSeparator1.Size = new System.Drawing.Size(10, 47);
            this.guna2VSeparator1.TabIndex = 32;
            this.guna2VSeparator1.Click += new System.EventHandler(this.guna2VSeparator1_Click);
            // 
            // btnCloseDirectory
            // 
            this.btnCloseDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseDirectory.Animated = true;
            this.btnCloseDirectory.BackColor = System.Drawing.Color.Transparent;
            this.btnCloseDirectory.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnCloseDirectory.BorderRadius = 6;
            this.btnCloseDirectory.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCloseDirectory.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCloseDirectory.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCloseDirectory.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCloseDirectory.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.btnCloseDirectory.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnCloseDirectory.ForeColor = System.Drawing.Color.White;
            this.btnCloseDirectory.Image = ((System.Drawing.Image)(resources.GetObject("btnCloseDirectory.Image")));
            this.btnCloseDirectory.Location = new System.Drawing.Point(1076, 12);
            this.btnCloseDirectory.Name = "btnCloseDirectory";
            this.btnCloseDirectory.Size = new System.Drawing.Size(38, 31);
            this.btnCloseDirectory.TabIndex = 33;
            this.btnCloseDirectory.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblDirectoryName
            // 
            this.lblDirectoryName.AutoEllipsis = true;
            this.lblDirectoryName.BackColor = System.Drawing.Color.Transparent;
            this.lblDirectoryName.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDirectoryName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDirectoryName.Location = new System.Drawing.Point(12, 14);
            this.lblDirectoryName.Name = "lblDirectoryName";
            this.lblDirectoryName.Size = new System.Drawing.Size(792, 28);
            this.lblDirectoryName.TabIndex = 7;
            this.lblDirectoryName.Text = "Directory: ";
            this.lblDirectoryName.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Silver;
            this.label2.Location = new System.Drawing.Point(14, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Directory";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Separator1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Separator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.guna2Separator1.Location = new System.Drawing.Point(1, 70);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(1130, 10);
            this.guna2Separator1.TabIndex = 72;
            // 
            // pnlFileOptions
            // 
            this.pnlFileOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFileOptions.BackColor = System.Drawing.Color.Transparent;
            this.pnlFileOptions.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.pnlFileOptions.BorderRadius = 14;
            this.pnlFileOptions.BorderThickness = 1;
            this.pnlFileOptions.Controls.Add(this.lblSharedToName);
            this.pnlFileOptions.Controls.Add(this.lblSelectedDirName);
            this.pnlFileOptions.Controls.Add(this.lblFileTableName);
            this.pnlFileOptions.Controls.Add(this.lblFilePanelName);
            this.pnlFileOptions.Controls.Add(this.btnDownload);
            this.pnlFileOptions.Controls.Add(this.btnShareFile);
            this.pnlFileOptions.Controls.Add(this.btnDelete);
            this.pnlFileOptions.Controls.Add(this.btnRenameFile);
            this.pnlFileOptions.Controls.Add(this.guna2Button28);
            this.pnlFileOptions.Controls.Add(this.lblFileNameOnPanel);
            this.pnlFileOptions.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.pnlFileOptions.ForeColor = System.Drawing.Color.Transparent;
            this.pnlFileOptions.Location = new System.Drawing.Point(776, 399);
            this.pnlFileOptions.Name = "pnlFileOptions";
            this.pnlFileOptions.Size = new System.Drawing.Size(314, 232);
            this.pnlFileOptions.TabIndex = 73;
            this.pnlFileOptions.Visible = false;
            // 
            // lblSharedToName
            // 
            this.lblSharedToName.AutoSize = true;
            this.lblSharedToName.BackColor = System.Drawing.Color.Transparent;
            this.lblSharedToName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSharedToName.ForeColor = System.Drawing.Color.White;
            this.lblSharedToName.Location = new System.Drawing.Point(193, 124);
            this.lblSharedToName.Name = "lblSharedToName";
            this.lblSharedToName.Size = new System.Drawing.Size(119, 21);
            this.lblSharedToName.TabIndex = 55;
            this.lblSharedToName.Text = "sharedToName";
            this.lblSharedToName.Visible = false;
            // 
            // lblSelectedDirName
            // 
            this.lblSelectedDirName.AutoSize = true;
            this.lblSelectedDirName.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedDirName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedDirName.ForeColor = System.Drawing.Color.White;
            this.lblSelectedDirName.Location = new System.Drawing.Point(193, 153);
            this.lblSelectedDirName.Name = "lblSelectedDirName";
            this.lblSelectedDirName.Size = new System.Drawing.Size(73, 21);
            this.lblSelectedDirName.TabIndex = 54;
            this.lblSelectedDirName.Text = "dirName";
            this.lblSelectedDirName.Visible = false;
            // 
            // lblFileTableName
            // 
            this.lblFileTableName.AutoSize = true;
            this.lblFileTableName.BackColor = System.Drawing.Color.Transparent;
            this.lblFileTableName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileTableName.ForeColor = System.Drawing.Color.White;
            this.lblFileTableName.Location = new System.Drawing.Point(193, 177);
            this.lblFileTableName.Name = "lblFileTableName";
            this.lblFileTableName.Size = new System.Drawing.Size(90, 21);
            this.lblFileTableName.TabIndex = 53;
            this.lblFileTableName.Text = "tableName";
            this.lblFileTableName.Visible = false;
            // 
            // lblFilePanelName
            // 
            this.lblFilePanelName.AutoSize = true;
            this.lblFilePanelName.BackColor = System.Drawing.Color.Transparent;
            this.lblFilePanelName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilePanelName.ForeColor = System.Drawing.Color.White;
            this.lblFilePanelName.Location = new System.Drawing.Point(193, 200);
            this.lblFilePanelName.Name = "lblFilePanelName";
            this.lblFilePanelName.Size = new System.Drawing.Size(93, 21);
            this.lblFilePanelName.TabIndex = 52;
            this.lblFilePanelName.Text = "panelName";
            this.lblFilePanelName.Visible = false;
            // 
            // btnDownload
            // 
            this.btnDownload.Animated = true;
            this.btnDownload.BackColor = System.Drawing.Color.Transparent;
            this.btnDownload.BorderRadius = 10;
            this.btnDownload.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDownload.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDownload.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDownload.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDownload.FillColor = System.Drawing.Color.Transparent;
            this.btnDownload.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnDownload.ForeColor = System.Drawing.Color.LightGray;
            this.btnDownload.Image = ((System.Drawing.Image)(resources.GetObject("btnDownload.Image")));
            this.btnDownload.ImageSize = new System.Drawing.Size(22, 22);
            this.btnDownload.Location = new System.Drawing.Point(14, 134);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(117, 31);
            this.btnDownload.TabIndex = 51;
            this.btnDownload.Text = "Download";
            this.btnDownload.TextOffset = new System.Drawing.Point(1, 0);
            this.btnDownload.Click += new System.EventHandler(this.guna2Button32_Click);
            // 
            // btnShareFile
            // 
            this.btnShareFile.Animated = true;
            this.btnShareFile.BackColor = System.Drawing.Color.Transparent;
            this.btnShareFile.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnShareFile.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnShareFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnShareFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnShareFile.FillColor = System.Drawing.Color.Empty;
            this.btnShareFile.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShareFile.ForeColor = System.Drawing.Color.LightGray;
            this.btnShareFile.Image = global::FlowSERVER1.Properties.Resources.icons8_share_64;
            this.btnShareFile.ImageOffset = new System.Drawing.Point(1, 0);
            this.btnShareFile.ImageSize = new System.Drawing.Size(24, 24);
            this.btnShareFile.Location = new System.Drawing.Point(14, 92);
            this.btnShareFile.Name = "btnShareFile";
            this.btnShareFile.Size = new System.Drawing.Size(136, 31);
            this.btnShareFile.TabIndex = 38;
            this.btnShareFile.Text = "Share this file";
            this.btnShareFile.TextOffset = new System.Drawing.Point(1, 0);
            this.btnShareFile.Click += new System.EventHandler(this.guna2Button29_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.BorderColor = System.Drawing.Color.Empty;
            this.btnDelete.BorderRadius = 5;
            this.btnDelete.BorderThickness = 1;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.btnDelete.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDelete.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDelete.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDelete.FillColor = System.Drawing.Color.Empty;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.LightGray;
            this.btnDelete.Image = global::FlowSERVER1.Properties.Resources.icons8_garbage_66;
            this.btnDelete.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnDelete.ImageSize = new System.Drawing.Size(26, 26);
            this.btnDelete.Location = new System.Drawing.Point(10, 179);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(129, 29);
            this.btnDelete.TabIndex = 39;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextOffset = new System.Drawing.Point(1, 0);
            this.btnDelete.Click += new System.EventHandler(this.guna2Button26_Click);
            // 
            // btnRenameFile
            // 
            this.btnRenameFile.Animated = true;
            this.btnRenameFile.BackColor = System.Drawing.Color.Transparent;
            this.btnRenameFile.BorderRadius = 10;
            this.btnRenameFile.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnRenameFile.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnRenameFile.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnRenameFile.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnRenameFile.FillColor = System.Drawing.Color.Empty;
            this.btnRenameFile.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRenameFile.ForeColor = System.Drawing.Color.LightGray;
            this.btnRenameFile.Image = global::FlowSERVER1.Properties.Resources.icons8_edit_48;
            this.btnRenameFile.Location = new System.Drawing.Point(18, 51);
            this.btnRenameFile.Name = "btnRenameFile";
            this.btnRenameFile.Size = new System.Drawing.Size(147, 31);
            this.btnRenameFile.TabIndex = 37;
            this.btnRenameFile.Text = "Rename this file";
            this.btnRenameFile.TextOffset = new System.Drawing.Point(1, 0);
            this.btnRenameFile.Click += new System.EventHandler(this.guna2Button30_Click);
            // 
            // guna2Button28
            // 
            this.guna2Button28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Button28.Animated = true;
            this.guna2Button28.BackColor = System.Drawing.Color.Transparent;
            this.guna2Button28.BorderColor = System.Drawing.Color.Transparent;
            this.guna2Button28.BorderRadius = 6;
            this.guna2Button28.BorderThickness = 1;
            this.guna2Button28.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button28.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button28.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button28.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button28.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.guna2Button28.Font = new System.Drawing.Font("Dubai", 14.25F, System.Drawing.FontStyle.Bold);
            this.guna2Button28.ForeColor = System.Drawing.Color.White;
            this.guna2Button28.Image = ((System.Drawing.Image)(resources.GetObject("guna2Button28.Image")));
            this.guna2Button28.ImageSize = new System.Drawing.Size(18, 18);
            this.guna2Button28.Location = new System.Drawing.Point(267, 11);
            this.guna2Button28.Name = "guna2Button28";
            this.guna2Button28.Size = new System.Drawing.Size(35, 32);
            this.guna2Button28.TabIndex = 49;
            this.guna2Button28.Click += new System.EventHandler(this.guna2Button28_Click);
            // 
            // lblFileNameOnPanel
            // 
            this.lblFileNameOnPanel.AutoEllipsis = true;
            this.lblFileNameOnPanel.BackColor = System.Drawing.Color.Transparent;
            this.lblFileNameOnPanel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileNameOnPanel.ForeColor = System.Drawing.Color.White;
            this.lblFileNameOnPanel.Location = new System.Drawing.Point(9, 11);
            this.lblFileNameOnPanel.Name = "lblFileNameOnPanel";
            this.lblFileNameOnPanel.Size = new System.Drawing.Size(253, 25);
            this.lblFileNameOnPanel.TabIndex = 48;
            this.lblFileNameOnPanel.Text = "somenwordfile.png";
            // 
            // guna2CircleButton1
            // 
            this.guna2CircleButton1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2CircleButton1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2CircleButton1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2CircleButton1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.guna2CircleButton1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton1.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton1.Location = new System.Drawing.Point(82, 52);
            this.guna2CircleButton1.Name = "guna2CircleButton1";
            this.guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton1.Size = new System.Drawing.Size(6, 6);
            this.guna2CircleButton1.TabIndex = 75;
            // 
            // lblFilesCount
            // 
            this.lblFilesCount.AutoSize = true;
            this.lblFilesCount.BackColor = System.Drawing.Color.Transparent;
            this.lblFilesCount.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilesCount.ForeColor = System.Drawing.Color.Silver;
            this.lblFilesCount.Location = new System.Drawing.Point(93, 45);
            this.lblFilesCount.Name = "lblFilesCount";
            this.lblFilesCount.Size = new System.Drawing.Size(50, 17);
            this.lblFilesCount.TabIndex = 76;
            this.lblFilesCount.Text = "12 Files";
            // 
            // DirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.ClientSize = new System.Drawing.Size(1128, 655);
            this.Controls.Add(this.lblFilesCount);
            this.Controls.Add(this.guna2CircleButton1);
            this.Controls.Add(this.pnlFileOptions);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCloseDirectory);
            this.Controls.Add(this.lblDirectoryName);
            this.Controls.Add(this.guna2VSeparator1);
            this.Controls.Add(this.btnUploadFile);
            this.Controls.Add(this.guna2Panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DirectoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Directory";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.guna2Panel3.ResumeLayout(false);
            this.guna2Panel3.PerformLayout();
            this.pnlFileOptions.ResumeLayout(false);
            this.pnlFileOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        public System.Windows.Forms.FlowLayoutPanel flwLayoutDirectory;
        private Guna.UI2.WinForms.Guna2Button btnUploadFile;
        private Guna.UI2.WinForms.Guna2VSeparator guna2VSeparator1;
        public Guna.UI2.WinForms.Guna2Button btnCloseDirectory;
        public System.Windows.Forms.Label label8;
        public Guna.UI2.WinForms.Guna2Button guna2Button6;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label lblDirectoryName;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2Panel pnlFileOptions;
        private System.Windows.Forms.Label lblSharedToName;
        private System.Windows.Forms.Label lblSelectedDirName;
        private System.Windows.Forms.Label lblFileTableName;
        private System.Windows.Forms.Label lblFilePanelName;
        private Guna.UI2.WinForms.Guna2Button btnDownload;
        private Guna.UI2.WinForms.Guna2Button btnShareFile;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnRenameFile;
        public Guna.UI2.WinForms.Guna2Button guna2Button28;
        private System.Windows.Forms.Label lblFileNameOnPanel;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        public System.Windows.Forms.Label lblFilesCount;
    }
}