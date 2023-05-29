using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class renameFORM : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        
        private string titleFile {get; set; }
        private string tableName{ get; set; }
        private string panelName { get; set; }
        private string sharedToName { get; set; }
        private string dirName { get; set; }

        public renameFORM(String _fileName,String _tableName, String _panelName, String _dirName = "", String _sharedToName = "") {

            InitializeComponent();

            titleFile = _fileName;
            tableName = _tableName;
            panelName = _panelName;
            sharedToName = _sharedToName;
            dirName = _dirName;

            label2.Text = titleFile;
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private async Task renameFileAsync(String newFileName) {

            if (tableName != "cust_sharing" && tableName != "folder_upload_info" && tableName != "file_info_directory") {

                string removeQuery = $"UPDATE {tableName} SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == "folder_upload_info") {

                using (MySqlCommand command = new MySqlCommand("UPDATE folder_upload_info SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldername", con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                    command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(dirName));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == "cust_sharing" && sharedToName != "sharedToName") {

                const string removeQuery = "UPDATE cust_sharing SET CUST_FILE_PATH = @newname WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                    cmd.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                    cmd.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    cmd.Parameters.AddWithValue("@sharedname", sharedToName);

                    await cmd.ExecuteNonQueryAsync();
                }

            }
            else if (tableName == "cust_sharing" && sharedToName == "sharedToName") {

                const string removeQuery = "UPDATE cust_sharing SET CUST_FILE_PATH = @newname WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                    cmd.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                    cmd.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    cmd.Parameters.AddWithValue("@sharedname", sharedToName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            else if (tableName == "file_info_directory") {

                using (MySqlCommand command = new MySqlCommand("UPDATE upload_info_directory SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname", con)) {
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(titleFile));
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(dirName));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

            }

            Control[] matches = new Control[0];

            if (tableName != "cust_sharing" && tableName != "folder_upload_info" && tableName != "file_info_directory") {
                matches = Form1.instance.Controls.Find(panelName, true);
            } else if (tableName == "file_info_directory") {
                matches = Form3.instance.Controls.Find(panelName, true);
            }

            if (matches.Length > 0 && matches[0] is Panel) {

                Panel myPanel = (Panel)matches[0];

                Label titleLabel = myPanel.Controls.OfType<Label>().LastOrDefault();
                titleLabel.Text = newFileName;
            }

            label5.Visible = true;
            label5.ForeColor = ColorTranslator.FromHtml("#50a832");
            label5.Text = $"File has been renamed to {newFileName}.";
        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            string fileExtensions = titleFile.Split('.').Last();
            string newFileName = guna2TextBox1.Text + "." + fileExtensions;

            if(String.IsNullOrEmpty(newFileName)) {
                return;
            }

            await renameFileAsync(newFileName);

        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}
