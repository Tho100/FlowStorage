using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class picFORM : Form {
        public static picFORM instance;
        private static String TableName;
        private static String Directoryname;
        private static bool IsFromShared; // @ Shared to others
        private static bool IsFromSharing;  // @ Shared to me 
        private static MySqlConnection con = ConnectionModel.con;

        public picFORM(Image userImage, int width, int height,string title,string _TableName, string _DirectoryName, string _UploaderName,bool _IsFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            instance = this;

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            var setupImage = resizeUserImage(userImage, new Size(width, height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = title;
            TableName = _TableName;
            Directoryname = _DirectoryName;
            IsFromShared = _IsFromShared;
            IsFromSharing = _isFromSharing;

            label7.Text = $"({width}x{height})";
             
            if (_isShared == true) {
                _getName = _UploaderName.Replace("Shared","");
                label4.Text = "Shared To";
                guna2Button5.Visible = false;
                label3.Visible = true;
                label3.Text = getCommentSharedToOthers() != "" ? getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _UploaderName;
                label4.Text = "Uploaded By";
                label3.Visible = true;
                label3.Text = getCommentSharedToMe() != "" ? getCommentSharedToMe() : "(No Comment)";
            }

            label2.Text = _getName;

            ToolTip saveTip = new ToolTip();
            saveTip.SetToolTip(this.guna2Button4,"Download Image");
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using(MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename",con)) {
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
                using(MySqlDataReader readerComment = command.ExecuteReader()) {
                    while(readerComment.Read()) {
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746")); using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }

        public static Image resizeUserImage(Image userImage, Size size) {
            return (Image)(new Bitmap(userImage,size));
        }    

        private void picFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.TopMost = true;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }
        
        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if (TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", Directoryname);
            }
            else if (TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", Directoryname);
            }
            else if (TableName == "file_info") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info", Directoryname);
            } else if (TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", Directoryname,IsFromShared);
            }
            this.TopMost = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string[] parts = label1.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text,getExtension,IsFromSharing, TableName, Directoryname);
            _showSharingFileFORM.Show();
        }
    }
}
