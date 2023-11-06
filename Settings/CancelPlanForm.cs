using FlowSERVER1.AlertForms;
using FlowSERVER1.Model;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.Settings {
    public partial class CancelPlanForm : Form {

        private readonly MySqlConnection con = ConnectionModel.con;

        public CancelPlanForm() {
            InitializeComponent();
            lblCurrentAccountPlan.Text = Globals.accountType;
        }

        private async Task CancelUserSubscriptionPlan() {

            try {

                StripeModel.CancelCustomerSubscription(Globals.custEmail);
                StripeModel.DeleteCustomer(Globals.custEmail);

                const string updateUserAccountQuery = "UPDATE cust_type SET ACC_TYPE = @type WHERE CUST_EMAIL = @email AND CUST_USERNAME = @username";
                using (MySqlCommand command = new MySqlCommand(updateUserAccountQuery, con)) {
                    command.Parameters.AddWithValue("@username", Globals.custUsername);
                    command.Parameters.AddWithValue("@email", Globals.custEmail);
                    command.Parameters.AddWithValue("@type", "Basic");
                    await command.ExecuteNonQueryAsync();
                }

                const string insertBuyerQuery = "DELETE FROM cust_buyer WHERE CUST_USERNAME = @username";
                using (MySqlCommand commandSecond = new MySqlCommand(insertBuyerQuery, con)) {
                    commandSecond.Parameters.AddWithValue("@username", Globals.custUsername);
                    await commandSecond.ExecuteNonQueryAsync();
                }

                Globals.accountType = "Basic";

                SettingsForm.instance.lblAccountType.Text = "Basic";
                SettingsForm.instance.InitiailizeUIOnAccountType("Basic");
                SettingsForm.instance.pnlCancelPlan.Visible = false;

                new CustomAlert(title: "Subscription plan cancelled successfully", subheader: $"You downgraded your account from {Globals.accountType} to Basic and you'll no longer be charged.").Show();

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
