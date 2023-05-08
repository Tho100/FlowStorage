using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X509;

namespace FlowSERVER1 {
    public partial class LogIN : Form {
        public static LogIN instance;

        private MySqlConnection con = ConnectionModel.con;

        private MySqlCommand command = ConnectionModel.command;

        private string accType;
        private string decryptMainKey;
        private string encryptionKeyVal;
        private string pinDecryptionKey;
        private string CurrentLang = "";
        private int attemptCurr = 0;
        private string custEmail {get; set; }
        private string custUsername { get; set; }
        private string inputGetEmail {get; set; }
        private Form1 _form {get; set; } =  Form1.instance;

        /// <summary>
        /// Initialize panel data
        /// </summary>

        // Date label
        private const string DateLabelFontName = "Segoe UI Semibold";
        private const float DateLabelFontSize = 10f;
        private readonly Font DateLabelFont = new Font(DateLabelFontName, DateLabelFontSize, FontStyle.Bold);

        // Title label
        private const string TitleLabelFontName = "Segoe UI Semibold";
        private const float TitleLabelFontSize = 12f;
        private readonly Font TitleLabelFont = new Font(TitleLabelFontName, TitleLabelFontSize, FontStyle.Bold);

        // Panel
        private readonly Color BorderColor = ColorTranslator.FromHtml("#212121");
        private readonly Color DarkGrayColor = Color.DarkGray;
        private readonly Color GainsboroColor = Color.Gainsboro;
        private readonly Color TransparentColor = Color.Transparent;
        private readonly Point TitleLabelLoc = new Point(12, 182);
        private readonly Point DateLabelLoc = new Point(12, 208);

        // Garbage button
        private readonly Color BorderColor2 = ColorTranslator.FromHtml("#232323");
        private readonly Color FillColor = ColorTranslator.FromHtml("#4713BF");
        private readonly Image GarbageImage = FlowSERVER1.Properties.Resources.icons8_garbage_66;
        private readonly Point GarbageButtonLoc = new Point(189, 218);
        public LogIN() {
            InitializeComponent();
            instance = this;
        }

        public void setupAutoLogin(String _custUsername) {
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789012345"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789012345"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private string returnAuthValues(string whichColumn) {
            var concludeValue = new List<string>();

            string checkPasswordQuery = $"SELECT {whichColumn} FROM information WHERE CUST_EMAIL = @email";
            using (var command = new MySqlCommand(checkPasswordQuery, con)) {
                command.Parameters.AddWithValue("@email", inputGetEmail);
                using (var readerPass = command.ExecuteReader()) {
                    while (readerPass.Read()) {
                        concludeValue.Add(readerPass.GetString(0));
                    }
                }
            }

            return concludeValue.FirstOrDefault() ?? string.Empty;
        }

        private string computeAuthCase(string inputStr) {

            SHA256 sha256 = SHA256.Create();

            string _getAuthStrCase0 = inputStr;
            byte[] _getAuthBytesCase0 = Encoding.UTF8.GetBytes(_getAuthStrCase0);
            byte[] _authHashCase0 = sha256.ComputeHash(_getAuthBytesCase0);
            string _authStrCase0 = BitConverter.ToString(_authHashCase0).Replace("-", "");

            return _authStrCase0;
        }

        void setupRedundane() {

            var form = Form1.instance;
            var flowlayout = form.flowLayoutPanel1;
            var but6 = form.guna2Button6;
            var lab8 = form.label8;

            const string selectUserQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_EMAIL = @email";
            const string selectEmailQuery = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";

            using (var command = new MySqlCommand(selectUserQuery, con)) {
                command.Parameters.AddWithValue("@email", inputGetEmail);
                var username = command.ExecuteScalar() as string;

                if (username != null) {
                    using (var emailCommand = new MySqlCommand(selectEmailQuery, con)) {
                        emailCommand.Parameters.AddWithValue("@username", username);
                        var email = emailCommand.ExecuteScalar() as string;

                        if (email != null) {
                            custUsername = username;
                            custEmail = email;
                        }
                    }
                }
            }

            Form1.instance.accountTypeString = accType;
            flowlayout.Controls.Clear();
            form.listBox1.Items.Clear();
            form.label5.Text = custUsername;
            form.label24.Text = custEmail;
            but6.Visible = lab8.Visible = label4.Visible = Form1.instance.guna2Panel7.Visible = false;

            setupTime();

            if (flowlayout.Controls.Count == 0) {
                Form1.instance.label8.Visible = true;
                Form1.instance.guna2Button6.Visible = true;
            }
        }

        private async Task _generateUserFolder(String userName) {

            String[] itemFolder = { "Home", "Shared To Me", "Shared Files" };
            _form.listBox1.Items.AddRange(itemFolder);
            _form.listBox1.SelectedIndex = 0;

            List<String> updatesTitle = new List<String>();

            String getTitles = "SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getTitles, ConnectionModel.con)) {
                command.Parameters.AddWithValue("@username", userName);
                using (MySqlDataReader fold_Reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await fold_Reader.ReadAsync()) {
                        updatesTitle.Add(EncryptionModel.Decrypt(fold_Reader.GetString(0)));
                    }
                }
            }

            foreach (String titleEach in updatesTitle) {
                _form.listBox1.Items.Add(titleEach);
            }
        }

        /// <summary>
        /// 
        /// Function to clear Garbage icon and "It's empty here" label
        /// 
        /// </summary>
        private void clearRedundane() {
            _form.guna2Button6.Visible = false;
            _form.label8.Visible = false;
        }

        /// <summary>
        /// 
        /// Load user directories on success login
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passUser"></param>
        /// <param name="rowLength"></param>
        /// <returns></returns>
        private async Task _generateUserDirectory(String userName, int rowLength) {

            List<Tuple<string, string>> filesInfoDirs = new List<Tuple<string, string>>();
            string selectFileData = $"SELECT DIR_NAME, UPLOAD_DATE FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string dirName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        string uploadDate = reader.GetString(1);
                        filesInfoDirs.Add(new Tuple<string, string>(dirName, uploadDate));
                    }
                }
            }

            int top = 275;
            int h_p = 100;

            for (int i = 0; i < rowLength - 1; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = "ABC02" + i,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };
                top += h_p;
                _form.flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = "LabG" + i;
                dateLab.Font = DateLabelFont;
                dateLab.ForeColor = DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = DateLabelLoc;
                dateLab.Text = filesInfoDirs[i].Item2;

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = TitleLabelFont;
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfoDirs[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 6;
                picMain_Q.Width = 241;
                picMain_Q.Height = 165;
                picMain_Q.Visible = true;

                picMain_Q.MouseHover += (_senderM, _ev) => {
                    panelF.ShadowDecoration.Enabled = true;
                    panelF.ShadowDecoration.BorderRadius = 8;
                };

                picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                    panelF.ShadowDecoration.Enabled = false;
                };

                Guna2Button remBut = new Guna2Button();
                panelF.Controls.Add(remBut);
                remBut.Name = "Rem" + i;
                remBut.Width = 39;
                remBut.Height = 35;
                remBut.FillColor = FillColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = BorderColor2;
                remBut.Image = GarbageImage;
                remBut.Visible = true;
                remBut.Location = GarbageButtonLoc;

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", userName);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (_form.flowLayoutPanel1.Controls.Count == 0) {
                            _form.label8.Visible = true;
                            _form.guna2Button6.Visible = true;
                        }
                    }
                };

                picMain_Q.Image = FlowSERVER1.Properties.Resources.DirIcon;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader"); ShowAlert.Show();

                    Form3 displayDirectory = new Form3(titleLab.Text);
                    displayDirectory.Show();

                    Application.OpenForms
                       .OfType<Form>()
                       .Where(getForm => String.Equals(getForm.Name, "RetrievalAlert"))
                       .ToList()
                       .ForEach(getForm => getForm.Close());
                };
            }
        }

        /// <summary>
        /// 
        /// Load user home files on success login
        /// 
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>
        
        int top = 275;
        int h_p = 100;

        private async Task _generateUserFiles(String _tableName, String parameterName, int currItem) {

            List<(string, string)> filesInfo = new List<(string, string)>();
            string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM {_tableName} WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add((fileName, uploadDate));
                    }
                }
            }

            for (int i = 0; i < currItem; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = parameterName + i,
                    Width = 240,
                    Height = 262,
                    BorderColor = BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = TransparentColor,
                    Location = new Point(600, top)
                };

                top += h_p;
                _form.flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q; 

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = "LabG" + i;
                dateLab.Font = DateLabelFont;
                dateLab.ForeColor = DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = DateLabelLoc;
                dateLab.Text = filesInfo[i].Item2;

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = TitleLabelFont;
                titleLab.ForeColor = GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = TitleLabelLoc;
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfo[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 6;
                picMain_Q.Width = 226;
                picMain_Q.Height = 165;
                picMain_Q.Visible = true;

                picMain_Q.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panelF.Width - picMain_Q.Width) / 2;

                picMain_Q.Location = new Point(picMain_Q_x, 10);

                picMain_Q.MouseHover += (_senderM, _ev) => {
                    panelF.ShadowDecoration.Enabled = true;
                    panelF.ShadowDecoration.BorderRadius = 8;
                };

                picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                    panelF.ShadowDecoration.Enabled = false;
                };

                Guna2Button remBut = new Guna2Button();
                panelF.Controls.Add(remBut);
                remBut.Name = "Rem" + i;
                remBut.Width = 39;
                remBut.Height = 35;
                remBut.FillColor = FillColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = BorderColor2;
                remBut.Image = GarbageImage;
                remBut.Visible = true;
                remBut.Location = GarbageButtonLoc;

                remBut.Click += (sender_im, e_im) => {

                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", _form.label5.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (_form.flowLayoutPanel1.Controls.Count == 0) {
                            _form.label8.Visible = true;
                            _form.guna2Button6.Visible = true;
                        }
                    }
                };

                _form.guna2Button6.Visible = false;
                _form.label8.Visible = false;

                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == "file_info") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Encrypt(readBase64.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        await Task.Run(async () => {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                img.Image = await Task.Run(() => new Bitmap(toMs));
                            }
                        });
                    }


                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", Form1.instance.label5.Text);
                        displayPic.Show();

                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_expand") {
                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    if (_extTypes == ".py") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (_extTypes == ".txt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (_extTypes == ".html") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                    }
                    else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                    }
                    else if (_extTypes == ".sql") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (_extTypes == ".csv") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }
                    picMain_Q.Click += (sender_t, e_t) => {
                        txtFORM txtFormShow = new txtFORM("LOLOL", "file_info_expand", titleLab.Text, "null", Form1.instance.label5.Text);
                        txtFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        using (exeFORM displayExe = new exeFORM(titleLab.Text, "file_info_exe", "null", Form1.instance.label5.Text)) {
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
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        await Task.Run(async () => {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                img.Image = await Task.Run(() => new Bitmap(toMs));
                            }
                        });
                    }

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid", "null", Form1.instance.label5.Text);
                        vidFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_excel") {
                    img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text, "file_info_excel", "null", Form1.instance.label5.Text);
                        exlForm.Show();
                    };
                }

                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    picMain_Q.Click += (sender_aud, e_aud) => {
                        Form bgBlur = new Form();
                        using (audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null", Form1.instance.label5.Text)) {
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
                    clearRedundane();
                }

                if (_tableName == "file_info_gif") {

                    String getImgQue = "SELECT CUST_THUMB FROM " + _tableName + " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(getImgQue, con);
                    command.Parameters.AddWithValue("@username", _form.label5.Text);

                    MySqlDataAdapter da_Read = new MySqlDataAdapter(command);
                    DataSet ds_Read = new DataSet();
                    da_Read.Fill(ds_Read);
                    MemoryStream ms = new MemoryStream((byte[])ds_Read.Tables[0].Rows[i]["CUST_THUMB"]);
                    img.Image = new Bitmap(ms);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "file_info_gif", "null", Form1.instance.label5.Text)) {
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

                if (_tableName == "file_info_apk") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        using (apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk", "null")) {
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

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        using (pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf", "null", Form1.instance.label5.Text)) {
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

                            displayPdf.Owner = bgBlur;
                            displayPdf.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_ptx, e_ptx) => {
                        Form bgBlur = new Form();
                        using (msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null", Form1.instance.label5.Text)) {
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

                            displayMsi.Owner = bgBlur;
                            displayMsi.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    picMain_Q.Click += (sender_ptx, e_ptx) => {
                        Form bgBlur = new Form();
                        using (wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word", "null", Form1.instance.label5.Text)) {
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

                            displayMsi.Owner = bgBlur;
                            displayMsi.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }
            }
        }

        /// <summary>
        /// 
        /// Perform authentication and if authentication failed alert the user else
        /// load user files information on login e.g Directory, folders, files
        /// 
        /// </summary>

        private int _countRow(String _tableName) {
            String countRowTableQuery = $"SELECT COUNT(CUST_USERNAME) FROM {_tableName} WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(countRowTableQuery, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                int totalRowInt = Convert.ToInt32(command.ExecuteScalar());
                return totalRowInt;
            }
        }

        private async Task fetchUserData() {

            var form = Form1.instance;
            var flowlayout = form.flowLayoutPanel1;
            var but6 = form.guna2Button6;
            var lab8 = form.label8;
            var _getEmail = guna2TextBox1.Text;
            inputGetEmail = _getEmail;
            var _getPass = guna2TextBox2.Text;
            var _getPin = guna2TextBox4.Text;

            try {

                if(!string.IsNullOrEmpty(returnAuthValues("CUST_PASSWORD"))) {
                    decryptMainKey = returnAuthValues("CUST_PASSWORD");
                    if(!string.IsNullOrEmpty(returnAuthValues("CUST_PIN"))) {
                        pinDecryptionKey = returnAuthValues("CUST_PIN");
                    }
                }

            } catch (Exception) {
                label4.Visible = true;
            }
         
            if (computeAuthCase(_getPass) == decryptMainKey && computeAuthCase(_getPin) == pinDecryptionKey) {

                setupRedundane();

                this.Close();

                Thread _retrievalAlertForm = new Thread(() => new RetrievalAlert("Connecting to your account...","login").ShowDialog());
                _retrievalAlertForm.Start();

                await getCurrentLang();
                setupUILanguage(CurrentLang);
                setupTime();

                var _form = Form1.instance;

                _form.guna2TextBox1.Text = String.Empty;
                _form.guna2TextBox2.Text = String.Empty;
                _form.guna2TextBox3.Text = String.Empty;

                try {

                    Dictionary<string, string> tableToFileType = new Dictionary<string, string>
                        {
                        {"file_info", "imgFile"},
                        {"file_info_expand", "txtFile"},
                        {"file_info_exe", "exeFile"},
                        {"file_info_vid", "vidFile"},
                        {"file_info_excel", "exlFile"},
                        {"file_info_audi", "audiFile"},
                        {"file_info_gif", "gifFile"},
                        {"file_info_apk", "apkFile"},
                        {"file_info_pdf", "pdfFile"},
                        {"file_info_ptx", "ptxFile"},
                        {"file_info_msi", "msiFile"},
                        {"file_info_word", "docFile"},
                        {"file_info_directory", "dirFile"}
                    };

                    foreach (string tableName in tableToFileType.Keys) {
                        if (_countRow(tableName) > 0) {
                            if (tableToFileType[tableName] == "dirFile") {
                                await _generateUserDirectory(tableName, _countRow(tableName));
                            }
                            else {
                                await _generateUserFiles(tableName, tableToFileType[tableName], _countRow(tableName));
                            }
                        }
                    }

                    await _generateUserFolder(custUsername);

                    RetrievalAlert retrievalAlertForm = Application.OpenForms.OfType<RetrievalAlert>().FirstOrDefault();
                    retrievalAlertForm?.Close();

                    Form1.instance.label4.Text = Form1.instance.flowLayoutPanel1.Controls.Count.ToString();
                    Form1.instance._TypeValues.Clear();
                    Form1.instance._TypeValuesOthers.Clear();

                    if (guna2CheckBox2.Checked) {
                        setupAutoLogin(Form1.instance.label5.Text);
                    }

                } catch (Exception) {
                    MessageBox.Show("An error occurred. Please hit the refresh button to resolve this.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

            } else {

                label4.Visible = true;

                if(attemptCurr == 5) {
                    this.Close();
                }
            }
        }


        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        /// <summary>
        /// 
        /// Perform authentication and fetch user data on login
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                attemptCurr++;

                await fetchUserData();

                int calculatePercentageUsage = 0;
                if (int.TryParse(Form1.instance.label4.Text, out int getCurrentCount) && int.TryParse(await getAccountTypeNumber(), out int getLimitedValue)) {
                    if (getLimitedValue == 0) {
                        Form1.instance.label20.Text = "0%";
                    }
                    else {
                        calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                        Form1.instance.label20.Text = calculatePercentageUsage.ToString() + "%";
                    }
                    Form1.instance.label6.Text = await getAccountTypeNumber();
                    Form1.instance.guna2ProgressBar1.Value = calculatePercentageUsage;
                }


            }
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async Task<string> getAccountTypeNumber() {
            string accountType = "";
            string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            accType = accountType;

            if (accountType == "Basic") {
                return "20";
            }
            else if (accountType == "Max") {
                return "500";
            }
            else if (accountType == "Express") {
                return "1000";
            }
            else if (accountType == "Supreme") {
                return "2000";
            }

            return "0";
        }

        /// <summary>
        /// Setup UI languages for labels
        /// </summary>
        /// <param name="_custLang"></param>
        private void setupUILanguage(String _custLang) {
            var Form_1 = Form1.instance;
            if (_custLang == "MY") {

                Form_1.label2.Text = "Kiraan Item";
                Form_1.label10.Text = "Muat-Naik";
                Form_1.guna2Button2.Text = "Muat-Naik Fail";
                Form_1.guna2Button12.Text = "Muat-Naik Folder";
                Form_1.guna2Button1.Text = "Buat Direktori";
                Form_1.guna2Button7.Text = "Perkongsian Fail";
                Form_1.guna2Button7.Size = new Size(159, 47);
                Form_1.label28.Text = "Kepentingan";
            }

            if (_custLang == "US") {

                Form_1.label2.Text = "Item Count";
                Form_1.label10.Text = "Upload";
                Form_1.guna2Button2.Text = "Upload File";
                Form_1.guna2Button12.Text = "Upload Folder";
                Form_1.guna2Button1.Text = "Create Directory";
                Form_1.guna2Button7.Text = "File Sharing";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentials";
            }

            if (_custLang == "GER") {
                Form_1.label10.Text = "Hochladen";
                Form_1.label2.Text = "Stückzahl";
                Form_1.guna2Button2.Text = "Datei hochladen";
                Form_1.guna2Button12.Text = "Ordner hochladen";
                Form_1.guna2Button1.Text = "Verzeichnis erstellen";
                Form_1.guna2Button7.Text = "Datenaustausch";
                Form_1.guna2Button7.Size = new Size(159, 47);
                Form_1.label28.Text = "Essentials";
            }

            if (_custLang == "JAP") {
                Form_1.label10.Text = "アップロード";
                Form_1.label2.Text = "アイテム数";
                Form_1.guna2Button2.Text = "ファイルをアップロードする";
                Form_1.guna2Button12.Text = "フォルダのアップロード";
                Form_1.guna2Button1.Text = "ディレクトリの作成";
                Form_1.guna2Button7.Text = "ファイル共有";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "必需品";
            }

            if (_custLang == "ESP") {
                Form_1.label10.Text = "Subir";
                Form_1.label2.Text = "Recuento de elementos";
                Form_1.guna2Button2.Text = "Subir archivo";
                Form_1.guna2Button12.Text = "Cargar carpeta";
                Form_1.guna2Button1.Text = "Crear directorio";
                Form_1.guna2Button7.Text = "Compartición de archivos";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Esenciales";
            }

            if (_custLang == "FRE") {
                Form_1.label10.Text = "Télécharger";
                Form_1.label2.Text = "Nombre d'éléments";
                Form_1.guna2Button2.Text = "Téléverser un fichier";
                Form_1.guna2Button12.Text = "Télécharger le dossier";
                Form_1.guna2Button1.Text = "Créer le répertoire";
                Form_1.guna2Button7.Text = "Partage de fichiers";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentiel";
                Form_1.guna2Button5.Text = "Paramètres";
            }

            if (_custLang == "POR") {
                Form_1.label10.Text = "Carregar";
                Form_1.label2.Text = "Contagem de itens";
                Form_1.guna2Button2.Text = "Subir arquivo";
                Form_1.guna2Button12.Text = "Carregar Pasta";
                Form_1.guna2Button1.Text = "Criar diretório";
                Form_1.guna2Button7.Text = "Compartilhamento de arquivos";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essenciais";
            }

            if (_custLang == "CHI") {
                Form_1.label10.Text = "上传";
                Form_1.label2.Text = "物品数量";
                Form_1.guna2Button2.Text = "上传文件";
                Form_1.guna2Button12.Text = "上传文件夹";
                Form_1.guna2Button1.Text = "创建目录";
                Form_1.guna2Button7.Text = "文件共享";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "要点";
            }

            if(_custLang == "RUS") {
                Form_1.label10.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.guna2Button2.Text = "Загрузить файл";
                Form_1.guna2Button12.Text = "Загрузить папку";
                Form_1.guna2Button1.Text = "Создать каталог";
                Form_1.guna2Button7.Text = "Общий доступ к файлам";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Основные";
            }

            if(_custLang == "DUT") {
                Form_1.label10.Text = "Uploaden";
                Form_1.label2.Text = "Aantal artikelen";
                Form_1.guna2Button2.Text = "Bestand uploaden";
                Form_1.guna2Button12.Text = "Map uploaden";
                Form_1.guna2Button1.Text = "Directory aanmaken";
                Form_1.guna2Button7.Text = "Bestanden delen";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentials";
            }
        }

        /// <summary>
        /// Retrieve user current language
        /// </summary>
        private async Task getCurrentLang() {

            String _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                using (MySqlDataReader readLang = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await readLang.ReadAsync()) {
                        CurrentLang = readLang.GetString(0);
                    }
                }
            }

        }

        public void setupTime() {

            Form1 form = Form1.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;

            DateTime now = DateTime.Now;
            int hours = now.Hour;
            string greeting = "";

            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + lab5.Text;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + lab5.Text;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + lab5.Text ;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おはよう " + lab5.Text + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buen día " + lab5.Text + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bonjour " + lab5.Text + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Bom dia " + lab5.Text + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "早上好 " + lab5.Text + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Доброе утро " + lab5.Text + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemorgen " + lab5.Text + " :)";
                }
            }

            else if (hours >= 12 && hours <= 16) {
                if (CurrentLang == "US") {
                    greeting = "Good Afternoon, " + lab5.Text;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Petang, " + lab5.Text;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Tag, " + lab5.Text;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "こんにちは, " + lab5.Text;
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas tardes " + lab5.Text + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bon après-midi " + lab5.Text + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa tarde " + lab5.Text + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "下午好 " + lab5.Text + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Добрый день " + lab5.Text + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemiddag " + lab5.Text + " :)";
                }

            }
             else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (CurrentLang == "US") {
                        greeting = "Good Late Evening, " + lab5.Text;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + lab5.Text;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten späten Abend, " + lab5.Text;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "buenas tardes " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый день " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + lab5.Text + " :)";
                    }

                }
                else {
                    if (CurrentLang == "US") {
                        greeting = "Good Evening, " + lab5.Text;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Petang, " + lab5.Text;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten Abend, " + lab5.Text;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "Buenas terdes " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый вечер " + lab5.Text + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + lab5.Text + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (CurrentLang == "US") {
                    greeting = "Good Night, " + lab5.Text;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Malam, " + lab5.Text;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Nacth, " + lab5.Text;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おやすみ " + lab5.Text + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas noches " + lab5.Text + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "bonne nuit " + lab5.Text + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa noite " + lab5.Text + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "晚安 " + lab5.Text + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Спокойной ночи " + lab5.Text + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Welterusten " + lab5.Text + " :)";
                }

            }
            lab1.Text = greeting;
        }

        private void LogIN_Load(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e) {

        }

        private void guna2Button4_Click_1(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox4.Text, "[^0-9]")) {
                guna2TextBox4.Text = guna2TextBox4.Text.Remove(guna2TextBox4.Text.Length - 1);
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label3_Click_1(object sender, EventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            emailValidate _showPasswordRecovery = new emailValidate();
            _showPasswordRecovery.Show();
            
            this.Close();

        }

        private void trackBar1_Scroll(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
