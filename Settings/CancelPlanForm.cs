using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Model;
using FlowstorageDesktop.Temporary;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowstorageDesktop.Settings {
    public partial class CancelPlanForm : Form {

        private readonly MySqlConnection con = ConnectionModel.con;
        private readonly TemporaryDataUser tempDataUser = new TemporaryDataUser();

        public CancelPlanForm() {
            InitializeComponent();
            lblCurrentAccountPlan.Text = tempDataUser.AccountType;
        }

        private async Task CancelUserSubscriptionPlan() {

            try {

                StripeModel.CancelCustomerSubscription(tempDataUser.Email);
                StripeModel.DeleteCustomer(tempDataUser.Email);

                const string updateUserAccountQuery = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(updateUserAccountQuery, con)) {
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@email", tempDataUser.Email);
                    command.Parameters.AddWithValue("@type", "Basic");
                    await command.ExecuteNonQueryAsync();
                }

                const string insertBuyerQuery = "DELETE FROM cust_buyer WHERE CUST_USERNAME = @username";
                using (MySqlCommand commandSecond = new MySqlCommand(insertBuyerQuery, con)) {
                    commandSecond.Parameters.AddWithValue("@username", tempDataUser.Username);
                    await commandSecond.ExecuteNonQueryAsync();
                }

                tempDataUser.AccountType = "Basic";

                SettingsForm.instance.lblAccountType.Text = "Basic";
                SettingsForm.instance.InitiailizeUIOnAccountType("Basic");
                SettingsForm.instance.pnlCancelPlan.Visible = false;

                new CustomAlert(title: "Subscription plan cancelled successfully", subheader: $"You downgraded your account from {tempDataUser.AccountType} to Basic and you'll no longer be charged.").Show();

                this.Close();

            } catch (Exception) {
                new CustomAlert(title: "Failed to cancel subscription", subheader: "Something went wrong while trying to cancel your subscription plan please try again later.");
            }
        }

        private async void guna2Button1_Click(object sender, EventArgs e) {
            await CancelUserSubscriptionPlan();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
