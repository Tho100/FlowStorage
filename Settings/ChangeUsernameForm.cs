using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using FlowSERVER1.AlertForms;
using FlowSERVER1.Global;

namespace FlowSERVER1 {
    public partial class ChangeUsernameForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

        public ChangeUsernameForm() {
            InitializeComponent();
            this.txtFieldCurrentUsername.Text = Globals.custUsername;
        }

        private void chagneUserForm_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
        private async Task SetupChangeUsername(String tableName, String setUsername) {

            string updateUsernameQuery = $"UPDATE {tableName} SET CUST_USERNAME = '" + setUsername + "'WHERE CUST_USERNAME = @username";

            var param = new Dictionary<string, string>
            {
                { "@username",Globals.custUsername},
            };


            await crud.Insert(updateUsernameQuery, param);
           
        }

        private async Task SetupChangeUsernameSharing(String setUsername) {

            string updateQuery = "UPDATE cust_sharing SET CUST_FROM = '" + setUsername + "' WHERE CUST_FROM = @username";
            var param = new Dictionary<string, string>
{
                { "@username", Globals.custUsername}
            };


            await crud.Insert(updateQuery, param);

        }

        private int UsernameIsExistVerification(String newUsername) {

            List<String> _usernameValues = new List<string>();

            const string getUsernameQuery = "SELECT CUST_USERNAME FROM information WHERE CUST_USERNAME = @username";
            using(MySqlCommand command = new MySqlCommand(getUsernameQuery,con)) {
                command.Parameters.AddWithValue("@username", newUsername);
                using(MySqlDataReader reader = command.ExecuteReader()) {
                    if(reader.Read()) {
                        _usernameValues.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
            }

            return _usernameValues.Count;
        }
        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void UpdateLocalUsername(String custUsername) {

            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            if (!Directory.Exists(appDataPath)) {

                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else {
                Directory.Delete(appDataPath, true);
                DirectoryInfo setupDir = Directory.CreateDirectory(appDataPath);
                using (StreamWriter _performWrite = File.CreateText(appDataPath + "\\CUST_DATAS.txt")) {
                    _performWrite.WriteLine(EncryptionModel.Encrypt(custUsername, "0123456789085746"));
                }
                setupDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private async void guna2Button1_Click(object sender, EventArgs e) {

            try {

                var getNewUsername = txtFieldNewUsername.Text;
                var getCustAuth = txtFieldAuth.Text;

                if(getNewUsername == Globals.custUsername) {
                    lblAlert.Text = "Please enter a new username.";
                    lblAlert.Visible = true;
                    return;
                }

                if(getNewUsername == String.Empty) {
                    lblAlert.Text = "Please enter a username.";
                    lblAlert.Visible = true;
                    return;
                }

                if(getCustAuth == String.Empty) {
                    lblAlert.Text = "Please enter your password.";
                    lblAlert.Visible = true;
                    return;
                }

                if(UsernameIsExistVerification(getNewUsername) > 0) {
                    lblAlert.Text = "Username is taken.";
                    lblAlert.Visible = true;
                    return;
                }

                if(await crud.ReturnUserAuth() == EncryptionModel.computeAuthCase(getCustAuth)) {

                    string[] tableNames = {
                        "information","cust_type","file_info", "file_info_expand", 
                        "file_info_word", "file_info_excel", "file_info_pdf", "file_info_audi", 
                        "file_info_vid", "sharing_info","lang_info","file_info_apk","file_info_exe",
                        "file_info_msi","file_info_directory","upload_info_directory","folder_upload_info"
                    };

                    for(int i=0; i<tableNames.Length; i++) {
                        await SetupChangeUsername(tableNames[i], getNewUsername);
                    }

                    foreach(var publicTablesPs in GlobalsTable.publicTablesPs) {
                        await SetupChangeUsername(publicTablesPs, getNewUsername);
                    }

                    await SetupChangeUsernameSharing(getNewUsername);

                    UpdateLocalUsername(getNewUsername);

                    new CustomAlert(title: "Username Updated",$"You've changed your username to '{getNewUsername}' from {Globals.custUsername}. Restart to fully apply changes.").Show();

                    Globals.custUsername = getNewUsername;

                } else {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Password is incorrect.";
                }

            } catch (Exception) {
                new CustomAlert(title: "An error occurred", subheader: "Failed to change your username due to unknown error, are you connected to the internet?").Show();
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            guna2Button5.Visible = false;
            guna2Button6.Visible = true;
            txtFieldAuth.PasswordChar = '\0';
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
            txtFieldAuth.PasswordChar = '*';
        }
    }
}
