using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Authentication;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Model;
using FlowstorageDesktop.Temporary;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class SignInForm : Form {

        public static SignInForm instance;

        private readonly SignUpForm mainForm;

        private readonly HomePage accessHomePage = HomePage.instance;

        private readonly UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();
        private readonly StartupQuery startupQuery = new StartupQuery();
        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string _returnedAuth0 { get; set; }
        private string _returnedAuth1 { get; set; }
        private string _custUsername { get; set; }
        private string _inputGetEmail { get; set; }
        private int _attemptCurr { get; set; } = 0;
        private HomePage _homePage { get; set; } = HomePage.instance;

        public SignInForm(SignUpForm mainForm) {
            InitializeComponent();

            this.mainForm = mainForm;

            instance = this;
        }

        private async Task SetupUserInformation() {

            var flowLayout = accessHomePage.flwLayoutHome;
            var garbageButton = accessHomePage.btnGarbageImage;
            var itsEmptyHereLabel = accessHomePage.lblEmptyHere;

            _custUsername = await userAuthQuery.GetUsernameByEmail(_inputGetEmail);

            flowLayout.Controls.Clear();
            accessHomePage.lstFoldersPage.Items.Clear();

            tempDataUser.Username = _custUsername;
            tempDataUser.Email = _inputGetEmail;

            garbageButton.Visible = itsEmptyHereLabel.Visible = lblAlert.Visible = false;

            if (flowLayout.Controls.Count == 0) {
                buildEmptyBody();
            }

        }

        private void buildEmptyBody() {
            accessHomePage.lblEmptyHere.Visible = true;
            accessHomePage.btnGarbageImage.Visible = true;
        }

        private async Task GenerateUserFolders(string username) {

            List<string> foldersName = await startupQuery.GetFolders(username);

            _homePage.lstFoldersPage.Items.AddRange(foldersName.ToArray());
            
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

                var authenticationInformation = await userAuthQuery.GetAccountAuthentication(_inputGetEmail);

                if (!string.IsNullOrEmpty(authenticationInformation["password"])) {
                    _returnedAuth0 = authenticationInformation["password"];

                    if (!string.IsNullOrEmpty(authenticationInformation["pin"])) {
                        _returnedAuth1 = authenticationInformation["pin"];

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

                StartPopupForm.StartRetrievalPopup(isFromLogin: true);

                try {

                    HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();
                    HomePage.instance.lblCurrentPageText.Text = "Home";

                    await GenerateUserData();

                    Application.OpenForms.OfType<RetrievalAlert>().FirstOrDefault().Close();

                    if (guna2CheckBox2.Checked) {
                        new AutoLoginModel().SetupAutoLoginData(tempDataUser.Username, tempDataUser.Email);

                    }

                } catch (Exception) {
                    MessageBox.Show(
                        "An error occurred. Check your internet connection and try again.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            _homePage.CallInitialStartupData = true;

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

                bool userCanSignIn = await VerifyUserAuthentication();

                if (userCanSignIn) {

                    int calculatePercentageUsage = 0;
                    int uploadLimit = await userAuthQuery.GetUploadLimit(tempDataUser.Username);

                    tempDataUser.AccountType = Globals.uploadFileLimitToAccountType[uploadLimit];

                    if (int.TryParse(HomePage.instance.lblItemCountText.Text, out int getCurrentCount) && int.TryParse(uploadLimit.ToString(), out int getLimitedValue)) {
                        if (getLimitedValue == 0) {
                            HomePage.instance.lblUsagePercentage.Text = "0%";

                        } else {
                            calculatePercentageUsage = (getCurrentCount * 100) / getLimitedValue;
                            HomePage.instance.lblUsagePercentage.Text = calculatePercentageUsage.ToString() + "%";

                        }

                        HomePage.instance.lblLimitUploadText.Text = uploadLimit.ToString();
                        HomePage.instance.progressBarUsageStorage.Value = calculatePercentageUsage;

                    }

                    ShowHomePageOnSuceededSignIn();

                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to sign-in to your account", subheader: "Are you connected to the internet?").Show();
            }

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
