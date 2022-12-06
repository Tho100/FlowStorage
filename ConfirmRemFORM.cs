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
        public ConfirmRemFORM() {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            String _getPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FlowStorageInfos";
            String _getAuth = _getPath + "\\CUST_DATAS.txt";
            if (File.Exists(_getAuth)) {
                String _UsernameFirst = File.ReadLines(_getAuth).First();
                if (_UsernameFirst == remAccFORM.instance.label1.Text) {
                    Directory.Delete(_getPath,true);
                }
            } 

            try {
                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 10851;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command,command_Read;
                con.Open();

                List<string> passValues_ = new List<string>();

                String checkPassword_Query = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
                command_Read = new MySqlCommand(checkPassword_Query,con);
                command_Read = con.CreateCommand();
                command_Read.CommandText = checkPassword_Query;
                command_Read.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                MySqlDataReader readerPass_ = command_Read.ExecuteReader();

                while(readerPass_.Read()) {
                    passValues_.Add(readerPass_.GetString(0));
                }
                readerPass_.Close();

                var decryptPass = EncryptionModel.Decrypt(passValues_[0],"ABHABH24");
                var encryptedPass = passValues_[0];

                if(guna2TextBox1.Text == decryptPass) {
                    String remInfo_Query = "DELETE FROM information WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remInfo_Query, con);
                    command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password",encryptedPass);
                    command.ExecuteNonQuery();

                    String remImg_Query = "DELETE FROM file_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remImg_Query, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    String remTxt_Query = "DELETE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remTxt_Query, con);
                    command.Parameters.AddWithValue("@username", remAccFORM.instance.label1.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    String remFolder = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remFolder, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    String remDirectory = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remDirectory, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    String remUploadDir = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remUploadDir, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    this.Close();
                    Application.OpenForms["remAccFORM"].Close();
                    Form1.instance.guna2Panel7.Visible = true;
                    Form1.instance.listBox1.Items.Clear();
                } else {
                    label1.Visible = true;
                }
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
