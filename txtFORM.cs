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
namespace FlowSERVER1 {
    public partial class txtFORM : Form {
        public static txtFORM instance;
        public txtFORM(String getText,String fileName) {
            InitializeComponent();
            instance = this;
            //guna2TextBox1.Text = getText;
            label1.Text = fileName;
            label2.Text = Form1.instance.label5.Text;

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

        }
    }
}
