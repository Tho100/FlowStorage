using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class BackupRecoveryKeyForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public BackupRecoveryKeyForm() {
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

            var saveDialog  = new SaveFileDialog();
            saveDialog.FileName = "FlowstorageRECOVERYKEY.txt";

            if (saveDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllText(saveDialog.FileName, contentValue);

            }
           
        }

        private async void guna2Button1_Click(object sender, EventArgs e) {

            try {

                string passwordHashedInput = EncryptionModel.computeAuthCase(txtFieldAuth.Text);
                string pinHashedInput = EncryptionModel.computeAuthCase(txtFieldPIN.Text);

                var authenticationInfo = await userAuthQuery.GetAccountAuthentication(tempDataUser.Email);

                string password = authenticationInfo["password"];
                string pin = authenticationInfo["pin"];

                if(pinHashedInput == pin && passwordHashedInput == password) {

                    string recoveryHashedToken = await userAuthQuery.GetRecoveryKeyByEmail(tempDataUser.Email);

                    SaveRecoveryTokenToLocal(recoveryHashedToken);

                } else {
                    new CustomAlert(
                        title: "Export failed", subheader: "Password or PIN is incorrect.").Show();

                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Export failed", subheader: "An unknown error occurred, are you connected to the internet?").Show();

            }
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
