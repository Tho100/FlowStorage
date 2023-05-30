using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class ErrorLoadAlert : Form {
        public ErrorLoadAlert() {
            InitializeComponent();
            this.Text = "Please Restart";
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            Application.Restart();
            Environment.Exit(0);
        }

        private void errorLoad_Load(object sender, EventArgs e) {

        }
    }
}
