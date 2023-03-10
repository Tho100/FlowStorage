using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class msiFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _TableName;
        public static String _DirectoryName;
        private static bool _IsFromShared;
        public msiFORM(String _fileName,String tabName, String dirName,String _UploaderUsername, bool _isFromShared = false) {
            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderUsername, @"^([\w\-]+)").Value == "Shared";

            if (_isShared == true) {
                _getName = _UploaderUsername;
            }
            else {
                _getName = "Uploaded By " + _UploaderUsername;
            }

            label1.Text = _fileName;
            label2.Text = _getName;
            _TableName = tabName;
            _DirectoryName = dirName;
            _IsFromShared = _isFromShared;
        }

        private void msiFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirectoryName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            }
            else if (_TableName == "file_info_msi") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_msi", _DirectoryName);
            } 
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_msi", _DirectoryName, _IsFromShared);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
    }
}
