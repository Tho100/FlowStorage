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
        private static String TableName;
        private static String Directoryname;
        public picFORM(Image userImage, int width, int height,string title,string _TableName, string _DirectoryName, string _UploaderName) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeUserImage(userImage,new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = title;
            label2.Text = "Uploaded By " + _UploaderName;
            TableName = _TableName;
            Directoryname = _DirectoryName;

            ToolTip saveTip = new ToolTip();
            saveTip.SetToolTip(this.guna2Button4,"Download Image");
        }

        public static Image resizeUserImage(Image userImage, Size size) {
            return (Image)(new Bitmap(userImage,size));
        }    

        private void picFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.TopMost = true;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            label1.AutoSize = false;
        }
        
        private void guna2Button4_Click(object sender, EventArgs e) {
            if (TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", Directoryname);
            }
            else if (TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", Directoryname);
            }
            else if (TableName == "file_info") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info", Directoryname);
            } else if (TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", Directoryname);
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }
    }
}
