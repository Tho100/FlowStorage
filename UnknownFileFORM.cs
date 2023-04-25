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
    public partial class UnknownFileFORM : Form {
        public UnknownFileFORM(String invalidFileName) {
            InitializeComponent();

            label1.Text = $"File Name: {invalidFileName}";
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
