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
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {

    /// <summary>
    /// Presentation viewer form
    /// </summary>
    public partial class ptxFORM : Form {

        private readonly MySqlConnection con = ConnectionModel.con;

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

        public ptxFORM(string fileName,string tableName, string directoryName,string uploaderName, bool _isFromShared = false, bool _isFromSharing = true) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._TableName = tableName;
            this._DirectoryName = directoryName;
            this._IsFromShared = _isFromShared;
            this.IsFromSharing = _isFromSharing;

            if (_isFromShared == true) {

                btnEditComment.Visible = true;
                guna2Button7.Visible = true;

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

            lblUploaderName.Text = uploaderName;

            try {

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your presentation.", "Loader").ShowDialog());
                ShowAlert.Start();

                setupPtx(LoaderModel.LoadFile(_TableName, _DirectoryName, lblFileName.Text, _isFromShared));

            }  catch (Exception) {
                MessageBox.Show("Failed to load this file.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        public void setupPtx(Byte[] _getByte) {
            var _memoryStream = new MemoryStream(_getByte);
            lblFileSize.Text = $"{FileSize.fileSize(_getByte):F2}Mb";
            LoadPtx(_memoryStream);
        }
        public void LoadPtx(Stream _getStream) {
            officeViewer1.LoadFromStream(_getStream);
           
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            SaverModel.SaveSelectedFile(lblFileName.Text, _TableName, _DirectoryName, _IsFromShared);
            this.TopMost = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            lblFileName.AutoSize = false;
            lblUploaderName.AutoSize = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            lblFileName.AutoSize = true;
            lblUploaderName.AutoSize = true;
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
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
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

        private void lblFileSize_Click(object sender, EventArgs e) {

        }
    }
}
