using System;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class PaymentSuceededAlert : Form {
        public static PaymentSuceededAlert instance;
        public PaymentSuceededAlert(String accountType) {
            InitializeComponent();
            instance = this;
            this.lblAccountType.Text = accountType;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

        private void successPay_Load(object sender, EventArgs e) {

        }
    }
}
