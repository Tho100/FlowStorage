﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class LoadAlertFORM : Form {
        public LoadAlertFORM() {
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

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
            Application.Exit();
        }
    }
}
