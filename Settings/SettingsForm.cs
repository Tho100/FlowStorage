using FlowSERVER1.AlertForms;
using FlowSERVER1.Settings;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {

    public partial class SettingsForm : Form {

        static public SettingsForm instance;

        readonly private MySqlConnection con = ConnectionModel.con;

        private List<int> _totalUploadToday { get; set; } = new List<int>();
        private List<int> _totalUploadAllTime { get; set; } = new List<int>();
        private string _currentUserLanguage { get; set; } = "US";
        private string _newSelectedUserLanguage { get; set; } = null;

        private string _selectedAccountType { get; set; }
        public SettingsForm() {

            InitializeComponent();

            CloseForm.closeForm("SettingsLoadingAlert");

            instance = this;

            lblUserEmail.Text = Globals.custEmail;
            lblUserUsername.Text = Globals.custUsername;

            foreach (var axis in chart1.ChartAreas[0].Axes) {
                axis.MajorGrid.Enabled = false;
                axis.MinorGrid.Enabled = false;
            }

            if(Globals.accountType != "Basic") {
                pnlCancelPlan.Visible = true;
            }

            InitializeSettingsAsync();
        }

        private async void InitializeSettingsAsync() {

            try {

                await GetCurrentLanguage();

                InitializeUILanguage(_currentUserLanguage);
                InitiailizeUIOnAccountType(lblAccountType.Text);
                InitializeUploadLimitLabel();

                chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                string[] _halfTablesName = { "info", "info_expand", "info_vid", "info_pdf", "info_apk", "info_exe", "info_word", "info_ptx", "info_audi", "info_excel" };
                string[] _chartXAxisValues = { "Image", "Text", "Video", "PDF", "APK", "Exe", "Document", "Presentation", "Audio", "Excel" };

                foreach ((string tableName, string chartName) in _halfTablesName.Zip(_chartXAxisValues, (a, b) => (a, b))) {
                    await GenerateUploadChart(chartName, "file_" + tableName.ToLower());
                    await TotalUploadFileTodayCount("file_" + tableName.ToLower());
                    await TotalUploadFile("file_" + tableName.ToLower());
                }

                await TotalUploadDirectoryCount();

                int _totalUploadTodayCount = _totalUploadToday.Sum(x => Convert.ToInt32(x));
                lblCountFileUploadToday.Text = _totalUploadTodayCount.ToString();

                int _totalUploadOvertime = _totalUploadAllTime.Sum(x => Convert.ToInt32(x));
                lblTotalUploadFileCount.Text = _totalUploadOvertime.ToString();

            }
            catch (Exception) {
                new CustomAlert(title: "An error occurred", "Something went wrong while trying to open Settings.");
            }
        }

        #region Sharing section

        /// <summary>
        /// This function will delete user file sharing password 
        /// if they have one setup
        /// </summary>
        /// <param name="_custUsername"></param>
        private void RemoveSharingAuth() {

            const string setPassSharingQuery = "UPDATE sharing_info SET SET_PASS = @setPass WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(setPassSharingQuery, con)) {
                command.Parameters.AddWithValue("@setPass", "DEF");
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.ExecuteNonQuery();
            }

        }
        private void guna2Button27_Click(object sender, EventArgs e) {

            if (MessageBox.Show("Remove File Sharing password?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                RemoveSharingAuth();

                btnAddSharingAuth.Visible = true;
                btnAddSharingAuth.Enabled = true;

                Globals.sharingAuthStatus = "DEF";

                btnRmvSharingAuth.Visible = false;
                btnRmvSharingAuth.Enabled = false;
            }
        }

        /// <summary>
        /// This function will retrieve the 
        /// current status of user file sharing (disabled, or enabled)
        /// </summary>
        private string RetrieveIsSharingDisabled() {

            const string querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

                string isEnabled = Convert.ToString(command.ExecuteScalar());
                string concludeOutput = isEnabled == "1" ? "1" : "0";
                return concludeOutput;
            }
        }

        /// <summary>
        /// Function to start disabling file sharing
        /// </summary>
        /// <param name="_custUsername"></param>
        private void DisableSharing(String _custUsername) {

            const string disableSharingQuery = "UPDATE sharing_info SET DISABLED = 1 WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(disableSharingQuery, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Function to enable file sharing
        /// </summary>
        /// <param name="_custUsername"></param>
        private void EnableSharing(String _custUsername) {

            const string enabelSharingQuery = "UPDATE sharing_info SET DISABLED = 0 WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(enabelSharingQuery, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Disable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button24_Click(object sender, EventArgs e) {

            if (MessageBox.Show("Disable file sharing? You can always enable this option again later.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                btnDisableSharing.Enabled = false;
                btnDisableSharing.Visible = false;

                btnEnableSharing.Visible = true;
                btnEnableSharing.Enabled = true;

                lblDisableFileSharing.Text = "Enable File Sharing";
                lblDescDisableSharing.Text = "Enabling file sharing will allows people to share a file to you";

                Globals.sharingDisabledStatus = "1";

                DisableSharing(Globals.custUsername);

            }
        }

        /// <summary>
        /// Enable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button26_Click(object sender, EventArgs e) {

            btnDisableSharing.Enabled = true;
            btnDisableSharing.Visible = true;

            btnEnableSharing.Visible = false;
            btnEnableSharing.Enabled = false;

            lblDisableFileSharing.Text = "Disable File Sharing";
            lblDescDisableSharing.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";

            Globals.sharingDisabledStatus = "0";

            EnableSharing(Globals.custUsername);

        }

        private string RetrieveFileSharingAuth() {

            string hasPass = "";
            const string selectQuery = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectQuery, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

                object result = command.ExecuteScalar();
                if (result != null) {
                    hasPass = result.ToString();
                }
            }

            return hasPass;
        }

        #endregion END - Sharing section

        #region User statistics section

        /// <summary>
        /// This function will tells user how many files
        /// they have uploaded (in total)
        /// </summary>
        /// <param name="_tableName"></param>
        private async Task TotalUploadFile(String tableName) {

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if(origin == "Home") {

                if (tableName == "file_info") {

                    List<string> dateLabels = new List<string>(HomePage.instance.flwLayoutHome.Controls
                        .OfType<Guna2Panel>()
                        .SelectMany(panel => panel.Controls.OfType<Label>())
                        .Where(label=> label.Text.Contains("/") && label.Text.Any(char.IsDigit))
                        .Select(label => label.Text));

                    int totalUploadCount = dateLabels.Count();
                    _totalUploadAllTime.Add(totalUploadCount);
                }

            } else {

                string queryCount = $"SELECT COUNT(*) FROM {tableName} WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(queryCount, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    int totalUploadCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _totalUploadAllTime.Add(totalUploadCount);
                }

            }
        }

        private async Task TotalUploadFileTodayCount(String tableName) {

            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if (origin == "Home") {
                    
                if(tableName == "file_info") {

                    List<string> todayDateLabels = new List<string>(HomePage.instance.flwLayoutHome.Controls
                        .OfType<Guna2Panel>()
                        .SelectMany(panel => panel.Controls.OfType<Label>())
                        .Where(label => label.Text == currentDate)
                        .Select(label => label.Text));

                    int totalCount = todayDateLabels.Count();
                    _totalUploadToday.Add(totalCount);
                }

            } else {

                string queryCount = $"SELECT COUNT(*) FROM {tableName} WHERE CUST_USERNAME = @username AND UPLOAD_DATE = @date";
                using (MySqlCommand command = new MySqlCommand(queryCount, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@date", currentDate);

                    int totalCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _totalUploadToday.Add(totalCount);
                }
            }

        }

        private async Task TotalUploadDirectoryCount() {

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if (origin == "Home") {

                HashSet<string> directoriesName = new HashSet<string>(HomePage.instance.flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Where(label => label.Text.All(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)))
                    .Where(label => !string.Equals(label.Text, "directory", StringComparison.OrdinalIgnoreCase))
                    .Select(label => label.Text.ToLower()));

                int countTotalDir = directoriesName.Count();
                lblTotalDirUploadCount.Text = countTotalDir.ToString();

            } else {

                const string countDirQuery = "SELECT COUNT(*) FROM file_info_directory WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(countDirQuery, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    int totalDir = Convert.ToInt32(await command.ExecuteScalarAsync());
                    lblTotalDirUploadCount.Text = totalDir.ToString();
                }
            }

            int countTotalFolders = HomePage.instance.lstFoldersPage.Items.Count - 3;
            lblTotalFolderUploadCount.Text = countTotalFolders.ToString();
        }


        /// <summary>
        /// This will generates statistical chart
        /// that tells user how many files they've
        /// uploaded by date
        /// </summary>
        /// <param name="_serName"></param>
        /// <param name="_tableName"></param>

        private async Task GenerateUploadChart(String seriesName, String tableName) {

            string querySelectDate = $"SELECT UPLOAD_DATE, COUNT(UPLOAD_DATE) FROM {tableName} WHERE CUST_USERNAME = @username GROUP BY UPLOAD_DATE HAVING COUNT(UPLOAD_DATE) > 0";

            using (MySqlCommand command = new MySqlCommand(querySelectDate, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string date = reader.GetString(0);
                        int count = reader.GetInt32(1);
                        chart1.Series[seriesName].Points.AddXY(date, count);
                    }
                }
            }
        }
      
        #endregion END - User statistics section

        private void guna2Button14_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button3_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button25_Click_1(object sender, EventArgs e) => this.Close();

        private void guna2Button1_Click(object sender, EventArgs e) => new RemoveAccountForm().Show();

        private void guna2Button23_Click(object sender, EventArgs e) => new AddAuthSharing().Show();
        private void guna2Button12_Click(object sender, EventArgs e) => new ChangeAuthForm().Show();
        private void guna2Button13_Click(object sender, EventArgs e) => new ChangeUsernameForm().Show();
        private void guna2Button1_Click_2(object sender, EventArgs e) => new BackupRecoveryKeyForm().Show();
        private void label5_Click(object sender, EventArgs e) => Clipboard.SetText(Globals.custUsername);
        private void label76_Click(object sender, EventArgs e) => Clipboard.SetText(Globals.custEmail);
        private void guna2Button2_Click_2(object sender, EventArgs e) => new CancelPlanForm().Show();
        private void guna2Panel4_Paint(object sender, PaintEventArgs e) {

        }

        private void remAccFORM_Load(object sender, EventArgs e) {

        }

        private void chart1_Click(object sender, EventArgs e) {

        }

        private void tabPage2_Click(object sender, EventArgs e) {
        }


        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e) {

        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void label37_Click(object sender, EventArgs e) {

        }

        private void label38_Click(object sender, EventArgs e) {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel10_Paint(object sender, PaintEventArgs e) {

        }

        private void label28_Click_1(object sender, EventArgs e) {

        }

        private void guna2Panel3_Paint_2(object sender, PaintEventArgs e) {

        }

        private void label29_Click_1(object sender, EventArgs e) {

        }

        private void label30_Click_1(object sender, EventArgs e) {

        }

        private void guna2Panel9_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e) {

        }

        public void InitiailizeUIOnAccountType(String selectedAcc) {
            if (selectedAcc == "Supreme") {
                btnOpenExpressPayment.Enabled = false;
                btnOpenSupremePayment.Enabled = false;
                btnOpenMaxPayment.Enabled = false;
                btnUseSupreme.Visible = false;
                lblLimitedUpload.Text = "Limited to 2000";
            }
            else if (selectedAcc == "Express") {
                btnOpenExpressPayment.Enabled = false;
                btnOpenMaxPayment.Enabled = false;
                btnUseExpress.Visible = false;
                lblLimitedUpload.Text = "Limited to 800";
            }
            else if (selectedAcc == "Max") {
                btnOpenMaxPayment.Enabled = false;
                btnUseMax.Visible = false;
                lblLimitedUpload.Text = "Limited to 150";
            } 
            else if (selectedAcc == "Basic") {
                lblLimitedUpload.Text = "Limited to 25";
            }
        }

        private void InitializeUploadLimitLabel() {

            string accountType = Globals.accountType;

            lblAccountType.Text = accountType;

            if (accountType == "Basic") {
                if (this._currentUserLanguage == "US") {
                    lblLimitedUpload.Text = "Limited to 20";
                }
                else if (_currentUserLanguage == "MY") {
                    lblLimitedUpload.Text = "Terhad Kepada 20";
                }
                else if (_currentUserLanguage == "GER") {
                    lblLimitedUpload.Text = "Begrenzt Auf 20";
                }
                else if (_currentUserLanguage == "JAP") {
                    lblLimitedUpload.Text = "20 個限定";
                }
                else if (_currentUserLanguage == "ESP") {
                    lblLimitedUpload.Text = "Limitado a 20";
                }
                else if (_currentUserLanguage == "POR") {
                    lblLimitedUpload.Text = "Limitado a 20";
                }

            }
            else if (accountType == "Max") {
                if (_currentUserLanguage == "US") {
                    lblLimitedUpload.Text = "Limited to 150";
                }
                else if (_currentUserLanguage == "MY") {
                    lblLimitedUpload.Text = "Terhad Kepada 150";
                }
                else if (_currentUserLanguage == "GER") {
                    lblLimitedUpload.Text = "Begrenzt Auf 150";
                }
                else if (_currentUserLanguage == "JAP") {
                    lblLimitedUpload.Text = "150 個限定";
                }
                else if (_currentUserLanguage == "ESP") {
                    lblLimitedUpload.Text = "Limitado a 150";
                }
                else if (_currentUserLanguage == "POR") {
                    lblLimitedUpload.Text = "Limitado a 150";
                }
                btnOpenMaxPayment.Enabled = false;
            }
            else if (accountType == "Express") {
                if (_currentUserLanguage == "US") {
                    lblLimitedUpload.Text = "Limited to 800";
                }
                else if (_currentUserLanguage == "MY") {
                    lblLimitedUpload.Text = "Terhad Kepada 800";
                }
                else if (_currentUserLanguage == "GER") {
                    lblLimitedUpload.Text = "Begrenzt Auf 800";
                }
                else if (_currentUserLanguage == "JAP") {
                    lblLimitedUpload.Text = "1000 個限定";
                }
                else if (_currentUserLanguage == "ESP") {
                    lblLimitedUpload.Text = "Limitado a 800";
                }
                else if (_currentUserLanguage == "POR") {
                    lblLimitedUpload.Text = "Limitado a 800";
                }
                btnOpenMaxPayment.Enabled = false;
                btnOpenExpressPayment.Enabled = false;
            }
            else if (accountType == "Supreme") {
                if (_currentUserLanguage == "US") {
                    lblLimitedUpload.Text = "Limited to 2000";
                }
                else if (_currentUserLanguage == "MY") {
                    lblLimitedUpload.Text = "Terhad Kepada 2000";
                }
                else if (_currentUserLanguage == "GER") {
                    lblLimitedUpload.Text = "Begrenzt Auf 2000";
                }
                else if (_currentUserLanguage == "JAP") {
                    lblLimitedUpload.Text = "2000 個限定";
                }
                else if (_currentUserLanguage == "ESP") {
                    lblLimitedUpload.Text = "Limitado a 2000";
                }
                else if (_currentUserLanguage == "POR") {
                    lblLimitedUpload.Text = "Limitado a 2000";
                }
                btnOpenMaxPayment.Enabled = false;
                btnOpenExpressPayment.Enabled = false;
                btnOpenSupremePayment.Enabled = false;
            }

        }

        private static string DecryptApi(string key, string cipherText) {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer)) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        #region Account upggrade section

        private async void ValdateAccountUpgradePayment() {

            try {

                ///var _setupApiKey = DecryptApi("0afe74-gksuwpe8r", ConfigurationManager.ConnectionStrings["asfhuwdajdwdwpo=#k"].ConnectionString);

                string dateTimeNow = DateTime.Now.ToString("dd/MM/yyyy");

                const string key = "sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN"; // Replace with valid KEY
                Stripe.StripeConfiguration.ApiKey = key; 

                var service = new Stripe.CustomerService();
                var customers = service.List();

                List<string> custEmails = new List<string>();

                foreach (Stripe.Customer customer in customers) {
                    custEmails.Add(customer.Email);
                }

                if (custEmails.Contains(Globals.custEmail)) {

                    var options = new Stripe.CustomerListOptions {
                        Email = Globals.custEmail,
                        Limit = 1
                    };

                    var customersOptions = service.List(options);

                    var customerBuyer = customersOptions.Data[0];
                    var customerId = customerBuyer.Id;

                    const string updateUserAccountQuery = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(updateUserAccountQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@email", Globals.custEmail);
                        command.Parameters.AddWithValue("@type", _selectedAccountType);
                        await command.ExecuteNonQueryAsync();
                    }


                    const string insertBuyerQuery = "INSERT INTO cust_buyer(CUST_USERNAME,CUST_EMAIL,ACC_TYPE,CUST_ID,PURCHASE_DATE) VALUES (@username,@email,@type,@id,@date)";
                    using (MySqlCommand commandSecond = new MySqlCommand(insertBuyerQuery, con)) {
                        commandSecond.Parameters.AddWithValue("@username", Globals.custUsername);
                        commandSecond.Parameters.AddWithValue("@email", Globals.custEmail);
                        commandSecond.Parameters.AddWithValue("@type", _selectedAccountType);
                        commandSecond.Parameters.AddWithValue("@id", customerId);
                        commandSecond.Parameters.AddWithValue("@date", dateTimeNow);

                        await commandSecond.ExecuteNonQueryAsync();
                    }

                    lblAccountType.Text = _selectedAccountType;
                    Globals.accountType = _selectedAccountType;

                    new PaymentSuceededAlert(_selectedAccountType).Show();

                    InitiailizeUIOnAccountType(_selectedAccountType);

                }
                else {
                    new CustomAlert(
                        title: "Cannot proceed",
                        subheader: "You have to make a payment on the web first to use this plan.").Show();
                }

            }
            catch (Exception) {
                new CustomAlert(title: "Cannot proceed", subheader: "Failed to make a payment.").Show();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            _selectedAccountType = "Max";
            btnUseMax.Visible = true;
            Process.Start("https://buy.stripe.com/test_9AQ16Y9Hb6GbfKwcMP"); // Live mode
        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            _selectedAccountType = "Supreme";
            btnUseSupreme.Visible = true;
            Process.Start("https://buy.stripe.com/3csdTTeDxcxI2cMcMO"); // Test mode
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _selectedAccountType = "Express";
            btnUseExpress.Visible = true;
            Process.Start("https://buy.stripe.com/6oEbLLanh41c3gQ9AA"); // Live mode
        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            ValdateAccountUpgradePayment();
        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            ValdateAccountUpgradePayment();
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            ValdateAccountUpgradePayment();
        }

        #endregion END - Account upgrade section

        private void label9_Click(object sender, EventArgs e) {

        }

        private void label31_Click_1(object sender, EventArgs e) {

        }

        private void guna2Panel15_Paint(object sender, PaintEventArgs e) {

        }


        private void tabPage4_Click(object sender, EventArgs e) {

        }

        #region UI section

        private void SetupUIGreeting() {

            DateTime now = DateTime.Now;

            var hours = now.Hour;
            string greeting = null;

            if (hours >= 1 && hours <= 12) {
                if (_newSelectedUserLanguage == "US") {
                    greeting = "Good Morning, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "MY") {
                    greeting = "Selemat Pagi, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "GER") {
                    greeting = "Guten Morgen, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "JAP") {
                    greeting = "おはよう " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "ESP") {
                    greeting = "Buen día " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "FRE") {
                    greeting = "Bonjour " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "POR") {
                    greeting = "Bom dia " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "CHI") {
                    greeting = "早上好 " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "RUS") {
                    greeting = "Доброе утро " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "DUT") {
                    greeting = "Goedemorgen " + Globals.custUsername + " :)";
                }
            }

            else if (hours >= 12 && hours <= 16) {
                if (_newSelectedUserLanguage == "US") {
                    greeting = "Good Afternoon " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "MY") {
                    greeting = "Selamat Petang " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "GER") {
                    greeting = "Guten Tag " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "JAP") {
                    greeting = "こんにちは " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "ESP") {
                    greeting = "Buenas tardes " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "FRE") {
                    greeting = "Bon après-midi " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "POR") {
                    greeting = "Boa tarde " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "CHI") {
                    greeting = "下午好 " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "RUS") {
                    greeting = "Добрый день " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "DUT") {
                    greeting = "Goedemiddag " + Globals.custUsername + " :)";
                }
            }
            else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (_newSelectedUserLanguage == "US") {
                        greeting = "Good Late Evening, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "MY") {
                        greeting = "Selamat Lewat-Petang, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "GER") {
                        greeting = "Guten späten Abend, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "ESP") {
                        greeting = "buenas tardes " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "RUS") {
                        greeting = "Добрый день " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }

                }
                else {
                    if (_newSelectedUserLanguage == "US") {
                        greeting = "Good Evening, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "MY") {
                        greeting = "Selamat Petang, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "GER") {
                        greeting = "Guten Abend, " + Globals.custUsername;
                    }
                    else if (_newSelectedUserLanguage == "JAP") {
                        greeting = "こんばんは " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "ESP") {
                        greeting = "Buenas terdes " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "FRE") {
                        greeting = "bonne soirée " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "POR") {
                        greeting = "Boa noite " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "CHI") {
                        greeting = "晚上好 " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "RUS") {
                        greeting = "Добрый вечер " + Globals.custUsername + " :)";
                    }
                    else if (_newSelectedUserLanguage == "DUT") {
                        greeting = "Goedeavond " + Globals.custUsername + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (_newSelectedUserLanguage == "US") {
                    greeting = "Good Night, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "MY") {
                    greeting = "Selamat Malam, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "GER") {
                    greeting = "Guten Nacth, " + Globals.custUsername;
                }
                else if (_newSelectedUserLanguage == "JAP") {
                    greeting = "おやすみ " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "ESP") {
                    greeting = "Buenas noches " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "FRE") {
                    greeting = "bonne nuit " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "POR") {
                    greeting = "Boa noite " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "CHI") {
                    greeting = "晚安 " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "RUS") {
                    greeting = "Спокойной ночи " + Globals.custUsername + " :)";
                }
                else if (_newSelectedUserLanguage == "DUT") {
                    greeting = "Welterusten " + Globals.custUsername + " :)";
                }
            }

            HomePage.instance.lblGreetingText.Text = greeting;

        }
        private void InitializeUILanguage(String _custLang) {

            var Form_1 = HomePage.instance;

            if (_custLang == "MY") {
                lblSettings4.Text = "Tetapan";
                label75.Text = "Alamat Email";
                tabSharingPage.Text = "Perkongsian Fail";
                tabLangPage.Text = "Bahasa";
                tabUpgradePage.Text = "Naik Taraf";
                tabStatsPage.Text = "Perangkaan";
                tabAccountPage.Text = "Akaun";
                label4.Text = "Nama Pengguna";
                label7.Text = "Jenis Akaun";
                label38.Text = "Muat Naik Item";

                lblSettings3.Text = "Tetapan";

                lblSetPassword.Text = "Perlukan Kata-Laluan";
                lblDescSetAuthSharing.Text = "Minta kata-laluan sebelum orang dibenarkan berkongsi fail kepada anda";

                lblDisableFileSharing.Text = "Melumpuhkan Perkongsian Fail";
                lblDescDisableSharing.Text = "Melumpuhkan Perkongsian Fail akan tidak benarkan orang berkongsi fail kepada anda. Anda masih boleh berkongsi fail kepada orang lain";

                lblChangeMyPassword.Text = "Ubah kata-laluan";
                lblDescChangeAuth.Text = "Ubah kata-laluan akaun Flowstorage anda";

                lblDeleteMyAccount.Text = "Padam akaun saya";
                label3.Text = "Akaun Flowstorage anda akan dipadam bersama-sama dengan data anda";

                lblSettings.Text = "Tetapan";
                lblSettings2.Text = "Tetapan";

                label13.Text = "Kiraan Fail";
                label9.Text = "Kiraan Direktori";
                label11.Text = "Kiraan Folder";
                label31.Text = "Jumlah Upload Hari-ini";
                lblFile.Text = "Kiraan Fail";

                btnUpdatePassword.Text = "Ubah";
                btnDltAccount.Text = "Padam Akaun";

                Form_1.lblUpload.Text = "Muat-Naik";
                Form_1.btnUploadFile.Text = "Muat-Naik Fail";
                Form_1.btnUploadFolder.Text = "Muat-Naik Folder";
                Form_1.btnCreateDirectory.Text = "Buat Direktori";
                Form_1.btnFileSharing.Text = "Perkongsian Fail";
                Form_1.btnFileSharing.Size = new Size(159, 47);
                Form_1.lblEssentials.Text = "Kepentingan";
                Form_1.label2.Text = "Kiraan Item";
            }

            if (_custLang == "US") {
                lblSettings4.Text = "Settings";
                label75.Text = "Email Address";
                tabSharingPage.Text = "File Sharing";
                tabLangPage.Text = "Languages";
                tabUpgradePage.Text = "Upgrade";
                tabStatsPage.Text = "Statistics";
                tabAccountPage.Text = "Account";
                label4.Text = "Username";
                label7.Text = "Account Type";
                label38.Text = "Item Upload";
                lblSettings3.Text = "Settings";

                lblSetPassword.Text = "Required Password";
                lblDescSetAuthSharing.Text = "Ask for password before people can share a file to you";

                lblDisableFileSharing.Text = "Disable File Sharing";
                lblDescDisableSharing.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";

                lblChangeMyPassword.Text = "Change my password";
                lblDescChangeAuth.Text = "Change your Flowstorage account password";

                lblDeleteMyAccount.Text = "Delete my account";
                label3.Text = "Your Flowstorage account along with your data will be deleted";

                lblSettings.Text = "Settings";
                lblSettings2.Text = "Settings";

                label13.Text = "Files Count";
                label9.Text = "Directory Count";
                label11.Text = "Folders Count";
                label31.Text = "Total Upload Today";
                lblFile.Text = "File Count";

                btnUpdatePassword.Text = "Change";
                btnDltAccount.Text = "Delete Account";

                Form_1.lblUpload.Text = "Upload";
                Form_1.label2.Text = "Item Count";
                Form_1.btnUploadFile.Text = "Upload File";
                Form_1.btnUploadFolder.Text = "Upload Folder";
                Form_1.btnCreateDirectory.Text = "Create Directory";
                Form_1.btnFileSharing.Text = "File Sharing";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "DUT") {
                lblSettings4.Text = "Instellingen";
                label75.Text = "E-mailadres";
                tabSharingPage.Text = "Bestanden delen";
                tabLangPage.Text = "Talen";
                tabUpgradePage.Text = "Opwaarderen";
                tabStatsPage.Text = "Statistieken";
                tabAccountPage.Text = "Account";
                label4.Text = "Gebruikersnaam";
                label7.Text = "Accounttype";
                label38.Text = "Item uploaden";
                lblSettings3.Text = "Instellingen";

                lblSetPassword.Text = "Vereist wachtwoord";
                lblDescSetAuthSharing.Text = "Vraag om wachtwoord voordat mensen een bestand met je kunnen delen";

                lblDisableFileSharing.Text = "Bestandsdeling uitschakelen";
                lblDescDisableSharing.Text = "Als u bestanden delen uitschakelt, kunnen mensen geen bestand met u delen. U kunt echter nog steeds met mensen delen.";

                lblChangeMyPassword.Text = "Mijn wachtwoord wijzigen";
                lblDescChangeAuth.Text = "Wijzig het wachtwoord van uw Flowstorage-account";

                lblDeleteMyAccount.Text = "Verwijder mijn account";
                label3.Text = "Uw Flowstorage-account wordt samen met uw gegevens verwijderd";

                lblSettings.Text = "Instellingen";
                lblSettings2.Text = "Instellingen";

                label13.Text = "Aantal bestanden";
                label9.Text = "Aantal mappen";
                label11.Text = "Aantal mappen";
                label31.Text = "Totale upload vandaag";
                lblFile.Text = "Bestand";

                btnUpdatePassword.Text = "Wijzigen";
                btnDltAccount.Text = "Aanmelden";

                Form_1.lblUpload.Text = "Uploaden";
                Form_1.label2.Text = "Aantal artikelen";
                Form_1.btnUploadFile.Text = "Bestand uploaden";
                Form_1.btnUploadFolder.Text = "Map uploaden";
                Form_1.btnCreateDirectory.Text = "Directory aanmaken";
                Form_1.btnFileSharing.Text = "Bestanden delen";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "Essentials";
            }

            if (_custLang == "RUS") {
                lblSettings4.Text = "Настройки";
                label75.Text = "электронная почта";
                tabSharingPage.Text = "Общий доступ к файлам и";
                tabLangPage.Text = "Языки";
                tabUpgradePage.Text = "Обновить";
                tabStatsPage.Text = "Статистика";
                tabAccountPage.Text = "Учетная запись";
                label4.Text = "Имя пользователя";
                label7.Text = "Тип учетной записи";
                label38.Text = "Загрузка элемента";
                lblSettings3.Text = "Настройки";

                lblSetPassword.Text = "Требуемый пароль";
                lblDescSetAuthSharing.Text = "Запрашивайте пароль, прежде чем люди смогут поделиться с вами файлом";

                lblDisableFileSharing.Text = "Отключить общий доступ к файлам";
                lblDescDisableSharing.Text = "Отключение общего доступа к файлам не позволит людям делиться файлами с вами. Однако вы все равно можете делиться с другими людьми.";

                lblChangeMyPassword.Text = "Изменить мой пароль";
                lblDescChangeAuth.Text = "Измените пароль своей учетной записи Flowstorage";

                lblDeleteMyAccount.Text = "Удалить мою учетную запись";
                label3.Text = "Ваша учетная запись Flowstorage вместе с вашими данными будет удалена";

                lblSettings.Text = "Настройки";
                lblSettings2.Text = "Настройки";

                label13.Text = "Счетчик файлов";
                label9.Text = "Счетчик каталогов";
                label11.Text = "Счетчик папок";
                label31.Text = "Всего сегодня загружено";
                lblFile.Text = "Файл";

                btnUpdatePassword.Text = "Изменить";
                btnDltAccount.Text = "Удалить аккаунт";

                Form_1.lblUpload.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.btnUploadFile.Text = "Загрузить файл";
                Form_1.btnUploadFolder.Text = "Загрузить папку";
                Form_1.btnCreateDirectory.Text = "Создать каталог";
                Form_1.btnFileSharing.Text = "Общий доступ к файлам";
                Form_1.btnFileSharing.Size = new Size(129, 47);
                Form_1.lblEssentials.Text = "Основные";
            }

            if (_custLang == "GER") {
                lblSettings4.Text = "Einstellungen";
                label75.Text = "E-Mail-Addresse";
                tabSharingPage.Text = "Datenaustausch";
                tabLangPage.Text = "Sprachen";
                tabUpgradePage.Text = "Aktualisierung";
                tabStatsPage.Text = "Statistiken";
                tabAccountPage.Text = "Konto";
                label4.Text = "Nutzername";
                label7.Text = "Konto Typ";
                label38.Text = "Artikel hochladen";

                lblSettings3.Text = "Einstellungen";

                lblSetPassword.Text = "Erforderliches Passwort";
                lblDescSetAuthSharing.Text = "Nach dem Passwort fragen, bevor andere eine Datei für Sie freigeben können";

                lblDisableFileSharing.Text = "Dateifreigabe deaktivieren";
                lblDescDisableSharing.Text = "Das Deaktivieren der Dateifreigabe erlaubt anderen nicht, eine Datei mit Ihnen zu teilen. Sie können sie jedoch immer noch mit anderen teilen.";

                lblChangeMyPassword.Text = "Ändere mein Passwort";
                lblDescChangeAuth.Text = "Ändern Sie das Passwort Ihres Flowstorage-Kontos";

                lblDeleteMyAccount.Text = "Mein Konto löschen";
                label3.Text = "Ihr Flowstorage-Konto wird zusammen mit Ihren Daten gelöscht";

                lblSettings.Text = "Einstellungen";
                lblSettings2.Text = "Einstellungen";

                label13.Text = "Files Count";
                label9.Text = "Directory Count";
                label11.Text = "Folders Count";
                label31.Text = "Total Upload Today";
                lblFile.Text = "File";

                btnUpdatePassword.Text = "Ändern";
                btnDltAccount.Text = "Konto Löschen";

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
                lblSettings4.Text = "設定";
                tabSharingPage.Text = "ファイル共有";
                tabLangPage.Text = "言語";
                tabUpgradePage.Text = "アップグレード";
                tabStatsPage.Text = "統計";
                tabAccountPage.Text = "アカウント";
                label4.Text = "ユーザー名";
                label7.Text = "口座の種類";
                label38.Text = "アイテムのアップロード";

                lblSettings3.Text = "設定";

                lblSetPassword.Text = "必要なパスワード";
                lblDescSetAuthSharing.Text = "ファイルを共有する前にパスワードを要求する";

                lblDisableFileSharing.Text = "ファイル共有を無効にする";
                lblDescDisableSharing.Text = "ファイル共有を無効にすると、他のユーザーがファイルを共有することはできなくなります。ただし、他のユーザーと共有することはできます。";

                lblChangeMyPassword.Text = "パスワードを変更する";
                lblDescChangeAuth.Text = "Flowstorage アカウントのパスワードを変更する";

                lblDeleteMyAccount.Text = "アカウントを削除します";
                label3.Text = "Flowstorage アカウントとデータが削除されます";

                lblSettings.Text = "設定";
                lblSettings2.Text = "設定";

                label13.Text = "ファイル数";
                label9.Text = "ディレクトリ数";
                label11.Text = "フォルダ数";
                label31.Text = "今日の合計アップロード";
                lblFile.Text = "ファイル";

                btnUpdatePassword.Text = "変化";
                btnDltAccount.Text = "アカウントを削除する";

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
                lblSettings4.Text = "Ajustes";
                label75.Text = "Correo electrónico";
                tabSharingPage.Text = "Compartición de archivos";
                tabLangPage.Text = "Idiomas";
                tabUpgradePage.Text = "Mejora";
                tabStatsPage.Text = "Estadísticas";
                tabAccountPage.Text = "Cuenta";
                label4.Text = "Nombre de usuario";
                label7.Text = "Tipo de cuenta";
                label38.Text = "Carga de artículo";

                lblSettings3.Text = "Configuración";

                lblSetPassword.Text = "Contraseña requerida";
                lblDescSetAuthSharing.Text = "Solicite la contraseña antes de que la gente pueda compartir un archivo con usted";

                lblDisableFileSharing.Text = "Desactivar uso compartido de archivos";
                lblDescDisableSharing.Text = "Deshabilitar el uso compartido de archivos no permitirá que las personas compartan un archivo contigo. Sin embargo, aún puedes compartirlo con otras personas.";

                lblChangeMyPassword.Text = "cambiar mi contraseña";
                lblDescChangeAuth.Text = "Cambiar la contraseña de su cuenta Flowstorage";

                lblDeleteMyAccount.Text = "Borrar mi cuenta";
                label3.Text = "Se eliminará su cuenta de Flowstorage junto con sus datos";

                lblSettings.Text = "Ajustes";
                lblSettings2.Text = "Ajustes";

                label13.Text = "Recuento de archivos";
                label9.Text = "Recuento de directorios";
                label11.Text = "Número de carpetas";
                label31.Text = "Subida total hoy";
                lblFile.Text = "Archivo";

                btnUpdatePassword.Text = "Cambiar";
                btnDltAccount.Text = "Borrar cuenta";

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
                lblSettings4.Text = "Paramètres";
                tabSharingPage.Text = "Partage de fichiers";
                tabLangPage.Text = "Langages";
                tabUpgradePage.Text = "Améliorer";
                tabStatsPage.Text = "Statistiques";
                tabAccountPage.Text = "Compte";
                label4.Text = "Nom d'utilisateur";
                label7.Text = "Type de compte";
                label38.Text = "Téléchargement de l'article";

                lblSettings3.Text = "Paramètres";

                lblSetPassword.Text = "Mot de passe requis";
                lblDescSetAuthSharing.Text = "Demandez un mot de passe avant que les gens puissent partager un fichier avec vous";

                lblDisableFileSharing.Text = "Désactiver le partage de fichiers";
                lblDescDisableSharing.Text = "La désactivation du partage de fichiers ne permettra pas aux autres de partager un fichier avec vous. Cependant, vous pouvez toujours partager avec d'autres personnes.";

                lblChangeMyPassword.Text = "Changer mon mot de passe";
                lblDescChangeAuth.Text = "Modifier le mot de passe de votre compte Flowstorage";

                lblDeleteMyAccount.Text = "Supprimer mon compte";
                label3.Text = "Votre compte Flowstorage ainsi que vos données seront supprimés";

                lblSettings.Text = "Paramètres";
                lblSettings2.Text = "Paramètres";

                label13.Text = "Nombre de fichiers";
                label9.Text = "Nombre de répertoires";
                label11.Text = "Nombre de dossiers";
                label31.Text = "Téléchargement total aujourd'hui";
                lblFile.Text = "Déposer";

                btnUpdatePassword.Text = "Changement";
                btnDltAccount.Text = "Supprimer le compte";

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
                lblSettings4.Text = "Configurações";
                tabSharingPage.Text = "Compartilhamento de arquivos";
                tabLangPage.Text = "línguas";
                tabUpgradePage.Text = "Atualizar";
                tabStatsPage.Text = "Estatisticas";
                tabAccountPage.Text = "Conta";
                label4.Text = "Nome de usuário";
                label7.Text = "tipo de conta";
                label38.Text = "Carregamento de item";

                lblSettings3.Text = "Configurações";

                lblSetPassword.Text = "Senha Necessária";
                lblDescSetAuthSharing.Text = "Pedir senha antes que as pessoas possam compartilhar um arquivo com você";

                lblDisableFileSharing.Text = "Desativar Compartilhamento de Arquivos";
                lblDescDisableSharing.Text = "Desativar o compartilhamento de arquivos não permitirá que as pessoas compartilhem um arquivo com você. No entanto, você ainda pode compartilhar com outras pessoas.";

                lblChangeMyPassword.Text = "Mudar minha senha";
                lblDescChangeAuth.Text = "Altere a senha da sua conta Flowstorage";

                lblDeleteMyAccount.Text = "Deletar minha conta";
                label3.Text = "Sua conta Flowstorage junto com seus dados serão excluídos";

                lblSettings.Text = "Configurações";
                lblSettings2.Text = "Configurações";

                label13.Text = "Contagem de arquivos";
                label9.Text = "Contagem de diretório";
                label11.Text = "Contagem de Pastas";
                label31.Text = "Carregamento total hoje";
                lblFile.Text = "Arquivo";

                btnUpdatePassword.Text = "Mudar";
                btnDltAccount.Text = "Deletar conta";

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
                lblSettings4.Text = "设置";
                tabSharingPage.Text = "文件共享";
                tabLangPage.Text = "语言";
                tabUpgradePage.Text = "升级";
                tabStatsPage.Text = "统计数据";
                tabAccountPage.Text = "帐户";
                label4.Text = "用户名";
                label7.Text = "帐户类型";
                label38.Text = "项目上传";

                lblSettings3.Text = "设置";

                lblSetPassword.Text = "需要密码";
                lblDescSetAuthSharing.Text = "在别人分享文件给你之前要求输入密码";

                lblDisableFileSharing.Text = "禁用文件共享";
                lblDescDisableSharing.Text = "禁用文件共享将不允许其他人与您共享文件。但是您仍然可以与其他人共享。";

                lblChangeMyPassword.Text = "修改我的密码";
                lblDescChangeAuth.Text = "更改您的流存账户密码";

                lblDeleteMyAccount.Text = "删除我的账户";
                label3.Text = "您的 Flowstorage 帐户以及您的数据将被删除";

                lblSettings.Text = "设置";
                lblSettings2.Text = "设置";

                label13.Text = "文件数";
                label9.Text = "目录计数";
                label11.Text = "文件夹数";
                label31.Text = "今日上传总量";
                lblFile.Text = "文件";
                lblSettings3.Text = "设置";

                btnUpdatePassword.Text = "改变";
                btnDltAccount.Text = "删除帐户";

                Form_1.lblUpload.Text = "上传";
                Form_1.label2.Text = "物品数量";
                Form_1.btnUploadFile.Text = "上传文件";
                Form_1.btnUploadFolder.Text = "上传文件夹";
                Form_1.btnCreateDirectory.Text = "创建目录";
                Form_1.btnFileSharing.Text = "文件共享";
                Form_1.btnFileSharing.Size = new Size(125, 47);
                Form_1.lblEssentials.Text = "要点";
            }
        }

        private async Task GetCurrentLanguage() {

            if(Globals.currentLanguage == String.Empty) {

                const string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(_selectLang, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);

                    using (MySqlDataReader _readLang = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                        if (await _readLang.ReadAsync()) {
                            _currentUserLanguage = _readLang.GetString(0);
                            Globals.currentLanguage = _readLang.GetString(0);
                        }
                    }
                }
            } else {
                _currentUserLanguage = Globals.currentLanguage;
            }
        }

        private void UpdateLanguage(String _custLang) {

            const string updateLangQuery = "UPDATE lang_info SET CUST_LANG = @lang WHERE CUST_USERNAME = @username";

            using (MySqlCommand command = new MySqlCommand(updateLangQuery, con)) {
                command.Parameters.AddWithValue("@lang", _custLang);
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.ExecuteNonQuery();
            }
        }

        private void LanguageChanger(String _custLang) {

            if (_custLang == "US") {
                btnSetLangUS.Text = "Default";
                btnSetLangUS.ForeColor = Color.Gainsboro;
                btnSetLangUS.Enabled = false;
                UpdateLanguage("US");
                InitializeUILanguage("US");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "MY") {
                btnSetLangMY.Text = "Default";
                btnSetLangMY.ForeColor = Color.Gainsboro;
                btnSetLangMY.Enabled = false;
                UpdateLanguage("MY");
                InitializeUILanguage("MY");

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "JAP") {
                btnSetLangJAP.Text = "Default";
                btnSetLangJAP.ForeColor = Color.Gainsboro;
                btnSetLangJAP.Enabled = false;
                UpdateLanguage("JAP");
                InitializeUILanguage("JAP");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "GER") {
                guna2Button15.Text = "Default";
                guna2Button15.ForeColor = Color.Gainsboro;
                guna2Button15.Enabled = false;
                UpdateLanguage("GER");
                InitializeUILanguage("GER");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;

            }

            if (_custLang == "ESP") {
                guna2Button16.Text = "Default";
                guna2Button16.ForeColor = Color.Gainsboro;
                guna2Button16.Enabled = false;
                UpdateLanguage("ESP");
                InitializeUILanguage("ESP");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "FRE") {
                guna2Button20.Text = "Default";
                guna2Button20.ForeColor = Color.Gainsboro;
                guna2Button20.Enabled = false;
                UpdateLanguage("FRE");
                InitializeUILanguage("FRE");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "POR") {
                guna2Button21.Text = "Default";
                guna2Button21.ForeColor = Color.Gainsboro;
                guna2Button21.Enabled = false;
                UpdateLanguage("POR");
                InitializeUILanguage("POR");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "CHI") {
                guna2Button22.Text = "Default";
                guna2Button22.ForeColor = Color.Gainsboro;
                guna2Button22.Enabled = false;
                UpdateLanguage("CHI");
                InitializeUILanguage("CHI");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

            if (_custLang == "RUS") {
                guna2Button31.Text = "Default";
                guna2Button31.ForeColor = Color.Gainsboro;
                guna2Button31.Enabled = false;
                UpdateLanguage("RUS");
                InitializeUILanguage("RUS");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button30.Text = "Set as default";
                guna2Button30.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button30.Enabled = true;
            }

            if (_custLang == "DUT") {
                guna2Button30.Text = "Default";
                guna2Button30.ForeColor = Color.Gainsboro;
                guna2Button30.Enabled = false;
                UpdateLanguage("DUT");
                InitializeUILanguage("DUT");

                btnSetLangMY.Text = "Set as default";
                btnSetLangMY.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangMY.Enabled = true;

                btnSetLangUS.Text = "Set as default";
                btnSetLangUS.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangUS.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                btnSetLangJAP.Text = "Set as default";
                btnSetLangJAP.ForeColor = Color.FromArgb(55, 0, 179);
                btnSetLangJAP.Enabled = true;

                guna2Button15.Text = "Set as default";
                guna2Button15.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button15.Enabled = true;

                guna2Button20.Text = "Set as default";
                guna2Button20.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button20.Enabled = true;

                guna2Button21.Text = "Set as default";
                guna2Button21.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button21.Enabled = true;

                guna2Button22.Text = "Set as default";
                guna2Button22.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button22.Enabled = true;

                guna2Button31.Text = "Set as default";
                guna2Button31.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button31.Enabled = true;
            }

        }

        private void guna2Button18_Click(object sender, EventArgs e) {
            LanguageChanger("MY");
            Globals.currentLanguage = "MY";
            _newSelectedUserLanguage = "MY";
            SetupUIGreeting();
        }

        private void guna2Button17_Click(object sender, EventArgs e) {
            LanguageChanger("JAP");
            Globals.currentLanguage = "JAP";
            _newSelectedUserLanguage = "JAP";
            SetupUIGreeting();
        }

        private void guna2Button19_Click(object sender, EventArgs e) {
            LanguageChanger("US");
            Globals.currentLanguage = "US";
            _newSelectedUserLanguage = "US";
            SetupUIGreeting();
        }

        private void guna2Button31_Click(object sender, EventArgs e) {
            LanguageChanger("RUS");
            Globals.currentLanguage = "RUS";
            _newSelectedUserLanguage = "RUS";
            SetupUIGreeting();
        }

        private void guna2Button30_Click(object sender, EventArgs e) {
            LanguageChanger("DUT");
            Globals.currentLanguage = "DUT";
            _newSelectedUserLanguage = "DUT";
            SetupUIGreeting();
        }

        private void guna2Button16_Click(object sender, EventArgs e) {
            LanguageChanger("ESP");
            Globals.currentLanguage = "ESP";
            _newSelectedUserLanguage = "ESP";
            SetupUIGreeting();
        }

        private void guna2Button20_Click(object sender, EventArgs e) {
            LanguageChanger("FRE");
            Globals.currentLanguage = "FRE";
            _newSelectedUserLanguage = "FRE";
            SetupUIGreeting();
        }

        private void guna2Button21_Click(object sender, EventArgs e) {
            LanguageChanger("POR");
            Globals.currentLanguage = "POR";
            _newSelectedUserLanguage = "POR";
            SetupUIGreeting();
        }

        private void guna2Button22_Click(object sender, EventArgs e) {
            LanguageChanger("CHI");
            Globals.currentLanguage = "CHI";
            _newSelectedUserLanguage = "CHI";
            SetupUIGreeting();
        }

        private void guna2Button15_Click(object sender, EventArgs e) {
            LanguageChanger("GER");
            Globals.currentLanguage = "GER";
            _newSelectedUserLanguage = "GER";
            SetupUIGreeting();
        }

        #endregion END - UI section

        private void guna2Panel20_Paint(object sender, PaintEventArgs e) {

        }

        private void label56_Click(object sender, EventArgs e) {

        }

        private void tabPage3_Click(object sender, EventArgs e) {

        }

        private void tabPage5_Click(object sender, EventArgs e) {
        }

        /// <summary>
        /// If user selected File Sharing tab page
        /// then retrieve their current file sharing information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2TabControl1_Click(object sender, EventArgs e) {

            try {

                if (tabControlSettings.SelectedIndex == 3) {
                    if (_currentUserLanguage == "US") {
                        btnSetLangUS.Text = "Default";
                        btnSetLangUS.Enabled = false;
                        btnSetLangUS.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "MY") {
                        btnSetLangMY.Text = "Default";
                        btnSetLangMY.Enabled = false;
                        btnSetLangMY.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "JAP") {
                        btnSetLangJAP.Text = "Default";
                        btnSetLangJAP.Enabled = false;
                        btnSetLangJAP.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "GER") {
                        guna2Button15.Text = "Default";
                        guna2Button15.Enabled = false;
                        guna2Button15.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "ESP") {
                        guna2Button16.Text = "Default";
                        guna2Button16.Enabled = false;
                        guna2Button16.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "FRE") {
                        guna2Button20.Text = "Default";
                        guna2Button20.Enabled = false;
                        guna2Button20.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "CHI") {
                        guna2Button22.Text = "Default";
                        guna2Button22.Enabled = false;
                        guna2Button22.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "POR") {
                        guna2Button21.Text = "Default";
                        guna2Button21.Enabled = false;
                        guna2Button21.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "RUS") {
                        guna2Button31.Text = "Default";
                        guna2Button31.Enabled = false;
                        guna2Button31.ForeColor = Color.Gainsboro;
                    }

                    if (_currentUserLanguage == "DUT") {
                        guna2Button30.Text = "Default";
                        guna2Button30.Enabled = false;
                        guna2Button30.ForeColor = Color.Gainsboro;
                    }
                }

                if (tabControlSettings.SelectedIndex == 2) {

                    if(Globals.sharingDisabledStatus == String.Empty 
                    && Globals.sharingAuthStatus == String.Empty) {
                        Globals.sharingDisabledStatus = RetrieveIsSharingDisabled();
                        Globals.sharingAuthStatus = RetrieveFileSharingAuth();
                    }

                    if (Globals.sharingAuthStatus != "DEF") {

                        btnRmvSharingAuth.Visible = true;
                        btnRmvSharingAuth.Enabled = true;

                        btnAddSharingAuth.Visible = false;
                        btnAddSharingAuth.Enabled = false;
                    }
                    else {

                        btnRmvSharingAuth.Visible = false;
                        btnRmvSharingAuth.Enabled = false;

                        btnAddSharingAuth.Visible = true;
                        btnAddSharingAuth.Enabled = true;
                    }

                    if (Globals.sharingDisabledStatus == "1") {
                        btnDisableSharing.Enabled = false;
                        btnDisableSharing.Visible = false;

                        btnEnableSharing.Visible = true;
                        btnEnableSharing.Enabled = true;

                        lblDisableFileSharing.Text = "Enable File Sharing";
                        lblDescDisableSharing.Text = "Enabling file sharing will allows people to share a file to you";

                    }
                    else {
                        btnDisableSharing.Enabled = true;
                        btnDisableSharing.Visible = true;

                        btnEnableSharing.Visible = false;
                        btnEnableSharing.Enabled = false;

                        lblDisableFileSharing.Text = "Disable File Sharing";
                        lblDescDisableSharing.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";
                    }
                }

            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        private void guna2Panel14_Paint(object sender, PaintEventArgs e) {

        }

        private void label34_Click_1(object sender, EventArgs e) {

        }

        private void label27_Click_2(object sender, EventArgs e) {

        }

        private void guna2Panel13_Paint(object sender, PaintEventArgs e) {

        }

        private void label22_Click(object sender, EventArgs e) {

        }
        private void label58_Click(object sender, EventArgs e) {

        }

        private void label5_Click_1(object sender, EventArgs e) {

        }

        private void label66_Click(object sender, EventArgs e) {

        }

        private void lblSetPassword_Click(object sender, EventArgs e) {

        }

        private void label18_Click(object sender, EventArgs e) {

        }

        private void label12_Click(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void label39_Click(object sender, EventArgs e) {

        }
    }
}
