using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FlowstorageDesktop.Helper {
    public class PanelGenerator {

        public void GeneratePanel(string parameterName, int length, List<(string,string, string)> filesInfo, List<EventHandler> onPressed, List<EventHandler> onPressedMoreButton, List<Image> pictureImage, bool moreButtonVisible = true, bool isFromDirectory = false) {

            for(int i=0; i<length; i++) {

                var panel = new Guna2Panel() {
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
                    HomePage.instance.flwLayoutHome.Controls.Add(panel);
                } else {
                    DirectoryForm.instance.flwLayoutDirectory.Controls.Add(panel);
                }

                Label dateLabel = new Label();
                panel.Controls.Add(dateLabel);
                dateLabel.Name = $"LabG{i}";
                dateLabel.BackColor = GlobalStyle.TransparentColor;
                dateLabel.Font = GlobalStyle.DateLabelFont;
                dateLabel.ForeColor = GlobalStyle.DarkGrayColor;
                dateLabel.Location = GlobalStyle.DateLabelLoc;
                dateLabel.Text = filesInfo[i].Item2;

                Label titleLab = new Label();
                panel.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}"; 
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Location = GlobalStyle.TitleLabelLoc;
                titleLab.AutoEllipsis = true;
                titleLab.Width = 160; 
                titleLab.Height = 20; 
                titleLab.Text = filesInfo[i].Item1;

                Guna2PictureBox panelImage = new Guna2PictureBox();
                panel.Controls.Add(panelImage);
                panelImage.Name = "ImgG" + i;
                panelImage.SizeMode = PictureBoxSizeMode.CenterImage;
                panelImage.BorderRadius = 8;
                panelImage.Width = 190; 
                panelImage.Height = 145; 
                panelImage.Click += onPressed[i];

                panelImage.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panel.Width - panelImage.Width) / 2;

                panelImage.Location = new Point(picMain_Q_x, 10);

                panelImage.MouseHover += (_senderM, _ev) => {
                    panel.ShadowDecoration.Enabled = true;
                    panel.ShadowDecoration.BorderRadius = 8;
                };

                panelImage.MouseLeave += (_senderQ, _evQ) => {
                    panel.ShadowDecoration.Enabled = false;
                };

                Guna2Button moreOptionsButton = new Guna2Button();
                panel.Controls.Add(moreOptionsButton);
                moreOptionsButton.Name = "Rem" + i;
                moreOptionsButton.Width = 29;
                moreOptionsButton.Height = 26;
                moreOptionsButton.ImageOffset = GlobalStyle.GarbageOffset;
                moreOptionsButton.FillColor = GlobalStyle.TransparentColor;
                moreOptionsButton.BorderRadius = 6;
                moreOptionsButton.BorderThickness = 1;
                moreOptionsButton.BorderColor = GlobalStyle.TransparentColor;
                moreOptionsButton.Image = GlobalStyle.GarbageImage; 
                moreOptionsButton.Location = GlobalStyle.GarbageButtonLoc; 
                moreOptionsButton.Visible = moreButtonVisible;

                moreOptionsButton.Click += onPressedMoreButton[i];

                var img = ((Guna2PictureBox)panel.Controls["ImgG" + i]);
                img.Image = pictureImage[i];

            };
        }

        public void GeneratePublicStoragePanel(string parameterName, int length, List<(string, string, string, string)> filesInfo, List<EventHandler> onPressed, List<EventHandler> onPressedMoreButton, List<Image> pictureImage, List<string> usernameList, bool moreButtonVisible = true) {

            for (int i = 0; i < length; i++) {

                var panel = new Guna2Panel() {
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

                HomePage.instance.flwLayoutHome.Controls.Add(panel);

                Label dateLabel = new Label();
                panel.Controls.Add(dateLabel);
                dateLabel.Name = $"LabG{i}";
                dateLabel.BackColor = GlobalStyle.TransparentColor;
                dateLabel.Font = GlobalStyle.DateLabelFont;
                dateLabel.ForeColor = GlobalStyle.DarkGrayColor;
                dateLabel.Width = 200;
                dateLabel.Location = new Point(12, 261);
                dateLabel.Text = $"{usernameList[i]} · {filesInfo[i].Item2}";

                Guna2Panel tagBackground = new Guna2Panel();
                panel.Controls.Add(tagBackground);
                tagBackground.BorderRadius = 11;
                tagBackground.Location = new Point(12, 188);
                tagBackground.Size = new Size(108, 24);
                tagBackground.FillColor = GlobalStyle.psBackgroundColorTag[filesInfo[i].Item3];
                tagBackground.BringToFront();

                Label tagLabel = new Label();
                panel.Controls.Add(tagLabel);
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
                panel.Controls.Add(titleLab);
                titleLab.Name = $"titleImgL{i}";
                titleLab.Font = GlobalStyle.TitleLabelFont;
                titleLab.ForeColor = GlobalStyle.GainsboroColor;
                titleLab.Location = new Point(12, 218);
                titleLab.AutoEllipsis = true;
                titleLab.Width = 200;
                titleLab.Height = 20;
                titleLab.Text = filesInfo[i].Item4;

                Label fileNameLabel = new Label();
                panel.Controls.Add(fileNameLabel);
                fileNameLabel.Name = $"fileNameLabelL{i}";
                fileNameLabel.Font = GlobalStyle.PsTitleLabelFont;
                fileNameLabel.ForeColor = GlobalStyle.DarkGrayColor;
                fileNameLabel.Location = new Point(12, 240);
                fileNameLabel.AutoEllipsis = true;
                fileNameLabel.Width = 200;
                fileNameLabel.Height = 20;
                fileNameLabel.Text = filesInfo[i].Item1;

                Guna2PictureBox panelImage = new Guna2PictureBox();
                panel.Controls.Add(panelImage);
                panelImage.Name = "ImgG" + i;
                panelImage.SizeMode = PictureBoxSizeMode.CenterImage;
                panelImage.BorderRadius = 8;
                panelImage.Width = 238;
                panelImage.Height = 165;
                panelImage.Click += onPressed[i];

                panelImage.Anchor = AnchorStyles.None;

                int picMain_Q_x = (panel.Width - panelImage.Width) / 2;

                panelImage.Location = new Point(picMain_Q_x, 10);

                panelImage.MouseHover += (_senderM, _ev) => {
                    panel.ShadowDecoration.Enabled = true;
                    panel.ShadowDecoration.BorderRadius = 8;
                };

                panelImage.MouseLeave += (_senderQ, _evQ) => {
                    panel.ShadowDecoration.Enabled = false;
                };

                Guna2Button moreOptionsButton = new Guna2Button();
                panel.Controls.Add(moreOptionsButton);
                moreOptionsButton.Name = "Rem" + i;
                moreOptionsButton.Width = 29;
                moreOptionsButton.Height = 26;
                moreOptionsButton.ImageOffset = GlobalStyle.GarbageOffset;
                moreOptionsButton.FillColor = GlobalStyle.TransparentColor;
                moreOptionsButton.BorderRadius = 6;
                moreOptionsButton.BorderThickness = 1;
                moreOptionsButton.BorderColor = GlobalStyle.TransparentColor;
                moreOptionsButton.Image = GlobalStyle.GarbageImage;
                moreOptionsButton.Location = new Point(246, 228);
                moreOptionsButton.Visible = moreButtonVisible;

                moreOptionsButton.Click += onPressedMoreButton[i];

                var img = ((Guna2PictureBox)panel.Controls["ImgG" + i]);
                img.Image = pictureImage[i];

            };
        }
    }
}
