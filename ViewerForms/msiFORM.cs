using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class msiFORM : Form {

        public string TableName;
        public string DirectoryName;

        public msiFORM(String fileName,String tableName, String directoryName,String uploaderUsername, bool _isFromShared = false) {
            InitializeComponent();
            this.TableName = tableName;
            this.DirectoryName = directoryName;
        }

        private void msiFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            SaverModel.SaveSelectedFile(lblFileName.Text, TableName, DirectoryName);
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }
    }
}
