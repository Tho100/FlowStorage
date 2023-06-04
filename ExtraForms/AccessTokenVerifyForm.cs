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
    public partial class AccessTokenVerifyForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public AccessTokenVerifyForm() {
            InitializeComponent();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// This button will check for verification and 
        /// determine if the user entered password is correct or not,
        /// if correct then view access token.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            string _queStr = "";

            string _selectQue = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(_selectQue, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);

                using (MySqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        _queStr = reader.GetString(0);
                    }
                }
            }

            if (EncryptionModel.Decrypt(_queStr) == guna2TextBox1.Text) {
                SettingsForm.instance.guna2Button28.Visible = false;
                SettingsForm.instance.guna2Button29.Visible = true;
                SettingsForm.instance.guna2TextBox2.PasswordChar = '\0';
                SettingsForm.instance.guna2TextBox2.Enabled = true;
                SettingsForm.instance.tokenCheckCurr++;
                this.Close();
            }
            else {
                label1.Visible = true;
            }

        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}