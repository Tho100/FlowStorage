﻿using System;
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

            string checkPassword_Query = "SELECT " + _WhichColumn + " FROM information WHERE CUST_EMAIL = @email";

            using (MySqlCommand command = new MySqlCommand(checkPassword_Query, con)) {
                command.CommandText = checkPassword_Query;
                command.Parameters.AddWithValue("@email", guna2TextBox1.Text);

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
                command.Parameters.AddWithValue("@email", guna2TextBox1.Text);

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
                command.Parameters.AddWithValue("@email", guna2TextBox1.Text);

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
                    label4.Text = "Account not found.";
                    label4.Visible = true;
                    return;
                }

                if(returnValues("CUST_PIN") == "") {
                    label4.Text = "Account not found.";
                    label4.Visible = true;
                    return;
                }

                if(EncryptionModel.computeAuthCase(guna2TextBox4.Text) != returnValues("CUST_PIN")) {
                    label4.Text = "PIN key is incorrect.";
                    label4.Visible = true;
                    return;
                }

                if(guna2TextBox2.Text != returnValuesRecov()) {
                    label4.Visible = true;
                    label4.Text = "Invalid recovery key.";
                    return;
                }

                ResetAuthForm _showPasswordRecovery = new ResetAuthForm(Globals.custUsername);
                _showPasswordRecovery.Show();

                this.Close();


            } catch (Exception) {
                label4.Visible = true;
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox4.Text, "[^0-9]")) {
                guna2TextBox4.Text = guna2TextBox4.Text.Remove(guna2TextBox4.Text.Length - 1);
            }
        }
    }
}