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
                    greeting = "Good Morning " + form.label5.Text + " :) " + getMorningKeys;
                    form.pictureBox2.Visible = true;
                    form.pictureBox1.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                else if (hours >= 12 && hours <= 16) {
                    greeting = "Good Afternoon " + form.label5.Text + " :)";
                    form.pictureBox2.Visible = true;
                    form.pictureBox1.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                else if (hours >= 16 && hours <= 21) {
                    greeting = "Good Evening " + form.label5.Text + " :)";
                    form.pictureBox3.Visible = true;
                    form.pictureBox2.Visible = false;
                    form.pictureBox1.Visible = false;
                }
                else if (hours >= 21 && hours <= 24) {
                    greeting = "Good Night " + form.label5.Text + " :)";
                    form.pictureBox1.Visible = true;
                    form.pictureBox2.Visible = false;
                    form.pictureBox3.Visible = false;
                }
                form.label1.Text = greeting;
            }
            catch (Exception) {
                MessageBox.Show("Oh no! unable to retrieve the time :(( sooo sadd :CCCC");
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e) {

            string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
            string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
            string username = "root"; // epiz_33067528 | root
            string password = "nfreal-yt10";
            int mainPort_ = 14024;
            string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            String _getEmail = guna2TextBox1.Text;
            String _getPass = guna2TextBox2.Text;
            String _getUser = guna2TextBox3.Text;
            Control flowlayout = Form1.instance.flowLayoutPanel1;

            con.Open();
            
            String verifyEmailQue = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
            command = con.CreateCommand();
            command.CommandText = verifyEmailQue;
            command.Parameters.AddWithValue("@email", _getEmail);

            List<String> emailExists = new List<String>();
            List<String> usernameExists = new List<String>();

            MySqlDataReader emailReader = command.ExecuteReader();
            while (emailReader.Read()) {
                emailExists.Add(emailReader.GetString(0));
            }
            emailReader.Close();

            String verifyUsernameQue = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            command = con.CreateCommand();
            command.CommandText = verifyUsernameQue;
            command.Parameters.AddWithValue("@username", _getUser);

            MySqlDataReader usernameReader = command.ExecuteReader();
            while (usernameReader.Read()) {
                usernameExists.Add(usernameReader.GetString(0));
            }
            usernameReader.Close();

            if (emailExists.Count() >= 1 || usernameExists.Count() >= 1) {
                if(emailExists.Count() >= 1) {
                    label5.Visible = true;
                    label5.Text = "Email already exists.";
                } 
                if(usernameExists.Count() >= 1) {
                    label7.Visible = true;
                    label7.Text = "Username is taken.";
                }
            }
            else {
                label5.Visible = false;
                label4.Visible = false;
                label7.Visible = false;
                if (!String.IsNullOrEmpty(_getUser)) {
                    if (!String.IsNullOrEmpty(_getPass)) {
                        if (!String.IsNullOrEmpty(_getEmail)) {
                            flowlayout.Controls.Clear();
                            if (flowlayout.Controls.Count == 0) {
                                Form1.instance.label8.Visible = true;
                                Form1.instance.guna2Button6.Visible = true;
                            }
                            if (Form1.instance.setupLabel.Text.Length > 14) {
                                var label = Form1.instance.setupLabel;
                                label.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                                label.Location = new Point(3, 27);
                            }

                            var encryptionPass = EncryptionModel.Encrypt(_getPass, "ABHABH24");
                            Form1.instance.setupLabel.Text = _getUser;
                            Form1.instance.label3.Text = encryptionPass;

                            String getDate = DateTime.Now.ToString("MM/dd/yyyy");

                            string query = "INSERT INTO information(CUST_USERNAME,CUST_PASSWORD,CREATED_DATE,CUST_EMAIL) VALUES(@CUST_USERNAME,@CUST_PASSWORD,@CREATED_DATE,@CUST_EMAIL)";

                            using (var cmd = new MySqlCommand(query, con)) {
                                cmd.Parameters.AddWithValue("@CUST_USERNAME", _getUser);
                                cmd.Parameters.AddWithValue("@CUST_PASSWORD", encryptionPass);
                                cmd.Parameters.AddWithValue("@CREATED_DATE", getDate);
                                cmd.Parameters.AddWithValue("@CUST_EMAIL", _getEmail);
                                cmd.ExecuteNonQuery();
                            }

                            label5.Visible = false;
                            label4.Visible = false;
                            Form1.instance.label5.Text = _getUser;
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
                else {
                    label7.Visible = true;
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
