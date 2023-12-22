using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Settings;
using FlowstorageDesktop.SharingQuery;
using FlowstorageDesktop.Temporary;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {

    public partial class SettingsForm : Form {

        static public SettingsForm instance;

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private SharingOptionsQuery sharingOptions = new SharingOptionsQuery();
        readonly private CurrencyConverter currencyConverter = new CurrencyConverter();

        readonly private TemporaryDataSharing tempDataSharing = new TemporaryDataSharing();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private List<int> _totalUploadToday { get; set; } = new List<int>();
        private List<int> _totalUploadAllTime { get; set; } = new List<int>();
        private string _selectedAccountType { get; set; }
        public SettingsForm() {

            InitializeComponent();

            ClosePopupForm.CloseCustomPopup("SettingsLoadingAlert");

            instance = this;

            lblUserEmail.Text = tempDataUser.Email;
            lblUserUsername.Text = tempDataUser.Username;

            foreach (var axis in chart1.ChartAreas[0].Axes) {
                axis.MajorGrid.Enabled = false;
                axis.MinorGrid.Enabled = false;
            }

            if(tempDataUser.AccountType != "Basic") {
                pnlCancelPlan.Visible = true;
            }

            InitializeSettingsAsync();
        }

        private async void InitializeSettingsAsync() {

            try {

                InitiailizeUIOnAccountType(lblAccountType.Text);
                InitializeUploadLimitLabel();

                chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                string[] halfTablesName = { "info_image", "info_text", "info_video", "info_pdf", "info_apk", "info_exe", "info_word", "info_ptx", "info_audio", "info_excel" };
                string[] chartXAxisValues = { "Image", "Text", "Video", "PDF", "APK", "Exe", "Document", "Presentation", "Audio", "Excel" };

                foreach ((string tableName, string chartName) in halfTablesName.Zip(chartXAxisValues, (a, b) => (a, b))) {
                    await GenerateUploadChart(chartName, "file_" + tableName.ToLower());
                    await TotalUploadFileTodayCount("file_" + tableName.ToLower());
                    await TotalUploadFile("file_" + tableName.ToLower());
                }

                await TotalUploadDirectoryCount();

                int _totalUploadTodayCount = _totalUploadToday.Sum(x => Convert.ToInt32(x));
                lblCountFileUploadToday.Text = _totalUploadTodayCount.ToString();

                int _totalUploadOvertime = _totalUploadAllTime.Sum(x => Convert.ToInt32(x));
                lblTotalUploadFileCount.Text = _totalUploadOvertime.ToString();

            } catch (Exception) {
                new CustomAlert(
                    title: "An error occurred", "Something went wrong while trying to open Settings.");

            }
        }

        #region Sharing section

        private async void guna2Button27_Click(object sender, EventArgs e) {

            if (MessageBox.Show("Remove File Sharing password?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                await sharingOptions.RemoveSharingAuth();

                btnAddSharingAuth.Visible = true;
                btnAddSharingAuth.Enabled = true;

                tempDataSharing.SharingAuthStatus = "DEF";

                btnRmvSharingAuth.Visible = false;
                btnRmvSharingAuth.Enabled = false;
            }
        }

        /// <summary>
        /// Disable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button24_Click(object sender, EventArgs e) {

            if (MessageBox.Show("Disable file sharing? You can always enable this option again later.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                btnDisableSharing.Enabled = false;
                btnDisableSharing.Visible = false;

                btnEnableSharing.Visible = true;
                btnEnableSharing.Enabled = true;

                lblDisableFileSharing.Text = "Enable File Sharing";
                lblDescDisableSharing.Text = "Enabling file sharing will allows people to share a file to you";

                tempDataSharing.SharingDisabledStatus = "1";

                await sharingOptions.DisableSharing();

            }
        }

        /// <summary>
        /// Enable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button26_Click(object sender, EventArgs e) {

            btnDisableSharing.Enabled = true;
            btnDisableSharing.Visible = true;

            btnEnableSharing.Visible = false;
            btnEnableSharing.Enabled = false;

            lblDisableFileSharing.Text = "Disable File Sharing";
            lblDescDisableSharing.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";

            tempDataSharing.SharingDisabledStatus = "0";

            await sharingOptions.EnableSharing();

        }

        #endregion END - Sharing section

        #region User statistics section

        /// <summary>
        /// This function will tells user how many files
        /// they have uploaded (in total)
        /// </summary>
        /// <param name="_tableName"></param>
        private async Task TotalUploadFile(string tableName) {

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if(origin == "Home") {

                if (tableName == GlobalsTable.homeImageTable) {

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
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    int totalUploadCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _totalUploadAllTime.Add(totalUploadCount);
                }

            }
        }

        private async Task TotalUploadFileTodayCount(string tableName) {

            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if (origin == "Home") {
                    
                if(tableName == GlobalsTable.homeImageTable) {

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
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@date", currentDate);

                    int totalCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                    _totalUploadToday.Add(totalCount);
                }
            }

        }

        private async Task TotalUploadDirectoryCount() {

            string origin = HomePage.instance.lblCurrentPageText.Text;

            if (origin == "Home") {
                var directoriesName = new HashSet<string>(HomePage.instance.flwLayoutHome.Controls
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
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    int totalDir = Convert.ToInt32(await command.ExecuteScalarAsync());
                    lblTotalDirUploadCount.Text = totalDir.ToString();
                }

            }

            int countTotalFolders = HomePage.instance.lstFoldersPage.Items.Count;
            lblTotalFolderUploadCount.Text = countTotalFolders.ToString();

        }


        /// <summary>
        /// This will generates statistical chart
        /// that tells user how many files they've
        /// uploaded by date
        /// </summary>
        /// <param name="_serName"></param>
        /// <param name="_tableName"></param>

        private async Task GenerateUploadChart(string seriesName, string tableName) {

            string querySelectDate = $"SELECT UPLOAD_DATE, COUNT(UPLOAD_DATE) FROM {tableName} WHERE CUST_USERNAME = @username GROUP BY UPLOAD_DATE HAVING COUNT(UPLOAD_DATE) > 0";

            using (MySqlCommand command = new MySqlCommand(querySelectDate, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);

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


        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button3_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button25_Click_1(object sender, EventArgs e) => this.Close();

        private void guna2Button1_Click(object sender, EventArgs e) => new RemoveAccountForm().Show();

        private void guna2Button23_Click(object sender, EventArgs e) => new AddAuthSharing().Show();
        private void guna2Button12_Click(object sender, EventArgs e) => new ChangeAuthForm().Show();
        private void guna2Button1_Click_2(object sender, EventArgs e) => new BackupRecoveryKeyForm().Show();
        private void label5_Click(object sender, EventArgs e) => Clipboard.SetText(tempDataUser.Username);
        private void label76_Click(object sender, EventArgs e) => Clipboard.SetText(tempDataUser.Email);
        private void guna2Button2_Click_2(object sender, EventArgs e) => new CancelPlanForm().Show();

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

        public void InitiailizeUIOnAccountType(string selectedAcc) {

            if (selectedAcc == "Supreme") {
                btnOpenExpressPayment.Enabled = false;
                btnOpenSupremePayment.Enabled = false;
                btnOpenMaxPayment.Enabled = false;
                btnUseSupreme.Visible = false;
                lblLimitedUpload.Text = "Limited to 2000";

            } else if (selectedAcc == "Express") {
                btnOpenExpressPayment.Enabled = false;
                btnOpenMaxPayment.Enabled = false;
                btnUseExpress.Visible = false;
                lblLimitedUpload.Text = "Limited to 800";

            } else if (selectedAcc == "Max") {
                btnOpenMaxPayment.Enabled = false;
                btnUseMax.Visible = false;
                lblLimitedUpload.Text = "Limited to 150";

            }  else if (selectedAcc == "Basic") {
                lblLimitedUpload.Text = "Limited to 25";

            }

        }

        private void InitializeUploadLimitLabel() {

            string accountType = tempDataUser.AccountType;

            lblAccountType.Text = accountType;

            if (accountType == "Basic") {
                lblLimitedUpload.Text = "Limited to 25";
                
            } else if (accountType == "Max") {
                lblLimitedUpload.Text = "Limited to 150";
                btnOpenMaxPayment.Enabled = false;

            } else if (accountType == "Express") {
                lblLimitedUpload.Text = "Limited to 800";
                btnOpenMaxPayment.Enabled = false;
                btnOpenExpressPayment.Enabled = false;

            } else if (accountType == "Supreme") {
                lblLimitedUpload.Text = "Limited to 2000";
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

                if (custEmails.Contains(tempDataUser.Email)) {

                    var options = new Stripe.CustomerListOptions {
                        Email = tempDataUser.Email,
                        Limit = 1
                    };

                    var customersOptions = service.List(options);

                    var customerBuyer = customersOptions.Data[0];
                    var customerId = customerBuyer.Id;

                    const string updateUserAccountQuery = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(updateUserAccountQuery, con)) {
                        command.Parameters.AddWithValue("@username", tempDataUser.Username);
                        command.Parameters.AddWithValue("@email", tempDataUser.Email);
                        command.Parameters.AddWithValue("@type", _selectedAccountType);
                        await command.ExecuteNonQueryAsync();
                    }


                    const string insertBuyerQuery = "INSERT INTO cust_buyer(CUST_USERNAME,CUST_EMAIL,ACC_TYPE,CUST_ID,PURCHASE_DATE) VALUES (@username,@email,@type,@id,@date)";
                    using (MySqlCommand commandSecond = new MySqlCommand(insertBuyerQuery, con)) {
                        commandSecond.Parameters.AddWithValue("@username", tempDataUser.Username);
                        commandSecond.Parameters.AddWithValue("@email", tempDataUser.Email);
                        commandSecond.Parameters.AddWithValue("@type", _selectedAccountType);
                        commandSecond.Parameters.AddWithValue("@id", customerId);
                        commandSecond.Parameters.AddWithValue("@date", dateTimeNow);

                        await commandSecond.ExecuteNonQueryAsync();
                    }

                    lblAccountType.Text = _selectedAccountType;
                    tempDataUser.AccountType = _selectedAccountType;

                    new PaymentSuceededAlert(_selectedAccountType).Show();

                    InitiailizeUIOnAccountType(_selectedAccountType);

                } else {
                    new CustomAlert(
                        title: "Cannot proceed",
                        subheader: "You have to make a payment on the web first to use this plan.").Show();
                }

            } catch (Exception) {
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
        private async void guna2TabControl1_Click(object sender, EventArgs e) {

            try {

                if (tabControlSettings.SelectedIndex == 2) {
                    if(tempDataSharing.SharingDisabledStatus == string.Empty && tempDataSharing.SharingAuthStatus == string.Empty) {

                        tempDataSharing.SharingDisabledStatus = await sharingOptions
                            .RetrieveIsSharingDisabled(tempDataUser.Username);

                        tempDataSharing.SharingAuthStatus = await sharingOptions
                            .ReceiverHasAuthVerification(tempDataUser.Username);
                    }

                    if (tempDataSharing.SharingAuthStatus != "DEF") {

                        btnRmvSharingAuth.Visible = true;
                        btnRmvSharingAuth.Enabled = true;

                        btnAddSharingAuth.Visible = false;
                        btnAddSharingAuth.Enabled = false;

                    } else {

                        btnRmvSharingAuth.Visible = false;
                        btnRmvSharingAuth.Enabled = false;

                        btnAddSharingAuth.Visible = true;
                        btnAddSharingAuth.Enabled = true;
                    }

                    if (tempDataSharing.SharingDisabledStatus == "1") {
                        btnDisableSharing.Enabled = false;
                        btnDisableSharing.Visible = false;

                        btnEnableSharing.Visible = true;
                        btnEnableSharing.Enabled = true;

                        lblDisableFileSharing.Text = "Enable File Sharing";
                        lblDescDisableSharing.Text = "Enabling file sharing will allows people to share a file to you";

                    } else {
                        btnDisableSharing.Enabled = true;
                        btnDisableSharing.Visible = true;

                        btnEnableSharing.Visible = false;
                        btnEnableSharing.Enabled = false;

                        lblDisableFileSharing.Text = "Disable File Sharing";
                        lblDescDisableSharing.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";
                    }
                }

                if(tabControlSettings.SelectedIndex == 3) {
                    await currencyConverter.ConvertToLocalCurrency();

                }

            } catch (Exception) { }

        }

        private void label22_Click(object sender, EventArgs e) {

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

        private void label41_Click(object sender, EventArgs e) {

        }

        private void label42_Click(object sender, EventArgs e) {

        }

        private void label39_Click_1(object sender, EventArgs e) {

        }

        private void label36_Click(object sender, EventArgs e) {

        }

        private void label72_Click(object sender, EventArgs e) {

        }

        private void label40_Click(object sender, EventArgs e) {

        }

        private void label8_Click_1(object sender, EventArgs e) {

        }

        private void lblSupremePricing_Click(object sender, EventArgs e) {

        }
    }
}
