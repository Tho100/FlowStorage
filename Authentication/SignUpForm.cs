
using FlowSERVER1.AlertForms;

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

                if (AccountStartupValidation(username) == String.Empty) {
                    pnlRegistration.Visible = true;
                    return;
                }

                Globals.custUsername = username;

                using (var command = new MySqlCommand("SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username", con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
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

                BuildGreetingLabel();
                await GetUserAccountType();
                await GetUserLanguage();

                int getCurrentCount = int.Parse(accessHomePage.lblItemCountText.Text);
                int getLimitedValue = int.Parse(accessHomePage.lblLimitUploadText.Text);
                int calculatePercentageUsage = (int)(((float)getCurrentCount / getLimitedValue) * 100);
                accessHomePage.lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                accessHomePage.progressBarUsageStorage.Value = calculatePercentageUsage;

                pnlRegistration.Visible = false;

                BuildUILanguage();
                ShowHomePage();

            }
            catch (Exception) {
                // TODO: Ignore
            }

            finally {

                try {

                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                    new DirectoryInfo(path).Attributes |= (FileAttributes.Directory | FileAttributes.Hidden);
                    Application.OpenForms.OfType<Form>().Where(form => form.Name == "LoadAlertFORM").ToList().ForEach(form => form.Close());

                }
                catch (Exception) {
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
        private void ShowHomePage() {
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

        private async Task<string> GetUserAccountType() {

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
        /// <param name="emailInput"></param>
        /// <returns></returns>
        private bool ValidateEmailInput(String emailInput) {
            const string _regPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(_regPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(emailInput);
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
        private string AccountStartupValidation(string username) {

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
        private void BuildUILanguage() {
            var Form_1 = HomePage.instance;
            Form_1.label2.Text = "Item Count";
            Form_1.lblUpload.Text = "Upload";
            Form_1.btnUploadFile.Text = "Upload File";
            Form_1.btnUploadFolder.Text = "Upload Folder";
            Form_1.btnCreateDirectory.Text = "Create Directory";
            Form_1.btnFileSharing.Text = "File Sharing";
            Form_1.btnFileSharing.Size = new Size(125, 47);
            Form_1.lblEssentials.Text = "Essentials";
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
        private async void btnSignUp_Click(object sender, EventArgs e) {

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
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Username is taken.";
                    }
                    if (existsInfosMail.Count >= 1) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Email already exists.";
                    }
                }
                else {

                    lblAlertEmail.Visible = false;
                    lblAlertPassword.Visible = false;
                    lblAlertUsername.Visible = false;

                    if (_getUser.Contains("&") || _getUser.Contains(";") || _getUser.Contains("?") || _getUser.Contains("%")) {
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Special characters is not allowed.";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getPin)) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "Please add a PIN number.";
                        return;
                    }

                    if (!(_getPin.All(char.IsDigit))) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "PIN must be a number.";
                        return;
                    }

                    if (_getPin.Length != 3) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "PIN Number must have 3 digits.";
                        return;
                    }

                    if (!ValidateEmailInput(_getEmail) == true) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Entered email is not valid.";
                        return;
                    }

                    if (_getUser.Length > 20) {
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Username character length limit is 20.";
                        return;
                    }

                    if (_getPass.Length < 5) {
                        lblAlertPassword.Visible = true;
                        lblAlertPassword.Text = "Password must be longer than 5 characters.";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getEmail)) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Please add your email";
                        return;
                    }

                    if (String.IsNullOrEmpty(_getPass)) {
                        lblAlertPassword.Visible = true;
                        return;

                    }

                    if (String.IsNullOrEmpty(_getUser)) {
                        lblAlertUsername.Visible = true;
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

                    InsertUserRegistrationData(_getUser, _getEmail, _getPass, _getPin);

                    await GetUserLanguage();
                    ClearRegistrationFields();
                    StupAutoLogin(_getUser);
                    BuildGreetingLabel();

                    accessHomePage.lblUsagePercentage.Text = "0%";
                    accessHomePage.progressBarUsageStorage.Value = 0;

                    string[] itemsFolder = { "Home", "Shared To Me", "Shared Files" };
                    accessHomePage.lstFoldersPage.Items.AddRange(itemsFolder);
                    accessHomePage.lstFoldersPage.SelectedIndex = 0;

                    ShowHomePage();
                }
            }
            catch (Exception) {
                new CustomAlert(title: "Failed to register your account", subheader: "Are you connected to the internet?").Show();
            }
        }

        private void InsertUserRegistrationData(string getUser, string getEmail, string getAuth, string getPin) {

            var _setupRecov = RandomString(16) + getUser;
            var _removeSpacesRecov = new string(_setupRecov.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            var _setupTok = (RandomString(12) + getUser).ToLower();
            var _removeSpacesTok = new string(_setupTok.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            string _getDate = DateTime.Now.ToString("MM/dd/yyyy");

            using (var transaction = con.BeginTransaction()) {

                try {

                    MySqlCommand command = con.CreateCommand();

                    command.CommandText = @"INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN,RECOV_TOK,ACCESS_TOK)
                            VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL,@CUST_PIN,@RECOV_TOK,@ACCESS_TOK)";
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@CUST_PASSWORD", EncryptionModel.computeAuthCase(getAuth));
                    command.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                    command.Parameters.AddWithValue("@CUST_EMAIL", getEmail);
                    command.Parameters.AddWithValue("@CUST_PIN", EncryptionModel.computeAuthCase(getPin));
                    command.Parameters.AddWithValue("@RECOV_TOK", EncryptionModel.Encrypt(_removeSpacesRecov));
                    command.Parameters.AddWithValue("@ACCESS_TOK", EncryptionModel.computeAuthCase(_removeSpacesTok));

                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE)
                            VALUES(@CUST_USERNAME,@CUST_EMAIL,@ACC_TYPE)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@CUST_EMAIL", getEmail);
                    command.Parameters.AddWithValue("@ACC_TYPE", "Basic");
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO lang_info(CUST_USERNAME,CUST_LANG)
                            VALUES(@CUST_USERNAME,@CUST_LANG)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@CUST_LANG", "US");
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO sharing_info(CUST_USERNAME,DISABLED,SET_PASS)
                            VALUES(@CUST_USERNAME,@DISABLED,@SET_PASS)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
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
        private void BuildGreetingLabel() {

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = $"Good Night {Globals.custUsername}";

            if (hours >= 1 && hours <= 12) {
                greeting = "Good Morning, " + Globals.custUsername;
            }
            else if (hours >= 12 && hours <= 16) {
                greeting = "Good Afternoon, " + Globals.custUsername;
            }
            else if (hours >= 16 && hours <= 21) {

                if (hours == 20 || hours == 21) {
                    greeting = "Good Late Evening, " + Globals.custUsername;
                }

            }
            else if (hours >= 21 && hours <= 24) {
                greeting = "Good Night, " + Globals.custUsername;
            }

            accessHomePage.lblGreetingText.Text = greeting;

        }

        /// <summary>
        /// Create file and insert user username into that file in a sub folder 
        /// called FlowStorageInfos located in %appdata%
        /// </summary>
        /// <param name="_custUsername">Username of user</param>
        private void StupAutoLogin(String custUsername) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private async Task GetUserLanguage() {
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

        private void ClearRegistrationFields() {

            lblAlertUsername.Visible = false;
            lblAlertPassword.Visible = false;
            lblAlertPin.Visible = false;
            pnlRegistration.Visible = false;

            accessHomePage.lblLimitUploadText.Text = "25";

            txtBoxUsernameField.Text = String.Empty;
            txtBoxAuth0Field.Text = String.Empty;
            txtBoxEmailField.Text = String.Empty;
            txtBoxAuth1Field.Text = String.Empty;
        }

        private void guna2Button10_Click(object sender, EventArgs e) => new SignInForm(this).Show();

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void txtBoxAuth1Field_TextChanged(object sender, EventArgs e) {

        }

        private void SignUpForm_Load(object sender, EventArgs e) {

        }

        private void bannerPictureBox_Click(object sender, EventArgs e) {

        }
    }
}
