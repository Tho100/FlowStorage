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
    public partial class gifFORM : Form {
        public static gifFORM instance;
        public gifFORM(String _titleName) {
            InitializeComponent();
            var _form = Form1.instance;
            instance = this;

            label2.Text = "Uploaded By " + _form.label5.Text;
            label1.Text = _titleName;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
