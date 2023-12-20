using FlowSERVER1.Temporary;
using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class LimitedDirAlert : Form {

        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public LimitedDirAlert() {
            InitializeComponent();
            this.label7.Text = $"Current Account: {tempDataUser.AccountType}";
        }


        private void DirErFORM_Load(object sender, EventArgs e) {

        }

        private void label8_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click_1(object sender, EventArgs e) => this.Close();
        
        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new SettingsForm();
            remAccShow.Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];
            this.Close();
        }
    }
}
