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
    public partial class resetPas : Form {
        private static MySqlCommand command = ConnectionModel.command;
        private static MySqlConnection con = ConnectionModel.con;
        private static String CurrentEmail;
        public resetPas(String _currentEmail) {
            InitializeComponent();
            CurrentEmail = _currentEmail;
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            guna2Button5.Visible = false;
            guna2Button6.Visible = true;
            guna2TextBox3.PasswordChar = '\0';
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button5.Visible = true;
            guna2Button6.Visible = false;
            guna2TextBox3.PasswordChar = '*';
        }

        private void setupInformationUpdate(String _custEmail, String _newPass) {

            var _encryptionModel = EncryptionModel.Encrypt(_newPass, "0123456789085746");

            String updatePasswordQuery = "UPDATE information SET CUST_PASSWORD = @newpass WHERE CUST_EMAIL = @email";
            command = new MySqlCommand(updatePasswordQuery, con);
            command.Parameters.AddWithValue("@newpass", _encryptionModel);
            command.Parameters.AddWithValue("@email", _custEmail);
            command.ExecuteNonQuery();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            try {
                var _getNewPass = guna2TextBox1.Text;
                var _getVerify = guna2TextBox3.Text;
                if (_getNewPass == _getVerify) {
                    if (_getNewPass != String.Empty) {
                        if (_getVerify != String.Empty) {
                            if (MessageBox.Show("Do you want to proceed your action?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                                setupInformationUpdate(CurrentEmail, _getNewPass);
                                Form bgBlur = new Form();
                                using (successResPas displayDirectory = new successResPas()) {
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
                        else {
                            label4.Visible = true;
                            label4.Text = "New password does not match.";
                        }
                    }
                    else {
                        label4.Visible = true;
                        label4.Text = "Please add a new password.";
                    }
                }
                else {
                    label4.Visible = true;
                    label4.Text = "New password does not match.";
                }
            }
            catch (Exception) {
                MessageBox.Show("There's a problem while attempting to change your password.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button3_Click_1(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button4.Visible = true;
            guna2TextBox1.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            guna2Button4.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox1.PasswordChar = '*';
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
