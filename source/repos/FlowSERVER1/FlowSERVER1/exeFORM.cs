using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class exeFORM : Form {
        public exeFORM(String getTitle) {
            InitializeComponent();
            label1.Text = getTitle;
        }

        private void exeFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
