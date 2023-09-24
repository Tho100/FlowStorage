using FlowSERVER1.Global;
using FlowSERVER1.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FlowSERVER1 {

    /// <summary>
    /// Load user text file 
    /// </summary>

    public partial class TextForm : Form {

        public readonly TextForm instance;

        readonly private MySqlConnection con = ConnectionModel.con;

        private bool _isFromSharing { get; set; }
        private string _directoryName { get; set; }
        private string _tableName { get; set; }

        readonly private UpdateChanges saveChanges = new UpdateChanges();

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

        public TextForm(String getText, String tableName, String fileName, String directoryName, String uploaderName, bool isFromSharing = false) {

            InitializeComponent();

            try {

                instance = this;

                this.lblFileName.Text = fileName;
                this._tableName = tableName;
                this._directoryName = directoryName;
                this._isFromSharing = isFromSharing;

                var FileExt_ = lblFileName.Text.Substring(lblFileName.Text.LastIndexOf('.')).TrimStart();

                if (isFromSharing == true) {

                    btnEditComment.Visible = true;
                    guna2Button12.Visible = true;

                    isFromSharing = true;

                    label4.Text = "Shared To";
                    btnShareFile.Visible = false;
                    lblUserComment.Visible = true;
                    lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
                }
                else {
                    label4.Text = "Uploaded By";
                    lblUserComment.Visible = true;
                    lblUserComment.Text = GetComment.getCommentSharedToMe(fileName: fileName) != "" ? GetComment.getCommentSharedToMe(fileName: fileName) : "(No Comment)";
                }

                if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                    label4.Text = "Uploaded By";
                    string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                    lblUserComment.Visible = true;
                    lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
                }

                lblUploaderName.Text = uploaderName;

                if (tableName == GlobalsTable.directoryUploadTable && getText == "") {

                    const string getTxtQuery = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));
                        command.Parameters.AddWithValue("@dirname", EncryptionModel.Encrypt(DirectoryForm.instance.lblDirectoryName.Text));

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                byte[] toBytes = Convert.FromBase64String(EncryptionModel.Decrypt(reader.GetString(0)));

                                lblFileSize.Text = $"{GetFileSize.fileSize(toBytes):F2}Mb";

                                string toBase64Decoded = System.Text.Encoding.UTF8.GetString(toBytes);
                                richTextBox1.Text = toBase64Decoded;
                            }
                        }

                    }

                }
                else if (getText == "" && tableName == GlobalsTable.folderUploadTable) {

                    const string getTxtQuery = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND FOLDER_TITLE = @foldername AND CUST_FILE_PATH = @filename";

                    using (var command = new MySqlCommand(getTxtQuery, con)) {
                        command.Parameters.AddWithValue("@username", Globals.custUsername);
                        command.Parameters.AddWithValue("@foldername", EncryptionModel.Encrypt(HomePage.instance.lstFoldersPage.GetItemText(HomePage.instance.lstFoldersPage.SelectedItem)));
                        command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(fileName));

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                byte[] toBytes = Convert.FromBase64String(EncryptionModel.Decrypt(reader.GetString(0)));
                                lblFileSize.Text = $"{GetFileSize.fileSize(toBytes):F2}Mb";

                                string toBase64Decoded = System.Text.Encoding.UTF8.GetString(toBytes);
                                richTextBox1.Text = toBase64Decoded;
                            }
                        }

                    }

                }
                else if (tableName == GlobalsTable.homeTextTable) {
                    const string getTxtQuery = "SELECT CUST_FILE FROM file_info_expand WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    RetrieveTextData(getTxtQuery, FileExt_, Globals.custUsername);
                }
                else if (tableName == GlobalsTable.sharingTable && _isFromSharing == false) {
                    const string getTxtQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    RetrieveTextData(getTxtQuery, FileExt_, Globals.custUsername);
                }
                else if (tableName == GlobalsTable.sharingTable && _isFromSharing == true) {
                    const string getTxtQuery = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                    RetrieveTextData(getTxtQuery, FileExt_, Globals.custUsername);
                }
                else if (tableName == "ps_info_text") {
                    const string getTxtQuery = "SELECT CUST_FILE FROM ps_info_text WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    RetrieveTextData(getTxtQuery, FileExt_, uploaderName);
                }

                if (FileExt_ == ".py") {
                    PythonSyntax();
                }
                if (FileExt_ == ".html") {
                    HTMLSyntax();
                }
                if (FileExt_ == ".css") {
                    CSSSyntax();
                }
                if (FileExt_ == ".js") {
                    JSSyntax();
                }

            }
            catch (Exception) {

                Application.OpenForms
                .OfType<Form>()
                .Where(form => String.Equals(form.Name, "SheetRetrieval"))
                .ToList()
                .ForEach(form => form.Close());

                MessageBox.Show("Failed to load this file.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void RetrieveTextData(String PerformQue, String FileExtension, String uploaderName) {

            string getTxtQuery = PerformQue;
            using (var command = new MySqlCommand(getTxtQuery, con)) {
                command.Parameters.AddWithValue("@username", uploaderName);
                command.Parameters.AddWithValue("@filename", EncryptionModel.Encrypt(lblFileName.Text));

                using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        Byte[] toBytes = Convert.FromBase64String(EncryptionModel.Decrypt(reader.GetString(0)));

                        lblFileSize.Text = $"{GetFileSize.fileSize(toBytes):F2}Mb";

                        String toDecodedBase64 = Encoding.UTF8.GetString(toBytes);
                        richTextBox1.Text = toDecodedBase64;
                    }
                }
            }

            if (FileExtension == ".py") {
                PythonSyntax();
            }
            if (FileExtension == ".html") {
                HTMLSyntax();
            }
            if (FileExtension == ".css") {
                CSSSyntax();
            }
            if (FileExtension == ".js") {
                JSSyntax();
            }

        }

        /// <summary>
        /// Syntax colorizer (py,js,html,css)
        /// </summary>

        private void PythonSyntax() {
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

            ColorizePattern("'", _brownRGB);
            ColorizePattern("//", _Gray);

        }

        private void JSSyntax() {
            Color _blueRGB = Color.FromArgb(49, 100, 169);
            Color _purpleRGB = Color.FromArgb(142, 94, 175);
            Color _yellowRGB = Color.FromArgb(250, 195, 4);

            ColorizePattern("def", _blueRGB);
            ColorizePattern("class", _blueRGB);
            ColorizePattern("let", _blueRGB);
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

        private void HTMLSyntax() {
            Color _blueRGB = Color.FromArgb(30, 58, 165);
            Color _cyanRGB = Color.FromArgb(96, 194, 251);

            ColorizePattern("div", _blueRGB);
            ColorizePattern("!DOCTYPE", _blueRGB);
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

            ColorizePattern("\\<", Color.Gray);
            ColorizePattern("\\>", Color.Gray);

        }
        private void CSSSyntax() {
            Color _orangeRgb = Color.FromArgb(196, 135, 59);
            Color _cyanRGB = Color.FromArgb(96, 194, 251);

            ColorizePattern("body", _orangeRgb);
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
            ColorizePattern("font-weight", _cyanRGB);
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
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
            lblFileName.AutoSize = true;
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
            var FileExt_ = lblFileName.Text.Substring(lblFileName.Text.LastIndexOf('.')).TrimStart();
            SaveFileDialog _OpenDialog = new SaveFileDialog();
            _OpenDialog.FileName = lblFileName.Text;
            _OpenDialog.Filter = "Files|*" + FileExt_;
            try {
                if (_OpenDialog.ShowDialog() == DialogResult.OK) {
                    File.WriteAllText(_OpenDialog.FileName, guna2textbox1.Text);
                }
            }
            catch (Exception) {
                MessageBox.Show("An error occurred while attempting to save file.", "Flowstorage",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            if (this._tableName != "ps_info_text") {
                btnSaveChanges.Visible = true;
            }
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

        private void guna2Button5_Click(object sender, EventArgs e) {
            string getExtension = lblFileName.Text.Substring(lblFileName.Text.Length - 4);
            new shareFileFORM(lblFileName.Text, getExtension,
                _isFromSharing, _tableName, _directoryName).Show();
        }

        private async void btnSaveChanges_Click(object sender, EventArgs e) {

            if (CallDialogResultSave.CallDialogResult(_isFromSharing) == DialogResult.Yes) {

                string getStrings = richTextBox1.Text;
                byte[] getBytesText = Encoding.UTF8.GetBytes(getStrings);
                string base64Strings = Convert.ToBase64String(getBytesText);

                string fileName = lblFileName.Text;

                await saveChanges.SaveChangesUpdate(fileName, base64Strings, _tableName, _isFromSharing, _directoryName);
                MessageBox.Show("Changes saved successfully.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void guna2Button11_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button12.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private async void guna2Button12_Click(object sender, EventArgs e) {
            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button12.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }
    }
}
