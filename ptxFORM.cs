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

namespace FlowSERVER1 {
    public partial class ptxFORM : Form {
        public ptxFORM(String _Title) {
            InitializeComponent();
            label1.Text = _Title;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;

            try {

                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 10851;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                String _readPtxValues = "SELECT CUST_FILE FROM file_info_ptx WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

                command = new MySqlCommand(_readPtxValues, con);
                command = con.CreateCommand();
                command.CommandText = _readPtxValues;
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filetitle", label1.Text);
                
                MySqlDataReader _ptxReader = command.ExecuteReader();
                if (_ptxReader.Read()) {
                    var get_ptxValues = (byte[])_ptxReader["CUST_FILE"];
                    //var _bmp = new Bitmap(new MemoryStream(get_ptxValues));
                    /*using(var _Memorystream = new MemoryStream(get_ptxValues)) {
                        //Bitmap _Bitmap = new Bitmap(_Memorystream); 
                        Image _Image = Image.FromStream(_Memorystream);
                    }*/
                }
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {

                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 10851;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                String _readPtxValues = "SELECT CUST_FILE FROM file_info_ptx WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filetitle";

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
                    if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_OpenDialog.FileName, get_apkValues);
                    }
                }

                //var get_apkValues = apkValues[0];
                //MemoryStream _msApk = new MemoryStream(apkValues[0]);
                //Byte[] _msByteAr = _msApk.ToArray();
                //_msApk.Close();

            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
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
    }
}
