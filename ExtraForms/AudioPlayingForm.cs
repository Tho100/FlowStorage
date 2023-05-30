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
    public partial class AudioPlayingForm : Form {
        public AudioPlayingForm(String audioName) {

            InitializeComponent();
            label2.Text = audioName;
        }

        private void AudioSubFORM_Load(object sender, EventArgs e) {

            Rectangle screenBounds = Screen.GetBounds(this);

            int x = screenBounds.Right - this.Width - 10; 
            int y = screenBounds.Bottom - this.Height - 45; 
            this.Location = new Point(x, y);
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
