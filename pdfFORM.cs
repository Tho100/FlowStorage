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
using PdfiumViewer;
using System.IO;
using System.Dynamic;

namespace FlowSERVER1 {
    public partial class pdfFORM : Form {
        public static string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
        public static string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
        public static string username = "root"; // epiz_33067528 | root
        public static string password = "nfreal-yt10";
        public static int mainPort_ =  11160;
        public static string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
        public MySqlConnection con = new MySqlConnection(constring);
        public MySqlCommand command;
        public pdfFORM(String _FileTitle) {
            InitializeComponent();
            label1.Text = _FileTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;

            con.Open();
            // @ SELECT PDF BYTES

            String _getPdfBytes = "SELECT CUST_FILE FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(_getPdfBytes,con);
            command = con.CreateCommand();
            command.CommandText = _getPdfBytes;
            command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", label1.Text);

            MySqlDataReader _readBytes = command.ExecuteReader();
            if(_readBytes.Read()) { 
                var getPdfValues = (byte[])_readBytes["CUST_FILE"];
                //LoadPdf(getPdfValues);
            }
            _readBytes.Close();
        }

        public void LoadPdf(byte[] pdfBytes) {
            var stream = new MemoryStream(pdfBytes);
            LoadPdf(stream);
        }

        public void LoadPdf(Stream stream) {
            // Create PDF Document
            var pdfDocument = PdfDocument.Load(stream);

            // Load PDF Document into WinForms Control
            //pdfViewer1.Load(pdfDocument);
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
            String _getPdfBytes = "SELECT CUST_FILE FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(_getPdfBytes, con);
            command = con.CreateCommand();
            command.CommandText = _getPdfBytes;
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", label1.Text);

            MySqlDataReader _readBytes = command.ExecuteReader();
            if (_readBytes.Read()) {
                SaveFileDialog _openDialog = new SaveFileDialog();
                _openDialog.Filter = "Acrobat Files|*.pdf";
                if(_openDialog.ShowDialog() == DialogResult.OK) {
                    var _getBytes = (byte[])_readBytes["CUST_FILE"];
                    File.WriteAllBytes(_openDialog.FileName,_getBytes);
                }
            }
            _readBytes.Close();
        }

        private void pdfRenderer1_Click(object sender, EventArgs e) {
            MessageBox.Show("FUCK YOU LOL");
        }
    }
}
