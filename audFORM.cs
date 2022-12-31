﻿using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Media;
using NAudio.Wave;
using System.Linq;
using System.Drawing;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace FlowSERVER1 {
    public partial class audFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _TabName = "";
        private Random _setRand;
        public audFORM(String titleName,String _TableName) {
            InitializeComponent();
            label1.Text = titleName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
            _TabName = _TableName;
            _setRand = new Random();
            timer1.Stop();
            timer2.Stop();

            guna2TrackBar1.ValueChanged += new EventHandler(this.guna2TrackBar1_ValueChanged);

        }

        SoundPlayer _getSoundPlayer = null;
        WaveOut _mp3WaveOut = null;

        private void guna2Button5_Click(object sender, EventArgs e) {
            try {
                
                timer1.Start();
                timer2.Start();
                String _audType = label1.Text.Substring(label1.Text.Length-3);

                Task.Run(() => {
                    String _selectAud = "SELECT CUST_FILE FROM " + _TabName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = new MySqlCommand(_selectAud, con);
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", label1.Text);

                    MySqlDataReader _AudReader = command.ExecuteReader();
                    if(_AudReader.Read()) {
                        var _getByteAud = (byte[])_AudReader["CUST_FILE"];
                        if (_audType == "wav") {
                            using (MemoryStream _ms = new MemoryStream(_getByteAud)) {
                                SoundPlayer player = new SoundPlayer(_ms);
                                _getSoundPlayer = player;
                                player.Play();
                            }
                        } else if (_audType == "mp3") {
                            mp3ToWav(_getByteAud);
                        }
                    }
                    _AudReader.Close();
                });
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to play this audio.","Flowstorage");
            }
            guna2Button6.Visible = true;
            guna2Button5.Visible = false;
        }

        private void mp3ToWav(Byte[] _mp3ByteIn) {
            Stream _setupStream = new MemoryStream(_mp3ByteIn);
            var _NReader = new NAudio.Wave.Mp3FileReader(_setupStream);
            var _setupWaveOut = new WaveOut();
            _setupWaveOut.Init(_NReader);
            _setupWaveOut.Play();
            _mp3WaveOut = _setupWaveOut;
        }


        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
                timer1.Stop();
                timer2.Stop();
            }
            if (_mp3WaveOut != null) {
                _mp3WaveOut.Pause();
                timer1.Stop();
                timer2.Stop();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
                timer1.Stop();
                timer2.Stop();
            }
            if (_mp3WaveOut != null) {
                _mp3WaveOut.Pause();
                timer1.Stop();
                timer2.Stop();
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
            label1.AutoSize = false;
            label2.AutoSize = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
            label2.AutoSize = true;
        }

        private void audFORM_Load(object sender, EventArgs e) {
                
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {

                String _selectAud = "SELECT CUST_FILE FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(_selectAud, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);

                MySqlDataReader _AudReader = command.ExecuteReader();
                if (_AudReader.Read()) {
                    var _getByteAud = (byte[])_AudReader["CUST_FILE"];
                    SaveFileDialog _dialog = new SaveFileDialog();
                    _dialog.Filter = "Audio Files|*.mp3;*.wav";
                    _dialog.FileName = label1.Text;
                    if(_dialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_dialog.FileName,_getByteAud);
                    }
                }
                _AudReader.Close();
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to save this audio.", "Flowstorage");
            }
        }

        private void zedGraphControl1_Load(object sender, EventArgs e) {

        }

        private void waveViewer1_Load(object sender, EventArgs e) {

        }

        private void chart1_Click(object sender, EventArgs e) {

        }
        bool isUp = false;
        int pic1YUpdate = 72;
        private async void timer1_Tick(object sender, EventArgs e) {
            // 267, 80: wave2
            // 46, 69: wave1
            int pic1X = guna2PictureBox1.Location.X;
            int pic1Y = guna2PictureBox1.Location.Y;

            for(int i=0; i<=500; i++) {
                if(pic1YUpdate == 72) {
                    guna2PictureBox1.Location = new Point(pic1X, pic1YUpdate);
                    pic1YUpdate = pic1Y;
                } else if (pic1YUpdate == pic1Y) {
                    guna2PictureBox1.Location = new Point(pic1X, pic1Y);
                    pic1YUpdate = 72;
                }
              await Task.Delay(80);
            }
            /*
            if(pic1YUpdate == pic1Y) {
                guna2PictureBox1.Location = new Point(pic1X, pic1Y);
                pic1YUpdate = 15;
            }*/


            //int yUp = _setRand.Next(pic1X, 110);
            //int yDown = _setRand.Next(pic1X, pic1Y);

        }

        private void guna2Button7_Click(object sender, EventArgs e) {

        }

        private void guna2Button7_Click_1(object sender, EventArgs e) {

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e) {

        }

        public void guna2TrackBar1_ValueChanged(object sender, EventArgs e) {
            label4.Text = guna2TrackBar1.Value.ToString() + "%";
        }
        int pic2YUpdate = 58;
        private async void timer2_Tick(object sender, EventArgs e) {
            int pic2X = guna2PictureBox3.Location.X;
            int pic2Y = guna2PictureBox3.Location.Y;

            for (int i = 0; i <= 500; i++) {
                if (pic2YUpdate == 58) {
                    guna2PictureBox3.Location = new Point(pic2X, pic2YUpdate);
                    pic2YUpdate = pic2Y;
                }
                else if (pic2YUpdate == pic2Y) {
                    guna2PictureBox3.Location = new Point(pic2X, pic2Y);
                    pic2YUpdate = 58;
                }
                await Task.Delay(80);
            }
        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "bgBlurForm"))
              .ToList()
              .ForEach(form => form.Hide());
        }
    }
}
