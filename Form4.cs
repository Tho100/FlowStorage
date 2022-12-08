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
using MySql.Data.MySqlClient;
using MySql.Data;

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

        public void clearRedundane() {
            Form1.instance.guna2Button6.Visible = false;
            Form1.instance.label8.Visible = false;
        }
        
        public void uploadDir(String DirectoryTitle_) {
            //
        }

        public void generateDir(int currMain, String getDirTitle) {
            try {

                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 17873;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                var flowlayout = Form1.instance.flowLayoutPanel1;
                int top = 275;
                int h_p = 100;
                var panelPic = new Guna2Panel() {
                    Name = "DirPan" + currMain,
                    Width = 240,
                    Height = 262,
                    BorderRadius = 8,
                    FillColor = ColorTranslator.FromHtml("#121212"),
                    BackColor = Color.Transparent,
                    Location = new Point(600, top)
                };

                top += h_p;
                flowlayout.Controls.Add(panelPic);
                var panel = ((Guna2Panel)flowlayout.Controls["DirPan" + currMain]);

                Label dirName = new Label();
                panel.Controls.Add(dirName);
                dirName.Name = "DirName" + currMain;
                dirName.Text = getDirTitle;
                dirName.Visible = true;
                dirName.Enabled = true;
                dirName.Width = 220;
                dirName.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
                dirName.ForeColor = Color.Gainsboro;
                dirName.Location = new Point(12, 182);

                Label directoryLab = new Label();
                panel.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + currMain;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
                directoryLab.ForeColor = Color.DarkGray;
                directoryLab.Location = new Point(12, 208);
                directoryLab.BackColor = Color.Transparent;
                directoryLab.Width = 1000;
                directoryLab.Text = "Directory";

                Guna2PictureBox picBanner = new Guna2PictureBox();
                panel.Controls.Add(picBanner);
                picBanner.Name = "PicBanner" + currMain;
                picBanner.Image = FlowSERVER1.Properties.Resources.icon1;//Image.FromFile(@"C:\USERS\USER\Downloads\Gallery\icon1.png");
                picBanner.SizeMode = PictureBoxSizeMode.CenterImage;
                picBanner.BorderRadius = 8;
                picBanner.Width = 240;
                picBanner.Height = 164;
                picBanner.Visible = true;
                picBanner.Enabled = true;

                picBanner.MouseHover += (_senderM, _ev) => {
                    panel.ShadowDecoration.Enabled = true;
                    panel.ShadowDecoration.BorderRadius = 8;
                };

                picBanner.MouseLeave += (_senderQ, _evQ) => {
                    panel.ShadowDecoration.Enabled = false;
                };

                Guna2Button remButTxt = new Guna2Button();
                panel.Controls.Add(remButTxt);
                remButTxt.Name = "RemTxtBut" + currMain;
                remButTxt.Width = 39;
                remButTxt.Height = 35;
                remButTxt.FillColor = ColorTranslator.FromHtml("#4713BF");
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = ColorTranslator.FromHtml("#232323");
                remButTxt.Image = Image.FromFile(@"C:\Users\USER\Downloads\Gallery\icons8-garbage-66.png");
                remButTxt.Visible = true;
                remButTxt.Location = new Point(189, 218);
                remButTxt.BringToFront();

                remButTxt.Click += (sender_tx, e_tx) => {
                    var titleFile = dirName.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' directory?", "Flow Storage System", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {
                        //deletionMethod(titleFile, nameTable);
                        panel.Dispose();
                        Form1.instance.label4.Text = Form1.instance.flowLayoutPanel1.Controls.Count.ToString();


                        String _removeDirQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirQuery, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", getDirTitle);
                        command.ExecuteNonQuery();

                        String _removeUploadQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeUploadQuery, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", getDirTitle);
                        command.ExecuteNonQuery();

                    }

                    if (Form1.instance.flowLayoutPanel1.Controls.Count == 0) {
                        Form1.instance.label8.Visible = true;
                        Form1.instance.guna2Button6.Visible = true;
                    }
                };

                picBanner.Click += (sender_f, e_f) => {
                    Form backgrounDblur = new Form();
                    try {
                      using(Form3 dirForm = new Form3(dirName.Text)) {
                            backgrounDblur.StartPosition = FormStartPosition.Manual;
                            backgrounDblur.FormBorderStyle = FormBorderStyle.None;
                            backgrounDblur.Opacity = .28d;
                            backgrounDblur.BackColor = Color.Black;
                            backgrounDblur.WindowState = FormWindowState.Maximized;
                            backgrounDblur.TopMost = true;
                            backgrounDblur.Location = this.Location;
                            backgrounDblur.StartPosition = FormStartPosition.Manual;
                            backgrounDblur.ShowInTaskbar = false;
                            backgrounDblur.Show();

                            dirForm.Owner = backgrounDblur;

                            dirForm.ShowDialog();

                            backgrounDblur.Dispose();
                        }
                    } catch (Exception eq) {
                        //
                    }

                    //Form3 dirForm = new Form3(dirName.Text);
                    //dirForm.Show();
                };
                clearRedundane();
                this.Close();
            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }
        }
        public static int value_Dir = 0;
        public void guna2Button2_Click(object sender, EventArgs e) {
            String _GetDirTitle = guna2TextBox1.Text;
            if(_GetDirTitle != String.Empty) {
                value_Dir++;
                generateDir(value_Dir,_GetDirTitle);
            } 
            try {
            
                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 17873;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                var currentDate = DateTime.Now.ToString("dd/MM/yyyy");

                String _insertValues = "INSERT INTO file_info_directory(DIR_NAME,CUST_USERNAME,CUST_PASSWORD) VALUES (@DIR_NAME,@CUST_USERNAME,@CUST_PASSWORD)";
                command = new MySqlCommand(_insertValues,con);
                command.Parameters.Add("@DIR_NAME",MySqlDbType.Text);
                command.Parameters.Add("@CUST_USERNAME", MySqlDbType.Text);
                command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);

                command.Parameters["@DIR_NAME"].Value = _GetDirTitle;
                command.Parameters["@CUST_USERNAME"].Value = Form1.instance.label5.Text;
                command.Parameters["@CUST_PASSWORD"].Value = Form1.instance.label3.Text;

                command.ExecuteNonQuery();

            } catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }

            /*Form3 dir_page = new Form3();
            dir_page.Text = "Directory: " + _GetDirTitle;
            dir_page.label1.Text = _GetDirTitle;
            dir_page.Show();
            this.Close();*/

        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
