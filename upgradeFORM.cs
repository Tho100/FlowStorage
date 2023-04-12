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
    public partial class upgradeFORM : Form {
        public upgradeFORM(String _curAcc) {
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
            var remAccShow = new remAccFORM(label5.Text, Form1.instance.label24.Text);
            remAccShow.Show();
            remAccFORM.instance.guna2TabControl1.SelectedTab = remAccFORM.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }
    }
}
