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

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

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

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private async Task GetAccessToken() {

            const string query = "SELECT ACCESS_TOK FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader read =  (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    if (await read.ReadAsync()) {
                        txtFieldToken.Text = read.GetString(0);
                    }
                    read.Close();
                }
            }
        }



        private async void guna2Button3_Click(object sender, EventArgs e) {

            try {

                string userInputPin = EncryptionModel.computeAuthCase(txtFieldPin.Text);

                if(userInputPin == await crud.ReturnUserPIN()) {
                    await GetAccessToken();
                } else {
                    MessageBox.Show("PIN is incorrect.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            } catch (Exception) {
                MessageBox.Show("Please try again later.", "An error occurred",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            Clipboard.SetText(txtFieldToken.Text);
            label13.Visible = true;
        }
    }
}
