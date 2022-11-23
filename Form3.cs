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
using MySql;
using MySql.Data.MySqlClient;
using System.IO;

namespace FlowSERVER1
{

    public partial class Form3 : Form
    {
        public Form3(String sendTitle_)
        {
            InitializeComponent();
            this.Icon = new Icon(@"C:\Users\USER\Documents\FlowStorage4.ico");
            label1.Text = sendTitle_;
        }

        public void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e) {
            //Form4 get_dir_title = new Form4();
            //string dir_title = get_dir_title.guna2TextBox1.Text;
            //label3.Text = dir_title;
            //this.Text = "Directory: " + label3.Text;

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            string server = "localhost";
            string db = "flowserver_db";
            string username = "root";
            string password = "nfreal-yt10";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK) {
                guna2PictureBox1.Image = new Bitmap(open.FileName);
                guna2TextBox1.Text = open.FileName;

                guna2TextBox1.Visible = true;
                guna2PictureBox1.Visible = true;

                MemoryStream ms = new MemoryStream();
                guna2PictureBox1.Image.Save(ms,guna2PictureBox1.Image.RawFormat);
                byte[] img = ms.ToArray(); 

                String query = "INSERT INTO file_info(CUST_FILE_PATH,CUST_FILE) VALUES (@CUST_FILE_PATH,@CUST_FILE)";

                con.Open();

                command = new MySqlCommand(query,con);

                command.Parameters.Add("@CUST_FILE_PATH",MySqlDbType.VarChar,255);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.Blob);

                command.Parameters["@CUST_FILE_PATH"].Value = open.FileName;
                command.Parameters["@CUST_FILE"].Value = img;

                if(command.ExecuteNonQuery() == 1) {
                    MessageBox.Show("INSERTED");
                } else {
                    MessageBox.Show("FAILED");
                }

            }
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }

        private void panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {

        }
    }
}