using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class AddAuthSharing : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public AddAuthSharing() {
            InitializeComponent();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            txtFieldAuth.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            txtFieldAuth.PasswordChar = '\0';
        }

        /// <summary>
        /// This button will add/update password
        /// for user file sharing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            if (txtFieldAuth.Text != String.Empty) {
                if (MessageBox.Show("Confirm password for File Sharing?.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                    SettingsForm.instance.guna2Button23.Visible = false;
                    SettingsForm.instance.guna2Button23.Enabled = false;

                    SettingsForm.instance.btnRmvSharingAuth.Visible = true;
                    SettingsForm.instance.btnRmvSharingAuth.Enabled = true;

                    const string addSharingAuthQuery = "UPDATE sharing_info SET SET_PASS = @getval WHERE CUST_USERNAME = @username";
                    using (MySqlCommand command = new MySqlCommand(addSharingAuthQuery, con)) {
                        command.Parameters.AddWithValue("@getval", EncryptionModel.computeAuthCase(txtFieldAuth.Text));
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("You've successfully added a password for File Sharing.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else {
                lblAlert.Visible = true;
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
