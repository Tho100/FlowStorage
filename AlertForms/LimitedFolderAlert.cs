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
    public partial class LimitedFolderAlert : Form {
        public LimitedFolderAlert(String accountType,String message, bool subMessageVisible) {
            InitializeComponent();

            this.label7.Text = $"Current Account: {accountType}";
            this.label8.Text = message;
            this.label6.Visible = subMessageVisible;

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new SettingsForm();
            remAccShow.Show();
            SettingsForm.instance.tabControlSettings.SelectedTab = SettingsForm.instance.tabControlSettings.TabPages["tabPage3"];
            this.Close();
        }

        private void limitFolderFORM_Load(object sender, EventArgs e) {

        }
    }
}
