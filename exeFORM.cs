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
using MySql.Data;
using System.IO;
using System.Globalization;

namespace FlowSERVER1 {
    public partial class exeFORM : Form {
        public static exeFORM instance;
        public exeFORM(String getTitle) {
            InitializeComponent();
            label1.Text = getTitle;
            instance = this;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            label3.Text = "405Mb";
        }

        private void exeFORM_Load(object sender, EventArgs e) {
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

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
        }

        private void guna2Button4_Click(object sender, EventArgs e) {

            string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
            string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
            string username = "root"; // epiz_33067528 | root
            string password = "nfreal-yt10";
            int mainPort_ = 13560;
            string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            con.Open();

            String _readExeBytes = "SELECT CUST_FILE FROM file_info_exe WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";
            command = new MySqlCommand(_readExeBytes,con);
            command = con.CreateCommand();
            command.CommandText = _readExeBytes;
            command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filetitle", label1.Text);
            
            MySqlDataReader _exeReader = command.ExecuteReader();
            if(_exeReader.Read()) {
                var _getExeValues = (byte[])_exeReader["CUST_FILE"];
                SaveFileDialog _OpenDialog = new SaveFileDialog();
                _OpenDialog.Filter = "Exe|*.exe";
                if(_OpenDialog.ShowDialog() == DialogResult.OK) {
                    File.WriteAllBytes(_OpenDialog.FileName,_getExeValues);
                }
            }
        }
    }
}
