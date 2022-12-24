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

namespace FlowSERVER1 {
    public partial class remAccFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static remAccFORM instance;
        public remAccFORM(String _accName) {
            InitializeComponent();
            instance = this;

            label5.Text = _accName;

            String _getAccType = "SELECT ACC_TYPE FROM CUST_TYPE WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_getAccType, con);
            command.Parameters.AddWithValue("@username", label5.Text);

            List<String> _types = new List<String>();
            MySqlDataReader _readType = command.ExecuteReader();
            while (_readType.Read()) {
                _types.Add(_readType.GetString(0));
            }
            _readType.Close();

            String _accType = _types[0];
            label6.Text = _accType;
            if(_accType == "Basic") {
                label37.Text = "Limited to 10 files";
            } else if (_accType == "Max") {
                label37.Text = "Limited to 25 files";
            }
            else if (_accType == "Express") {
                label37.Text = "Limited to 40 files";
            }
            else if (_accType == "Supreme") {
                label37.Text = "Limited to 95 files";
            }

            var countTotalFolders = Form1.instance.listBox1.Items.Count-1;
            label20.Text = countTotalFolders.ToString();

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

            /* The problem is say if you have the same 
                file type that was uploaded twice then it will be seen as 
                1,1 and not 2
            @SUMMARY: File is numerically counted instead of summing the values*/

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
            if (_totalRow.Count() >= 0) {
                if (_tableName == "file_info") {
                    label26.Text = _totalRow.Count().ToString();
                }
                if (_tableName == "file_info_expand") {
                    //label27.Text = _totalRow[0].ToString();
                }
                if (_tableName == "file_info_vid") {
                    //label28.Text = _totalRow[0].ToString();
                }
            }
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

        private void _setupPayment(String TypeOf) {
            string url = "";

            string business = "nfrealyt@gmail.com";     
//            string description = "Donation";            
            string country = "MY";                  
            string currency = "MYR";            

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + TypeOf +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            _setupPayment("Max account for Flowstorage");
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
            _setupPayment("Supreme account for Flowstorage");
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _setupPayment("Express account for Flowstorage");
        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
