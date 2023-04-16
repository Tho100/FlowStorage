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
    public partial class DirErFORM : Form {
        public DirErFORM(String curAcc) {
            InitializeComponent();
           // label3.Text = "Current Account: " + curAcc;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void DirErFORM_Load(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click_1(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new remAccFORM(Form1.instance.label5.Text, Form1.instance.label24.Text);
            remAccShow.Show();
            remAccFORM.instance.guna2TabControl1.SelectedTab = remAccFORM.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }
    }
}
