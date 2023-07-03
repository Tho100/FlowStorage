using Guna.UI2.WinForms;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.Helper {
    public class PanelGenerator {

        public void generatePanel(string parameterName, int length, List<(string,string, string)> filesInfo, List<EventHandler> onPressed, List<EventHandler> onPressedMoreButton, List<Image> pictureImage, bool isFromPs = false) {

            for(int i=0; i<length; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = $"{parameterName + i}",
                    Width = 200, 
                    Height = 222, 
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };
                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;
                HomePage.instance.flowLayoutPanel1.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = $"LabG{i}";
                dateLab.BackColor = GlobalStyle.TransparentColor;
                dateLab.Font = GlobalStyle.DateLabelFont;
                dateLab.ForeColor = GlobalStyle.DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Location = GlobalStyle.DateLabelLoc;
                dateLab.Text = filesInfo[i].Item2;

                Guna2CircleButton seperatorButton = new Guna2CircleButton();
                if(isFromPs) {
                    panelF.Controls.Add(seperatorButton);
                }
                seperatorButton.Location = GlobalStyle.PsSeperatorBut;
                seperatorButton.Size = GlobalStyle.PsSeperatorButSize;
                seperatorButton.FillColor = GlobalStyle.PsSeperatorColor;
                seperatorButton.BringToFront();

                Label psButtonTag = new Label();
                if(isFromPs) {
                    panelF.Controls.Add(psButtonTag);
                }
                psButtonTag.Name = $"ButTag{i}";
                psButtonTag.Font = GlobalStyle.PsLabelTagFont;
                psButtonTag.BackColor = GlobalStyle.TransparentColor; 
                psButtonTag.ForeColor = isFromPs == true ? GlobalStyle.psBackgroundColorTag[filesInfo[i].Item3] : GlobalStyle.TransparentColor;
                psButtonTag.Visible = isFromPs == true;
                psButtonTag.Location = GlobalStyle.PsLabelTagLoc;
                psButtonTag.Text = isFromPs == true ? filesInfo[i].Item3 : String.Empty;
                psButtonTag.BringToFront();

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}"; 
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.AutoEllipsis = true;
                titleLab.Width = 160; 
                titleLab.Height = 20; 
                titleLab.Text = filesInfo[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 8;
                picMain_Q.Width = 190; 
                picMain_Q.Height = 145; 
                picMain_Q.Visible = true;
                picMain_Q.Click += onPressed[i];

                picMain_Q.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panelF.Width - picMain_Q.Width) / 2;

                picMain_Q.Location = new Point(picMain_Q_x, 10);

                picMain_Q.MouseHover += (_senderM, _ev) => {
                    panelF.ShadowDecoration.Enabled = true;
                    panelF.ShadowDecoration.BorderRadius = 8;
                };

                picMain_Q.MouseLeave += (_senderQ, _evQ) => {
                    panelF.ShadowDecoration.Enabled = false;
                };

                Guna2Button remBut = new Guna2Button();
                panelF.Controls.Add(remBut);
                remBut.Name = "Rem" + i;
                remBut.Width = 29;
                remBut.Height = 26;
                remBut.ImageOffset = GlobalStyle.GarbageOffset;
                remBut.FillColor = GlobalStyle.TransparentColor;
                remBut.BorderRadius = 6;
                remBut.BorderThickness = 1;
                remBut.BorderColor = GlobalStyle.TransparentColor;
                remBut.Image = GlobalStyle.GarbageImage; 
                remBut.Visible = isFromPs == false;
                remBut.Location = GlobalStyle.GarbageButtonLoc; 
                remBut.Enabled = isFromPs == false;

                remBut.Click += onPressedMoreButton[i];

                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);
                img.Image = pictureImage[i];

            };
        }

    }
}
