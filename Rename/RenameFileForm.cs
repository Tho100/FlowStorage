using FlowstorageDesktop.Global;
using FlowstorageDesktop.Temporary;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class RenameFileForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string _fileName { get; set; }
        private string _tableName{ get; set; }
        private string _panelName { get; set; }
        private string _sharedToName { get; set; }
        private string _directoryName { get; set; }

        public RenameFileForm(string fileName, string tableName, string panelName, string directoryName = "", string sharedToName = "") {

            InitializeComponent();

            this._fileName = fileName;
            this._tableName = tableName;
            this._panelName = panelName;
            this._sharedToName = sharedToName;
            this._directoryName = directoryName;

            this.lblFileName.Text = fileName;

            string fileNameWithoutType = Path.GetFileNameWithoutExtension(fileName);
            txtFieldNewFileName.Text = fileNameWithoutType;
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private async Task RenameFileAsync(string newFileName) {

            if (GlobalsTable.publicTables.Contains(_tableName) || GlobalsTable.publicTablesPs.Contains(_tableName)) {

                string removeQuery = $"UPDATE {_tableName} SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand command = new MySqlCommand(removeQuery, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

                GlobalsData.filesMetadataCacheHome.Clear();

            } else if (_tableName == GlobalsTable.folderUploadTable) {

                using (MySqlCommand command = new MySqlCommand("UPDATE folder_upload_info SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_NAME = @foldername", con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                    command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(_directoryName));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

            } else if (_tableName == GlobalsTable.sharingTable && _sharedToName != "sharedToName") {

                const string removeQuery = "UPDATE cust_sharing SET CUST_FILE_PATH = @newname WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename AND CUST_TO = @sharedname";
                using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                    cmd.Parameters.AddWithValue("@username", tempDataUser.Username);
                    cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                    cmd.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    cmd.Parameters.AddWithValue("@sharedname", _sharedToName);

                    await cmd.ExecuteNonQueryAsync();
                }

            } else if (_tableName == GlobalsTable.sharingTable && _sharedToName == "sharedToName") {

                const string removeQuery = "UPDATE cust_sharing SET CUST_FILE_PATH = @newname WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                using (MySqlCommand cmd = new MySqlCommand(removeQuery, con)) {
                    cmd.Parameters.AddWithValue("@username", tempDataUser.Username);
                    cmd.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                    cmd.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    cmd.Parameters.AddWithValue("@sharedname", _sharedToName);

                    await cmd.ExecuteNonQueryAsync();
                }

            } else if (_tableName == GlobalsTable.directoryUploadTable) {

                using (MySqlCommand command = new MySqlCommand("UPDATE upload_info_directory SET CUST_FILE_PATH = @newname WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname", con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_fileName));
                    command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_directoryName));
                    command.Parameters.AddWithValue("@newname", EncryptionModel.Encrypt(newFileName));
                    await command.ExecuteNonQueryAsync();
                }

            }

            UpdateFileLabelUI(newFileName);

            lblAlert.Visible = true;
            lblAlert.ForeColor = ColorTranslator.FromHtml("#50a832");
            lblAlert.Text = $"File has been renamed to {newFileName}.";
        }

        private void UpdateFileLabelUI(string newFileName) {

            Control[] matches = new Control[0];

            if (_tableName != GlobalsTable.sharingTable && _tableName != GlobalsTable.folderUploadTable && _tableName != GlobalsTable.directoryUploadTable) {
                matches = HomePage.instance.Controls.Find(_panelName, true);
            }
            else if (_tableName == GlobalsTable.directoryUploadTable) {
                matches = DirectoryForm.instance.Controls.Find(_panelName, true);
            }

            if (matches.Length > 0 && matches[0] is Guna2Panel) {

                Guna2Panel myPanel = (Guna2Panel)matches[0];

                Label titleLabel = myPanel.Controls.OfType<Label>().LastOrDefault();
                titleLabel.Text = newFileName;
            }
        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            string fileExtensions = _fileName.Split('.').Last();
            string newFileName = txtFieldNewFileName.Text + "." + fileExtensions;

            if(string.IsNullOrEmpty(newFileName)) {
                return;
            }

            await RenameFileAsync(newFileName);

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void RenameFileForm_Load(object sender, EventArgs e) {

        }
    }
}
