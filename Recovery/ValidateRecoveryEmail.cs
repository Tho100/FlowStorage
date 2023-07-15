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

        string _valueToReturn = "";
        private string returnValues(String _WhichColumn) {

            List<String> _concludeValue = new List<String>();

            string checkPassword_Query = $"SELECT {_WhichColumn} FROM information WHERE CUST_EMAIL = @email";

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
                _valueToReturn = _concludeValue[0];
            }

            return _valueToReturn;
        }

        string _recovTok = "";
        private string returnValuesRecov() {

            List<String> _concludeValue = new List<String>();

            const string checkPassword_Query = "SELECT RECOV_TOK FROM information WHERE CUST_EMAIL = @email";

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
                _recovTok = EncryptionModel.Decrypt(_concludeValue[0]);
            }

            return _recovTok;
        }

        private string emailExistsCheck() {

            List<String> _concludeValue = new List<String>();

            string _custEmail = "";
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

                if(emailExistsCheck() == "") {
                    lblAlert.Text = "Account not found.";
                    lblAlert.Visible = true;
                    return;
                }

                if(returnValues("CUST_PIN") == "") {
                    lblAlert.Text = "Account not found.";
                    lblAlert.Visible = true;
                    return;
                }

                if(EncryptionModel.computeAuthCase(txtFieldPin.Text) != returnValues("CUST_PIN")) {
                    lblAlert.Text = "PIN key is incorrect.";
                    lblAlert.Visible = true;
                    return;
                }

                if(txtFieldRecoveryKey.Text != returnValuesRecov()) {
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
