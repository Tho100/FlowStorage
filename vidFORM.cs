using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class vidFORM : Form {
        public static vidFORM instance;
        public vidFORM(Image getThumb, int width, int height, String getTitle,String path) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            label3.Text = path;
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

        // Play
        private void guna2Button5_Click(object sender, EventArgs e) {
            try {
                _wmpVid.uiMode = "none";
                _wmpVid.Visible = true;
                _wmpVid.URL = label3.Text;
                _wmpVid.Ctlcontrols.play();
                guna2Button6.Visible = true;
                guna2Button5.Visible = false;
            } catch (Exception eq) {
                //
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _wmpVid.Ctlcontrols.pause();
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e) {

        }

        private void pictureBox1_Click_2(object sender, EventArgs e) {

        }
    }
}
