using FlowSERVER1.Global;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class RemoveAccountForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public RemoveAccountForm() {
            InitializeComponent();
        }

        private async Task RemoveUserData(String tableName) {

            if (tableName != "cust_sharing") {
                string query = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    await command.ExecuteNonQueryAsync();
                }
            }
            else {

                const string query = "DELETE FROM cust_sharing WHERE CUST_FROM = @username";
                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        private string ReturnCustomColumn(String columnName) {

            List<string> _concludeValue = new List<string>();

            string checkPassword_Query = $"SELECT {columnName} FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(checkPassword_Query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader readerPass_ = command.ExecuteReader()) {
                    while (readerPass_.Read()) {
                        _concludeValue.Add(readerPass_.GetString(0));
                    }
                }
            }

            return _concludeValue[0];

        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                var decryptPass = EncryptionModel.Decrypt(ReturnCustomColumn("CUST_PASSWORD"));
                var decryptPin = EncryptionModel.Decrypt(ReturnCustomColumn("CUST_PIN"));

                if (txtFieldPIN.Text == decryptPin) {
                    if (txtFieldAuth.Text == decryptPass) {
                        if (MessageBox.Show("Delete your account?\nYour data will be deleted PERMANENTLY.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                            String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                            String _getAuth = _getPath + "\\CUST_DATAS.txt";
                            if (File.Exists(_getAuth)) {
                                Directory.Delete(_getPath, true);
                            }

                            foreach (string tablesName in GlobalsTable.publicTables) {
                                await RemoveUserData(tablesName);
                            }

                            this.Close();

                            Application.OpenForms["remAccFORM"].Close();
                            HomePage.instance.lstFoldersPage.Items.Clear();
                        }
                    }
                    else {
                        lblAlert.Visible = true;
                        lblAlert.Text = "Password is incorrect.";
                    }
                }
                else {
                    lblAlert.Visible = true;
                    lblAlert.Text = "PIN is incorrect.";
                }
            }
            catch (Exception) {
                MessageBox.Show("Failed to delete account.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtFieldPIN.Text, "[^0-9]")) {
                txtFieldPIN.Text = txtFieldPIN.Text.Remove(txtFieldPIN.Text.Length - 1);
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void ConfirmRemFORM_Load(object sender, EventArgs e) {

        }
    }
}
