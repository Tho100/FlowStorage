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
    public partial class PassSharingFORM : Form {

        private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;

        public PassSharingFORM() {
            InitializeComponent();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            guna2TextBox2.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            guna2TextBox2.PasswordChar = '\0';
        }

        /// <summary>
        /// This button will add/update password
        /// for user file sharing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            if(guna2TextBox2.Text != String.Empty) {
                if(MessageBox.Show("Confirm password for File Sharing?.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {

                    remAccFORM.instance.guna2Button23.Visible = false;
                    remAccFORM.instance.guna2Button23.Enabled = false;

                    remAccFORM.instance.guna2Button27.Visible = true;
                    remAccFORM.instance.guna2Button27.Enabled = true;

                    String _customizeQuery = "UPDATE sharing_info SET SET_PASS = @getval WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(_customizeQuery,con);
                    command.Parameters.AddWithValue("@getval",EncryptionModel.computeAuthCase(guna2TextBox2.Text));
                    command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show("You've successfully added a password for File Sharing.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            } else {
                label4.Visible = true;
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
