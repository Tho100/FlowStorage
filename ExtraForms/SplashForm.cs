using System;
using System.Timers;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class SplashForm : Form {
        private MySqlConnection con {get; set;} = ConnectionModel.con;
        private System.Timers.Timer keepAliveTimer;

        public SplashForm() {

            InitializeComponent();

            try {

                con.Open();

                keepAliveTimer = new System.Timers.Timer();
                keepAliveTimer.Interval = 30000; // 30 seconds
                keepAliveTimer.Elapsed += new ElapsedEventHandler(KeepAliveTimer_Elapsed);
                keepAliveTimer.Enabled = true;

            } catch (Exception) {
                MessageBox.Show("Are you connected to the internet?", "Flowstorage: An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void KeepAliveTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if (con.Ping() == false) {
                con.Close();
                con.Open();
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
