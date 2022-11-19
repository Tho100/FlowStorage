using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class remAccFORM : Form {
        public static remAccFORM instance;
        public remAccFORM(String _accName) {
            InitializeComponent();
            label5.Text = _accName;
            instance = this;
        }

        private void remAccFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            ConfirmRemFORM con_Show = new ConfirmRemFORM();
            con_Show.Show();
        }
    }
}
