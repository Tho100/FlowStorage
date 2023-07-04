using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FlowSERVER1.AlertForms;
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
                    return;
                }

                Globals.custUsername = username;

                using (var command = new MySqlCommand("SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username", con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                        if (await reader.ReadAsync()) {
                            Globals.custEmail = reader.GetString(0);
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

                accessHomePage.lstFoldersPage.Items.AddRange(itemsFolder.Concat(updatesTitle).ToArray());
                accessHomePage.lstFoldersPage.SelectedIndex = 0;
                accessHomePage.lblItemCountText.Text = accessHomePage.flwLayoutHome.Controls.Count.ToString();

                buildGreetingLabel();
                await getAccountTypeNumber();
                await getCurrentLang();

                if (int.TryParse(accessHomePage.lblItemCountText.Text, out int getCurrentCount) && int.TryParse(accessHomePage.lblLimitUploadText.Text, out int getLimitedValue)) {
                    int calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                    accessHomePage.lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";
                    accessHomePage.progressBarUsageStorage.Value = calculatePercentageUsage;
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

        /// <summary>
        /// 
        /// Hide and close current registration form and 
        /// display the HomePage
        /// 
        /// </summary>
        private void showHomePage() {
            Hide();
            accessHomePage.ShowDialog();
            accessHomePage.FormClosed += HomePage_HomePageClosed;
            Close();
        }

        /// <summary>
        /// 
        /// Verify if there's one more more forms is opened,
        /// if true then close them all and terminate the program on exit
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomePage_HomePageClosed(object sender, FormClosedEventArgs e) {
            if (Application.OpenForms.Count >= 1) {
                Application.Exit();
            }
        }

        private async Task<string> getAccountTypeNumber() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
                accessHomePage.lblLimitUploadText.Text = accountType;
            }

            Globals.accountType = accountType;
            accessHomePage.lblLimitUploadText.Text = Globals.uploadFileLimit[Globals.accountType].ToString();
            
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
                Form_1.lblUpload.Text = "Muat-Naik";
                Form_1.btnUploadFile.Text = "Muat-Naik Fail";
                Form_1.btnUploadFolder.Text = "Muat-Naik Folder";
                Form_1.btnCreateDirectory.Text = "Buat Direktori";
                Form_1.btnFileSharing.Text = "Perkongsian Fail";
                Form_1.btnFileSharing.Size = new Size(159, 47);
                Form_1.lblEssentials.Text = "Kepentingan";
            }

            if (_custLang == "US") {

                Form_1.label2.Text = "Item Count";
                Form_1.lblUpload.Text = "Upload";
                Form_1.btnUploadFile.Text = "Upload File";
                Form_1.btnUploadFolder.Text = "Upload Folder";
                Form_1.btnCreateDirectory.Text = "Create Directory";
                Form_1.btnFileSharing.Text = "File Sharing";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "GER") {
                Form_1.lblUpload.Text = "Hochladen";
                Form_1.label2.Text = "Stückzahl";
                Form_1.btnUploadFile.Text = "Datei hochladen";
                Form_1.btnUploadFolder.Text = "Ordner hochladen";
                Form_1.btnCreateDirectory.Text = "Verzeichnis erstellen";
                Form_1.btnFileSharing.Text = "Datenaustausch";
                Form_1.btnFileSharing.Size = new Size(159, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "JAP") {
                Form_1.lblUpload.Text = "アップロード";
                Form_1.label2.Text = "アイテム数";
                Form_1.btnUploadFile.Text = "ファイルをアップロードする";
                Form_1.btnUploadFolder.Text = "フォルダのアップロード";
                Form_1.btnCreateDirectory.Text = "ディレクトリの作成";
                Form_1.btnFileSharing.Text = "ファイル共有";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "必需品";
            }

            if (_custLang == "ESP") {
                Form_1.lblUpload.Text = "Subir";
                Form_1.label2.Text = "Recuento de elementos";
                Form_1.btnUploadFile.Text = "Subir archivo";
                Form_1.btnUploadFolder.Text = "Cargar carpeta";
                Form_1.btnCreateDirectory.Text = "Crear directorio";
                Form_1.btnFileSharing.Text = "Compartición de archivos";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Esenciales";
            }

            if (_custLang == "FRE") {
                Form_1.lblUpload.Text = "Télécharger";
                Form_1.label2.Text = "Nombre d'éléments";
                Form_1.btnUploadFile.Text = "Téléverser un fichier";
                Form_1.btnUploadFolder.Text = "Télécharger le dossier";
                Form_1.btnCreateDirectory.Text = "Créer le répertoire";
                Form_1.btnFileSharing.Text = "Partage de fichiers";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentiel";
            }

            if (_custLang == "POR") {
                Form_1.lblUpload.Text = "Carregar";
                Form_1.label2.Text = "Contagem de itens";
                Form_1.btnUploadFile.Text = "Subir arquivo";
                Form_1.btnUploadFolder.Text = "Carregar Pasta";
                Form_1.btnCreateDirectory.Text = "Criar diretório";
                Form_1.btnFileSharing.Text = "Compartilhamento de arquivos";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essenciais";
            }

            if (_custLang == "CHI") {
                Form_1.lblUpload.Text = "上传";
                Form_1.label2.Text = "物品数量";
                Form_1.btnUploadFile.Text = "上传文件";
                Form_1.btnUploadFolder.Text = "上传文件夹";
                Form_1.btnCreateDirectory.Text = "创建目录";
                Form_1.btnFileSharing.Text = "文件共享";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "要点";
            }

            if (_custLang == "RUS") {
                Form_1.lblUpload.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.btnUploadFile.Text = "Загрузить файл";
                Form_1.btnUploadFolder.Text = "Загрузить папку";
                Form_1.btnCreateDirectory.Text = "Создать каталог";
                Form_1.btnFileSharing.Text = "Общий доступ к файлам";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Основные";
            }

            if (_custLang == "DUT") {
                Form_1.lblUpload.Text = "Uploaden";
                Form_1.label2.Text = "Aantal artikelen";
                Form_1.btnUploadFile.Text = "Bestand uploaden";
                Form_1.btnUploadFolder.Text = "Map uploaden";
                Form_1.btnCreateDirectory.Text = "Directory aanmaken";
                Form_1.btnFileSharing.Text = "Bestanden delen";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
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

                Control flowlayout = accessHomePage.flwLayoutHome;

                string _getUser = txtBoxUsernameField.Text;
                string _getEmail = txtBoxEmailField.Text;
                string _getPass = txtBoxAuth0Field.Text;
                string _getPin = txtBoxAuth1Field.Text;

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

                    if (!(_getPin.All(char.IsDigit))) {
                        label30.Visible = true;
                        label30.Text = "PIN must be a number.";
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
                        HomePage.instance.lblEmptyHere.Visible = true;
                        HomePage.instance.btnGarbageImage.Visible = true;
                    }

                    Globals.custUsername = _getUser;
                    Globals.custEmail = _getEmail;
                    Globals.accountType = "Basic";

                    insertRegistrationData(_getUser,_getEmail,_getPass,_getPin);

                    await getCurrentLang();
                    clearRegistrationValue();
                    setupAutoLogin(_getUser);
                    buildGreetingLabel();

                    accessHomePage.lblUsagePercentage.Text = "0%";
                    accessHomePage.progressBarUsageStorage.Value = 0;

                    string[] itemsFolder = { "Home", "Shared To Me", "Shared Files" };
                    accessHomePage.lstFoldersPage.Items.AddRange(itemsFolder);
                    accessHomePage.lstFoldersPage.SelectedIndex = 0;

                    showHomePage();
                }
            }
            catch (Exception) {
                new CustomAlert(title: "Failed to register your account",subheader: "Are you connected to the internet?").Show();
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

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = $"Good Night {Globals.custUsername}";

            if (hours >= 1 && hours <= 12) {
                greeting = "Good Morning, " + Globals.custUsername;
            } else if (hours >= 12 && hours <= 16) {
                greeting = "Good Afternoon, " + Globals.custUsername;
            } else if (hours >= 16 && hours <= 21) {

                if (hours == 20 || hours == 21) {
                    greeting = "Good Late Evening, " + Globals.custUsername;
                }

            } else if (hours >= 21 && hours <= 24) {
                greeting = "Good Night, " + Globals.custUsername;
            }

            accessHomePage.lblGreetingText.Text = greeting;

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
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private async Task getCurrentLang() {
            const string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (var command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
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

            accessHomePage.lblLimitUploadText.Text = "25";

            txtBoxUsernameField.Text = String.Empty;
            txtBoxAuth0Field.Text = String.Empty;
            txtBoxEmailField.Text = String.Empty;
            txtBoxAuth1Field.Text = String.Empty;
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            SignInForm login_page = new SignInForm(this);
            login_page.Show();
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void txtBoxAuth1Field_TextChanged(object sender, EventArgs e) {

        }
    }
}
