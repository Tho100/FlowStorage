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

                List<string> passValues_ = new List<string>();

                String checkPassword_Query = "SELECT CUST_PASSWORD FROM information WHERE CUST_USERNAME = @username";
                ConnectionModel.command = new MySqlCommand(checkPassword_Query,ConnectionModel.con);
                ConnectionModel.command = ConnectionModel.con.CreateCommand();
                ConnectionModel.command.CommandText = checkPassword_Query;
                ConnectionModel.command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                MySqlDataReader readerPass_ = ConnectionModel.command.ExecuteReader();

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

                    String remInfo_Query = "DELETE FROM information WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    ConnectionModel.command = new MySqlCommand(remInfo_Query, ConnectionModel.con);
                    ConnectionModel.command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                    ConnectionModel.command.Parameters.AddWithValue("@password",encryptedPass);
                    ConnectionModel.command.ExecuteNonQuery();

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

                    String remApk_Query = "DELETE FROM file_info_apk WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remApk_Query, con);
                    command.Parameters.AddWithValue("@username", remAccFORM.instance.label1.Text);
                    command.Parameters.AddWithValue("@password", encryptedPass);
                    command.ExecuteNonQuery();

                    String remPdf_Query = "DELETE FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password";
                    command = new MySqlCommand(remPdf_Query, con);
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

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
