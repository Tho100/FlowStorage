using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Diagnostics;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;

namespace FlowSERVER1 {
    public partial class sharingFORM : Form {
        private static String _FileName;
        private static String _FilePath;
        private static String _retrieved;
        private static String _getExt;
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static sharingFORM instance;
        public String _verifySetPas = "";
        public sharingFORM() {
            InitializeComponent();
            instance = this;
        }

        private void sharingFORM_Load(object sender, EventArgs e) {

            Application.DoEvents();

            flowLayoutPanel1.Controls.Clear();
            String getFilesType = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
            command = new MySqlCommand(getFilesType, con);
            command = con.CreateCommand();
            command.CommandText = getFilesType;
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

            List<String> _TypeValues = new List<String>();

            Application.DoEvents();

            MySqlDataReader _readType = command.ExecuteReader();
            while (_readType.Read()) {
                _TypeValues.Add(_readType.GetString(0));// Append ToAr;
            }
            _readType.Close();
            generateUserShared(_TypeValues, "DIRPAR", _TypeValues.Count);

            Application.DoEvents();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// This function will determine if the 
        /// user is exists or not 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int userIsExists(String _receiverUsername) {
            String countUser = "SELECT COUNT(*) FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(countUser,con);
            command.Parameters.AddWithValue("@username",_receiverUsername);
            var setupCount = command.ExecuteScalar();
            int ToInt = Convert.ToInt32(setupCount);
            return ToInt;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            OpenFileDialog _OpenDialog = new OpenFileDialog();
            _OpenDialog.Filter = "All Files|*.*";
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                var getEx = _OpenDialog.FileName;
                var getName = _OpenDialog.SafeFileName;
                var retrieved = Path.GetExtension(getEx);
                _FileName = getName;
                _FilePath = _OpenDialog.FileName;
                _getExt = getEx;
                _retrieved = retrieved;
                guna2TextBox2.Text = getName;
            }
        }

        /// <summary>
        /// Main function for sharing file 
        /// </summary>
        private void startSharing() {
            int _accType = accountType(guna2TextBox1.Text);
            int _countReceiverFile = countReceiverShared(guna2TextBox1.Text);
            long fileSizeInMB = 0;

            if (_accType != _countReceiverFile) {
                if (_currentFileName != guna2TextBox2.Text) {

                    Byte[] _getBytes = File.ReadAllBytes(_FilePath);
                    fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                    Thread showUploadAlert = new Thread(() => new UploadAlrt(_FileName, Form1.instance.label5.Text, "cust_sharing", _controlName, guna2TextBox1.Text, _fileSize: fileSizeInMB).ShowDialog());
                    showUploadAlert.Start();

                    String varDate = DateTime.Now.ToString("dd/MM/yyyy");
                    String insertQuery = "INSERT INTO cust_sharing (CUST_TO,CUST_FROM,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE,FILE_EXT,CUST_THUMB) VALUES (@CUST_TO,@CUST_FROM,@CUST_FILE_PATH,@UPLOAD_DATE,@CUST_FILE,@FILE_EXT,@CUST_THUMB)";
                    command = new MySqlCommand(insertQuery, con);

                    command.Parameters.Add("@CUST_TO", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_FROM", MySqlDbType.Text);
                    command.Parameters.Add("@CUST_THUMB", MySqlDbType.LongBlob);
                    command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text);
                    command.Parameters.Add("@FILE_EXT", MySqlDbType.Text);
                    command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255);
                    command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob);

                    command.Parameters["@CUST_FROM"].Value = Form1.instance.label5.Text;
                    command.Parameters["@CUST_TO"].Value = guna2TextBox1.Text;
                    command.Parameters["@CUST_FILE_PATH"].Value = _FileName;
                    command.Parameters["@UPLOAD_DATE"].Value = varDate;
                    command.Parameters["@FILE_EXT"].Value = _retrieved;

                    Application.DoEvents();

                    _currentFileName = guna2TextBox2.Text;

                    if (_retrieved == ".png" || _retrieved == ".jpg" || _retrieved == ".jpeg" || _retrieved == ".bmp") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;//File.ReadAllBytes(_FilePath);
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".docx" || _retrieved == ".doc") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".pptx" || _retrieved == ".ppt") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".exe") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".mp3" || _retrieved == ".wav") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".pdf") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".apk") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".xlsx" || _retrieved == ".xls") {
                        var _toBase64 = Convert.ToBase64String(_getBytes);
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".gif") {
                        ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64String = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64String;
                        }
                        var _toBase64 = Convert.ToBase64String(File.ReadAllBytes(_FilePath));
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".txt" || _retrieved == ".html" || _retrieved == ".xml" || _retrieved == ".py" || _retrieved == ".css" || _retrieved == ".js" || _retrieved == ".sql") {
                        var nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_FilePath)) { //open.FileName
                            nonLine = ReadFileTxt.ReadToEnd();
                        }
                        var encryptValue = EncryptionModel.EncryptText(nonLine);
                        command.Parameters["@CUST_FILE"].Value = encryptValue;
                        command.ExecuteNonQuery();
                    }
                    else if (_retrieved == ".mp4" || _retrieved == ".mov" || _retrieved == ".webm" || _retrieved == ".avi" || _retrieved == ".wmv") {
                        ShellFile shellFile = ShellFile.FromFilePath(_FilePath);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64Str = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64Str;// To load: Bitmap -> Byte array
                        }
                        var _toBase64 = Convert.ToBase64String(File.ReadAllBytes(_FilePath));
                        command.Parameters["@CUST_FILE"].Value = _toBase64;
                        command.ExecuteNonQuery();
                    }

                    Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "UploadAlrt"))
                        .ToList()
                        .ForEach(form => form.Close());

                    sucessShare _showSuccessfullyTransaction = new sucessShare(guna2TextBox2.Text, guna2TextBox1.Text);
                    _showSuccessfullyTransaction.Show();

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "UploadAlrt"))
                    .ToList()
                    .ForEach(form => form.Close());

                    Application.DoEvents();

                }
                else {
                    MessageBox.Show("File is already sent.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else {
                MessageBox.Show("The receiver has reached the limit amount of files they can received.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// This function will retrieves user 
        /// account type 
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int accountType(String _receiverUsername) {

            int _allowedReturn = 12;
            String _accType = "";

            String _getAccountTypeQue = "SELECT acc_type FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccountTypeQue,con);
            command.Parameters.AddWithValue("@username",_receiverUsername);
            
            MySqlDataReader _readAccType = command.ExecuteReader();
            if(_readAccType.Read()) {
                _accType = _readAccType.GetString(0);
            }
            _readAccType.Close();
            if(_accType == "Max") {
                _allowedReturn = 30;
            } else if (_accType == "Express") {
                _allowedReturn = 50;
            } else if (_accType == "Supreme") {
                _allowedReturn = 170;
            } else if (_accType == "Basic") {
                _allowedReturn = 12;
            }
            return _allowedReturn;
        }

        /// <summary>
        /// This function will do check if the 
        /// receiver has password enabled for their file sharing
        /// </summary>
        private static string _hasPas = "DEF";
        private string hasPassword(String _custUsername) {
            String _storeVal = "";
            String _queryGet = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_queryGet,con);
            command.Parameters.AddWithValue("@username",_custUsername);

            MySqlDataReader _readPas = command.ExecuteReader();
            while(_readPas.Read()) {
                _hasPas = _readPas.GetString(0);
            }
            _readPas.Close();
            if(_hasPas == "DEF") {
                _hasPas = "DEF";
            } else {
                _storeVal = _hasPas;
            }
            return _storeVal;
        }

        /// <summary>
        /// This function will count the number of 
        /// files the user has been shared
        /// </summary>
        /// <param name="_receiverUsername"></param>
        /// <returns></returns>
        private int countReceiverShared(String _receiverUsername) {
            String _countFileShared = "SELECT COUNT(*) FROM cust_sharing WHERE CUST_TO = @username";
            command = new MySqlCommand(_countFileShared, con);
            command.Parameters.AddWithValue("@username", _receiverUsername);

            var _getValue = command.ExecuteScalar();
            int _toInt = Convert.ToInt32(_getValue);
            return _toInt;
            
        }

        /// <summary>
        /// This function will retrieve user current 
        /// file sharing status (enabeled or disabled)
        /// </summary>
        private static string _isEnabled = "";
        private String retrieveDisabled(String _custUsername) {
            String _concludeOutput = "";
            String _queryRetrieve = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_queryRetrieve,con);
            command.Parameters.AddWithValue("@username",_custUsername);

            MySqlDataReader _readDisabled = command.ExecuteReader();
            while(_readDisabled.Read()) {
                _isEnabled = _readDisabled.GetString(0);
            }
            _readDisabled.Close();

            if(_isEnabled == "1") {
                _concludeOutput = "1";
            } else {
                _concludeOutput = "0";
            }
            return _concludeOutput;
        }

        private static String _controlName = null;
        private static String _currentFileName = "";

        /// <summary>
        /// Button to start file sharing 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {
            try {

                if(guna2TextBox1.Text != Form1.instance.label5.Text) {
                    if(guna2TextBox1.Text != String.Empty) {
                        if(guna2TextBox2.Text != String.Empty) {
                            if(userIsExists(guna2TextBox1.Text) > 0) {
                                if(!(retrieveDisabled(guna2TextBox1.Text) == "0")) {
                                    MessageBox.Show("The user " + guna2TextBox1.Text + " disabled their file sharing.","Sharing Failed",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                } else {
                                    if(hasPassword(guna2TextBox1.Text) != "") {
                                        AskPassSharing _askPassForm = new AskPassSharing(guna2TextBox1.Text,guna2TextBox2.Text,_FilePath,_retrieved);
                                        _askPassForm.Show();
                                    } else {
                                        startSharing();
                                    }
                            }
                        }
                        else {
                            MessageBox.Show("User '" + guna2TextBox1.Text + "' not found.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                      }
                    }
                } else {
                    MessageBox.Show("You can't share to yourself.", "Sharing Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception eq) {
                MessageBox.Show("An unknown error occurred.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            flowLayoutPanel1.Controls.Clear();
            String getFilesType = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
            command = new MySqlCommand(getFilesType, con);
            command = con.CreateCommand();
            command.CommandText = getFilesType;
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

            List<String> _TypeValues = new List<String>();

            Application.DoEvents();
            MySqlDataReader _readType = command.ExecuteReader();
            while (_readType.Read()) {
                _TypeValues.Add(_readType.GetString(0));// Append ToAr;
            }
            _readType.Close();
            generateUserShared(_TypeValues, "DIRPAR", _TypeValues.Count);
        }

        private string uploaderName() {
            String _theName = "";
            List<String> _returnName = new List<string>();
            String selectUploaderName = "SELECT CUST_FROM FROM cust_sharing where CUST_TO = @username";
            command = new MySqlCommand(selectUploaderName,con);
            command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);

            MySqlDataReader _ReadUser = command.ExecuteReader();
            if(_ReadUser.Read()) {
                _returnName.Add(_ReadUser.GetString(0));
            }
            _ReadUser.Close();
            if(_returnName.Count > 0) {
                _theName = _returnName[0];
            }
            return _theName;
        }

        private void generateUserShared(List<String> _extTypes, String parameterName, int itemCurr) {

            Application.DoEvents();

            var form1 = Form1.instance;
            var UploaderUsername = uploaderName();
            String varDate = DateTime.Now.ToString("dd/MM/yyyy");
            flowLayoutPanel1.Controls.Clear();

            List<String> typeValues = new List<String>(_extTypes);
            for (int q = 0; q < itemCurr; q++) {
                int top = 275;
                int h_p = 100;
                var panelTxt = new Guna2Panel() {
                    Name = parameterName + q,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelTxt);
                var mainPanelTxt = ((Guna2Panel)flowLayoutPanel1.Controls[parameterName + q]);
                _controlName = parameterName + q;

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = "TxtBox" + q;
                textboxPic.Width = 240;
                textboxPic.Height = 164;
                textboxPic.BorderRadius = 8;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                List<String> filesNames = new List<string>();

                String _selectFileName = "SELECT CUST_FILE_PATH FROM cust_sharing WHERE CUST_TO = @username";
                command = new MySqlCommand(_selectFileName, con);
                command = con.CreateCommand();
                command.CommandText = _selectFileName;
                command.Parameters.AddWithValue("@username", form1.label5.Text);

                MySqlDataReader _readFileNames = command.ExecuteReader();
                while (_readFileNames.Read()) {
                    filesNames.Add(_readFileNames.GetString(0));
                }
                _readFileNames.Close();

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = "LabVidUp" + q;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesNames[q];

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = "RemTxtBut" + q;
                remButTxt.Width = 39;
                remButTxt.Height = 35;
                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                remButTxt.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remButTxt.Visible = true;
                remButTxt.Location = new Point(189, 218);
                remButTxt.BringToFront();

                remButTxt.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

                        mainPanelTxt.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label5.Visible = true;
                        }
                        label6.Text = "Files Count: " + flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = "LabTxtUp" + q;
                dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                dateLabTxt.ForeColor = Color.DarkGray;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = new Point(12, 208);
                dateLabTxt.Width = 1000;
                dateLabTxt.Text = varDate;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                Application.DoEvents();

                if (typeValues[q] == ".png" || typeValues[q] == ".jpeg" || typeValues[q] == ".jpg" || typeValues[q] == ".bmp") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    Application.DoEvents();
                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[q]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    textboxPic.Image = new Bitmap(_toMs);
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text,UploaderUsername)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayPic.Owner = bgBlur;
                            displayPic.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".pptx" || typeValues[q] == ".pptx") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", label1.Text,UploaderUsername);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".pdf") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", label1.Text,UploaderUsername);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".apk") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (apkFORM displayApk = new apkFORM(titleLab.Text, UploaderUsername, "cust_sharing",label1.Text)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayApk.Owner = bgBlur;
                            displayApk.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".docx" || typeValues[q] == ".doc") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", label1.Text,UploaderUsername);
                        displayDoc.Show();
                    };
                }

                if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    textboxPic.Click += (sender_im, e_im) => {
                        exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername);
                        displayXls.Show();
                    };        
                }

                if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", label1.Text,UploaderUsername);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".mp4" || typeValues[q] == ".mov" || typeValues[q] == ".webm" || typeValues[q] == ".avi" || typeValues[q] == ".wmv") {
                    String getImgQue = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(getImgQue, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    MySqlDataAdapter da_Read = new MySqlDataAdapter(command);
                    DataSet ds_Read = new DataSet();
                    da_Read.Fill(ds_Read);
                    MemoryStream ms = new MemoryStream((byte[])ds_Read.Tables[0].Rows[q]["CUST_THUMB"]);

                    textboxPic.Image = new Bitmap(ms);
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        vidFORM displayAud = new vidFORM(defaultImage,getWidth,getHeight,titleLab.Text, "cust_sharing", label1.Text, UploaderUsername);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".exe") {
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", label1.Text,UploaderUsername)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayExe.Owner = bgBlur;
                            displayExe.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".txt" || typeValues[q] == ".html" || typeValues[q] == ".xml" || typeValues[q] == ".py" || typeValues[q] == ".css" || typeValues[q] == ".js" || typeValues[q] == ".sql") {
                    List<String> _contValues = new List<String>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    MySqlDataReader _ReadConts = command.ExecuteReader();
                    if(_ReadConts.Read()) {
                        _contValues.Add(_ReadConts.GetString(0));
                    }
                    _ReadConts.Close();

                    if (typeValues[q] == ".py") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                    }
                    else if (typeValues[q] == ".txt") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                    }
                    else if (typeValues[q] == ".html") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                    }
                    else if (typeValues[q] == ".css") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                    }
                    else if (typeValues[q] == ".js") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    }
                    else if (typeValues[q] == ".sql") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (txtFORM displayTxt = new txtFORM(_contValues[0], "cust_sharing",titleLab.Text,label1.Text, UploaderUsername)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayTxt.Owner = bgBlur;
                            displayTxt.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[q] == ".gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    Application.DoEvents();
                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[q]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    textboxPic.Image = new Bitmap(_toMs);
                    textboxPic.Click += (sender_im, e_im) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayGif.Owner = bgBlur;
                            displayGif.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                Application.DoEvents();

                Application.OpenForms
                   .OfType<Form>()
                   .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                   .ToList()
                   .ForEach(form => form.Close());

                label6.Text = "Files Count: " + flowLayoutPanel1.Controls.Count.ToString();

                if (flowLayoutPanel1.Controls.Count > 0) {
                    label5.Visible = false;
                }
            }
        }
    }
}
