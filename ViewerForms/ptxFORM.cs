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
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {

    /// <summary>
    /// Presentation viewer form
    /// </summary>
    public partial class ptxFORM : Form {

        private MySqlConnection con = ConnectionModel.con;

        private string _TableName;
        private string _DirectoryName;
        private bool _IsFromShared;
        private bool IsFromSharing;  

        /// <summary>
        /// 
        /// Load file based on table name
        /// 
        /// </summary>
        /// <param name="_Title"></param>
        /// <param name="_Table"></param>
        /// <param name="_Directory"></param>
        /// <param name="_UploaderName"></param>

        public ptxFORM(String _Title,String _Table, String _Directory,String _UploaderName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            this.label1.Text = _Title;
            this._TableName = _Table;
            this._DirectoryName = _Directory;
            this._IsFromShared = _isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isShared == true) {

                guna2Button6.Visible = true;
                guna2Button7.Visible = true;

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

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your presentation.", "Loader").ShowDialog());
                ShowAlert.Start();

                if (_TableName == "file_info_ptx") {   
                    setupPtx(LoaderModel.LoadFile("file_info_ptx",_DirectoryName,label1.Text));
                } else if (_TableName == "upload_info_directory") {
                    setupPtx(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                }
                else if (_TableName == "folder_upload_info") { 
                    setupPtx(LoaderModel.LoadFile("folder_upload_info", _DirectoryName, label1.Text));
                }
                else if (_TableName == "cust_sharing") {
                    setupPtx(LoaderModel.LoadFile("cust_sharing", _DirectoryName, label1.Text,_isFromShared));
                }

            }  catch (Exception) {
                MessageBox.Show("Failed to load this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private string getCommentSharedToMe() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text)); using (MySqlDataReader readerComment = command.ExecuteReader()) {
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text)); using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (readerComment.Read()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        public void setupPtx(Byte[] _getByte) {
            var _memoryStream = new MemoryStream(_getByte);
            LoadPtx(_memoryStream);
        }
        public void LoadPtx(Stream _getStream) {
            officeViewer1.LoadFromStream(_getStream);
           
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirectoryName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            }
            else if (_TableName == "file_info_ptx") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_ptx", _DirectoryName);
            } else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirectoryName,_IsFromShared);
            }
            this.TopMost = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            label1.AutoSize = false;
            label2.AutoSize = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
            label2.AutoSize = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ptxFORM_Load(object sender, EventArgs e) {

        }

        private void officeViewer1_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "bgBlurForm"))
              .ToList()
              .ForEach(form => form.Hide());
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = label1.Text.Substring(label1.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text, getExtension, IsFromSharing, _TableName, _DirectoryName);
            _showSharingFileFORM.Show();
        }

        private async Task saveChangesComment(String updatedComment) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Globals.custUsername);
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
    }
}