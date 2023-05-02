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
    /// <summary>
    /// Create directory form class
    /// </summary>
    public partial class Form4 : Form
    {
        public static Form4 instance;

        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;

        public Form4() {
            InitializeComponent();
            this.Text = "Create New Directory";
            instance = this;
        }
        public void Form4_Load(object sender, EventArgs e) {

        }

        public void clearRedundane() {
            Form1.instance.guna2Button6.Visible = false;
            Form1.instance.label8.Visible = false;
        }
        

        /// <summary>
        /// Start generating user directory panel into main form
        /// </summary>
        /// <param name="currMain"></param>
        /// <param name="getDirTitle"></param>
        public void generateDir(int currMain, String getDirTitle) {

            try {

                var flowlayout = Form1.instance.flowLayoutPanel1;
                int top = 275;
                int h_p = 100;
                var panelPic = new Guna2Panel() {
                    Name = "DirPan" + currMain,
                    Width = 240,
                    Height = 262,
                    BorderColor = ColorTranslator.FromHtml("#212121"),
                    BorderThickness = 1,
                    BorderRadius = 8,
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
                picBanner.Image = FlowSERVER1.Properties.Resources.DirIcon;
                picBanner.SizeMode = PictureBoxSizeMode.CenterImage;
                picBanner.BorderRadius = 8;
                picBanner.Width = 226;
                picBanner.Height = 165;
                picBanner.Visible = true;
                picBanner.Enabled = true;

                picBanner.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panel.Width - picBanner.Width) / 2;
                picBanner.Location = new Point(picMain_Q_x, 10);

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
                remButTxt.Image = FlowSERVER1.Properties.Resources.icons8_garbage_66;
                remButTxt.Visible = true;
                remButTxt.Location = new Point(189, 218);
                remButTxt.BringToFront();

                remButTxt.Click += (sender_tx, e_tx) => {
                    var titleFile = dirName.Text;
                    DialogResult verifyDialog = MessageBox.Show("Delete '" + titleFile + "' directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (verifyDialog == DialogResult.Yes) {

                        panel.Dispose();
                        Form1.instance.label4.Text = Form1.instance.flowLayoutPanel1.Controls.Count.ToString();

                        String _removeDirQuery = "DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeDirQuery, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(getDirTitle,EncryptionKey.KeyValue));
                        command.ExecuteNonQuery();

                        String _removeUploadQuery = "DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname";
                        command = new MySqlCommand(_removeUploadQuery, con);
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(getDirTitle,EncryptionKey.KeyValue));
                        command.ExecuteNonQuery();

                    }

                    if (Form1.instance.flowLayoutPanel1.Controls.Count == 0) {
                        Form1.instance.label8.Visible = true;
                        Form1.instance.guna2Button6.Visible = true;
                    }
                };

                picBanner.Click += (sender_f, e_f) => {

                    RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your directory files.","Loader");
                    ShowAlert.Show();
                    Form3 displayDirectory = new Form3(getDirTitle);
                    displayDirectory.Show();

                    Application.OpenForms
                   .OfType<Form>()
                   .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                   .ToList()
                   .ForEach(form => form.Close());
                };

                clearRedundane();

                this.Close();

            } catch (Exception) {
                MessageBox.Show("Are you conneted to the internet?","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        public void DisplayError(String accType) {
            Form bgBlur = new Form();
            using (DirErFORM displayPic = new DirErFORM(accType)) {
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.FormBorderStyle = FormBorderStyle.None;
                bgBlur.Opacity = .24d;
                bgBlur.BackColor = Color.Black;
                bgBlur.WindowState = FormWindowState.Maximized;
                bgBlur.TopMost = true;
                bgBlur.Location = this.Location;
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.ShowInTaskbar = false;
                bgBlur.Show();

                displayPic.Owner = bgBlur;
                displayPic.ShowDialog();

                bgBlur.Dispose();
            }
        }

        public void DisplayErrorUpgrade(String accType) {
            Form bgBlur = new Form();
            using (DirErFORM displayPic = new DirErFORM(accType)) {
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.FormBorderStyle = FormBorderStyle.None;
                bgBlur.Opacity = .24d;
                bgBlur.BackColor = Color.Black;
                bgBlur.WindowState = FormWindowState.Maximized;
                bgBlur.TopMost = true;
                bgBlur.Location = this.Location;
                bgBlur.StartPosition = FormStartPosition.Manual;
                bgBlur.ShowInTaskbar = false;
                bgBlur.Show();

                displayPic.Owner = bgBlur;
                displayPic.ShowDialog();

                bgBlur.Dispose();
            }
        }

        /// <summary>
        /// Verify necessary stuff before allowing user to create directory
        /// </summary>

        public static int value_Dir = 0;
        public static string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
        private async void guna2Button2_Click(object sender, EventArgs e) {
            String filesCount = Form1.instance.label4.Text;
            int totalFiles = int.Parse(filesCount);

            String username = Form1.instance.label5.Text;

            string accType = "";
            using (var command = new MySqlCommand("SELECT ACC_TYPE FROM cust_type WHERE CUST_USERNAME = @username", con)) {
                command.Parameters.AddWithValue("@username", username);
                using (var reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        accType = reader.GetString(0);
                    }
                }
            }

            String dirTitle = guna2TextBox1.Text.Trim();
            if (!String.IsNullOrEmpty(dirTitle)) {
                try {
                    var countSameDirCommand = new MySqlCommand("SELECT COUNT(DIR_NAME) FROM file_info_directory WHERE DIR_NAME = @dirname", con);
                    countSameDirCommand.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(dirTitle,EncryptionKey.KeyValue));
                    int countSameDir = Convert.ToInt32(countSameDirCommand.ExecuteScalar());

                    if (countSameDir < 1) {
                        var countTotalDirCommand = new MySqlCommand("SELECT COUNT(DIR_NAME) FROM file_info_directory WHERE CUST_USERNAME = @username", con);
                        countTotalDirCommand.Parameters.AddWithValue("@username", username);
                        int countTotalDir = Convert.ToInt32(countTotalDirCommand.ExecuteScalar());

                        int maxFilesCount = 0;
                        int maxDirCount = 0;
                        switch (accType) {
                            case "Supreme":
                                maxFilesCount = 2000;
                                maxDirCount = 5;
                                break;
                            case "Basic":
                                maxFilesCount = 20;
                                maxDirCount = 2;
                                break;
                            case "Max":
                                maxFilesCount = 500;
                                maxDirCount = 2;
                                break;
                            case "Express":
                                maxFilesCount = 1000;
                                maxDirCount = 2;
                                break;
                        }

                        if (totalFiles != maxFilesCount && countTotalDir != maxDirCount) {
                            value_Dir++;

                            if(Form1.instance.listBox1.Items[Form1.instance.listBox1.SelectedIndex] != "Home") {
                                MessageBox.Show("You can only create a directory on Home folder.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
                            } else {

                                generateDir(value_Dir, dirTitle);
                                Form1.instance.label4.Text = Form1.instance.flowLayoutPanel1.Controls.Count.ToString();

                                var insertValuesCommand = new MySqlCommand("INSERT INTO file_info_directory(DIR_NAME,CUST_USERNAME) VALUES (@DIR_NAME,@CUST_USERNAME)", con);
                                insertValuesCommand.Parameters.AddWithValue("@DIR_NAME", EncryptionModel.Encrypt(dirTitle, EncryptionKey.KeyValue));
                                insertValuesCommand.Parameters.AddWithValue("@CUST_USERNAME", username);
                                insertValuesCommand.ExecuteNonQuery();
                            }
                        }
                        else {
                            DisplayErrorUpgrade(accType);
                        }
                    }
                    else {
                        MessageBox.Show("Directory with this name already exists", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception) {
                    // @ 
                }
            }
        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
