using FlowstorageDesktop.Global;
using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class MsiForm : Form {
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }

        public MsiForm(string fileName, string tableName, string directoryName, string uploaderUsername, bool isFromShared = false) {
            InitializeComponent();

            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;

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

            lblUploaderName.Text = uploaderUsername;

        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName);
        }

        private void btnShareFile_Click(object sender, EventArgs e) {
            new shareFileFORM(
                lblFileName.Text, _isFromShared, _tableName, _directoryName).Show();
        }

    }
}
