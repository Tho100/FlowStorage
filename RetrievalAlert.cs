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
    public partial class RetrievalAlert : Form {
        public static RetrievalAlert instance;
        private static String OriginFrom = "";
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
                label8.Text = "Failed to cancel the operation.";
                //LoaderModel.stopFileRetrievalLoad = true;
            }
        }
    }
}
