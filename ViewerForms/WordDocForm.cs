using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class WordDocForm : Form {

        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }

        /// <summary>
        /// 
        /// Load user docx,doc, document based on table name
        /// 
        /// </summary>
        /// <param name="_docName"></param>
        /// <param name="_Table"></param>
        /// <param name="_Directory"></param>
        /// <param name="_UploaderName"></param>

        public WordDocForm(String fileName, String tableName, String directoryName, String uploaderName, bool isFromShared = false, bool isFromSharing = true) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            if (_isFromShared == true) {
                label4.Text = "Shared To";
                btnEditComment.Visible = true;
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";

            } else {
                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";

            }

            if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                label4.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

            }

            lblUploaderName.Text = uploaderName;

            try {

                new Thread(() => new RetrievalAlert("Flowstorage is retrieving your document.", "Loader").ShowDialog()).Start();

                InitializeDoc(LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text, _isFromShared));

                CloseForm.CloseRetrievalPopup();

            } catch (Exception) {
                MessageBox.Show(
                    "Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void InitializeDoc(byte[] docBytes) {

            if (docBytes != null) {

                lblFileSize.Text = $"{GetFileSize.fileSize(docBytes):F2}Mb";
                MemoryStream docStream = new MemoryStream(docBytes);
                documentViewer1.LoadFromStream(docStream);
            }

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
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
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
            new shareFileFORM(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();

        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button12.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button12_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button12.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {

        }

        private void docDocumentViewer1_Click(object sender, EventArgs e) {

        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {

        }

        private void docViewer1_Click(object sender, EventArgs e) {

        }

        private void documentViewer1_Click(object sender, EventArgs e) {

        }
    }
}
