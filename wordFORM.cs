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

namespace FlowSERVER1 {
    public partial class wordFORM : Form {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public wordFORM(String _docName) {
            InitializeComponent();

            label1.Text = _docName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;

            try {
                String _getDocByte = "SELECT CUST_FILE FROM file_info_word WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = con.CreateCommand();
                command.CommandText = _getDocByte;
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);

                MySqlDataReader _readerBytes = command.ExecuteReader();
                if (_readerBytes.Read()) {
                    var _getBytes = (byte[])_readerBytes["CUST_FILE"];
                    setupDocx(_getBytes);
                }
                _readerBytes.Close();
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to load this document.", "Flowstorage");
            }
        }

        public void setupDocx(Byte[] _getByte) {
            var _getStream = new MemoryStream(_getByte);
            loadDocx(_getStream);
        }

        public void loadDocx(Stream _getStream) {
           docDocumentViewer1.LoadFromStream(_getStream, Spire.Doc.FileFormat.Docx);
        }

        private void wordFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                String _retrieveBytes = "SELECT CUST_FILE FROM file_info_word WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = con.CreateCommand();
                command.CommandText = _retrieveBytes;
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);

                MySqlDataReader _byteReader = command.ExecuteReader();
                if (_byteReader.Read()) {
                    var _getBytes = (byte[])_byteReader["CUST_FILE"];
                    SaveFileDialog _dialog = new SaveFileDialog();
                    _dialog.Filter = "Word Document|*.docx";
                    if (_dialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_dialog.FileName, _getBytes);
                    }
                }
                _byteReader.Close();
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to download this file.", "Flowstorage");
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }
    }
}
