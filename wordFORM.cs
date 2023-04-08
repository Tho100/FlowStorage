﻿using System;
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
using System.Threading;

namespace FlowSERVER1 {
    public partial class wordFORM : Form {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static String _TableName;
        public static String _DirectoryName;
        private static bool _IsFromShared;
        private static bool IsFromSharing;  // Shared to me
        /// <summary>
        /// 
        /// Load user docx,doc, document based on table name
        /// 
        /// </summary>
        /// <param name="_docName"></param>
        /// <param name="_Table"></param>
        /// <param name="_Directory"></param>
        /// <param name="_UploaderName"></param>

        public wordFORM(String _docName,String _Table, String _Directory, String _UploaderName, bool _isFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            String _getName = "";
            bool _isShared = System.Text.RegularExpressions.Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            label1.Text = _docName;
            _TableName = _Table;
            _DirectoryName = _Directory;
            _IsFromShared = _isFromShared;
            IsFromSharing = _isFromSharing;

            if (_isShared == true) {
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

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your document.","Loader").ShowDialog());
                ShowAlert.Start();

                if (_TableName == "file_info_word") {
                    setupDocx(LoaderModel.LoadFile("file_info_word","null",label1.Text));
                } else if (_TableName == "upload_info_directory") {
                    setupDocx(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                }
                else if (_TableName == "folder_upload_info") {
                    setupDocx(LoaderModel.LoadFile("folder_upload_info",_DirectoryName,label1.Text));
                } else if (_TableName == "cust_sharing") {
                    setupDocx(LoaderModel.LoadFile("cust_sharing", _DirectoryName, label1.Text,_isFromShared));
                }
            }
            catch (Exception) {
                Form bgBlur = new Form();
                using (errorLoad displayError = new errorLoad()) {
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.FormBorderStyle = FormBorderStyle.None;
                    bgBlur.Opacity = .24d;
                    bgBlur.BackColor = Color.Black;
                    bgBlur.WindowState = FormWindowState.Maximized;
                    bgBlur.TopMost = true;
                    bgBlur.Location = this.Location;
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.ShowInTaskbar = false;
                    bgBlur.Show();

                    displayError.Owner = bgBlur;
                    displayError.ShowDialog();

                    bgBlur.Dispose();
                }
            }
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
                using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = readerComment.GetString(0);
                    }
                }
            }
            return returnComment;
        }
        public void setupDocx(Byte[] _getByte) {
            var _getStream = new MemoryStream(_getByte);
            loadDocx(_getStream);
            
        }

        public void loadDocx(Stream _getStream) {
           docDocumentViewer1.LoadFromStream(_getStream, Spire.Doc.FileFormat.Docx);
        }

        private void wordFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if(_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text,"upload_info_directory",_DirectoryName);
            } else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            } else if (_TableName == "file_info_word") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_word", _DirectoryName);
            } else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirectoryName,_IsFromShared);
            }
            this.TopMost = true;
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

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string[] parts = label1.Text.Split('.');
            string getExtension = "." + parts[1];
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirectoryName);
            _showSharingFileFORM.Show();

        }

        private void label6_Click(object sender, EventArgs e) {

        }
    }
}
