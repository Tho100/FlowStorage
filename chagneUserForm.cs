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
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class chagneUserForm : Form {

        private String CurrentUsername;
        private String NewUsername;

        private MySqlCommand command = ConnectionModel.command;
        readonly private MySqlConnection con = ConnectionModel.con;
        public chagneUserForm(String _CurrentUsername) {
            InitializeComponent();
            CurrentUsername = _CurrentUsername;
            guna2TextBox3.Text = CurrentUsername;
        }

        private void chagneUserForm_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
        private void setupChangeUsername(String _tableName, String _setUsername) {
    
            String updateQuery = "UPDATE " + _tableName + " SET CUST_USERNAME = '" + _setUsername + "' WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(updateQuery,con);
            command.Parameters.AddWithValue("@username",CurrentUsername);
            command.ExecuteNonQuery();
           
        }

        private void setupChangeUsernameSharing(String _setUsername) {

            String updateQuery = "UPDATE cust_sharing SET CUST_FROM = '" + _setUsername + "' WHERE CUST_FROM = @username";
            command = new MySqlCommand(updateQuery, con);
            command.Parameters.AddWithValue("@username", CurrentUsername);
            command.ExecuteNonQuery();

        }

        private int verifyIfNotExists(String _newUsername) {
            List<String> _usernameValues = new List<string>();
            String getUsername = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(getUsername,con);
            command.Parameters.AddWithValue("@username",_newUsername);

            MySqlDataReader _ReadUsername = command.ExecuteReader();
            if(_ReadUsername.Read()) {
                var _getString = _ReadUsername.GetString(0);
                _usernameValues.Add(_getString);
            }
            _ReadUsername.Close();
            return _usernameValues.Count;
        }
        private string authReturnOriginal(String _usernameEntered) {
            List<String> _passValues = new List<string>();
            String getUsername = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            command = new MySqlCommand(getUsername, con);
            command.Parameters.AddWithValue("@username", _usernameEntered);

            MySqlDataReader _ReadPass = command.ExecuteReader();
            if (_ReadPass.Read()) {
                var _getStringPassword = _ReadPass.GetString(0);
                _passValues.Add(_getStringPassword);
            }
            _ReadPass.Close();
            String _passValuesString = _passValues[0];
            return _passValuesString;
        }
        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private string computeAuthCase(string inputStr) {

            SHA256 sha256 = SHA256.Create();

            string _getAuthStrCase0 = inputStr;
            byte[] _getAuthBytesCase0 = Encoding.UTF8.GetBytes(_getAuthStrCase0);
            byte[] _authHashCase0 = sha256.ComputeHash(_getAuthBytesCase0);
            string _authStrCase0 = BitConverter.ToString(_authHashCase0).Replace("-", "");

            return _authStrCase0;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            try {
                var _getNewUsername = guna2TextBox1.Text;
                var _getCustPass = guna2TextBox2.Text;
                if(_getNewUsername != CurrentUsername) {
                    if(_getNewUsername != String.Empty) {
                        if(_getCustPass != String.Empty) {
                            if(verifyIfNotExists(_getNewUsername) > 0) {
                                label4.Text = "Username is taken.";
                                label4.Visible = true;
                            } else {
                                if(authReturnOriginal(CurrentUsername) == computeAuthCase(_getCustPass)) {
                                    NewUsername = _getNewUsername;
                                    setupChangeUsername("information", NewUsername);
                                    setupChangeUsername("cust_type",NewUsername);
                                    setupChangeUsername("file_info", NewUsername);
                                    setupChangeUsername("file_info_expand", NewUsername);
                                    setupChangeUsername("file_info_word", NewUsername);
                                    setupChangeUsername("file_info_audi", NewUsername);
                                    setupChangeUsername("file_info_ptx", NewUsername);
                                    setupChangeUsername("file_info_pdf", NewUsername);
                                    setupChangeUsername("file_info_gif", NewUsername);
                                    setupChangeUsername("file_info_vid", NewUsername);
                                    setupChangeUsername("file_info_exe", NewUsername);
                                    setupChangeUsername("file_info_apk", NewUsername);
                                    setupChangeUsername("file_info_directory", NewUsername);

                                    setupChangeUsername("upload_info_directory", NewUsername);
                                    setupChangeUsername("folder_upload_info", NewUsername);
                                    setupChangeUsernameSharing(NewUsername);

                                    Form bgBlur = new Form();
                                    using (successChangeUser displayDirectory = new successChangeUser(_getNewUsername,CurrentUsername)) {
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
                                else {
                                    label4.Text = "Password is incorrect.";
                                    label4.Visible = true;
                                }
                            }
                        } else {
                            label4.Text = "Please enter your password.";

                            label4.Visible = true;                            
                            label4.Visible = true;
                        }
                    } else {
                        label4.Text = "Please enter a username.";
                    }
                } else {
                    label4.Text = "Please enter a new username.";
                    label4.Visible = true;
                }
            } catch (Exception) {
                MessageBox.Show("There's a problem while attempting to change your username.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e) {

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
    }
}
