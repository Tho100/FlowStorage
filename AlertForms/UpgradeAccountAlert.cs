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
        public UpgradeAccountAlert(String _curAcc) {
            InitializeComponent();
            label3.Text = "Current Account: " + _curAcc; 
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            Application.OpenForms
                              .OfType<Form>()
                              .Where(form => String.Equals(form.Name, "upgradeFORM"))
                              .ToList()
                              .ForEach(form => form.Close());
        }

        private void upgradeFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new SettingsForm(label5.Text, HomePage.instance.label24.Text);
            remAccShow.Show();
            SettingsForm.instance.guna2TabControl1.SelectedTab = SettingsForm.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }
    }
}
