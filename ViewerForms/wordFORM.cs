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
using System.IO;
using System.Threading;

namespace FlowSERVER1 {
    public partial class wordFORM : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public string _TableName;
        public string _DirectoryName;
        private bool _IsFromShared;
        private bool IsFromSharing;  

        /// <summary>
        /// 
        /// Load user docx,doc, document based on table name
        /// 
        /// </summary>
        /// <param name="_docName"></param>
        /// <param name="_Table"></param>
        /// <param name="_Directory"></param>
        /// <param name="_UploaderName"></param>

        public wordFORM(String fileName,String tableName, String directoryName, String uploaderName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._TableName = tableName;
            this._DirectoryName = directoryName;
            this._IsFromShared = _isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isFromShared == true) {
                label4.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
            }
            else {
                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";
            }

            if (Globals.publicTablesPs.Contains(tableName)) {
                label4.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(tableName: tableName, fileName: fileName, uploaderName: uploaderName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
            }

            lblUploaderName.Text = uploaderName;

            try {

                new Thread(() => new RetrievalAlert("Flowstorage is retrieving your document.","Loader").ShowDialog()).Start();

                setupDocx(LoaderModel.LoadFile(_TableName, _DirectoryName, lblFileName.Text, _isFromShared));

                CloseForm.closeForm("RetrievalAlert");
            }
            catch (Exception) {
                MessageBox.Show("Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void setupDocx(byte[] _getByte) {

            lblFileSize.Text = $"{FileSize.fileSize(_getByte):F2}Mb";

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
            SaverModel.SaveSelectedFile(lblFileName.Text,_TableName,_DirectoryName,_IsFromShared);
            this.TopMost = true;
        }

        /// <summary>
        /// 
        /// Maximize form
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        /// <summary>
        ///
        /// Set form to normal size
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        /// <summary>
        /// 
        /// Minimize form
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// Open share file form
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(lblFileName.Text, getExtension, IsFromSharing, _TableName, _DirectoryName);
            _showSharingFileFORM.Show();

        }

        private void label6_Click(object sender, EventArgs e) {

        }


        /// <summary>
        /// 
        /// Update comment
        /// 
        /// </summary>
        /// <param name="updatedComment"></param>
        /// <returns></returns>
        private async Task saveChangesComment(String updatedComment) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            btnEditComment.Visible = false;
            guna2Button12.Visible = true;
            lblUserComment.Visible = false;
            guna2TextBox4.Text = lblUserComment.Text;
        }

        private async void guna2Button12_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            lblUserComment.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button12.Visible = false;
            guna2TextBox4.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }
    }
}
