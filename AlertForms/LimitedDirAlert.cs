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
    public partial class LimitedDirAlert : Form {
        public LimitedDirAlert() {
            InitializeComponent();
            this.label7.Text = $"Current Account: {Globals.accountType}";
        }


        private void DirErFORM_Load(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click_1(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new SettingsForm();
            remAccShow.Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabPage3"];
            this.Close();
        }
    }
}
