namespace FlowstorageDesktop {
    partial class SignInForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignInForm));
            this.btnSignIn = new Guna.UI2.WinForms.Guna2Button();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.lblPin = new System.Windows.Forms.Label();
            this.txtFieldPIN = new Guna.UI2.WinForms.Guna2TextBox();
            this.chcBoxRememberMe = new Guna.UI2.WinForms.Guna2CheckBox();
            this.lblAlert = new System.Windows.Forms.Label();
            this.txtFieldAuth = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtFieldEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnCloseForm = new Guna.UI2.WinForms.Guna2Button();
            this.guna2VSeparator1 = new Guna.UI2.WinForms.Guna2VSeparator();
            this.guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            this.btnShowPassword = new Guna.UI2.WinForms.Guna2Button();
            this.btnHidePassword = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // btnSignIn
            // 
            this.btnSignIn.Animated = true;
            this.btnSignIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.btnSignIn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnSignIn.BorderRadius = 12;
            this.btnSignIn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSignIn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSignIn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSignIn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSignIn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.btnSignIn.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.btnSignIn.ForeColor = System.Drawing.Color.White;
            this.btnSignIn.Location = new System.Drawing.Point(50, 321);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(304, 54);
            this.btnSignIn.TabIndex = 15;
            this.btnSignIn.Text = "Sign In";
            this.btnSignIn.TextOffset = new System.Drawing.Point(0, -2);
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPassword.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblPassword.Location = new System.Drawing.Point(55, 207);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(99, 20);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "Password";
            // 
            // lblEmail
            // 
            this.lblEmail.BackColor = System.Drawing.Color.Transparent;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblEmail.Location = new System.Drawing.Point(55, 113);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(99, 20);
            this.lblEmail.TabIndex = 5;
            this.lblEmail.Text = "Email";
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.linkLabel1.Location = new System.Drawing.Point(194, 396);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(149, 17);
            this.linkLabel1.TabIndex = 51;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Forgot your password?";
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
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
            this.guna2CircleButton1.Location = new System.Drawing.Point(180, 403);
            this.guna2CircleButton1.Name = "guna2CircleButton1";
            this.guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton1.Size = new System.Drawing.Size(5, 5);
            this.guna2CircleButton1.TabIndex = 50;
            // 
            // lblPin
            // 
            this.lblPin.BackColor = System.Drawing.Color.Transparent;
            this.lblPin.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPin.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblPin.Location = new System.Drawing.Point(269, 207);
            this.lblPin.Name = "lblPin";
            this.lblPin.Size = new System.Drawing.Size(45, 20);
            this.lblPin.TabIndex = 48;
            this.lblPin.Text = "PIN";
            // 
            // txtFieldPIN
            // 
            this.txtFieldPIN.BackColor = System.Drawing.Color.Transparent;
            this.txtFieldPIN.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldPIN.BorderRadius = 8;
            this.txtFieldPIN.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFieldPIN.DefaultText = "";
            this.txtFieldPIN.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFieldPIN.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFieldPIN.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldPIN.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldPIN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldPIN.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldPIN.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFieldPIN.ForeColor = System.Drawing.Color.White;
            this.txtFieldPIN.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldPIN.Location = new System.Drawing.Point(268, 232);
            this.txtFieldPIN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFieldPIN.MaxLength = 3;
            this.txtFieldPIN.Name = "txtFieldPIN";
            this.txtFieldPIN.PasswordChar = '*';
            this.txtFieldPIN.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtFieldPIN.PlaceholderText = "";
            this.txtFieldPIN.SelectedText = "";
            this.txtFieldPIN.Size = new System.Drawing.Size(86, 49);
            this.txtFieldPIN.TabIndex = 47;
            this.txtFieldPIN.TextChanged += new System.EventHandler(this.guna2TextBox4_TextChanged);
            // 
            // chcBoxRememberMe
            // 
            this.chcBoxRememberMe.AutoSize = true;
            this.chcBoxRememberMe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.chcBoxRememberMe.Checked = true;
            this.chcBoxRememberMe.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.chcBoxRememberMe.CheckedState.BorderRadius = 3;
            this.chcBoxRememberMe.CheckedState.BorderThickness = 1;
            this.chcBoxRememberMe.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.chcBoxRememberMe.CheckMarkColor = System.Drawing.Color.WhiteSmoke;
            this.chcBoxRememberMe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chcBoxRememberMe.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chcBoxRememberMe.ForeColor = System.Drawing.Color.Silver;
            this.chcBoxRememberMe.Location = new System.Drawing.Point(60, 395);
            this.chcBoxRememberMe.Name = "chcBoxRememberMe";
            this.chcBoxRememberMe.Size = new System.Drawing.Size(116, 21);
            this.chcBoxRememberMe.TabIndex = 23;
            this.chcBoxRememberMe.Text = "Remember Me";
            this.chcBoxRememberMe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chcBoxRememberMe.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.chcBoxRememberMe.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.chcBoxRememberMe.UncheckedState.BorderRadius = 3;
            this.chcBoxRememberMe.UncheckedState.BorderThickness = 1;
            this.chcBoxRememberMe.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.chcBoxRememberMe.UseVisualStyleBackColor = false;
            // 
            // lblAlert
            // 
            this.lblAlert.BackColor = System.Drawing.Color.Transparent;
            this.lblAlert.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(0)))), ((int)(((byte)(1)))));
            this.lblAlert.Location = new System.Drawing.Point(49, 291);
            this.lblAlert.Name = "lblAlert";
            this.lblAlert.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblAlert.Size = new System.Drawing.Size(266, 20);
            this.lblAlert.TabIndex = 19;
            this.lblAlert.Text = "Password or PIN number is incorrect";
            this.lblAlert.Visible = false;
            // 
            // txtFieldAuth
            // 
            this.txtFieldAuth.BackColor = System.Drawing.Color.Transparent;
            this.txtFieldAuth.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldAuth.BorderRadius = 8;
            this.txtFieldAuth.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFieldAuth.DefaultText = "";
            this.txtFieldAuth.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFieldAuth.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFieldAuth.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldAuth.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldAuth.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldAuth.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldAuth.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFieldAuth.ForeColor = System.Drawing.Color.White;
            this.txtFieldAuth.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldAuth.Location = new System.Drawing.Point(53, 232);
            this.txtFieldAuth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFieldAuth.Name = "txtFieldAuth";
            this.txtFieldAuth.PasswordChar = '*';
            this.txtFieldAuth.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtFieldAuth.PlaceholderText = "Enter your password";
            this.txtFieldAuth.SelectedText = "";
            this.txtFieldAuth.Size = new System.Drawing.Size(207, 49);
            this.txtFieldAuth.TabIndex = 17;
            // 
            // txtFieldEmail
            // 
            this.txtFieldEmail.BackColor = System.Drawing.Color.Transparent;
            this.txtFieldEmail.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldEmail.BorderRadius = 8;
            this.txtFieldEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFieldEmail.DefaultText = "";
            this.txtFieldEmail.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFieldEmail.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFieldEmail.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldEmail.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFieldEmail.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtFieldEmail.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldEmail.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFieldEmail.ForeColor = System.Drawing.Color.White;
            this.txtFieldEmail.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFieldEmail.Location = new System.Drawing.Point(50, 139);
            this.txtFieldEmail.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFieldEmail.Name = "txtFieldEmail";
            this.txtFieldEmail.PasswordChar = '\0';
            this.txtFieldEmail.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtFieldEmail.PlaceholderText = "Enter your email address";
            this.txtFieldEmail.SelectedText = "";
            this.txtFieldEmail.Size = new System.Drawing.Size(304, 49);
            this.txtFieldEmail.TabIndex = 16;
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 25;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(13, 23);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(203, 25);
            this.lblHeader.TabIndex = 6;
            this.lblHeader.Text = "Sign In to Flowstorage";
            // 
            // btnCloseForm
            // 
            this.btnCloseForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseForm.Animated = true;
            this.btnCloseForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.btnCloseForm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnCloseForm.BorderRadius = 6;
            this.btnCloseForm.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCloseForm.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCloseForm.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCloseForm.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCloseForm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(0)))), ((int)(((byte)(179)))));
            this.btnCloseForm.Location = new System.Drawing.Point(613, 16);
            this.btnCloseForm.Name = "btnCloseForm";
            this.btnCloseForm.Size = new System.Drawing.Size(38, 31);
            this.btnCloseForm.TabIndex = 54;
            this.btnCloseForm.Image = ((System.Drawing.Image)(resources.GetObject("btnCloseForm.Image")));
            this.btnCloseForm.Click += new System.EventHandler(this.btnCloseForm_Click);
            // 
            // guna2VSeparator1
            // 
            this.guna2VSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2VSeparator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2VSeparator1.Location = new System.Drawing.Point(402, 72);
            this.guna2VSeparator1.Name = "guna2VSeparator1";
            this.guna2VSeparator1.Size = new System.Drawing.Size(10, 425);
            this.guna2VSeparator1.TabIndex = 56;
            // 
            // guna2Separator1
            // 
            this.guna2Separator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Separator1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guna2Separator1.Location = new System.Drawing.Point(-18, 66);
            this.guna2Separator1.Name = "guna2Separator1";
            this.guna2Separator1.Size = new System.Drawing.Size(700, 10);
            this.guna2Separator1.TabIndex = 55;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.Animated = true;
            this.btnShowPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnShowPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnShowPassword.BorderRadius = 10;
            this.btnShowPassword.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnShowPassword.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnShowPassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnShowPassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnShowPassword.FillColor = System.Drawing.Color.Empty;
            this.btnShowPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnShowPassword.ForeColor = System.Drawing.Color.White;
            this.btnShowPassword.Image = global::FlowstorageDesktop.Properties.Resources.icons8_closed_eye_241;
            this.btnShowPassword.Location = new System.Drawing.Point(12, 240);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(31, 26);
            this.btnShowPassword.TabIndex = 20;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // btnHidePassword
            // 
            this.btnHidePassword.Animated = true;
            this.btnHidePassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.btnHidePassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(3)))), ((int)(((byte)(153)))));
            this.btnHidePassword.BorderRadius = 10;
            this.btnHidePassword.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHidePassword.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHidePassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHidePassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHidePassword.FillColor = System.Drawing.Color.Empty;
            this.btnHidePassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnHidePassword.ForeColor = System.Drawing.Color.White;
            this.btnHidePassword.Image = global::FlowstorageDesktop.Properties.Resources.icons8_eye_24;
            this.btnHidePassword.Location = new System.Drawing.Point(12, 240);
            this.btnHidePassword.Name = "btnHidePassword";
            this.btnHidePassword.Size = new System.Drawing.Size(31, 26);
            this.btnHidePassword.TabIndex = 21;
            this.btnHidePassword.Visible = false;
            this.btnHidePassword.Click += new System.EventHandler(this.btnHidePassword_Click);
            // 
            // SignInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.ClientSize = new System.Drawing.Size(663, 498);
            this.Controls.Add(this.guna2VSeparator1);
            this.Controls.Add(this.guna2Separator1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.guna2CircleButton1);
            this.Controls.Add(this.btnCloseForm);
            this.Controls.Add(this.lblPin);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.txtFieldPIN);
            this.Controls.Add(this.btnShowPassword);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.btnHidePassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.chcBoxRememberMe);
            this.Controls.Add(this.btnSignIn);
            this.Controls.Add(this.lblAlert);
            this.Controls.Add(this.txtFieldEmail);
            this.Controls.Add(this.txtFieldAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SignInForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign In";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Guna.UI2.WinForms.Guna2Button btnSignIn;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblAlert;
        private Guna.UI2.WinForms.Guna2TextBox txtFieldAuth;
        public Guna.UI2.WinForms.Guna2TextBox txtFieldEmail;
        public Guna.UI2.WinForms.Guna2Button btnShowPassword;
        public Guna.UI2.WinForms.Guna2Button btnHidePassword;
        private Guna.UI2.WinForms.Guna2CheckBox chcBoxRememberMe;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Label lblHeader;
        public Guna.UI2.WinForms.Guna2TextBox txtFieldPIN;
        private System.Windows.Forms.Label lblPin;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private Guna.UI2.WinForms.Guna2VSeparator guna2VSeparator1;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2Button btnCloseForm;
    }
}