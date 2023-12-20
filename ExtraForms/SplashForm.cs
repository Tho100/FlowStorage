using System;
using System.Timers;
using System.Windows.Forms;
using FlowSERVER1.AlertForms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class SplashForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        private System.Timers.Timer keepAliveTimer;

        public SplashForm() {

            InitializeComponent();

            try {

                con.Open();

                keepAliveTimer = new System.Timers.Timer();
                keepAliveTimer.Interval = 23100; 
                keepAliveTimer.Elapsed += new ElapsedEventHandler(KeepAliveTimer_Elapsed);
                keepAliveTimer.Enabled = true;

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to start Flowstorage", subheader: "Are you connected to the internet?").Show();
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
