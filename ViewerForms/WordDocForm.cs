using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using System;
using System.IO;
using System.Windows.Forms;

namespace FlowstorageDesktop {
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

        public WordDocForm(string fileName, string tableName, string directoryName, string uploaderName, bool isFromShared = false, bool isFromSharing = true) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            label4.Text = isFromShared ? "Shared To" : "Uploaded By";

            lblUserComment.Visible = true;

            if (isFromShared) {
                string comment = GetComment.getCommentSharedToOthers(fileName: fileName);
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
                btnEditComment.Visible = true;
                btnShareFile.Visible = false;

            } else {

                if (GlobalsTable.publicTables.Contains(tableName) || tableName == GlobalsTable.directoryUploadTable || tableName == GlobalsTable.folderUploadTable) {
                    lblUserComment.Text = "(No Comment)";

                } else if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                    string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                    lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

                } else {
                    string comment = GetComment.getCommentSharedToMe(fileName: fileName);
                    lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

                }
            }

            lblUploaderName.Text = uploaderName;

            try {

                StartPopupForm.StartRetrievalPopup();

                InitializeDoc(LoaderModel.LoadFile(_tableName, _directoryName, lblFileName.Text, _isFromShared));

                ClosePopupForm.CloseRetrievalPopup();

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

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();
        
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

        /// <summary>
        /// 
        /// Open share file form
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button5_Click(object sender, EventArgs e) {
            new ShareSelectedFileForm(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();

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

            lblUserComment.Text = txtFieldComment.Text != string.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button12.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

    }
}
