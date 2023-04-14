using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Linq;

namespace FlowSERVER1 {

    /// <summary>
    /// 
    /// Alerts user on file upload class form
    /// that comes with button that allows the user to cancel the operation
    /// 
    /// </summary>

    public partial class UploadAlrt : Form {

        /// <summary>
        /// 
        /// Intialize necessary variables that will
        /// ease cancellation operation
        /// 
        /// </summary>

        public static UploadAlrt instance;
        public static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String UploaderName;
        private static String ControlName;
        private static String TableName;
        private static String FileName;
        private static String DirectoryName;
        private static String FileExt;
        public UploadAlrt(String _fileName,String _uploaderName,String _tableName,String _controlName,String _dirName, long _fileSize = 0) {
            InitializeComponent();
            instance = this;
            label1.Text = _fileName;
            UploaderName = _uploaderName;
            ControlName = _controlName;
            TableName = _tableName;
            FileName = _fileName;
            DirectoryName = _dirName;
            FileExt = _fileName.Substring(_fileName.Length-3);
            guna2TextBox1.Text = label1.Text;

            if(_fileSize != 101) {
                label3.Text = _fileSize.ToString() + "MB";
                label3.Visible = true;
            } else {
                label3.Visible = false;
            }
        }

        private void UploadAlrt_Load(object sender, EventArgs e) {

        }
        /// <summary>
        /// File deletion for normal file
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="TableName"></param>
        private void FileDeletionNormal(String FileName, String TableName) {
            String fileDeletionQuery = "DELETE FROM " + TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(fileDeletionQuery, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(FileName, EncryptionKey.KeyValue));
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// File deletion function for directory
        /// </summary>
        private void FileDeletionDirectory(String _FileName) {
            String fileDeletionQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(fileDeletionQuery, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName, EncryptionKey.KeyValue));
            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(DirectoryName,EncryptionKey.KeyValue));
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// File deletion for folder
        /// </summary>
        /// <param name="_FileName"></param>
        /// <param name="_FoldName"></param>
        private void FileDeletionFolder(String _FileName, String _FoldName) {
            String fileDeletionQuery = "DELETE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";
            command = new MySqlCommand(fileDeletionQuery, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName, EncryptionKey.KeyValue));
            command.Parameters.AddWithValue("@foldtitle", _FoldName);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// File deletion function for sharing
        /// </summary>
        private void FileDeletionSharing(String _CustTo,String _FileName) {
            String fileDeletionQuery = "DELETE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(fileDeletionQuery, con);
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(_FileName,EncryptionKey.KeyValue));
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// (Button) Delete file that has been cancelled on upload and
        /// remove them from database based on table name 
        /// (deletion will be executes on Form1 catch section)
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button10_Click(object sender, EventArgs e) {

            try {

                label9.Text = "Cancelling Operation...";
                if(con.State == System.Data.ConnectionState.Open) {

                    // @ Close connection before turning it back on for file deletion
                    con.Close();

                    if(con.State == System.Data.ConnectionState.Closed) {

                        // @ This will shows a form that alert user about the file upload 
                        // cancellation
                        Thread waitForm = new Thread(() => new cancelFORM(FileName).ShowDialog());
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
                            if (FileExt == ".png" || FileExt == ".jpeg" || FileExt == ".jpg" || FileExt == ".bmp") {
                                FileDeletionNormal(FileName, "file_info");
                            }
                            else if (FileExt == ".msi") {
                                FileDeletionNormal(FileName, "file_info_msi");
                            }
                            else if (FileExt == ".mp3" || FileExt == ".wav") {
                                FileDeletionNormal(getName, "file_info_audi");
                            }
                            else if (FileExt == ".docx" || FileExt == ".doc") {
                                FileDeletionNormal(getName, "file_info_word");
                            }
                            else if (FileExt == ".pptx" || FileExt == ".ppt") {
                                FileDeletionNormal(getName, "file_info_ptx");
                            }
                            else if (FileExt == ".pdf") {
                                FileDeletionNormal(getName, "file_info_pdf");
                            }
                            else if (FileExt == ".txt" || FileExt == ".py" || FileExt == ".html" || FileExt == ".js" || FileExt == ".css" || FileExt == ".sql") {
                                FileDeletionNormal(getName, "file_info_expand");
                            }
                            else if (FileExt == ".gif") {
                                FileDeletionNormal(getName, "file_info_gif");
                            }
                            else if (FileExt == ".exe") {
                                FileDeletionNormal(getName, "file_info_exe");
                            }
                        } else if (TableName == "upload_info_directory") {
                            if (FileExt == ".png" || FileExt == ".jpeg" || FileExt == ".jpg" || FileExt == ".bmp") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".msi") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".mp3" || FileExt == ".wav") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".docx" || FileExt == ".doc") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".pptx" || FileExt == ".ppt") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".pdf") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".txt" || FileExt == ".py" || FileExt == ".html" || FileExt == ".js" || FileExt == ".css" || FileExt == ".sql") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".gif") {
                                FileDeletionDirectory(getName);
                            }
                            else if (FileExt == ".exe") {
                                FileDeletionDirectory(getName);
                            }
                        } else if (TableName == "folder_upload_info") {
                            if (FileExt == ".png" || FileExt == ".jpeg" || FileExt == ".jpg" || FileExt == ".bmp") {
                                FileDeletionFolder(getName, "file_info");
                            }
                            else if (FileExt == ".msi") {
                                FileDeletionFolder(getName, "file_info_msi");
                            }
                            else if (FileExt == ".mp3" || FileExt == ".wav") {
                                FileDeletionFolder(getName, "file_info_audi");
                            }
                            else if (FileExt == ".docx" || FileExt == ".doc") {
                                FileDeletionFolder(getName, "file_info_word");
                            }
                            else if (FileExt == ".pptx" || FileExt == ".ppt") {
                                FileDeletionFolder(getName, "file_info_ptx");
                            }
                            else if (FileExt == ".pdf") {
                                FileDeletionFolder(getName, "file_info_pdf");
                            }
                            else if (FileExt == ".txt" || FileExt == ".py" || FileExt == ".html" || FileExt == ".js" || FileExt == ".css" || FileExt == ".sql") {
                                FileDeletionFolder(getName, "file_info_expand");
                            }
                            else if (FileExt == ".gif") {
                                FileDeletionFolder(getName, "file_info_gif");
                            }
                            else if (FileExt == ".exe") {
                                FileDeletionFolder(getName, "file_info_exe");
                            }
                        } else if (TableName == "cust_sharing") {
                            // @ Note: Directory name refer to receiver name
                            FileDeletionSharing(DirectoryName,getName);
                        }
                    }
                }

                if (TableName == "null") {

                    Control foundControl = null;
                    foreach(Control _getControls in Form1.instance.flowLayoutPanel1.Controls) {
                        if(_getControls.Name == ControlName) {
                            foundControl = _getControls; 
                            break;
                        }
                    }

                    if (foundControl != null) {
                        Form1.instance.flowLayoutPanel1.Controls.Remove(foundControl);
                        foundControl.Dispose();
                    }
                
                    Form1.instance.label4.Text = Form1.instance.flowLayoutPanel1.Controls.Count.ToString();

                    if(Form1.instance.flowLayoutPanel1.Controls.Count == 0) {
                        Form1.instance.guna2Button6.Visible = true;
                        Form1.instance.label8.Visible = true;
                    }

                } else if (TableName == "upload_info_directory") {
                    Control foundControl = null;
                    foreach (Control _getControls in Form3.instance.flowLayoutPanel1.Controls) {
                        if (_getControls.Name == ControlName) {
                            foundControl = _getControls;
                            break;
                        }
                    }

                    if (foundControl != null) {
                        Form3.instance.flowLayoutPanel1.Controls.Remove(foundControl);
                        foundControl.Dispose();
                    }

                    if (Form3.instance.flowLayoutPanel1.Controls.Count == 0) {
                        Form3.instance.guna2Button6.Visible = true;
                        Form3.instance.label8.Visible = true;
                    }

                } else if (TableName == "folder_upload_info") {
                    Control foundControl = null;
                    foreach (Control _getControls in Form1.instance.flowLayoutPanel1.Controls) {
                        if (_getControls.Name == ControlName) {
                            foundControl = _getControls;
                            break;
                        }
                    }

                    if (foundControl != null) {
                        Form1.instance.flowLayoutPanel1.Controls.Remove(foundControl);
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
    }
}
