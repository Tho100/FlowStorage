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
    public partial class gifFORM : Form {
        public static gifFORM instance;
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static String _TableName;
        public gifFORM(String _titleName,String _tableName) {
            InitializeComponent();
            var _form = Form1.instance;
            instance = this;
            label2.Text = "Uploaded By " + _form.label5.Text;
            label1.Text = _titleName;
            _TableName = _tableName;

            try {

                if(_TableName == "file_info_gif") {
                    String _readGifFiles = "SELECT CUST_FILE FROM file_info_gif WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username",_form.label5.Text);
                    command.Parameters.AddWithValue("@filepath",_titleName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();
                    if(_readByteValues.Read()) {
                        var _byteValues = (byte[])_readByteValues["CUST_FILE"];
                        MemoryStream _memStream = new MemoryStream(_byteValues);
                        guna2PictureBox1.Image = Image.FromStream(_memStream);
                        guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    } 
                    _readByteValues.Close();
                } else if (_TableName == "upload_info_directory") {
                    String _readGifFiles = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", _form.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _titleName);
                    MessageBox.Show("Title name: " + _titleName,"Dir name: " + label1.Text);

                    MySqlDataReader _readByteValues = command.ExecuteReader();
                    if (_readByteValues.Read()) {
                        var _byteValues = (byte[])_readByteValues["CUST_FILE"];
                        MemoryStream _memStream = new MemoryStream(_byteValues);
                        guna2PictureBox1.Image = Image.FromStream(_memStream);
                        guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    _readByteValues.Close();
                }
            } catch (Exception eq) {
                MessageBox.Show("Failed to play the file.","Flowstorage");
            }
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
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void gifFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {

                String _readGifFiles = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

                command = new MySqlCommand(_readGifFiles, con);
                command = con.CreateCommand();
                command.CommandText = _readGifFiles;
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filetitle", label1.Text);

                MySqlDataReader _gifReader = command.ExecuteReader();
                if (_gifReader.Read()) {
                    var get_apkValues = (byte[])_gifReader["CUST_FILE"];
                    SaveFileDialog _OpenDialog = new SaveFileDialog();
                    _OpenDialog.Filter = "Gif|*.gif";
                    if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_OpenDialog.FileName, get_apkValues);
                    }
                }
                _gifReader.Close();
            } catch (Exception eq) {
                MessageBox.Show("Failed to download this file.","Flowstorage");
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
