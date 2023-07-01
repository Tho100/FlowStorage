using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowSERVER1.AlertForms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class ChangeUsernameForm : Form {

        private String CurrentUsername;

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

        public ChangeUsernameForm(String _CurrentUsername) {
            InitializeComponent();
            this.CurrentUsername = _CurrentUsername;
            this.guna2TextBox3.Text = CurrentUsername;
        }

        private void chagneUserForm_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
        private async Task setupChangeUsername(String _tableName, String _setUsername) {

            string updateUsernameQuery = $"UPDATE {_tableName} SET CUST_USERNAME = '" + _setUsername + "'WHERE CUST_USERNAME = @username";

            var param = new Dictionary<string, string>
            {
                { "@username",CurrentUsername},
            };


            await crud.Insert(updateUsernameQuery, param);
           
        }

        private async Task setupChangeUsernameSharing(String _setUsername) {

            string updateQuery = "UPDATE cust_sharing SET CUST_FROM = '" + _setUsername + "' WHERE CUST_FROM = @username";
            var param = new Dictionary<string, string>
{
                { "@username", CurrentUsername}
            };


            await crud.Insert(updateQuery, param);

        }

        private int verifyIfNotExists(String _newUsername) {

            List<String> _usernameValues = new List<string>();

            const string getUsernameQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(getUsernameQuery,con)) {
                command.Parameters.AddWithValue("@username",_newUsername);
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        _usernameValues.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
            }

            return _usernameValues.Count;
        }
        private string authReturnOriginal(String _usernameEntered) {

            List<string> _passValues = new List<string>();

            const string getAuthQuery = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(getAuthQuery,con)) {
                command.Parameters.AddWithValue("@username", _usernameEntered);
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        _passValues.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
            }

            return _passValues[0];

        }
        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void updateLocalUsername(String _custUsername) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(_custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private async void guna2Button1_Click(object sender, EventArgs e) {

            try {

                var _getNewUsername = guna2TextBox1.Text;
                var _getCustPass = guna2TextBox2.Text;

                if(_getNewUsername == CurrentUsername) {
                    label4.Text = "Please enter a new username.";
                    label4.Visible = true;
                    return;
                }

                if(_getNewUsername == String.Empty) {
                    label4.Text = "Please enter a username.";
                    label4.Visible = true;
                    return;
                }

                if(_getCustPass == String.Empty) {
                    label4.Text = "Please enter your password.";
                    label4.Visible = true;
                    return;
                }

                if(verifyIfNotExists(_getNewUsername) > 0) {
                    label4.Text = "Username is taken.";
                    label4.Visible = true;
                    return;
                }

                if(authReturnOriginal(CurrentUsername) == EncryptionModel.computeAuthCase(_getCustPass)) {


                    string[] tableNames = {
                        "information","cust_type","file_info", "file_info_expand", 
                        "file_info_word", "file_info_excel", "file_info_pdf", "file_info_audi", 
                        "file_info_vid", "sharing_info","lang_info","file_info_apk","file_info_exe",
                        "file_info_msi","file_info_directory","upload_info_directory","folder_upload_info"
                    };

                    for(int i=0; i<tableNames.Length; i++) {
                        await setupChangeUsername(tableNames[i], _getNewUsername);
                    }

                    foreach(var publicTablesPs in Globals.publicTablesPs) {
                        await setupChangeUsername(publicTablesPs, _getNewUsername);
                    }

                    await setupChangeUsernameSharing(_getNewUsername);

                    updateLocalUsername(_getNewUsername);

                    CustomAlert alert = new CustomAlert(title: "Username Updated",$"You've changed your username to '{_getNewUsername}' from {CurrentUsername}. Restart to fully apply changes.");
                    alert.Show();

                    Globals.custUsername = _getNewUsername;

                } else {
                    label4.Visible = true;
                    label4.Text = "Password is incorrect.";
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
