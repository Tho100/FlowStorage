using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class AudioPlayingForm : Form {
        public AudioPlayingForm(string audioName) {

            InitializeComponent();
            label2.Text = audioName;
        }

        private void AudioSubFORM_Load(object sender, EventArgs e) {

            Rectangle screenBounds = Screen.GetBounds(this);

            int x = screenBounds.Right - Width - 10;
            int y = screenBounds.Bottom - Height - 45;
            Location = new Point(x, y);
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
