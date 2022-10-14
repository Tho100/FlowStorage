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
    public partial class picFORM : Form {
        public static picFORM instance;
        public picFORM(Image userImage, int width, int height,string title) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeUserImage(userImage,new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = title;
            label2.Text = Form1.instance.label5.Text;

            ToolTip saveTip = new ToolTip();
            saveTip.SetToolTip(this.guna2Button4,"Download Image");
        }

        public static Image resizeUserImage(Image userImage, Size size) {
            return (Image)(new Bitmap(userImage,size));
        }    

        private void picFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }
        
        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog() {Filter = @"PNG|.*png|JPG|.*jpg|JPEG|.*jpeg"}) {
                    if(saveFileDialog.ShowDialog() == DialogResult.OK) {
                        guna2PictureBox1.Image.Save(saveFileDialog.FileName);
                    }    
                }
            } catch (Exception eq) {
                MessageBox.Show("An error occurred while trying to save image\nMake sure you connected to the internet.","Flow Storage System");
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }
    }
}
