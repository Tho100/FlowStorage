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
    public partial class emailValidate : Form {
        private static MySqlCommand command = ConnectionModel.command;
        private static MySqlConnection con = ConnectionModel.con;
        public emailValidate() {
            InitializeComponent();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        String _valueToReturn = "";
        String returnValues(String _WhichColumn) {

            List<String> _concludeValue = new List<String>();

            String checkPassword_Query = "SELECT " + _WhichColumn + " FROM information WHERE CUST_EMAIL = @email";
            command = new MySqlCommand(checkPassword_Query, con);
            command = ConnectionModel.con.CreateCommand();
            command.CommandText = checkPassword_Query;
            command.Parameters.AddWithValue("@email", guna2TextBox1.Text);
            MySqlDataReader readerPass_ = command.ExecuteReader();

            while (readerPass_.Read()) {
                _concludeValue.Add(readerPass_.GetString(0));
            }
            readerPass_.Close();
            if (_concludeValue[0] != "") {
                _valueToReturn = _concludeValue[0];
            }
            return _valueToReturn;
        }

        private String emailExistsCheck() {
            String _custEmail = "";
            List<String> _concludeValue = new List<String>();

            String checkPassword_Query = "SELECT CUST_EMAIL FROM information WHERE CUST_EMAIL = @email";
            command = new MySqlCommand(checkPassword_Query, con);
            command = ConnectionModel.con.CreateCommand();
            command.CommandText = checkPassword_Query;
            command.Parameters.AddWithValue("@email", guna2TextBox1.Text);
            MySqlDataReader readerPass_ = command.ExecuteReader();

            while (readerPass_.Read()) {
                _concludeValue.Add(readerPass_.GetString(0));
            }
            readerPass_.Close();
            if (_concludeValue[0] != "") {
                _custEmail = _concludeValue[0];
            }
            return _custEmail;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            try {
                if(emailExistsCheck() != "") {
                    if (EncryptionModel.Decrypt(returnValues("CUST_PIN"), "0123456789085746") != "") {
                        var pinDecryptionKey = EncryptionModel.Decrypt(returnValues("CUST_PIN"), "0123456789085746");
                        if(guna2TextBox4.Text == pinDecryptionKey) {
                                
                             confirmPas _showPasswordRecovery = new confirmPas(_custEmail: guna2TextBox1.Text);
                            _showPasswordRecovery.Show();
                            this.Close();

                        } else {
                            label4.Visible = true;
                        }
                    } else {
                        label4.Visible = true;
                    }
                } else {
                    label4.Visible = true;
                }
            }
            catch (Exception) {
                label4.Visible = true;
            }
        }
    }
}
