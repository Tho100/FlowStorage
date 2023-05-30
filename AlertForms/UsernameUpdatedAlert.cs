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
    public partial class UsernameUpdatedAlert : Form {
        public UsernameUpdatedAlert(String _newUsername, String _oldUsername) {
            InitializeComponent();
            label1.Text = _newUsername;
            label3.Text = _oldUsername;
            SettingsForm.instance.label5.Text = _newUsername;
        }

        private void successChangeUser_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            Application.Restart();
            Environment.Exit(0);
        }
    }
}
