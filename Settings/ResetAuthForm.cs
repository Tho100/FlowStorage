using FlowSERVER1.AlertForms;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class ResetAuthForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

        public ResetAuthForm() {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void SetupInformationUpdate(String newAuth) {

            const string updatePasswordQuery = "UPDATE information SET CUST_PASSWORD = @newpass WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(updatePasswordQuery, con)) {
                command.Parameters.AddWithValue("@newpass", EncryptionModel.computeAuthCase(newAuth));
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.ExecuteNonQuery();
            }

        }

        private async void guna2Button1_Click(object sender, EventArgs e) {

            try {

                var _getNewPass = guna2TextBox1.Text;
                var _getVerify = guna2TextBox3.Text;
                var _getOldPass = guna2TextBox2.Text;

                if (_getNewPass != _getVerify) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "New password does not match.";
                    return;
                }

                if (_getNewPass == String.Empty) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Please add a new password.";
                    return;
                }

                if (_getVerify == String.Empty) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "New password does not match.";
                    return;
                }

                if (await crud.ReturnUserAuth() != EncryptionModel.computeAuthCase(_getOldPass)) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Password is incorrect.";
                    return;
                }

                if (MessageBox.Show("Do you want to proceed your action?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {

                    SetupInformationUpdate(_getNewPass);

                    Form bgBlur = new Form();
                    using (SuccessResetAuthAlert displayDirectory = new SuccessResetAuthAlert()) {
                        bgBlur.StartPosition = FormStartPosition.Manual;
                        bgBlur.FormBorderStyle = FormBorderStyle.None;
                        bgBlur.Opacity = .24d;
                        bgBlur.BackColor = Color.Black;
                        bgBlur.WindowState = FormWindowState.Maximized;
                        bgBlur.TopMost = true;
                        bgBlur.Location = this.Location;
                        bgBlur.StartPosition = FormStartPosition.Manual;
                        bgBlur.ShowInTaskbar = false;
                        bgBlur.Show();

                        displayDirectory.Owner = bgBlur;
                        displayDirectory.ShowDialog();

                        bgBlur.Dispose();
                    }
                }

            }
            catch (Exception) {
                new CustomAlert(title: "An error occurred", subheader: "Failed to change your password due to unknown error, are you connected to the internet?").Show();
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button4.Visible = true;
            guna2TextBox1.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            guna2Button4.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox1.PasswordChar = '*';
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            guna2Button5.Visible = false;
            guna2Button6.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void label6_Click(object sender, EventArgs e) {

        }
    }
}
