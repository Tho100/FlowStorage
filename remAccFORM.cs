﻿using System;
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

namespace FlowSERVER1 {
    public partial class remAccFORM : Form {
        public static remAccFORM instance;
        public static string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
        public static string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
        public static string username = "root"; // epiz_33067528 | root
        public static string password = "nfreal-yt10";
        public static int mainPort_ = 15817;
        public static string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
        public MySqlConnection con = new MySqlConnection(constring);
        public MySqlCommand command;
        public remAccFORM(String _accName) {
            InitializeComponent();
            instance = this;

            label5.Text = _accName;

            var countTotalFolders = Form1.instance.listBox1.Items.Count-1;
            label20.Text = countTotalFolders.ToString();

            con.Open();

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

            con.Close();

            // @SUMMARY Total upload charts stats

            con.Open();

            List<String> _datesValues = new List<string>();
            List<int> _totalRow = new List<int>();

            String _countUpload = "SELECT UPLOAD_DATE,COUNT(UPLOAD_DATE) FROM file_info WHERE CUST_USERNAME = @username GROUP BY UPLOAD_DATE HAVING COUNT(UPLOAD_DATE) >= 1";
            command = con.CreateCommand();
            command.CommandText = _countUpload;
            command.Parameters.AddWithValue("@username",label5.Text);

            MySqlDataReader _readRowUpload = command.ExecuteReader();
            while(_readRowUpload.Read()) {
                _totalRow.Add(_readRowUpload.GetInt32("COUNT(UPLOAD_DATE)"));
                _datesValues.Add(_readRowUpload.GetString("UPLOAD_DATE"));
            }
            _readRowUpload.Close();

            for(int i=0; i<_totalRow.Count(); i++) {
                chart1.Series["Total Upload"].Points.AddXY(_datesValues[i],_totalRow[i]);
            }

            con.Close();
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
    }
}
