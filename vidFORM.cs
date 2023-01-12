using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using LibVLCSharp.Shared.MediaPlayerElement;
using LibVLCSharp.WinForms;
using LibVLCSharp.Shared;

namespace FlowSERVER1 {
    public partial class vidFORM : Form {
        public static vidFORM instance;
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public vidFORM(Image getThumb, int width, int height, String getTitle,String path) {
            InitializeComponent();
            instance = this;
            var setupImage = resizeImage(getThumb, new Size(width,height));
            guna2PictureBox1.Image = setupImage;
            label1.Text = getTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            label3.Text = path;
        }

        public static Image resizeImage(Image userImg, Size size) {
            return (Image)(new Bitmap(userImg,size));
        }

        private void vidFORM_Load(object sender, EventArgs e) {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
        }

        // Play
        private void guna2Button5_Click(object sender, EventArgs e) {
            try {
                /*
                _wmpVid.uiMode = "none";
                _wmpVid.Visible = true;
                _wmpVid.URL = label3.Text;
                _wmpVid.Ctlcontrols.play();*/
                //vlcControl1.Visible = true;
            
                String Select_VidByte = "SELECT CUST_FILE FROM file_info_vid WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(Select_VidByte, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);

                List<Byte> _ByteValues = new List<Byte>();
                MySqlDataReader _ReadBytes = command.ExecuteReader();
                if(_ReadBytes.Read()) {
                    var _retrieveBytesValue = (byte[])_ReadBytes["CUST_FILE"];
                    Stream _toStream = new MemoryStream(_retrieveBytesValue);
                    //vlcControl1.Play(new (_toStream));
                    var libvlc = new LibVLC("--input-repeat=2");
                    var media = new Media(libvlc,new StreamMediaInput(_toStream));
                    var mediaPlayer = new MediaPlayer(media);
                    mediaPlayer.Play();
                   // vlcControl1.Play(mediaPlayer);
                    //videoView1.MediaPlayer.Play(media);
                    //mediaPlayer.Play();
                    //videoView1.MediaPlayer.Play(mediaPlayer);//videoView1.MediaPlayer.Play()
                }
               
                guna2Button6.Visible = true;
                guna2Button5.Visible = false;
            } catch (Exception eq) {
                MessageBox.Show(eq.Message, "Flowstorage");
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
           // _wmpVid.Ctlcontrols.pause();
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e) {

        }

        private void pictureBox1_Click_2(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                String Select_VidByte = "SELECT CUST_FILE FROM file_info_vid WHERE CUST_USERNAME = @username";
                command = new MySqlCommand(Select_VidByte,con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                
                List<Byte> _ByteValues = new List<Byte>();
                MySqlDataReader _ReadBytes = command.ExecuteReader();
                if(_ReadBytes.Read()) {
                    var _retrieveBytesValue = (byte[])_ReadBytes["CUST_FILE"];
                    SaveFileDialog _fileDialog = new SaveFileDialog();
                    _fileDialog.FileName = label1.Text;
                    if(_fileDialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_fileDialog.FileName,_retrieveBytesValue);
                    }
                }

            } catch (Exception) {
                MessageBox.Show("Failed to download this video.","Flowstorage");
            }
        }

        private void videoView1_Click(object sender, EventArgs e) {

        }

        private void vlcControl1_Click(object sender, EventArgs e) {

        }
    }
}
