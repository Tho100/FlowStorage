using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class vidFORM : Form {
        public static vidFORM instance;
        public vidFORM(Image getThumb, int width, int height, String getTitle) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
        }

        public static Image resizeImage(Image userImg, Size size) {
            return (Image)(new Bitmap(userImg,size));
        }

        private void vidFORM_Load(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

        }
    }
}
