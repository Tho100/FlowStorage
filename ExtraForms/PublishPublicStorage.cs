using FlowSERVER1.AlertForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.ExtraForms {
    public partial class PublishPublicStorage : Form {
        public PublishPublicStorage(string fileName) {
            InitializeComponent();
            txtFieldFileName.Text = fileName;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            HomePage.instance.publicStorageUserComment = null;
            HomePage.instance.publicStorageUserTag = null;
            HomePage.instance.publicStorageClosed = true;
            this.Hide();

            return;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            HomePage.instance.publicStorageUserComment = txtFieldComment.Text;
            if(HomePage.instance.publicStorageUserTag == null) {
                new CustomAlert(title: "Upload Failed","Please select a tag.").Show();
                return;
            }


            this.Close();
        }

        private void txtFieldComment_TextChanged(object sender, EventArgs e) {
            lblCountCharComment.Text = $"{txtFieldComment.TextLength}/295";
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e) {

        }

        private void btnEnter_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Entertainment";
            btnEnter.FillColor = Color.Orange;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnPolitics.FillColor = Color.DarkGray;
        }

        private void btnGaming_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Gaming";
            btnGaming.FillColor = Color.SteelBlue;
            btnRandom.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnPolitics.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
        }

        private void btnSoftware_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Software";
            btnSoftware.FillColor = Color.MediumSeaGreen;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnPolitics.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
        }

        private void btnPolitics_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Politics";
            btnPolitics.FillColor = Color.Firebrick;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
        }

        private void btnRandom_Click(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Random";
            btnRandom.FillColor = Color.DimGray;
            btnGaming.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnPolitics.FillColor = Color.DarkGray;
        }

        private void guna2Button2_Click_1(object sender, EventArgs e) {
            HomePage.instance.publicStorageUserTag = "Music";
            btnMusic.FillColor = Color.Tomato;
            btnRandom.FillColor = Color.DarkGray;
            btnGaming.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnPolitics.FillColor = Color.DarkGray;
        }
    }
}
