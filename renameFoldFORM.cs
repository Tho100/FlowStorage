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
    public partial class renameFoldFORM : Form {
        private static string folderTitle {get; set; }
        public renameFoldFORM(String foldTitle) {
            InitializeComponent();

            folderTitle = foldTitle;
            label2.Text = foldTitle;

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void renameFoldFORM_Load(object sender, EventArgs e) {

        }
    }
}
