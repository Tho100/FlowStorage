using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class SucessSharedAlert : Form {
        public SucessSharedAlert(string receiverName) {
            InitializeComponent();
            label8.Text = $"This file has been shared to {receiverName}.";
        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();
        
    }
}
