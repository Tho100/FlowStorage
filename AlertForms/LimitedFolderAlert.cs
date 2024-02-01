using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class LimitedFolderAlert : Form {
        public LimitedFolderAlert(string accountType, string message, bool subMessageVisible) {
            InitializeComponent();

            this.label7.Text = $"Current Account: {accountType}";
            this.label8.Text = message;
            this.label6.Visible = subMessageVisible;

        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void guna2Button1_Click(object sender, EventArgs e) {
            new SettingsForm().Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabUpgradePage"];
            this.Close();
        }

        private void limitFolderFORM_Load(object sender, EventArgs e) {

        }
    }
}
