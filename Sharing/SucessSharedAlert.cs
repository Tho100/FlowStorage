using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class SucessSharedAlert : Form {
        public SucessSharedAlert(String fileName, String receiverName) {
            InitializeComponent();
            label7.Text = "File Name: " + fileName;
            label8.Text = "File sharing to " + receiverName + " was completed.";
        }

        private void sucessShare_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
