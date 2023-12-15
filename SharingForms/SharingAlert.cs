using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.Sharing {
    public partial class SharingAlert : Form {

        public SharingAlert(string shareToName) {
            InitializeComponent();
            lblSharingTo.Text = $"Sharing to {shareToName}";
        }

        private void SharingAlert_Load(object sender, EventArgs e) {

        }

        private void lblSharingTo_Click(object sender, EventArgs e) {

        }

        private void btnCancelSharing_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
