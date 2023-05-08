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
    public partial class limitFolderFORM : Form {
        public limitFolderFORM(String accountType,String message, bool subMessageVisible) {
            InitializeComponent();

            label7.Text = $"Current Account: {accountType}";
            label8.Text = message;
            label6.Visible = subMessageVisible;

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            var remAccShow = new remAccFORM(Form1.instance.label5.Text, Form1.instance.label24.Text);
            remAccShow.Show();
            remAccFORM.instance.guna2TabControl1.SelectedTab = remAccFORM.instance.guna2TabControl1.TabPages["tabPage3"];
            this.Close();
        }

        private void limitFolderFORM_Load(object sender, EventArgs e) {

        }
    }
}
