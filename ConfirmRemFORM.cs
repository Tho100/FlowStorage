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
    public partial class ConfirmRemFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public ConfirmRemFORM() {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            try {

                void remove_ItemsTab(String _tableName) {
                    String _remQueryBegin = "DELETE FROM " + _tableName +  " WHERE CUST_USERNAME = @username";
                    command = new MySqlCommand(_remQueryBegin, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.ExecuteNonQuery();
                }

                List<string> passValues_ = new List<string>();

                String checkPassword_Query = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(checkPassword_Query,con);
                command = ConnectionModel.con.CreateCommand();
                command.CommandText = checkPassword_Query;
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                MySqlDataReader readerPass_ = command.ExecuteReader();

                while(readerPass_.Read()) {
                    passValues_.Add(readerPass_.GetString(0));
                }
                readerPass_.Close();

                var decryptPass = EncryptionModel.Decrypt(passValues_[0],"ABHABH24");
                var encryptedPass = passValues_[0];

                if(guna2TextBox1.Text == decryptPass) {
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
                    remove_ItemsTab("file_info_apk");
                    remove_ItemsTab("file_info_exe");
                    remove_ItemsTab("file_info_excel");
                    remove_ItemsTab("file_info_directory");
                    remove_ItemsTab("upload_info_directory");
                    remove_ItemsTab("folder_upload_info");
                    remove_ItemsTab("file_info_msi");
                    remove_ItemsTab("cust_type");

                    this.Close();

                    Application.OpenForms["remAccFORM"].Close();
                    Form1.instance.guna2Panel7.Visible = true;
                    Form1.instance.listBox1.Items.Clear();
                } else {
                    label1.Visible = true;
                }
            } catch (Exception eq) {
                MessageBox.Show("Failed to delete account.","Flowstorage");
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
