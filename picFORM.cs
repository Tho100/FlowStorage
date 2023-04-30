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
        public static picFORM instance; 

        private bool IsDragging {get; set; } = false;
        private bool IsVisibleFilterPanel {get; set; } = false;
        private bool ImageisRotated {get; set; } = false;
        private Point dragStartPosition {get; set; }
        private Image defaultImage {get; set; } 
        private Bitmap filteredImage {get; set; }
        private bool isGrayed { get; set; }
        private int gaussianBlurValue { get; set; }
        private int brightnessValue { get; set; }
        private float saturationValue {get; set; }


        private static String TableName {get; set; }
        private static String Directoryname {get; set; }
        private static bool IsFromShared {get; set; } // @ Shared to others
        private static bool IsFromSharing {get; set; }  // @ Shared to me 
        private static MySqlConnection con {get; set; } = ConnectionModel.con;

        public picFORM(Image userImage, int width, int height,string title,string _TableName, string _DirectoryName, string _UploaderName,bool _IsFromShared = false, bool _isFromSharing = true) {
            InitializeComponent();

            instance = this;

            filterPanel.MouseDown += filterPanel_MouseDown;
            IntiailizePictureAsync(userImage,width,height,title,_TableName,_DirectoryName,_UploaderName,_IsFromShared, _isFromSharing);

        }

        private async void IntiailizePictureAsync(Image userImage, int width, int height, string title, string _TableName, string _DirectoryName, string _UploaderName, bool _IsFromShared = false, bool _isFromSharing = true) {

            String _getName = "";
            bool _isShared = Regex.Match(_UploaderName, @"^([\w\-]+)").Value == "Shared";

            var setupImage = new Bitmap(userImage);//resizeUserImage(userImage, new Size(width, height));

            defaultImage = setupImage;
            filteredImage = new Bitmap(defaultImage);

            guna2PictureBox1.Image = setupImage;

            label1.Text = title;
            TableName = _TableName;
            Directoryname = _DirectoryName;
            IsFromShared = _IsFromShared;
            IsFromSharing = _isFromSharing;
            label7.Text = $"({width}x{height})";

            if (_isShared == true) {
                _getName = _UploaderName.Replace("Shared", "");
                label4.Text = "Shared To";
                guna2Button5.Visible = false;
                label3.Visible = true;
                label3.Text = await getCommentSharedToOthers() != "" ? await getCommentSharedToOthers() : "(No Comment)";
            }
            else {
                _getName = " " + _UploaderName;
                label4.Text = "Uploaded By";
                label3.Visible = true;
                label3.Text = await getCommentSharedToMe() != "" ? await getCommentSharedToMe() : "(No Comment)";
            }

            label2.Text = _getName;

            ToolTip saveTip = new ToolTip();
            saveTip.SetToolTip(this.guna2Button4, "Download Image");
        }

        private async Task<string> getCommentSharedToMe() {
            String returnComment = "";
            using(MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename",con)) {
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746"));
                using(MySqlDataReader readerComment = (MySqlDataReader) await command.ExecuteReaderAsync()) {
                    while(await readerComment.ReadAsync()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
        }

        private async Task<string> getCommentSharedToOthers() {
            String returnComment = "";
            using (MySqlCommand command = new MySqlCommand("SELECT CUST_COMMENT FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename", con)) {
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, "0123456789085746")); using (MySqlDataReader readerComment = command.ExecuteReader()) {
                    while (await readerComment.ReadAsync()) {
                        returnComment = EncryptionModel.Decrypt(readerComment.GetString(0));
                    }
                }
            }
            return returnComment;
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
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", Directoryname);
            }
            else if (TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", Directoryname);
            }
            else if (TableName == "file_info") {
                SaverModel.SaveSelectedFile(label1.Text, "file_info", Directoryname);
            } else if (TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", Directoryname,IsFromShared);
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
            string getExtension = label1.Text.Substring(label1.Text.Length - 4);
            shareFileFORM _showSharingFileFORM = new shareFileFORM(label1.Text,getExtension,IsFromSharing, TableName, Directoryname);
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
            string encryptVals = EncryptionModel.Encrypt(toBase64String,EncryptionKey.KeyValue);
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
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text, EncryptionKey.KeyValue));
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
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

            string query = "UPDATE cust_sharing SET CUST_COMMENT = @updatedComment WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
            using (var command = new MySqlCommand(query, con)) {
                command.Parameters.AddWithValue("@updatedComment", updatedComment);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(label1.Text));
                await command.ExecuteNonQueryAsync();
            }

        }

        private void guna2Button9_Click(object sender, EventArgs e) {
            guna2TextBox4.Enabled = true;
            guna2TextBox4.Visible = true;
            guna2Button9.Visible = false;
            guna2Button10.Visible = true;
            label3.Visible = false;
            guna2TextBox4.Text = label3.Text;
        }

        private async void guna2Button10_Click(object sender, EventArgs e) {
            if (label3.Text != guna2TextBox4.Text) {
                await saveChangesComment(guna2TextBox4.Text);
            }

            label3.Text = guna2TextBox4.Text != String.Empty ? guna2TextBox4.Text : label3.Text;
            guna2Button9.Visible = true;
            guna2Button10.Visible = false;
            guna2TextBox4.Visible = false;
            label3.Visible = true;
            label3.Refresh();
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
    }
}
