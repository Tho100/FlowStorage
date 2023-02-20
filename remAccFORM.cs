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
        private String CurrentLang = "";
        private String NewLang = "";
        public remAccFORM(String _accName) {
            InitializeComponent();
            instance = this;
            label5.Text = _accName;

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.guna2Button11, "Item upload indicate how many file/directory you can upload.");

            try {

                Application.DoEvents();
                
                getCurrentLang();
                setupUILanguage(CurrentLang);
                setupRedundane(label6.Text);
                getAccType();
                countTotalAll();

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

            } catch (Exception) {
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
                }
                
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
                if(CurrentLang == "US") {
                    label37.Text = "Limited to 12";
                } else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 12";
                } else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 12";
                } else if (CurrentLang == "JAP") {
                    label37.Text = "12 個限定";
                } else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 12";
                }
            }
            else if (_accType == "Max") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 30";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 30";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 30";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "30 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 30";
                }
                guna2Button5.Enabled = false;
            }
            else if (_accType == "Express") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 85";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 85";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 85";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "85 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 85";
                }
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
            }
            else if (_accType == "Supreme") {
                if (CurrentLang == "US") {
                    label37.Text = "Limited to 170";
                }
                else if (CurrentLang == "MY") {
                    label37.Text = "Terhad Kepada 170";
                }
                else if (CurrentLang == "GER") {
                    label37.Text = "Begrenzt Auf 170";
                }
                else if (CurrentLang == "JAP") {
                    label37.Text = "170 個限定";
                }
                else if (CurrentLang == "ESP") {
                    label37.Text = "Limitado a 170";
                }
                guna2Button5.Enabled = false;
                guna2Button6.Enabled = false;
                guna2Button7.Enabled = false;
            }
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
            } catch (Exception) {
                MessageBox.Show("There's a problem while attempting to logout your account.","Flowstorage");
            }
        }

        private void label42_Click(object sender, EventArgs e) {

        }

       



        private void guna2Button5_Click(object sender, EventArgs e) {
            //_setupPayment("Max account for Flowstorage",2);
            _selectedAcc = "Max";
            guna2Button8.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/bIY9AEfERf5O2OI4gj"); // Live mode
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
            System.Diagnostics.Process.Start("https://buy.stripe.com/6oEeUY0JX8Hq4WQ5ko"); // Test mode
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _selectedAcc = "Express";
            guna2Button9.Visible = true;
            System.Diagnostics.Process.Start("https://buy.stripe.com/bIY9AE9gtf5O9d6bIK"); // Live mode
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
                label37.Text = "Limited to 30";
            }
        }

        void setupAccount() {

            try {

                var _setupApiKey = ConfigurationManager.ConnectionStrings["APISETUP"].ConnectionString;
                Stripe.StripeConfiguration.SetApiKey(_setupApiKey);
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
                    ///String _insertNew = "INSERT INTO cust_type(CUST_USERNAME,CUST_EMAIL,ACC_TYPE) VALUES (@username,@email,@type)";
                    //command = new MySqlCommand(_insertNew, con);
                    //command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    //command.Parameters.AddWithValue("@email", CustUserValues[0]);
                    //command.Parameters.AddWithValue("@type", _selectedAcc);
                    String _insertNew = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                    command = new MySqlCommand(_insertNew, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@email", CustUserValues[0]);
                    command.Parameters.AddWithValue("@type", _selectedAcc);
                    if (command.ExecuteNonQuery() == 1) {
                        //   String _deleteOld = "DELETE FROM cust_type WHERE CUST_USERNAME = @username";
                        //   command = new MySqlCommand(_deleteOld, con);
                        //   command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        //   command.ExecuteNonQuery();

                        /*String _deleteOld = "UPDATE FROM cust_type WHERE CUST_USERNAME = @username";
                        command = new MySqlCommand(_deleteOld, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.ExecuteNonQuery();*/

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
            var picturebox2 = form.pictureBox2;
            var picturebox3 = form.pictureBox3;
            var picturebox1 = form.pictureBox1;
       
            DateTime now = DateTime.Now;
            var hours = now.Hour;
            String greeting = null;
            if (hours >= 1 && hours <= 12) {
                if (NewLang == "US") {
                    greeting = "Good Morning " + lab5.Text + " :) ";
                }
                else if (NewLang == "MY") {
                    greeting = "Selemat Pagi " + lab5.Text + " :) ";
                }
                else if (NewLang == "GER") {
                    greeting = "Guten Morgen " + lab5.Text + " :)";
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
                picturebox2.Visible = true;
                picturebox1.Visible = false;
                picturebox3.Visible = false;
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

                picturebox2.Visible = true;
                picturebox1.Visible = false;
                picturebox3.Visible = false;
            }
            else if (hours >= 16 && hours <= 21) {
                if (hours == 20 || hours == 21) {
                    if (NewLang == "US") {
                        greeting = "Good Late Evening " + lab5.Text + " :)";
                    }
                    else if (NewLang == "MY") {
                        greeting = "Selamat Lewat-Petang " + lab5.Text + " :)";
                    } else if (NewLang == "GER") {
                        greeting = "Guten späten Abend " + lab5.Text + " :)";
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
                }
                else {
                    if (NewLang == "US") {
                        greeting = "Good Evening " + lab5.Text + " :)";
                    }
                    else if (NewLang == "MY") {
                        greeting = "Selamat Petang " + lab5.Text + " :)";
                    } else if (NewLang == "GER") {
                        greeting = "Guten Abend " + lab5.Text + " :)";
                    } else if (NewLang == "JAP") {
                        greeting = "こんばんは " + lab5.Text + " :)";
                    }
                    else if (NewLang == "ESP") {
                        greeting = "Buenas terdes " + lab5.Text + " :)";
                    }
                    else if (NewLang == "FRE") {
                        greeting = "bonne soirée " + lab5.Text + " :)";
                    }
                }

                picturebox3.Visible = true;
                picturebox2.Visible = false;
                picturebox1.Visible = false;
            }
            else if (hours >= 21 && hours <= 24) {
                if (NewLang == "US") {
                    greeting = "Good Night " + lab5.Text + " :)";
                }
                else if (NewLang == "MY") {
                    greeting = "Selamat Malam " + lab5.Text + " :)";
                }
                else if (NewLang == "GER") {
                    greeting = "Guten Nacth " + lab5.Text + " :)";
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
                picturebox1.Visible = true;
                picturebox2.Visible = false;
                picturebox3.Visible = false;
            }
            lab1.Text = greeting;
        }
        private void setupUILanguage(String _custLang) {
            var Form_1 = Form1.instance;
            if(_custLang == "MY") {
                label21.Text = "Tetapan";
                tabPage4.Text = "Bahasa";
                tabPage3.Text = "Naik Taraf";
                tabPage2.Text = "Perangkaan";
                tabPage1.Text = "Akaun";
                label4.Text = "Nama Pengguna";
                label7.Text = "Jenis Akaun";
                label38.Text = "Muat Naik Item";

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
                Form_1.label29.Text = "Lain-lain";
                Form_1.guna2Button3.Text = "Tambah Akaun";
                Form_1.guna2Button5.Text = "Tetapan";
                Form_1.label2.Text = "Kiraan Item";
            }

            if(_custLang == "US") {
                label21.Text = "Settings";
                tabPage4.Text = "Languages";
                tabPage3.Text = "Upgrade";
                tabPage2.Text = "Statistics";
                tabPage1.Text = "Account";
                label4.Text = "Username";
                label7.Text = "Account Type";
                label38.Text = "Item Upload";

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
                Form_1.label29.Text = "Others";
                Form_1.guna2Button3.Text = "Add Account";
                Form_1.guna2Button5.Text = "Settings";
            }

            if(_custLang == "GER") {
                label21.Text = "Einstellungen";
                tabPage4.Text = "Sprachen";
                tabPage3.Text = "Aktualisierung";
                tabPage2.Text = "Statistiken";
                tabPage1.Text = "Konto";
                label4.Text = "Nutzername";
                label7.Text = "Konto Typ";
                label38.Text = "Artikel hochladen";

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
                Form_1.label29.Text = "Others";
                Form_1.guna2Button3.Text = "Konto hinzufügen";
                Form_1.guna2Button5.Text = "Einstellungen";
            }

            if (_custLang == "JAP") {
                label21.Text = "設定";
                tabPage4.Text = "言語";
                tabPage3.Text = "アップグレード";
                tabPage2.Text = "統計";
                tabPage1.Text = "アカウント";
                label4.Text = "ユーザー名";
                label7.Text = "口座の種類";
                label38.Text = "アイテムのアップロード";

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
                Form_1.label29.Text = "その他";
                Form_1.guna2Button3.Text = "アカウントを追加する";
                Form_1.guna2Button5.Text = "設定";
            }

            if (_custLang == "ESP") {
                label21.Text = "Ajustes";
                tabPage4.Text = "Idiomas";
                tabPage3.Text = "Mejora";
                tabPage2.Text = "Estadísticas";
                tabPage1.Text = "Cuenta";
                label4.Text = "Nombre de usuario";
                label7.Text = "Tipo de cuenta";
                label38.Text = "Carga de artículo";

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
                Form_1.label29.Text = "Otros";
                Form_1.guna2Button3.Text = "Añadir cuenta";
                Form_1.guna2Button5.Text = "Ajustes";
            }

            if (_custLang == "FRE") {
                label21.Text = "Paramètres";
                tabPage4.Text = "Langages";
                tabPage3.Text = "Améliorer";
                tabPage2.Text = "Statistiques";
                tabPage1.Text = "Compte";
                label4.Text = "Nom d'utilisateur";
                label7.Text = "Type de compte";
                label38.Text = "Téléchargement de l'article";

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
                Form_1.label29.Text = "Autres";
                Form_1.guna2Button3.Text = "Ajouter un compte";
                Form_1.guna2Button5.Text = "Paramètres";
            }
        }

        private void getCurrentLang() {
            String _selectLang = "SELECT CUST_LANG FROM lang_info WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_selectLang,con);
            command.Parameters.AddWithValue("@username",label5.Text);

            MySqlDataReader _readLang = command.ExecuteReader();
            if(_readLang.Read()) {
                CurrentLang = _readLang.GetString(0);
            }
            _readLang.Close();
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
    }
}
