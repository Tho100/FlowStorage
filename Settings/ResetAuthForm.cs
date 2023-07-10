using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowSERVER1.AlertForms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class ResetAuthForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        private string CurrentUsername;

        public ResetAuthForm(String _currentUsername) {
            InitializeComponent();
            this.CurrentUsername = _currentUsername;
        }

        private void label4_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private string authReturnOriginal(String _usernameEntered) {

            List<string> _passValues = new List<string>();

            const string getUsername = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(getUsername, con)) {
                command.Parameters.AddWithValue("@username", _usernameEntered);

                using (MySqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        string getStringPassword = reader.GetString(0);
                        _passValues.Add(getStringPassword);
                    }
                }
            }

            string passValuesString = _passValues[0];
            return passValuesString;

        }

        private void setupInformationUpdate(String _custUsername,String _newPass) {

            const string updatePasswordQuery = "UPDATE information SET CUST_PASSWORD = @newpass WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(updatePasswordQuery, con)) {
                command.Parameters.AddWithValue("@newpass", computeAuthCase(_newPass));
                command.Parameters.AddWithValue("@username", _custUsername);
                command.ExecuteNonQuery();
            }

        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            try {

                var _getNewPass = guna2TextBox1.Text;
                var _getVerify = guna2TextBox3.Text;
                var _getOldPass = guna2TextBox2.Text;

                if(_getNewPass != _getVerify) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "New password does not match.";
                    return;
                }

                if(_getNewPass == String.Empty) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Please add a new password.";
                    return;
                }

                if(_getVerify == String.Empty) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "New password does not match.";
                    return;
                }

                if(authReturnOriginal(CurrentUsername) != computeAuthCase(_getOldPass)) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Password is incorrect.";
                    return;
                }

                if (MessageBox.Show("Do you want to proceed your action?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {

                    setupInformationUpdate(CurrentUsername, _getNewPass);

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

        private string computeAuthCase(string inputStr) {

            SHA256 sha256 = SHA256.Create();

            string _getAuthStrCase0 = inputStr;
            byte[] _getAuthBytesCase0 = Encoding.UTF8.GetBytes(_getAuthStrCase0);
            byte[] _authHashCase0 = sha256.ComputeHash(_getAuthBytesCase0);
            string _authStrCase0 = BitConverter.ToString(_authHashCase0).Replace("-", "");

            return _authStrCase0;
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
