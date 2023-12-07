using FlowSERVER1.AlertForms;
using FlowSERVER1.Authentication;
using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class SignInForm : Form {

        public static SignInForm instance;

        private readonly SignUpForm mainForm;

        private readonly HomePage accessHomePage = HomePage.instance;

        private readonly MySqlConnection con = ConnectionModel.con;

        private string _returnedAuth0 { get; set; }
        private string _returnedAuth1 { get; set; }
        private string _custUsername { get; set; }
        private string _inputGetEmail { get; set; }
        private int _attemptCurr { get; set; } = 0;
        private string _currentLang { get; set; } = "US";
        private HomePage _homePage { get; set; } = HomePage.instance;

        public SignInForm(SignUpForm mainForm) {
            InitializeComponent();

            this.mainForm = mainForm;

            instance = this;
        }

        private void SetupAutoLogin(String username, String email) {

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";

            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(username));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(email));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            } else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(username));
                    _performWrite.WriteLine(EncryptionModel.Encrypt(email));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }

        }

        private string ReturnCustomColumn(string whichColumn) {

            var concludeValue = new List<string>();

            string checkPasswordQuery = $"SELECT {whichColumn} FROM information WHERE CUST_EMAIL = @email";
            using (var command = new MySqlCommand(checkPasswordQuery, con)) {
                command.Parameters.AddWithValue("@email", _inputGetEmail);
                using (var readerPass = command.ExecuteReader()) {
                    while (readerPass.Read()) {
                        concludeValue.Add(readerPass.GetString(0));
                    }
                }
            }

            return concludeValue.FirstOrDefault() ?? string.Empty;
        }

        private async Task SetupUserInformation() {

            var flowLayout = accessHomePage.flwLayoutHome;
            var garbageButton = accessHomePage.btnGarbageImage;
            var itsEmptyHereLabel = accessHomePage.lblEmptyHere;

            const string selectUsernameQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_EMAIL = @email";

            using (var command = new MySqlCommand(selectUsernameQuery, con)) {
                command.Parameters.AddWithValue("@email", _inputGetEmail);
                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        _custUsername = reader.GetString(0);
                    }
                }
            }
 
            flowLayout.Controls.Clear();
            accessHomePage.lstFoldersPage.Items.Clear();

            Globals.custUsername = _custUsername;
            Globals.custEmail = _inputGetEmail;

            garbageButton.Visible = itsEmptyHereLabel.Visible = lblAlert.Visible = false;

            if (flowLayout.Controls.Count == 0) {
                buildEmptyBody();
            }

        }

        private void buildEmptyBody() {
            accessHomePage.lblEmptyHere.Visible = true;
            accessHomePage.btnGarbageImage.Visible = true;
        }

        private async Task GenerateUserFolders(String userName) {

            string[] itemFolder = { "Home", "Shared To Me", "Shared Files" };
            _homePage.lstFoldersPage.Items.AddRange(itemFolder);
            _homePage.lstFoldersPage.SelectedIndex = 0;

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
                _homePage.lstFoldersPage.Items.Add(titleEach);
            }
        }

        /// <summary>
        /// 
        /// Verify user input authentication and process
        /// sign in verification and load user data
        /// 
        /// </summary>
        /// <returns>verifyStatus</returns>
        private async Task<bool> VerifyUserAuthentication() {

            var inputEmail = txtFieldEmail.Text;
            _inputGetEmail = inputEmail;

            var inputAuth0 = txtFieldAuth.Text;
            var inputAuth1 = txtFieldPIN.Text;

            bool authenticationSuccessful = false;

            try {

                if (!string.IsNullOrEmpty(ReturnCustomColumn("CUST_PASSWORD"))) {
                    _returnedAuth0 = ReturnCustomColumn("CUST_PASSWORD");
                    if (!string.IsNullOrEmpty(ReturnCustomColumn("CUST_PIN"))) {
                        _returnedAuth1 = ReturnCustomColumn("CUST_PIN");
                    }
                }

            } catch (Exception) {
                lblAlert.Visible = true;
            }

            if (EncryptionModel.computeAuthCase(inputAuth0) == _returnedAuth0 &&
                EncryptionModel.computeAuthCase(inputAuth1) == _returnedAuth1) {

                authenticationSuccessful = true;

                await SetupUserInformation();

                this.Close();

                Thread _retrievalAlertForm = new Thread(() => new RetrievalAlert("Connecting to your account...", "login").ShowDialog());
                _retrievalAlertForm.Start();

                try {

                    HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();

                    GlobalsData.fileTypeValuesSharedToMe.Clear();
                    GlobalsData.fileTypeValuesSharedToOthers.Clear();

                    await GenerateUserData();

                    RetrievalAlert retrievalAlertForm = Application.OpenForms.OfType<RetrievalAlert>().FirstOrDefault();
                    retrievalAlertForm?.Close();

                    if (guna2CheckBox2.Checked) {
                        SetupAutoLogin(Globals.custUsername, Globals.custEmail);
                    }

                } catch (Exception) {
                    MessageBox.Show(
                        "An error occurred. Check your internet connection and try again.",
                        "Flowstorage",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

            } else {
                CloseFormOnLimitSignInAttempt();
            }

            return authenticationSuccessful;
        }

        /// <summary>
        /// 
        /// Login failed for 5 attempts then close the form.
        /// 
        /// </summary>
        private void CloseFormOnLimitSignInAttempt() {
            lblAlert.Visible = true;
            if (_attemptCurr == 5) {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// Generate user data into HomePage 
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task GenerateUserData() {

            await GenerateUserFolders(_custUsername);
        }

        private void HomePage_HomePageClosed(object sender, FormClosedEventArgs e) {
            if (Application.OpenForms.Count >= 1) {
                Application.Exit();
            }
        }

        private void ShowHomePageOnSuceededSignIn() {
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

                _attemptCurr++;

                var verifyAuthenticaton = await VerifyUserAuthentication();
                if (verifyAuthenticaton) {

                    int calculatePercentageUsage = 0;
                    if (int.TryParse(HomePage.instance.lblItemCountText.Text, out int getCurrentCount) && int.TryParse(await GetUserAccountType(), out int getLimitedValue)) {
                        if (getLimitedValue == 0) {
                            HomePage.instance.lblUsagePercentage.Text = "0%";

                        } else {
                            calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                            HomePage.instance.lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                        }

                        HomePage.instance.lblLimitUploadText.Text = await GetUserAccountType();
                        HomePage.instance.progressBarUsageStorage.Value = calculatePercentageUsage;

                    }

                    ShowHomePageOnSuceededSignIn();

                }

            } catch (Exception) {
                new CustomAlert(title: "Failed to sign-in to your account", subheader: "Are you connected to the internet?").Show();
            }

        }

        private async Task<string> GetUserAccountType() {

            string accountType = "";

            const string querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                accountType = Convert.ToString(await command.ExecuteScalarAsync());
            }

            Globals.accountType = accountType;
            return Globals.uploadFileLimit[accountType].ToString();
        }

        private void LogIN_Load(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            txtFieldAuth.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            txtFieldAuth.PasswordChar = '\0';
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e) {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtFieldPIN.Text, "[^0-9]")) {
                txtFieldPIN.Text = txtFieldPIN.Text.Remove(txtFieldPIN.Text.Length - 1);
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

        private void label5_Click(object sender, EventArgs e) {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            new ValidateRecoveryEmail().Show();
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) => this.Close();
        
    }
}
