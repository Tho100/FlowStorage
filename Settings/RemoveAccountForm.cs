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
using System.IO;

namespace FlowSERVER1 {
    public partial class RemoveAccountForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        public RemoveAccountForm() {
            InitializeComponent();
        }

        private void remove_ItemsTab(String _tableName) {

            if (_tableName != "cust_sharing") {
                string _remQueryBegin = $"DELETE FROM {_tableName} WHERE CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(_remQueryBegin, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.ExecuteNonQuery();
                }
            }
            else {

                string _remSharingBeings = "DELETE FROM cust_sharing WHERE CUST_FROM = @username";
                using (MySqlCommand command = new MySqlCommand(_remSharingBeings, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.ExecuteNonQuery();
                }
            }

        }

        private string returnValues(String _WhichColumn) {

            List<string> _concludeValue = new List<string>();

            string checkPassword_Query = $"SELECT {_WhichColumn} FROM information WHERE CUST_USERNAME = @username";
            using (MySqlCommand command = new MySqlCommand(checkPassword_Query, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                using (MySqlDataReader readerPass_ = command.ExecuteReader()) {
                    while (readerPass_.Read()) {
                        _concludeValue.Add(readerPass_.GetString(0));
                    }
                }
            }

            return _concludeValue[0];

        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                var decryptPass = EncryptionModel.Decrypt(returnValues("CUST_PASSWORD"), "0123456789085746");
                var decryptPin = EncryptionModel.Decrypt(returnValues("CUST_PIN"), "0123456789085746");

                if(guna2TextBox2.Text == decryptPin) {
                    if (guna2TextBox1.Text == decryptPass) {
                        if(MessageBox.Show("Delete your account?\nYour data will be deleted PERMANENTLY.", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                             
                            String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
                            String _getAuth = _getPath + "\\CUST_DATAS.txt";
                            if (File.Exists(_getAuth)) {
                                Directory.Delete(_getPath,true);
                            }

                            remove_ItemsTab("information");
                            remove_ItemsTab("file_info");
                            remove_ItemsTab("file_info_expand");
                            remove_ItemsTab("file_info_ptx");
                            remove_ItemsTab("file_info_pdf");
                            remove_ItemsTab("file_info_gif");
                            remove_ItemsTab("file_info_word");
                            remove_ItemsTab("file_info_apk");
                            remove_ItemsTab("file_info_exe");
                            remove_ItemsTab("file_info_audi");
                            remove_ItemsTab("file_info_vid");
                            remove_ItemsTab("file_info_excel");
                            remove_ItemsTab("file_info_directory");
                            remove_ItemsTab("upload_info_directory");
                            remove_ItemsTab("folder_upload_info");
                            remove_ItemsTab("file_info_msi");
                            remove_ItemsTab("cust_type");
                            remove_ItemsTab("lang_info");
                            remove_ItemsTab("cust_buyer");
                            remove_ItemsTab("cust_sharing");
                            remove_ItemsTab("sharing_info");

                            this.Close();

                            Application.OpenForms["remAccFORM"].Close();
                            HomePage.instance.lstFoldersPage.Items.Clear();
                        }
                    } else {
                        label1.Visible = true;
                        label1.Text = "Password is incorrect.";
                    }
                } else {
                    label1.Visible = true;
                    label1.Text = "PIN is incorrect.";
                }
            } catch (Exception) {
                MessageBox.Show("Failed to delete account.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e) {
            if (System.Text.RegularExpressions.Regex.IsMatch(guna2TextBox2.Text, "[^0-9]")) {
                guna2TextBox2.Text = guna2TextBox2.Text.Remove(guna2TextBox2.Text.Length - 1);
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void ConfirmRemFORM_Load(object sender, EventArgs e) {

        }
    }
}
