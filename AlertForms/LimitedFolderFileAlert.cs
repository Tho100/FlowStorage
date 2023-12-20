using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class LimitedFolderFileAlert : Form {
        public LimitedFolderFileAlert(string _currentAccount) {
            InitializeComponent();
            label3.Text = $"Current Account: {_currentAccount}";
        }

        private void UpgradeFormFold_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            SettingsForm remAccShow = new SettingsForm();
            remAccShow.Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabPage3"];
            Close();
        }
    }
}
