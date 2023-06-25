using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;

namespace FlowSERVER1 {
    public partial class picFORM : Form {

        public readonly picFORM instance; 

        private bool IsDragging {get; set; } = false;
        private bool IsVisibleFilterPanel {get; set; } = false;
        private Point dragStartPosition {get; set; }
        private Image defaultImage {get; set; } 
        private Bitmap filteredImage {get; set; }
        private bool isGrayed { get; set; }
        private int gaussianBlurValue { get; set; }
        private int brightnessValue { get; set; }
        private float saturationValue {get; set; }

        private string TableName {get; set; }
        private string Directoryname {get; set; }
        private bool IsFromShared {get; set; } 
        private bool IsFromSharing {get; set; } 
        private MySqlConnection con {get; set; } = ConnectionModel.con;

        public picFORM(Image userImage, int width, int height,string title,string tableName, string directoryName, string uploaderName,bool _IsFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            instance = this;

            filterPanel.MouseDown += filterPanel_MouseDown;
            initializePicture(userImage,width,height,title, tableName, directoryName, uploaderName, _IsFromShared, _isFromSharing);

        }

        private void initializePicture(Image userImage, int width, int height, string title, string tableName, string directoryName, string uploaderName, bool _IsFromShared = false, bool _isFromSharing = true) {


            this.lblFileName.Text = title;
            this.TableName = tableName;
            this.Directoryname = directoryName;
            this.IsFromShared = _IsFromShared;
            this.IsFromSharing = _isFromSharing;
            this.lblImageResolution.Text = $"({width}x{height})";

            var setupImage = new Bitmap(userImage);

            this.defaultImage = setupImage;
            this.filteredImage = new Bitmap(defaultImage);

            guna2PictureBox1.Image = setupImage;

            if (_IsFromShared == true) {
                label4.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: title) != "" ? GetComment.getCommentSharedToOthers(fileName: title) : "(No Comment)";
            }
            else {
                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: title) != "" ? GetComment.getCommentSharedToMe(fileName: title) : "(No Comment)";
            }

            if (Globals.publicTablesPs.Contains(tableName)) {
                label4.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(tableName: tableName, fileName: title, uploaderName: uploaderName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
            }

            lblUploaderName.Text = uploaderName;

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
            if (TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "upload_info_directory", Directoryname);
            }
            else if (TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "folder_upload_info", Directoryname);
            }
            else if (TableName == "file_info") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "file_info", Directoryname);
            } else if (TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(lblFileName.Text, "cust_sharing", Directoryname,IsFromShared);
            }
            this.TopMost = true;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(lblFileName.Text,getExtension,IsFromSharing, TableName, Directoryname);
            _showSharingFileFORM.Show();
        }

        private void label7_Click(object sender, EventArgs e) {

        }

        private void filterPanel_Paint(object sender, PaintEventArgs e) {
            
            filterPanel.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(filterPanel, true, null);
            
        }

        private void filterPanel_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                IsDragging = true;
                filterPanel.BringToFront(); 
                filterPanel.MouseMove += filterPanel_MouseMove;
                filterPanel.MouseDown += filterPanel_MouseDown;
                dragStartPosition = e.Location;
                filterPanel.BringToFront();
            }
        }

        private void filterPanel_MouseMove(object sender, MouseEventArgs e) {
            if (IsDragging && e.Button == MouseButtons.Left) {
                int x = filterPanel.Left + e.X - dragStartPosition.X;
                int y = filterPanel.Top + e.Y - dragStartPosition.Y;
                Rectangle panelRect = new Rectangle(x, y, filterPanel.Width, filterPanel.Height);
                if (filterPanel.Parent.ClientRectangle.Contains(panelRect)) {
                    filterPanel.Location = new Point(x, y);
                    filterPanel.Invalidate(); 
                }
            }
        }

        private void filterPanel_MouseUp(object sender, MouseEventArgs e) {
            IsDragging = false;
            filterPanel.MouseMove -= filterPanel_MouseMove;
            filterPanel.MouseDown -= filterPanel_MouseDown;
            filterPanel.MouseUp -= filterPanel_MouseUp;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void filterSaturation(float value) {

            SaturationCorrection filterSet = new SaturationCorrection(value);
            filterSet.ApplyInPlace(filteredImage);
            guna2PictureBox1.Image = filteredImage;
        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {

            label8.Text = guna2TrackBar1.Value.ToString() + "%";

            int getValue = guna2TrackBar1.Value;
            saturationValue = (float)getValue/100f;

            applyFilters();

            //filterSaturation(toFloat);
        }

        private void label6_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 
        /// Apply gaussian blur filter to image
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void filterGaussianBlur(int value) {

            int reduceValue = (int)(value*50/100);

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
        private void filterGrayscale() {
            Grayscale filter = Grayscale.CommonAlgorithms.BT709;
            filteredImage = filter.Apply(filteredImage);
        }

        /// <summary>
        /// 
        /// Apply brightness filter to the image
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void filterBrightness(int value) {
            BrightnessCorrection filter = new BrightnessCorrection(value);
            filter.ApplyInPlace(filteredImage);
        }


        int rotateValue = 0;
        Bitmap rotatedImage;
        private void applyFilters() {

            guna2Button7.Visible = true;
            guna2Button8.Visible = true;

            try {

                filteredImage = new Bitmap(defaultImage);

                if (isGrayed) {
                    filterGrayscale();
                }

                if (gaussianBlurValue > 0) {
                    filterGaussianBlur(gaussianBlurValue);
                }

                if (brightnessValue != 0) {
                    filterBrightness(brightnessValue);
                }

                if (saturationValue != 0) {
                    filterSaturation(saturationValue);
                }

           
                guna2PictureBox1.Image = filteredImage;

            } catch (Exception) {
                MessageBox.Show("Cannot apply Grayscale with this filter.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
            byte[] byteVals = (byte[])convertByte.ConvertTo(filteredImage,typeof(byte[]));
            string toBase64String = Convert.ToBase64String(byteVals);
            string encryptVals = EncryptionModel.Encrypt(toBase64String);
            await saveChanges(encryptVals);
        }

        private async Task saveChanges(string values) {

            try {
  
                if (IsFromShared == true) {
                    await executeChanges("UPDATE cust_sharing SET CUST_FILE = @newval WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename",values);
                } else if (IsFromShared == true) {
                    await executeChanges("UPDATE cust_sharing SET CUST_FILE = @newval WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename", values);
                } else {
                    await executeChanges("UPDATE file_info SET CUST_FILE = @newval WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename", values);
                }

            } catch (Exception) {
                MessageBox.Show("Failed to save changes.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private async Task executeChanges(string query,string values) {

            string que = query;
            using (MySqlCommand command = new MySqlCommand(que, con)) {
                command.Parameters.AddWithValue("@newval", values);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                if (await command.ExecuteNonQueryAsync() == 1) {
                    MessageBox.Show("Changes saved successfully. Please hit the refresh button on the top of folder tab to see changes.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            //guna2ToggleSwitch1.Checked = false;
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e) {

        }

        private async Task saveChangesComment(String updatedComment) {

            const string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Globals.custUsername);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            btnEditComment.Visible = false;
            guna2Button10.Visible = true;
            lblUserComment.Visible = false;
            guna2TextBox4.Text = lblUserComment.Text;
        }

        private async void guna2Button10_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            lblUserComment.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button10.Visible = false;
            guna2TextBox4.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            rotateValue += 90;

            // Reset the rotation angle to 0 after it reaches 360 degrees
            if (rotateValue >= 360) {
                rotateValue = 0;
                guna2PictureBox1.Image = filteredImage; // reset image
            }
            else {
                // Create a new Bitmap object with the rotated dimensions
                if (rotatedImage == null) {
                    rotatedImage = new Bitmap(filteredImage.Height, filteredImage.Width);
                }

                // Create a Graphics object from the rotated image
                using (Graphics g = Graphics.FromImage(rotatedImage)) {
                    // Rotate the image by the current rotation angle
                    g.Clear(Color.Transparent);
                    g.RotateTransform(rotateValue);

                    // Draw the original image onto the rotated image, adjusting for the new dimensions
                    g.DrawImage(filteredImage, new Rectangle(0, -filteredImage.Height, filteredImage.Width, filteredImage.Height));

                    // Set the PictureBox control's Image property to the rotated image
                    guna2PictureBox1.Image = rotatedImage;
                }
            }
            applyFilters();
        }

        private void guna2Button11_Click_1(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }
    }
}
