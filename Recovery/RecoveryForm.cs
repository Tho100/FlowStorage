using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class RecoveryForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public RecoveryForm() {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void RecovFORM_Load(object sender, EventArgs e) {

        }

        private void saveFiles(string contentValue) {
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.FileName = "FlowstorageRECOVERYKEY.txt";
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllText(_OpenDialog.FileName, contentValue);
            }
           
        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            try {

                string authcase0 = EncryptionModel.computeAuthCase(guna2TextBox1.Text);
                string authcase1 = EncryptionModel.computeAuthCase(guna2TextBox2.Text);

                if(authcase0 == retrieveCase().ElementAt(0) && authcase1 == retrieveCase().ElementAt(1)) {
                    saveFiles(retrieveRecov());
                } else {
                    MessageBox.Show("Password or PIN is incorrect.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

            } catch (Exception) {
                MessageBox.Show("Failed to export recovery key.","Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string retrieveRecov() {

            string concludeStrings = "";
            const string query = "SELECT RECOV_TOK FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader read = command.ExecuteReader()) {
                    while (read.Read()) {
                        concludeStrings = read.GetString(0);
                    }
                }
            }

            return EncryptionModel.Decrypt(concludeStrings);

        }

        private List<string> retrieveCase() {

            List<string> concludeStrings = new List<string>();

            const string query = "SELECT CUST_PASSWORD, CUST_PIN FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(query,con)) {
                command.Parameters.AddWithValue("@username",Globals.custUsername);
                using(MySqlDataReader read = command.ExecuteReader()) {
                    while(read.Read()) { 
                        concludeStrings.Add(read.GetString(0));
                        concludeStrings.Add(read.GetString(1));
                    }
                }
            }
            return concludeStrings;
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox2.Text, "[^0-9]")) {
                guna2TextBox2.Text = guna2TextBox2.Text.Remove(guna2TextBox2.Text.Length - 1);
            }
        }
    }
}
