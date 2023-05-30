using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public partial class PaymentSuceededAlert : Form {
        public static PaymentSuceededAlert instance;
        public PaymentSuceededAlert(String _accType) {
            InitializeComponent();
            instance = this;
            label1.Text = _accType;
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void successPay_Load(object sender, EventArgs e) {

        }
    }
}
