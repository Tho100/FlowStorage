using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Media;
using NAudio.Wave;

namespace FlowSERVER1 {
    public partial class audFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public audFORM(String titleName) {
            InitializeComponent();
            label1.Text = titleName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
        }
        SoundPlayer _getSoundPlayer = null;
        Byte[] _mp3Byte = null;
        private void guna2Button5_Click(object sender, EventArgs e) {
            try {

                String _audType = label1.Text.Substring(label1.Text.Length-3);

                String _selectAud = "SELECT CUST_FILE FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(_selectAud, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);

                MySqlDataReader _AudReader = command.ExecuteReader();
                if(_AudReader.Read()) {
                    var _getByteAud = (byte[])_AudReader["CUST_FILE"];
                    if(_audType == "wav") {
                        using(MemoryStream _ms = new MemoryStream(_getByteAud)) {
                            SoundPlayer player = new SoundPlayer(_ms);
                            _getSoundPlayer = player;
                            player.Play();
                        }
                    } else if (_audType == "mp3") {
                        mp3ToWav(_getByteAud);
                    }
                }
                _AudReader.Close();
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to play this audio.","Flowstorage");
            }
            guna2Button6.Visible = true;
            guna2Button5.Visible = false;
        }

        private void mp3ToWav(Byte[] _mp3ByteIn) {
            using(var _retMs = new MemoryStream()) {
                using(var _ms = new MemoryStream(_mp3ByteIn)) {
                    using(Mp3FileReader _reader = new Mp3FileReader(_ms)) {
                        var _Rs = new RawSourceWaveStream(_reader,new WaveFormat(16000,1));
                        using(WaveStream PCMStream = WaveFormatConversionStream.CreatePcmStream(_Rs)) {
                            WaveFileWriter.WriteWavFileToStream(_retMs,PCMStream);
                            _mp3Byte = _retMs.ToArray();
                            if (_mp3Byte != null) {
                                using (MemoryStream _mp3Ms = new MemoryStream(_mp3Byte)) {
                                    SoundPlayer player = new SoundPlayer(_mp3Ms);
                                    _getSoundPlayer = player;
                                    player.Play();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
            if(_getSoundPlayer != null) {
                _getSoundPlayer.Stop();
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
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
    }
}
