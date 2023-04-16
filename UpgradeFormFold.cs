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
    public partial class UpgradeFormFold : Form {
        public UpgradeFormFold(String _currentAccount) {
            InitializeComponent();
            label3.Text = "Current Account: " + _currentAccount;
        }

        private void UpgradeFormFold_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new remAccFORM(label5.Text, Form1.instance.label24.Text);
            remAccShow.Show();
            remAccFORM.instance.guna2TabControl1.SelectedTab = remAccFORM.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }
    }
}
