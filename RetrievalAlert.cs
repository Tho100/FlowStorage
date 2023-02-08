using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace FlowSERVER1 {
    public partial class RetrievalAlert : Form {
        public static RetrievalAlert instance;
        private static String OriginFrom = "";
        private static MySqlConnection con = ConnectionModel.con;
        public RetrievalAlert(String alertMessage,String _orignFrom) {
            InitializeComponent();
            label8.Text = alertMessage;
            OriginFrom = _orignFrom;
            instance = this;
            if(_orignFrom == "login") {
                guna2Button10.Visible = false;
            } else {
                guna2Button10.Visible = true;
            }
            
        }

        private void RetrievalAlert_Load(object sender, EventArgs e) {

        }

        private void guna2Button10_Click(object sender, EventArgs e) {
            label8.Text = "Cancelling operation...";
            if(OriginFrom == "Saver") {
                SaverModel.stopFileRetrieval = true;
            } else if (OriginFrom == "Loader") {
                if(label8.Text == "Flowstorage is retrieving your directory files.") {
                    label8.Text = "Failed to cancel the operation.";
                } else {

                    try {

                        Application.DoEvents();

                        label9.Text = "Cancelling Operation...";
                        if (con.State == System.Data.ConnectionState.Open) {

                            // @ Close connection before turning it back on for retrieval cancellation
                            con.Close();

                            if (con.State == System.Data.ConnectionState.Closed) {

                                // @ Turn connection back on 
                                con.Open();
                            }
                        }
                    } catch (Exception) {
                        con.Close();
                        if(con.State == System.Data.ConnectionState.Closed) {
                            con.Open();
                        }
                    }

                    Application.OpenForms
                               .OfType<Form>()
                               .Where(form => String.Equals(form.Name, "Form3"))
                               .ToList()
                               .ForEach(form => form.Close());

                    this.Close();
                }
            }
        }
    }
}
