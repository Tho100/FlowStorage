using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowSERVER1.Settings {
    public partial class FeedbackForm : Form {

        readonly private MySqlConnection con = ConnectionModel.con;
        readonly private Crud crud = new Crud();

        public FeedbackForm() {
            InitializeComponent();
        }

        private async Task sendFeedback(string inputFeedback) {

            try {

                string currentDate = DateTime.Now.ToString("dd/mm/yyyy");

                const string query = "INSERT INTO feedback_info VALUES (@username,@feedback,@date)";
                var param = new Dictionary<string, string> 
                {
                    { "@username", Globals.custUsername },
                    { "@feedback", inputFeedback },
                    { "@date", currentDate },
                };


                await crud.Insert(query, param);

                var alertForm = new AlertForms.CustomAlert(title: "Feedback sent", subheader: $"Thank you {Globals.custUsername} for your feedback! We really appreciate it.");
                alertForm.Show();

            } catch (Exception) {
                new AlertForms.CustomAlert(
                    title: "Failed to send feedback",
                    subheader: "Check your internet connection and try again."
                ).Show();
            }
        }

        private async void guna2Button1_Click(object sender, EventArgs e) {
            await sendFeedback(guna2TextBox4.Text);
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e) {

        }
    }
}
