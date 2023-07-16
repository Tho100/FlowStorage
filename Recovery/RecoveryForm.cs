using FlowSERVER1.AlertForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private void SaveRecoveryTokenToLocal(string contentValue) {
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.FileName = "FlowstorageRECOVERYKEY.txt";
            if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllText(_OpenDialog.FileName, contentValue);
            }
           
        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            try {

                string authcase0 = EncryptionModel.computeAuthCase(txtFieldAuth.Text);
                string authcase1 = EncryptionModel.computeAuthCase(txtFieldPIN.Text);

                if(authcase0 == RetrieveCases().ElementAt(0) && authcase1 == RetrieveCases().ElementAt(1)) {
                    SaveRecoveryTokenToLocal(RetrieveRecoveryToken());
                } else {
                    new CustomAlert(title: "Export failed", subheader: "Password or PIN is incorrect.").Show();
                }

            } catch (Exception) {
                new CustomAlert(title: "Export failed", subheader: "An unknown error occurred, are you connected to the internet?").Show();
            }
        }

        private string RetrieveRecoveryToken() {

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

        private List<string> RetrieveCases() {

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
            if (System.Text.RegularExpressions.Regex.IsMatch(txtFieldPIN.Text, "[^0-9]")) {
                txtFieldPIN.Text = txtFieldPIN.Text.Remove(txtFieldPIN.Text.Length - 1);
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
