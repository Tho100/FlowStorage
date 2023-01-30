using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class UploadAlrt : Form {
        public static UploadAlrt instance;
        public static MySqlConnection conn = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String UploaderName;
        public UploadAlrt(String _fileName,String _uploaderName) {
            InitializeComponent();
            instance = this;
            label1.Text = _fileName;
            UploaderName = _uploaderName;
        }

        private void UploadAlrt_Load(object sender, EventArgs e) {

        }
       
        private void guna2Button10_Click(object sender, EventArgs e) {
            // FileDeletion("file_info_apk");
            /* FileDeletion("file_info");
             FileDeletion("file_info_msi");
             FileDeletion("file_info_exe");
             FileDeletion("file_info_msi");
             FileDeletion("file_info_gif");
             FileDeletion("file_info_audi");
             FileDeletion("file_info_word");
             FileDeletion("file_info_expand");
             FileDeletion("file_info_pdf");
             FileDeletion("file_info_ptx");*/
            conn.Close();
            this.Close();
        }
    }
}
