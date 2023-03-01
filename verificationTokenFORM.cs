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
    public partial class verificationTokenFORM : Form {
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        public verificationTokenFORM() {
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
            String _queStr = "";
            String _selectQue = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(_selectQue,con);
            command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);

            MySqlDataReader _readQue = command.ExecuteReader();
            while(_readQue.Read()) {
                _queStr = _readQue.GetString(0);
            }
            _readQue.Close();

            if(EncryptionModel.Decrypt(_queStr, "0123456789085746") == guna2TextBox1.Text) {
                remAccFORM.instance.guna2Button28.Visible = false;
                remAccFORM.instance.guna2Button29.Visible = true;
                remAccFORM.instance.guna2TextBox2.PasswordChar = '\0';
                remAccFORM.instance.guna2TextBox2.Enabled = true;
                remAccFORM.instance.tokenCheckCurr++;
                this.Close();
            } else {
                label1.Visible = true;
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}
