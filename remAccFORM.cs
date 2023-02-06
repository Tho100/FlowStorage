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
//using Stripe;

namespace FlowSERVER1 {
    /// <summary>
    /// Settings form class
    /// </summary>
    public partial class remAccFORM : Form {

        public static remAccFORM instance;
        public static String _selectedAcc;

        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private List<int> _TotalUploadToday = new List<int>();
        private List<int> _TotalUploadOvertime = new List<int>();
        private List<String> _TotalUploadDirectoryToday = new List<String>();

        public remAccFORM(String _accName) {
            InitializeComponent();
            instance = this;
            label5.Text = _accName;
            this.Text = "Settings";

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.guna2Button11, "Item upload indicate how many file/directory you can upload.");

            try {

                Application.DoEvents();

                LeastMostUpload("file_info");
                LeastMostUpload("file_info_expand");
                getAccType();
                countTotalAll();

                // @SUMMARY Retrieve account creation date and display the date on label

                String _getJoinDate = "SELECT CREATED_DATE FROM information WHERE CUST_USERNAME = @username";
                command = con.CreateCommand();
                command.CommandText = _getJoinDate;
                command.Parameters.AddWithValue("@username",label5.Text);
            
                List<String> _JoinedDateValues = new List<String>();

                MySqlDataReader _readDate = command.ExecuteReader();
                while(_readDate.Read()) {
                    _JoinedDateValues.Add(_readDate.GetString(0));
                }
                _readDate.Close();
                var joinedDate = _JoinedDateValues[0];
                label16.Text = joinedDate;

                chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;
                generateChart("Image", "file_info");
                generateChart("Text", "file_info_expand");
                generateChart("Video", "file_info_vid");
                generateChart("PDF", "file_info_pdf");
                generateChart("APK", "file_info_apk");
                generateChart("Exe", "file_info_exe");
                generateChart("GIF", "file_info_gif");
                generateChart("Document", "file_info_word");
                generateChart("Presentation", "file_info_ptx");
                generateChart("Audio","file_info_audi");
                generateChart("Excel","file_info_excel");

                TotalUploadFileTodayCount("file_info");
                TotalUploadFileTodayCount("file_info_pdf");
                TotalUploadFileTodayCount("file_info_expand");
                TotalUploadFileTodayCount("file_info_exe");
                TotalUploadFileTodayCount("file_info_word");
                TotalUploadFileTodayCount("file_info_ptx");
                TotalUploadFileTodayCount("file_info_gif");
                TotalUploadFileTodayCount("file_info_audi");
                TotalUploadFileTodayCount("file_info_vid");
                TotalUploadFileTodayCount("file_info_excel");
                TotalUploadDirectoryTodayCount();

                var _totalUploadTodayCount = _TotalUploadToday.Sum(x => Convert.ToInt32(x));
                label26.Text = _totalUploadTodayCount.ToString();


                TotalUploadFile("file_info");
                TotalUploadFile("file_info_pdf");
                TotalUploadFile("file_info_expand");
                TotalUploadFile("file_info_exe");
                TotalUploadFile("file_info_word");
                TotalUploadFile("file_info_ptx");
                TotalUploadFile("file_info_gif");
                TotalUploadFile("file_info_audi");
                TotalUploadFile("file_info_vid");
                TotalUploadFile("file_info_excel");

                var _totalUploadOvertime = _TotalUploadOvertime.Sum(x => Convert.ToInt32(x));
                label12.Text = _totalUploadOvertime.ToString();

                Application.DoEvents();

            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
                /*
                Form bgBlur = new Form();
                using (waitFORM displayWait = new waitFORM()) {
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.FormBorderStyle = FormBorderStyle.None;
                    bgBlur.Opacity = .24d;
                    bgBlur.BackColor = Color.Black;
                    bgBlur.WindowState = FormWindowState.Maximized;
                    bgBlur.TopMost = true;
                    bgBlur.Location = this.Location;
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.ShowInTaskbar = false;
                    bgBlur.Show();

                    displayWait.Owner = bgBlur;
                    displayWait.ShowDialog();

                    bgBlur.Dispose();
                */
            }

        }

        /// <summary>
        /// This function will tells user how many files
        /// they have uploaded (in total)
        /// </summary>
        /// <param name="_tableName"></param>
        public void TotalUploadFile(String _tableName) {
            String _CountQue = "SELECT COUNT(*) FROM " + _tableName + " WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_CountQue,con);
            command.Parameters.AddWithValue("@username",label5.Text);

            var totalCount = command.ExecuteScalar();
            int intTotalCount = Convert.ToInt32(totalCount);
            _TotalUploadOvertime.Add(intTotalCount);
        }

        /// <summary>
        /// This function will tells user the number of 
        /// files they've uploaded a day
        /// </summary>
        /// <param name="_TableName"></param>
        public void TotalUploadFileTodayCount(String _TableName) {
            String _CurDate = DateTime.Now.ToString("dd/MM/yyyy");
            String _QueryCount = "SELECT COUNT(CUST_USERNAME) FROM " + _TableName + " WHERE CUST_USERNAME = @username AND UPLOAD_DATE = @date";
            command = new MySqlCommand(_QueryCount,con);
            command.Parameters.AddWithValue("@date",_CurDate);
            command.Parameters.AddWithValue("@username",label5.Text);

            var _totalCount = command.ExecuteScalar();
            int _toInt = Convert.ToInt32(_totalCount);
            _TotalUploadToday.Add(_toInt);
        }
        /// <summary>
        /// This function will tells user the number
        /// of directory they have created a day
        /// </summary>
        public void TotalUploadDirectoryTodayCount() {
            String _CurDate = DateTime.Now.ToString("dd/MM/yyyy");
            String _QueryCount = "SELECT DIR_NAME FROM upload_info_directory WHERE CUST_USERNAME = @username AND UPLOAD_DATE = @date";
            command = new MySqlCommand(_QueryCount, con);
            command.Parameters.AddWithValue("@date", _CurDate);
            command.Parameters.AddWithValue("@username", label5.Text);

            MySqlDataReader _ReadDir = command.ExecuteReader();
            while(_ReadDir.Read()) {    
                _TotalUploadDirectoryToday.Add(_ReadDir.GetString(0));
            }
            _ReadDir.Close();

            List<String> _DistinctDir = _TotalUploadDirectoryToday.Distinct().ToList();
            label30.Text = _DistinctDir.Count().ToString();
        }

        public void getAccType() {
            String GetAccType = "SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(GetAccType, con);
            command.Parameters.AddWithValue("@username", label5.Text);

            List<String> _types = new List<String>();
            MySqlDataReader _readType = command.ExecuteReader();
            while (_readType.Read()) {
                _types.Add(_readType.GetString(0));
            }
            _readType.Close();

            String _accType = _types[0];
            label6.Text = _accType;
            if (_accType == "Basic") {
                label37.Text = "Limited to 25";
            }
            else if (_accType == "Max") {
                label37.Text = "Limited to 50";
                guna2Button5.Enabled = false;
            }
            else if (_accType == "Express") {
                label37.Text = "Limited to 85";
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
            }
            else if (_accType == "Supreme") {
                label37.Text = "Limited to 170";
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
                guna2Button7.Enabled = false;
            }
        }
        public void LeastMostUpload(String _TabName) {

  
        }
        public void countTotalAll() {
    
            String CountDirQue = "SELECT COUNT(*) FROM file_info_directory WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(CountDirQue,con);
            command.Parameters.AddWithValue("@username",label5.Text);
            var _setupCount = command.ExecuteScalar();
            int _totalDir = Convert.ToInt32(_setupCount);
            label19.Text = _totalDir.ToString();

            var countTotalFolders = Form1.instance.listBox1.Items.Count - 1;
            label20.Text = countTotalFolders.ToString();
        }

        
        /// <summary>
        /// This will generates statistical chart
        /// that tells user how many files they've
        /// uploaded by date
        /// </summary>
        /// <param name="_serName"></param>
        /// <param name="_tableName"></param>

        public void generateChart(String _serName, String _tableName) {

            List<String> _datesValues = new List<string>();
            List<int> _totalRow = new List<int>();

            String _countUpload = "SELECT UPLOAD_DATE,COUNT(UPLOAD_DATE) FROM " + _tableName + " WHERE CUST_USERNAME = @username GROUP BY UPLOAD_DATE HAVING COUNT(UPLOAD_DATE) > 0"; //
            command = con.CreateCommand();
            command.CommandText = _countUpload;
            command.Parameters.AddWithValue("@username", label5.Text);

            MySqlDataReader _readRowUploadTexts = command.ExecuteReader();
            while (_readRowUploadTexts.Read()) {
                _totalRow.Add(_readRowUploadTexts.GetInt32("COUNT(UPLOAD_DATE)"));
                _datesValues.Add(_readRowUploadTexts.GetString("UPLOAD_DATE"));
            }
            _readRowUploadTexts.Close();

            List<int> _fileUploadValues = new List<int>();
            for (int i = 0; i < _totalRow.Count(); i++) {
                chart1.Series[_serName].Points.AddXY(_datesValues[i], _totalRow[i]);
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
            
 //           richTextBox2.Text = JsonConvert.DeserializeObject<Stripe.Customer>(customers).Email;// yes.ToString();
            //}

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
                    String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                    String _getAuth = _getPath + "\\CUST_DATAS.txt";
                    if (File.Exists(_getAuth)) {
                        Directory.Delete(_getPath, true);
                    }
                    Application.OpenForms["remAccFORM"].Close();
                    Form1.instance.guna2Panel7.Visible = true;
                    Form1.instance.listBox1.Items.Clear();
                }
            } catch (Exception eq) {
                MessageBox.Show("There's a problem while attempting to logout your account.","Flowstorage");
            }
        }

        private void label42_Click(object sender, EventArgs e) {

        }

        private async void _setupPayment(String TypeOf,double Pricing) {
          
            Task.Run(() => {
                // ADD CUSTOMER
                Stripe.StripeConfiguration.SetApiKey("sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN");
                var options = new Stripe.CustomerCreateOptions {
                    Email = Form1.instance.label24.Text,
                    Name = Form1.instance.label5.Text
                };

                var service = new Stripe.CustomerService();
                Stripe.Customer customer = service.Create(options);
                var emailCust = customer.Email;
                //

                // START PAYMENT

                var servicePayment = new Stripe.PaymentIntentService();
                var optionpayments = new Stripe.PaymentIntentCreateOptions {
                    Amount = 1,
                    SetupFutureUsage = "off_session",
                    Currency = "USD",
                };
                var paymentIntent = servicePayment.Create(optionpayments);
                System.Diagnostics.Process.Start(paymentIntent.ToString());
            });
 /*
                var myCharge = new Stripe.ChargeCreateOptions();
                // always set these properties
                myCharge.Amount = 1;
                myCharge.Currency = "USD";
                myCharge.ReceiptEmail = "nfrealyt@gmail.com";
                myCharge.Description = "Sample Charge";
                myCharge.Source = "pk_test_51MO4YYF2lxRV33xseOZm26PlyARkh28Qw3aUPX6Xkc5diqY8aHSBFQIDW3QHFdLHJtA8csgKgWPsrVj6jEW9DmoU00i4y5mv4n";
                myCharge.Capture = true;
                var chargeService = new Stripe.ChargeService();
                Stripe.Charge stripeCharge = chargeService.Create(myCharge);*/

                // Newly created customer is returned
                //var CustEmail = customer.Email;
                //MessageBox.Show(CustEmail);
            




            /*string url = "";
            string business = "nfrealyt@gmail.com";
            //            string description = "Donation";            
            string country = "MY";
            string currency = "USD";

            url += "https://www.paypal.com/cgi-bin/webscr/" +
                "?cmd=" + "_xclick" +
                "&amount=" + Pricing +
                "&business=" + business +
                "&item_name=" + TypeOf;
            paypalFORM _showPayment = new paypalFORM(url,TypeOf);
            _showPayment.Show();*/


            // System.Diagnostics.Process.Start(url);
            /*
            var values = new Dictionary<string, string>
                {
                    { "thing1", "hello" },
                    { "thing2", "world" }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);

            var responseString = await response.Content.ReadAsStringAsync();*/

            /* "?cmd=" + "_donations" +
               "&amount=245" + 
               "&business=" + business +
               "&lc=" + country +
               "&item_name=" + TypeOf +
               "&currency_code=" + currency +
               "&bn=" + "PP%2dDonationsBF";*/

            /*var config = new PayPalConfiguration(PayPalEnvironment.NoNetwork,"UTZE2PQSX5A6C") {
                AcceptCreditCards = true,
                MerchantName = "Flowstorage",
                Language = "eng",
            };
            CrossPayPalManager.Init(config);
            var Result = await CrossPayPalManager.Current.Buy(new PayPalItem(TypeOf,new Decimal (Pricing),"USD"),new decimal(0));
            if(Result.Status == PayPalStatus.Cancelled) {
                MessageBox.Show("USER CANCELLED");
            } else if (Result.Status == PayPalStatus.Error) {
                MessageBox.Show("ERROR");
            }
            else if (Result.Status == PayPalStatus.Successful) {
                MessageBox.Show("SUCCEED");
            }
            */
            // var CustClient = new HttpClient {BaseAddress = new Uri("https://www.paypal.com/cgi-bin/webscr/?cmd=_xclick&amount=2&business=nfrealyt@gmail.com&item_name=Max%20account%20for%20Flowstorage") };
            // CustClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(ApiConstant))

            // https://www.paypal.com/paypalme/flowstoragepaypal //https://www.paypal.com/cgi-bin/webscr 
        }



        private void guna2Button5_Click(object sender, EventArgs e) {
            //_setupPayment("Max account for Flowstorage",2);
            _selectedAcc = "Max";
            guna2Button8.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/test_eVa9Du9Hb9SndCoeUU"); // Test mode
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
            System.Diagnostics.Process.Start("https://buy.stripe.com/test_dR63f69Hbc0v55S5km"); // Test mode
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _selectedAcc = "Express";
            guna2Button9.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/test_7sI02U4mR1lR69W001"); // Test mode
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
                label37.Text = "Limited to 170";
            }
            else if (_selectedAcc == "Express") {
                guna2Button6.Enabled = false;
                guna2Button5.Enabled = false;
                guna2Button9.Visible = false;
                label37.Text = "Limited to 85";
            }
            else if (_selectedAcc == "Max") {
                guna2Button5.Enabled = false;
                guna2Button8.Visible = false;
                label37.Text = "Limited to 50";
            }
        }

        void setupAccount() {
            try {
                Stripe.StripeConfiguration.SetApiKey("sk_test_51MO4YYF2lxRV33xsBfTJLQypyLBjhoxYdz18VoLrZZ6hin4eJrAV9O6NzduqR02vosmC4INFgBgxD5TkrkpM3sZs00hqhx3ZzN");
                var service = new Stripe.CustomerService();
                var customers = service.List();

                String lastId = null;
                String LastEmail = null;

                // Enumerate the first page of the list
                foreach (Stripe.Customer customer in customers) {
                    lastId = customer.Id;
                    LastEmail = customer.Email;
                }


     //           var options = new Stripe.CustomerListOptions {
   //                 Limit = 1
 //               };
                //var service = new Stripe.CustomerService();
//                Stripe.StripeList<Stripe.Customer> customers = service.List(options);

                List<String> CustUserValues = new List<String>();
                String _selectCustEmail = "SELECT CUST_EMAIL FROM information WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(_selectCustEmail, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                MySqlDataReader _readEmail = command.ExecuteReader();
                if (_readEmail.Read()) {
                    CustUserValues.Add(_readEmail.GetString(0));
                }
                _readEmail.Close();

               /* foreach (var item in customers) {
                    richTextBox2.Text = item.ToString();
                }
                String testing = richTextBox2.Text;
                var f = testing.IndexOf(CustUserValues[0]) + Environment.NewLine;
                var fq = testing.Substring(testing.IndexOf(CustUserValues[0]));
                var result = Regex.Match(fq, @"^([\w\-]+)");*/ 
                if (LastEmail == CustUserValues[0]) {
                    String _insertNew = "INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE) VALUES (@username,@email,@type)";
                    command = new MySqlCommand(_insertNew, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@email", CustUserValues[0]);
                    command.Parameters.AddWithValue("@type", _selectedAcc);
                    if (command.ExecuteNonQuery() == 1) {
                        String _deleteOld = "DELETE FROM cust_type WHERE CUST_USERNAME = @username";
                        command = new MySqlCommand(_deleteOld, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.ExecuteNonQuery();

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
            catch (Exception eq) {
                noPayment _showFailed = new noPayment();
                _showFailed.Show();
                //MessageBox.Show("Failed to proceed with " + _selectedAcc + " account setup.\n\n Please ensure that your Flowstorage email is the same as your email on payment.", "Flowstorage");
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
    }
}
