using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace FlowSERVER1 {
    public partial class apkFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _TableName;
        public static String _DirName;
        public apkFORM(String _titleFile, String _userName,String _tabName, String _dirName) {
            InitializeComponent();
            label1.Text = _titleFile;
            label2.Text = "Uploaded by " + _userName;
            _TableName = _tabName;
            _DirName = _dirName;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void apkFORM_Load(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            label1.AutoSize = true;
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your APK data.");
                ShowAlert.Show();
                Application.DoEvents();
                if (_TableName == "file_info_apk") {
                    String _readApkFiles = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

                    command = new MySqlCommand(_readApkFiles,con);
                    command = con.CreateCommand();
                    command.CommandText = _readApkFiles;
                    command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle",label1.Text);

                    MySqlDataReader _apkReader = command.ExecuteReader();
                    if(_apkReader.Read()) {
                        Application.OpenForms
                     .OfType<Form>()
                     .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                     .ToList()
                     .ForEach(form => form.Close());
                        var get_apkValues = (byte[])_apkReader["CUST_FILE"];
                        SaveFileDialog _OpenDialog = new SaveFileDialog();
                        _OpenDialog.Filter = "APK|*.apk";
                        _OpenDialog.FileName = label1.Text;
                        if(_OpenDialog.ShowDialog() == DialogResult.OK) {
                            File.WriteAllBytes(_OpenDialog.FileName,get_apkValues);
                        }
                    }
                    _apkReader.Close();
                } else if (_TableName == "upload_info_directory") {
                    String _readApkFiles = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle AND DIR_NAME = @dirname";

                    command = new MySqlCommand(_readApkFiles, con);
                    command = con.CreateCommand();
                    command.CommandText = _readApkFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filetitle", label1.Text);
                    command.Parameters.AddWithValue("@dirname", _DirName);

                    MySqlDataReader _apkReader = command.ExecuteReader();
                    if (_apkReader.Read()) {
                        Application.OpenForms
                     .OfType<Form>()
                     .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                     .ToList()
                     .ForEach(form => form.Close());
                        var get_apkValues = (byte[])_apkReader["CUST_FILE"];
                        SaveFileDialog _OpenDialog = new SaveFileDialog();
                        _OpenDialog.Filter = "APK|*.apk";
                        _OpenDialog.FileName = label1.Text;
                        if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                            File.WriteAllBytes(_OpenDialog.FileName, get_apkValues);
                        }
                    }
                    _apkReader.Close();
                }
                     
                //var get_apkValues = apkValues[0];
                //MemoryStream _msApk = new MemoryStream(apkValues[0]);
                //Byte[] _msByteAr = _msApk.ToArray();
                //_msApk.Close();

            } catch (Exception eq) {
                MessageBox.Show("Failed to download this file.","Flowstorage");
            }
        }
    }
}
