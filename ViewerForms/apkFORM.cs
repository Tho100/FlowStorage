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

        readonly private MySqlConnection con = ConnectionModel.con;

        private string _TableName;
        private string _DirName;
        private bool IsFromSharing;   
        private bool isFromShared = false;

        public apkFORM(String fileName, String uploaderUsername,String tableName, String directoryName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            string _getName = "";
            bool _isShared = Regex.Match(uploaderUsername, @"^([\w\-]+)").Value == "Shared";

            this.lblFileName.Text = fileName;
            this._TableName = tableName;
            this._DirName = directoryName;
            this.isFromShared = _isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                guna2Button7.Visible = true;
                btnEditComment.Visible = true;

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
                    SaverModel.SaveSelectedFile(lblFileName.Text, "file_info_apk", _DirName);
                }
                else if (_TableName == "upload_info_directory") {
                    SaverModel.SaveSelectedFile(lblFileName.Text, "upload_info_directory", _DirName);
                } else if (_TableName == "folder_upload_info") {
                    SaverModel.SaveSelectedFile(lblFileName.Text, "folder_upload_info", _DirName);
                }
                else if (_TableName == "cust_sharing") {
                    SaverModel.SaveSelectedFile(lblFileName.Text, "cust_sharing", _DirName,isFromShared);
                }

            } catch (Exception) {
                MessageBox.Show("Failed to download this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Question);
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(lblFileName.Text, getExtension, IsFromSharing,_TableName,_DirName);
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

        private async void guna2Button7_Click(object sender, EventArgs e) {

            if (lblUserComment.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            lblUserComment.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button7.Visible = false;
            guna2TextBox4.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            btnEditComment.Visible = false;
            guna2Button7.Visible = true;
            lblUserComment.Visible = false;
            guna2TextBox4.Text = lblUserComment.Text;
        }
    }
}
