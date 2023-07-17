using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class UnknownTypeAlert : Form {
        public UnknownTypeAlert(String invalidFileName) {
            InitializeComponent();

            this.label1.Text = $"File Name: {invalidFileName}";
        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();
    }
}
