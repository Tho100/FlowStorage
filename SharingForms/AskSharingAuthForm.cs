using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using FlowSERVER1.Sharing;
using FlowSERVER1.SharingQuery;
using FlowSERVER1.AuthenticationQuery;

namespace FlowSERVER1 {
    public partial class AskSharingAuthForm : Form {
        private string _receiverUsername { get; set; }
        private string _fileName { get; set; }
        private string _tableName { get; set; }
        private string _directoryName { get; set; }
        private bool _isFromShared { get; set; }
        private string _userComment { get; set; }

        readonly private Crud crud = new Crud();

        readonly private SharingOptionsQuery sharingOptionsQuery = new SharingOptionsQuery();
        readonly private ShareFileQuery shareFileQuery = new ShareFileQuery();
        readonly private UserAuthenticationQuery userAuthQuery = new UserAuthenticationQuery();

        public AskSharingAuthForm(string receiverUsername, string comment, string fileName) {
            InitializeComponent();
            this._receiverUsername = receiverUsername;
            this._fileName = fileName;
            this._userComment = comment;
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            guna2Button3.Visible = false;
            guna2Button1.Visible = true;
            txtFieldReceiverAuth.PasswordChar = '*';
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            guna2Button3.Visible = true;
            guna2Button1.Visible = false;
            txtFieldReceiverAuth.PasswordChar = '\0';
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.Close();
        }

        private async Task StartSharingFile() {

            int receiverUploadLimit = await userAuthQuery.GetUploadLimit(_receiverUsername);
            int receiverCurrentTotalUploaded = await shareFileQuery.CountReceiverTotalShared(_receiverUsername);

            if (receiverUploadLimit != receiverCurrentTotalUploaded) {

                new Thread(() => new SharingAlert(shareToName: _receiverUsername).ShowDialog()).Start();

                await shareFileQuery.InitializeFileShare(
                    _receiverUsername, _userComment, _fileName, _tableName, _directoryName, _isFromShared);

                ClosePopupForm.CloseSharingPopup();

                new SucessSharedAlert(_receiverUsername).Show();

            }

        }

        /// <summary>
        /// This button will start sharing file if 
        /// the entered password is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void guna2Button2_Click(object sender, EventArgs e) {

            var receiverAuthInput = txtFieldReceiverAuth.Text;
            var actualReceiverAuth = EncryptionModel.computeAuthCase(await sharingOptionsQuery.GetReceiverAuth(_receiverUsername));
            
            if(EncryptionModel.computeAuthCase(receiverAuthInput) == actualReceiverAuth) {
                await StartSharingFile();

            } else {
                lblAlert.Visible = true;

            }

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}
