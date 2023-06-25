using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.ExtraForms {
    public partial class PublishPublicStorage : Form {
        public PublishPublicStorage(string fileName) {
            InitializeComponent();
            txtFieldFileName.Text = fileName;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserComment = txtFieldComment.Text;
            this.Close();
        }

    }
}
