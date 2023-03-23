using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using Guna.UI2.WinForms;
using System.IO;
using System.Text.RegularExpressions;

namespace FlowSERVER1 {

    /// <summary>
    /// Load user text file 
    /// </summary>

    public partial class txtFORM : Form {
        public static txtFORM instance;
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;

        /// <summary>
        /// 
        /// Retrieve text data based on table name 
        /// 
        /// </summary>
        /// <param name="getText"></param>
        /// <param name="tableName"></param>
        /// <param name="fileName"></param>
        /// <param name="_directory"></param>
        /// <param name="_UploaderUsername"></param>

        public txtFORM(String getText,String tableName,String fileName,String _directory,String _UploaderUsername) {
            InitializeComponent();

            try {

                String _getName = "";
                bool _isShared = Regex.Match(_UploaderUsername, @"^([\w\-]+)").Value == "Shared";

                if (_isShared == true) {
                    _getName = _UploaderUsername;
                }
                else {
                    _getName = "Uploaded By " + _UploaderUsername;
                }

                instance = this;
                label1.Text = fileName;
                label2.Text = _getName;
                var FileExt_ = label1.Text.Substring(label1.Text.LastIndexOf('.')).TrimStart();

                if (tableName == "upload_info_directory" & getText == "") {

                    string getTxtQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", label1.Text);
                        command.Parameters.AddWithValue("@dirname", Form3.instance.label1.Text);

                        string textValuesF = "";
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                textValuesF = reader.GetString(0);
                            }
                        }

                        string decryptedTextValues = EncryptionModel.DecryptText(textValuesF);
                        richTextBox1.Text = decryptedTextValues;
                    }

                    if (FileExt_ == ".py") {
                        pythonSyntax();
                    }
                    if (FileExt_ == ".html") {
                        htmlSyntax();
                    }
                    if (FileExt_ == ".css") {
                        cssSyntax();
                    }

                } else if (getText == "" && tableName == "folder_upload_info") {

                    string getTxtQuery = "SELECT CONVERT(CUST_FILE USING utf8) FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@foldername", Form1.instance.listBox1.GetItemText(Form1.instance.listBox1.SelectedItem));
                        command.Parameters.AddWithValue("@filename", fileName);

                        string textValuesF = "";
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                textValuesF = reader.GetString(0);
                            }
                        }

                        string decryptedTextValues = EncryptionModel.DecryptText(textValuesF);
                        richTextBox1.Text = decryptedTextValues;
                    }


                    if (FileExt_ == ".py") {
                        pythonSyntax();
                    }
                    if (FileExt_ == ".html") {
                        htmlSyntax();
                    }
                    if (FileExt_ == ".css") {
                        cssSyntax();
                    }
                    if (FileExt_ == ".js") {
                        jsSyntax();
                    }
                } else if (tableName == "file_info_expand") {

                    string getTxtQuery = "SELECT CUST_FILE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", label1.Text);

                        string textValuesF = "";
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                textValuesF = reader.GetString(0);
                            }
                        }

                        string decryptedTextValues = EncryptionModel.DecryptText(textValuesF);
                        richTextBox1.Text = decryptedTextValues;
                    }


                    if (FileExt_ == ".py") {
                        pythonSyntax();
                    }
                    if (FileExt_ == ".html") {
                        htmlSyntax();
                    }
                    if(FileExt_ == ".css") {
                        cssSyntax();
                    }
                    if(FileExt_ == ".js") {
                        jsSyntax();
                    }

                } else if (tableName == "cust_sharing") {

                    string getTxtQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                        command.Parameters.AddWithValue("@filename", label1.Text);

                        string textValuesF = "";
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                textValuesF = reader.GetString(0);
                            }
                        }

                        string decryptedTextValues = EncryptionModel.DecryptText(textValuesF);
                        richTextBox1.Text = decryptedTextValues;
                    }

                    if (FileExt_ == ".py") {
                        pythonSyntax();
                    }
                    if (FileExt_ == ".html") {
                        htmlSyntax();
                    }
                    if (FileExt_ == ".css") {
                        cssSyntax();
                    }
                    if (FileExt_ == ".js") {
                        jsSyntax();
                    }
                }
            } catch (Exception) {

                Application.OpenForms
                .OfType<Form>()
                .Where(form => String.Equals(form.Name, "SheetRetrieval"))
                .ToList()
                .ForEach(form => form.Close());

                MessageBox.Show("Cannot load this file. There's an ongoing upload.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Syntax colorizer (py,js,html,css)
        /// </summary>

        public void pythonSyntax() {
            Color _blueRGB = Color.FromArgb(49, 100, 169);
            Color _purpleRGB = Color.FromArgb(142, 94, 175);
            Color _yellowRGB = Color.FromArgb(250, 195, 4);
            Color _brownRGB = Color.FromArgb(133, 31, 0);
            Color _Gray = Color.Gray;

            ColorizePattern("def", _blueRGB);
            ColorizePattern("class", _blueRGB);

            ColorizePattern("import", _purpleRGB);
            ColorizePattern("from", _purpleRGB);
            ColorizePattern("while", _purpleRGB);
            ColorizePattern("if", _purpleRGB);
            ColorizePattern("break", _purpleRGB);
            ColorizePattern("return", _purpleRGB);
           
            ColorizePattern("input", _yellowRGB);
            ColorizePattern("print", _yellowRGB);
            ColorizePattern("\\)", Color.Yellow);
            ColorizePattern("\\(", Color.Yellow);

            ColorizePattern("'",_brownRGB);
            ColorizePattern("//", _Gray);

        }

        public void jsSyntax() {
            Color _blueRGB = Color.FromArgb(49, 100, 169);
            Color _purpleRGB = Color.FromArgb(142, 94, 175);
            Color _yellowRGB = Color.FromArgb(250, 195, 4);

            ColorizePattern("def", _blueRGB);
            ColorizePattern("class", _blueRGB);
            ColorizePattern("let",_blueRGB);
            ColorizePattern("this", _blueRGB);

            ColorizePattern("import", _purpleRGB);
            ColorizePattern("from", _purpleRGB);
            ColorizePattern("while", _purpleRGB);
            ColorizePattern("if", _purpleRGB);
            ColorizePattern("break", _purpleRGB);
            ColorizePattern("return", _purpleRGB);

            ColorizePattern("input", _yellowRGB);
            ColorizePattern("print", _yellowRGB);
            ColorizePattern("\\)", Color.Yellow);
            ColorizePattern("\\(", Color.Yellow);
        }

        public void htmlSyntax() {
            Color _blueRGB = Color.FromArgb(30, 58, 165);
            Color _cyanRGB = Color.FromArgb(96, 194, 251);

            ColorizePattern("div",_blueRGB);
            ColorizePattern("!DOCTYPE",_blueRGB);
            ColorizePattern("/head", _blueRGB);
            ColorizePattern("head", _blueRGB);
            ColorizePattern("body", _blueRGB);
            ColorizePattern("/body", _blueRGB);
            ColorizePattern("script", _blueRGB);
            ColorizePattern("/script", _blueRGB);
            ColorizePattern("header", _blueRGB);
            ColorizePattern("/header", _blueRGB);
            ColorizePattern("title", _blueRGB);
            ColorizePattern("/title", _blueRGB);
            ColorizePattern("html", _blueRGB);
            ColorizePattern("/html", _blueRGB);
            ColorizePattern("meta", _blueRGB);

            ColorizePattern("h1", _blueRGB);
            ColorizePattern("/h1", _blueRGB);
            ColorizePattern("h2", _blueRGB);
            ColorizePattern("/h2", _blueRGB);
            ColorizePattern("h3", _blueRGB);
            ColorizePattern("/h3", _blueRGB);
            ColorizePattern("h4", _blueRGB);
            ColorizePattern("/h4", _blueRGB);

            ColorizePattern("src", _cyanRGB);
            ColorizePattern("content", _cyanRGB);
            ColorizePattern("rel", _cyanRGB);
            ColorizePattern("type", _cyanRGB);
            ColorizePattern("class", _cyanRGB);
            ColorizePattern("charset", _cyanRGB);

            ColorizePattern("\\<",Color.Gray);
            ColorizePattern("\\>", Color.Gray);

        }

        public void cssSyntax() {
            Color _orangeRgb = Color.FromArgb(196, 135, 59);
            Color _cyanRGB = Color.FromArgb(96, 194, 251);

            ColorizePattern("body",_orangeRgb);
            ColorizePattern("class", _orangeRgb);
            ColorizePattern("button", _orangeRgb);
            ColorizePattern("h2", _orangeRgb);
            ColorizePattern("h1", _orangeRgb);
            ColorizePattern("h3", _orangeRgb);
            ColorizePattern("h4", _orangeRgb);

            ColorizePattern("background-color", _cyanRGB);
            ColorizePattern("background", _cyanRGB);
            ColorizePattern("box-sizing", _cyanRGB);
            ColorizePattern("max-height", _cyanRGB);
            ColorizePattern("max-width", _cyanRGB);
            ColorizePattern("text-align", _cyanRGB);
            ColorizePattern("color", _cyanRGB);
            ColorizePattern("cursor", _cyanRGB);
            ColorizePattern("border", _cyanRGB);
            ColorizePattern("content", _cyanRGB);
            ColorizePattern("position", _cyanRGB);
            ColorizePattern("width", _cyanRGB);
            ColorizePattern("top", _cyanRGB);
            ColorizePattern("height", _cyanRGB);
            ColorizePattern("right", _cyanRGB);
            ColorizePattern("font-weight",_cyanRGB);
            ColorizePattern("outline", _cyanRGB);
            ColorizePattern("z-index", _cyanRGB);
            ColorizePattern("border-radius", _cyanRGB);
            ColorizePattern("font-weight", _cyanRGB);
            ColorizePattern("padding", _cyanRGB);
            ColorizePattern("margin", _cyanRGB);
            ColorizePattern("font-family", _cyanRGB);
            ColorizePattern("font-size", _cyanRGB);
            ColorizePattern("left", _cyanRGB);
            ColorizePattern("text-decoration", _cyanRGB);
            ColorizePattern("text-transform", _cyanRGB);
        }
        private void ColorizePattern(string pattern, Color color) {
            int selectStart = this.richTextBox1.SelectionStart;
            foreach (Match match in Regex.Matches(richTextBox1.Text, pattern)) {
                richTextBox1.Select(match.Index, match.Length);
                richTextBox1.SelectionColor = color;
                richTextBox1.Select(selectStart, 0);
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
            };
        }

        private void txtFORM_Load(object sender, EventArgs e) {
            Application.OpenForms
             .OfType<Form>()
             .Where(form => String.Equals(form.Name, "SheetRetrieval"))
             .ToList()
             .ForEach(form => form.Close());
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            label1.AutoSize = true;
        }

        private void haha_TextChanged(object sender, EventArgs e) {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) {

        }
        /// <summary>
        /// Retrieve text from TextBox and save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e) {
            var FileExt_ = label1.Text.Substring(label1.Text.LastIndexOf('.')).TrimStart();
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.FileName = label1.Text;
            _OpenDialog.Filter = "Files|*" + FileExt_;
            try {
                if(_OpenDialog.ShowDialog() == DialogResult.OK) {
                    File.WriteAllText(_OpenDialog.FileName,guna2textbox1.Text);
                }
            } catch (Exception) {
                MessageBox.Show("An error occurred while attempting to save file.","Flowstorage",
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e) {
           
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            Application.OpenForms
              .OfType<Form>()
              .Where(form => String.Equals(form.Name, "bgBlurForm"))
              .ToList()
              .ForEach(form => form.Hide());
        }
    }
}
