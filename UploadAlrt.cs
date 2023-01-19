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
    public partial class UploadAlrt : Form {
        public static UploadAlrt instance;
        public UploadAlrt(String _fileName) {
            InitializeComponent();
            instance = this;
            label1.Text = _fileName;
        }

        private void UploadAlrt_Load(object sender, EventArgs e) {

        }
    }
}
