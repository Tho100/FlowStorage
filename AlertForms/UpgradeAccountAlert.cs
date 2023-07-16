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
    public partial class UpgradeAccountAlert : Form {
        public UpgradeAccountAlert() {
            InitializeComponent();
            this.label3.Text = $"Current Account: {Globals.accountType}"; 
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void upgradeFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new SettingsForm();
            remAccShow.Show();
            SettingsForm.instance.guna2TabControl1.SelectedTab = SettingsForm.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }
    }
}
