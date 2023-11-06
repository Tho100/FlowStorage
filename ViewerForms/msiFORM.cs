using FlowSERVER1.Global;
using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class MsiForm : Form {
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }

        public MsiForm(String fileName, String tableName, String directoryName, String uploaderUsername, bool isFromShared = false) {
            InitializeComponent();

            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;

            if (isFromShared == true) {
                btnEditComment.Visible = true;
                guna2Button9.Visible = true;

                label6.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";

            } else {
                label6.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";

            }

            if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                label6.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

            }

            lblUploaderName.Text = uploaderUsername;

        }

        private void msiFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName);
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void btnShareFile_Click(object sender, EventArgs e) {
            string[] parts = lblFileName.Text.Split('.');
            string getExtension = "." + parts[1];
            new shareFileFORM(lblFileName.Text, getExtension,
                _isFromShared, _tableName, _directoryName).Show();
        }
    }
}
