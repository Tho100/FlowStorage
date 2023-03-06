using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;

namespace FlowSERVER1 {

    /// <summary>
    /// PDF Viewer form
    /// </summary>
    /// 
    public partial class pdfFORM : Form {
        private static String _TableName;
        private static String _DirName;

        /// <summary>
        /// Load file based on table name 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_tableName"></param>
        /// <param name="_DirectoryName"></param>
        /// <param name="_UploaderName"></param>
        
        public pdfFORM(String _FileTitle, String _tableName, String _DirectoryName,String _UploaderName) {
            InitializeComponent();
            label1.Text = _FileTitle;
            label2.Text = "Uploaded By " + _UploaderName;
            _TableName = _tableName;
            _DirName = _DirectoryName;

            try {

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your portable document.", "Loader").ShowDialog());
                ShowAlert.Start();

                if (_tableName == "file_info_pdf") {
                    setupPdf(LoaderModel.LoadFile("file_info_pdf", _DirectoryName, label1.Text));
                } else if (_tableName == "upload_info_directory") {
                    setupPdf(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                } else if (_tableName == "folder_upload_info") {
                    setupPdf(LoaderModel.LoadFile("folder_upload_info",_DirectoryName,label1.Text));
                }
                else if (_tableName == "cust_sharing") {
                    setupPdf(LoaderModel.LoadFile("cust_sharing", _DirectoryName, label1.Text));
                }
            } catch (Exception) {
                Form bgBlur = new Form();
                using (errorLoad displayError = new errorLoad()) {
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.FormBorderStyle = FormBorderStyle.None;
                    bgBlur.Opacity = .24d;
                    bgBlur.WindowState = FormWindowState.Maximized;
                    bgBlur.TopMost = true;
                    bgBlur.BackColor = Color.Black;
                    bgBlur.Location = this.Location;
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.ShowInTaskbar = false;
                    bgBlur.Show();

                    displayError.Owner = bgBlur;
                    displayError.ShowDialog();

                    bgBlur.Dispose();
                }
                //MessageBox.Show("Failed to load this file.","Flowstorage");
            }
        }
        // @SUMMARY Convert bytes of PDF file to stream
        public void setupPdf(byte[] pdfBytes) {
            if(pdfBytes != null) {
                var _getStream = new MemoryStream(pdfBytes);
                LoadPdf(_getStream);
            }
        }
        // @SUMMARY Load stream of bytes to pdf renderer
        public void LoadPdf(Stream stream) {
            pdfDocumentViewer1.LoadFromStream(stream);
            //var _pdfDocumentSetup = PdfDocument.Load(stream);
            //pdfRenderer1.Load(_pdfDocumentSetup); // original control: pdfRenderer
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
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
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirName);
            }
            else if (_TableName == "file_info_pdf") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_pdf", _DirName);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirName);
            }
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
    }
}
