using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class splashFORM : Form {
        private static MySqlConnection con {get; set;} = ConnectionModel.con;
        public splashFORM() {

            InitializeComponent();

            try {

                con.Open();

            } catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void splashFORM_Load(object sender, EventArgs e) {

        }

        private void timer1_Tick(object sender, EventArgs e) {
            timer1.Start();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
