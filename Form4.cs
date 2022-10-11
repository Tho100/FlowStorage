using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
namespace FlowSERVER1
{
    public partial class Form4 : Form
    {
        public static Form4 instance;
        public Form4() {
            InitializeComponent();
            this.Text = "Create New Directory Page";
            this.Icon = new Icon(@"C:\Users\USER\Documents\FlowStorage4.ico");
            instance = this;
        }

        public void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        public void button3_Click(object sender, EventArgs e)
        {
         
        }

        public void Form4_Load(object sender, EventArgs e) {

        }

        private void textBox2_TextChanged(object sender, EventArgs e) {

        }
        int campf_q = 0;
        public void guna2Button2_Click(object sender, EventArgs e) {     
            campf_q++;
            string get_dir_title = guna2TextBox1.Text;
            Form3 dir_page = new Form3();
            dir_page.Text = "Directory: " + get_dir_title;
            dir_page.label1.Text = get_dir_title;
            //dir_page.Show();
            //this.Close();

            var flowlayout = Form1.instance.flowLayoutPanel1;
            int top = 275;
            int h_p = 100;
            var panelPic = new Guna2Panel() {
                Name = "DirPan" + campf_q,
                Width = 240,
                Height = 262,
                BorderRadius = 8,
                FillColor = ColorTranslator.FromHtml("#121212"),
                BackColor = Color.Transparent,
                Location = new Point(600, top)
            };

            top += h_p;
            flowlayout.Controls.Add(panelPic);
            var panel = ((Guna2Panel)flowlayout.Controls["DirPan" + campf_q]);

            Label dirName = new Label();
            panel.Controls.Add(dirName);
            dirName.Name = "DirName" + campf_q;
            dirName.Text = get_dir_title;
            dirName.Visible = true;
            dirName.Enabled = true;
            dirName.Font = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            dirName.ForeColor = Color.Gainsboro;
            dirName.Location = new Point(10, 10);
            dirName.Width = 1000;

            Label directoryLab = new Label();
            panel.Controls.Add(directoryLab);
            directoryLab.Name = "DirLab" + campf_q;
            directoryLab.Visible = true;
            directoryLab.Enabled = true;
            directoryLab.Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold);
            directoryLab.ForeColor = Color.DarkGray;
            directoryLab.Location = new Point(10, 40);
            directoryLab.BackColor = Color.Transparent;
            directoryLab.Width = 1000;
            directoryLab.Text = "Directory";

            Guna2PictureBox picBanner = new Guna2PictureBox();
            panel.Controls.Add(picBanner);
            picBanner.Name = "PicBanner" + campf_q;
            picBanner.Width = 240;
            picBanner.Height = 191;
            picBanner.BorderRadius = 8;
            picBanner.Location = new Point(0,72);
            picBanner.FillColor = ColorTranslator.FromHtml("#232323");
            picBanner.Visible = true;
            picBanner.Enabled = true;

        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }
    }
}
