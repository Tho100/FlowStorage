using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class RenameFolderFileForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public RenameFolderFileForm(String foldTitle) {
            InitializeComponent();

            this.lblCurrentFolderName.Text = foldTitle;
            txtFieldNewFolderName.Text = foldTitle;

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
        
        private async Task RenameFolderAsync(string newFolderName,string oldFolderName) {

            const string queryRename = "UPDATE folder_upload_info SET FOLDER_TITLE = @newtitle WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @oldtitle";
            using (MySqlCommand command = new MySqlCommand(queryRename, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@newtitle", EncryptionModel.Encrypt(newFolderName));
                command.Parameters.AddWithValue("@oldtitle", EncryptionModel.Encrypt(oldFolderName));

                try {

                    await command.ExecuteNonQueryAsync();

                    int indexOld = HomePage.instance.lstFoldersPage.FindString(oldFolderName);
                    if(indexOld != ListBox.NoMatches) {
                        HomePage.instance.lstFoldersPage.Items[indexOld] = newFolderName;
                    }

                    lblAlert.Visible = true;
                    lblAlert.ForeColor = ColorTranslator.FromHtml("#50a832");
                    lblAlert.Text = $"Folder has been renamed to {newFolderName}.";

                } catch (Exception) {
                    lblAlert.Visible = true;
                    lblAlert.ForeColor = Color.Firebrick;
                    lblAlert.Text = "Failed to rename this folder. Please try again.";
                }
            }
        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            string newFolderName = txtFieldNewFolderName.Text;
            string oldFolderName = lblCurrentFolderName.Text;

            if(String.IsNullOrEmpty(newFolderName)) {
                return;
            }

            if(newFolderName == oldFolderName) {
                lblAlert.Visible = true;
                lblAlert.Text = "New folder name cannot be the same with old one.";
                return;
            }

            if (HomePage.instance.lstFoldersPage.Items.Contains(newFolderName)) {
                lblAlert.Visible = true;
                lblAlert.Text = "Folder with this name already exists.";
                return;
            }

            lblAlert.Visible = false;
            await RenameFolderAsync(newFolderName,oldFolderName);

        }
    }
}
