using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Linq;
using System.Drawing;

using FlowSERVER1.Global;

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

        private string ControlName { get; set; }
        private string TableName { get; set; }
        private string FileName { get; set; }
        private string DirectoryName { get; set; }

        private System.Windows.Forms.Timer timer;
        private int progressValue = 0;

        public UploadingAlert(String fileName, String tableName,String controlName,String directoryName, long fileSize = 0) {

            InitializeComponent();

            instance = this;

            this.lblFileName.Text = fileName;
            this.ControlName = controlName;
            this.TableName = tableName;
            this.FileName = fileName;
            this.DirectoryName = directoryName;

            if(fileSize != 101) {
                lblFileSize.Text = fileSize.ToString() + "MB";
                lblFileSize.Visible = true;
            } else {
                lblFileSize.Visible = false;
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
        private void FileDeletionNormal(String FileName, String TableName) {

            string fileDeletionQuery = $"DELETE FROM {TableName} WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            using(MySqlCommand command = new MySqlCommand(fileDeletionQuery,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(FileName));
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// File deletion function for directory
        /// </summary>
        private void FileDeletionDirectory(String _FileName) {

            const string fileDeletionQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";

            using(MySqlCommand command = new MySqlCommand(fileDeletionQuery,con)) {
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName));
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
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
                command.Parameters.AddWithValue("@username", Globals.custUsername);
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
                         .Where(form => String.Equals(form.Name, "cancelFORM"))
                         .ToList()
                         .ForEach(form => form.Close());

                        var FileExt = FileName.Substring(FileName.Length-4);
                        var getName = FileName;

                        if(TableName == "null") {

                            if (Globals.imageTypes.Contains(FileExt)) {
                                FileDeletionNormal(FileName, GlobalsTable.homeImageTable);

                            } else if (FileExt == ".msi") {
                                FileDeletionNormal(FileName, GlobalsTable.homeMsiTable);

                            } else if (Globals.audioTypes.Contains(FileExt)) {
                                FileDeletionNormal(getName, GlobalsTable.homeAudioTable);

                            } else if (Globals.wordTypes.Contains(FileExt)) {
                                FileDeletionNormal(getName, GlobalsTable.homeWordTable);

                            } else if (Globals.ptxTypes.Contains(FileExt)) {
                                FileDeletionNormal(getName, GlobalsTable.homePtxTable);

                            } else if (FileExt == ".pdf") {
                                FileDeletionNormal(getName, GlobalsTable.homePdfTable);

                            } else if (Globals.textTypes.Contains(FileExt)) {
                                FileDeletionNormal(getName, GlobalsTable.homeTextTable);

                            } else if (FileExt == ".exe") {
                                FileDeletionNormal(getName, GlobalsTable.homeExeTable);

                            }

                        } else if (TableName == GlobalsTable.directoryUploadTable) {

                            if (Globals.imageTypes.Contains(FileExt)) {
                                FileDeletionDirectory(getName);

                            } else if (FileExt == ".msi") {
                                FileDeletionDirectory(getName);

                            } else if (Globals.audioTypes.Contains(FileExt)) {
                                FileDeletionDirectory(getName);

                            } else if (Globals.wordTypes.Contains(FileExt)) {
                                FileDeletionDirectory(getName);

                            } else if (Globals.ptxTypes.Contains(FileExt)) {
                                FileDeletionDirectory(getName);

                            } else if (FileExt == ".pdf") {
                                FileDeletionDirectory(getName);

                            } else if (Globals.textTypes.Contains(FileExt)) {
                                FileDeletionDirectory(getName);

                            } else if (FileExt == ".exe") {
                                FileDeletionDirectory(getName);
                            }

                        } else if (TableName == GlobalsTable.folderUploadTable) {
                            if (Globals.imageTypes.Contains(FileExt)) {
                                FileDeletionFolder(getName, GlobalsTable.homeImageTable);

                            } else if (FileExt == ".msi") {
                                FileDeletionFolder(getName, GlobalsTable.homeMsiTable);

                            } else if (Globals.audioTypes.Contains(FileExt)) {
                                FileDeletionFolder(getName, GlobalsTable.homeAudioTable);

                            } else if (Globals.wordTypes.Contains(FileExt)) {
                                FileDeletionFolder(getName, GlobalsTable.homeWordTable);

                            } else if (Globals.ptxTypes.Contains(FileExt)) {
                                FileDeletionFolder(getName, GlobalsTable.homePtxTable);

                            } else if (FileExt == ".pdf") {
                                FileDeletionFolder(getName, GlobalsTable.homePdfTable);

                            } else if (Globals.textTypes.Contains(FileExt)) {
                                FileDeletionFolder(getName, GlobalsTable.homeTextTable);

                            } else if (FileExt == ".exe") {
                                FileDeletionFolder(getName, GlobalsTable.homeExeTable);

                            }
                        } else if (TableName == GlobalsTable.sharingTable) {
                            FileDeletionSharing(getName);
                        }
                    }
                }

                if (TableName == "null") {

                    Control foundControl = null;
                    foreach(Control _getControls in HomePage.instance.flwLayoutHome.Controls) {
                        if(_getControls.Name == ControlName) {
                            foundControl = _getControls; 
                            break;
                        }
                    }

                    if (foundControl != null) {
                        HomePage.instance.flwLayoutHome.Controls.Remove(foundControl);
                        foundControl.Dispose();
                    }
                
                    HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();

                    if(HomePage.instance.flwLayoutHome.Controls.Count == 0) {
                        HomePage.instance.btnGarbageImage.Visible = true;
                        HomePage.instance.lblEmptyHere.Visible = true;
                    }

                } else if (TableName == GlobalsTable.directoryUploadTable) {
                    Control foundControl = null;
                    foreach (Control _getControls in DirectoryForm.instance.flwLayoutDirectory.Controls) {
                        if (_getControls.Name == ControlName) {
                            foundControl = _getControls;
                            break;
                        }
                    }

                    if (foundControl != null) {
                        DirectoryForm.instance.flwLayoutDirectory.Controls.Remove(foundControl);
                        foundControl.Dispose();
                    }

                    if (DirectoryForm.instance.flwLayoutDirectory.Controls.Count == 0) {
                        DirectoryForm.instance.guna2Button6.Visible = true;
                        DirectoryForm.instance.label8.Visible = true;
                    }

                } else if (TableName == GlobalsTable.folderUploadTable) {
                    Control foundControl = null;
                    foreach (Control _getControls in HomePage.instance.flwLayoutHome.Controls) {
                        if (_getControls.Name == ControlName) {
                            foundControl = _getControls;
                            break;
                        }
                    }

                    if (foundControl != null) {
                        HomePage.instance.flwLayoutHome.Controls.Remove(foundControl);
                        foundControl.Dispose();
                    }
                }

            } catch (Exception) {
                MessageBox.Show("Cancellation failed, file is already uploaded.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
