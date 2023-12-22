﻿using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class SucessSharedAlert : Form {
        public SucessSharedAlert(String receiverName) {
            InitializeComponent();
            label8.Text = $"This file has been shared to {receiverName}.";
        }

        private void sucessShare_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
