using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Linq;
using System.Drawing;

using FlowSERVER1.Global;
using System.IO;
using FlowSERVER1.Temporary;

namespace FlowSERVER1 {

    /// <summary>
    /// 
    /// Alerts user on file upload class form
    /// that comes with button that allows the user to cancel the operation
    /// 
    /// </summary>

    public partial class UploadingAlert : Form {

        /// <summary>
        /// 
        /// Intialize necessary variables that will
        /// ease cancellation operation
        /// 
        /// </summary>

        public static UploadingAlert instance;

        private readonly MySqlConnection con = ConnectionModel.con;
        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string TableName { get; set; }
        private string FileName { get; set; }
        private string DirectoryName { get; set; }

        private System.Windows.Forms.Timer timer;
        private int progressValue = 0;

        public UploadingAlert(string fileName, string tableName, string directoryName, long fileSize = 0) {

            InitializeComponent();

            instance = this;

            this.lblFileName.Text = fileName;
            this.TableName = tableName;
            this.FileName = fileName;
            this.DirectoryName = directoryName;

            if(tableName == GlobalsTable.folderUploadTable) {
                lblFileSize.Visible = false;

            } else {
                lblFileSize.Text = fileSize.ToString() + "MB";
                lblFileSize.Visible = true;

            }

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5;
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e) {
            // Update the progress bar value
            loadingProgressBar.Value = progressValue;

            // Increment the progress value
            progressValue++;
            if (progressValue > loadingProgressBar.Maximum) {
                progressValue = 0; // Reset the progress value when it reaches the maximum
            }
        }

        private void UploadAlrt_Load(object sender, EventArgs e) {

            Rectangle screenBounds = Screen.GetBounds(this);

            int x = screenBounds.Right - this.Width - 10;
            int y = screenBounds.Bottom - this.Height - 45;
            this.Location = new Point(x, y);

        }
        /// <summary>
        /// File deletion for normal file
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="TableName"></param>
        private void FileDeletionNormal(string fileName, string tableName) {

            string fileDeletionQuery = $"DELETE FROM {tableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using(MySqlCommand command = new MySqlCommand(fileDeletionQuery,con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// File deletion function for directory
        /// </summary>
        private void FileDeletionDirectory(string fileName) {

            const string fileDeletionQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";

            using(MySqlCommand command = new MySqlCommand(fileDeletionQuery,con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(DirectoryName));
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// File deletion for folder
        /// </summary>
        /// <param name="_FileName"></param>
        /// <param name="_FoldName"></param>
        private void FileDeletionFolder(string _FileName, string _FoldName) {

            const string fileDeletionQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";

            using (MySqlCommand command = new MySqlCommand(fileDeletionQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName));
                command.Parameters.AddWithValue("@foldtitle", _FoldName);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// File deletion function for sharing
        /// </summary>
        private void FileDeletionSharing(string _FileName) {

            const string fileDeletionQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";

            using (MySqlCommand command = new MySqlCommand(fileDeletionQuery, con)) {
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName));

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// (Button) Delete file that has been cancelled on upload and
        /// remove them from database based on table name 
        /// (deletion will be executes on HomePage catch section)
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelUpload_Click(object sender, EventArgs e) {

            try {

                if(con.State == System.Data.ConnectionState.Open) {

                    // @ Close connection before turning it back on for file deletion
                    con.Close();

                    if(con.State == System.Data.ConnectionState.Closed) {

                        // @ This will shows a form that alert user about the file upload 
                        // cancellation
                        Thread waitForm = new Thread(() => new CancellingAlert().ShowDialog());
                        waitForm.Start();

                        // @ Turn connection back on to delete the cancelled file
                        con.Open();

                        Application.OpenForms
                         .OfType<Form>()
                         .Where(form => string.Equals(form.Name, "cancelFORM"))
                         .ToList()
                         .ForEach(form => form.Close());

                        string fileType = Path.GetExtension(FileName);

                        if(TableName == string.Empty) {

                            if (Globals.imageTypes.Contains(fileType)) {
                                FileDeletionNormal(FileName, GlobalsTable.homeImageTable);

                            } else if (fileType == ".msi") {
                                FileDeletionNormal(FileName, GlobalsTable.homeMsiTable);

                            } else if (Globals.audioTypes.Contains(fileType)) {
                                FileDeletionNormal(FileName, GlobalsTable.homeAudioTable);

                            } else if (Globals.wordTypes.Contains(fileType)) {
                                FileDeletionNormal(FileName, GlobalsTable.homeWordTable);

                            } else if (Globals.ptxTypes.Contains(fileType)) {
                                FileDeletionNormal(FileName, GlobalsTable.homePtxTable);

                            } else if (fileType == ".pdf") {
                                FileDeletionNormal(FileName, GlobalsTable.homePdfTable);

                            } else if (Globals.textTypes.Contains(fileType)) {
                                FileDeletionNormal(FileName, GlobalsTable.homeTextTable);

                            } else if (fileType == ".exe") {
                                FileDeletionNormal(FileName, GlobalsTable.homeExeTable);

                            }

                        } else if (TableName == GlobalsTable.directoryUploadTable) {

                            if (Globals.imageTypes.Contains(fileType)) {
                                FileDeletionDirectory(FileName);

                            } else if (fileType == ".msi") {
                                FileDeletionDirectory(FileName);

                            } else if (Globals.audioTypes.Contains(fileType)) {
                                FileDeletionDirectory(FileName);

                            } else if (Globals.wordTypes.Contains(fileType)) {
                                FileDeletionDirectory(FileName);

                            } else if (Globals.ptxTypes.Contains(fileType)) {
                                FileDeletionDirectory(FileName);

                            } else if (fileType == ".pdf") {
                                FileDeletionDirectory(FileName);

                            } else if (Globals.textTypes.Contains(fileType)) {
                                FileDeletionDirectory(FileName);

                            } else if (fileType == ".exe") {
                                FileDeletionDirectory(FileName);
                            }

                        } else if (TableName == GlobalsTable.folderUploadTable) {
                            if (Globals.imageTypes.Contains(fileType)) {
                                FileDeletionFolder(FileName, GlobalsTable.homeImageTable);

                            } else if (fileType == ".msi") {
                                FileDeletionFolder(FileName, GlobalsTable.homeMsiTable);

                            } else if (Globals.audioTypes.Contains(fileType)) {
                                FileDeletionFolder(FileName, GlobalsTable.homeAudioTable);

                            } else if (Globals.wordTypes.Contains(fileType)) {
                                FileDeletionFolder(FileName, GlobalsTable.homeWordTable);

                            } else if (Globals.ptxTypes.Contains(fileType)) {
                                FileDeletionFolder(FileName, GlobalsTable.homePtxTable);

                            } else if (fileType == ".pdf") {
                                FileDeletionFolder(FileName, GlobalsTable.homePdfTable);

                            } else if (Globals.textTypes.Contains(fileType)) {
                                FileDeletionFolder(FileName, GlobalsTable.homeTextTable);

                            } else if (fileType == ".exe") {
                                FileDeletionFolder(FileName, GlobalsTable.homeExeTable);

                            }
                        } else if (TableName == GlobalsTable.sharingTable) {
                            FileDeletionSharing(FileName);
                        }
                    }
                }

            } catch (Exception) {
                MessageBox.Show(
                    "Cancellation failed, file is already uploaded.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            this.Close();

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label9_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
