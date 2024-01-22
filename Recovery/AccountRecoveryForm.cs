using FlowstorageDesktop.AuthenticationQuery;
using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class ValidateRecoveryEmail : Form {

        public ValidateRecoveryEmail() {
            InitializeComponent();
        }

        private async void btnVerify_Click(object sender, EventArgs e) {

            try {

                string emailInput = txtFieldEmail.Text;
                string recoveryInput = txtFieldRecoveryKey.Text;

                var userAuthQuery = new UserAuthenticationQuery();

                if (await userAuthQuery.GetEmailByEmail(emailInput) == string.Empty) {
                    lblAlert.Text = "Account not found.";
                    lblAlert.Visible = true;
                    return;
                }

                if(await userAuthQuery.GetRecoveryKeyByEmail(emailInput) != recoveryInput) {
                    lblAlert.Visible = true;
                    lblAlert.Text = "Invalid recovery key.";
                    return;
                }

                new ChangeAuthForm().Show();

                this.Close();


            } catch (Exception) {
                lblAlert.Visible = true;

            }
        }

        private void guna2Button1_Click(object sender, EventArgs e) => this.Close();

    }
}
