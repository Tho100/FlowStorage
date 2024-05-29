using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class WordsForm : Form {

        readonly public WordsForm instance;
        public string _tableName { get; set; }
        public string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }

        public WordsForm(string fileName, string tableName, string directoryName, string uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            instance = this;

            lblFileName.Text = fileName;

            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            label6.Text = isFromShared ? "Shared To" : "Uploaded By";

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

        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            new ShareSelectedFileForm(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button9.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button9_Click(object sender, EventArgs e) {

            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != string.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button9.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();

        }

    }
}
