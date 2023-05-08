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
using MySql.Data;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {
    public partial class exeFORM : Form {

        public static exeFORM instance;

        public MySqlConnection con = ConnectionModel.con;
        public MySqlCommand command = ConnectionModel.command;

        public Byte[] GlobalByte;
        public String _TableName;
        public String _DirectoryName;

        private bool _isFromShared;
        private bool IsFromSharing;  // Shared to me 

        public exeFORM(String getTitle,String tableName, String directoryName, String _UploaderUsername,bool isFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderUsername, @"^([\w\-]+)").Value == "Shared";

            label1.Text = getTitle;
            instance = this;
            _TableName = tableName;
            _DirectoryName = directoryName;
            _isFromShared = isFromShared;
            IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                guna2Button3.Visible = true;
                guna2Button9.Visible = true;

                _getName = _UploaderUsername.Replace("Shared", "");
                label6.Text = "Shared To";
                guna2Button5.Visible = false;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _UploaderUsername;
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        private string getCommentSharedToOthers() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }
        private void exeFORM_Load(object sender, EventArgs e) {
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }
        private void guna2Button4_Click(object sender, EventArgs e) {
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirectoryName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            }
            else if (_TableName == "file_info_exe") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_exe", _DirectoryName);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirectoryName,_isFromShared);
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string[] parts = label1.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirectoryName);
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

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button3.Visible = false;
            guna2Button9.Visible = true;
            label3.Visible = false;
            guna2TextBox4.Text = label3.Text;
        }

        private async void guna2Button9_Click(object sender, EventArgs e) {
            if (label3.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            label3.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label3.Text;
            guna2Button3.Visible = true;
            guna2Button9.Visible = false;
            guna2TextBox4.Visible = false;
            label3.Visible = true;
            label3.Refresh();
        }

    }
}
