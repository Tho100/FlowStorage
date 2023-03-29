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
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static Byte[] GlobalByte;
        public static String _TableName;
        public static String _DirectoryName;
        private static bool _isFromShared;
        private static bool IsFromSharing;  // Shared to me 

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
                _getName = _UploaderUsername;
                guna2Button5.Visible = false;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? "Comment: '" + getCommentSharedToOthers() + "'" : "Comment: (No Comment)";
            }
            else {
                _getName = "Uploaded By " + _UploaderUsername;
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
        private void exeFORM_Load(object sender, EventArgs e) {
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
            label1.AutoSize = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            label1.AutoSize = true;
        }
        private void setupDialog(Byte[] _getExeValues) {
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.Filter = "Exe|*.exe";
            _OpenDialog.FileName = label1.Text;
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(_OpenDialog.FileName, _getExeValues);
            }
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
    }
}
