using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FlowSERVER1.Helper {
    public class PanelGenerator {

        public void GeneratePanel(string parameterName, int length, List<(string,string, string)> filesInfo, List<EventHandler> onPressed, List<EventHandler> onPressedMoreButton, List<Image> pictureImage, bool isFromPs = false, bool moreButtonVisible = true, bool isFromDirectory = false) {

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

                if(!isFromDirectory) {
                    HomePage.instance.flwLayoutHome.Controls.Add(panelPic_Q);
                } else {
                    DirectoryForm.instance.flwLayoutDirectory.Controls.Add(panelPic_Q);
                }

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
                remBut.Visible = moreButtonVisible;
                remBut.BringToFront();
                remBut.Location = GlobalStyle.GarbageButtonLoc; 
                remBut.Enabled = moreButtonVisible;

                remBut.Click += onPressedMoreButton[i];

                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);
                img.Image = pictureImage[i];

            };
        }

        public void GeneratePublicStoragePanel(string parameterName, int length, List<(string, string, string, string)> filesInfo, List<EventHandler> onPressed, List<EventHandler> onPressedMoreButton, List<Image> pictureImage, List<String> usernameList, bool moreButtonVisible = true) {

            for (int i = 0; i < length; i++) {

                var panelPic_Q = new Guna2Panel() {
                    Name = $"{parameterName + i}",
                    Width = 250,
                    Height = 288,
                    BorderColor = GlobalStyle.BorderColor,
                    BorderThickness = 1,
                    BorderRadius = 8,
                    BackColor = GlobalStyle.TransparentColor,
                    Location = new Point(600, Globals.PANEL_GAP_TOP)
                };

                Globals.PANEL_GAP_TOP += Globals.PANEL_GAP_HEIGHT;

                HomePage.instance.flwLayoutHome.Controls.Add(panelPic_Q);

                var panelF = (Guna2Panel)panelPic_Q;

                Label dateLab = new Label();
                panelF.Controls.Add(dateLab);
                dateLab.Name = $"LabG{i}";
                dateLab.BackColor = GlobalStyle.TransparentColor;
                dateLab.Font = GlobalStyle.DateLabelFont;
                dateLab.ForeColor = GlobalStyle.DarkGrayColor;
                dateLab.Visible = true;
                dateLab.Enabled = true;
                dateLab.Width = 200;
                dateLab.Location = new Point(12, 261);
                dateLab.Text = $"{usernameList[i]} · {filesInfo[i].Item2}";

                Guna2Panel tagBackground = new Guna2Panel();
                panelF.Controls.Add(tagBackground);
                tagBackground.BorderRadius = 11;
                tagBackground.Location = new Point(12, 188);
                tagBackground.Size = new Size(108, 24);
                tagBackground.FillColor = GlobalStyle.psBackgroundColorTag[filesInfo[i].Item3];
                tagBackground.BringToFront();

                Label tagLabel = new Label();
                panelF.Controls.Add(tagLabel);
                tagLabel.Name = $"ButTag{i}";
                tagLabel.Font = GlobalStyle.PsLabelTagFont;
                tagLabel.Height = 15;
                tagLabel.Width = 85;
                tagLabel.BackColor = GlobalStyle.psBackgroundColorTag[filesInfo[i].Item3];
                tagLabel.ForeColor = GlobalStyle.GainsboroColor;
                tagLabel.Visible = true;
                tagLabel.TextAlign = ContentAlignment.MiddleCenter;

                int centerX = (tagBackground.Width - tagLabel.Width) / 2;
                tagLabel.Location = new Point(centerX+15, GlobalStyle.PsLabelTagLoc.Y+1);

                tagLabel.Text = filesInfo[i].Item3;
                tagLabel.BringToFront();

                Label titleLab = new Label();
                panelF.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}";
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Visible = true;
                titleLab.Enabled = true;
                titleLab.Location = new Point(12, 218);
                titleLab.AutoEllipsis = true;
                titleLab.Width = 200;
                titleLab.Height = 20;
                titleLab.Text = filesInfo[i].Item4;

                Label fileNameLabel = new Label();
                panelF.Controls.Add(fileNameLabel);
                fileNameLabel.Name = $"fileNameLabelL{i}";
                fileNameLabel.Font = GlobalStyle.PsTitleLabelFont;
                fileNameLabel.ForeColor = GlobalStyle.DarkGrayColor;
                fileNameLabel.Visible = true;
                fileNameLabel.Enabled = true;
                fileNameLabel.Location = new Point(12, 240);
                fileNameLabel.AutoEllipsis = true;
                fileNameLabel.Width = 200;
                fileNameLabel.Height = 20;
                fileNameLabel.Text = filesInfo[i].Item1;

                Guna2PictureBox picMain_Q = new Guna2PictureBox();
                panelF.Controls.Add(picMain_Q);
                picMain_Q.Name = "ImgG" + i;
                picMain_Q.SizeMode = PictureBoxSizeMode.CenterImage;
                picMain_Q.BorderRadius = 8;
                picMain_Q.Width = 238;
                picMain_Q.Height = 165;
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
                remBut.Visible = moreButtonVisible;
                remBut.BringToFront();
                remBut.Location = new Point(246, 228);
                remBut.Enabled = moreButtonVisible;

                remBut.Click += onPressedMoreButton[i];

                var img = ((Guna2PictureBox)panelF.Controls["ImgG" + i]);
                img.Image = pictureImage[i];

            };
        }
    }
}
