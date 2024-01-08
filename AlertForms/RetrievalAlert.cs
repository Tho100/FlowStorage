using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    public partial class RetrievalAlert : Form {

        public static RetrievalAlert instance;

        readonly private MySqlConnection con = ConnectionModel.con;
        private string originFrom { get; set; }

        public RetrievalAlert(string alertMessage, bool isFromLogin) {

            InitializeComponent();

            this.lblMessage.Text = alertMessage;

            instance = this;

            if(isFromLogin) {
                btnCancelRetrieval.Visible = false;

            } else {
                btnCancelRetrieval.Visible = true;

            }
            
        }

        private void RetrievalAlert_Load(object sender, EventArgs e) {

        }

        private void btnCancelRetrieval_Click(object sender, EventArgs e) {

            try {

                lblMessage.Text = "Cancelling operation...";

                if(originFrom == "Saver") {
                    SaverModel.stopFileRetrieval = true;

                } else if (originFrom == "Loader") {
                    if(lblMessage.Text == "Flowstorage is retrieving your directory files." || lblMessage.Text == "Flowstorage is retrieving your folder files." || lblMessage.Text == "Flowstorage is retrieving your shared files...") {
                        lblMessage.Text = "Failed to cancel the operation.";

                    } else {

                        try {

                            lblHeader.Text = "Cancelling Operation...";
                            if (con.State == System.Data.ConnectionState.Open) {

                                // @ Close connection before turning it back on for retrieval cancellation
                                con.Close();

                                if (con.State == System.Data.ConnectionState.Closed) {

                                    // @ Turn connection back on so that the user can 
                                    // reuse the application properly
                                    con.Open();
                                }
                            }

                        } catch (Exception) {
                            con.Close();

                            if(con.State == System.Data.ConnectionState.Closed) {
                                con.Open();

                            }

                            this.Close();

                        }

                        this.Close();

                    }
                } 

            } catch (Exception) {
                this.Close();
            }
        }
    }
}
