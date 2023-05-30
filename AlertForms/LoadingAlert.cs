using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class LoadingAlert : Form {
        public LoadingAlert() {
            InitializeComponent();
            ToolTip exitButtonTt = new ToolTip();
            exitButtonTt.SetToolTip(guna2Button2,"Exit Flowstorage");
        }

        private void LoadAlertFORM_Load(object sender, EventArgs e) {
            this.TopMost = true;
            this.Focus();
            this.TopMost = true;
            this.TopLevel = true;
        }

        private void label8_Click(object sender, EventArgs e) {

        }
        /// <summary>
        /// Terminate thread and close form on 
        /// data retrieval (startup)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e) {

            foreach(var _Process in Process.GetProcessesByName("Flowstorage")) {
                _Process.Kill();
            }

            Application.OpenForms
                .OfType<Form>()
                .Where(form => String.Equals(form.Name, "LoadAlertFORM"))
                .ToList()
                .ForEach(form => form.Close());

            this.Close();
            Application.Exit();
        }
    }
}
