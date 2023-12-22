using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class RemoveAccountForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public RemoveAccountForm() {
            InitializeComponent();
        }

        private async Task RemoveUserData(string tableName) {

            if (tableName != GlobalsTable.sharingTable) {
                string query = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    await command.ExecuteNonQueryAsync();
                }

            } else {

                const string query = "DELETE FROM cust_sharing WHERE CUST_FROM = @username";
                using (MySqlCommand command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                var authenticationInfo = await userAuthQuery.GetAccountAuthentication(tempDataUser.Email);

                string password = authenticationInfo["password"];
                string pin = authenticationInfo["pin"];

                if (txtFieldPIN.Text == pin) {
                    if (txtFieldAuth.Text == password) {
                        if (MessageBox.Show("Delete your account?\nYour data will be deleted PERMANENTLY.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                            string fileLoc = path + "\\CUST_DATAS.txt";
                            if (File.Exists(fileLoc)) {
                                Directory.Delete(path, true);

                            }

                            foreach (string tablesName in GlobalsTable.publicTables) {
                                await RemoveUserData(tablesName);
                            }

                            this.Close();

                            Application.OpenForms["remAccFORM"].Close();
                            HomePage.instance.lstFoldersPage.Items.Clear();
                        }

                    } else {
                        lblAlert.Visible = true;
                        lblAlert.Text = "Password is incorrect.";

                    }

                } else {
                    lblAlert.Visible = true;
                    lblAlert.Text = "PIN is incorrect.";

                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to delete account", subheader: "Are you connected to the internet?");

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
