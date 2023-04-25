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
        private static bool IsFromSharing;  // Shared to me 
        private static bool isFromShared = false;
        public apkFORM(String _titleFile, String _userName,String _tabName, String _dirName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_userName, @"^([\w\-]+)").Value == "Shared";

            label1.Text = _titleFile;
            _TableName = _tabName;
            _DirName = _dirName;
            isFromShared = _isFromShared;
            IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                guna2Button7.Visible = true;
                guna2Button3.Visible = true;

                _getName = _userName.Replace("Shared", "");
                label6.Text = "Shared To";
                guna2Button5.Visible = false;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _userName;
                label6.Text = "Uploaded By";
                label3.Visible = true;
                label3.Text = getCommentSharedToMe() != "" ? getCommentSharedToMe() : "(No Comment)";
            }

            label2.Text = _getName;
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text,EncryptionKey.KeyValue));
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
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

        private void label3_Click(object sender, EventArgs e) {

        }

        private void apkFORM_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

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

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = label1.Text.Substring(label1.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing,_TableName,_DirName);
            _showSharingFileFORM.Show();
        }

        private async Task saveChangesComment(String updatedComment) {

            string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private async void guna2Button7_Click(object sender, EventArgs e) {

            if (label3.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            label3.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label3.Text;
            guna2Button3.Visible = true;
            guna2Button7.Visible = false;
            guna2TextBox4.Visible = false;
            label3.Visible = true;
            label3.Refresh();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button3.Visible = false;
            guna2Button7.Visible = true;
            label3.Visible = false;
            guna2TextBox4.Text = label3.Text;
        }
    }
}
