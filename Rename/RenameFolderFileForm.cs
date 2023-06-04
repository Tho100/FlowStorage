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
    public partial class RenameFolderFileForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        private MySqlCommand command = ConnectionModel.command;
        public RenameFolderFileForm(String foldTitle) {
            InitializeComponent();

            this.label2.Text = foldTitle;

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void renameFoldFORM_Load(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// On Confirm button click, verify any problem like if folder name
        /// already exists then alert the user, else rename the folder.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private async Task renameFolderAsync(string newFolderName,string oldFolderName) {

            const string queryRename = "UPDATE folder_upload_info SET FOLDER_TITLE = @newtitle WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @oldtitle";
            using (MySqlCommand command = new MySqlCommand(queryRename, con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@newtitle", EncryptionModel.Encrypt(newFolderName));
                command.Parameters.AddWithValue("@oldtitle", EncryptionModel.Encrypt(oldFolderName));

                try {

                    await command.ExecuteNonQueryAsync();

                    int indexOld = HomePage.instance.lstFoldersPage.FindString(oldFolderName);
                    if(indexOld != ListBox.NoMatches) {
                        HomePage.instance.lstFoldersPage.Items[indexOld] = newFolderName;
                    }

                    label5.Visible = true;
                    label5.ForeColor = ColorTranslator.FromHtml("#50a832");
                    label5.Text = $"Folder has been renamed to {newFolderName}.";

                } catch (Exception) {
                    label5.Visible = true;
                    label5.ForeColor = Color.Firebrick;
                    label5.Text = "Failed to rename this folder. Please try again.";
                }
            }
        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            string newFolderName = guna2TextBox1.Text;
            string oldFolderName = label2.Text;

            if(String.IsNullOrEmpty(newFolderName)) {
                return;
            }

            if(newFolderName == oldFolderName) {
                label5.Visible = true;
                label5.Text = "New folder name cannot be the same with old one.";
                return;
            }

            if (HomePage.instance.lstFoldersPage.Items.Contains(newFolderName)) {
                label5.Visible = true;
                label5.Text = "Folder with this name already exists.";
                return;
            }

            label5.Visible = false;
            await renameFolderAsync(newFolderName,oldFolderName);

        }
    }
}
