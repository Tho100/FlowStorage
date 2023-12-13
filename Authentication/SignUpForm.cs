﻿using FlowSERVER1.AlertForms;

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

                string startupPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                if (!Directory.Exists(startupPath)) {
                    return;
                }

                new DirectoryInfo(startupPath).Attributes &= ~FileAttributes.Hidden;

                string authFile = Path.Combine(startupPath, "CUST_DATAS.txt");
                if (!File.Exists(authFile) || new FileInfo(authFile).Length == 0) {
                    return;
                }

                string username = EncryptionModel.Decrypt(File.ReadLines(authFile).First());
                string email = EncryptionModel.Decrypt(File.ReadLines(authFile).Skip(1).First());

                if (AccountStartupValidation(username) == String.Empty) {
                    pnlRegistration.Visible = true;
                    return;
                }

                Globals.custUsername = username;
                Globals.custEmail = email;

                var foldersName = new List<string>();

                using (var command = new MySqlCommand("SELECT DISTINCT FOLDER_TITLE FROM folder_upload_info WHERE CUST_USERNAME = @username", ConnectionModel.con)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            foldersName.Add(EncryptionModel.Decrypt(reader.GetString(0)));
                        }
                    }
                }

                accessHomePage.CallInitialStartupData = true;

                accessHomePage.lstFoldersPage.Items.AddRange(foldersName.ToArray());
                accessHomePage.lblCurrentPageText.Text = "Home";

                await GetUserAccountType();

                pnlRegistration.Visible = false;

                ShowHomePage();

            } catch (Exception) {
                new CustomAlert(title: "Something went wrong", subheader: "Are you connected to the internet?")
                    .Show();

            } finally {

                string infosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlowStorageInfos");

                if(Directory.Exists(infosPath)) {
                    new DirectoryInfo(infosPath).Attributes |= (FileAttributes.Directory | FileAttributes.Hidden);
                    Application.OpenForms.OfType<Form>().Where(form => form.Name == "LoadAlertFORM").ToList().ForEach(form => form.Close());
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

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
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
                string concludeUsername = (string) command.ExecuteScalar();
                return concludeUsername ?? String.Empty;
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
        private async void btnSignUp_Click(object sender, EventArgs e) {

            try {

                Control flowlayout = accessHomePage.flwLayoutHome;

                string usernameInput = txtBoxUsernameField.Text;
                string emailInput = txtBoxEmailField.Text;
                string passwordInput = txtBoxAuth0Field.Text;
                string pinInput = txtBoxAuth1Field.Text;

                List<string> existsInfosMail = new List<string>();
                List<string> existsInfosUser = new List<string>();

                using (MySqlCommand command = con.CreateCommand()) {
                    const string verifyQueryUser = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
                    command.CommandText = verifyQueryUser;
                    command.Parameters.AddWithValue("@username", usernameInput);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            existsInfosUser.Add(reader.GetString(0));
                        }
                    }

                    const string verifyQueryMail = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
                    command.CommandText = verifyQueryMail;
                    command.Parameters.AddWithValue("@email", emailInput);

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

                } else {

                    lblAlertEmail.Visible = false;
                    lblAlertPassword.Visible = false;
                    lblAlertUsername.Visible = false;

                    if (usernameInput.Contains("&") || usernameInput.Contains(";") || usernameInput.Contains("?") || usernameInput.Contains("%")) {
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Special characters is not allowed.";
                        return;
                    }

                    if (String.IsNullOrEmpty(pinInput)) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "Please add a PIN number.";
                        return;
                    }

                    if (!(pinInput.All(char.IsDigit))) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "PIN must be a number.";
                        return;
                    }

                    if (pinInput.Length != 3) {
                        lblAlertPin.Visible = true;
                        lblAlertPin.Text = "PIN Number must have 3 digits.";
                        return;
                    }

                    if (!ValidateEmailInput(emailInput) == true) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Entered email is not valid.";
                        return;
                    }

                    if (usernameInput.Length > 20) {
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Username character length limit is 20.";
                        return;
                    }

                    if (passwordInput.Length < 5) {
                        lblAlertPassword.Visible = true;
                        lblAlertPassword.Text = "Password must be longer than 5 characters.";
                        return;
                    }

                    if (String.IsNullOrEmpty(emailInput)) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Please add your email";
                        return;
                    }

                    if (String.IsNullOrEmpty(passwordInput)) {
                        lblAlertPassword.Visible = true;
                        return;

                    }

                    if (String.IsNullOrEmpty(usernameInput)) {
                        lblAlertUsername.Visible = true;
                        return;
                    }

                    flowlayout.Controls.Clear();

                    if (flowlayout.Controls.Count == 0) {
                        HomePage.instance.lblEmptyHere.Visible = true;
                        HomePage.instance.btnGarbageImage.Visible = true;
                    }

                    accessHomePage.CallInitialStartupData = false;

                    Globals.custUsername = usernameInput;
                    Globals.custEmail = emailInput;
                    Globals.accountType = "Basic";

                    InsertUserRegistrationData(usernameInput, emailInput, passwordInput, pinInput);

                    ClearRegistrationFields();
                    StupAutoLogin(usernameInput, emailInput);

                    accessHomePage.lblCurrentPageText.Text = "Home";
                    accessHomePage.lblUsagePercentage.Text = "0%";
                    accessHomePage.progressBarUsageStorage.Value = 0;

                    ShowHomePage();
                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to register your account", subheader: "Are you connected to the internet?").Show();

            }
        }

        private void InsertUserRegistrationData(string getUser, string getEmail, string getAuth, string getPin) {

            var _setupRecov = RandomString(16) + getUser;
            var _removeSpacesRecov = new string(_setupRecov.Where(c => !Char.IsWhiteSpace(c)).ToArray());

            string _getDate = DateTime.Now.ToString("MM/dd/yyyy");

            using (var transaction = con.BeginTransaction()) {

                try {

                    MySqlCommand command = con.CreateCommand();

                    command.CommandText = @"INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL,CUST_PIN,RECOV_TOK)
                            VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL,@CUST_PIN,@RECOV_TOK)";
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@CUST_PASSWORD", EncryptionModel.computeAuthCase(getAuth));
                    command.Parameters.AddWithValue("@CREATED_DATE", _getDate);
                    command.Parameters.AddWithValue("@CUST_EMAIL", getEmail);
                    command.Parameters.AddWithValue("@CUST_PIN", EncryptionModel.computeAuthCase(getPin));
                    command.Parameters.AddWithValue("@RECOV_TOK", EncryptionModel.Encrypt(_removeSpacesRecov));

                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE)
                            VALUES(@CUST_USERNAME,@CUST_EMAIL,@ACC_TYPE)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@CUST_EMAIL", getEmail);
                    command.Parameters.AddWithValue("@ACC_TYPE", "Basic");
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO sharing_info(CUST_USERNAME,DISABLED,SET_PASS,PASSWORD_DISABLED)
                            VALUES(@CUST_USERNAME,@DISABLED,@SET_PASS,@PASSWORD_DISABLED)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CUST_USERNAME", getUser);
                    command.Parameters.AddWithValue("@DISABLED", "0");
                    command.Parameters.AddWithValue("@SET_PASS", "DEF");
                    command.Parameters.AddWithValue("@PASSWORD_DISABLED", "1");
                    command.ExecuteNonQuery();

                    transaction.Commit();

                } catch (Exception) {
                    transaction.Rollback();

                }

            }
        }

        /// <summary>
        /// Create file and insert user username into that file in a sub folder 
        /// called FlowStorageInfos located in %appdata%
        /// </summary>
        /// <param name="_custUsername">Username of user</param>
        private void StupAutoLogin(String custUsername, String custEmail) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);

                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custEmail));
                }

                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            } else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custEmail));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

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
