using System;
using System.Windows.Forms;

namespace FlowstorageDesktop.AlertForms {
    public partial class CustomAlert : Form {
        public CustomAlert(string title, string subheader) {
            InitializeComponent();
            lblTitle.Text = title;
            lblSubHeader.Text = subheader;
        }

        private void CustomAlert_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();
    }
}
