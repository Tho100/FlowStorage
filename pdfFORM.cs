using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
//using PdfiumViewer;

namespace FlowSERVER1 {
    public partial class pdfFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _TableName;
        public static String _DirName;
        public pdfFORM(String _FileTitle, String _tableName, String _DirectoryName) {
            InitializeComponent();
            label1.Text = _FileTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            _TableName = _tableName;
            _DirName = _DirectoryName;

            try {

                if(_tableName == "file_info_pdf") {
                    String _getPdfBytes = "SELECT CUST_FILE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(_getPdfBytes,con);
                    command = con.CreateCommand();
                    command.CommandText = _getPdfBytes;
                    command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);

                    MySqlDataReader _readBytes = command.ExecuteReader();
                    if(_readBytes.Read()) { 
                        var getPdfValues = (byte[])_readBytes["CUST_FILE"];
                        setupPdf(getPdfValues);
                    }
                    _readBytes.Close(); 
                } else if (_tableName == "upload_info_directory") {
                    String _getPdfBytes = "SELECT CUST_FILE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command = new MySqlCommand(_getPdfBytes, con);
                    command = con.CreateCommand();
                    command.CommandText = _getPdfBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);
                    command.Parameters.AddWithValue("@dirname", _DirName);

                    MySqlDataReader _readBytes = command.ExecuteReader();
                    if (_readBytes.Read()) {
                        var getPdfValues = (byte[])_readBytes["CUST_FILE"];
                        setupPdf(getPdfValues);
                    }
                    _readBytes.Close();
                }
            } catch (Exception eq) {
                MessageBox.Show("Failed to load this file.","Flowstorage");
            }
        }
        // @SUMMARY Convert bytes of PDF file to stream
        public void setupPdf(byte[] pdfBytes) {
            var _getStream = new MemoryStream(pdfBytes);
            LoadPdf(_getStream);
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

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                if(_TableName == "file_info_pdf") {
                    String _getPdfBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(_getPdfBytes, con);
                    command = con.CreateCommand();
                    command.CommandText = _getPdfBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);

                    MySqlDataReader _readBytes = command.ExecuteReader();
                    if (_readBytes.Read()) {
                        SaveFileDialog _openDialog = new SaveFileDialog();
                        _openDialog.Filter = "Acrobat Files|*.pdf";
                        _openDialog.FileName = label1.Text;
                        if(_openDialog.ShowDialog() == DialogResult.OK) {
                            var _getBytes = (byte[])_readBytes["CUST_FILE"];
                            File.WriteAllBytes(_openDialog.FileName,_getBytes);
                        }
                    }
                    _readBytes.Close();
                } else if (_TableName == "upload_info_directory") {
                    String _getPdfBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command = new MySqlCommand(_getPdfBytes, con);
                    command = con.CreateCommand();
                    command.CommandText = _getPdfBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);
                    command.Parameters.AddWithValue("@dirname", _DirName);

                    MySqlDataReader _readBytes = command.ExecuteReader();
                    if (_readBytes.Read()) {
                        SaveFileDialog _openDialog = new SaveFileDialog();
                        _openDialog.Filter = "Acrobat Files|*.pdf";
                        _openDialog.FileName = label1.Text;
                        if (_openDialog.ShowDialog() == DialogResult.OK) {
                            var _getBytes = (byte[])_readBytes["CUST_FILE"];
                            File.WriteAllBytes(_openDialog.FileName, _getBytes);
                        }
                    }
                    _readBytes.Close();
                }
            } catch (Exception eq) {
                MessageBox.Show("Failed to download this file.","Flowstorage");
            }
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
    }
}
