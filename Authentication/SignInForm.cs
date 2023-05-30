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
using FlowSERVER1.Authentication;

namespace FlowSERVER1 {
    public partial class SignInForm : Form {

        public static SignInForm instance;

        private readonly SignUpForm mainForm;

        private readonly HomePage accessHomePage = HomePage.instance;

        private readonly MySqlConnection con = ConnectionModel.con;

        private MySqlCommand command = ConnectionModel.command;

        private Crud crudClass = new Crud();


        private string returnedAuth0 {get; set; }
        private string returnedAuth1 {get; set; }
        private string CurrentLang {get; set; } = "";
        private int attemptCurr {get; set; } = 0;
        private string custEmail {get; set; }
        private string custUsername { get; set; }
        private string inputGetEmail {get; set; }
        private HomePage _form {get; set; } =  HomePage.instance;

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
        public SignInForm(SignUpForm mainForm) {
            InitializeComponent();

            this.mainForm = mainForm;
                
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

        private async void setupRedundane() {

            var flowLayout = accessHomePage.flowLayoutPanel1;
            var garbageButton = accessHomePage.guna2Button6;
            var itsEmptyHereLabel = accessHomePage.label8;

            const string selectUserQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_EMAIL = @email";
            const string selectEmailQuery = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";

            using (var command = new MySqlCommand(selectUserQuery, con)) {
                command.Parameters.AddWithValue("@email", inputGetEmail);
                var username = await command.ExecuteScalarAsync() as string;

                if (username != null) {
                    using (var emailCommand = new MySqlCommand(selectEmailQuery, con)) {
                        emailCommand.Parameters.AddWithValue("@username", username);
                        var email = await emailCommand.ExecuteScalarAsync() as string;

                        if (email != null) {
                            custUsername = username;
                            custEmail = email;
                        }
                    }
                }
            }

            flowLayout.Controls.Clear();
            accessHomePage.listBox1.Items.Clear();
            Globals.custUsername = custUsername;
            Globals.custEmail = custEmail;

            garbageButton.Visible = itsEmptyHereLabel.Visible = label4.Visible = false;

            buildGreetingLabel();

            if(flowLayout.Controls.Count == 0) {
                buildEmptyBody();
            }

        }

        private void buildEmptyBody() {
            accessHomePage.label8.Visible = true;
            accessHomePage.guna2Button6.Visible = true;
        }

        private async Task _generateUserFolder(String userName) {

            string[] itemFolder = { "Home", "Shared To Me", "Shared Files" };
            _form.listBox1.Items.AddRange(itemFolder);
            _form.listBox1.SelectedIndex = 0;

            List<string> updatesTitle = new List<string>();

            const string getTitles = "SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getTitles, ConnectionModel.con)) {
                command.Parameters.AddWithValue("@username", userName);
                using (MySqlDataReader fold_Reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await fold_Reader.ReadAsync()) {
                        updatesTitle.Add(EncryptionModel.Decrypt(fold_Reader.GetString(0)));
                    }
                }
            }

            foreach (string titleEach in updatesTitle) {
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
        private async Task _generateUserDirectory(int rowLength) {

            List<Tuple<string, string>> filesInfoDirs = new List<Tuple<string, string>>();
            const string selectFileData = "SELECT DIR_NAME, UPLOAD_DATE FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

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

                        const string noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        const string removeQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
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

                    DirectoryForm displayDirectory = new DirectoryForm(titleLab.Text);
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);

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

                        const string noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        string removeQuery = $"DELETE FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
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
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Encrypt(readBase64.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            Image setImageStream = Image.FromStream(toMs);
                            img.Image = setImageStream;
                        }
                       
                    }


                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", Globals.custUsername);
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
                        txtFORM txtFormShow = new txtFORM("LOLOL", "file_info_expand", titleLab.Text, "null", Globals.custUsername);
                        txtFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        using (exeFORM displayExe = new exeFORM(titleLab.Text, "file_info_exe", "null", Globals.custUsername)) {
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
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            Image setImageStream = Image.FromStream(toMs);
                            img.Image = setImageStream;
                        }
                    }

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid", "null", Globals.custUsername);
                        vidFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_excel") {
                    img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text, "file_info_excel", "null", Globals.custUsername);
                        exlForm.Show();
                    };
                }

                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    picMain_Q.Click += (sender_aud, e_aud) => {
                        Form bgBlur = new Form();
                        using (audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null", Globals.custUsername)) {
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
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    MySqlDataAdapter da_Read = new MySqlDataAdapter(command);
                    DataSet ds_Read = new DataSet();
                    da_Read.Fill(ds_Read);
                    MemoryStream ms = new MemoryStream((byte[])ds_Read.Tables[0].Rows[i]["CUST_THUMB"]);
                    img.Image = new Bitmap(ms);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "file_info_gif", "null", Globals.custUsername)) {
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
                        using (apkFORM displayPic = new apkFORM(titleLab.Text, Globals.custUsername, "file_info_apk", "null")) {
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
                        using (pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf", "null", Globals.custUsername)) {
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
                        using (msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null", Globals.custUsername)) {
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
                        using (wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word", "null", Globals.custUsername)) {
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
        /// Verify user input authentication and process
        /// sign in verification and load user data
        /// 
        /// </summary>
        /// <returns>verifyStatus</returns>
        private async Task<bool> verifyAuthentication() {

            var _getEmail = guna2TextBox1.Text;
            inputGetEmail = _getEmail;

            var inputAuth0 = guna2TextBox2.Text;
            var inputAuth1 = guna2TextBox4.Text;

            bool authenticationSuccessful = false;

            try {

                if (!string.IsNullOrEmpty(returnAuthValues("CUST_PASSWORD"))) {
                    returnedAuth0 = returnAuthValues("CUST_PASSWORD");
                    if (!string.IsNullOrEmpty(returnAuthValues("CUST_PIN"))) {
                        returnedAuth1 = returnAuthValues("CUST_PIN");
                    }
                }

            }
            catch (Exception) {
                label4.Visible = true;
            }

            if (EncryptionModel.computeAuthCase(inputAuth0) == returnedAuth0 &&
                EncryptionModel.computeAuthCase(inputAuth1) == returnedAuth1) {

                authenticationSuccessful = true;

                setupRedundane();
                this.Close();

                Thread _retrievalAlertForm = new Thread(() => new RetrievalAlert("Connecting to your account...", "login").ShowDialog());
                _retrievalAlertForm.Start();

                await getCurrentLang();
                setupUILanguage(CurrentLang);
                buildGreetingLabel();

                try {

                    await generateUserData();

                    RetrievalAlert retrievalAlertForm = Application.OpenForms.OfType<RetrievalAlert>().FirstOrDefault();
                    retrievalAlertForm?.Close();

                    HomePage.instance.label4.Text = HomePage.instance.flowLayoutPanel1.Controls.Count.ToString();
                    HomePage.instance._TypeValues.Clear();
                    HomePage.instance._TypeValuesOthers.Clear();

                    if (guna2CheckBox2.Checked) {
                        setupAutoLogin(Globals.custUsername);
                    }

                }
                catch (Exception) {
                    MessageBox.Show(
                        "An error occurred. Check your internet connection and try again.",
                        "Flowstorage",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            else {
                closeFormOnLimit();
            }

            return authenticationSuccessful;
        }

        /// <summary>
        /// 
        /// Login failed for 5 attempts then close the form.
        /// 
        /// </summary>
        private void closeFormOnLimit() {
            label4.Visible = true;
            if (attemptCurr == 5) {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// Generate user data into HomePage 
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task generateUserData() {

            await _generateUserFolder(custUsername);
        }

        private void HomePage_HomePageClosed(object sender, FormClosedEventArgs e) {
            if (Application.OpenForms.Count >= 1) {
                Application.Exit(); 
            }
        }

        private void showHomePage() {
            HomePage.instance.FormClosed += HomePage_HomePageClosed;
            HomePage.instance.Show();
            mainForm.Hide();
            Close();
        }

        private void label4_Click(object sender, EventArgs e) {

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

                var verifyAuthenticaton = await verifyAuthentication();
                if(verifyAuthenticaton) {

                    int calculatePercentageUsage = 0;
                    if (int.TryParse(HomePage.instance.label4.Text, out int getCurrentCount) && int.TryParse(await getAccountTypeNumber(), out int getLimitedValue)) {
                        if (getLimitedValue == 0) {
                            HomePage.instance.label20.Text = "0%";
                        }
                        else {
                            calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                            HomePage.instance.label20.Text = calculatePercentageUsage.ToString() + "%";
                        }
                        HomePage.instance.label6.Text = await getAccountTypeNumber();
                        HomePage.instance.guna2ProgressBar1.Value = calculatePercentageUsage;
                    }

                    showHomePage();

                }

            }
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async Task<string> getAccountTypeNumber() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            HomePage.instance.accountTypeString = accountType;

            if (accountType == "Basic") {
                return "30";
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
            var Form_1 = HomePage.instance;
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

            const string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader readLang = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await readLang.ReadAsync()) {
                        CurrentLang = readLang.GetString(0);
                    }
                }
            }

        }

        private void buildGreetingLabel() {

            HomePage form = HomePage.instance;
            var lab1 = form.label1;

            DateTime now = DateTime.Now;
            int hours = now.Hour;
            string greeting = "";

            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + Globals.custUsername ;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おはよう " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buen día " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bonjour " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Bom dia " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "早上好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Доброе утро " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemorgen " + Globals.custUsername + " :)";
                }
            }

            else if (hours >= 12 && hours <= 16) {
                if (CurrentLang == "US") {
                    greeting = "Good Afternoon, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Petang, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Tag, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "こんにちは, " + Globals.custUsername;
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas tardes " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "Bon après-midi " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa tarde " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "下午好 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Добрый день " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Goedemiddag " + Globals.custUsername + " :)";
                }

            }
             else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (CurrentLang == "US") {
                        greeting = "Good Late Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten späten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "buenas tardes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый день " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }

                }
                else {
                    if (CurrentLang == "US") {
                        greeting = "Good Evening, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "MY") {
                        greeting = "Selamat Petang, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "GER") {
                        greeting = "Guten Abend, " + Globals.custUsername;
                    }
                    else if (CurrentLang == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "ESP") {
                        greeting = "Buenas terdes " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "RUS") {
                        greeting = "Добрый вечер " + Globals.custUsername + " :)";
                    }
                    else if (CurrentLang == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (CurrentLang == "US") {
                    greeting = "Good Night, " + Globals.custUsername;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selamat Malam, " + Globals.custUsername;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Nacth, " + Globals.custUsername;
                }
                else if (CurrentLang == "JAP") {
                    greeting = "おやすみ " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "ESP") {
                    greeting = "Buenas noches " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "FRE") {
                    greeting = "bonne nuit " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "POR") {
                    greeting = "Boa noite " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "CHI") {
                    greeting = "晚安 " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "RUS") {
                    greeting = "Спокойной ночи " + Globals.custUsername + " :)";
                }
                else if (CurrentLang == "DUT") {
                    greeting = "Welterusten " + Globals.custUsername + " :)";
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

            ValidateRecoveryEmail _showPasswordRecovery = new ValidateRecoveryEmail();
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
