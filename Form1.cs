using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;
using System.Runtime.Caching;
using Xamarin.Forms.Internals;

namespace FlowSERVER1 {

    /// <summary>
    /// Main form class
    /// </summary>
    
    public partial class Form1 : Form {

        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;

        public static Form1 instance { get; set;} = new Form1();
        public Label setupLabel { get; set; }
        public string CurrentLang { get; private set; }
        public object _getValues { get; private set; }
        public string nameTableInsert { get; private set; }
        public BackgroundWorker worker { get; private set; }

        private string get_ex;
        private string getName;
        private string retrieved;
        private string retrievedName;
        private object keyValMain;
        private long fileSizeInMB;
        private string varDate;
        private string tableName;

        public Form1() {

            InitializeComponent();

            instance = this;

            var form4Instances = Application.OpenForms.OfType<Form>().Where(form => form.Name == "Form4").ToList();
            form4Instances.ForEach(form => form.Close());

            this.TopMost = false;

            setupLabel = label5;

            InitializeAsyncLoad();
        }


        /// <summary>
        /// 
        /// Load necessary data on program startup
        /// including files information
        /// 
        /// </summary>
        private async void InitializeAsyncLoad() {

            try {

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                if (!Directory.Exists(path)) {
                    return;
                }

                new DirectoryInfo(path).Attributes &= ~FileAttributes.Hidden;

                string authFile = Path.Combine(path, "CUST_DATAS.txt");
                if (!File.Exists(authFile) || new FileInfo(authFile).Length == 0) {
                    return;
                }

                string username = EncryptionModel.Decrypt(File.ReadLines(authFile).First(), EncryptionKey.KeyValue);

                guna2Panel7.Visible = false;
                label5.Text = username;

                using (var command = new MySqlCommand("SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username", con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        if (await reader.ReadAsync()) {
                            label24.Text = reader.GetString(0);
                        }
                    }
                }

                var itemsFolder = new[] { "Home", "Shared To Me", "Shared Files" };
                var updatesTitle = new List<string>();

                using (var command = new MySqlCommand("SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username", ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            updatesTitle.Add(reader.GetString(0));
                        }
                    }
                }

                listBox1.Items.AddRange(itemsFolder.Concat(updatesTitle).ToArray());
                listBox1.SelectedIndex = 0;
                label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                setupTime();
                await getAccountTypeNumber();
                await getCurrentLang();

                if (int.TryParse(label4.Text, out int getCurrentCount) && int.TryParse(label6.Text, out int getLimitedValue)) {
                    int calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                    label20.Text = calculatePercentageUsage.ToString() + "%";
                    guna2ProgressBar1.Value = calculatePercentageUsage;
                }


                setupUILanguage(CurrentLang);
            }
            catch (Exception FlowstorageDirNotFound) {
                // TODO: Ignore
            } finally {

                try {

                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                    new DirectoryInfo(path).Attributes |= (FileAttributes.Directory | FileAttributes.Hidden);
                    Application.OpenForms.OfType<Form>().Where(form => form.Name == "LoadAlertFORM").ToList().ForEach(form => form.Close());

                } catch (Exception FlowStorageDirNotFound) {
                    // TODO: Ignore
                }

            }
        }

        /// <summary>
        /// Start genertating UI labels based on user language
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

            if(_custLang == "GER") {
                Form_1.label10.Text = "Hochladen";
                Form_1.label2.Text = "Stückzahl";
                Form_1.guna2Button2.Text = "Datei hochladen";
                Form_1.guna2Button12.Text = "Ordner hochladen";
                Form_1.guna2Button1.Text = "Verzeichnis erstellen";
                Form_1.guna2Button7.Text = "Datenaustausch";
                Form_1.guna2Button7.Size = new Size(159, 47);
                Form_1.label28.Text = "Essentials";
            }

            if(_custLang == "JAP") {
                Form_1.label10.Text = "アップロード";
                Form_1.label2.Text = "アイテム数";
                Form_1.guna2Button2.Text = "ファイルをアップロードする";
                Form_1.guna2Button12.Text = "フォルダのアップロード";
                Form_1.guna2Button1.Text = "ディレクトリの作成";
                Form_1.guna2Button7.Text = "ファイル共有";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "必需品";
            }

            if(_custLang == "ESP") {
                Form_1.label10.Text = "Subir";
                Form_1.label2.Text = "Recuento de elementos";
                Form_1.guna2Button2.Text = "Subir archivo";
                Form_1.guna2Button12.Text = "Cargar carpeta";
                Form_1.guna2Button1.Text = "Crear directorio";
                Form_1.guna2Button7.Text = "Compartición de archivos";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Esenciales";
            }

            if(_custLang == "FRE") {
                Form_1.label10.Text = "Télécharger";
                Form_1.label2.Text = "Nombre d'éléments";
                Form_1.guna2Button2.Text = "Téléverser un fichier";
                Form_1.guna2Button12.Text = "Télécharger le dossier";
                Form_1.guna2Button1.Text = "Créer le répertoire";
                Form_1.guna2Button7.Text = "Partage de fichiers";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentiel";
            }

            if(_custLang == "POR") {
                Form_1.label10.Text = "Carregar";
                Form_1.label2.Text = "Contagem de itens";
                Form_1.guna2Button2.Text = "Subir arquivo";
                Form_1.guna2Button12.Text = "Carregar Pasta";
                Form_1.guna2Button1.Text = "Criar diretório";
                Form_1.guna2Button7.Text = "Compartilhamento de arquivos";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essenciais";
            }

            if(_custLang == "CHI") {
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
        /// Get user current language
        /// </summary>
        private async Task getCurrentLang() {
            String _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using(var command = new MySqlCommand(_selectLang,con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                using(MySqlDataReader readLang = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if(await readLang.ReadAsync()) {
                        CurrentLang = readLang.GetString(0);
                    }
                }   
            }
        }

        /// <summary>
        /// Generate user files on startup
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>
        /// 

        public async Task _generateUserFiles(String _tableName, String parameterName, int currItem) {

            List<Tuple<string, string>> filesInfo = new List<Tuple<string, string>>();
            string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM {_tableName} WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        string uploadDate = reader.GetString(1);
                        filesInfo.Add(new Tuple<string, string>(fileName, uploadDate));
                    }
                }
            }

            flowLayoutPanel1.Location = new Point(13, 10);
            flowLayoutPanel1.Size = new Size(1118, 579);

            int top = 275;
            int h_p = 100;

            for (int i = 0; i < currItem; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = $"{parameterName + i}",
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = $"LabG{i}";
                dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                dateLab.ForeColor = Color.DarkGray;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = new Point(12, 208);
                dateLab.Text = filesInfo[i].Item2;

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}"; 
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfo[i].Item1;

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
                remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remBut.Visible = true;
                remBut.Location = new Point(189, 218);

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        string removeQuery = $"DELETE FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                        using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));
                            command.ExecuteNonQuery();
                        }

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true; 
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                        int getCurrentCount = int.Parse(label4.Text);
                        int getLimitedValue = int.Parse(label6.Text);
                        int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
                        label20.Text = calculatePercentageUsage.ToString() + "%";

                        guna2ProgressBar1.Value = calculatePercentageUsage;

                    }
                };

                guna2Button6.Visible = false;
                label8.Visible = false;
                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                if (_tableName == "file_info") {
                    /*Dictionary<int, string> base64Encoded = new Dictionary<int, string>();

                    string cacheKeySet = $"user_{label5.Text}_images";
                    MemoryCache cache = MemoryCache.Default;

                    if (cache.Contains(cacheKeySet)) {
                        base64Encoded = (Dictionary<int, string>)cache.Get(cacheKeySet);
                    }
                    else {
                        string retrieveImgQuery = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.Add("@username", MySqlDbType.Text).Value = label5.Text;
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                int index = 0;
                                while (await readBase64.ReadAsync()) {
                                    base64Encoded[index++] = EncryptionModel.Decrypt(readBase64.GetString(0), "0123456789085746");
                                }
                            }
                        }
                        CacheItemPolicy cachePolicy = new CacheItemPolicy {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                        };
                        cache.Add(cacheKeySet, base64Encoded, cachePolicy);
                    }

                    if (base64Encoded.ContainsKey(i)) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = Image.FromStream(toMs);
                        }
                    }*/

                   List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = $"SELECT CUST_FILE FROM {_tableName} WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = label5.Text;
                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Decrypt(readBase64.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }
              
                    if (base64Encoded.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = Image.FromStream(toMs);
                        }
                    }

                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"file_info","null",label5.Text)) {
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

                if (_tableName == "file_info_expand") {
                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    if (_extTypes == ".py") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (_extTypes == ".txt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                    }
                    else if (_extTypes == ".html") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                    }
                    else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                    }
                    else if (_extTypes == ".js") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    } else if (_extTypes == ".sql") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (_extTypes == ".csv") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    picMain_Q.Click += (sender_t, e_t) => {

                        if (_extTypes == ".csv" || _extTypes == ".sql") {
                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                            _showRetrievalCsvAlert.Start();
                        }

                        txtFORM displayPic = new txtFORM("IGNORETHIS", "file_info_expand", titleLab.Text,"null",label5.Text);
                         displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    picMain_Q.Click += (sender_ex, e_ex) => {
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",label5.Text);
                        displayExe.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_vid") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username";
                    using (var command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);

                        using (var readBase64 = await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > i && !string.IsNullOrEmpty(base64Encoded[i])) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (var toMs = new MemoryStream(getBytes)) {
                            img.Image = Image.FromStream(toMs);
                        }
                    }

                    picMain_Q.Click += (sender_vq, e_vq) => {
                        var getImgName = (Guna2PictureBox)sender_vq;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
                        vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid","null",label5.Text);
                        vidFormShow.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_excel") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    picMain_Q.Click += (sender_vq, e_vq) => {
                        exlFORM exlForm = new exlFORM(titleLab.Text,"file_info_excel","null",label5.Text);
                        exlForm.Show();
                    };
                }
                if (_tableName == "file_info_audi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    picMain_Q.Click += (sender_Aud, e_Aud) => {
                        Form bgBlur = new Form();
                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi","null",label5.Text);
                        displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.WaitOnLoad = false;
                    img.Image = new Bitmap(_toMs);

                    picMain_Q.Click += (sender_gi, ex_gi) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",label5.Text)) {
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

                if (_tableName == "file_info_apk") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    picMain_Q.Click += (sender_ap, ex_ap) => {
                        Form bgBlur = new Form();
                        apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk","null");
                        displayPic.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_pdf") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    picMain_Q.Click += (sender_pd, e_pd) => {
                        Form bgBlur = new Form();
                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",label5.Text);
                        displayPdf.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_ptx") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",label5.Text);
                        displayPtx.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text,"file_info_msi","null",label5.Text);
                        displayMsi.Show();
                    };
                    clearRedundane();
                }

                if (_tableName == "file_info_word") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        Form bgBlur = new Form();
                        wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word","null",label5.Text);
                        displayMsi.Show();
                    };
                    clearRedundane();
                }
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        /// <summary>
        /// 
        /// Generate user folders files on folder selection 
        /// 
        /// </summary>
        /// <param name="_fileType">File type of the files</param>
        /// <param name="_foldTitle">Folder title</param>
        /// <param name="parameterName">Custom parameter name for panel</param>
        /// <param name="currItem"></param>
        private async Task _generateUserFold(List<String> _fileType, String _foldTitle, String parameterName, int currItem) {

            int top = 275;
            int h_p = 100;

            flowLayoutPanel1.Location = new Point(13, 10);
            flowLayoutPanel1.Size = new Size(1118, 579);

            var uploadAlertFormSucceeded = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
            uploadAlertFormSucceeded?.Close();

            var _setupRetrievalAlertThread = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your folder files.", "Loader").ShowDialog());
            _setupRetrievalAlertThread.Start();

            flowLayoutPanel1.Controls.Clear();

            try {

                List<String> typeValues = new List<String>(_fileType);

                List<Tuple<string, string>> filesInfo = new List<Tuple<string, string>>();
                string selectFileData = $"SELECT CUST_FILE_PATH, UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldname";
                using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldname", _foldTitle);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                            string uploadDate = reader.GetString(1);
                            filesInfo.Add(new Tuple<string, string>(fileName, uploadDate));
                        }
                    }
                }

                for (int i = 0; i < currItem; i++) {

                    var panelPic_Q = new Guna2Panel() {
                        Name = $"panelf{i}",
                        Width = 240,
                        Height = 262,
                        BorderRadius = 8,
                        FillColor = ColorTranslator.FromHtml("#121212"),
                        BackColor = Color.Transparent,
                        Location = new Point(600, top)
                    };
                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelPic_Q);

                    var panelF = (Guna2Panel)panelPic_Q;

                    Label dateLab = new Label();
                    panelF.Controls.Add(dateLab);
                    dateLab.Name = $"datef{i}";
                    dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                    dateLab.ForeColor = Color.DarkGray;
                    dateLab.Visible = true;
                    dateLab.Enabled = true;
                    dateLab.Location = new Point(12, 208);
                    dateLab.Text = filesInfo[i].Item2;

                    Label titleLab = new Label();
                    panelF.Controls.Add(titleLab);
                    titleLab.Name = $"titleImgL{i}";
                    titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                    titleLab.ForeColor = Color.Gainsboro;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = new Point(12, 182);
                    titleLab.Width = 220;
                    titleLab.Height = 30;
                    titleLab.Text = filesInfo[i].Item1;

                    Guna2PictureBox picMain_Q = new Guna2PictureBox();
                    panelF.Controls.Add(picMain_Q);
                    picMain_Q.Name = "imgf" + i;
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
                    remBut.Name = $"remf{i}";
                    remBut.Width = 39;
                    remBut.Height = 35;
                    remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                    remBut.BorderRadius = 6;
                    remBut.BorderThickness = 1;
                    remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                    remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                    remBut.Visible = true;
                    remBut.Location = new Point(189, 218);

                    remBut.Click += (sender_im, e_im) => {
                        var titleFile = titleLab.Text;
                        DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (verifyDialog == DialogResult.Yes) {

                            using (MySqlCommand command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                                command.ExecuteNonQuery();
                            }

                            using (MySqlCommand command = new MySqlCommand("DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername", con)) {
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));
                                command.Parameters.AddWithValue("@foldername", _foldTitle);
                                command.ExecuteNonQuery();
                            }

                            panelPic_Q.Dispose();

                            if (flowLayoutPanel1.Controls.Count == 0) {
                                label8.Visible = true;
                                guna2Button6.Visible = true;
                            }
                            label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                        }
                    };

                    guna2Button6.Visible = false;
                    label8.Visible = false;

                    var img = ((Guna2PictureBox)panelF.Controls["imgf" + i]);

                    if (typeValues[i] == ".png" || typeValues[i] == ".jpg" || typeValues[i] == ".jpeg") {

                        List<string> base64Encoded = new List<string>();

                        string cacheKeySetFold = $"user_images_folder";
                        MemoryCache cache = MemoryCache.Default;

                        if (cache.Contains(cacheKeySetFold)) {
                            base64Encoded = (List<string>)cache.Get(cacheKeySetFold);
                        }
                        else {

                            string retrieveImgQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle";
                            using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@foldtitle", _foldTitle);
                                using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        base64Encoded.Add(readBase64.GetString(0));
                                    }
                                }
                            }

                            CacheItemPolicy cachePolicy = new CacheItemPolicy {
                                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) 
                            };
                            cache.Add(cacheKeySetFold, base64Encoded, cachePolicy);
                        }

                        if (base64Encoded.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                img.Image = new Bitmap(toMs);
                            }
                        }

                        picMain_Q.Click += (sender, e) => {
                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text,"folder_upload_info","null",label5.Text)) {
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


                    if (typeValues[i] == ".txt" || typeValues[i] == ".py" || typeValues[i] == ".html" || typeValues[i] == ".css" || typeValues[i] == ".js" || typeValues[i] == ".sql" || typeValues[i] == ".csv") {

                        var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                        if (typeValues[i] == ".py") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                        }
                        else if (typeValues[i] == ".txt") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                        }
                        else if (_extTypes == ".html") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                        }
                        else if (_extTypes == ".css") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                        }
                        else if (_extTypes == ".js") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (_extTypes == ".csv") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        picMain_Q.Click += (sender_t, e_t) => {

                            if (_extTypes == ".csv" || _extTypes == ".sql") {
                                Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                                _showRetrievalCsvAlert.Start();
                            }

                            txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text,"null",label5.Text);
                            displayPic.Show();
                        };
                        clearRedundane();
                    }

                    if(typeValues[i] == ".mp4" || typeValues[i] == ".mov" || typeValues[i] == ".webm" || typeValues[i] == ".avi" || typeValues[i] == ".wmv") {

                        List<string> base64Encoded = new List<string>();

                        string cacheKeyThumbnailVid = $"user_thumb_folder";
                        MemoryCache cache = MemoryCache.Default;

                        if (cache.Contains(cacheKeyThumbnailVid)) {
                            base64Encoded = (List<string>)cache.Get(cacheKeyThumbnailVid);
                        } else {

                            string retrieveImgQuery = "SELECT CUST_THUMB FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                            using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                                command.Parameters.AddWithValue("@username", label5.Text);
                                command.Parameters.AddWithValue("@foldername", _foldTitle);
                                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));
                                using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                    while (await readBase64.ReadAsync()) {
                                        base64Encoded.Add(readBase64.GetString(0));
                                    }
                                }
                            }

                            CacheItemPolicy cachePolicy = new CacheItemPolicy {
                                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(12)
                            };
                            cache.Add(cacheKeyThumbnailVid, base64Encoded, cachePolicy);
                        }

                        byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = new Bitmap(toMs);
                        }

                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImg,getWidth,getHeight,titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayVid.Show();
                        };
                    }

                    if (typeValues[i] == ".gif") {
                        List<String> _base64Encoded = new List<string>();
                        String retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";
                        using (MySqlCommand command = new MySqlCommand(retrieveImg, con)) {
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@foldername", _foldTitle);
                            command.Parameters.AddWithValue("@filename", titleLab.Text);

                            using (MySqlDataReader _readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await _readBase64.ReadAsync()) {
                                    _base64Encoded.Add(_readBase64.GetString(0));
                                }
                            }
                        }

                        var _getBytes = Convert.FromBase64String(EncryptionModel.Decrypt(_base64Encoded[0], EncryptionKey.KeyValue));
                        MemoryStream _toMs = new MemoryStream(_getBytes);

                        img.Image = new Bitmap(_toMs);
                        img.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImg = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            using(gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                                bgBlur.FormBorderStyle = FormBorderStyle.None;
                                bgBlur.Opacity = .24d;
                                bgBlur.BackColor = Color.Black;
                                bgBlur.WindowState = FormWindowState.Maximized;
                                bgBlur.Name = "bgBlurForm";
                                bgBlur.TopMost = true;
                                bgBlur.Location = this.Location;
                                bgBlur.StartPosition = FormStartPosition.Manual;
                                bgBlur.ShowInTaskbar = false;
                                bgBlur.Show();

                                displayVid.Owner = bgBlur;
                                displayVid.ShowDialog();

                                bgBlur.Dispose();
                            }
                        };
                    }

                    if (typeValues[i] == ".xlsx" || typeValues[i] == ".xls") {
                        img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                        img.Click += (sender_aud, e_aud) => {
                            Form bgBlur = new Form();
                            exlFORM displayExl = new exlFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                            displayExl.Show();
                        };
                    }

                    if (typeValues[i] == ".wav" || typeValues[i] == ".mp3") {
                        var _getWidth = this.Width;
                        var _getHeight = this.Height;
                        img.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                        img.Click += (sender_aud, e_aud) => {
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".apk") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk",_foldTitle);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".exe") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_exe_96;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        img.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            exeFORM displayPic = new exeFORM(titleLab.Text, "file_info_exe", _foldTitle,"null");
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".pdf") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info",_foldTitle,label5.Text);
                            displayPic.Show();
                        };
                    }

                    if (typeValues[i] == ".docx" || typeValues[i] == ".doc") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".pptx" || typeValues[i] == ".ppt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        img.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayDoc.Show();
                        };
                    }

                    if (typeValues[i] == ".msi") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            msiFORM displayMsi = new msiFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                            displayMsi.Show();
                        };
                    }

                    var loadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "LoadAlertFORM");
                    loadAlertForm?.Close();

                    var retrievalAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "RetrievalAlert");
                    retrievalAlertForm?.Close();

                    var uploadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
                    uploadAlertForm?.Close();
                }

            } catch (Exception) {
                // @ Ignore exception after the user cancelled
                // folder file retrieval
            }
        }

        public void clearRedundane() {
            guna2Button6.Visible = false;
            label8.Visible = false;
        }

        public void showRedundane() {
            guna2Button6.Visible = true;
            label8.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e) {
            setupTime();
        }

        /// <summary>
        /// Setting up time label based on user chosen language
        /// </summary>
        public void setupTime() {

            var form = Form1.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;
            var picturebox2 = form.pictureBox2;
            var picturebox3 = form.pictureBox3;
            var picturebox1 = form.pictureBox1;

            DateTime now = DateTime.Now;
            var hours = now.Hour;
            String greeting = "Good Night " + label5.Text;
            picturebox1.Visible = true;
            if (hours >= 1 && hours <= 12) {
                if (CurrentLang == "US") {
                    greeting = "Good Morning, " + lab5.Text;
                }
                else if (CurrentLang == "MY") {
                    greeting = "Selemat Pagi, " + lab5.Text;
                }
                else if (CurrentLang == "GER") {
                    greeting = "Guten Morgen, " + lab5.Text;
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

                picturebox2.Visible = true;
                picturebox1.Visible = false;
                picturebox3.Visible = false;
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
                    greeting = "こんにちは " + lab5.Text + " :)";
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

                picturebox2.Visible = true;
                picturebox1.Visible = false;
                picturebox3.Visible = false;
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
                        greeting = "Guten späten Abend " + lab5.Text + " :)";
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

                picturebox3.Visible = true;
                picturebox2.Visible = false;
                picturebox1.Visible = false;
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

                picturebox1.Visible = true;
                picturebox2.Visible = false;
                picturebox3.Visible = false;
            }
            lab1.Text = greeting;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            Form4 create_dir = new Form4();
            create_dir.Show();
        }

        private Byte[] convertFileToByte(String _filePath) {

            Byte[] fileData = null;

            using (FileStream fs = File.OpenRead(_filePath)) {
                using (BinaryReader binaryReader = new BinaryReader(fs)) {
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            return fileData;
        }

        /// <summary>
        /// 
        /// This function will opens a file dialog and 
        /// generate panel for selected file
        /// 
        /// </summary>

        // Add File  
        int curr = 0;
        int txtCurr = 0;
        int exeCurr = 0;
        int vidCurr = 0;
        int exlCurr = 0;
        int audCurr = 0;
        int gifCurr = 0;
        int apkCurr = 0;
        int pdfCurr = 0;
        int ptxCurr = 0;
        int msiCurr = 0;
        int docxCurr = 0;

        private int searchCurr = 0;
        private string searchPan = "";
        private Control titlePanelSearch;
        private async void _mainFileGenerator(int AccountType_, String _AccountTypeStr_) {

            void deletionMethod(string fileName, string tableName) {
                string query = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";

                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName, EncryptionKey.KeyValue));
                    command.ExecuteNonQuery();
                }

                if (flowLayoutPanel1.Controls.Count == 0) {
                    label8.Visible = true;
                    guna2Button6.Visible = true;
                }

                int getCurrentCount = int.Parse(label4.Text);
                int getLimitedValue = int.Parse(label6.Text);
                int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) - 5;
                label20.Text = calculatePercentageUsage.ToString() + "%";

                guna2ProgressBar1.Value = calculatePercentageUsage;

            }

            var open = new OpenFileDialog {
                Filter = "All Files|*.*|Images Files|*.jpg;*.jpeg;*.png;.bmp;|Video Files|*.mp4;*.webm;.mov;.wmv|Gif Files|*.gif|Text Files|*.txt;|Excel Files|*.xlsx;*.xls|Powerpoint Files|*.pptx;*.ppt|Word Documents|*.docx|Exe Files|*.exe|Audio Files|*.mp3;*.mpeg;*.wav|Programming/Scripting|*.py;*.cs;*.cpp;*.java;*.php;*.js;|Markup Languages|*.html;*.css;*.xml|Acrobat Files|*.pdf|Comma Separated Values|*.csv",
                Multiselect = true
            };

            varDate = DateTime.Now.ToString("dd/MM/yyyy");

            int curFilesCount = flowLayoutPanel1.Controls.Count;
            if (open.ShowDialog() == DialogResult.OK) { 

                List<string> _filValues = open.FileNames.Select(Path.GetFileName).ToList();

                if (open.FileNames.Length + curFilesCount > AccountType_) {
                    Form bgBlur = new Form();
                    using (upgradeFORM displayUpgrade = new upgradeFORM(_AccountTypeStr_)) {
                        bgBlur.StartPosition = FormStartPosition.Manual;
                        bgBlur.FormBorderStyle = FormBorderStyle.None;
                        bgBlur.Opacity = .24d;
                        bgBlur.BackColor = Color.Black;
                        bgBlur.Name = "bgBlurForm";
                        bgBlur.WindowState = FormWindowState.Maximized;
                        bgBlur.TopMost = true;
                        bgBlur.Location = this.Location;
                        bgBlur.StartPosition = FormStartPosition.Manual;
                        bgBlur.ShowInTaskbar = false;
                        bgBlur.Show();

                        displayUpgrade.Owner = bgBlur;
                        displayUpgrade.ShowDialog();

                        bgBlur.Dispose();
                    };

                    _filValues.Clear();

                } else {

                    foreach (var selectedItems in open.FileNames) {
                        _filValues.Add(Path.GetFileName(selectedItems));

                        void clearRedundane() {
                            label8.Visible = false;
                            guna2Button6.Visible = false;
                        }

                        get_ex = open.FileName;
                        getName = Path.GetFileName(selectedItems);
                        retrieved = Path.GetExtension(selectedItems); 
                        retrievedName = Path.GetFileNameWithoutExtension(open.FileName);
                        fileSizeInMB = 0;

                        async Task containThumbUpload(string nameTable, string getNamePath, object keyValMain) {

                            int getCurrentCount = int.Parse(label4.Text);
                            int getLimitedValue = int.Parse(label6.Text);
                            int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100) + 5;
                            label20.Text = calculatePercentageUsage.ToString() + "%";

                            guna2ProgressBar1.Value = calculatePercentageUsage;

                            try {

                                using (var command = new MySqlCommand()) {

                                    command.Connection = con;
                                    command.CommandText = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE, @CUST_THUMB)";
                                    command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getNamePath, "0123456789085746"));
                                    command.Parameters.AddWithValue("@CUST_USERNAME", label5.Text);
                                    command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                                    command.Parameters.AddWithValue("@CUST_FILE", keyValMain);

                                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                                        var toBitMap = shellFile.Thumbnail.Bitmap;
                                        using (var stream = new MemoryStream()) {
                                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                                            command.Parameters.AddWithValue("@CUST_THUMB", toBase64);
                                        }
                                    }

                                    await command.ExecuteNonQueryAsync();
                                }

                            } catch (Exception) {
                                /* TODO: User has cancelled the insert operation
                                 * then ignore 'object reference was not set to an object'
                                 * exception
                                 */
                            }

                            Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());

                        }

                        async void createPanelMain(String nameTable, String panName, int itemCurr, String keyVal) {
                            searchPan = panName;
                            nameTableInsert = nameTable;

                            if (fileSizeInMB < 1500) {

                                async Task startSending(string setValue) {

                                    int getCurrentCount = int.Parse(label4.Text);
                                    int getLimitedValue = int.Parse(label6.Text);
                                    int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100)+5;
                                    label20.Text = calculatePercentageUsage.ToString() + "%";

                                    guna2ProgressBar1.Value = calculatePercentageUsage;

                                    string insertQuery = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE)";

                                    try {

                                        using (var command = new MySqlCommand(insertQuery, con)) {
                                            command.Parameters.Add("@CUST_FILE_PATH", MySqlDbType.Text).Value = EncryptionModel.Encrypt(getName, EncryptionKey.KeyValue);
                                            command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text).Value = label5.Text;
                                            command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar, 255).Value = varDate;
                                            command.Parameters.Add("@CUST_FILE", MySqlDbType.LongBlob).Value = setValue;

                                            await command.ExecuteNonQueryAsync();
                                        }

                                        Application.OpenForms.OfType<Form>().Where(form => String.Equals(form.Name, "UploadAlrt")).ToList().ForEach(form => form.Close());
                                    }
                                    catch (Exception) {
                                        MessageBox.Show("Failed to upload this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                    }
                                }

                                int top = 275;
                                int h_p = 100;
                                var panelTxt = new Guna2Panel() {
                                    Name = panName + itemCurr,
                                    Width = 240,
                                    Height = 262,
                                    BorderRadius = 8,
                                    FillColor = ColorTranslator.FromHtml("#121212"),
                                    BackColor = Color.Transparent,
                                    Location = new Point(600, top)
                                };

                                top += h_p;
                                flowLayoutPanel1.Controls.Add(panelTxt);
                                var mainPanelTxt = (Guna2Panel)panelTxt;

                                var textboxPic = new Guna2PictureBox();
                                mainPanelTxt.Controls.Add(textboxPic);
                                textboxPic.Name = "TxtBox" + itemCurr;
                                textboxPic.Width = 240;
                                textboxPic.Height = 164;
                                textboxPic.BorderRadius = 8;
                                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                                textboxPic.Enabled = true;
                                textboxPic.Visible = true;

                                Label titleLab = new Label();
                                mainPanelTxt.Controls.Add(titleLab);
                                titleLab.Name = "LabVidUp" + itemCurr;
                                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                                titleLab.ForeColor = Color.Gainsboro;
                                titleLab.Visible = true;
                                titleLab.Enabled = true;
                                titleLab.Location = new Point(12, 182);
                                titleLab.Width = 220;
                                titleLab.Height = 30;
                                titleLab.Text = getName;

                                Guna2Button remButTxt = new Guna2Button();
                                mainPanelTxt.Controls.Add(remButTxt);
                                remButTxt.Name = "RemTxtBut" + itemCurr;
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

                                textboxPic.MouseHover += (_senderM, _ev) => {
                                    panelTxt.ShadowDecoration.Enabled = true;
                                    panelTxt.ShadowDecoration.BorderRadius = 8;
                                };

                                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                                    panelTxt.ShadowDecoration.Enabled = false;
                                };

                                var _setupUploadAlertThread = new Thread(() => new UploadAlrt(getName, label5.Text, "null", panName + itemCurr, "null", _fileSize: fileSizeInMB).ShowDialog());
                                _setupUploadAlertThread.Start();

                                if (nameTable == "file_info") {
                                    await startSending(keyVal);

                                    textboxPic.Image = new Bitmap(selectedItems);
                                    textboxPic.Click += (sender_f, e_f) => {
                                        var getImgName = (Guna2PictureBox)sender_f;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                                        Form bgBlur = new Form();
                                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, getName, "file_info", "null",label5.Text)) {
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

                                if (nameTable == "file_info_expand") {
                                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                                    await startSending(keyVal);
                                    
                                    if (_extTypes == ".py") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                                    }
                                    else if (_extTypes == ".txt") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                                    }
                                    else if (_extTypes == ".html") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                                    }
                                    else if (_extTypes == ".css") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                                    }
                                    else if (_extTypes == ".js") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                                    }
                                    else if (_extTypes == ".sql") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                                    }
                                    else if (_extTypes == ".csv") {
                                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                                    }

                                    var filePath = getName;

                                    textboxPic.Click += (sender_t, e_t) => {

                                        if(_extTypes == ".csv" || _extTypes == ".sql") {
                                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                                            _showRetrievalCsvAlert.Start();
                                        }

                                        txtFORM txtFormShow = new txtFORM("IGNORETHIS", "file_info_expand", filePath,"null",label5.Text);
                                        txtFormShow.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_exe") {

                                    keyValMain = keyVal;
                                    tableName = "file_info_exe";
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        Form bgBlur = new Form();
                                        exeFORM displayExe = new exeFORM(titleLab.Text,"file_info_exe","null",label5.Text);
                                        displayExe.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_vid") {
                                    await containThumbUpload(nameTable, getName, keyVal);
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems);
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        var getImgName = (Guna2PictureBox)sender_ex;
                                        var getWidth = getImgName.Image.Width;
                                        var getHeight = getImgName.Image.Height;
                                        Bitmap defaultImg = new Bitmap(getImgName.Image);

                                        vidFORM vidShow = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "file_info_vid","null",label5.Text);
                                        vidShow.Show();
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_audi") {
                                   await startSending(keyVal);

                                    var _getWidth = this.Width;
                                    var _getHeight = this.Height;
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null",label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_excel") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                                    textboxPic.Click += (sender_ex, e_ex) => {
                                        exlFORM displayPic = new exlFORM(titleLab.Text, "file_info_excel", "null", label5.Text);
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_gif") {
                                    await containThumbUpload(nameTable, getName, keyVal);
                                    ShellFile shellFile = ShellFile.FromFilePath(selectedItems); 
                                    Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                                    textboxPic.Image = toBitMap;

                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif","null",label5.Text)) {
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
                                if (nameTable == "file_info_apk") {
                                    keyValMain = keyVal;
                                    tableName = "file_info_apk";
                                    backgroundWorker1.RunWorkerAsync();

                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                                    textboxPic.Click += (sender_gi, e_gi) => {
                                        Form bgBlur = new Form();
                                        apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk","null");
                                        displayPic.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_pdf") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                                    textboxPic.Click += (sender_pd, e_pd) => {
                                        pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf","null",label5.Text);
                                        displayPdf.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_ptx") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx","null",label5.Text);
                                        displayPtx.ShowDialog();                                       
                                    };
                                    clearRedundane();
                                }
                                if (nameTable == "file_info_msi") {
                                    keyValMain = keyVal;
                                    tableName = "file_info_msi";
                                    backgroundWorker1.RunWorkerAsync();
                           
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        Form bgBlur = new Form();
                                        msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null",label5.Text);
                                        displayMsi.Show();
                                    };
                                    clearRedundane();
                                }

                                if (nameTable == "file_info_word") {
                                    await startSending(keyVal);
                                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                                    textboxPic.Click += (sender_ptx, e_ptx) => {
                                        wordFORM displayWord = new wordFORM(titleLab.Text, "file_info_word","null",label5.Text);
                                        displayWord.ShowDialog();
                                    };
                                    clearRedundane();
                                }

                                remButTxt.Click += (sender_tx, e_tx) => {
                                    var titleFile = titleLab.Text;
                                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                    if (verifyDialog == DialogResult.Yes) {
                                        deletionMethod(titleFile, nameTable);
                                        panelTxt.Dispose();
                                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                                    }

                                    if (flowLayoutPanel1.Controls.Count == 0) {
                                        label8.Visible = true;
                                        guna2Button6.Visible = true;
                                    }
                                };

                                Label dateLabTxt = new Label();
                                mainPanelTxt.Controls.Add(dateLabTxt);
                                dateLabTxt.Name = "LabTxtUp" + itemCurr;
                                dateLabTxt.Font = new Font("Segoe UI Semibold", 10,FontStyle.Bold);
                                dateLabTxt.ForeColor = Color.DarkGray;
                                dateLabTxt.Visible = true;
                                dateLabTxt.Enabled = true;
                                dateLabTxt.Location = new Point(12, 208);
                                dateLabTxt.Width = 1000;
                                dateLabTxt.Text = varDate;

                            } else {
                                MessageBox.Show("File is too large, max file size is 1.5GB.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            }
                        }

                        try {

                            Byte[] _toByte = File.ReadAllBytes(selectedItems);
                            fileSizeInMB = (_toByte.Length/1024)/1024;

                            if (retrieved == ".png" || retrieved == ".jpeg" || retrieved == ".jpg" || retrieved == ".ico" || retrieved == ".bmp" || retrieved == ".svg") {
                                curr++;
                                var getImg = new Bitmap(selectedItems);
                                var imgWidth = getImg.Width;
                                var imgHeight = getImg.Height;

                                if (retrieved != ".ico") {
                                    String _tempToBase64 = Convert.ToBase64String(_toByte);
                                    String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64, EncryptionKey.KeyValue);
                                    createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                }
                                else {
                                    Image retrieveIcon = Image.FromFile(selectedItems);
                                    byte[] dataIco;
                                    using (MemoryStream msIco = new MemoryStream()) {
                                        retrieveIcon.Save(msIco, System.Drawing.Imaging.ImageFormat.Png);
                                        dataIco = msIco.ToArray();
                                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(dataIco), EncryptionKey.KeyValue);
                                        String _encryptedValue = EncryptionModel.Encrypt(_tempToBase64, EncryptionKey.KeyValue);
                                        createPanelMain("file_info", "PanImg", curr, _encryptedValue);
                                    }
                                }
                            }

                            else if (retrieved == ".txt" || retrieved == ".html" || retrieved == ".xml" || retrieved == ".py" || retrieved == ".css" || retrieved == ".js" || retrieved == ".sql" || retrieved == ".csv") {
                                txtCurr++;
                                String nonLine = "";
                                using (StreamReader ReadFileTxt = new StreamReader(selectedItems)) { 
                                    nonLine = ReadFileTxt.ReadToEnd();
                                }
                                byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                                String getEncoded = Convert.ToBase64String(getBytes);
                                String encryptText = EncryptionModel.Encrypt(getEncoded, EncryptionKey.KeyValue);
                                createPanelMain("file_info_expand", "PanTxt", txtCurr, encryptText);
                            }

                            else if (retrieved == ".exe") {
                                exeCurr++;
                                var _toBase64 = Convert.ToBase64String(convertFileToByte(selectedItems));
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_exe", "PanExe", exeCurr, encryptText);

                            }
                            else if (retrieved == ".mp4" || retrieved == ".mov" || retrieved == ".webm" || retrieved == ".avi" || retrieved == ".wmv") {
                                vidCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_vid", "PanVid", vidCurr, encryptText);
                            }

                            else if (retrieved == ".xlsx" || retrieved == ".xls") {
                                exlCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_excel","PanExl",exlCurr,encryptText);
                            }

                            else if (retrieved == ".mp3" || retrieved == ".wav") {
                                audCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_audi", "PanAud", audCurr, encryptText); 
                                
                            }

                            else if (retrieved == ".gif") {
                                gifCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_gif", "PanGif", gifCurr, encryptText);
                            }

                            else if (retrieved == ".apk") {
                                apkCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_apk", "PanApk", apkCurr, encryptText);
                            }

                            else if (retrieved == ".pdf") {
                                pdfCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_pdf", "PanPdf", pdfCurr, encryptText);
                            }

                            else if (retrieved == ".pptx" || retrieved == ".ppt") {
                                ptxCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_ptx", "PanPtx", ptxCurr, encryptText);
                            }
                            else if (retrieved == ".msi") {
                                msiCurr++;
                                var _toBase64 = Convert.ToBase64String(convertFileToByte(selectedItems));
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_msi", "PanMsi", msiCurr, encryptText);
                            }
                            else if (retrieved == ".docx") {
                                docxCurr++;
                                var _toBase64 = Convert.ToBase64String(_toByte);
                                String encryptText = EncryptionModel.Encrypt(_toBase64, EncryptionKey.KeyValue);
                                createPanelMain("file_info_word", "PanDoc", docxCurr, encryptText);
                            }

                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "UploadAlrt"))
                            .ToList()
                            .ForEach(form => form.Close());
                           
                        } catch (Exception) {
                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "UploadAlrt"))
                            .ToList()
                            .ForEach(form => form.Close());
                        }

                        searchCurr = curr;

                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                }
            }
            Application.OpenForms
             .OfType<Form>()
             .Where(form => String.Equals(form.Name, "UploadAlrt"))
             .ToList()
             .ForEach(form => form.Close());
        }
  
        /// <summary>
        /// This function will shows alert form that tells the user to upgrade 
        /// their account when the total amount of files upload is exceeding
        /// the amount of file they can upload 
        /// </summary>
        /// <param name="CurAcc"></param>
        public void DisplayError(String CurAcc) {
            Form bgBlur = new Form();
            using (upgradeFORM displayPic = new upgradeFORM(CurAcc)) {
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
        }

        /// <summary>
        /// Select user account type and show file dialog to upload file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                String _currentFolder = listBox1.GetItemText(listBox1.SelectedItem);

                if(_currentFolder == "Home") {

                    String _accType = "";
                    String getAccTypeQuery = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
                    using (var command = new MySqlCommand(getAccTypeQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        List<String> types = new List<String>();
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                types.Add(reader.GetString(0));
                            }
                        }
                        _accType = types.FirstOrDefault();
                    }

                    int CurrentUploadCount = Convert.ToInt32(label4.Text);

                    if (_accType == "Basic") {
                        if (CurrentUploadCount != 20) {
                            _mainFileGenerator(20, _accType);
                        }
                        else {
                            DisplayError(_accType);
                        }
                    }

                    if (_accType == "Max") {
                        if (CurrentUploadCount != 500) {
                            _mainFileGenerator(500, _accType);
                        }
                        else {
                            DisplayError(_accType);
                        }
                    }

                    if (_accType == "Express") {
                        if (CurrentUploadCount != 1000) {
                            _mainFileGenerator(1000, _accType);
                        }
                        else {
                            DisplayError(_accType);
                        }
                    }

                    if (_accType == "Supreme") {
                        if (CurrentUploadCount != 2000) {
                            _mainFileGenerator(2000, _accType);
                        }
                        else {
                            DisplayError(_accType);
                        }
                    }
                } else {
                    MessageBox.Show("You can only upload a file on Home folder.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
            catch (Exception) {
                Form bgBlur = new Form();
                using (waitFORM displayWait = new waitFORM()) {
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

                    displayWait.Owner = bgBlur;
                    displayWait.ShowDialog();

                    bgBlur.Dispose();
                }

            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            LogIN login_page = new LogIN();
            login_page.Show();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// This button will show settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button5_Click(object sender, EventArgs e) {

            Task.Run(() => new settingsAlert().ShowDialog());

            var remAccShow = new remAccFORM(label5.Text, label24.Text);
            remAccShow.Show();

            var settingsAlertForms = Application.OpenForms
                .OfType<Form>()
                .Where(form => form.Name == "settingsAlert")
                .ToList();

            foreach (var form in settingsAlertForms) {
                form.Close();
            }
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e) {


        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label5_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e) {

        }


        private void guna2Button7_Click(object sender, EventArgs e) {

            sharingFORM _ShowSharing = new sharingFORM();
            _ShowSharing.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void pictureBox3_Click(object sender, EventArgs e) {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e) {

        }

        /// <summary>
        /// Validate user entered email address format
        /// </summary>
        /// <param name="_custEmail"></param>
        /// <returns></returns>
        private static bool validateEmailUser(String _custEmail) {
            string _regPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(_regPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(_custEmail);
        }

        /// <summary>
        /// Generate random string within range
        /// </summary>
        Random _setRandom = new Random();
        private string RandomString(int size, bool lowerCase = true) {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++) {
                var @char = (char)_setRandom.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        private int RandomInt(int size) {
            return _setRandom.Next(0,15);
        }

        /// <summary>
        /// 
        /// Sign up process for user information filling 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button11_Click(object sender, EventArgs e) {

            try {

                Control flowlayout = flowLayoutPanel1;
                String _getUser = guna2TextBox1.Text;
                String _getPass = guna2TextBox2.Text;
                String _getEmail = guna2TextBox3.Text;
                String _getPin = guna2TextBox4.Text;

                List<string> existsInfosMail = new List<string>();
                List<string> existsInfosUser = new List<string>();

                using (MySqlCommand command = con.CreateCommand()) {
                    string verifyQueryUser = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
                    command.CommandText = verifyQueryUser;
                    command.Parameters.AddWithValue("@username", _getUser);

                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            existsInfosUser.Add(reader.GetString(0));
                        }
                    }

                    string verifyQueryMail = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
                    command.CommandText = verifyQueryMail;
                    command.Parameters.AddWithValue("@email", _getEmail);

                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            existsInfosMail.Add(reader.GetString(0));
                        }
                    }

                }

                if (existsInfosUser.Count() >= 1 || existsInfosMail.Count() >= 1) {
                    if(existsInfosUser.Count() >= 1) {
                        label11.Visible = true;
                        label11.Text = "Username is taken.";
                    }
                    if (existsInfosMail.Count() >= 1) {
                        label22.Visible = true;
                        label22.Text = "Email already exists.";
                    }
                }
                else {

                    label22.Visible = false;
                    label12.Visible = false;
                    label11.Visible = false;

                    if (!(_getPass.Contains("!") || _getPass.Contains("!"))) {
                        if(!(_getUser.Contains("&") || _getUser.Contains(";") || _getUser.Contains("?") || _getUser.Contains("%"))) {
                            if(!String.IsNullOrEmpty(_getPin)) {
                            if(_getPin.Length == 3) {
                                if (validateEmailUser(_getEmail) == true) {
                                    if (_getUser.Length <= 20) {
                                        if (_getPass.Length > 5) {
                                            if (!String.IsNullOrEmpty(_getEmail)) {
                                                if (!String.IsNullOrEmpty(_getPass)) {
                                                    if (!String.IsNullOrEmpty(_getUser)) {
                                                        flowlayout.Controls.Clear();
                                                        if (flowlayout.Controls.Count == 0) {
                                                            Form1.instance.label8.Visible = true;
                                                            Form1.instance.guna2Button6.Visible = true;
                                                        }
                                                        if (Form1.instance.setupLabel.Text.Length > 14) {
                                                            var label = Form1.instance.setupLabel;
                                                            label.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                                                            label.Location = new Point(3, 27);
                                                        }

                                                        label5.Text = _getUser;
                                                        label24.Text = _getEmail;

                                                        var _setupTok = RandomInt(10) + RandomString(12) + "&/" + RandomInt(12) + "^*" + _getUser + RandomInt(12) + RandomString(12);
                                                        var _removeSpaces = new string(_setupTok.Where(c => !Char.IsWhiteSpace(c)).ToArray());
                                                        var _encryptTok = EncryptionModel.Encrypt(_removeSpaces, "0123456789085746");
                                                        string _getDate = DateTime.Now.ToString("MM/dd/yyyy");

                                                        using (var transaction = con.BeginTransaction()) {

                                                            try {

                                                                MySqlCommand command = con.CreateCommand();

                                                                command.CommandText = @"INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN,ACCESS_TOK)
                            VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL,@CUST_PIN,@ACCESS_TOK)";
                                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                                command.Parameters.AddWithValue("@CUST_PASSWORD", computeAuthCase(_getPass));
                                                                command.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                                                                command.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                                                command.Parameters.AddWithValue("@CUST_PIN", computeAuthCase(_getPin));
                                                                command.Parameters.AddWithValue("@ACCESS_TOK", _encryptTok);
                                                                command.ExecuteNonQuery();

                                                                command.CommandText = @"INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE)
                            VALUES(@CUST_USERNAME,@CUST_EMAIL,@ACC_TYPE)";
                                                                command.Parameters.Clear();
                                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                                command.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                                                command.Parameters.AddWithValue("@ACC_TYPE", "Basic");
                                                                command.ExecuteNonQuery();

                                                                command.CommandText = @"INSERT INTO lang_info(CUST_USERNAME,CUST_LANG)
                            VALUES(@CUST_USERNAME,@CUST_LANG)";
                                                                command.Parameters.Clear();
                                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                                command.Parameters.AddWithValue("@CUST_LANG", "US");
                                                                command.ExecuteNonQuery();

                                                                command.CommandText = @"INSERT INTO sharing_info(CUST_USERNAME,DISABLED,SET_PASS)
                            VALUES(@CUST_USERNAME,@DISABLED,@SET_PASS)";
                                                                command.Parameters.Clear();
                                                                command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                                                command.Parameters.AddWithValue("@DISABLED", "0");
                                                                command.Parameters.AddWithValue("@SET_PASS", "DEF");
                                                                command.ExecuteNonQuery();

                                                                transaction.Commit();
                                                            }
                                                            catch (Exception) {
                                                                transaction.Rollback();
                                                            }
                                                        }

                                                        setupAutoLogin(_getUser);

                                                        label11.Visible = false;
                                                        label12.Visible = false;
                                                        label30.Visible = false;
                                                        label6.Text = "20";
                                                        guna2Panel7.Visible = false;
                                                        guna2TextBox1.Text = String.Empty;
                                                        guna2TextBox2.Text = String.Empty;
                                                        guna2TextBox3.Text = String.Empty;
                                                        guna2TextBox4.Text = String.Empty;
                                                        await getCurrentLang();
                                                        setupTime();

                                                        label20.Text = "0" + "%";
                                                        guna2ProgressBar1.Value = 0;

                                                        String[] itemsFolder = { "Home", "Shared To Me", "Shared Files" };
                                                        listBox1.Items.AddRange(itemsFolder);
                                                        listBox1.SelectedIndex = 0;
                                                    }
                                                    else {
                                                        label11.Visible = true;
                                                    }
                                                }
                                                else {
                                                    label12.Visible = true;
                                                }
                                            }
                                            else {
                                                label22.Visible = true;
                                                label22.Text = "Please add your email";
                                            }

                                        }
                                        else {
                                            label12.Visible = true;
                                            label12.Text = "Password must be longer than 5 characters.";
                                        }
                                    }
                                    else {
                                        label11.Visible = true;
                                        label11.Text = "Username character length limit is 20.";
                                    }
                                }
                                else {
                                    label22.Visible = true;
                                    label22.Text = "Entered email is not valid.";
                                }
                            } else {
                                label30.Visible = true;
                                label30.Text = "PIN Number must have 3 digits.";
                                }
                            } else {
                                label30.Visible = true;
                                label30.Text = "Please add a PIN number.";
                            }
                        } else {
                            label11.Visible = true;
                            label11.Text = "Special characters is not allowed.";
                        }
                    } else {
                        label12.Visible = true;
                        label12.Text = "Special characters is not allowed.";
                    }
                }
            }
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private string computeAuthCase(string inputStr) {

            SHA256 sha256 = SHA256.Create();

            string _getAuthStrCase0 = inputStr;
            byte[] _getAuthBytesCase0 = Encoding.UTF8.GetBytes(_getAuthStrCase0);
            byte[] _authHashCase0 = sha256.ComputeHash(_getAuthBytesCase0);
            string _authStrCase0 = BitConverter.ToString(_authHashCase0).Replace("-", "");

            return _authStrCase0;
        }

        /// <summary>
        /// Create file and insert user username into that file in a sub folder 
        /// called FlowStorageInfos located in %appdata%
        /// </summary>
        /// <param name="_custUsername">Username of user</param>
        private void setupAutoLogin(String _custUsername) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if(!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            } else {
                Directory.Delete(appDataPath,true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button17_Click(object sender, EventArgs e) {

        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            LogIN showLogin = new LogIN();
            showLogin.Show();
        }


        private void guna2Button22_Click(object sender, EventArgs e) {

        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e) {

        }

        /// <summary>
        /// 
        /// Insert and generate file panel
        /// from dialog for folder
        /// 
        /// </summary>
        /// <param name="_getDirPath"></param>
        /// <param name="_getDirTitle"></param>
        /// <param name="_TitleValues"></param>
        /// 
        private async void folderDialog(String _getDirPath,String _getDirTitle,String[] _TitleValues) {

            String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);

            void deletionFoldFile(String _Username, String _fileName, String _foldTitle) {
                using (MySqlCommand command = new MySqlCommand()) {
                    String _remQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldtitle AND CUST_FILE_PATH = @filename";
                    command.Connection = con;
                    command.CommandText = _remQue;
                    command.Parameters.AddWithValue("@username", _Username);
                    command.Parameters.AddWithValue("@foldtitle", _foldTitle);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName, EncryptionKey.KeyValue));
                    if (command.ExecuteNonQuery() != 1) {
                        MessageBox.Show("There's an unknown error while attempting to delete this file.", "Error",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                }
            }

            int _IntCurr = 0;
            long fileSizeInMB = 0;

            int top = 275;
            int h_p = 100;

            var _setupUploadAlertThread = new Thread(() => new UploadAlrt(_getDirTitle, label5.Text, "folder_upload_info", "PanExlFold" + _IntCurr, _getDirTitle, _fileSize: fileSizeInMB).ShowDialog());
            _setupUploadAlertThread.Start();

            foreach (var _Files in Directory.EnumerateFiles(_getDirPath, "*")) {

                async Task setupUpload(String _tempToBase64,String thumbnailValue = null) {

                    string insertFoldQue = "INSERT INTO folder_upload_info(FOLDER_TITLE,CUST_USERNAME,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH,CUST_THUMB) VALUES (@FOLDER_TITLE,@CUST_USERNAME,@CUST_FILE,@FILE_TYPE,@UPLOAD_DATE,@CUST_FILE_PATH,@CUST_THUMB)";
                    using (var command = new MySqlCommand(insertFoldQue, con)) {
                        command.Parameters.AddWithValue("@FOLDER_TITLE", _getDirTitle);
                        command.Parameters.AddWithValue("@CUST_USERNAME", label5.Text);
                        command.Parameters.AddWithValue("@FILE_TYPE", Path.GetExtension(_Files));
                        command.Parameters.AddWithValue("@UPLOAD_DATE", DateTime.Now.ToString("dd/MM/yyyy"));
                        command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(Path.GetFileName(_Files), EncryptionKey.KeyValue));
                        command.Parameters.AddWithValue("@CUST_FILE", _tempToBase64);
                        command.Parameters.AddWithValue("@CUST_THUMB", thumbnailValue);
                        //command.Prepare();

                        await command.ExecuteNonQueryAsync();
                    }

                    Application.OpenForms.OfType<Form>().Where(form => form.Name == "UploadAlrt").ForEach(form => form.Close());

                }

                _IntCurr++;

                var panelVid = new Guna2Panel() {
                    Name = $"PanExlFold{_IntCurr}",
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelVid);
                var mainPanelTxt = (Guna2Panel)panelVid;
                _controlName = "PanExlFold" + _IntCurr;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{_IntCurr}";
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = _TitleValues[_IntCurr - 1]; 

                var textboxExl = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxExl);
                textboxExl.Name = $"ExeExlFold{_IntCurr}";
                textboxExl.Width = 240;
                textboxExl.Height = 164;
                textboxExl.FillColor = ColorTranslator.FromHtml("#232323");
                textboxExl.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxExl.BorderRadius = 8;
                textboxExl.Enabled = true;
                textboxExl.Visible = true;

                textboxExl.Click += (sender_w, ev_w) => {

                };

                textboxExl.MouseHover += (_senderM, _ev) => {
                    mainPanelTxt.ShadowDecoration.Enabled = true;
                    mainPanelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxExl.MouseLeave += (_senderQ, _evQ) => {
                    mainPanelTxt.ShadowDecoration.Enabled = false;
                };

                textboxExl.Click += (sender_eq, e_eq) => {

                };

                Guna2Button remButExl = new Guna2Button();
                mainPanelTxt.Controls.Add(remButExl);
                remButExl.Name = $"RemExlButFold{_IntCurr}";
                remButExl.Width = 39;
                remButExl.Height = 35;
                remButExl.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButExl.BorderRadius = 6;
                remButExl.BorderThickness = 1;
                remButExl.BorderColor = ColorTranslator.FromHtml("#232323");
                remButExl.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                remButExl.Visible = true;
                remButExl.Location = new Point(189, 218);

                remButExl.Click += (sender_vid, e_vid) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "'?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        deletionFoldFile(label5.Text, titleLab.Text, label26.Text);
                        panelVid.Dispose();
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }

                    if (flowLayoutPanel1.Controls.Count == 0) {
                        label8.Visible = true;
                        guna2Button6.Visible = true;
                    }
                };

                Label dateLabExl = new Label();
                mainPanelTxt.Controls.Add(dateLabExl);
                dateLabExl.Name = $"LabExlUpFold{_IntCurr}";
                dateLabExl.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                dateLabExl.ForeColor = Color.DarkGray;
                dateLabExl.Visible = true;
                dateLabExl.Enabled = true;
                dateLabExl.Location = new Point(12, 208);
                dateLabExl.Width = 1000;
                dateLabExl.Text = varDate;

                label8.Visible = false;
                guna2Button6.Visible = false;

                _titleText = titleLab.Text;

                var _extTypes = Path.GetExtension(_Files);

                try {

                    Byte[] _getBytes = File.ReadAllBytes(_Files);
                    fileSizeInMB = (_getBytes.Length / 1024) / 1024;

                    if (_extTypes == ".png" || _extTypes == ".jpg" || _extTypes == ".jpeg" || _extTypes == ".bmp" || _extTypes == ".psd") {

                        var _imgContent = new Bitmap(_Files);

                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        await setupUpload(_tobase64);

                        textboxExl.Image = _imgContent;
                        textboxExl.Click += (sender_f, e_f) => {
                            var getImgName = (Guna2PictureBox)sender_f;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", label5.Text)) {
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
                    if (_extTypes == ".txt" || _extTypes == ".py" || _extTypes == ".html" || _extTypes == ".css" || _extTypes == ".js" || _extTypes == ".sql" || _extTypes == ".csv") {
                        if (_extTypes == ".py") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                        }
                        else if (_extTypes == ".txt") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                        }
                        else if (_extTypes == ".html") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                        }
                        else if (_extTypes == ".css") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                        }
                        else if (_extTypes == ".js") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (_extTypes == ".csv") {
                            textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        String nonLine = "";
                        using (StreamReader ReadFileTxt = new StreamReader(_Files)) {
                            nonLine = ReadFileTxt.ReadToEnd();
                        }
                        byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(nonLine);
                        String getEncoded = Convert.ToBase64String(getBytes);
                        String encryptEncoded = EncryptionModel.Encrypt(getEncoded, EncryptionKey.KeyValue);

                        await setupUpload(encryptEncoded);

                        textboxExl.Click += (sender_t, e_t) => {

                            if (_extTypes == ".csv" || _extTypes == ".sql") {
                                Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                                _showRetrievalCsvAlert.Start();
                            }

                            txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text, "null", label5.Text);
                            displayPic.Show();
                        };

                    }

                    if (_extTypes == ".apk") {

                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                        textboxExl.Click += (sender_ap, e_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk", "null");
                            displayPic.ShowDialog();
                        };
                    }
                    if (_extTypes == ".mp4" || _extTypes == ".mov" || _extTypes == ".webm" || _extTypes == ".avi" || _extTypes == ".wmv") {
                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        String toBase64BitmapThumbnail;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            toBase64BitmapThumbnail = toBase64;
                        }

                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(_getBytes), EncryptionKey.KeyValue);
                        await setupUpload(_tempToBase64, toBase64BitmapThumbnail);

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            Form bgBlur = new Form();
                            vidFORM displayVid = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".gif") {
                        ShellFile shellFile = ShellFile.FromFilePath(_Files);
                        Bitmap toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            var toBase64 = Convert.ToBase64String(stream.ToArray());
                            command.Parameters["@CUST_THUMB"].Value = toBase64;
                        }

                        String _tempToBase64 = EncryptionModel.Encrypt(Convert.ToBase64String(_getBytes), "0123456789085746");
                        command.Parameters["@CUST_FILE"].Value = _tempToBase64;
                        command.ExecuteNonQuery();
                        command.Dispose();

                        textboxExl.Image = toBitMap;
                        textboxExl.Click += (sender_vid, e_vid) => {
                            var getImgName = (Guna2PictureBox)sender_vid;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayVid.ShowDialog();
                        };
                    }

                    if (_extTypes == ".pdf") {

                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".docx" || _extTypes == ".doc") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            wordFORM displayPic = new wordFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".xlsx" || _extTypes == ".xls") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);
                        
                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            exlFORM displayPic = new exlFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }


                    if (_extTypes == ".pptx" || _extTypes == ".ppt") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);
                        
                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            ptxFORM displayPic = new ptxFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                    if (_extTypes == ".mp3" || _extTypes == ".wav") {
                        String _tobase64 = Convert.ToBase64String(_getBytes);
                        String encryptValues = EncryptionModel.Encrypt(_tobase64, EncryptionKey.KeyValue);
                        await setupUpload(encryptValues);

                        textboxExl.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        textboxExl.Click += (sender_pdf, e_pdf) => {
                            Form bgBlur = new Form();
                            audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _selectedFolder, label5.Text);
                            displayPic.ShowDialog();
                        };
                    }

                }
                catch (Exception) {
                    //MessageBox.Show("An error ocurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

            var uploadAlertForm = Application.OpenForms.OfType<Form>().FirstOrDefault(form => form.Name == "UploadAlrt");
            uploadAlertForm?.Close();

            guna2Button13.FillColor = Color.FromArgb(255, 71, 19, 191);
            guna2Button9.FillColor = Color.Transparent;
            panel1.SendToBack();
            panel3.BringToFront();
            label9.Visible = true;
            listBox1.Visible = true;

            clearRedundane();
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
            
        }
        private String getAccountType() {
            List<String> _types = new List<String>();
            String _getAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccType, con);
            command.Parameters.AddWithValue("@username", label5.Text);

            MySqlDataReader readAccType = command.ExecuteReader();
            while (readAccType.Read()) {
                _types.Add(readAccType.GetString(0));
            }
            readAccType.Close();
            String _accType = _types[0];
            return _accType;
        }

        // FOLDER

        /// <summary>
        /// 
        /// Open FileDialog and ask user
        /// to select file and send those file 
        /// metadata into DB and generate panel file on folderDialog
        /// 
        /// </summary>
        private String _titleText;
        private String _controlName;
        private void guna2Button12_Click(object sender, EventArgs e) {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {

                var _getDirPath = dialog.FileName;
                var _accType = getAccountType();
                int _countFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.TopDirectoryOnly).Length;
                var _getDirTitle = new DirectoryInfo(_getDirPath).Name;

                if(!(listBox1.Items.Contains(_getDirTitle))) {

                    String[] _TitleValues = Directory.GetFiles(_getDirPath, "*").Select(Path.GetFileName).ToArray();
                    int _numberOfFiles = Directory.GetFiles(_getDirPath, "*", SearchOption.AllDirectories).Length;

                    if (_accType == "Basic") {
                        if (_numberOfFiles <= 20) {
                            flowLayoutPanel1.Controls.Clear();
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath,_getDirTitle,_TitleValues);
                            var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                            listBox1.SelectedIndex = _dirPosition;
                        }
                        else {
                            listBox1.SelectedIndex = 0;
                            DisplayErrorFolder(_accType);
                        }
                    }

                    if (_accType == "Max") {
                        if (_numberOfFiles <= 500) {
                            flowLayoutPanel1.Controls.Clear();
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                        }
                        else {
                            listBox1.SelectedIndex = 0;
                            DisplayErrorFolder(_accType);
                        }
                    }

                    if (_accType == "Express") {
                        if (_numberOfFiles <= 1000) {
                            flowLayoutPanel1.Controls.Clear();
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                            var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                            listBox1.SelectedIndex = _dirPosition;
                        }
                        else {
                            DisplayErrorFolder(_accType);
                            listBox1.SelectedItem = "Home";
                        }
                    }

                    if (_accType == "Supreme") {
                        if (_numberOfFiles <= 2000) {
                            listBox1.Items.Add(_getDirTitle);
                            folderDialog(_getDirPath, _getDirTitle, _TitleValues);
                            var _dirPosition = listBox1.Items.IndexOf(_getDirTitle);
                            listBox1.SelectedIndex = _dirPosition;
                        }
                        else {
                            DisplayErrorFolder(_accType);
                            listBox1.SelectedItem = "Home";
                        }
                    }
                } else {
                    MessageBox.Show("Folder already exists","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }
        /// <summary>
        /// Function to display alert form if the 
        /// number of user folder files exceeding the amount of file 
        /// they can upload
        /// </summary>
        /// <param name="CurAcc"></param>
        public void DisplayErrorFolder(String CurAcc) {
            Form bgBlur = new Form();
            using (UpgradeFormFold displayPic = new UpgradeFormFold(CurAcc)) {
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
        }

        private void label25_Click(object sender, EventArgs e) {

        }

        private async void buildHomeFiles() {

            async Task<int> _countRow(String _tableName) {
                using (var command = con.CreateCommand()) {
                    command.CommandText = $"SELECT COUNT(CUST_USERNAME) FROM {_tableName} WHERE CUST_USERNAME = @username";
                    command.Parameters.AddWithValue("@username", label5.Text);
                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            var tableNames = new Dictionary<string, string> {
                { "file_info", "imgFile" },
                { "file_info_expand", "txtFile" },
                { "file_info_exe", "exeFile" },
                { "file_info_vid", "vidFile" },
                { "file_info_excel", "exlFile" },
                { "file_info_audi", "audiFile" },
                { "file_info_gif", "gifFile" },
                { "file_info_apk", "apkFile" },
                { "file_info_pdf", "pdfFile" },
                { "file_info_ptx", "ptxFile" },
                { "file_info_msi", "msiFile" },
                { "file_info_word", "docFile" },
                { "file_info_directory", "dirFile" }
            };

            foreach (string tableName in tableNames.Keys) {
                if (await _countRow(tableName) > 0) {
                    if (tableNames[tableName] == "dirFile") {
                        await _generateUserDirectory(tableName, tableNames[tableName], await _countRow(tableName));
                    }
                    else {
                        await _generateUserFiles(tableName, tableNames[tableName], await _countRow(tableName));
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

        }

        private async void buildSharedToMe() {

            if (!_TypeValues.Any()) {
                string getFilesTypeQuery = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
                using (MySqlCommand command = new MySqlCommand(getFilesTypeQuery, ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            _TypeValues.Add(reader.GetString(0));
                        }
                    }
                }
                await generateUserShared(_TypeValues, "DirParMe", _TypeValues.Count);
            }
            else {
                await generateUserShared(_TypeValues, "DirParMe", _TypeValues.Count);
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

        }

        private async void buildSharedToOthers() {

            if (!_TypeValuesOthers.Any()) {
                string getFilesTypeOthers = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
                using (var command = new MySqlCommand(getFilesTypeOthers, con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    using (var readTypeOthers = await command.ExecuteReaderAsync()) {
                        while (await readTypeOthers.ReadAsync()) {
                            _TypeValuesOthers.Add(readTypeOthers.GetString(0));
                        }
                    }
                }
            }

            await generateUserSharedOthers(_TypeValuesOthers, "DirParOther", _TypeValuesOthers.Count);

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }
        }

        /// <summary>
        /// Select folder from listBox and start showing
        /// the files from selected folder
        /// </summary>

        public List<String> _TypeValuesOthers = new List<String>();
        public List<String> _TypeValues = new List<String>();
        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e) {

            try {

                int _selectedIndex = listBox1.SelectedIndex;
                String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);
                label26.Text = _selectedFolder;
                guna2Panel17.Visible = true;
                label27.Visible = true;
                label26.Visible = true;
                guna2Button19.Visible = true;

                if (_selectedFolder == "Home") {

                    guna2Button19.Visible = false;
                    guna2Button4.Visible = false;
                    guna2Button8.Visible = true;
                    flowLayoutPanel1.Controls.Clear();

                    buildHomeFiles();

                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                }
                else if (_selectedFolder != "Home" && _selectedFolder != "Shared To Me" && _selectedFolder != "Shared Files") {

                    guna2Button19.Visible = true;
                    guna2Button8.Visible = false;
                    flowLayoutPanel1.Controls.Clear();
                    flowLayoutPanel1.WrapContents = true;

                    var typesValues = new List<string>();
                    var getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                    using (var command = new MySqlCommand(getFileType, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _selectedFolder);
                        using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                typesValues.Add(reader.GetString(0));
                            }
                        }
                    }

                    var mainTypes = typesValues.Distinct().ToList();
                    var currMainLength = typesValues.Count;
                    await _generateUserFold(typesValues, _selectedFolder, "TESTING", currMainLength);

                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    if (flowLayoutPanel1.Controls.Count == 0) {
                        showRedundane();
                    }
                    else {
                        clearRedundane();
                    }

                } else if (_selectedIndex == 1) {

                    guna2Button4.Visible = true;
                    guna2Button8.Visible = false;
                    guna2Button19.Visible = false;
                    flowLayoutPanel1.Controls.Clear();

                    clearRedundane();

                    _callFilesInformationShared();

                    buildSharedToMe();

                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                }
                else if (_selectedIndex == 2) {

                    guna2Button4.Visible = true;
                    guna2Button8.Visible = false;
                    guna2Button19.Visible = false;
                    flowLayoutPanel1.Controls.Clear();

                    clearRedundane();

                    _callFilesInformationOthers();

                    buildSharedToOthers();

                    label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                }

            } catch (Exception) {

                flowLayoutPanel1.Controls.Clear();

                if (flowLayoutPanel1.Controls.Count == 0) {
                    showRedundane();
                }
                else {
                    clearRedundane();
                }

                MessageBox.Show("Hmm... something is wrong. Restarting Flowstorage may fix the problem.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// This function will delete user folder if 
        /// Garbage (delete folder) button is clicked
        /// </summary>
        /// <param name="foldName"></param>
        public void _removeFoldFunc(String foldName) {

            DialogResult verifyDeletion = MessageBox.Show("Delete " + foldName + " folder?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (verifyDeletion == DialogResult.Yes) {

                String removeFoldQue = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";
                command = new MySqlCommand(removeFoldQue, con);
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@foldername", foldName);
                command.ExecuteNonQuery();
               
                listBox1.Items.Remove(foldName);
                foreach (var _DupeItem in listBox1.Items) {
                    if (_DupeItem.ToString() == foldName) {
                        listBox1.Items.Remove(_DupeItem.ToString());
                    }
                }

                int indexSelected = listBox1.Items.IndexOf("Home");
                listBox1.SelectedIndex = indexSelected;
                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
            }
        }

        /// <summary>
        /// Retrieve username of file that has been shared to
        /// </summary>
        /// <returns></returns>
        private string uploaderName() {
            String selectUploaderName = "SELECT CUST_FROM FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(selectUploaderName, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }

        /// <summary>
        /// Retrieve username of user you shared file to
        /// </summary>
        /// <returns></returns>
        String getUploaderNameShared = "";
        private string sharedToName() {
            String selectUploaderName = "SELECT CUST_TO FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (MySqlCommand command = new MySqlCommand(selectUploaderName, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(getUploaderNameShared, "0123456789085746"));
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
            }

            return String.Empty;

        }
        
        /// <summary>
        /// Generate files that has been shared to other
        /// </summary>
        /// <param name="_extTypes"></param>
        /// <param name="parameterName"></param>
        /// <param name="itemCurr"></param>
        /// 
            
        List<Tuple<string, string>> filesInfoSharedOthers = new List<Tuple<string, string>>();
        private async void _callFilesInformationOthers() {
            filesInfoSharedOthers.Clear();
            string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_FROM = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        string uploadDate = reader.GetString(1);
                        filesInfoSharedOthers.Add(new Tuple<string, string>(fileName, uploadDate));
                    }
                }
            }
        }

        private async Task generateUserSharedOthers(List<String> _extTypes, String parameterName, int itemCurr) {

            var form1 = Form1.instance;

            List<String> typeValues = new List<String>(_extTypes);

            int top = 275;
            int h_p = 100;

            for (int q = 0; q < itemCurr; q++) {


                var panelTxt = new Guna2Panel() {
                    Name = $"{parameterName}{q}",
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelTxt);
                var mainPanelTxt = (Guna2Panel)panelTxt;  
                _controlName = $"{parameterName}{q}";

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = $"TxtBox{q}";
                textboxPic.Width = 240;
                textboxPic.Height = 164;
                textboxPic.BorderRadius = 8;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"LabVidUp{q}";
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfoSharedOthers[q].Item1;

                getUploaderNameShared = titleLab.Text;
                var setupSharedUsername = sharedToName();
                var SharedToName = getUploaderNameShared;

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = $"RemTxtBut{q}";
                remButTxt.Width = 39;
                remButTxt.Height = 35;
                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                remButTxt.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                remButTxt.Visible = true;
                remButTxt.Location = new Point(189, 218);
                remButTxt.BringToFront();

                remButTxt.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("'" + titleFile + "' Delete this shared file?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {

                        string noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        using (MySqlCommand cmd = new MySqlCommand(noSafeUpdate, con)) {
                            cmd.ExecuteNonQuery();
                        }

                        string removeQuery = "DELETE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                        using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                            cmd.Parameters.AddWithValue("@username", label5.Text);
                            cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile, EncryptionKey.KeyValue));
                            cmd.Parameters.AddWithValue("@sharedname", setupSharedUsername);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 1) {
                                // Ignore success file deletion
                            }
                            else {
                                MessageBox.Show("Failed to delete this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }

                        mainPanelTxt.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = $"LabTxtUp{q}";
                dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                dateLabTxt.ForeColor = Color.DarkGray;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = new Point(12, 208);
                dateLabTxt.Width = 1000;
                dateLabTxt.Text = filesInfoSharedOthers[q].Item2;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                if (typeValues[q] == ".png" || typeValues[q] == ".jpeg" || typeValues[q] == ".jpg" || typeValues[q] == ".bmp") {

                    /*Dictionary<int, string> base64Encoded = new Dictionary<int, string>();

                    string cacheKeySet = $"user_other_{label5.Text}_images";
                    MemoryCache cache = MemoryCache.Default;

                    if (cache.Contains(cacheKeySet)) {
                        base64Encoded = (Dictionary<int, string>)cache.Get(cacheKeySet);
                    }
                    else {
                        string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.Add("@username", MySqlDbType.Text).Value = label5.Text;
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                int index = 0;
                                while (await readBase64.ReadAsync()) {
                                    base64Encoded[index++] = EncryptionModel.Decrypt(readBase64.GetString(0), "0123456789085746");
                                }
                            }
                        }
                        CacheItemPolicy cachePolicy = new CacheItemPolicy {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
                        };
                        cache.Add(cacheKeySet, base64Encoded, cachePolicy);
                    }

                    if (base64Encoded.ContainsKey(q)) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = Image.FromStream(toMs);
                        }
                    }*/



                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = label5.Text;

                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Decrypt(readBase64.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }

                    if (base64Encoded.Count > q) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, "Shared "  + sharedToName(),true)) {
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
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", label1.Text, sharedToName(),true);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".pdf") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".apk") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        apkFORM displayApk = new apkFORM(titleLab.Text, "Shared To " + sharedToName(), "cust_sharing", label1.Text,true);
                        displayApk.Show();
                    };
                }

                if (typeValues[q] == ".msi") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayMsi.Show();
                    };
                }

                if (typeValues[q] == ".docx" || typeValues[q] == ".doc") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayDoc.Show();
                    };
                }

                if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    textboxPic.Click += (sender_im, e_im) => {
                        exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayXls.Show();
                    };
                }

                if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".mp4" || typeValues[q] == ".mov" || typeValues[q] == ".webm" || typeValues[q] == ".avi" || typeValues[q] == ".wmv") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));

                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > 0) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }


                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        vidFORM displayAud = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, "Shared " + sharedToName(),true);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".exe") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(),true);
                        displayExe.Show();
                    };
                }

                if (typeValues[q] == ".txt" || typeValues[q] == ".html" || typeValues[q] == ".xml" || typeValues[q] == ".py" || typeValues[q] == ".css" || typeValues[q] == ".js" || typeValues[q] == ".sql" || typeValues[q] == ".csv") {

                    if (typeValues[q] == ".py") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;
                    }
                    else if (typeValues[q] == ".txt") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;
                    }
                    else if (typeValues[q] == ".html") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;
                    }
                    else if (typeValues[q] == ".css") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;
                    }
                    else if (typeValues[q] == ".js") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    }
                    else if (typeValues[q] == ".sql") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (typeValues[q] == ".csv") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    textboxPic.Click += (sender_im, e_im) => {

                        txtFORM displayTxt = new txtFORM("", "cust_sharing", titleLab.Text, label1.Text, "Shared " +  sharedToName());
                        displayTxt.Show();
                    };
                }

                if (typeValues[q] == ".gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

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
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(),true)) {
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


                label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                if (flowLayoutPanel1.Controls.Count > 0) {
                    label8.Visible = false;
                    guna2Button6.Visible = false;
                }

            }
        }

        /// <summary>
        /// Generate panel for user shared file
        /// </summary>
        /// <param name="_extTypes"></param>
        /// <param name="parameterName"></param>
        /// <param name="itemCurr"></param>
        /// 

        List<Tuple<string, string>> filesInfoShared = new List<Tuple<string, string>>();
        private async void _callFilesInformationShared() {

            filesInfoShared.Clear();

            string selectFileData = "SELECT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_TO = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        string uploadDate = reader.GetString(1);
                        filesInfoShared.Add(new Tuple<string, string>(fileName, uploadDate));
                    }
                }
            }

        }

        private async Task generateUserShared(List<String> _extTypes, String parameterName, int itemCurr) {

            var form1 = Form1.instance;

            var UploaderUsername = uploaderName();

            List<String> typeValues = new List<String>(_extTypes);

            int top = 275;
            int h_p = 100;

            for (int q = 0; q < itemCurr; q++) {
                var panelTxt = new Guna2Panel() {
                    Name = $"parameterName{q}",
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowLayoutPanel1.Controls.Add(panelTxt);
                var mainPanelTxt = (Guna2Panel)panelTxt; 
                _controlName = parameterName + q;

                var textboxPic = new Guna2PictureBox();
                mainPanelTxt.Controls.Add(textboxPic);
                textboxPic.Name = $"TxtBox{q}";
                textboxPic.Width = 240;
                textboxPic.Height = 164;
                textboxPic.BorderRadius = 8;
                textboxPic.SizeMode = PictureBoxSizeMode.CenterImage;
                textboxPic.Enabled = true;
                textboxPic.Visible = true;

                Label titleLab = new Label();
                mainPanelTxt.Controls.Add(titleLab);
                titleLab.Name = $"LabVidUp{q}";
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfoShared[q].Item1;

                Guna2Button remButTxt = new Guna2Button();
                mainPanelTxt.Controls.Add(remButTxt);
                remButTxt.Name = $"RemTxtBut{q}";
                remButTxt.Width = 39;
                remButTxt.Height = 35;
                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                remButTxt.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                remButTxt.Visible = true;
                remButTxt.Location = new Point(189, 218);
                remButTxt.BringToFront();

                remButTxt.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        using (var command = new MySqlCommand(noSafeUpdate, con)) {
                            command.ExecuteNonQuery();
                        }

                        String removeQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                        using (var command = new MySqlCommand(removeQuery, con)) {
                            command.Parameters.AddWithValue("@username", form1.label5.Text);
                            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile, EncryptionKey.KeyValue));
                            command.ExecuteNonQuery();
                        }

                        mainPanelTxt.Dispose();

                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }

                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                Label dateLabTxt = new Label();
                mainPanelTxt.Controls.Add(dateLabTxt);
                dateLabTxt.Name = $"LabTxtUp{q}";
                dateLabTxt.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                dateLabTxt.ForeColor = Color.DarkGray;
                dateLabTxt.Visible = true;
                dateLabTxt.Enabled = true;
                dateLabTxt.Location = new Point(12, 208);
                dateLabTxt.Width = 1000;
                dateLabTxt.Text = filesInfoShared[q].Item2;

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                if (typeValues[q] == ".png" || typeValues[q] == ".jpeg" || typeValues[q] == ".jpg" || typeValues[q] == ".bmp") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = label5.Text;
                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(EncryptionModel.Decrypt(readBase64.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }

                    if (base64Encoded.Count > q) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, UploaderUsername,false)) {
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

                if (typeValues[q] == ".pptx" || typeValues[q] == ".ppt") {

                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername,false);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".pdf") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername,false);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".apk") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (apkFORM displayApk = new apkFORM(titleLab.Text, UploaderUsername, "cust_sharing", label1.Text,true)) {
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
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername, false);
                        displayDoc.Show();
                    };
                }

                if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    textboxPic.Click += (sender_im, e_im) => {
                        exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername, false);
                        displayXls.Show();
                    };
                }

                if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername, false);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".mp4" || typeValues[q] == ".mov" || typeValues[q] == ".webm" || typeValues[q] == ".avi" || typeValues[q] == ".wmv") {

                    List<string> base64Encoded = new List<string>();

                    string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));

                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > 0) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[0]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        vidFORM displayAud = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, UploaderUsername,false);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".exe") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername);
                        displayExe.Show();
                    };
                }

                if (typeValues[q] == ".txt" || typeValues[q] == ".html" || typeValues[q] == ".xml" || typeValues[q] == ".py" || typeValues[q] == ".css" || typeValues[q] == ".js" || typeValues[q] == ".sql" || typeValues[q] == ".csv") {

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
                    else if (typeValues[q] == ".csv") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        txtFORM displayTxt = new txtFORM("", "cust_sharing", titleLab.Text, label1.Text, UploaderUsername, false);
                        displayTxt.Show();
                    };
                }

                if (typeValues[q] == ".gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(EncryptionModel.Decrypt(_base64Encoded[q], EncryptionKey.KeyValue));
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    textboxPic.Image = new Bitmap(_toMs);
                    textboxPic.Click += (sender_im, e_im) => {
                        Form bgBlur = new Form();
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", label1.Text, UploaderUsername, false)) {
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

                Application.OpenForms
                          .OfType<Form>()
                          .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                          .ToList()
                          .ForEach(form => form.Close());


                label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                if (flowLayoutPanel1.Controls.Count > 0) {
                    label8.Visible = false;
                    guna2Button6.Visible = false;
                }
            }
        }

        // REMOVE DIR BUT
        private void guna2Button19_Click(object sender, EventArgs e) {
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "RetrievalAlert"))
              .ToList()
              .ForEach(form => form.Close());
            String _currentFold = listBox1.GetItemText(listBox1.SelectedItem);
            _removeFoldFunc(_currentFold);
        }


        /// <summary>
        /// Generate user directory from Home folder
        /// </summary>
        /// <param name="userName">Username of user</param>
        /// <param name="customParameter">Custom parameter for panel</param>
        /// <param name="rowLength"></param>
        private async Task _generateUserDirectory(String userName,String customParameter, int rowLength) {

            List<Tuple<string>> filesInfo = new List<Tuple<string>>();
            string selectFileData = $"SELECT DIR_NAME FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectFileData, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string fileName = EncryptionModel.Decrypt(reader.GetString(0), EncryptionKey.KeyValue);
                        filesInfo.Add(new Tuple<string>(fileName));
                    }
                }
            }

            flowLayoutPanel1.Location = new Point(13, 10);
            flowLayoutPanel1.Size = new Size(1118, 579);

            for (int i = 0; i < rowLength; i++) {
                int top = 275;
                int h_p = 100;

                var panelPic_Q = new Guna2Panel() {
                    Name = "ABC02" + i,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label directoryLab = new Label();
                panelF.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + i;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                directoryLab.ForeColor = Color.DarkGray;
                directoryLab.Location = new Point(12, 208);
                directoryLab.BackColor = Color.Transparent;
                directoryLab.Width = 75;
                directoryLab.Text = "Directory";

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titleImgL" + i;
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = filesInfo[i].Item1;

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
                remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                remBut.Visible = true;
                remBut.Location = new Point(189, 218);

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' Directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String _removeDirQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleLab.Text, EncryptionKey.KeyValue));
                        command.ExecuteNonQuery();

                        String _removeDirUploadQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirUploadQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleLab.Text,EncryptionKey.KeyValue));
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                    }
                };

                picMain_Q.Image = FlowSERVER1.Properties.Resources.DirIcon;
                picMain_Q.Click += (sender_dir, ev_dir) => {

                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader").ShowDialog());
                    ShowAlert.Start();

                    Form3 displayDirectory = new Form3(titleLab.Text);
                    displayDirectory.Show();

                    Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                    .ToList()
                    .ForEach(form => form.Close());
                };
            }
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private void guna2Panel16_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void label10_Click(object sender, EventArgs e) {

        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button9_Click(object sender, EventArgs e) {

        }

        private void label17_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox1_TextChanged_1(object sender, EventArgs e) {
           
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e) {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void label16_Click(object sender, EventArgs e) {

        }

        private void guna2GradientPanel1_Paint_1(object sender, PaintEventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {

        }

        private void label22_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox4.Text, "[^0-9]")) {
                guna2TextBox4.Text = guna2TextBox4.Text.Remove(guna2TextBox4.Text.Length - 1);
            }
        }

        private void guna2TextBox2_TextChanged_1(object sender, EventArgs e) {

        }

        private void label11_Click(object sender, EventArgs e) {

        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e) {

        }

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e) {

        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e) {
            this.Invalidate();
            base.OnScroll(e);
        }

        private void label19_Click(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint_1(object sender, PaintEventArgs e) {

        }

        private void backgroundWorker1_DoWork_2(object sender, DoWorkEventArgs e) {

        }

        private void backgroundWorker1_ProgressChanged_2(object sender, ProgressChangedEventArgs e) {
            
        }

        private void backgroundWorker1_RunWorkerCompleted_2(object sender, RunWorkerCompletedEventArgs e) {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {

        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e) {
            Process.Start("https://apps.microsoft.com/store/detail/flowstorage/9PKQW5LQLBT5");
        }
        /// <summary>
        /// 
        /// Generate user searched file in folder
        /// 
        /// </summary>
        /// <param name="_fileType"></param>
        /// <param name="_foldTitle"></param>
        /// <param name="parameterName"></param>
        /// <param name="currItem"></param>
        private async void folderSearching(List<String> _fileType, String _foldTitle, String parameterName, int currItem) {

            Application.DoEvents();

            List<String> typeValues = new List<String>(_fileType);

            flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i <= currItem-1; i++) {

                int top = 275;
                int h_p = 100;

                flowLayoutPanel1.Location = new Point(13, 10);
                flowLayoutPanel1.Size = new Size(1118, 579);

                var panelPic_Q = new Guna2Panel() {
                    Name = "panelf" + i,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };
                top += h_p;
                flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = ((Guna2Panel)flowLayoutPanel1.Controls["panelf" + i]);

                List<string> dateValues = new List<string>();
                List<string> titleValues = new List<string>();

                String selectDateValues = "SELECT UPLOAD_DATE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";
                using (MySqlCommand command = new MySqlCommand(selectDateValues, con)) {
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            dateValues.Add(reader.GetString(0));
                        }
                    }
                }

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = "datef" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                dateLab.ForeColor = Color.DarkGray;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = new Point(12, 208);
                dateLab.Text = dateValues[i];

                String selectFileName = "SELECT CUST_FILE_PATH FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";
                using (MySqlCommand command = new MySqlCommand(selectFileName, con)) {
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            titleValues.Add(reader.GetString(0));
                        }
                    }
                }

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = "titlef" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                titleLab.ForeColor = Color.Gainsboro;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 182);
                titleLab.Width = 220;
                titleLab.Height = 30;
                titleLab.Text = titleValues[i];

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "imgf" + i;
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
                remBut.Name = "remf" + i;
                remBut.Width = 39;
                remBut.Height = 35;
                remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remBut.Visible = true;
                remBut.Location = new Point(189, 218);

                remBut.Click += (sender_im, e_im) => {
                    var titleFile = titleLab.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        String removeQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile, EncryptionKey.KeyValue));
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.ExecuteNonQuery();

                        panelPic_Q.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label8.Visible = true;
                            guna2Button6.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();

                    }
                };

                guna2Button6.Visible = false;
                label8.Visible = false;

                Application.DoEvents();

                var img = ((Guna2PictureBox)panelF.Controls["imgf" + i]);

                if (typeValues[i] == ".png" || typeValues[i] == ".jpg" || typeValues[i] == ".jpeg") {

                    List<string> base64Encoded = new List<string>();
                    string retrieveImgQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";

                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            picMain_Q.Image = new Bitmap(toMs);
                        }
                    }

                    picMain_Q.Click += (sender, e) => {
                        var getImgName = (Guna2PictureBox)sender;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "folder_upload_info", "null", label5.Text)) {
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


                if (typeValues[i] == ".txt" || typeValues[i] == ".py" || typeValues[i] == ".html" || typeValues[i] == ".css" || typeValues[i] == ".js" || typeValues[i] == ".sql" || typeValues[i] == ".csv") {
                    String retrieveImg = "SELECT CONVERT(CUST_FILE USING utf8) FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

                    List<String> textValues_ = new List<String>();

                    MySqlDataReader _ReadTexts = command.ExecuteReader();
                    while (_ReadTexts.Read()) {
                        textValues_.Add(_ReadTexts.GetString(0));
                    }
                    _ReadTexts.Close();
                    var getMainText = textValues_[0];

                    var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                    if (typeValues[i] == ".py") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                    }
                    else if (typeValues[i] == ".txt") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                    }
                    else if (_extTypes == ".html") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                    }
                    else if (_extTypes == ".css") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                    }
                    else if (_extTypes == ".js") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                    }
                    else if (_extTypes == ".sql") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                    }
                    else if (_extTypes == ".csv") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    picMain_Q.Click += (sender_t, e_t) => {

                        if (_extTypes == ".csv" || _extTypes == ".sql") {
                            Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                            _showRetrievalCsvAlert.Start();
                        }

                        txtFORM displayPic = new txtFORM("", "folder_upload_info", titleLab.Text, "null", label5.Text);
                        displayPic.Show();
                    };
                    clearRedundane();
                }

                if (typeValues[i] == ".mp4" || typeValues[i] == ".mov" || typeValues[i] == ".webm" || typeValues[i] == ".avi" || typeValues[i] == ".wmv") {

                    List<string> base64Encoded = new List<string>();
                    string retrieveImgQuery = "SELECT CUST_THUMB FROM folder_upload_info WHERE CUST_THUMB IS NOT NULL AND CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";

                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@foldername", _foldTitle);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > i) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            img.Image = new Bitmap(toMs);
                        }
                    }

                    img.Click += (sender_vid, e_vid) => {
                        var getImgName = (Guna2PictureBox)sender_vid;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImg = new Bitmap(getImgName.Image);
                        Form bgBlur = new Form();
                        vidFORM displayVid = new vidFORM(defaultImg, getWidth, getHeight, titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayVid.Show();
                    };
                }

                if (typeValues[i] == ".gif") {
                    List<String> _base64Encoded = new List<string>();
                    String retrieveImg = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", label5.Text);
                    command.Parameters.AddWithValue("@foldername", _foldTitle);
                    command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

                    MySqlDataReader _readBase64 = command.ExecuteReader();
                    while (_readBase64.Read()) {
                        _base64Encoded.Add(_readBase64.GetString(0));
                    }
                    _readBase64.Close();

                    var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                    MemoryStream _toMs = new MemoryStream(_getBytes);

                    img.Image = new Bitmap(_toMs);
                    img.Click += (sender_vid, e_vid) => {
                        var getImgName = (Guna2PictureBox)sender_vid;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImg = new Bitmap(getImgName.Image);
                        Form bgBlur = new Form();
                        using (gifFORM displayVid = new gifFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text)) {
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.FormBorderStyle = FormBorderStyle.None;
                            bgBlur.Opacity = .24d;
                            bgBlur.BackColor = Color.Black;
                            bgBlur.WindowState = FormWindowState.Maximized;
                            bgBlur.Name = "bgBlurForm";
                            bgBlur.TopMost = true;
                            bgBlur.Location = this.Location;
                            bgBlur.StartPosition = FormStartPosition.Manual;
                            bgBlur.ShowInTaskbar = false;
                            bgBlur.Show();

                            displayVid.Owner = bgBlur;
                            displayVid.ShowDialog();

                            bgBlur.Dispose();
                        }
                    };
                }

                if (typeValues[i] == ".xlsx" || typeValues[i] == ".xls") {
                    img.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    img.Click += (sender_aud, e_aud) => {
                        Form bgBlur = new Form();
                        exlFORM displayExl = new exlFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayExl.Show();
                    };
                }

                if (typeValues[i] == ".wav" || typeValues[i] == ".mp3") {
                    var _getWidth = this.Width;
                    var _getHeight = this.Height;
                    img.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    img.Click += (sender_aud, e_aud) => {
                        audFORM displayPic = new audFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayPic.Show();
                    };
                }

                if (typeValues[i] == ".apk") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    img.Click += (sender_ap, e_ap) => {
                        apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "folder_upload_info", _foldTitle);
                        displayPic.Show();
                    };
                }

                if (typeValues[i] == ".exe") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_exe_96;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                    img.Click += (sender_ap, e_ap) => {
                        exeFORM displayPic = new exeFORM(titleLab.Text, "folder_upload_info", _foldTitle, "null");
                        displayPic.Show();
                    };
                }

                if (typeValues[i] == ".pdf") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    img.Click += (sender_pdf, e_pdf) => {
                        pdfFORM displayPic = new pdfFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayPic.Show();
                    };
                }

                if (typeValues[i] == ".docx" || typeValues[i] == ".doc") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    img.Click += (sender_pdf, e_pdf) => {
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayDoc.Show();
                    };
                }

                if (typeValues[i] == ".pptx" || typeValues[i] == ".ppt") {
                    img.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    img.Click += (sender_pdf, e_pdf) => {
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "folder_upload_info", _foldTitle, label5.Text);
                        displayDoc.Show();
                    };
                }

                if (typeValues[i] == ".msi") {
                    picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    picMain_Q.Click += (sender_pt, e_pt) => {
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "folder_upload_info", _foldTitle,label5.Text);
                        displayMsi.Show();
                    };
                }
            }

            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
            clearRedundane();

            if (label4.Text == "0") {
                showRedundane();
            }

            Application.DoEvents();

        }

        /// <summary>
        ///
        /// Starts generating searched file 
        /// from the input
        /// 
        /// </summary>
        /// <param name="_TableName"></param>
        private async void generateSearching(String _tableName, String parameterName, int currItem) {

            try {

                for (int i = 0; i < currItem; i++) {
                    int top = 275;
                    int h_p = 100;

                    flowLayoutPanel1.Location = new Point(13, 10);
                    flowLayoutPanel1.Size = new Size(1118, 579);

                    var panelPic_Q = new Guna2Panel() {
                        Name = parameterName + i,
                        Width = 240,
                        Height = 262,
                        BorderRadius = 8,
                        FillColor = ColorTranslator.FromHtml("#121212"),
                        BackColor = Color.Transparent,
                        Location = new Point(600, top)
                    };
                    top += h_p;
                    flowLayoutPanel1.Controls.Add(panelPic_Q);

                    var panelF = ((Guna2Panel)flowLayoutPanel1.Controls[parameterName + i]);

                    List<String> dateValues = new List<String>();
                    List<String> titleValues = new List<String>();

                    String getUpDate = $"SELECT UPLOAD_DATE FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";
                    using (MySqlCommand command = new MySqlCommand(getUpDate, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", $"%{guna2TextBox5.Text}%");
                        using (MySqlDataReader readerDate = command.ExecuteReader()) {
                            while (readerDate.Read()) {
                                dateValues.Add(readerDate.GetString(0));
                            }
                        }
                    }

                    Label dateLab = new Label();
                    panelF.Controls.Add(dateLab);
                    dateLab.Name = "LabG" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                    dateLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                    dateLab.ForeColor = Color.DarkGray;
                    dateLab.Visible = true;
                    dateLab.Enabled = true;
                    dateLab.Location = new Point(12, 208);
                    dateLab.Text = dateValues[i];

                    String getTitleQue = $"SELECT CUST_FILE_PATH FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";
                    using (MySqlCommand command = new MySqlCommand(getTitleQue, con)) {
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", $"%{guna2TextBox5.Text}%");
                        using (MySqlDataReader titleReader = command.ExecuteReader()) {
                            while (titleReader.Read()) {
                                titleValues.Add(EncryptionModel.Decrypt(titleReader.GetString(0), EncryptionKey.KeyValue));
                            }
                        }
                    }

                    Label titleLab = new Label();
                    panelF.Controls.Add(titleLab);
                    titleLab.Name = "titleImgL" + i;//Segoe UI Semibold, 11.25pt, style=Bold
                    titleLab.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                    titleLab.ForeColor = Color.Gainsboro;
                    titleLab.Visible = true;
                    titleLab.Enabled = true;
                    titleLab.Location = new Point(12, 182);
                    titleLab.Width = 220;
                    titleLab.Height = 30;
                    titleLab.Text = titleValues[i];

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
                    remBut.FillColor = ColorTranslator.FromHtml("#4713BF");
                    remBut.BorderRadius = 6;
                    remBut.BorderThickness = 1;
                    remBut.BorderColor = ColorTranslator.FromHtml("#232323");
                    remBut.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;//Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                    remBut.Visible = true;
                    remBut.Location = new Point(189, 218);

                    remBut.Click += (sender_im, e_im) => {
                        var titleFile = titleLab.Text;
                        DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' File?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (verifyDialog == DialogResult.Yes) {
                            String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                            command = new MySqlCommand(noSafeUpdate, con);
                            command.ExecuteNonQuery();

                            String removeQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";
                            command = new MySqlCommand(removeQuery, con);
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@filename", titleFile);
                            command.ExecuteNonQuery();

                            panelPic_Q.Dispose();
                            if (flowLayoutPanel1.Controls.Count == 0) {
                                label8.Visible = true;
                                guna2Button6.Visible = true;
                            }
                            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                        }
                    };

                    guna2Button6.Visible = false;
                    label8.Visible = false;
                    var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);

                    Application.DoEvents();

                    if (_tableName == "file_info") {

                        List<string> base64Encoded = new List<string>();
                        string retrieveImgQuery = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";

                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    base64Encoded.Add(readBase64.GetString(0));
                                }
                            }
                        }

                        if (base64Encoded.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                picMain_Q.Image = new Bitmap(toMs);
                            }
                        }

                        picMain_Q.Click += (sender, e) => {
                            var getImgName = (Guna2PictureBox)sender;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);

                            Form bgBlur = new Form();
                            using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info", "null", label5.Text)) {
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

                    if (_tableName == "file_info_word") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            wordFORM displayMsi = new wordFORM(titleLab.Text, "file_info_word", "null", label5.Text);
                            displayMsi.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_expand") {
                        var _extTypes = titleLab.Text.Substring(titleLab.Text.LastIndexOf('.')).TrimStart();
                        if (_extTypes == ".py") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_python_file_48;//Image.FromFile(@"C:\Users\USER\Downloads\icons8-python-file-48.png");
                        }
                        else if (_extTypes == ".txt") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_txt_48;//Image.FromFile(@"C:\users\USER\downloads\gallery\icons8-txt-48.png");
                        }
                        else if (_extTypes == ".html") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_html_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-html-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".css") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_css_filetype_48__1_;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-css-filetype-48 (1).png");
                        }
                        else if (_extTypes == ".js") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_javascript_50;
                        }
                        else if (_extTypes == ".sql") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_database_50__1_;
                        }
                        else if (_extTypes == ".csv") {
                            img.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                        }

                        picMain_Q.Click += (sender_t, e_t) => {

                            if (_extTypes == ".csv" || _extTypes == ".sql") {
                                Thread _showRetrievalCsvAlert = new Thread(() => new SheetRetrieval().ShowDialog());
                                _showRetrievalCsvAlert.Start();
                            }

                            txtFORM displayPic = new txtFORM("IGNORETHIS", "file_info_expand", titleLab.Text, "null", label5.Text);
                            displayPic.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_exe") {
                        img.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icons8-exe-48.png");
                        picMain_Q.Click += (sender_ex, e_ex) => {
                            Form bgBlur = new Form();
                            exeFORM displayExe = new exeFORM(titleLab.Text, "file_info_exe", "null", label5.Text);
                            displayExe.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_vid") {

                        List<string> base64Encoded = new List<string>();
                        string retrieveImgQuery = $"SELECT CUST_THUMB FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";

                        using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                            using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await readBase64.ReadAsync()) {
                                    base64Encoded.Add(readBase64.GetString(0));
                                }
                            }
                        }

                        if (base64Encoded.Count > i) {
                            byte[] getBytes = Convert.FromBase64String(base64Encoded[i]);
                            using (MemoryStream toMs = new MemoryStream(getBytes)) {
                                picMain_Q.Image = new Bitmap(toMs);
                            }
                        }

                        picMain_Q.Click += (sender_vq, e_vq) => {
                            var getImgName = (Guna2PictureBox)sender_vq;
                            var getWidth = getImgName.Image.Width;
                            var getHeight = getImgName.Image.Height;
                            Bitmap defaultImage = new Bitmap(getImgName.Image);
                            vidFORM vidFormShow = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "file_info_vid", "null", label5.Text);
                            vidFormShow.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_excel") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.excelIcon;
                        picMain_Q.Click += (sender_vq, e_vq) => {
                            exlFORM exlForm = new exlFORM(titleLab.Text, "file_info_excel", "null", label5.Text);
                            exlForm.Show();
                        };
                    }
                    if (_tableName == "file_info_audi") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                        picMain_Q.Click += (sender_Aud, e_Aud) => {
                            Form bgBlur = new Form();
                            audFORM displayPic = new audFORM(titleLab.Text, "file_info_audi", "null", label5.Text);
                            displayPic.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_gif") {
                        List<String> _base64Encoded = new List<string>();
                        String retrieveImg = "SELECT CUST_FILE FROM  " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";
                        command = new MySqlCommand(retrieveImg, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

                        MySqlDataReader _readBase64 = command.ExecuteReader();
                        while (_readBase64.Read()) {
                            _base64Encoded.Add(_readBase64.GetString(0));
                        }
                        _readBase64.Close();

                        var _getBytes = Convert.FromBase64String(_base64Encoded[i]);
                        MemoryStream _toMs = new MemoryStream(_getBytes);

                        img.WaitOnLoad = false;
                        img.Image = new Bitmap(_toMs);

                        picMain_Q.Click += (sender_gi, ex_gi) => {
                            Form bgBlur = new Form();
                            using (gifFORM displayPic = new gifFORM(titleLab.Text, "file_info_gif", "null", label5.Text)) {
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

                    if (_tableName == "file_info_apk") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;//Image.FromFile(@"C:\USERS\USER\Downloads\icons8-android-os-50.png");
                        picMain_Q.Click += (sender_ap, ex_ap) => {
                            Form bgBlur = new Form();
                            apkFORM displayPic = new apkFORM(titleLab.Text, label5.Text, "file_info_apk", "null");
                            displayPic.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_pdf") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                        picMain_Q.Click += (sender_pd, e_pd) => {
                            Form bgBlur = new Form();
                            pdfFORM displayPdf = new pdfFORM(titleLab.Text, "file_info_pdf", "null", label5.Text);
                            displayPdf.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_ptx") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            ptxFORM displayPtx = new ptxFORM(titleLab.Text, "file_info_ptx", "null", label5.Text);
                            displayPtx.Show();
                        };
                        clearRedundane();
                    }

                    if (_tableName == "file_info_msi") {
                        picMain_Q.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                        picMain_Q.Click += (sender_pt, e_pt) => {
                            Form bgBlur = new Form();
                            msiFORM displayMsi = new msiFORM(titleLab.Text, "file_info_msi", "null",label5.Text);
                            displayMsi.Show();
                        };
                        clearRedundane();
                    }
                }

                label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                clearRedundane();

               // Control _removeExtraPanel = flowLayoutPanel1.Controls[-1];
               // flowLayoutPanel1.Controls.Remove(_removeExtraPanel);

                if(label4.Text == "0") {
                    showRedundane();
                }

                Application.DoEvents();

            } catch (Exception) {
                // @ ignore index exception
            }
        }

        /// <summary>
        /// Start generating panel for Shared To Others file
        /// </summary>
        /// <param name="_extTypes"></param>
        /// <param name="parameterName"></param>
        /// <param name="itemCurr"></param>
        private async void generateUserSharedOthersSearching(List<String> _extTypes, String parameterName, int itemCurr) {

            var form1 = Form1.instance;
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

                List<String> filesNames = new List<String>();
                List<String> fileDates = new List<String>();

                String _selectFiles = $"SELECT DISTINCT CUST_FILE_PATH, UPLOAD_DATE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";
                using (MySqlCommand command = new MySqlCommand(_selectFiles, con)) {
                    command.Parameters.AddWithValue("@username", form1.label5.Text);
                    command.Parameters.AddWithValue("@filename", $"%{guna2TextBox5.Text}%");
                    using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            filesNames.Add(reader.GetString(0));
                            fileDates.Add(reader.GetString(1));
                        }
                    }
                }

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

                getUploaderNameShared = titleLab.Text;
                var setupSharedUsername = sharedToName();
                var SharedToName = getUploaderNameShared;

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
                    DialogResult verifyDialog = MessageBox.Show("'" + titleFile + "' Delete this shared file?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        String noSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
                        command = new MySqlCommand(noSafeUpdate, con);
                        command.ExecuteNonQuery();

                        //string _receiverName = sharedToName();

                        String removeQuery = "DELETE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                        command = new MySqlCommand(removeQuery, con);
                        command.Parameters.AddWithValue("@username", label5.Text);
                        command.Parameters.AddWithValue("@filename", titleFile);
                        command.Parameters.AddWithValue("@sharedname", setupSharedUsername);

                        if (command.ExecuteNonQuery() == 1) {
                            // @ ignore success file deletion
                        }
                        else {
                            MessageBox.Show("Failed to delete this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        mainPanelTxt.Dispose();
                        if (flowLayoutPanel1.Controls.Count == 0) {
                            label5.Visible = true;
                        }
                        label4.Text = flowLayoutPanel1.Controls.Count.ToString();
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
                dateLabTxt.Text = fileDates[q];

                textboxPic.MouseHover += (_senderM, _ev) => {
                    panelTxt.ShadowDecoration.Enabled = true;
                    panelTxt.ShadowDecoration.BorderRadius = 8;
                };

                textboxPic.MouseLeave += (_senderQ, _evQ) => {
                    panelTxt.ShadowDecoration.Enabled = false;
                };

                if (typeValues[q] == ".png" || typeValues[q] == ".jpeg" || typeValues[q] == ".jpg" || typeValues[q] == ".bmp") {

                    List<string> base64Encoded = new List<string>();
                    string retrieveImgQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";

                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                        using (MySqlDataReader readBase64 = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > q) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (picFORM displayPic = new picFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName())) {
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
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_powerpoint_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        ptxFORM displayPtx = new ptxFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".pdf") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_pdf_60__1_;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        pdfFORM displayPtx = new pdfFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayPtx.Show();
                    };
                }

                if (typeValues[q] == ".apk") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_android_os_50;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        apkFORM displayApk = new apkFORM(titleLab.Text, "Shared To " + sharedToName(), "cust_sharing", label1.Text, true);
                        displayApk.Show();
                    };
                }

                if (typeValues[q] == ".msi") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_software_installer_32;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        msiFORM displayMsi = new msiFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayMsi.Show();
                    };
                }

                if (typeValues[q] == ".docx" || typeValues[q] == ".doc") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_microsoft_word_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        wordFORM displayDoc = new wordFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayDoc.Show();
                    };
                }

                if (typeValues[q] == ".xlsx" || typeValues[q] == ".xls") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.excelIcon;
                    textboxPic.Click += (sender_im, e_im) => {
                        exlFORM displayXls = new exlFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayXls.Show();
                    };
                }

                if (typeValues[q] == ".wav" || typeValues[q] == ".mp3") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_audio_file_60;
                    textboxPic.Click += (sender_im, e_im) => {
                        audFORM displayAud = new audFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".mp4" || typeValues[q] == ".mov" || typeValues[q] == ".webm" || typeValues[q] == ".avi" || typeValues[q] == ".wmv") {

                    List<string> base64Encoded = new List<string>();
                    string retrieveImgQuery = "SELECT CUST_THUMB FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";

                    using (MySqlCommand command = new MySqlCommand(retrieveImgQuery, con)) {
                        command.Parameters.AddWithValue("@username", form1.label5.Text);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");
                        using (MySqlDataReader readBase64 = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                            while (await readBase64.ReadAsync()) {
                                base64Encoded.Add(readBase64.GetString(0));
                            }
                        }
                    }

                    if (base64Encoded.Count > q) {
                        byte[] getBytes = Convert.FromBase64String(base64Encoded[q]);
                        using (MemoryStream toMs = new MemoryStream(getBytes)) {
                            textboxPic.Image = new Bitmap(toMs);
                        }
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        vidFORM displayAud = new vidFORM(defaultImage, getWidth, getHeight, titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayAud.Show();
                    };
                }

                if (typeValues[q] == ".exe") {
                    textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_exe_48;
                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);
    
                        Form bgBlur = new Form();
                        exeFORM displayExe = new exeFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true);
                        displayExe.Show();
                    };
                }

                if (typeValues[q] == ".txt" || typeValues[q] == ".html" || typeValues[q] == ".xml" || typeValues[q] == ".py" || typeValues[q] == ".css" || typeValues[q] == ".js" || typeValues[q] == ".sql" || typeValues[q] == ".csv") {
                    List<String> _contValues = new List<String>();
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);
                    command.Parameters.AddWithValue("@filename", "%" + guna2Button5.Text + "%");

                    MySqlDataReader _ReadConts = command.ExecuteReader();
                    if (_ReadConts.Read()) {
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
                    else if (typeValues[q] == ".csv") {
                        textboxPic.Image = FlowSERVER1.Properties.Resources.icons8_csv_48;
                    }

                    textboxPic.Click += (sender_im, e_im) => {
                        var getImgName = (Guna2PictureBox)sender_im;
                        var getWidth = getImgName.Image.Width;
                        var getHeight = getImgName.Image.Height;
                        Bitmap defaultImage = new Bitmap(getImgName.Image);

                        Form bgBlur = new Form();
                        using (txtFORM displayTxt = new txtFORM(_contValues[0], "cust_sharing", titleLab.Text, label1.Text, "Shared To " + sharedToName())) {
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
                    String retrieveImg = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";
                    command = new MySqlCommand(retrieveImg, con);
                    command.Parameters.AddWithValue("@username", form1.label5.Text);                    
                    command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

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
                        using (gifFORM displayGif = new gifFORM(titleLab.Text, "cust_sharing", label1.Text, "Shared To " + sharedToName(), true)) {
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

                Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                    .ToList()
                    .ForEach(form => form.Close());

                label4.Text = flowLayoutPanel1.Controls.Count.ToString();
                clearRedundane();

                Control _removeExtraPanel = flowLayoutPanel1.Controls[-1];
                flowLayoutPanel1.Controls.Remove(_removeExtraPanel);

                if (label4.Text == "0") {
                    showRedundane();
                }

                Application.DoEvents();
            }
        }

        private async void callGeneratorSharedSearch() {
            String getFilesTypeOthers = $"SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
            using (MySqlCommand command = new MySqlCommand(getFilesTypeOthers, con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        _TypeValuesOthers.Add(reader.GetString(0));
                    }
                }
            }

            await generateUserSharedOthers(_TypeValuesOthers, "DIRPAR", _TypeValuesOthers.Count);
            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private int _countRowSearch(String _tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(CUST_USERNAME) FROM {_tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH LIKE @filename";
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@filename", $"%{guna2TextBox5.Text}%");
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private async void callGeneratorSearch() {

            string[] tableNames = { "file_info", "file_info_expand", "file_info_exe", "file_info_vid", "file_info_excel", "file_info_msi", "file_info_audi", "file_info_apk", "file_info_pdf", "file_info_word", "file_info_ptx", "file_info_gif", "file_info_directory" };

            foreach (string tableName in tableNames) {
                if (await _countRow(tableName) > 0) {
                    switch (tableName) {
                        case "file_info":
                            await _generateUserFiles(tableName, "imgFile", await _countRow(tableName));
                            break;
                        case "file_info_expand":
                            await _generateUserFiles(tableName, "txtFile", await _countRow(tableName));
                            break;
                        case "file_info_exe":
                            await _generateUserFiles(tableName, "exeFile", await _countRow(tableName));
                            break;
                        case "file_info_vid":
                            await _generateUserFiles(tableName, "vidFile", await _countRow(tableName));
                            break;
                        case "file_info_excel":
                            await _generateUserFiles(tableName, "exlFile", await _countRow(tableName));
                            break;
                        case "file_info_pdf":
                            await _generateUserFiles(tableName, "pdfFile", await _countRow(tableName));
                            break;
                        case "file_info_apk":
                            await _generateUserFiles(tableName, "apkFile", await _countRow(tableName));
                            break;
                        case "file_info_word":
                            await _generateUserFiles(tableName, "wordFile", await _countRow(tableName));
                            break;
                        case "file_info_ptx":
                            await _generateUserFiles(tableName, "ptxFile", await _countRow(tableName));
                            break;
                        case "file_info_gif":
                            await _generateUserFiles(tableName, "gifFile", await _countRow(tableName));
                            break;
                        case "file_info_directory":
                            await _generateUserDirectory(label5.Text, "dirPar", await _countRow(tableName));
                            break;

                        default:
                            break;
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

            label4.Text = flowLayoutPanel1.Controls.Count.ToString();

            Application.OpenForms
                    .OfType<Form>()
                    .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                    .ToList()
                    .ForEach(form => form.Close());
        }

        /// <summary>
        /// 
        /// Retrieve original files of folder
        /// 
        /// </summary>
        
        private async void callGeneratorFolder() {

            _TypeValuesOthers.Clear();
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.WrapContents = true;               

            clearRedundane();

            String _selectedFolder = listBox1.GetItemText(listBox1.SelectedItem);

            List<String> typesValues = new List<String>();
            String getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername";

            using (MySqlCommand command = new MySqlCommand(getFileType, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@foldername", _selectedFolder);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        typesValues.Add(reader.GetString(0));
                    }
                }
            }

            await _generateUserFold(typesValues, _selectedFolder, "QWERTY", typesValues.Count);

        }



     private async void backgroundWorker1_DoWork_3(object sender, DoWorkEventArgs e)
        {
            await Task.Run(async () =>
            {
                using (var command = con.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {tableName} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE) VALUES (@CUST_FILE_PATH, @CUST_USERNAME, @UPLOAD_DATE, @CUST_FILE)";
                    command.Parameters.AddWithValue("@CUST_FILE_PATH", EncryptionModel.Encrypt(getName, EncryptionKey.KeyValue));
                    command.Parameters.AddWithValue("@CUST_USERNAME", label5.Text);
                    command.Parameters.AddWithValue("@UPLOAD_DATE", varDate);
                    command.Parameters.AddWithValue("@CUST_FILE", keyValMain);
                    command.CommandTimeout = 15000;

                    await command.ExecuteNonQueryAsync();

                    var form = Application.OpenForms.OfType<Form>().FirstOrDefault(f => f.Name == "UploadAlrt");
                    form?.Close();
                    
                }
            });
        }

        private void backgroundWorker1_ProgressChanged_3(object sender, ProgressChangedEventArgs e) {
        }

        private void backgroundWorker1_RunWorkerCompleted_3(object sender, RunWorkerCompletedEventArgs e) {

        }


        /// ------------- TESTING ---------------

        /// <summary>
        /// Start encrypting connection 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button1_Click_1(object sender, EventArgs e) {
            richTextBox1.Text = EncryptConnection("0afe74-gksuwpe8r", richTextBox1.Text);
        }

        public static string EncryptConnection(string key, string plainInput) {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream)) {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private void pictureBox4_Click(object sender, EventArgs e) {

        }

        private void pictureBox6_Click(object sender, EventArgs e) {

        }

        

        /// <summary>
        /// 
        /// Refresh Shared To Me panel
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="typeValues"></param>
        /// <param name="dirName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task RefreshGenerateUserShared(List<string> typeValues, string dirName, int count) {
            if (typeValues.Count == 0) {
                using (MySqlCommand command = con.CreateCommand()) {
                    command.CommandText = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_TO = @username";
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                    using (MySqlDataReader _readType = command.ExecuteReader()) {
                        while (_readType.Read()) {
                            typeValues.Add(_readType.GetString(0));
                        }
                    }
                }
            }

            _callFilesInformationShared();

            await generateUserShared(typeValues, dirName, typeValues.Count);
        }

        /// <summary>
        /// 
        /// Refresh Shared To Others panel
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="typeValuesOthers"></param>
        /// <param name="dirName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task RefreshGenerateUserSharedOthers(List<string> typeValuesOthers, string dirName, int count) {

            if (typeValuesOthers.Count == 0) {
                using (MySqlCommand command = con.CreateCommand()) {
                    command.CommandText = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username";
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                    using (MySqlDataReader _readType = command.ExecuteReader()) {
                        while (_readType.Read()) {
                            typeValuesOthers.Add(_readType.GetString(0));
                        }
                    }
                }
            }

            _callFilesInformationOthers();

            await generateUserSharedOthers(typeValuesOthers, dirName, typeValuesOthers.Count);
        }

        /// <summary>
        /// Refresh Shared To Me/Shared To Others panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void guna2Button4_Click(object sender, EventArgs e) {

            int selectedIndex = listBox1.SelectedIndex;
            string username = Form1.instance.label5.Text;

            _TypeValues.Clear();
            _TypeValuesOthers.Clear();
            flowLayoutPanel1.Controls.Clear();

            if (selectedIndex == 1) {
                await RefreshGenerateUserShared(_TypeValues, "DirParMe", _TypeValues.Count);
            }
            else if (selectedIndex == 2) {
                await RefreshGenerateUserSharedOthers(_TypeValuesOthers, "DirParOther", _TypeValuesOthers.Count);
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }

            label4.Text = flowLayoutPanel1.Controls.Count.ToString();

        }

        private IEnumerable<Control> GetAllControls(Control parent) {
            var controls = parent.Controls.Cast<Control>();
            return controls.SelectMany(c => GetAllControls(c)).Concat(controls);
        }

        /// <summary>
        /// 
        /// Detect for text input and search
        /// file based on the input
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        bool _isSearched = false;
        private async void guna2TextBox5_TextChanged(object sender, EventArgs e) {

            String _selectedFolderSearch = listBox1.GetItemText(listBox1.SelectedItem);
            flowLayoutPanel1.Controls.Clear();

            try {

                int _countCharsLength = guna2TextBox5.Text.Length;

                if (_countCharsLength == 0) {

                    Thread _showRetrievalForm = new Thread(() => new LoadAlertFORM().ShowDialog());
                    _showRetrievalForm.Start();

                    if (label26.Text == "Home") {
                        callGeneratorSearch();
                    } else if (label26.Text != "Shared Files" && label26.Text != "Home" && label26.Text != "Shared To Me") {
                        callGeneratorFolder();
                    } else if (listBox1.SelectedIndex == 2) {
                        _TypeValuesOthers.Clear();
                        callGeneratorSharedSearch();
                    } else if (listBox1.SelectedIndex == 1) {
                        //
                    }

                } else {
                    
                    if(label26.Text == "Home") {

                        if (_countRowSearch("file_info") > 0) {
                            generateSearching("file_info", "imgFile", _countRowSearch("file_info"));
                        }
                        if (_countRowSearch("file_info_word") > 0) {
                            generateSearching("file_info_word", "docFile", _countRowSearch("file_info_word"));
                        }
                        if (_countRowSearch("file_info_expand") > 0) {
                            generateSearching("file_info_expand", "txtFile", _countRowSearch("file_info_expand"));
                        }
                        if (_countRowSearch("file_info_ptx") > 0) {
                            generateSearching("file_info_ptx", "ptxFile", _countRowSearch("file_info_ptx"));
                        }
                        if (_countRowSearch("file_info_pdf") > 0) {
                            generateSearching("file_info_pdf", "pdfFile", _countRowSearch("file_info_pdf"));
                        }
                        if (_countRowSearch("file_info_excel") > 0) {
                            generateSearching("file_info_excel", "exlFile", _countRowSearch("file_info_excel"));
                        }
                        if (_countRowSearch("file_info_gif") > 0) {
                            generateSearching("file_info_gif", "gifFile", _countRowSearch("file_info_gif"));
                        }
                        if (_countRowSearch("file_info_exe") > 0) {
                            generateSearching("file_info_exe", "exeFile", _countRowSearch("file_info_exe"));
                        }
                        if (_countRowSearch("file_info_vid") > 0) {
                            generateSearching("file_info_vid", "vidFile", _countRowSearch("file_info_vid"));
                        }
                        if (_countRowSearch("file_info_msi") > 0) {
                            generateSearching("file_info_msi", "msiFile", _countRowSearch("file_info_msi"));
                        }
                        if (_countRowSearch("file_info_audi") > 0) {
                            generateSearching("file_info_audi", "audFile", _countRowSearch("file_info_audi"));
                        }
                        if (_countRowSearch("file_info_directory") > 0) {
                            await _generateUserDirectory("file_info_directory", "dirFile", _countRowSearch("file_info_directory"));
                        }

                    } else if(label26.Text != "Shared Files" && label26.Text != "Home" && label26.Text != "Shared To Me") {

                        List<string> typesValues = new List<string>();
                        string getFileType = "SELECT file_type FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH LIKE @filename";

                        using (MySqlCommand command = new MySqlCommand(getFileType, con)) {
                            command.Parameters.AddWithValue("@username", label5.Text);
                            command.Parameters.AddWithValue("@foldername", _selectedFolderSearch);
                            command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

                            using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                                while (await reader.ReadAsync()) {
                                    typesValues.Add(reader.GetString(0));
                                }
                            }
                        }

                        List<string> mainTypes = typesValues.Distinct().ToList();
                        int currMainLength = typesValues.Count;

                        folderSearching(typesValues, _selectedFolderSearch, "SearchPar", currMainLength);

                    } else if (listBox1.SelectedIndex == 2) {

                        _TypeValuesOthers.Clear();

                        guna2Button4.Visible = true;
                        guna2Button19.Visible = false;
                        flowLayoutPanel1.Controls.Clear();

                        Application.DoEvents();

                        String getFilesTypeOthers = "SELECT FILE_EXT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH LIKE @filename";
                        command = new MySqlCommand(getFilesTypeOthers, con);
                        command = con.CreateCommand();
                        command.CommandText = getFilesTypeOthers;
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", "%" + guna2TextBox5.Text + "%");

                        MySqlDataReader _readTypeOthers = command.ExecuteReader();
                        while (_readTypeOthers.Read()) {
                            _TypeValuesOthers.Add(_readTypeOthers.GetString(0));// Append ToAr;
                        }
                        _readTypeOthers.Close();

                        generateUserSharedOthersSearching(_TypeValuesOthers, "SearchParShare", _TypeValuesOthers.Count);

                        if (flowLayoutPanel1.Controls.Count == 0) {
                            showRedundane();
                        }
                        else {
                            clearRedundane();
                        }

                        Application.DoEvents();

                    } else if (listBox1.SelectedIndex == 1) {

                    }
                }
           } catch (Exception) {
                //showRedundane();
            }

            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
            if (label4.Text == "0") {
                showRedundane();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        async Task<int> _countRow(String _tableName) {
            using (var command = con.CreateCommand()) {
                command.CommandText = $"SELECT COUNT(CUST_USERNAME) FROM {_tableName} WHERE CUST_USERNAME = @username";
                command.Parameters.AddWithValue("@username", label5.Text);
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

        private async void guna2Button8_Click(object sender, EventArgs e) {

            guna2Button19.Visible = false;
            guna2Button4.Visible = false;
            flowLayoutPanel1.Controls.Clear();

            string[] tableNames = { "file_info", "file_info_expand", "file_info_exe", "file_info_vid", "file_info_excel","file_info_msi","file_info_audi","file_info_apk","file_info_pdf","file_info_word","file_info_ptx","file_info_gif","file_info_directory"};

            foreach (string tableName in tableNames) {
                if (await _countRow(tableName) > 0) {
                    switch (tableName) {
                        case "file_info":
                            await _generateUserFiles(tableName, "imgFile", await _countRow(tableName));
                            break;
                        case "file_info_expand":
                            await _generateUserFiles(tableName, "txtFile", await _countRow(tableName));
                            break;
                        case "file_info_exe":
                            await _generateUserFiles(tableName, "exeFile", await _countRow(tableName));
                            break;
                        case "file_info_vid":
                            await _generateUserFiles(tableName, "vidFile", await _countRow(tableName));
                            break;
                        case "file_info_excel":
                            await _generateUserFiles(tableName, "exlFile", await _countRow(tableName));
                            break;
                        case "file_info_pdf":
                            await _generateUserFiles(tableName, "pdfFile", await _countRow(tableName));
                            break;
                        case "file_info_apk":
                            await _generateUserFiles(tableName, "apkFile", await _countRow(tableName));
                            break;
                        case "file_info_word":
                            await _generateUserFiles(tableName, "wordFile", await _countRow(tableName));
                            break;
                        case "file_info_ptx":
                            await _generateUserFiles(tableName, "ptxFile", await _countRow(tableName));
                            break;
                        case "file_info_gif":
                            await _generateUserFiles(tableName, "gifFile", await _countRow(tableName));
                            break;
                        case "file_info_directory":
                            await _generateUserDirectory(label5.Text,"dirPar", await _countRow(tableName));
                            break;

                        default:
                            break;
                    }
                }
            }

            if (flowLayoutPanel1.Controls.Count == 0) {
                showRedundane();
            }
            else {
                clearRedundane();
            }


            label4.Text = flowLayoutPanel1.Controls.Count.ToString();
        }

        private async Task<string> getAccountTypeNumber() {
            string accountType = "";
            string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
                label6.Text = accountType;
            }

            if (accountType == "Basic") {
                label6.Text = "20";
            }
            else if (accountType == "Max") {
                label6.Text = "500";
            }
            else if (accountType == "Express") {
                label6.Text = "1000";
            }
            else if (accountType == "Supreme") {
                label6.Text = "2000";
            }

            return accountType;
        }

        private void guna2Button9_Click_1(object sender, EventArgs e) {
            guna2Button9.FillColor = Color.FromArgb(255,71, 19, 191);
            guna2Button13.FillColor = Color.Transparent;
            panel3.SendToBack();
            panel1.BringToFront();
            label9.Visible = false;
            listBox1.Visible = false;
            guna2Button15.Visible = true;
            guna2VSeparator1.BringToFront();
        }

        private void guna2Button13_Click(object sender, EventArgs e) {
            guna2Button13.FillColor = Color.FromArgb(255, 71, 19, 191);
            guna2Button9.FillColor = Color.Transparent;
            panel1.SendToBack();
            panel3.BringToFront();
            label9.Visible = true;
            listBox1.Visible = true;
            guna2Button15.Visible = false;
            guna2VSeparator1.BringToFront();
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {
        }

        private void guna2Button14_Click(object sender, EventArgs e) {
            var remAccShow = new remAccFORM(label5.Text, label24.Text);
            remAccShow.Show();
            remAccFORM.instance.guna2TabControl1.SelectedTab = remAccFORM.instance.guna2TabControl1.TabPages["tabPage3"];
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e) {

        }

        private void guna2Button15_Click(object sender, EventArgs e) {

            try {

                DialogResult _confirmation = MessageBox.Show("Logout your account?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (_confirmation == DialogResult.Yes) {

                    panel1.SendToBack();

                    Form1.instance.label2.Text = "Item Count";
                    Form1.instance.label10.Text = "Upload";
                    Form1.instance.guna2Button2.Text = "Upload File";
                    Form1.instance.guna2Button12.Text = "Upload Folder";
                    Form1.instance.guna2Button1.Text = "Create Directory";
                    Form1.instance.guna2Button7.Text = "File Sharing";
                    Form1.instance.guna2Button7.Size = new Size(125, 47);
                    Form1.instance.label28.Text = "Essentials";

                    String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                    String _getAuth = _getPath + "\\CUST_DATAS.txt";

                    if (File.Exists(_getAuth)) {
                        if (Directory.Exists(_getPath)) {
                            Directory.Delete(_getPath, true);
                        }
                    }

                    Form1.instance.guna2Panel7.Visible = true;
                    Form1.instance.listBox1.Items.Clear();
                }
            }
            catch (Exception) {
                MessageBox.Show("There's a problem while attempting to logout your account. Please try again.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}