
using FlowSERVER1.AlertForms;

using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FlowSERVER1 {
    /// <summary>
    /// Create directory form class
    /// </summary>
    public partial class CreateDirectoryForm : Form {
        public static CreateDirectoryForm instance;
        private readonly MySqlConnection con = ConnectionModel.con;

        public CreateDirectoryForm() {
            InitializeComponent();
            this.Text = "Create New Directory";
            instance = this;
        }
        public void Form4_Load(object sender, EventArgs e) {

        }

        public void clearRedundane() {
            HomePage.instance.btnGarbageImage.Visible = false;
            HomePage.instance.lblEmptyHere.Visible = false;
        }


        /// <summary>
        /// Start generating user directory panel into main form
        /// </summary>
        /// <param name="currMain"></param>
        /// <param name="getDirTitle"></param>
        private void GenerateDirectory(int currMain, string getDirTitle) {

            try {

                var flowlayout = HomePage.instance.flwLayoutHome;
                var panelPic = new Guna2Panel() {
                    Name = "DirPan" + currMain,
                    Width = 200,
                    Height = 222,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

                flowlayout.Controls.Add(panelPic);

                var panel = ((Guna2Panel)flowlayout.Controls["DirPan" + currMain]);

                Label dirName = new Label();
                panel.Controls.Add(dirName);
                dirName.Name = "DirName" + currMain;
                dirName.Text = getDirTitle;
                dirName.Visible = true;
                dirName.Enabled = true;
                dirName.Font = GlobalStyle.TitleLabelFont;
                dirName.ForeColor = GlobalStyle.GainsboroColor;
                dirName.Visible = true;
                dirName.Enabled = true;
                dirName.Location = GlobalStyle.TitleLabelLoc;
                dirName.Width = 160;
                dirName.Height = 20;
                dirName.AutoEllipsis = true;

                Label directoryLab = new Label();
                panel.Controls.Add(directoryLab);
                directoryLab.Name = "DirLab" + currMain;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Font = GlobalStyle.DateLabelFont;
                directoryLab.ForeColor = GlobalStyle.DarkGrayColor;
                directoryLab.Visible = true;
                directoryLab.Enabled = true;
                directoryLab.Location = GlobalStyle.DateLabelLoc;
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
                remButTxt.Width = 29;
                remButTxt.Height = 26;
                remButTxt.ImageOffset = GlobalStyle.GarbageOffset;
                remButTxt.FillColor = GlobalStyle.TransparentColor;
                remButTxt.BorderRadius = 6;
                remButTxt.BorderThickness = 1;
                remButTxt.BorderColor = GlobalStyle.TransparentColor;
                remButTxt.Image = Globals.DirectoryGarbageImage;
                remButTxt.Visible = true;
                remButTxt.Location = GlobalStyle.GarbageButtonLoc;
                remButTxt.BringToFront();

                remButTxt.Click += (sender_tx, e_tx) => {


                    var titleFile = dirName.Text;

                    DialogResult verifyDialog = MessageBox.Show($"Delete '{titleFile}' directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (verifyDialog == DialogResult.Yes) {

                        using (var command = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", con)) {
                            command.ExecuteNonQuery();
                        }

                        using (var command = new MySqlCommand("DELETE FROM file_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleFile));
                            command.ExecuteNonQuery();
                        }

                        using (var command = new MySqlCommand("DELETE FROM upload_info_directory WHERE CUST_USERNAME = @username AND DIR_NAME = @dirname", con)) {
                            command.Parameters.AddWithValue("@username", Globals.custUsername);
                            command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(titleFile));
                            command.ExecuteNonQuery();
                        }

                        panel.Dispose();


                        if (HomePage.instance.flwLayoutHome.Controls.Count == 0) {
                            HomePage.instance.lblEmptyHere.Visible = true;
                            HomePage.instance.btnGarbageImage.Visible = true;
                        }

                        HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();
                    }

                };

                picBanner.Click += (sender_f, e_f) => {

                    RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your directory files.", "Loader");
                    ShowAlert.Show();
                    DirectoryForm displayDirectory = new DirectoryForm(getDirTitle);
                    displayDirectory.Show();

                    Application.OpenForms
                   .OfType<Form>()
                   .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                   .ToList()
                   .ForEach(form => form.Close());
                };

                clearRedundane();

                this.Close();

            }
            catch (Exception) {
                new CustomAlert(title: "Failed to create directory", subheader: "Are you connected to the internet?").Show();
            }
        }

        private void DisplayErrorUpgrade() {
            Form bgBlur = new Form();
            using (LimitedDirAlert displayPic = new LimitedDirAlert()) {
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
        /// 
        /// Check if directory with the same name already exist if not,
        /// then create the directory
        /// 
        /// </summary>

        private int value_Dir = 0;
        private async void guna2Button2_Click(object sender, EventArgs e) {

            string filesCount = HomePage.instance.lblItemCountText.Text;
            int totalFiles = int.Parse(filesCount);

            string username = Globals.custUsername;

            string dirTitle = guna2TextBox1.Text.Trim();
            if (!String.IsNullOrEmpty(dirTitle)) {

                try {

                    var countSameDirCommand = new MySqlCommand("SELECT COUNT(DIR_NAME) FROM file_info_directory WHERE DIR_NAME = @dirname", con);
                    countSameDirCommand.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(dirTitle));
                    int countSameDir = Convert.ToInt32(countSameDirCommand.ExecuteScalar());

                    if (countSameDir < 1) {

                        var countTotalDirCommand = new MySqlCommand("SELECT COUNT(DIR_NAME) FROM file_info_directory WHERE CUST_USERNAME = @username", con);
                        countTotalDirCommand.Parameters.AddWithValue("@username", username);
                        int countTotalDir = Convert.ToInt32(countTotalDirCommand.ExecuteScalar());

                        int maxFilesCount = Globals.uploadFileLimit[Globals.accountType];
                        int maxDirCount = Globals.uploadDirectoryLimit[Globals.accountType];

                        if (totalFiles != maxFilesCount && countTotalDir != maxDirCount) {

                            value_Dir++;

                            if ((string)HomePage.instance.lstFoldersPage.Items[HomePage.instance.lstFoldersPage.SelectedIndex] != "Home") {
                                MessageBox.Show("You can only create a directory on Home folder.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else {

                                GenerateDirectory(value_Dir, dirTitle);
                                HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();

                                var crud = new Crud();

                                const string addDirQuery = "INSERT INTO file_info_directory VALUES (@dirname,@username)";
                                var param = new Dictionary<string, string>
                                {
                                    { "@dirname", EncryptionModel.Encrypt(dirTitle)},
                                    { "@username",Globals.custUsername}
                                };


                                await crud.Insert(addDirQuery, param);

                            }
                        }
                        else {
                            DisplayErrorUpgrade();
                        }
                    }
                    else {
                        MessageBox.Show("Directory with this name already exists", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception) {
                    new CustomAlert(title: "An error occurred", subheader: "Failed to create directory. Please try again later.").Show();
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
