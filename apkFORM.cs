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

namespace FlowSERVER1 {
    public partial class apkFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _TableName;
        public static String _DirName;
        public apkFORM(String _titleFile, String _userName,String _tabName, String _dirName) {
            InitializeComponent();
            label1.Text = _titleFile;
            label2.Text = "Uploaded by " + _userName;
            _TableName = _tabName;
            _DirName = _dirName;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void apkFORM_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            label1.AutoSize = true;
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your APK data.","Saver");
                ShowAlert.Show();
                Application.DoEvents();
                if (_TableName == "file_info_apk") {
                    SaverModel.SaveSelectedFile(label1.Text, "file_info_apk", _DirName);
                }
                else if (_TableName == "upload_info_directory") {
                    SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
                } else if (_TableName == "folder_upload_info") {
                    SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
                }
                else if (_TableName == "cust_sharing") {
                    SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName);
                }
            } catch (Exception) {
                MessageBox.Show("Failed to download this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Question);
            }
        }
    }
}
