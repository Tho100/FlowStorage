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

        readonly public exeFORM instance;

        readonly private MySqlConnection con = ConnectionModel.con;

        public byte[] GlobalByte;
        public string _TableName;
        public string _DirectoryName;

        private bool _isFromShared;
        private bool IsFromSharing;  

        public exeFORM(String fileName,String tableName, String directoryName, String uploaderUsername,bool isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            instance = this;

            string _getName = "";
            bool _isShared = Regex.Match(uploaderUsername, @"^([\w\-]+)").Value == "Shared";

            lblFileName.Text = fileName;

            this._TableName = tableName;
            this._DirectoryName = directoryName;
            this._isFromShared = isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                btnEditComment.Visible = true;
                guna2Button9.Visible = true;

                _getName = uploaderUsername.Replace("Shared", "");
                label6.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
            }
            else {
                _getName = " " + uploaderUsername;
                label6.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";
            }

            lblUploaderName.Text = _getName;
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
                SaverModel.SaveSelectedFile(lblFileName.Text, "upload_info_directory", _DirectoryName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "folder_upload_info", _DirectoryName);
            }
            else if (_TableName == "file_info_exe") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "file_info_exe", _DirectoryName);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "cust_sharing", _DirectoryName,_isFromShared);
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string[] parts = lblFileName.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(lblFileName.Text, getExtension, IsFromSharing, _TableName, _DirectoryName);
            _showSharingFileFORM.Show();
        }

        private async Task saveChangesComment(String updatedComment) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            btnEditComment.Visible = false;
            guna2Button9.Visible = true;
            lblUserComment.Visible = false;
            guna2TextBox4.Text = lblUserComment.Text;
        }

        private async void guna2Button9_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            lblUserComment.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button9.Visible = false;
            guna2TextBox4.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

    }
}
