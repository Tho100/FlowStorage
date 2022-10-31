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
namespace FlowSERVER1 {
    public partial class ConfirmRemFORM : Form {
        public ConfirmRemFORM() {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            try {
                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command,command_Read;
                con.Open();

                List<string> passValues_ = new List<string>();

                String checkPassword_Query = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
                command_Read = new MySqlCommand(checkPassword_Query,con);
                command_Read = con.CreateCommand();
                command_Read.CommandText = checkPassword_Query;
                command_Read.Parameters.AddWithValue("@username",remAccFORM.instance.label1.Text);
                MySqlDataReader readerPass_ = command_Read.ExecuteReader();

                while(readerPass_.Read()) {
                    passValues_.Add(readerPass_.GetString(0));
                }
                readerPass_.Close();

                if(guna2TextBox1.Text == passValues_[0]) {
                    String remInfo_Query = "DELETE FROM information WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remInfo_Query, con);
                    command.Parameters.AddWithValue("@username",remAccFORM.instance.label1.Text);
                    command.Parameters.AddWithValue("@password",guna2TextBox1.Text);
                    command.ExecuteNonQuery();

                    String remImg_Query = "DELETE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remImg_Query, con);
                    command.Parameters.AddWithValue("@username", remAccFORM.instance.label1.Text);
                    command.Parameters.AddWithValue("@password", guna2TextBox1.Text);
                    command.ExecuteNonQuery();

                    String remTxt_Query = "DELETE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remTxt_Query, con);
                    command.Parameters.AddWithValue("@username", remAccFORM.instance.label1.Text);
                    command.Parameters.AddWithValue("@password", guna2TextBox1.Text);
                    command.ExecuteNonQuery();

                    this.Close();
                    Application.OpenForms["remAccFORM"].Close();
                    Form1.instance.guna2Panel7.Visible = true;
                } else {
                    MessageBox.Show("WRONG PASSWORD");
                }
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
