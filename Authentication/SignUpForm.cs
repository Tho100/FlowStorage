using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Model;
using FlowstorageDesktop.Temporary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FlowstorageDesktop.Authentication {
    public partial class SignUpForm : Form {

        private readonly UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();
        private readonly StartupQuery startupQuery = new StartupQuery();

        private readonly HomePage accessHomePage = new HomePage();
        private readonly SignUpQuery signUpQuery = new SignUpQuery();

        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

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

                var autoLoginData = new AutoLoginModel().ReadAutoLoginData();

                if(string.IsNullOrEmpty(autoLoginData)) {
                    return;
                }

                string username = EncryptionModel.Decrypt(File.ReadLines(autoLoginData).First());
                string email = EncryptionModel.Decrypt(File.ReadLines(autoLoginData).Skip(1).First());

                if (string.IsNullOrEmpty(username)) {
                    pnlRegistration.Visible = true;
                    return;
                }

                tempDataUser.Username = username;
                tempDataUser.Email = email;

                List<string> folders = await startupQuery.GetFolders(username);

                accessHomePage.CallInitialStartupData = true;

                string uploadLimit = (await userAuthQuery.GetUploadLimit(tempDataUser.Username)).ToString();
                string accountType = await userAuthQuery.GetAccountType(tempDataUser.Username);

                accessHomePage.lstFoldersPage.Items.AddRange(folders.ToArray());
                accessHomePage.lblCurrentPageText.Text = "Home";
                accessHomePage.lblLimitUploadText.Text = uploadLimit;

                tempDataUser.AccountType = accountType;

                pnlRegistration.Visible = false;

                ShowHomePage();

            } catch (Exception) {
                new CustomAlert(
                    title: "Something went wrong", subheader: "Are you connected to the internet?").Show();

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

        /// <summary>
        /// Validate user entered email address format
        /// </summary>
        /// <param name="emailInput"></param>
        /// <returns></returns>
        private bool ValidateEmailInput(string emailInput) {
            const string _regPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(_regPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(emailInput);
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

                var accountsInfo = await signUpQuery.VerifyUsernameAndEmail(usernameInput, emailInput);

                int usernamesCount = accountsInfo["username"];
                int emailsCount = accountsInfo["email"];

                if (usernamesCount >= 1 || emailsCount >= 1) {

                    if (usernamesCount >= 1) {
                        lblAlertUsername.Visible = true;
                        lblAlertUsername.Text = "Username is taken.";
                    }

                    if (emailsCount >= 1) {
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

                    if (string.IsNullOrEmpty(pinInput)) {
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

                    if (!ValidateEmailInput(emailInput)) {
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

                    if (string.IsNullOrEmpty(emailInput)) {
                        lblAlertEmail.Visible = true;
                        lblAlertEmail.Text = "Please add your email";
                        return;
                    }

                    if (string.IsNullOrEmpty(passwordInput)) {
                        lblAlertPassword.Visible = true;
                        return;

                    }

                    if (string.IsNullOrEmpty(usernameInput)) {
                        lblAlertUsername.Visible = true;
                        return;
                    }

                    flowlayout.Controls.Clear();

                    if (flowlayout.Controls.Count == 0) {
                        HomePage.instance.lblEmptyHere.Visible = true;
                        HomePage.instance.btnGarbageImage.Visible = true;
                    }

                    accessHomePage.CallInitialStartupData = false;

                    tempDataUser.Username = usernameInput;
                    tempDataUser.Email = emailInput;
                    tempDataUser.AccountType = "Basic";

                    await signUpQuery.InsertUserRegistrationData(
                        usernameInput, emailInput, passwordInput, pinInput);

                    ClearRegistrationFields();

                    new AutoLoginModel().SetupAutoLoginData(usernameInput, emailInput);

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

        private void ClearRegistrationFields() {

            lblAlertUsername.Visible = false;
            lblAlertPassword.Visible = false;
            lblAlertPin.Visible = false;
            pnlRegistration.Visible = false;

            accessHomePage.lblLimitUploadText.Text = "25";

            txtBoxUsernameField.Text = string.Empty;
            txtBoxAuth0Field.Text = string.Empty;
            txtBoxEmailField.Text = string.Empty;
            txtBoxAuth1Field.Text = string.Empty;
        }

        private void guna2Button10_Click(object sender, EventArgs e) => new SignInForm(this).Show();

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void txtBoxAuth1Field_TextChanged(object sender, EventArgs e) {
            if (Regex.IsMatch(txtBoxAuth1Field.Text, "[^0-9]")) {
                txtBoxAuth1Field.Text = txtBoxAuth1Field.Text.Remove(txtBoxAuth1Field.Text.Length - 1);
            }
        }

        private void SignUpForm_Load(object sender, EventArgs e) {

        }

        private void bannerPictureBox_Click(object sender, EventArgs e) {

        }
    }
}
