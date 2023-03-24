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
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class apkFORM : Form {
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String _TableName;
        private static String _DirName;
        private static bool isFromShared = false;
        public apkFORM(String _titleFile, String _userName,String _tabName, String _dirName, bool _isFromShared = false) {

            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_userName, @"^([\w\-]+)").Value == "Shared";

            label1.Text = _titleFile;
            _TableName = _tabName;
            _DirName = _dirName;
            isFromShared = _isFromShared;

            if (_isShared == true) {
                _getName = _userName;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? "Comment: '" + getCommentSharedToOthers() + "'" : "Comment: (No Comment)";
            }
            else {
                _getName = "Uploaded By " + _userName;
                label3.Visible = true;
                label3.Text = getCommentSharedToMe() != "" ? "Comment: '" + getCommentSharedToMe() + "'" : "Comment: (No Comment)";
            }

            label2.Text = _getName;
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }

        private string getCommentSharedToOthers() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
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

                if (_TableName == "file_info_apk") {
                    SaverModel.SaveSelectedFile(label1.Text, "file_info_apk", _DirName);
                }
                else if (_TableName == "upload_info_directory") {
                    SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
                } else if (_TableName == "folder_upload_info") {
                    SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
                }
                else if (_TableName == "cust_sharing") {
                    SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName,isFromShared);
                }
            } catch (Exception) {
                MessageBox.Show("Failed to download this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Question);
            }
        }
    }
}
