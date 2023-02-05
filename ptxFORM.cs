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

namespace FlowSERVER1 {

    /// <summary>
    /// Presentation viewer form
    /// </summary>
    public partial class ptxFORM : Form {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static String _TableName;
        public static String _DirectoryName;

        /// <summary>
        /// 
        /// Load file based on table name
        /// 
        /// </summary>
        /// <param name="_Title"></param>
        /// <param name="_Table"></param>
        /// <param name="_Directory"></param>
        /// <param name="_UploaderName"></param>


        public ptxFORM(String _Title,String _Table, String _Directory,String _UploaderName) {
            InitializeComponent();
            label1.Text = _Title;
            label2.Text = "Uploaded By " + _UploaderName;
            _TableName = _Table;
            _DirectoryName = _Directory;

            try {

                Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your presentation.", "Loader").ShowDialog());
                ShowAlert.Start();
                Application.DoEvents();

                if (_TableName == "file_info_ptx") {   
                    setupPtx(LoaderModel.LoadFile("file_info_ptx",_DirectoryName,label1.Text));
                } else if (_TableName == "upload_info_directory") {
                    setupPtx(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                }
                else if (_TableName == "folder_upload_info") { 
                    setupPtx(LoaderModel.LoadFile("folder_upload_info", _DirectoryName, label1.Text));
                }
                else if (_TableName == "cust_sharing") {
                    setupPtx(LoaderModel.LoadFile("cust_sharing", _DirectoryName, label1.Text));
                }
            }  catch (Exception) {
                Form bgBlur = new Form();
                using (errorLoad displayError = new errorLoad()) {
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.FormBorderStyle = FormBorderStyle.None;
                    bgBlur.Opacity = .24d;
                    bgBlur.BackColor = Color.Black;
                    bgBlur.WindowState = FormWindowState.Maximized;
                    bgBlur.TopMost = true;
                    bgBlur.Location = this.Location;
                    bgBlur.StartPosition = FormStartPosition.Manual;
                    bgBlur.ShowInTaskbar = false;
                    bgBlur.Show();

                    displayError.Owner = bgBlur;
                    displayError.ShowDialog();

                    bgBlur.Dispose();
                }
            }
        }

        public void setupPtx(Byte[] _getByte) {
            var _memoryStream = new MemoryStream(_getByte);
            LoadPtx(_memoryStream);
        }
        public void LoadPtx(Stream _getStream) {
            //var _ptxSetup = PdfDocument.Load(_getStream);
            officeViewer1.LoadFromStream(_getStream);
           
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _DirectoryName);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            }
            else if (_TableName == "file_info_ptx") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_ptx", _DirectoryName);
            } else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _DirectoryName);
            }
            this.TopMost = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            label1.AutoSize = false;
            label2.AutoSize = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
            label2.AutoSize = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ptxFORM_Load(object sender, EventArgs e) {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {

        }

        private void pdfViewer1_Load(object sender, EventArgs e) {

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
    }
}
