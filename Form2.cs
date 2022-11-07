using System;
using System.IO;
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
using MySql;
using Guna.UI2.WinForms;
using System.Diagnostics;
using System.Globalization;

namespace FlowSERVER1
{
    public partial class Form2 : Form
    {
        public static Form2 instance;
        public Form2()
        {
            InitializeComponent();
            this.Text = "Login Page";
            this.Icon = new Icon(@"C:\Users\USER\Documents\FlowStorage4.ico");
            instance = this;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }

        private void textBox3_TextChanged(object sender, EventArgs e) {

        }
        int one = 0;
        int two = 0;
        public void setupTime() {
            var form = Form1.instance;
            try {
                string[] morningKeys = { "start your day with a coffee?", "" };
                var random = new Random();
                var getKeyRand = random.Next(0, 1);
                var getMorningKeys = morningKeys[getKeyRand];
                DateTime now = DateTime.Now;
                var hours = now.Hour;
                String greeting = null;
                if (hours >= 1 && hours <= 12) {
                    greeting = "Good Morning " + label5.Text + " :) " + getMorningKeys;
                    form.pictureBox2.Visible = true;
                    form.pictureBox1.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                else if (hours >= 12 && hours <= 16) {
                    greeting = "Good Afternoon " + label5.Text + " :)";
                    form.pictureBox2.Visible = true;
                    form.pictureBox1.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                else if (hours >= 16 && hours <= 21) {
                    greeting = "Good Evening " + label5.Text + " :)";
                    form.pictureBox3.Visible = true;
                    form.pictureBox2.Visible = false;
                    form.pictureBox1.Visible = false;
                }
                else if (hours >= 21 && hours <= 24) {
                    greeting = "Good Night " + label5.Text + " :)";
                    form.pictureBox1.Visible = true;
                    form.pictureBox2.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                label1.Text = greeting;
            }
            catch (Exception) {
                MessageBox.Show("Oh no! unable to retrieve the time :(( sooo sadd :CCCC");
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            string server = "localhost";
            string db = "flowserver_db";
            string username = "root";
            string password = "nfreal-yt10";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            MySqlConnection con = new MySqlConnection(constring);

            MySqlCommand command;

            string get_user = guna2TextBox1.Text;
            string get_pass = guna2TextBox2.Text;
            var flowlayout = Form1.instance.flowLayoutPanel1;

            con.Open();
            //String verifyQue = "SELECT * FROM information WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
            String verifyQue = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            command = con.CreateCommand();
            command.CommandText = verifyQue;
            command.Parameters.AddWithValue("@username", get_user);
            //            command.Parameters.AddWithValue("@password", get_pass);

            List<string> userExists = new List<string>();

            MySqlDataReader userReader = command.ExecuteReader();
            while (userReader.Read()) {
                userExists.Add(userReader.GetString(0));
            }

            userReader.Close();
            con.Close();

            if (userExists.Count() >= 1) {
                label4.Visible = true;
                label4.Text = "Username is taken.";
            }
            else {
                label4.Visible = false;
                if (!String.IsNullOrEmpty(get_pass)) {
                    if (!String.IsNullOrEmpty(get_user)) {
                        flowlayout.Controls.Clear();
                        if (flowlayout.Controls.Count == 0) {
                            Form1.instance.label8.Visible = true;
                            Form1.instance.guna2Button6.Visible = true;
                        }
                        Form1.instance.setupLabel.Text = get_user;
                        Form1.instance.label3.Text = get_pass;
                        if (Form1.instance.setupLabel.Text.Length > 14) {
                            var label = Form1.instance.setupLabel;
                            label.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                            label.Location = new Point(3, 27);
                        }

                        con.Open();
                        string query = "INSERT INTO information(CUST_USERNAME,CUST_PASSWORD) VALUES(@CUST_USERNAME,@CUST_PASSWORD)";

                        using (var cmd = new MySqlCommand(query, con)) {
                            cmd.Parameters.AddWithValue("@CUST_USERNAME", get_user);
                            cmd.Parameters.AddWithValue("@CUST_PASSWORD", get_pass);
                            cmd.ExecuteNonQuery();
                        }

                        label5.Visible = false;
                        label4.Visible = false;
                        Form1.instance.label5.Text = get_user;
                        Form1.instance.listBox1.Items.Clear();
                        Form1.instance.listBox1.Items.Add("Home");
                        Form1.instance.listBox1.SelectedIndex = 0;
                        setupTime();

                        this.Close();
                    }
                    else {
                        label5.Visible = true;
                    }
                }
                else {
                    label4.Visible = true;
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
            LogIN SignForm = new LogIN();
            SignForm.Show();
        }

        private void Form2_Load(object sender, EventArgs e) {
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button4.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            guna2Button3.Visible = true;
            guna2Button4.Visible = false;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
