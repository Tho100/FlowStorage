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
    /// <summary>
    /// GIF viewer form
    /// </summary>
    public partial class gifFORM : Form {
        public static gifFORM instance;
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static String _TableName;
        public static String _Directory;

        /// <summary>
        /// 
        /// Load GIF based on table name
        /// 
        /// </summary>
        /// <param name="_titleName"></param>
        /// <param name="_tableName"></param>
        /// <param name="_directoryName"></param>
        /// <param name="_uploaderName"></param>

        public gifFORM(String _titleName,String _tableName, String _directoryName,String _uploaderName) {
            InitializeComponent();
            var _form = Form1.instance;
            instance = this;
            label2.Text = "Uploaded By " + _uploaderName;
            label1.Text = _titleName;
            _TableName = _tableName;
            _Directory = _directoryName;

            try {
                if(_TableName == "file_info_gif") {                  
                    MemoryStream _memStream = new MemoryStream(LoaderModel.LoadFile("file_info_gif", _directoryName, label1.Text));
                    guna2PictureBox1.Image = Image.FromStream(_memStream);
                    guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                } else if (_TableName == "upload_info_directory") {
                        MemoryStream _memStream = new MemoryStream(LoaderModel.LoadFile("upload_info_directory",_directoryName,label1.Text));
                        guna2PictureBox1.Image = Image.FromStream(_memStream);
                        guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                } else if (_TableName == "folder_upload_info") {
                        MemoryStream _memStream = new MemoryStream(LoaderModel.LoadFile("folder_upload_info",_directoryName,label1.Text));
                        guna2PictureBox1.Image = Image.FromStream(_memStream);
                        guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else if (_TableName == "cust_sharing") {
                        MemoryStream _memStream = new MemoryStream(LoaderModel.LoadFile("cust_sharing", _directoryName, label1.Text));
                        guna2PictureBox1.Image = Image.FromStream(_memStream);
                        guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
            } catch (Exception eq) {
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
            if (_TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", _Directory);
            }
            else if (_TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", _Directory);
            }
            else if (_TableName == "file_info_gif") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info_gif", _Directory);
            }
            else if (_TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", _Directory);
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
