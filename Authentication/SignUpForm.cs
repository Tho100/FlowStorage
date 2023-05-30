using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.Authentication {
    public partial class SignUpForm : Form {

        private readonly MySqlConnection con = ConnectionModel.con;
        private readonly HomePage accessHomePage = new HomePage();
        
        private HomePage homePage;

        public SignUpForm() {
            InitializeComponent();
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

                string username = EncryptionModel.Decrypt(File.ReadLines(authFile).First());

                if (accountStartupValidation(username) == String.Empty) {
                    guna2Panel7.Visible = true;
                    accessHomePage.label5.Text = "Guest";
                    return;
                }

                accessHomePage.label5.Text = username;

                using (var command = new MySqlCommand("SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username", con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        if (await reader.ReadAsync()) {
                            accessHomePage.label24.Text = reader.GetString(0);
                        }
                    }
                }

                var itemsFolder = new[] { "Home", "Shared To Me", "Shared Files" };
                var updatesTitle = new List<string>();

                using (var command = new MySqlCommand("SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username", ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            updatesTitle.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                        }
                    }
                }

                accessHomePage.listBox1.Items.AddRange(itemsFolder.Concat(updatesTitle).ToArray());
                accessHomePage.listBox1.SelectedIndex = 0;
                accessHomePage.label4.Text = accessHomePage.flowLayoutPanel1.Controls.Count.ToString();


                buildGreetingLabel();
                await getAccountTypeNumber();
                await getCurrentLang();

                if (int.TryParse(accessHomePage.label4.Text, out int getCurrentCount) && int.TryParse(accessHomePage.label6.Text, out int getLimitedValue)) {
                    int calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                    accessHomePage.label20.Text = calculatePercentageUsage.ToString() + "%";
                    accessHomePage.guna2ProgressBar1.Value = calculatePercentageUsage;
                }

                guna2Panel7.Visible = false;

                buildUILanguage(accessHomePage.CurrentLang);
                showHomePage();

            } catch (Exception FlowstorageDirNotFound) {
                // TODO: Ignore
            }
            finally {

                try {

                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                    new DirectoryInfo(path).Attributes |= (FileAttributes.Directory | FileAttributes.Hidden);
                    Application.OpenForms.OfType<Form>().Where(form => form.Name == "LoadAlertFORM").ToList().ForEach(form => form.Close());

                }
                catch (Exception FlowStorageDirNotFound) {
                    // TODO: Ignore
                }

            }
        }

        private void showHomePage() {
            Hide();
            accessHomePage.ShowDialog();
            accessHomePage.FormClosed += HomePage_HomePageClosed;
            Close();
        }

        private void HomePage_HomePageClosed(object sender, FormClosedEventArgs e) {
            if (Application.OpenForms.Count == 1) {
                Application.Exit();
            }
        }


        private async Task<string> getAccountTypeNumber() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", accessHomePage.label5.Text);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
                accessHomePage.label6.Text = accountType;
            }

            accessHomePage.accountTypeString = accountType;

            if (accountType == "Basic") {
                accessHomePage.label6.Text = "20";
            }
            else if (accountType == "Max") {
                accessHomePage.label6.Text = "500";
            }
            else if (accountType == "Express") {
                accessHomePage.label6.Text = "1000";
            }
            else if (accountType == "Supreme") {
                accessHomePage.label6.Text = "2000";
            }

            return accountType;
        }

        /// <summary>
        /// Validate user entered email address format
        /// </summary>
        /// <param name="_custEmail"></param>
        /// <returns></returns>
        private bool validateEmailUser(String _custEmail) {
            const string _regPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(_regPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(_custEmail);
        }

        /// <summary>
        /// Generate random string within range
        /// </summary>
        private string RandomString(int size, bool lowerCase = true) {

            Random _setRandom = new Random();

            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;

            for (var i = 0; i < size; i++) {
                var @char = (char)_setRandom.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        /// <summary>
        /// 
        /// Retrieve user username based on the parameter 'username'
        /// which is retrieved from the cust_datas.txt. 
        /// 
        /// Usage; If returns String.Empty then show sign up panel else load data
        ///        based on the username
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private string accountStartupValidation(string username) {

            const string getUsername = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getUsername, con)) {
                command.Parameters.AddWithValue("@username", username);
                string concludeUsername = (string)command.ExecuteScalar();
                return concludeUsername ?? String.Empty;
            }
        }


        /// <summary>
        /// Start genertating UI labels based on user language
        /// </summary>
        /// <param name="_custLang"></param>
        private void buildUILanguage(String _custLang) {

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

            if (_custLang == "RUS") {
                Form_1.label10.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.guna2Button2.Text = "Загрузить файл";
                Form_1.guna2Button12.Text = "Загрузить папку";
                Form_1.guna2Button1.Text = "Создать каталог";
                Form_1.guna2Button7.Text = "Общий доступ к файлам";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Основные";
            }

            if (_custLang == "DUT") {
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
        /// 
        /// Sign Up button pressed
        /// 
        /// Retrieve input fields then encrypt/hashed it's values
        /// and transfer them into database.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button11_Click(object sender, EventArgs e) {

            try {

                Control flowlayout = accessHomePage.flowLayoutPanel1;

                string _getUser = guna2TextBox1.Text;
                string _getPass = guna2TextBox2.Text;
                string _getEmail = guna2TextBox3.Text;
                String _getPin = guna2TextBox4.Text;

                List<string> existsInfosMail = new List<string>();
                List<string> existsInfosUser = new List<string>();

                using (MySqlCommand command = con.CreateCommand()) {
                    const string verifyQueryUser = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
                    command.CommandText = verifyQueryUser;
                    command.Parameters.AddWithValue("@username", _getUser);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            existsInfosUser.Add(reader.GetString(0));
                        }
                    }

                    const string verifyQueryMail = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
                    command.CommandText = verifyQueryMail;
                    command.Parameters.AddWithValue("@email", _getEmail);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            existsInfosMail.Add(reader.GetString(0));
                        }
                    }

                }

                if (existsInfosUser.Count >= 1 || existsInfosMail.Count >= 1) {
                    if (existsInfosUser.Count >= 1) {
                        label11.Visible = true;
                        label11.Text = "Username is taken.";
                    }
                    if (existsInfosMail.Count >= 1) {
                        label22.Visible = true;
                        label22.Text = "Email already exists.";
                    }
                }
                else {

                    label22.Visible = false;
                    label12.Visible = false;
                    label11.Visible = false;

                    if (_getUser.Contains("&") || _getUser.Contains(";") || _getUser.Contains("?") || _getUser.Contains("%")) {
                        label11.Visible = true;
                        label11.Text = "Special characters is not allowed.";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getPin)) {
                        label30.Visible = true;
                        label30.Text = "Please add a PIN number.";
                        return;
                    }

                    if (_getPin.Length != 3) {
                        label30.Visible = true;
                        label30.Text = "PIN Number must have 3 digits.";
                        return;
                    }

                    if (!validateEmailUser(_getEmail) == true) {
                        label22.Visible = true;
                        label22.Text = "Entered email is not valid.";
                        return;
                    }

                    if (_getUser.Length > 20) {
                        label11.Visible = true;
                        label11.Text = "Username character length limit is 20.";
                        return;
                    }

                    if (_getPass.Length < 5) {
                        label12.Visible = true;
                        label12.Text = "Password must be longer than 5 characters.";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getEmail)) {
                        label22.Visible = true;
                        label22.Text = "Please add your email";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getPass)) {
                        label12.Visible = true;
                        return;

                    }

                    if (String.IsNullOrEmpty(_getUser)) {
                        label11.Visible = true;
                        return;
                    }

                    flowlayout.Controls.Clear();

                    if (flowlayout.Controls.Count == 0) {
                        HomePage.instance.label8.Visible = true;
                        HomePage.instance.guna2Button6.Visible = true;
                    }

                    accessHomePage.label5.Text = _getUser;
                    accessHomePage.label24.Text = _getEmail;

                    insertRegistrationData(_getUser,_getEmail,_getPass,_getPin);

                    await getCurrentLang();
                    clearRegistrationValue();
                    setupAutoLogin(_getUser);
                    buildGreetingLabel();

                    accessHomePage.label20.Text = "0%";
                    accessHomePage.guna2ProgressBar1.Value = 0;

                    string[] itemsFolder = { "Home", "Shared To Me", "Shared Files" };
                    accessHomePage.listBox1.Items.AddRange(itemsFolder);
                    accessHomePage.listBox1.SelectedIndex = 0;

                    showHomePage();
                }
            }
            catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void insertRegistrationData(string _getUser, string _getEmail, string _getPass, string _getPin) {

            var _setupRecov = RandomString(16) + _getUser;
            var _removeSpacesRecov = new string(_setupRecov.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            var _setupTok = (RandomString(12) + _getUser).ToLower();
            var _removeSpacesTok = new string(_setupTok.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            string _getDate = DateTime.Now.ToString("MM/dd/yyyy");

            using (var transaction = con.BeginTransaction()) {

                try {

                    MySqlCommand command = con.CreateCommand();

                    command.CommandText = @"INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN,RECOV_TOK,ACCESS_TOK)
                            VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL,@CUST_PIN,@RECOV_TOK,@ACCESS_TOK)";
                    command.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                    command.Parameters.AddWithValue("@CUST_PASSWORD", EncryptionModel.computeAuthCase(_getPass));
                    command.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                    command.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                    command.Parameters.AddWithValue("@CUST_PIN", EncryptionModel.computeAuthCase(_getPin));
                    command.Parameters.AddWithValue("@RECOV_TOK", EncryptionModel.Encrypt(_removeSpacesRecov));
                    command.Parameters.AddWithValue("@ACCESS_TOK", EncryptionModel.computeAuthCase(_removeSpacesTok));

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
        }

        /// <summary>
        /// 
        /// Setup greeting label based on current time and translate 
        /// them into user default language (English on registration).
        /// 
        /// </summary>
        private void buildGreetingLabel() {

            var form = HomePage.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = $"Good Night {accessHomePage.label5.Text}";

            if (hours >= 1 && hours <= 12) {
                greeting = "Good Morning, " + lab5.Text;
            } else if (hours >= 12 && hours <= 16) {
                greeting = "Good Afternoon, " + lab5.Text;
            } else if (hours >= 16 && hours <= 21) {

                if (hours == 20 || hours == 21) {
                    greeting = "Good Late Evening, " + lab5.Text;
                }

            } else if (hours >= 21 && hours <= 24) {
                greeting = "Good Night, " + lab5.Text;
            }

            accessHomePage.label1.Text = greeting;

        }

        /// <summary>
        /// Create file and insert user username into that file in a sub folder 
        /// called FlowStorageInfos located in %appdata%
        /// </summary>
        /// <param name="_custUsername">Username of user</param>
        private void setupAutoLogin(String _custUsername) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private async Task getCurrentLang() {
            const string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", accessHomePage.label5.Text);
                using (MySqlDataReader readLang = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await readLang.ReadAsync()) {
                        accessHomePage.CurrentLang = readLang.GetString(0);
                    }
                }
            }
        }

        private void clearRegistrationValue() {

            label11.Visible = false;
            label12.Visible = false;
            label30.Visible = false;
            guna2Panel7.Visible = false;

            accessHomePage.label6.Text = "20";
            accessHomePage.accountTypeString = "Basic";

            guna2TextBox1.Text = String.Empty;
            guna2TextBox2.Text = String.Empty;
            guna2TextBox3.Text = String.Empty;
            guna2TextBox4.Text = String.Empty;
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            SignInForm login_page = new SignInForm(this);
            login_page.Show();
        }
    }
}
