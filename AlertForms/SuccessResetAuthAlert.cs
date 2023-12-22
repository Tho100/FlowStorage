using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class SuccessResetAuthAlert : Form {
        public SuccessResetAuthAlert() {
            InitializeComponent();
        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void btnUpgradePlan_Click(object sender, EventArgs e) {
            Application.Restart();
            Environment.Exit(0);
        }
    }
}
