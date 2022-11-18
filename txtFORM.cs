using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;
using System.IO;

namespace FlowSERVER1 {
    public partial class txtFORM : Form {
        public static txtFORM instance;
        public txtFORM(String getText,String fileName) {
            InitializeComponent();
            instance = this;
            label1.Text = fileName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            if(getText == "") {
                MessageBox.Show("TESTING");
                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

                MySqlConnection con = new MySqlConnection(constring);

                MySqlCommand command;

                con.Open();
                String retrieveImg = "SELECT CONVERT(CUST_FILE USING utf8) FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND FOLDER_TITLE = @foldername AND FILE_TYPE = @filetype";
                command = new MySqlCommand(retrieveImg, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                command.Parameters.AddWithValue("@foldername", Form1.instance.listBox1.GetItemText(Form1.instance.listBox1.SelectedItem));
                command.Parameters.AddWithValue("@filetype", ".txt");

                List<String> textValues_ = new List<String>();

                MySqlDataReader _ReadTexts = command.ExecuteReader();
                while (_ReadTexts.Read()) {
                    textValues_.Add(_ReadTexts.GetString(0));
                }
                var getMainText = textValues_[0];
                guna2textbox1.Text = getMainText;
            } else {
                MessageBox.Show("TESTING1");
                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                string countRow = "SELECT COUNT(CUST_FILE) FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                command = new MySqlCommand(countRow, con);
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);

                var rowTotal = command.ExecuteScalar();
                var intTotalRow = Convert.ToInt32(rowTotal);

                string getTxtQue = "SELECT CUST_FILE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(getTxtQue,con);
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password",Form1.instance.label3.Text);
                command.Parameters.AddWithValue("@filename",label1.Text);

                List<string> textValuesF = new List<string>();

                MySqlDataReader txtReader = command.ExecuteReader();
                while(txtReader.Read()) {
                    textValuesF.Add(txtReader.GetString(0));
                }
                txtReader.Close();

                guna2textbox1.Text = textValuesF[0];
            }

            //guna2TextBox1.Text = getText;

        }

        private void txtFORM_Load(object sender, EventArgs e) {

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
        }

        private void haha_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.Filter = "Text Files|*.txt";
            try {
                if(_OpenDialog.ShowDialog() == DialogResult.OK) {
                    File.WriteAllText(_OpenDialog.FileName,guna2textbox1.Text);
                }
            } catch (Exception eq) {
                MessageBox.Show("An error occurred while attempting to save file.","Flow Storage",
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
