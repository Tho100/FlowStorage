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
using Stripe.Checkout;
//using Stripe;

namespace FlowSERVER1 {
    public partial class remAccFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static remAccFORM instance;
        public List<int> _TotalUploadToday = new List<int>();
        public List<int> _TotalUploadOvertime = new List<int>();
        public List<String> _TotalUploadDirectoryToday = new List<String>();
        private static readonly HttpClient client = new HttpClient();
        public remAccFORM(String _accName) {
            InitializeComponent();
            instance = this;
            label5.Text = _accName;
            this.ShowInTaskbar = false;
            this.Text = "Settings";

            try {
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

                TotalUploadFileTodayCount("file_info");
                TotalUploadFileTodayCount("file_info_pdf");
                TotalUploadFileTodayCount("file_info_expand");
                TotalUploadFileTodayCount("file_info_exe");
                TotalUploadFileTodayCount("file_info_word");
                TotalUploadFileTodayCount("file_info_ptx");
                TotalUploadFileTodayCount("file_info_gif");
                TotalUploadFileTodayCount("file_info_audi");
                TotalUploadFileTodayCount("file_info_vid");
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

                var _totalUploadOvertime = _TotalUploadOvertime.Sum(x => Convert.ToInt32(x));
                label12.Text = _totalUploadOvertime.ToString();

                /* The problem is say if you have the same 
                    file type that was uploaded twice then it will be seen as 
                    1,1 and not 2
                @SUMMARY: File is numerically counted instead of summing the values*/
            } catch (Exception) {
                /*Form bgBlur = new Form();
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
                }*/
            }

        }


        public void TotalUploadFile(String _tableName) {
            String _CountQue = "SELECT COUNT(*) FROM " + _tableName + " WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_CountQue,con);
            command.Parameters.AddWithValue("@username",label5.Text);

            var totalCount = command.ExecuteScalar();
            int intTotalCount = Convert.ToInt32(totalCount);
            _TotalUploadOvertime.Add(intTotalCount);
        }
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
            String GetAccType = "SELECT ACC_TYPE FROM CUST_TYPE WHERE CUST_USERNAME = @username";
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
                label37.Text = "Limited to 10";
            }
            else if (_accType == "Max") {
                label37.Text = "Limited to 25";
            }
            else if (_accType == "Express") {
                label37.Text = "Limited to 40";
            }
            else if (_accType == "Supreme") {
                label37.Text = "Limited to 95";
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

        // @SUMMARY Total upload charts stats

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
                    Email = Form1.instance.label24.Text
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
            _setupPayment("Max account for Flowstorage",2);
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
            _setupPayment("Supreme account for Flowstorage",9.99);
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _setupPayment("Express account for Flowstorage",5);
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
    }
}
