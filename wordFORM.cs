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
        public static String _TableName;
        public static String _DirectoryName;

        public wordFORM(String _docName,String _Table, String _Directory) {
            InitializeComponent();
            label1.Text = _docName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            _TableName = _Table;
            _DirectoryName = _Directory;

            try {
                if(_TableName == "file_info_word") {
                    setupDocx(LoaderModel.LoadFile("file_info_word","null",label1.Text));
                    /*String _getDocByte = "SELECT CUST_FILE FROM " + _Table + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = con.CreateCommand();
                    command.CommandText = _getDocByte;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);

                    MySqlDataReader _readerBytes = command.ExecuteReader();
                    if (_readerBytes.Read()) {
                        var _getBytes = (byte[])_readerBytes["CUST_FILE"];
                        if(_getBytes != null) {
                            setupDocx(_getBytes);
                        }
                    }
                    _readerBytes.Close();*/
                } else if (_TableName == "upload_info_directory") {
                    setupDocx(LoaderModel.LoadFile("upload_info_directory", _DirectoryName, label1.Text));
                    /*  String _getDocByte = "SELECT CUST_FILE FROM " + _Table + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                      command = con.CreateCommand();
                      command.CommandText = _getDocByte;
                      command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                      command.Parameters.AddWithValue("@filename", label1.Text);
                      command.Parameters.AddWithValue("@dirname", _DirectoryName);

                      MySqlDataReader _readerBytes = command.ExecuteReader();
                      if (_readerBytes.Read()) {
                          var _getBytes = (byte[])_readerBytes["CUST_FILE"];
                          if (_getBytes != null) {
                              setupDocx(_getBytes);
                          }
                      }
                      _readerBytes.Close();*/
                }
                else if (_TableName == "folder_upload_info") {
                    setupDocx(LoaderModel.LoadFile("folder_upload_info",_DirectoryName,label1.Text));
                    /*String _getDocByte = "SELECT CUST_FILE FROM " + _Table + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";
                    command = con.CreateCommand();
                    command.CommandText = _getDocByte;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);
                    command.Parameters.AddWithValue("@foldtitle", _DirectoryName);

                    MySqlDataReader _readerBytes = command.ExecuteReader();
                    if (_readerBytes.Read()) {
                        var _getBytes = (byte[])_readerBytes["CUST_FILE"];
                        if (_getBytes != null) {
                            setupDocx(_getBytes);
                        }
                    }
                    _readerBytes.Close();*/
                }
            }
            catch (Exception) {
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
            if(_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text,"upload_info_directory",_DirectoryName);
            } else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _DirectoryName);
            } else if (_TableName == "file_info_word") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_word", _DirectoryName);
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
