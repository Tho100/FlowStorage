using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FlowSERVER1 {

    /// <summary>
    /// PDF Viewer form
    /// </summary>
    /// 
    public partial class PdfForm : Form {

        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromSharing { get; set; }

        /// <summary>
        /// Load file based on table name 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_tableName"></param>
        /// <param name="_DirectoryName"></param>
        /// <param name="_UploaderName"></param>

        public PdfForm(String fileName, String tableName, String directoryName, String uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            this.lblFileName.Text = fileName;
            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromSharing = isFromSharing;

            if (isFromShared == true) {
                btnEditComment.Visible = true;

                label4.Text = "Shared To";
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

                new Thread(() => new RetrievalAlert("Flowstorage is retrieving your portable document.", "Loader").
                ShowDialog()).Start();

                InitializePDF(LoaderModel.LoadFile(tableName, directoryName, lblFileName.Text, isFromShared));

            } catch (Exception) {
                MessageBox.Show("Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /// <summary>
        /// 
        /// Convert PDF bytes to stream and load it into viewer
        /// 
        /// </summary>
        /// <param name="pdfBytes"></param>
        private void InitializePDF(byte[] pdfBytes) {
            if (pdfBytes != null) {
                lblFileSize.Text = $"{GetFileSize.fileSize(pdfBytes):F2}Mb";
                MemoryStream convertToStream = new MemoryStream(pdfBytes);
                pdfDocumentViewer1.LoadFromStream(convertToStream);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void pdfFORM_Load(object sender, EventArgs e) {

        }
        /// <summary>
        /// Save file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;

            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromSharing);

            this.TopMost = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void pdfViewer1_Load(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void pdfRenderer1_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "bgBlurForm"))
              .ToList()
              .ForEach(form => form.Hide());
        }

        private void pdfDocumentViewer1_Click_1(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            new shareFileFORM(lblFileName.Text, getExtension,
                _isFromSharing, _tableName, _directoryName).Show();
        }

        private void label6_Click(object sender, EventArgs e) {

        }

        private void label16_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button7.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button7_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button7.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }
    }
}
