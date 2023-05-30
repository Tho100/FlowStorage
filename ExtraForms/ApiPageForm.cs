using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class ApiPageForm : Form {

        readonly private MySqlCommand command = ConnectionModel.command;
        readonly private MySqlConnection con = ConnectionModel.con;

        public ApiPageForm() {
            InitializeComponent();
        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e) {

        }

        private void apiFORM_Load(object sender, EventArgs e) {

        }

        private void pictureBox2_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private async Task getAccessToken() {

            const string query = "SELECT ACCESS_TOK FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader read =  (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await read.ReadAsync()) {
                        guna2TextBox2.Text = read.GetString(0);
                    }
                    read.Close();
                }
            }
        }

        private async void guna2Button3_Click(object sender, EventArgs e) {

            string query = "SELECT CUST_PIN FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader read = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await read.ReadAsync()) {
                        if(EncryptionModel.computeAuthCase(guna2TextBox1.Text) == read.GetString(0)) {
                            read.Close();
                            await getAccessToken(); 
                        } else {
                            MessageBox.Show("PIN is incorrect.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            Clipboard.SetText(guna2TextBox2.Text);
            label13.Visible = true;
        }
    }
}
