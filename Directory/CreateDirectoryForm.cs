
using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Query.DataCaller;
using FlowstorageDesktop.Temporary;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {

    public partial class CreateDirectoryForm : Form {

        public static CreateDirectoryForm instance;

        private readonly MySqlConnection con = ConnectionModel.con;
        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public CreateDirectoryForm() {
            InitializeComponent();
            instance = this;
        }

        public void ClearRedundane() {
            HomePage.instance.btnGarbageImage.Visible = false;
            HomePage.instance.lblEmptyHere.Visible = false;
        }


        /// <summary>
        /// Start generating user directory panel into main form
        /// </summary>
        /// <param name="currMain"></param>
        /// <param name="getDirTitle"></param>
        private void GenerateDirectory(int currParameter, string directoryName) {

            try {

                var flowlayout = HomePage.instance.flwLayoutHome;

                var panelPic = new Guna2Panel() {
                    Name = "DirPan" + currParameter,
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

                var panel = ((Guna2Panel)flowlayout.Controls["DirPan" + currParameter]);

                Label dirName = new Label();
                panel.Controls.Add(dirName);
                dirName.Name = "DirName" + currParameter;
                dirName.Text = directoryName;
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
                directoryLab.Name = "DirLab" + currParameter;
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
                picBanner.Name = "PicBanner" + currParameter;
                picBanner.Image = FlowstorageDesktop.Properties.Resources.DirIcon;
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
                remButTxt.Name = "RemTxtBut" + currParameter;
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

                remButTxt.Click += async (sender_tx, e_tx) => {

                    DialogResult verifyDialog = MessageBox.Show($"Delete '{directoryName}' directory?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (verifyDialog == DialogResult.Yes) {

                        var directoryDataCaller = new DirectoryDataCaller();
                        await directoryDataCaller.DeleteDirectory(directoryName);

                        panel.Dispose();

                        if (HomePage.instance.flwLayoutHome.Controls.Count == 0) {
                            HomePage.instance.lblEmptyHere.Visible = true;
                            HomePage.instance.btnGarbageImage.Visible = true;
                        }

                        HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();
                    }

                };

                picBanner.Click += (sender_f, e_f) => {

                    StartPopupForm.StartRetrievalPopup();

                    new DirectoryForm(directoryName).Show();

                    ClosePopupForm.CloseRetrievalPopup();

                };

                ClearRedundane();

                this.Close();

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to create directory", subheader: "Are you connected to the internet?").Show();
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

        private async Task ValidateAndCreateDirectory(int currentTotalFiles, string directoryName) {

            var directoriesName = new HashSet<string>(HomePage.instance.flwLayoutHome.Controls
                .OfType<Guna2Panel>()
                .SelectMany(panel => panel.Controls.OfType<Label>())
                .Where(label => label.Text.All(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)))
                .Where(label => !string.Equals(label.Text, "directory", StringComparison.OrdinalIgnoreCase))
                .Select(label => label.Text.ToLower()));

            int countTotalDir = directoriesName.Count();

            if (directoryName.ToLower().Contains("directory")) {
                MessageBox.Show("Can't name directory `directory`.", "Flowstorage", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (directoryName.Contains(".")) {
                MessageBox.Show("Can't name directory with end punctuation symbol.", "Flowstorage",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!(directoriesName.Contains(directoryName))) {

                int maxFilesCount = Globals.uploadFileLimit[tempDataUser.AccountType];
                int maxDirCount = Globals.uploadDirectoryLimit[tempDataUser.AccountType];

                if (currentTotalFiles != maxFilesCount && countTotalDir != maxDirCount) {

                    if (HomePage.instance.lblCurrentPageText.Text != "Home") {
                        MessageBox.Show("You can only create a directory on Home folder.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    } else {

                        GenerateDirectory(countTotalDir++, directoryName);
                        HomePage.instance.lblItemCountText.Text = HomePage.instance.flwLayoutHome.Controls.Count.ToString();

                        var crud = new Crud();

                        const string addDirQuery = "INSERT INTO file_info_directory VALUES (@dirname,@username)";
                        var param = new Dictionary<string, string>
                        {
                            { "@dirname", EncryptionModel.Encrypt(directoryName)},
                            { "@username",tempDataUser.Username}
                        };


                        await crud.Insert(addDirQuery, param);

                    }

                } else {
                    DisplayErrorUpgrade();

                }

            } else {
                MessageBox.Show(
                    "Directory with this name already exists", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        private async void btnCreateDirectory_Click(object sender, EventArgs e) {

            try {

                string filesCount = HomePage.instance.lblItemCountText.Text;
                int totalFiles = int.Parse(filesCount);

                string directoryNameInput = txtFieldDirName.Text.Trim();

                if (!string.IsNullOrEmpty(directoryNameInput)) {
                    await ValidateAndCreateDirectory(totalFiles, directoryNameInput);

                }

            } catch (Exception) {
                new CustomAlert(
                    title: "An error occurred", subheader: "Failed to create directory. Please try again later.").Show();

            }

        }

        private void guna2Button1_Click(object sender, EventArgs e) => this.Close();

    }
}
