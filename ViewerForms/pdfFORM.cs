﻿using System;
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

        private string _TableName;
        private string _DirName;
        private bool _IsFromShared;
        private bool IsFromSharing;

        readonly private MySqlConnection con = ConnectionModel.con;

        /// <summary>
        /// Load file based on table name 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_tableName"></param>
        /// <param name="_DirectoryName"></param>
        /// <param name="_UploaderName"></param>

        public pdfFORM(String fileName, String tableName, String directoryName,String uploaderName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(uploaderName, @"^([\w\-]+)").Value == "Shared";

            this.lblFileName.Text = fileName;
            this._TableName = tableName;
            this._DirName = directoryName;
            this._IsFromShared = _isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                btnEditComment.Visible = true;
                btnEditComment.Visible = true;

                _getName = uploaderName.Replace("Shared", "");
                label4.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
            }
            else {
                _getName = " " + uploaderName;
                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";
            }

            lblUploaderName.Text = _getName;

            try {

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your portable document.", "Loader").ShowDialog());
                ShowAlert.Start();

                if (tableName == "file_info_pdf") {
                    setupPdf(LoaderModel.LoadFile("file_info_pdf", directoryName, lblFileName.Text));
                } else if (tableName == "upload_info_directory") {
                    setupPdf(LoaderModel.LoadFile("upload_info_directory", directoryName, lblFileName.Text));
                } else if (tableName == "folder_upload_info") {
                    setupPdf(LoaderModel.LoadFile("folder_upload_info", directoryName, lblFileName.Text));
                }
                else if (tableName == "cust_sharing") {
                    setupPdf(LoaderModel.LoadFile("cust_sharing", directoryName, lblFileName.Text,_isFromShared));
                }

            } catch (Exception) {
                MessageBox.Show("Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 
        /// Convert PDF bytes to stream and load it into viewer
        /// 
        /// </summary>
        /// <param name="pdfBytes"></param>
        public void setupPdf(byte[] pdfBytes) {
            if(pdfBytes != null) {
                lblFileSize.Text = $"{FileSize.fileSize(pdfBytes):F2}Mb";
                var _getStream = new MemoryStream(pdfBytes);
                LoadPdf(_getStream);
            }
        }
        /// <summary>
        /// 
        /// Load stream of PDF bytes into viewer
        /// 
        /// </summary>
        /// <param name="stream"></param>
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
                SaverModel.SaveSelectedFile(lblFileName.Text, "upload_info_directory", _DirName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "folder_upload_info", _DirName);
            }
            else if (_TableName == "file_info_pdf") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "file_info_pdf", _DirName);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "cust_sharing", _DirName,_IsFromShared);
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
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(lblFileName.Text, getExtension, IsFromSharing, _TableName, _DirName);
            _showSharingFileFORM.Show();
        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label16_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

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

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            btnEditComment.Visible = false;
            guna2Button7.Visible = true;
            lblUserComment.Visible = false;
            guna2TextBox4.Text = lblUserComment.Text;
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

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }
    }
}
