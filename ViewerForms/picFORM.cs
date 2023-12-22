using AForge.Imaging.Filters;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using System.Collections.Generic;
using Guna.UI2.WinForms;
using System.Linq;
using System.IO;
using FlowstorageDesktop.Temporary;

namespace FlowstorageDesktop {
    public partial class PicForm : Form {

        public readonly PicForm instance;

        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private bool IsVisibleFilterPanel { get; set; } = false;
        private Image defaultImage { get; set; }
        private Bitmap filteredImage { get; set; }
        private bool isGrayed { get; set; }
        private int gaussianBlurValue { get; set; }
        private int brightnessValue { get; set; }
        private float saturationValue { get; set; }

        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private bool _isFromSharing { get; set; }
        private MySqlConnection con { get; set; } = ConnectionModel.con;

        public PicForm(Image userImage, int width, int height, string title, string tableName, string directoryName, string uploaderName, bool isFromShared = false, bool isFromSharing = false) {

            InitializeComponent();

            instance = this;

            InitializePicture(
                userImage, width, height, title, tableName, directoryName,
                uploaderName, isFromShared, isFromSharing);
        }

        private void InitializePicture(
            Image userImage, int width, int height,
            string title, string tableName, string directoryName,
            string uploaderName, bool isFromShared = false, bool isFromSharing = true) {

            this.lblFileName.Text = title;
            this.lblImageResolution.Text = $"({width}x{height})";

            this._tableName = tableName;
            this._directoryName = directoryName;
            this._isFromShared = isFromShared;
            this._isFromSharing = isFromSharing;

            var setupImage = new Bitmap(userImage);

            this.defaultImage = setupImage;
            this.filteredImage = new Bitmap(defaultImage);

            pbImage.Image = setupImage;

            if (isFromShared == true) {
                label4.Text = "Shared To";
                btnEditComment.Visible = true;
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: title) != "" ? GetComment.getCommentSharedToOthers(fileName: title) : "(No Comment)";

            } else {
                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: title) != "" ? GetComment.getCommentSharedToMe(fileName: title) : "(No Comment)";

            }

            if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                label4.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(fileName: title);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;

            }

            lblUploaderName.Text = uploaderName;

            if(_tableName == GlobalsTable.folderUploadTable || _tableName == GlobalsTable.homeImageTable) {
                btnPrevious.Visible = true;
                btnNext.Visible = true;

            } else {
                btnPrevious.Visible = false;
                btnNext.Visible = false;

            }

        }

        private void picFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.TopMost = true;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName, _isFromShared);
            this.TopMost = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            new shareFileFORM(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();
        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void filterPanel_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {

            label8.Text = guna2TrackBar1.Value.ToString() + "%";

            int getValue = guna2TrackBar1.Value;
            saturationValue = getValue / 100f;

            applyFilters();

        }

        /// <summary>
        /// 
        /// Apply saturation filter to image
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void FilterSaturation(float value) {

            SaturationCorrection filterSet = new SaturationCorrection(value);

            filterSet.ApplyInPlace(filteredImage);
            pbImage.Image = filteredImage;

        }

        /// <summary>
        /// 
        /// Apply gaussian blur filter to image
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void FilterGaussianBlur(int value) {

            int reduceValue = value * 50 / 100;

            GaussianBlur filter = new GaussianBlur();

            for (int i = 0; i < reduceValue; i++) {
                filter.ApplyInPlace(filteredImage);
            }

        }

        /// <summary>
        /// 
        /// Apply grayscale to the image
        /// 
        /// </summary>
        private void FilterGrayscale() {
            Grayscale filter = Grayscale.CommonAlgorithms.BT709;
            filteredImage = filter.Apply(filteredImage);
        }

        /// <summary>
        /// 
        /// Apply brightness filter to the image
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void FilterBrightness(int value) {
            BrightnessCorrection filter = new BrightnessCorrection(value);
            filter.ApplyInPlace(filteredImage);
        }

        private void applyFilters() {

            guna2Button7.Visible = true;
            guna2Button8.Visible = true;

            try {

                filteredImage = new Bitmap(defaultImage);

                if (isGrayed) {
                    FilterGrayscale();
                }

                if (gaussianBlurValue > 0) {
                    FilterGaussianBlur(gaussianBlurValue);
                }

                if (brightnessValue != 0) {
                    FilterBrightness(brightnessValue);
                }

                if (saturationValue != 0) {
                    FilterSaturation(saturationValue);
                }

                pbImage.Image = filteredImage;

            } catch (Exception) {
                MessageBox.Show("Cannot apply Grayscale with this filter.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        bool gaussianDialogShown = false;
        private void guna2TrackBar3_Scroll(object sender, ScrollEventArgs e) {

            label13.Text = guna2TrackBar3.Value.ToString() + "%";
            gaussianBlurValue = guna2TrackBar3.Value;

            if (!gaussianDialogShown) {
                DialogResult result = MessageBox.Show("Gaussian Blur isn't recommended for low-end systems as it can cause performance issues. Do you want to continue?", "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) {
                    return;
                }
                gaussianDialogShown = true;
            }

            applyFilters();

        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e) {
            isGrayed = !isGrayed;
            applyFilters();
        }

        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e) {
            label11.Text = guna2TrackBar2.Value.ToString() + "%";
            brightnessValue = guna2TrackBar2.Value;

            applyFilters();
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            IsVisibleFilterPanel = !IsVisibleFilterPanel;
            filterPanel.Visible = IsVisibleFilterPanel;
        }

        /// <summary>
        /// 
        /// Conert Bitmap image to byte then save the changes
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void guna2Button7_Click(object sender, EventArgs e) {

            ImageConverter convertByte = new ImageConverter();

            byte[] byteVals = (byte[])convertByte.ConvertTo(filteredImage, typeof(byte[]));

            string toBase64String = Convert.ToBase64String(byteVals);
            string encryptVals = EncryptionModel.Encrypt(toBase64String);

            await SaveChanges(encryptVals);

        }

        private async Task SaveChanges(string values) {

            try {

                if (_isFromShared == true && _tableName == GlobalsTable.sharingTable) {
                    await ExecuteChanges("UPDATE cust_sharing SET CUST_FILE = @newval WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", values);

                } else if (_isFromShared == false && _tableName == GlobalsTable.sharingTable) {
                    await ExecuteChanges("UPDATE cust_sharing SET CUST_FILE = @newval WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", values);

                } else if (_tableName == GlobalsTable.homeImageTable){
                    await ExecuteChanges("UPDATE file_info_image SET CUST_FILE = @newval WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename", values);

                } else if (_tableName == GlobalsTable.directoryUploadTable){
                    await ExecuteChangesDirectory("UPDATE upload_info_directory SET CUST_FILE = @newval WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname", values);

                } else if (_tableName == GlobalsTable.folderUploadTable) {
                    await ExecuteChangesFolder("UPDATE folder_upload_info SET CUST_FILE = @newval WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldname", values);

                } else {
                    MessageBox.Show("Can't apply filter for this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            } catch (Exception) {
                MessageBox.Show("Failed to save changes.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async Task ExecuteChanges(string query, string values) {

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@newval", values);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                if (await command.ExecuteNonQueryAsync() == 1) {
                    MessageBox.Show("Changes saved successfully.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private async Task ExecuteChangesDirectory(string query, string values) {

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@newval", values);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(_directoryName));
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                if (await command.ExecuteNonQueryAsync() == 1) {
                    MessageBox.Show("Changes saved successfully.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private async Task ExecuteChangesFolder(string query, string values) {

            string folderName = HomePage.instance.lblCurrentPageText.Text;

            using (MySqlCommand command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@newval", values);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                command.Parameters.AddWithValue("@foldname", EncryptionModel.Encrypt(folderName));
                command.Parameters.AddWithValue("@username", tempDataUser.Username);
                if (await command.ExecuteNonQueryAsync() == 1) {
                    MessageBox.Show("Changes saved successfully.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            guna2TrackBar1.Value = 0;
            guna2TrackBar2.Value = 0;
            guna2TrackBar3.Value = 0;
            label11.Text = "0%";
            label13.Text = "0%";
            label8.Text = "0%";
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button10.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button10_Click(object sender, EventArgs e) {

            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button10.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();

        }

        private void guna2Button11_Click_1(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private List<string> GetImageBase64Encoded() {

            List<string> imageBase64Encoded = new List<string>();

            if(_tableName == GlobalsTable.homeImageTable) {
                imageBase64Encoded = GlobalsData.base64EncodedImageHome;

            } else if (_tableName == GlobalsTable.folderUploadTable) {
                imageBase64Encoded = GlobalsData.base64EncodedImageFolder;

            } 

            return imageBase64Encoded;

        }

        private void SwitchImageImplementation(int direction) {

            try {

                string currentPage = HomePage.instance.lblCurrentPageText.Text;

                if (currentPage == "Home") {
                    _tableName = GlobalsTable.homeImageTable;

                } else if (currentPage != "Public Storage" || currentPage != "Home" || currentPage != "Shared To Me" || currentPage != "Shared Files") {
                    _tableName = GlobalsTable.folderUploadTable;

                } 

                List<string> fileNames = new List<string>(HomePage.instance.flwLayoutHome.Controls
                    .OfType<Guna2Panel>()
                    .SelectMany(panel => panel.Controls.OfType<Label>())
                    .Where(label => Globals.imageTypes.Any(ext => label.Text.ToLower().EndsWith(ext)))
                    .Where(label => label.Text.IndexOf('.') >= 0)
                    .Where(label => label.Text.Contains('.'))
                    .Select(label => label.Text.ToLower()));

                int currentFileIndex = fileNames.IndexOf(lblFileName.Text);

                int nextFileIndex = direction == -1 ? currentFileIndex - 1 : currentFileIndex + 1;
                string fileName = fileNames[nextFileIndex];
                string imageBase64Encoded = GetImageBase64Encoded().ElementAt(nextFileIndex);

                byte[] imageBytes = Convert.FromBase64String(imageBase64Encoded);
                using (MemoryStream stream = new MemoryStream(imageBytes)) {

                    Bitmap defaultImage = new Bitmap(stream);

                    int width = defaultImage.Width;
                    int height = defaultImage.Height;

                    if(_tableName == GlobalsTable.homeImageTable) {
                        PicForm displayPic = new PicForm(defaultImage, width, height, fileName, GlobalsTable.homeImageTable, String.Empty, tempDataUser.Username);
                        displayPic.Show();

                    } else if (_tableName == GlobalsTable.folderUploadTable) {
                        PicForm displayPic = new PicForm(defaultImage, width, height, fileName, GlobalsTable.folderUploadTable, String.Empty, tempDataUser.Username);
                        displayPic.Show();

                    } 

                    this.Close();

                }
    
            } catch (ArgumentOutOfRangeException) {};
        }

        private void guna2Button9_Click_1(object sender, EventArgs e) {
            SwitchImageImplementation(1);
        }

        private void guna2Button12_Click(object sender, EventArgs e) {
            SwitchImageImplementation(-1);
        }
    }
}
