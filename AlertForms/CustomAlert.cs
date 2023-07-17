using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.AlertForms {
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
