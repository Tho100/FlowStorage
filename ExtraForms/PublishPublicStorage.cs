using FlowSERVER1.AlertForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlowSERVER1.ExtraForms {
    public partial class PublishPublicStorage : Form {
        public PublishPublicStorage(string fileName) {
            InitializeComponent();
            txtFieldFileName.Text = fileName;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {

            HomePage.instance.PublicStorageUserComment = null;
            HomePage.instance.PublicStorageUserTag = null;
            HomePage.instance.PublicStorageUserTitle = null;
            HomePage.instance.PublicStorageClosed = true;
            this.Hide();

            return;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {

            HomePage.instance.PublicStorageUserComment = txtFieldComment.Text;
            HomePage.instance.PublicStorageUserTitle =  string.IsNullOrEmpty(txtFieldTitle.Text) 
                ? "Untitled" : txtFieldTitle.Text;

            if(HomePage.instance.PublicStorageUserTag == null) {
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
            HomePage.instance.PublicStorageUserTag = "Entertainment";
            btnEnter.FillColor = Color.Orange;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void btnGaming_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Gaming";
            btnGaming.FillColor = Color.SteelBlue;
            btnRandom.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void btnSoftware_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Software";
            btnSoftware.FillColor = Color.MediumSeaGreen;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void btnPolitics_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Education";
            btnEducation.FillColor = Color.Firebrick;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void btnRandom_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Random";
            btnRandom.FillColor = Color.DimGray;
            btnGaming.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void guna2Button2_Click_1(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Music";
            btnMusic.FillColor = Color.Tomato;
            btnRandom.FillColor = Color.DarkGray;
            btnGaming.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnData.FillColor = Color.DarkGray;
            btnCreativity.FillColor = Color.DarkGray;
        }

        private void PublishPublicStorage_Load(object sender, EventArgs e) {

        }

        private void btnData_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Data";
            btnData.FillColor = Color.DarkTurquoise;
            btnCreativity.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
        }

        private void btnCreativity_Click(object sender, EventArgs e) {
            HomePage.instance.PublicStorageUserTag = "Creativity";
            btnCreativity.FillColor = Color.BlueViolet;
            btnData.FillColor = Color.DarkGray;
            btnEducation.FillColor = Color.DarkGray;
            btnGaming.FillColor = Color.DarkGray;
            btnRandom.FillColor = Color.DarkGray;
            btnSoftware.FillColor = Color.DarkGray;
            btnEnter.FillColor = Color.DarkGray;
            btnMusic.FillColor = Color.DarkGray;
        }

        private void txtFieldFileName_TextChanged(object sender, EventArgs e) {

        }
    }
}
