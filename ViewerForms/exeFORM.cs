using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class exeFORM : Form {

        readonly public exeFORM instance;
        public string _tableName { get; set; }
        public string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }

        public exeFORM(String fileName, String tableName, String directoryName, String uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            instance = this;

            lblFileName.Text = fileName;

            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

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

            lblUploaderName.Text = uploaderName;

        }
        private void exeFORM_Load(object sender, EventArgs e) {
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }
        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string[] parts = lblFileName.Text.Split('.');
            string getExtension = "." + parts[1];
            new shareFileFORM(lblFileName.Text, getExtension,
                _isFromSharing, _tableName, _directoryName).Show();
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

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button9.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }
    }
}
