using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.AuthenticationQuery;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.SharingQuery;
using FlowstorageDesktop.Temporary;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class shareFileFORM : Form {

        readonly private SharingOptionsQuery sharingOptions = new SharingOptionsQuery();
        readonly private ShareFileQuery shareFile = new ShareFileQuery();
        readonly private UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();

        readonly private Crud crud = new Crud();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();

        private string _fileName { get; set; }
        private string _tableName {get; set;}
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }

        public shareFileFORM(String fileName, bool isFromShared, String tableName, String directoryName) {

            InitializeComponent();

            this._fileName = fileName;
            this._isFromShared = isFromShared;
            this._tableName = tableName;
            this._directoryName = directoryName;

            lblFileName.Text = fileName;

        }

        private async Task StartSharingFile() {

            string shareToName = txtFieldShareToName.Text;

            int receiverUploadLimit = await userAuthQuery.GetUploadLimit(shareToName);
            int receiverCurrentTotalUploaded = await shareFile.CountReceiverTotalShared(shareToName);

            string comment = txtFieldComment.Text;

            if (receiverUploadLimit != receiverCurrentTotalUploaded) {

                StartPopupForm.StartSharingPopup(shareToName);
                
                await shareFile.InitializeFileShare(
                    shareToName, comment, _fileName, _tableName, _directoryName, _isFromShared);

                ClosePopupForm.CloseSharingPopup();

                new SucessSharedAlert(txtFieldShareToName.Text).Show();

            }
        }

        private async void guna2Button2_Click(object sender, EventArgs e) {

            try {

                if (txtFieldShareToName.Text == tempDataUser.Username) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: "You can't share to yourself.").Show();
                    return;
                }

                if (txtFieldShareToName.Text == String.Empty) {
                    return;
                }

                if (await sharingOptions.UserExistVerification(txtFieldShareToName.Text) == 0) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} does not exist.").Show();
                    return;
                }

                if (await shareFile.FileIsUploadedVerification(txtFieldShareToName.Text, _fileName) > 0) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: "This file is already shared.").Show();
                    return;
                }

                if (!(await sharingOptions.RetrieveIsSharingDisabled(txtFieldShareToName.Text) == "0")) {
                    new CustomAlert(
                        title: "Sharing failed", subheader: $"The user {txtFieldShareToName.Text} disabled their file sharing.").Show();
                    return;
                }

                if (await sharingOptions.ReceiverHasAuthVerification(txtFieldShareToName.Text) != "") {
                    new AskSharingAuthForm(
                        txtFieldShareToName.Text, txtFieldComment.Text, _fileName).Show();
                    return;
                }

                await StartSharingFile();
                               
            } catch (Exception) {
                MessageBox.Show(
                    "An unknown error occurred.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {
            lblCountCharComment.Text = txtFieldComment.Text.Length.ToString() + "/295";
        }

        private void shareFileFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnShareToName_TextChanged(object sender, EventArgs e) {

        }
    }
}
