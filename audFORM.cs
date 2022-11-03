using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Media;

namespace FlowSERVER1 {
    public partial class audFORM : Form {
        public audFORM(String titleName) {
            InitializeComponent();
            //label3.Text = _path;
            label1.Text = titleName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

            try {

                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();

                List<Byte> audValues = new List<Byte>();

                String _selectAud = "SELECT CUST_FILE FROM file_info_audi WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = new MySqlCommand(_selectAud, con);
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);
                //MySqlDataAdapter ad = new MySqlDataAdapter(command);
                //DataSet ds = new DataSet();
                //ad.Fill(ds);
                
                MySqlDataReader _dr = command.ExecuteReader();

                while(_dr.Read()) {
                    audValues.Add(_dr.GetByte(0));
                }
                                                        
                _dr.Close();

                using(MemoryStream _ms = new MemoryStream(audValues[0])) {
                    SoundPlayer player = new SoundPlayer(_ms);
                    player.Play();
                }
                
                ///DataTable _DT = new DataTable();
                //_DT.Load(_dr); 
                //Byte[] audioSetup_ = (byte[])_DT.Rows[0][0];

                //_dr.Close();

        
                /*
                byte[] sound = ASCIIEncoding.Default.GetBytes(audioSetup_.ToString());
                using(MemoryStream _ms = new MemoryStream(sound)) {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(_ms);
                    player.Load();
                    player.Play();
                }
                */



                /*
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                
                sound.Stream.Position = 0;     // Manually rewind stream 
                player.Stream = null;    // Then we have to set stream to null 
                player.Stream = sound;  // And set it again, to force it to be loaded again... 
                player.Play();          // Yes! We can play the sound! 
                */


                //DataTable _DT = new DataTable();
                //_DT.Load(_dr); 
                //Byte[] audioSetup_ = (byte[])_DT.Rows[0][0];
                //File.WriteAllBytes("C:\\test.wma", audioSetup_);
                /*
                string name = Path.ChangeExtension(Path.GetRandomFileName(), ".mp3");
                string path = Path.Combine(Path.GetTempPath(), name);
                File.WriteAllBytes(path, audioSetup_);
                Process.Start(path);*/

            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }

            
            //_wmpVid.uiMode = "none";
            //_wmpVid.Visible = true;
            //_wmpVid.URL = label3.Text;
            //_wmpVid.Ctlcontrols.play();
            guna2Button6.Visible = true;
            guna2Button5.Visible = false;
        }

        private void guna2Button6_Click(object sender, EventArgs e) {
            _wmpVid.Ctlcontrols.pause();
            guna2Button6.Visible = false;
            guna2Button5.Visible = true;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
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
    }
}
