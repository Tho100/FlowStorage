using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;

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
        public UploadAlrt(String _fileName,String _uploaderName,String _tableName,String _controlName,String _dirName) {
            InitializeComponent();
            instance = this;
            label1.Text = _fileName;
            UploaderName = _uploaderName;
            ControlName = _controlName;
            TableName = _tableName;
            FileName = _fileName;
            DirectoryName = _dirName;
            FileExt = _fileName.Substring(_fileName.Length-3);
        }

        private void UploadAlrt_Load(object sender, EventArgs e) {

        }
       /// <summary>
       /// File deletion function for normal file
       /// </summary>
       /// <param name="_tableName"></param>
        private void FileDeletionNormal(String _tableName) {
            Application.DoEvents();
            String fileDeletionQuery = "DELETE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(fileDeletionQuery,con);
            command.Parameters.AddWithValue("@username",UploaderName);
            command.Parameters.AddWithValue("@filename", FileName);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// File deletion function for directory
        /// </summary>
        private void FileDeletionDirectory() {
            Application.DoEvents();
            String fileDeletionQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(fileDeletionQuery, con);
            command.Parameters.AddWithValue("@username", UploaderName);
            command.Parameters.AddWithValue("@filename", FileName);
            command.Parameters.AddWithValue("@dirname", DirectoryName);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 
        /// (Button) Delete file that has been cancelled on upload and
        /// remove them from database based on table name
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button10_Click(object sender, EventArgs e) {
            
            Application.DoEvents();

            if(TableName == "null") {
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

                if(FileExt == "png" || FileExt == "jpeg" || FileExt == "jpg" || FileExt == "bmp") {
                    FileDeletionNormal("file_info");
                } else if (FileExt == "msi") {
                    FileDeletionNormal("file_info_msi");
                } else if (FileExt == "mp3" || FileExt == "wav") {
                    FileDeletionNormal("file_info_audi");
                } else if (FileExt == "docx" || FileExt == "doc") {
                    FileDeletionNormal("file_info_word");
                } else if (FileExt == "pptx" || FileExt == "ppt") {
                    FileDeletionNormal("file_info_ptx");
                } else if (FileExt == "pdf") {
                    FileDeletionNormal("file_info_pdf");
                } else if (FileExt == "txt" || FileExt == "py" || FileExt == "html" || FileExt == "js" || FileExt == "css") {
                    FileDeletionNormal("file_info_expand");
                } else if (FileExt == "gif") {
                    FileDeletionNormal("file_info_gif");
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

                FileDeletionDirectory();
                if (Form3.instance.flowLayoutPanel1.Controls.Count == 0) {
                    Form3.instance.guna2Button6.Visible = true;
                    Form3.instance.label8.Visible = true;
                }

            } else if (TableName == "cust_sharing") {
                Control foundControl = null;
                foreach (Control _getControls in sharingFORM.instance.flowLayoutPanel1.Controls) {
                    if (_getControls.Name == ControlName) {
                        foundControl = _getControls;
                        break;
                    }
                }

                if (foundControl != null) {
                    sharingFORM.instance.flowLayoutPanel1.Controls.Remove(foundControl);
                    foundControl.Dispose();
                }
            }

            this.Close();
        }
    }
}
