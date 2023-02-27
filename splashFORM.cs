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
    public partial class splashFORM : Form {
        public splashFORM() {
            InitializeComponent();
        }

        private void splashFORM_Load(object sender, EventArgs e) {
        }

        private void timer1_Tick(object sender, EventArgs e) {
            timer1.Start();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
