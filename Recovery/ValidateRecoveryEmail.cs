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
    public partial class ValidateRecoveryEmail : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public ValidateRecoveryEmail() {
            InitializeComponent();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private string SelectValueCustomColumn(String columnName) {

            string returnedValue = null;

            List<string> listReturnedValues = new List<string>();

            string query = $"SELECT {columnName} FROM information WHERE CUST_EMAIL = @email";

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@email", txtFieldEmail.Text);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        listReturnedValues.Add(reader.GetString(0));
                    }
                }
            }

            if (listReturnedValues.Count > 0 && listReturnedValues[0] != "") {
                returnedValue = listReturnedValues[0];
            }

            return returnedValue;
        }

        private string ReturnRecoveryValue() {

            string recoveryTokenValue = null;

            List<string> listRecoveryToken = new List<string>();

            const string checkPassword_Query = "SELECT RECOV_TOK FROM information WHERE CUST_EMAIL = @email";

            using (MySqlCommand command = new MySqlCommand(checkPassword_Query, con)) {
                command.CommandText = checkPassword_Query;
                command.Parameters.AddWithValue("@email", txtFieldEmail.Text);

                using (MySqlDataReader readerPass_ = command.ExecuteReader()) {
                    while (readerPass_.Read()) {
                        listRecoveryToken.Add(readerPass_.GetString(0));
                    }
                }
            }

            if (listRecoveryToken.Count > 0 && listRecoveryToken[0] != "") {
                recoveryTokenValue = EncryptionModel.Decrypt(listRecoveryToken[0]);
            }

            return recoveryTokenValue;
        }

        private string EmailIsExistVerification() {

            List<String> _concludeValue = new List<String>();

            string _custEmail = null;
            const string checkPassword_Query = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";

            using (MySqlCommand command = new MySqlCommand(checkPassword_Query, con)) {
                command.CommandText = checkPassword_Query;
                command.Parameters.AddWithValue("@email", txtFieldEmail.Text);

                using (MySqlDataReader readerPass_ = command.ExecuteReader()) {
                    while (readerPass_.Read()) {
                        _concludeValue.Add(readerPass_.GetString(0));
                    }
                }
            }

            if (_concludeValue.Count > 0 && _concludeValue[0] != "") {
                _custEmail = _concludeValue[0];
            }

            return _custEmail;
        }


        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                if(EmailIsExistVerification() == null) {
                    lblAlert.Text = "Account not found.";
                    lblAlert.Visible = true;
                    return;
                }

                if(SelectValueCustomColumn("CUST_PIN") == null) {
                    lblAlert.Text = "Account not found.";
                    lblAlert.Visible = true;
                    return;
                }

                if(EncryptionModel.computeAuthCase(txtFieldPin.Text) != SelectValueCustomColumn("CUST_PIN")) {
                    lblAlert.Text = "PIN key is incorrect.";
                    lblAlert.Visible = true;
                    return;
                }

                if(txtFieldRecoveryKey.Text != ReturnRecoveryValue()) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Invalid recovery key.";
                    return;
                }

                ResetAuthForm _showPasswordRecovery = new ResetAuthForm();
                _showPasswordRecovery.Show();

                this.Close();


            } catch (Exception) {
                lblAlert.Visible = true;
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtFieldPin.Text, "[^0-9]")) {
                txtFieldPin.Text = txtFieldPin.Text.Remove(txtFieldPin.Text.Length - 1);
            }
        }
    }
}
