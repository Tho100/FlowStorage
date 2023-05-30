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
    public partial class SucessSharedAlert : Form {
        public SucessSharedAlert(String _fileName, String _receiverName) {
            InitializeComponent();
            label7.Text = "File Name: " + _fileName;
            label8.Text = "File sharing to " + _receiverName + " was completed.";
        }

        private void sucessShare_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
