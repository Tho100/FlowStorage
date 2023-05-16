using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Globalization;
using Ubiety.Dns.Core;
using Stripe.Infrastructure;
using System.Text.RegularExpressions;
using Stripe.Checkout;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
//using Stripe;

namespace FlowSERVER1 {
    /// <summary>
    /// Settings form class
    /// </summary>
    public partial class remAccFORM : Form {

        public static remAccFORM instance;
        public static string _selectedAcc;

        public int tokenCheckCurr = 0;

        private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;

        private List<int> _TotalUploadToday = new List<int>();
        private List<int> _TotalUploadOvertime = new List<int>();
        private List<string> _TotalUploadDirectoryToday = new List<string>();

        private string CurrentLang = "";
        private string NewLang = "";

        private int CurrDateStats = 0;
        private string JoinedDate = "";

        public remAccFORM(String _accName,String _emailAddr) {

            InitializeComponent();

            instance = this;
            label5.Text = _accName;

            int indexOfAllias = _emailAddr.IndexOf("@");
            string maskedEmail = _emailAddr.Substring(0, 1) + new String('*', indexOfAllias - 2) + _emailAddr.Substring(indexOfAllias - 1);
            label76.Text = maskedEmail;

            foreach (var axis in chart1.ChartAreas[0].Axes) {
                axis.MajorGrid.Enabled = false;
                axis.MinorGrid.Enabled = false;
            }

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(guna2Button11, "Item upload indicate how many file/directory you can upload.");

            initializeSettingsAsync();
        }

        private async void initializeSettingsAsync() {

            try {

                await getCurrentLang();
                setupUILanguage(CurrentLang);
                setupRedundane(label6.Text);
                await GetAccountType();
                await countTotalAll();

                chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

                string[] tableNames = { "info", "info_expand", "info_vid", "info_pdf", "info_apk", "info_exe", "info_gif", "info_word", "info_ptx", "info_audi", "info_excel" };
                string[] chartTypes = { "Image", "Text", "Video", "PDF", "APK", "Exe", "GIF", "Document", "Presentation", "Audio", "Excel" };
                foreach ((string tableName, string chartName) in tableNames.Zip(chartTypes, (a, b) => (a, b))) {
                    await generateChart(chartName, "file_" + tableName.ToLower());
                    await TotalUploadFileTodayCount("file_" + tableName.ToLower());
                    await TotalUploadFile("file_" + tableName.ToLower());
                }

                await TotalUploadDirectoryTodayCount();

                int _totalUploadTodayCount = _TotalUploadToday.Sum(x => Convert.ToInt32(x));
                label26.Text = _totalUploadTodayCount.ToString();

                int _totalUploadOvertime = _TotalUploadOvertime.Sum(x => Convert.ToInt32(x));
                label12.Text = _totalUploadOvertime.ToString();

            }
            catch (Exception) {
                using (var bgBlur = new Form {
                    StartPosition = FormStartPosition.Manual,
                    FormBorderStyle = FormBorderStyle.None,
                    Opacity = .24d,
                    BackColor = Color.Black,
                    WindowState = FormWindowState.Maximized,
                    TopMost = true,
                    Location = this.Location,
                    ShowInTaskbar = false
                }) {
                    using (var displayWait = new waitFORM { Owner = bgBlur }) {
                        bgBlur.Show();
                        displayWait.ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// This function will retrieve the 
        /// current status of user file sharing (disabled, or enabled)
        /// </summary>
        private String retrieveDisabled(String _custUsername) {
            String querySelectDisabled = "SELECT DISABLED FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(querySelectDisabled, con)) {
                command.Parameters.AddWithValue("@username", _custUsername);

                string isEnabled = Convert.ToString(command.ExecuteScalar());
                string concludeOutput = isEnabled == "1" ? "1" : "0";
                return concludeOutput;
            }
        }

        /// <summary>
        /// This function will tells user how many files
        /// they have uploaded (in total)
        /// </summary>
        /// <param name="_tableName"></param>
        private async Task TotalUploadFile(String _tableName) {
            String countQuery = $"SELECT COUNT(*) FROM {_tableName} WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(countQuery, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                int totalCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                _TotalUploadOvertime.Add(totalCount);
            }
        }

        /// <summary>
        /// This function will tells user the number of 
        /// files they've uploaded a day
        /// </summary>
        /// <param name="_TableName"></param>
        private async Task TotalUploadFileTodayCount(String _TableName) {
            String currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            String queryCount = $"SELECT COUNT(CUST_USERNAME) FROM {_TableName} WHERE CUST_USERNAME = @username AND UPLOAD_DATE = @date";
            using (MySqlCommand command = new MySqlCommand(queryCount, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@date", currentDate);

                int totalCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                _TotalUploadToday.Add(totalCount);
            }
        }

        /// <summary>
        /// This function will tells user the number
        /// of directory they have created a day
        /// </summary>
        private async Task TotalUploadDirectoryTodayCount() {
            String currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            String querySelectName = "SELECT DIR_NAME FROM upload_info_directory WHERE CUST_USERNAME = @username AND UPLOAD_DATE = @date";
            using (MySqlCommand command = new MySqlCommand(querySelectName, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                command.Parameters.AddWithValue("@date", currentDate);

                using (MySqlDataReader reader = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        _TotalUploadDirectoryToday.Add(reader.GetString(0));
                    }
                }
            }

            int distinctDirCount = _TotalUploadDirectoryToday.Distinct().Count();
            label30.Text = distinctDirCount.ToString();
        }

        private async Task GetAccountType() {

            String accountType = "";
            String querySelectType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username LIMIT 1";
            using (MySqlCommand command = new MySqlCommand(querySelectType, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                accountType = Convert.ToString(await command.ExecuteScalarAsync());
                label6.Text = accountType;
            }

            if (accountType == "Basic") {
                if(CurrentLang == "US") {
                    label37.Text = "Limited to 20";
                } else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 20";
                } else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 20";
                } else if (CurrentLang == "JAP") {
                    label37.Text = "20 個限定";
                } else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 20";
                }
                else if (CurrentLang == "POR") {
                    label37.Text = "Limitado a 20";
                }

            }
            else if (accountType == "Max") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 500";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 500";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 500";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "500 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 500";
                }
                else if (CurrentLang == "POR") {
                    label37.Text = "Limitado a 500";
                }
                guna2Button5.Enabled = false;
            }
            else if (accountType == "Express") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 1000";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 1000";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 1000";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "1000 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 1000";
                }
                else if (CurrentLang == "POR") {
                    label37.Text = "Limitado a 450";
                }
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
            }
            else if (accountType == "Supreme") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 2000";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 2000";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 2000";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "2000 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 2000";
                }
                else if (CurrentLang == "POR") {
                    label37.Text = "Limitado a 2000";
                }
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
                guna2Button7.Enabled = false;
            }
        }
        private async Task countTotalAll() {

            String countDirQuery = "SELECT COUNT(*) FROM file_info_directory WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(countDirQuery, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);
                int totalDir = Convert.ToInt32(await command.ExecuteScalarAsync());
                label19.Text = totalDir.ToString();
            }

            int countTotalFolders = Form1.instance.listBox1.Items.Count - 3;
            label20.Text = countTotalFolders.ToString();
        }


        /// <summary>
        /// This will generates statistical chart
        /// that tells user how many files they've
        /// uploaded by date
        /// </summary>
        /// <param name="_serName"></param>
        /// <param name="_tableName"></param>

        private async Task generateChart(String _serName, String _tableName) {
            string querySelectDate = $"SELECT UPLOAD_DATE,COUNT(UPLOAD_DATE) FROM {_tableName} WHERE CUST_USERNAME = @username GROUP BY UPLOAD_DATE HAVING COUNT(UPLOAD_DATE) > 0";

            using (MySqlCommand command = new MySqlCommand(querySelectDate, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        string date = reader.GetString(0);
                        int count = reader.GetInt32(1);
                        chart1.Series[_serName].Points.AddXY(date, count);
                    }
                }
            }
        }

        private void remAccFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            ConfirmRemFORM con_Show = new ConfirmRemFORM();
            con_Show.Show();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void chart1_Click(object sender, EventArgs e) {

        }

        private void tabPage2_Click(object sender, EventArgs e) {
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {

            this.Close();
        }

        private void tabPage1_Click(object sender, EventArgs e) {

        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            
            try {
                
                DialogResult _confirmation = MessageBox.Show("Logout your account?","Flowstorage",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if(_confirmation == DialogResult.Yes) {

                    guna2Panel1.SendToBack();
                    Form1.instance.panel1.SendToBack();

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
                        Directory.Delete(_getPath, true);
                    }

                    Application.OpenForms["remAccFORM"].Close();
                    Form1.instance.guna2Panel7.Visible = true;
                    Form1.instance.listBox1.Items.Clear();
                }
            } catch (Exception) {
                MessageBox.Show("There's a problem while attempting to logout your account. Please try again.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void label42_Click(object sender, EventArgs e) {

        }

       



        private void guna2Button5_Click(object sender, EventArgs e) {
            _selectedAcc = "Max";
            guna2Button8.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/6oEbLLanh41c3gQ9AA"); // Live mode
        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void label37_Click(object sender, EventArgs e) {

        }

        private void label38_Click(object sender, EventArgs e) {

        }

        private void guna2Separator7_Click(object sender, EventArgs e) {

        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            _selectedAcc = "Supreme";
            guna2Button10.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/3csdTTeDxcxI2cMcMO"); // Test mode
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _selectedAcc = "Express";
            guna2Button9.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/6oEbLLanh41c3gQ9AA"); // Live mode
        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel10_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel3_Paint_1(object sender, PaintEventArgs e) {

        }

        private void label34_Click(object sender, EventArgs e) {

        }

        private void guna2Separator6_Click(object sender, EventArgs e) {

        }

        private void label30_Click(object sender, EventArgs e) {

        }

        private void label29_Click(object sender, EventArgs e) {

        }

        private void guna2Separator4_Click(object sender, EventArgs e) {

        }

        private void guna2Separator5_Click(object sender, EventArgs e) {

        }

        private void label26_Click(object sender, EventArgs e) {

        }

        private void label27_Click(object sender, EventArgs e) {

        }

        private void label28_Click(object sender, EventArgs e) {

        }

        private void label31_Click(object sender, EventArgs e) {

        }

        private void label28_Click_1(object sender, EventArgs e) {

        }

        private void label27_Click_1(object sender, EventArgs e) {

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

        void setupRedundane(String _selectedAcc) {
            if (_selectedAcc == "Supreme") {
                guna2Button6.Enabled = false;
                guna2Button7.Enabled = false;
                guna2Button5.Enabled = false;
                guna2Button10.Visible = false;
                label37.Text = "Limited to 2000";
            }
            else if (_selectedAcc == "Express") {
                guna2Button6.Enabled = false;
                guna2Button5.Enabled = false;
                guna2Button9.Visible = false;
                label37.Text = "Limited to 1000";
            }
            else if (_selectedAcc == "Max") {
                guna2Button5.Enabled = false;
                guna2Button8.Visible = false;
                label37.Text = "Limited to 500";
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
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private void setupAccount() {

            try {

                var _setupApiKey = DecryptApi("0afe74-gksuwpe8r", ConfigurationManager.ConnectionStrings["asfhuwdajdwdwpo=#k"].ConnectionString);
                Stripe.StripeConfiguration.SetApiKey(_setupApiKey);
                var service = new Stripe.CustomerService();
                var customers = service.List();

                String lastId = null;
                String LastEmail = null;

                List<String> _custEmails = new List<String>();

                // @ Enumerate the first page of the list
                foreach (Stripe.Customer customer in customers) {
                    _custEmails.Add(customer.Id);
                    lastId = customer.Id;
                    LastEmail = customer.Email;
                }

                List<String> CustUserValues = new List<String>();
                String _selectCustEmail = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(_selectCustEmail, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                MySqlDataReader _readEmail = command.ExecuteReader();
                if (_readEmail.Read()) {
                    CustUserValues.Add(_readEmail.GetString(0));
                }
                _readEmail.Close();

                if (_custEmails.Contains(CustUserValues[0])) {
                    String _insertNew = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                    command = new MySqlCommand(_insertNew, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@email", CustUserValues[0]);
                    command.Parameters.AddWithValue("@type", _selectedAcc);
                    if (command.ExecuteNonQuery() == 1) {

                        String _insertPayment = "INSERT INTO cust_buyer(CUST_USERNAME,CUST_EMAIL,ACC_TYPE,CUST_ID) VALUES (@username,@email,@type,@id)";
                        command = new MySqlCommand(_insertPayment, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@email", CustUserValues[0]);
                        command.Parameters.AddWithValue("@type", _selectedAcc);
                        command.Parameters.AddWithValue("@id", lastId);
                        command.ExecuteNonQuery();

                        // @ IF PAYMENT IS DONE THEN REMOVE THE CUSTOMER FROM DASHBOARD
                        var delService = new Stripe.CustomerService();
                        delService.Delete(lastId);

                        successPay _showSucceeded = new successPay(_selectedAcc);
                        _showSucceeded.Show();
                        label6.Text = _selectedAcc;
                        setupRedundane(_selectedAcc);
                    }
                } else {
                    noPayment _showFailed = new noPayment();
                    _showFailed.Show();
                }
                // @ ERROR OCCURS WHEN BUYER IS A GUEST ACCOUNT
            }
            catch (Exception) {
                noPayment _showFailed = new noPayment();
                _showFailed.Show();
            }
        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            setupAccount();
        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            setupAccount();
        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            setupAccount();
        }

        private void guna2Panel7_Paint_1(object sender, PaintEventArgs e) {

        }

        private void guna2Button13_Click(object sender, EventArgs e) {
            chagneUserForm _ShowUsernameChangerForm = new chagneUserForm(label5.Text);
            _ShowUsernameChangerForm.Show();
        }

        private void label9_Click(object sender, EventArgs e) {

        }

        private void label31_Click_1(object sender, EventArgs e) {

        }

        private void guna2Panel11_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button12_Click(object sender, EventArgs e) {
            resPasFORM _showChangePassForm = new resPasFORM(label5.Text);
            _showChangePassForm.Show();
        }

        private void label57_Click(object sender, EventArgs e) {

        }

        private void guna2Panel15_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button14_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void tabPage4_Click(object sender, EventArgs e) {

        }

        private void setupTime() {
            var form = Form1.instance;
            var lab1 = form.label1;
            var lab5 = form.label5;
       
            DateTime now = DateTime.Now;
            var hours = now.Hour;
            String greeting = null;
            if (hours >= 1 && hours <= 12) {
                if (NewLang == "US") {
                    greeting = "Good Morning, " + lab5.Text;
                }
                else if (NewLang == "MY") {
                    greeting = "Selemat Pagi, " + lab5.Text;
                }
                else if (NewLang == "GER") {
                    greeting = "Guten Morgen, " + lab5.Text;
                }
                else if (NewLang == "JAP") {
                    greeting = "おはよう " + lab5.Text + " :)";
                }
                else if (NewLang == "ESP") {
                    greeting = "Buen día " + lab5.Text + " :)";
                }
                else if (NewLang == "FRE") {
                    greeting = "Bonjour " + lab5.Text + " :)";
                }
                else if (NewLang == "POR") {
                    greeting = "Bom dia " + lab5.Text + " :)";
                }
                else if (NewLang == "CHI") {
                    greeting = "早上好 " + lab5.Text + " :)";
                }
                else if (NewLang == "RUS") {
                    greeting = "Доброе утро " + lab5.Text + " :)";
                }
                else if (NewLang == "DUT") {
                    greeting = "Goedemorgen " + lab5.Text + " :)";
                }
            }

            else if (hours >= 12 && hours <= 16) {
                if (NewLang == "US") {
                    greeting = "Good Afternoon " + lab5.Text + " :)";
                }
                else if (NewLang == "MY") {
                    greeting = "Selamat Petang " + lab5.Text + " :)";
                }
                else if (NewLang == "GER") {
                    greeting = "Guten Tag " + lab5.Text + " :)";
                }
                else if (NewLang == "JAP") {
                    greeting = "こんにちは " + lab5.Text + " :)";
                }
                else if (NewLang == "ESP") {
                    greeting = "Buenas tardes " + lab5.Text + " :)";
                }
                else if (NewLang == "FRE") {
                    greeting = "Bon après-midi " + lab5.Text + " :)";
                }
                else if (NewLang == "POR") {
                    greeting = "Boa tarde " + lab5.Text + " :)";
                }
                else if (NewLang == "CHI") {
                    greeting = "下午好 " + lab5.Text + " :)";
                }
                else if (NewLang == "RUS") {
                    greeting = "Добрый день " + lab5.Text + " :)";
                }
                else if (NewLang == "DUT") {
                    greeting = "Goedemiddag " + lab5.Text + " :)";
                }
            }
            else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (NewLang == "US") {
                        greeting = "Good Late Evening, " + lab5.Text;
                    }
                    else if (NewLang == "MY") {
                        greeting = "Selamat Lewat-Petang, " + lab5.Text;
                    } else if (NewLang == "GER") {
                        greeting = "Guten späten Abend, " + lab5.Text;
                    }
                    else if (NewLang == "JAP") {
                        greeting = "こんばんは " + lab5.Text + " :)";
                    }
                    else if (NewLang == "ESP") {
                        greeting = "buenas tardes " + lab5.Text + " :)";
                    }
                    else if (NewLang == "FRE") {
                        greeting = "bonne soirée " + lab5.Text + " :)";
                    }
                    else if (NewLang == "POR") {
                        greeting = "Boa noite " + lab5.Text + " :)";
                    }
                    else if (NewLang == "CHI") {
                        greeting = "晚上好 " + lab5.Text + " :)";
                    }
                    else if (NewLang == "RUS") {
                        greeting = "Добрый день " + lab5.Text + " :)";
                    }
                    else if (NewLang == "DUT") {
                        greeting = "Goedeavond " + lab5.Text + " :)";
                    }

                }
                else {
                    if (NewLang == "US") {
                        greeting = "Good Evening, " + lab5.Text;
                    }
                    else if (NewLang == "MY") {
                        greeting = "Selamat Petang, " + lab5.Text;
                    } else if (NewLang == "GER") {
                        greeting = "Guten Abend, " + lab5.Text;
                    } else if (NewLang == "JAP") {
                        greeting = "こんばんは " + lab5.Text + " :)";
                    }
                    else if (NewLang == "ESP") {
                        greeting = "Buenas terdes " + lab5.Text + " :)";
                    }
                    else if (NewLang == "FRE") {
                        greeting = "bonne soirée " + lab5.Text + " :)";
                    }
                    else if (NewLang == "POR") {
                        greeting = "Boa noite " + lab5.Text + " :)";
                    }
                    else if (NewLang == "CHI") {
                        greeting = "晚上好 " + lab5.Text + " :)";
                    }
                    else if (NewLang == "RUS") {
                        greeting = "Добрый вечер " + lab5.Text + " :)";
                    }
                    else if (NewLang == "DUT") {
                        greeting = "Goedeavond " + lab5.Text + " :)";
                    }
                }

            }
            else if (hours >= 21 && hours <= 24) {
                if (NewLang == "US") {
                    greeting = "Good Night, " + lab5.Text;
                }
                else if (NewLang == "MY") {
                    greeting = "Selamat Malam, " + lab5.Text;
                }
                else if (NewLang == "GER") {
                    greeting = "Guten Nacth, " + lab5.Text;
                }
                else if (NewLang == "JAP") {
                    greeting = "おやすみ " + lab5.Text + " :)";
                }
                else if (NewLang == "ESP") {
                    greeting = "Buenas noches " + lab5.Text + " :)";
                }
                else if (NewLang == "FRE") {
                    greeting = "bonne nuit " + lab5.Text + " :)";
                }
                else if (NewLang == "POR") {
                    greeting = "Boa noite " + lab5.Text + " :)";
                }
                else if (NewLang == "CHI") {
                    greeting = "晚安 " + lab5.Text + " :)";
                }
                else if (NewLang == "RUS") {
                    greeting = "Спокойной ночи " + lab5.Text + " :)";
                }
                else if (NewLang == "DUT") {
                    greeting = "Welterusten " + lab5.Text + " :)";
                }
            }

            lab1.Text = greeting;
        }
        private void setupUILanguage(String _custLang) {
            var Form_1 = Form1.instance;
            if(_custLang == "MY") {
                label21.Text = "Tetapan";
                label75.Text = "Alamat Email";
                tabPage5.Text = "Perkongsian Fail & API";
                tabPage4.Text = "Bahasa";
                tabPage3.Text = "Naik Taraf";
                tabPage2.Text = "Perangkaan";
                tabPage1.Text = "Akaun";
                label4.Text = "Nama Pengguna";
                label7.Text = "Jenis Akaun";
                label38.Text = "Muat Naik Item";

                label70.Text = "Tetapan";

                label67.Text = "Perlukan Kata-Laluan";
                label66.Text = "Minta kata-laluan sebelum orang dibenarkan berkongsi fail kepada anda";

                label69.Text = "Melumpuhkan Perkongsian Fail";
                label68.Text = "Melumpuhkan Perkongsian Fail akan tidak benarkan orang berkongsi fail kepada anda. Anda masih boleh berkongsi fail kepada orang lain";

                label58.Text = "Ubah nama-pengguna";
                label33.Text = "Nama pengguna akaun Flowstorage anda akan ditukar, data anda akan kekal";

                label18.Text = "Ubah kata-laluan";
                label8.Text = "Ubah kata-laluan akaun Flowstorage anda";

                label36.Text = "Log keluar akaun saya";
                label35.Text = "Flowstorage tidak akan log masuk akaun anda secara automatik semasa permulaan";

                label2.Text = "Padam akaun saya";
                label3.Text = "Akaun Flowstorage anda akan dipadam bersama-sama dengan data anda";
                
                label22.Text = "Tetapan";
                label1.Text = "Tetapan";

                label13.Text = "Kiraan Fail";
                label9.Text = "Kiraan Direktori";
                label11.Text = "Kiraan Folder";
                label31.Text = "Jumlah Upload Hari-ini";
                label28.Text = "Fail";
                label29.Text = "Direktori";
                label15.Text = "Tarikh Penciptaan Akaun";

                guna2Button12.Text = "Ubah";
                guna2Button13.Text = "Ubah";
                guna2Button4.Text = "Log-Keluar";
                guna2Button4.TextOffset = new Point(4,0);
                guna2Button11.Location = new Point(140, 61);
                guna2Button1.Text = "Padam Akaun";

                Form_1.label10.Text = "Muat-Naik";
                Form_1.guna2Button2.Text = "Muat-Naik Fail";
                Form_1.guna2Button12.Text = "Muat-Naik Folder";
                Form_1.guna2Button1.Text = "Buat Direktori";
                Form_1.guna2Button7.Text = "Perkongsian Fail";
                Form_1.guna2Button7.Size = new Size(159, 47);
                Form_1.label28.Text = "Kepentingan";
                Form_1.label2.Text = "Kiraan Item";
            }

            if(_custLang == "US") {
                label21.Text = "Settings";
                label75.Text = "Email Address";
                tabPage5.Text = "File Sharing";
                tabPage4.Text = "Languages";
                tabPage3.Text = "Upgrade";
                tabPage2.Text = "Statistics";
                tabPage1.Text = "Account";
                label4.Text = "Username";
                label7.Text = "Account Type";
                label38.Text = "Item Upload";
                label70.Text = "Settings";

                label67.Text = "Required Password";
                label66.Text = "Ask for password before people can share a file to you";

                label69.Text = "Disable File Sharing";
                label68.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";

                label58.Text = "Change my username";
                label33.Text = "Your Flowstorage account username will be changes but your data is remains";

                label18.Text = "Change my password";
                label8.Text = "Change your Flowstorage account password";

                label36.Text = "Logout my account";
                label35.Text = "Flowstorage will not automatically login your account on startup";

                label2.Text = "Delete my account";
                label3.Text = "Your Flowstorage account along with your data will be deleted";

                label22.Text = "Settings";
                label1.Text = "Settings";

                label13.Text = "Files Count";
                label9.Text = "Directory Count";
                label11.Text = "Folders Count";
                label31.Text = "Total Upload Today";
                label28.Text = "File";
                label29.Text = "Directory";
                label15.Text = "Account Creation Date";

                guna2Button12.Text = "Change";
                guna2Button13.Text = "Change";
                guna2Button4.Text = "Logout";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(121, 61);
                guna2Button1.Text = "Delete Account";

                Form_1.label10.Text = "Upload";
                Form_1.label2.Text = "Item Count";
                Form_1.guna2Button2.Text = "Upload File";
                Form_1.guna2Button12.Text = "Upload Folder";
                Form_1.guna2Button1.Text = "Create Directory";
                Form_1.guna2Button7.Text = "File Sharing";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentials";
            }

            if(_custLang == "DUT") {
                label21.Text = "Instellingen";
                label75.Text = "E-mailadres";
                tabPage5.Text = "Bestanden delen & API";
                tabPage4.Text = "Talen";
                tabPage3.Text = "Opwaarderen";
                tabPage2.Text = "Statistieken";
                tabPage1.Text = "Account";
                label4.Text = "Gebruikersnaam";
                label7.Text = "Accounttype";
                label38.Text = "Item uploaden";
                label70.Text = "Instellingen";

                label67.Text = "Vereist wachtwoord";
                label66.Text = "Vraag om wachtwoord voordat mensen een bestand met je kunnen delen";

                label69.Text = "Bestandsdeling uitschakelen";
                label68.Text = "Als u bestanden delen uitschakelt, kunnen mensen geen bestand met u delen. U kunt echter nog steeds met mensen delen.";

                label58.Text = "Wijzig mijn gebruikersnaam";
                label33.Text = "De gebruikersnaam van uw Flowstorage-account wordt gewijzigd, maar uw gegevens blijven behouden";

                label18.Text = "Mijn wachtwoord wijzigen";
                label8.Text = "Wijzig het wachtwoord van uw Flowstorage-account";

                label36.Text = "Mijn account afmelden";
                label35.Text = "Flowstorage logt niet automatisch in op uw account bij het opstarten";

                label2.Text = "Verwijder mijn account";
                label3.Text = "Uw Flowstorage-account wordt samen met uw gegevens verwijderd";

                label22.Text = "Instellingen";
                label1.Text = "Instellingen";

                label13.Text = "Aantal bestanden";
                label9.Text = "Aantal mappen";
                label11.Text = "Aantal mappen";
                label31.Text = "Totale upload vandaag";
                label28.Text = "Bestand";
                label29.Text = "Map";
                label15.Text = "Aanmaakdatum account";

                guna2Button12.Text = "Wijzigen";
                guna2Button13.Text = "Wijzigen";
                guna2Button4.Text = "Uitloggen";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(134, 61);
                guna2Button1.Text = "Aanmelden";

                Form_1.label10.Text = "Uploaden";
                Form_1.label2.Text = "Aantal artikelen";
                Form_1.guna2Button2.Text = "Bestand uploaden";
                Form_1.guna2Button12.Text = "Map uploaden";
                Form_1.guna2Button1.Text = "Directory aanmaken";
                Form_1.guna2Button7.Text = "Bestanden delen";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "Essentials";
            }

            if(_custLang == "RUS") {
                label21.Text = "Настройки";
                label75.Text = "электронная почта";
                tabPage5.Text = "Общий доступ к файлам и API";
                tabPage4.Text = "Языки";
                tabPage3.Text = "Обновить";
                tabPage2.Text = "Статистика";
                tabPage1.Text = "Учетная запись";
                label4.Text = "Имя пользователя";
                label7.Text = "Тип учетной записи";
                label38.Text = "Загрузка элемента";
                label70.Text = "Настройки";

                label67.Text = "Требуемый пароль";
                label66.Text = "Запрашивайте пароль, прежде чем люди смогут поделиться с вами файлом";

                label69.Text = "Отключить общий доступ к файлам";
                label68.Text = "Отключение общего доступа к файлам не позволит людям делиться файлами с вами. Однако вы все равно можете делиться с другими людьми.";

                label58.Text = "Изменить мое имя пользователя";
                label33.Text = "Имя пользователя вашей учетной записи Flowstorage будет изменено, но ваши данные останутся";

                label18.Text = "Изменить мой пароль";
                label8.Text = "Измените пароль своей учетной записи Flowstorage";

                label36.Text = "Выйти из моей учетной записи";
                label35.Text = "Flowstorage не будет автоматически входить в вашу учетную запись при запуске";

                label2.Text = "Удалить мою учетную запись";
                label3.Text = "Ваша учетная запись Flowstorage вместе с вашими данными будет удалена";

                label22.Text = "Настройки";
                label1.Text = "Настройки";

                label13.Text = "Счетчик файлов";
                label9.Text = "Счетчик каталогов";
                label11.Text = "Счетчик папок";
                label31.Text = "Всего сегодня загружено";
                label28.Text = "Файл";
                label29.Text = "Каталог";
                label15.Text = "Дата создания учетной записи";

                guna2Button12.Text = "Изменить";
                guna2Button13.Text = "Изменить";
                guna2Button4.Text = "Выход";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(166, 61);
                guna2Button1.Text = "Удалить аккаунт";

                Form_1.label10.Text = "Загрузить";
                Form_1.label2.Text = "Количество предметов";
                Form_1.guna2Button2.Text = "Загрузить файл";
                Form_1.guna2Button12.Text = "Загрузить папку";
                Form_1.guna2Button1.Text = "Создать каталог";
                Form_1.guna2Button7.Text = "Общий доступ к файлам";
                Form_1.guna2Button7.Size = new Size(129, 47);
                Form_1.label28.Text = "Основные";
            }

            if(_custLang == "GER") {
                label21.Text = "Einstellungen";
                label75.Text = "E-Mail-Addresse";
                tabPage5.Text = "Datenaustausch & API";
                tabPage4.Text = "Sprachen";
                tabPage3.Text = "Aktualisierung";
                tabPage2.Text = "Statistiken";
                tabPage1.Text = "Konto";
                label4.Text = "Nutzername";
                label7.Text = "Konto Typ";
                label38.Text = "Artikel hochladen";

                label70.Text = "Einstellungen";

                label67.Text = "Erforderliches Passwort";
                label66.Text = "Nach dem Passwort fragen, bevor andere eine Datei für Sie freigeben können";

                label69.Text = "Dateifreigabe deaktivieren";
                label68.Text = "Das Deaktivieren der Dateifreigabe erlaubt anderen nicht, eine Datei mit Ihnen zu teilen. Sie können sie jedoch immer noch mit anderen teilen.";

                label58.Text = "Ändere meinen Benutzernamen";
                label33.Text = "Der Benutzername Ihres Flowstorage-Kontos wird geändert, Ihre Daten bleiben jedoch erhalten";

                label18.Text = "Ändere mein Passwort";
                label8.Text = "Ändern Sie das Passwort Ihres Flowstorage-Kontos";

                label36.Text = "Von meinem Konto abmelden";
                label35.Text = "Flowstorage meldet sich beim Start nicht automatisch in Ihrem Konto an";

                label2.Text = "Mein Konto löschen";
                label3.Text = "Ihr Flowstorage-Konto wird zusammen mit Ihren Daten gelöscht";

                label22.Text = "Einstellungen";
                label1.Text = "Einstellungen";

                label13.Text = "Files Count";
                label9.Text = "Directory Count";
                label11.Text = "Folders Count";
                label31.Text = "Total Upload Today";
                label28.Text = "File";
                label29.Text = "Directory";
                label15.Text = "Erstellungsdatum des Kontos";

                guna2Button12.Text = "Ändern";
                guna2Button13.Text = "Ändern";
                guna2Button4.Text = "Ausloggen";
                guna2Button4.TextOffset = new Point(10, 0);
                guna2Button11.Location = new Point(155, 61);
                guna2Button1.Text = "Konto Löschen";

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
                label21.Text = "設定";
                tabPage5.Text = "ファイル共有";
                tabPage4.Text = "言語";
                tabPage3.Text = "アップグレード";
                tabPage2.Text = "統計";
                tabPage1.Text = "アカウント";
                label4.Text = "ユーザー名";
                label7.Text = "口座の種類";
                label38.Text = "アイテムのアップロード";

                label70.Text = "設定";

                label67.Text = "必要なパスワード";
                label66.Text = "ファイルを共有する前にパスワードを要求する";

                label69.Text = "ファイル共有を無効にする";
                label68.Text = "ファイル共有を無効にすると、他のユーザーがファイルを共有することはできなくなります。ただし、他のユーザーと共有することはできます。";

                label58.Text = "ユーザー名を変更する";
                label33.Text = "Flowstorage アカウントのユーザー名は変更されますが、データはそのまま残ります";

                label18.Text = "パスワードを変更する";
                label8.Text = "Flowstorage アカウントのパスワードを変更する";

                label36.Text = "アカウントをログアウトする";
                label35.Text = "Flowstorage は、起動時にアカウントに自動的にログインしません";

                label2.Text = "アカウントを削除します";
                label3.Text = "Flowstorage アカウントとデータが削除されます";

                label22.Text = "設定";
                label1.Text = "設定";

                label13.Text = "ファイル数";
                label9.Text = "ディレクトリ数";
                label11.Text = "フォルダ数";
                label31.Text = "今日の合計アップロード";
                label28.Text = "ファイル";
                label29.Text = "ディレクトリ";
                label15.Text = "アカウント作成日";

                guna2Button12.Text = "変化";
                guna2Button13.Text = "変化";
                guna2Button4.Text = "ログアウト";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(199, 61);
                guna2Button1.Text = "アカウントを削除する";

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
                label21.Text = "Ajustes";
                label75.Text = "Correo electrónico";
                tabPage5.Text = "Compartición de archivos & API";
                tabPage4.Text = "Idiomas";
                tabPage3.Text = "Mejora";
                tabPage2.Text = "Estadísticas";
                tabPage1.Text = "Cuenta";
                label4.Text = "Nombre de usuario";
                label7.Text = "Tipo de cuenta";
                label38.Text = "Carga de artículo";

                label70.Text = "Configuración";

                label67.Text = "Contraseña requerida";
                label66.Text = "Solicite la contraseña antes de que la gente pueda compartir un archivo con usted";

                label69.Text = "Desactivar uso compartido de archivos";
                label68.Text = "Deshabilitar el uso compartido de archivos no permitirá que las personas compartan un archivo contigo. Sin embargo, aún puedes compartirlo con otras personas.";

                label58.Text = "cambiar mi nombre de usuario";
                label33.Text = "El nombre de usuario de su cuenta de Flowstorage cambiará, pero sus datos permanecerán";

                label18.Text = "cambiar mi contraseña";
                label8.Text = "Cambiar la contraseña de su cuenta Flowstorage";

                label36.Text = "cerrar sesión en mi cuenta";
                label35.Text = "Flowstorage no iniciará sesión automáticamente en su cuenta al iniciar";

                label2.Text = "Borrar mi cuenta";
                label3.Text = "Se eliminará su cuenta de Flowstorage junto con sus datos";

                label22.Text = "Ajustes";
                label1.Text = "Ajustes";

                label13.Text = "Recuento de archivos";
                label9.Text = "Recuento de directorios";
                label11.Text = "Número de carpetas";
                label31.Text = "Subida total hoy";
                label28.Text = "Archivo";
                label29.Text = "Directorio";
                label15.Text = "Fecha de creación de la cuenta";

                guna2Button12.Text = "Cambiar";
                guna2Button13.Text = "Cambiar";
                guna2Button4.Text = "Cerrar sesión";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(152, 61);
                guna2Button1.Text = "Borrar cuenta";

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
                label21.Text = "Paramètres";
                tabPage5.Text = "Partage de fichiers & API";
                tabPage4.Text = "Langages";
                tabPage3.Text = "Améliorer";
                tabPage2.Text = "Statistiques";
                tabPage1.Text = "Compte";
                label4.Text = "Nom d'utilisateur";
                label7.Text = "Type de compte";
                label38.Text = "Téléchargement de l'article";

                label70.Text = "Paramètres";

                label67.Text = "Mot de passe requis";
                label66.Text = "Demandez un mot de passe avant que les gens puissent partager un fichier avec vous";

                label69.Text = "Désactiver le partage de fichiers";
                label68.Text = "La désactivation du partage de fichiers ne permettra pas aux autres de partager un fichier avec vous. Cependant, vous pouvez toujours partager avec d'autres personnes.";

                label58.Text = "Changer mon nom d'utilisateur";
                label33.Text = "Le nom d'utilisateur de votre compte Flowstorage sera modifié, mais vos données resteront";

                label18.Text = "Changer mon mot de passe";
                label8.Text = "Modifier le mot de passe de votre compte Flowstorage";

                label36.Text = "Déconnecter mon compte";
                label35.Text = "Flowstorage will not automatically login your account on startup";

                label2.Text = "Supprimer mon compte";
                label3.Text = "Votre compte Flowstorage ainsi que vos données seront supprimés";

                label22.Text = "Paramètres";
                label1.Text = "Paramètres";

                label13.Text = "Nombre de fichiers";
                label9.Text = "Nombre de répertoires";
                label11.Text = "Nombre de dossiers";
                label31.Text = "Téléchargement total aujourd'hui";
                label28.Text = "Déposer";
                label29.Text = "Annuaire";
                label15.Text = "Date de création du compte";

                guna2Button12.Text = "Changement";
                guna2Button13.Text = "Changement";
                guna2Button4.Text = "Se déconnecter";
                guna2Button4.TextOffset = new Point(-3, 0);
                guna2Button11.Location = new Point(230, 61);
                guna2Button1.Text = "Supprimer le compte";

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
                label21.Text = "Configurações";
                tabPage5.Text = "Compartilhamento de arquivos & API";
                tabPage4.Text = "línguas";
                tabPage3.Text = "Atualizar";
                tabPage2.Text = "Estatisticas";
                tabPage1.Text = "Conta";
                label4.Text = "Nome de usuário";
                label7.Text = "tipo de conta";
                label38.Text = "Carregamento de item";

                label70.Text = "Configurações";

                label67.Text = "Senha Necessária";
                label66.Text = "Pedir senha antes que as pessoas possam compartilhar um arquivo com você";

                label69.Text = "Desativar Compartilhamento de Arquivos";
                label68.Text = "Desativar o compartilhamento de arquivos não permitirá que as pessoas compartilhem um arquivo com você. No entanto, você ainda pode compartilhar com outras pessoas.";

                label58.Text = "Alterar meu nome de usuário";
                label33.Text = "O nome de usuário da sua conta Flowstorage será alterado, mas seus dados permanecerão";

                label18.Text = "Mudar minha senha";
                label8.Text = "Altere a senha da sua conta Flowstorage";

                label36.Text = "Sair da minha conta";
                label35.Text = "O Flowstorage não fará login automaticamente em sua conta na inicialização";

                label2.Text = "Deletar minha conta";
                label3.Text = "Sua conta Flowstorage junto com seus dados serão excluídos";

                label22.Text = "Configurações";
                label1.Text = "Configurações";

                label13.Text = "Contagem de arquivos";
                label9.Text = "Contagem de diretório";
                label11.Text = "Contagem de Pastas";
                label31.Text = "Carregamento total hoje";
                label28.Text = "Arquivo";
                label29.Text = "Diretório";
                label15.Text = "Data de criação da conta";

                guna2Button12.Text = "Mudar";
                guna2Button13.Text = "Mudar";
                guna2Button4.Text = "Sair";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(196, 61);
                guna2Button1.Text = "Deletar conta";

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
                label21.Text = "设置";
                tabPage5.Text = "文件共享";
                tabPage4.Text = "语言";
                tabPage3.Text = "升级";
                tabPage2.Text = "统计数据";
                tabPage1.Text = "帐户";
                label4.Text = "用户名";
                label7.Text = "帐户类型";
                label38.Text = "项目上传";

                label70.Text = "设置";

                label67.Text = "需要密码";
                label66.Text = "在别人分享文件给你之前要求输入密码";

                label69.Text = "禁用文件共享";
                label68.Text = "禁用文件共享将不允许其他人与您共享文件。但是您仍然可以与其他人共享。";

                label58.Text = "更改我的用户名";
                label33.Text = "您的 Flowstorage 帐户用户名将更改，但您的数据将保留";

                label18.Text = "修改我的密码";
                label8.Text = "更改您的流存账户密码";

                label36.Text = "注销我的帐户";
                label35.Text = "Flowstorage 不会在启动时自动登录您的帐户";

                label2.Text = "删除我的账户";
                label3.Text = "您的 Flowstorage 帐户以及您的数据将被删除";

                label22.Text = "设置";
                label1.Text = "设置";

                label13.Text = "文件数";
                label9.Text = "目录计数";
                label11.Text = "文件夹数";
                label31.Text = "今日上传总量";
                label28.Text = "文件";
                label29.Text = "目录";
                label15.Text = "帐户创建日期";
                label70.Text = "设置";

                guna2Button12.Text = "改变";
                guna2Button13.Text = "改变";
                guna2Button4.Text = "登出";
                guna2Button4.TextOffset = new Point(0, 0);
                guna2Button11.Location = new Point(102, 61);
                guna2Button1.Text = "删除帐户";

                Form_1.label10.Text = "上传";
                Form_1.label2.Text = "物品数量";
                Form_1.guna2Button2.Text = "上传文件";
                Form_1.guna2Button12.Text = "上传文件夹";
                Form_1.guna2Button1.Text = "创建目录";
                Form_1.guna2Button7.Text = "文件共享";
                Form_1.guna2Button7.Size = new Size(125, 47);
                Form_1.label28.Text = "要点";
            }
        }

        private async Task getCurrentLang() {
            string _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(_selectLang, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                using (MySqlDataReader _readLang = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await _readLang.ReadAsync()) {
                        CurrentLang = _readLang.GetString(0);
                    }
                }
            }
        }

        private void updateLang(String _custLang) {
            String _updateQuery = "UPDATE lang_info SET CUST_LANG = @lang WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_updateQuery,con);
            command.Parameters.AddWithValue("@lang",_custLang);
            command.Parameters.AddWithValue("@username", label5.Text);
            command.ExecuteNonQuery();
        }

        private void languageChanger(String _custLang) {

            ///////////////////////////////////////////////
            
            if (_custLang == "US") {
                guna2Button19.Text = "Default";
                guna2Button19.ForeColor = Color.Gainsboro;
                guna2Button19.Enabled = false;
                updateLang("US");
                setupUILanguage("US");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                guna2Button18.Text = "Default";
                guna2Button18.ForeColor = Color.Gainsboro;
                guna2Button18.Enabled = false;
                updateLang("MY");
                setupUILanguage("MY");

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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

            if(_custLang == "JAP") {
                guna2Button17.Text = "Default";
                guna2Button17.ForeColor = Color.Gainsboro;
                guna2Button17.Enabled = false;
                updateLang("JAP");
                setupUILanguage("JAP");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

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

            if(_custLang == "GER") {
                guna2Button15.Text = "Default";
                guna2Button15.ForeColor = Color.Gainsboro;
                guna2Button15.Enabled = false;
                updateLang("GER");
                setupUILanguage("GER");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("ESP");
                setupUILanguage("ESP");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("FRE");
                setupUILanguage("FRE");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("POR");
                setupUILanguage("POR");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("CHI");
                setupUILanguage("CHI");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("RUS");
                setupUILanguage("RUS");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
                updateLang("DUT");
                setupUILanguage("DUT");

                guna2Button18.Text = "Set as default";
                guna2Button18.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button18.Enabled = true;

                guna2Button19.Text = "Set as default";
                guna2Button19.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button19.Enabled = true;

                guna2Button16.Text = "Set as default";
                guna2Button16.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button16.Enabled = true;

                guna2Button17.Text = "Set as default";
                guna2Button17.ForeColor = Color.FromArgb(55, 0, 179);
                guna2Button17.Enabled = true;

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
            languageChanger("MY");
            NewLang = "MY";
            setupTime();
        }

        private void guna2Button17_Click(object sender, EventArgs e) {
            languageChanger("JAP");
            NewLang = "JAP";
            setupTime();
        }

        private void guna2Button19_Click(object sender, EventArgs e) {
            languageChanger("US");
            NewLang = "US";
            setupTime();
        }

        private void guna2Button15_Click(object sender, EventArgs e) {
            languageChanger("GER");
            NewLang = "GER";
            setupTime();
        }

        private void guna2Panel21_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel20_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button16_Click(object sender, EventArgs e) {
            languageChanger("ESP");
            NewLang = "ESP";
            setupTime();
        }

        private void guna2Button20_Click(object sender, EventArgs e) {
            languageChanger("FRE");
            NewLang = "FRE";
            setupTime();
        }

        private void guna2Button21_Click(object sender, EventArgs e) {
            languageChanger("POR");
            NewLang = "POR";
            setupTime();
        }

        private void guna2Button22_Click(object sender, EventArgs e) {
            languageChanger("CHI");
            NewLang = "CHI";
            setupTime();
        }

        private void label56_Click(object sender, EventArgs e) {

        }

        private void tabPage3_Click(object sender, EventArgs e) {

        }

        private void guna2Button25_Click_1(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button23_Click(object sender, EventArgs e) {
            PassSharingFORM _passForm = new PassSharingFORM();
            _passForm.Show();
        }

        /// <summary>
        /// Function to start disabling file sharing
        /// </summary>
        /// <param name="_custUsername"></param>
        private void disableSharing(String _custUsername) {
            String _disableQuery = "UPDATE sharing_info SET DISABLED = 1 WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_disableQuery,con);
            command.Parameters.AddWithValue("@username",_custUsername);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Function to enable file sharing
        /// </summary>
        /// <param name="_custUsername"></param>
        private void enableSharing(String _custUsername) {
            String _disableQuery = "UPDATE sharing_info SET DISABLED = 0 WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_disableQuery, con);
            command.Parameters.AddWithValue("@username", _custUsername);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Disable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button24_Click(object sender, EventArgs e) {

            if(MessageBox.Show("Disable file sharing? You can always enable this option again later.","Flowstorage",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes) {

                guna2Button24.Enabled = false;
                guna2Button24.Visible = false;

                guna2Button26.Visible = true;
                guna2Button26.Enabled = true;

                label69.Text = "Enable File Sharing";
                label68.Text = "Enabling file sharing will allows people to share a file to you";

                disableSharing(label5.Text);

            }
        }

        /// <summary>
        /// Enable file sharing button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button26_Click(object sender, EventArgs e) {

            guna2Button24.Enabled = true;
            guna2Button24.Visible = true;

            guna2Button26.Visible = false;
            guna2Button26.Enabled = false;

            label69.Text = "Disable File Sharing";
            label68.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";

            enableSharing(label5.Text);

        }

        private void tabPage5_Click(object sender, EventArgs e) {
        }

 
        private string retrieveFileSharingPas() {
            String hasPass = "";
            String selectQuery = "SELECT SET_PASS FROM sharing_info WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(selectQuery, con)) {
                command.Parameters.AddWithValue("@username", label5.Text);

                object result = command.ExecuteScalar();
                if (result != null) {
                    hasPass = result.ToString();
                }
            }

            return hasPass;
        }

        /// <summary>
        /// This function will retrieve user access token
        /// </summary>
        /// <param name="custUsername"></param>
        /// <returns></returns>
        private string getAccessToken(String custUsername) {
            String _localTok = "";
            String _selecTokQue = "SELECT ACCESS_TOK FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_selecTokQue,con);
            command.Parameters.AddWithValue("@username",custUsername);

            MySqlDataReader _readTok = command.ExecuteReader();
            while(_readTok.Read()) {
                _localTok = EncryptionModel.Decrypt(_readTok.GetString(0), "0123456789085746");
            }
            _readTok.Close();
            return _localTok.ToLower();
        }

        /// <summary>
        /// If user selected File Sharing tab page
        /// then retrieve their current file sharing information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2TabControl1_Click(object sender, EventArgs e) {

            if(guna2TabControl1.SelectedIndex == 1) {
                CurrDateStats++;
                String getJoinDateQuery = "SELECT CREATED_DATE FROM information WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(getJoinDateQuery, con);
                command.Parameters.AddWithValue("@username", label5.Text);

                String joinedDate = command.ExecuteScalar()?.ToString();
                if (joinedDate != null) {
                    label16.Text = joinedDate;
                }

            } else {
                if(CurrDateStats > 0) {
                    label16.Text = JoinedDate;
                }
            }

            if(guna2TabControl1.SelectedIndex == 3) {
                if (CurrentLang == "US") {
                    guna2Button19.Text = "Default";
                    guna2Button19.Enabled = false;
                    guna2Button19.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "MY") {
                    guna2Button18.Text = "Default";
                    guna2Button18.Enabled = false;
                    guna2Button18.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "JAP") {
                    guna2Button17.Text = "Default";
                    guna2Button17.Enabled = false;
                    guna2Button17.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "GER") {
                    guna2Button15.Text = "Default";
                    guna2Button15.Enabled = false;
                    guna2Button15.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "ESP") {
                    guna2Button16.Text = "Default";
                    guna2Button16.Enabled = false;
                    guna2Button16.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "FRE") {
                    guna2Button20.Text = "Default";
                    guna2Button20.Enabled = false;
                    guna2Button20.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "CHI") {
                    guna2Button22.Text = "Default";
                    guna2Button22.Enabled = false;
                    guna2Button22.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "POR") {
                    guna2Button21.Text = "Default";
                    guna2Button21.Enabled = false;
                    guna2Button21.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "RUS") {
                    guna2Button31.Text = "Default";
                    guna2Button31.Enabled = false;
                    guna2Button31.ForeColor = Color.Gainsboro;
                }

                if (CurrentLang == "DUT") {
                    guna2Button30.Text = "Default";
                    guna2Button30.Enabled = false;
                    guna2Button30.ForeColor = Color.Gainsboro;
                }
            }

            if(guna2TabControl1.SelectedIndex == 2) {
                
                //guna2TextBox2.Text = getAccessToken(label5.Text);

                if(retrieveFileSharingPas() != "DEF") {

                    guna2Button27.Visible = true;
                    guna2Button27.Enabled = true;

                    guna2Button23.Visible = false;
                    guna2Button23.Enabled = false;
                } else {

                    guna2Button27.Visible = false;
                    guna2Button27.Enabled = false;

                    guna2Button23.Visible = true;
                    guna2Button23.Enabled = true;
                }

                if (retrieveDisabled(label5.Text) == "1") {
                    guna2Button24.Enabled = false;
                    guna2Button24.Visible = false;

                    guna2Button26.Visible = true;
                    guna2Button26.Enabled = true;

                    label69.Text = "Enable File Sharing";
                    label68.Text = "Enabling file sharing will allows people to share a file to you";

                }
                else {
                    guna2Button24.Enabled = true;
                    guna2Button24.Visible = true;

                    guna2Button26.Visible = false;
                    guna2Button26.Enabled = false;

                    label69.Text = "Disable File Sharing";
                    label68.Text = "Disabling file sharing will not allow people to share a file to you. You can still share to people however.";
                }
            }
        }

        /// <summary>
        /// This function will delete user file sharing password 
        /// if they have one setup
        /// </summary>
        /// <param name="_custUsername"></param>
        private void removePasSharing(String _custUsername) {
            String removeQuery = "UPDATE sharing_info SET SET_PASS = @setPass WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(removeQuery, con);
            command.Parameters.AddWithValue("@setPass", "DEF");
            command.Parameters.AddWithValue("@username", _custUsername);
            command.ExecuteNonQuery();

        }
        private void guna2Button27_Click(object sender, EventArgs e) {
            if(MessageBox.Show("Remove File Sharing password?","Flowstorage",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes) {

                removePasSharing(label5.Text);
                guna2Button23.Visible = true;
                guna2Button23.Enabled = true;

                guna2Button27.Visible = false;
                guna2Button27.Enabled = false;
            }
        }

        /// <summary>
        /// This button will show user access token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button28_Click(object sender, EventArgs e) {
            if(tokenCheckCurr == 0) {
                verificationTokenFORM _showVerForm = new verificationTokenFORM();
                _showVerForm.Show();
            } else {
                guna2TextBox2.Enabled = true;
                guna2Button28.Visible = false;
                guna2Button29.Visible = true;
                guna2TextBox2.PasswordChar = '\0';
            }
        }

        private void guna2Button29_Click(object sender, EventArgs e) {
            guna2Button29.Visible = false;
            guna2Button28.Visible = true;
            guna2TextBox2.PasswordChar = '*';
        }

        private void label65_Click(object sender, EventArgs e) {

        }

        private void label64_Click(object sender, EventArgs e) {

        }

        private void guna2Panel14_Paint(object sender, PaintEventArgs e) {

        }

        private void label34_Click_1(object sender, EventArgs e) {

        }

        private void label27_Click_2(object sender, EventArgs e) {

        }

        private void guna2Panel13_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button31_Click(object sender, EventArgs e) {
            languageChanger("RUS");
            NewLang = "RUS";
            setupTime();
        }

        private void guna2Button30_Click(object sender, EventArgs e) {
            languageChanger("DUT");
            NewLang = "DUT";
            setupTime();
        }

        private void label78_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// Open API web page on browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://flowstorage.netlify.app/api_web/index.html");
        }

        private void label22_Click(object sender, EventArgs e) {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            RecovFORM showRecoveryForm = new RecovFORM();
            showRecoveryForm.Show();
        }

        private void label52_Click(object sender, EventArgs e) {

        }
    }
}
