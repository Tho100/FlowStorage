using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlowSERVER1 {

    /// <summary>
    /// PDF Viewer form
    /// </summary>
    /// 
    public partial class pdfFORM : Form {
        private static String _TableName;
        private static String _DirName;
        private static bool _IsFromShared;
        private static bool IsFromSharing;

        private static MySqlConnection con = ConnectionModel.con;

        /// <summary>
        /// Load file based on table name 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_tableName"></param>
        /// <param name="_DirectoryName"></param>
        /// <param name="_UploaderName"></param>

        public pdfFORM(String _FileTitle, String _tableName, String _DirectoryName,String _UploaderName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            label1.Text = _FileTitle;
            _TableName = _tableName;
            _DirName = _DirectoryName;
            _IsFromShared = _isFromShared;
            IsFromSharing = _isFromSharing;
            if (_isShared == true) {

                guna2Button6.Visible = true;
                guna2Button6.Visible = true;

                _getName = _UploaderName.Replace("Shared", "");
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

            try {

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your portable document.", "Loader").ShowDialog());
                ShowAlert.Start();

                if (_tableName == "file_info_pdf") {
                    setupPdf(LoaderModel.LoadFile("file_info_pdf", _DirectoryName, label1.Text));
                } else if (_tableName == "upload_info_directory") {
                    setupPdf(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                } else if (_tableName == "folder_upload_info") {
                    setupPdf(LoaderModel.LoadFile("folder_upload_info",_DirectoryName,label1.Text));
                }
                else if (_tableName == "cust_sharing") {
                    setupPdf(LoaderModel.LoadFile("cust_sharing", _DirectoryName, label1.Text,_isFromShared));
                }

            } catch (Exception) {
                MessageBox.Show("Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746")); using (MySqlDataReader readerComment = command.ExecuteReader()) {
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746")); using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }


        // @SUMMARY Convert bytes of PDF file to stream
        public void setupPdf(byte[] pdfBytes) {
            if(pdfBytes != null) {
                label16.Text = $"{FileSize.fileSize(pdfBytes):F2}Mb";
                var _getStream = new MemoryStream(pdfBytes);
                LoadPdf(_getStream);
            }
        }
        // @SUMMARY Load stream of bytes to pdf renderer
        public void LoadPdf(Stream stream) {
            pdfDocumentViewer1.LoadFromStream(stream);
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void pdfFORM_Load(object sender, EventArgs e) {

        }
        /// <summary>
        /// Save file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
            }
            else if (_TableName == "file_info_pdf") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_pdf", _DirName);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName,_IsFromShared);
            }
            this.TopMost = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void pdfViewer1_Load(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void pdfRenderer1_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "bgBlurForm"))
              .ToList()
              .ForEach(form => form.Hide());
        }

        private void pdfDocumentViewer1_Click_1(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = label1.Text.Substring(label1.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirName);
            _showSharingFileFORM.Show();
        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label16_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

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

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button6.Visible = false;
            guna2Button7.Visible = true;
            label3.Visible = false;
            guna2TextBox4.Text = label3.Text;
        }

        private async void guna2Button7_Click(object sender, EventArgs e) {
            if (label3.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            label3.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label3.Text;
            guna2Button6.Visible = true;
            guna2Button7.Visible = false;
            guna2TextBox4.Visible = false;
            label3.Visible = true;
            label3.Refresh();
        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }
    }
}
