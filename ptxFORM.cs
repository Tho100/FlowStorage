﻿using System;
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

namespace FlowSERVER1 {
    public partial class ptxFORM : Form {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static String _TableName;
        public static String _DirectoryName;

        public ptxFORM(String _Title,String _Table, String _Directory) {
            InitializeComponent();
            label1.Text = _Title;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            _TableName = _Table;
            _DirectoryName = _Directory;
            try {
                if(_TableName == "file_info_ptx") {
                    String _readPtxValues = "SELECT CUST_FILE FROM " + _Table + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

                    command = new MySqlCommand(_readPtxValues, con);
                    command = con.CreateCommand();
                    command.CommandText = _readPtxValues;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle", label1.Text);
                
                    MySqlDataReader _ptxReader = command.ExecuteReader();
                    if (_ptxReader.Read()) {
                        var get_ptxValues = (byte[])_ptxReader["CUST_FILE"];
                        setupPtx(get_ptxValues);
                    }
                    _ptxReader.Close();
                } else {
                    String _readPtxValues = "SELECT CUST_FILE FROM " + _Table + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle AND DIR_NAME = @dirname";

                    command = new MySqlCommand(_readPtxValues, con);
                    command = con.CreateCommand();
                    command.CommandText = _readPtxValues;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle", label1.Text);
                    command.Parameters.AddWithValue("@dirname", _Directory);

                    MySqlDataReader _ptxReader = command.ExecuteReader();
                    if (_ptxReader.Read()) {
                        var get_ptxValues = (byte[])_ptxReader["CUST_FILE"];
                        setupPtx(get_ptxValues);
                    }
                    _ptxReader.Close();
                }
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
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
            try {
                if(_TableName == "file_info_ptx") {
                    String _readPtxValues = "SELECT CUST_FILE FROM " + _TableName + "  WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

                    command = new MySqlCommand(_readPtxValues, con);
                    command = con.CreateCommand();
                    command.CommandText = _readPtxValues;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle", label1.Text);
                
                    MySqlDataReader _ptxReader = command.ExecuteReader();
                    if (_ptxReader.Read()) {
                        var get_apkValues = (byte[])_ptxReader["CUST_FILE"];
                        SaveFileDialog _OpenDialog = new SaveFileDialog();
                        _OpenDialog.Filter = "PowerPoint|*.pptx;*.ppt";
                        _OpenDialog.FileName = label1.Text;
                        if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                            File.WriteAllBytes(_OpenDialog.FileName, get_apkValues);
                        }
                    }
                    _ptxReader.Close();
                } else {
                    String _readPtxValues = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle AND DIR_NAME = @dirname";

                    command = new MySqlCommand(_readPtxValues, con);
                    command = con.CreateCommand();
                    command.CommandText = _readPtxValues;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle", label1.Text);
                    command.Parameters.AddWithValue("@dirname", _DirectoryName);

                    MySqlDataReader _ptxReader = command.ExecuteReader();
                    if (_ptxReader.Read()) {
                        var get_ptxValues = (byte[])_ptxReader["CUST_FILE"];
                        setupPtx(get_ptxValues);
                    }
                    _ptxReader.Close();
                }

            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
                //MessageBox.Show("Failed to download this file.","Flowstorage");
            }
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
