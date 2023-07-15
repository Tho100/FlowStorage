using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using ClosedXML.Excel;
using System.Threading.Tasks;

using FlowSERVER1.Helper;
using FlowSERVER1.Global;
using FlowSERVER1.AlertForms;

namespace FlowSERVER1 {

    public partial class ExcelForm : Form {

        readonly public ExcelForm instance;

        private string _directoryName { get; set; }
        private string _tableName { get; set; }

        private int _previousSelectedIndex { get; set; } = 0;
        private int _currentSheetIndex { get; set; } = 1;
        private int _changedIndex { get; set; } = 0;
        private byte[] _sheetsByte { get; set; }
        private bool _isFromSharing { get; set; }

        readonly private MySqlConnection con = ConnectionModel.con;

        /// <summary>
        /// 
        /// Load user excel workbook sheet based on table name 
        /// 
        /// </summary>
        /// <param name="titleName"></param>
        /// <param name="_TableName"></param>
        /// <param name="_DirectoryName"></param>
        /// <param name="_UploaderName"></param>

        public ExcelForm(String fileName, String tableName, String directoryName, String uploaderName, bool isFromShared = false) {

            InitializeComponent();

            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

            instance = this;

            this.lblFileName.Text = fileName;
            this._directoryName = directoryName;
            this._tableName = tableName;

            if (_isFromSharing == true) {

                btnEditComment.Visible = true;
                guna2Button9.Visible = true;

                label4.Text = "Shared To";
                btnShareFile.Visible = false;
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";

            } else {

                label4.Text = "Uploaded By";
                lblUserComment.Visible = true;
                lblUserComment.Text = GetComment.getCommentSharedToOthers(fileName: fileName) != "" ? GetComment.getCommentSharedToOthers(fileName: fileName) : "(No Comment)";
            }

            if (GlobalsTable.publicTablesPs.Contains(tableName)) {
                label4.Text = "Uploaded By";
                string comment = GetComment.getCommentPublicStorage(tableName: tableName, fileName: fileName, uploaderName: uploaderName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
            }

            lblUploaderName.Text = uploaderName;

            generateSheet(LoaderModel.LoadFile(tableName, _directoryName, fileName, isFromShared));
            _sheetsByte = LoaderModel.LoadFile(tableName, _directoryName, fileName);
           
        }


        /// <summary>
        /// Essential variable to prevent sheetname duplication
        /// </summary>

        int onlyOnceVarible = 0;

        /// <summary>
        /// Start generating workbook sheets
        /// </summary>
        /// <param name=""></param>
        private void generateSheet(byte[] _getByte) {

            lblFileSize.Text = $"{FileSize.fileSize(_getByte):F2}Mb";

            try {

                onlyOnceVarible++;

                using (MemoryStream _toStream = new MemoryStream(_getByte)) {
                    using (XLWorkbook workBook = new XLWorkbook(_toStream)) {

                        var worksheetNames = workBook.Worksheets;

                        if (onlyOnceVarible == 1) {
                            guna2ComboBox1.Items.AddRange(worksheetNames.ToArray());
                        }

                        if (_changedIndex >= 0 && _changedIndex < worksheetNames.Count) {
                            guna2ComboBox1.SelectedIndex = _changedIndex;
                            _currentSheetIndex = _changedIndex + 1;

                            IXLWorksheet workSheet = workBook.Worksheet(_currentSheetIndex);
                            DataTable dt = new DataTable();

                            bool firstRow = true;
                            foreach (IXLRangeRow row in workSheet.RangeUsed().Rows()) {
                                if (firstRow) {
                                    foreach (IXLCell cell in row.Cells()) {
                                        dt.Columns.Add(cell.Value.ToString());
                                    }
                                    firstRow = false;
                                }
                                else {
                                    dt.Rows.Add(row.Cells().Select(c => c.Value.ToString()).ToArray());
                                }
                            }
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception) {
                new CustomAlert(title: "Failed to load this workbook", subheader: "It may be broken or unsupported format.").Show();
            }
        }

        private void Form5_Load(object sender, EventArgs e) {

        }
        
        private void guna2Button3_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 12;
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.guna2BorderlessForm1.BorderRadius = 0;
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            SaverModel.SaveSelectedFile(lblFileName.Text,_tableName,_directoryName);
        }

        private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e) {

            string[] parts = lblFileName.Text.Split('.');
            string getExtension = "." + parts[1];

            new shareFileFORM(lblFileName.Text, getExtension, 
                _isFromSharing, _tableName, _directoryName).Show();
        }

        private async Task saveChangesUpdate(string textValues) {

            try {

                if (_tableName == GlobalsTable.homeExcelTable) {
                    const string updateQue = "UPDATE file_info_excel SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                        command.Parameters.Add("@update", MySqlDbType.LongText).Value = textValues;
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(lblFileName.Text);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                else if (_tableName == GlobalsTable.sharingTable && _isFromSharing == false) {

                    const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                        command.Parameters.Add("@update", MySqlDbType.LongText).Value = textValues;
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(lblFileName.Text);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                else if (_tableName == GlobalsTable.sharingTable && _isFromSharing == true) {

                    const string updateQue = "UPDATE cust_sharing SET CUST_FILE = @update WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                        command.Parameters.Add("@update", MySqlDbType.LongText).Value = textValues;
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(lblFileName.Text);
                        await command.ExecuteNonQueryAsync();
                    }

                }
                else if (_tableName == GlobalsTable.directoryUploadTable) {

                    const string updateQue = "UPDATE upload_info_directory SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                        command.Parameters.Add("@update", MySqlDbType.LongBlob).Value = textValues;
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        command.Parameters.Add("@dirname", MySqlDbType.Text).Value = EncryptionModel.Encrypt(_directoryName);
                        command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(lblFileName.Text);
                        command.ExecuteNonQuery();
                    }
                }
                else if (_tableName == "ps_info_excel") {

                    const string updateQue = "UPDATE ps_info_excel SET CUST_FILE = @update WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    using (MySqlCommand command = new MySqlCommand(updateQue, con)) {
                        command.Parameters.Add("@update", MySqlDbType.LongBlob).Value = textValues;
                        command.Parameters.Add("@username", MySqlDbType.Text).Value = Globals.custUsername;
                        command.Parameters.Add("@filename", MySqlDbType.LongText).Value = EncryptionModel.Encrypt(lblFileName.Text);
                        await command.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception) {
                MessageBox.Show("Failed to save changes.", "Flowstorage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private byte[] convertDataGridViewToByteArray(DataGridView dataGridView) {
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (XLWorkbook workbook = new XLWorkbook()) {
                    foreach (IXLWorksheet rowSheets in guna2ComboBox1.Items) {
                        IXLWorksheet sheets = (IXLWorksheet)rowSheets;
                        string sheetsName = sheets.Name.ToString();

                        DataTable dt = new DataTable();
                        foreach (DataGridViewColumn column in dataGridView.Columns) {
                            dt.Columns.Add(column.HeaderText);
                        }

                        foreach (DataGridViewRow row in dataGridView.Rows) {
                            if (row.IsNewRow)
                                continue;

                            DataRow dataRow = dt.NewRow();
                            for (int i = 0; i < dataGridView.Columns.Count; i++) {
                                dataRow[i] = row.Cells[i].Value;
                            }
                            dt.Rows.Add(dataRow);
                        }

                        workbook.Worksheets.Add(dt, sheetsName);
                    }

                    workbook.SaveAs(memoryStream);
                }

                return memoryStream.ToArray();
            }
        }

        /*private byte[] convertDataGridViewToByteArray(DataGridView dataGridView) {

            DataTable dt = dataGridView.DataSource as DataTable;
            if (dt != null) {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (XLWorkbook workbook = new XLWorkbook()) {
                        foreach (IXLWorksheet rowSheets in guna2ComboBox1.Items) {
                            IXLWorksheet sheets = (IXLWorksheet)rowSheets;
                            string sheetsName = sheets.Name.ToString();
                            workbook.Worksheets.Add(dt, sheetsName);
                        }

                        workbook.SaveAs(memoryStream);
                    }

                    return memoryStream.ToArray();
                }
            }

            return null;
        }*/

        private async void guna2Button6_Click(object sender, EventArgs e) {

            byte[] updatedBytes = convertDataGridViewToByteArray(dataGridView1);

            if (updatedBytes != null) {
                string toBase64String = Convert.ToBase64String(updatedBytes);
                string encryptedBase64 = EncryptionModel.Encrypt(toBase64String);
                await saveChangesUpdate(encryptedBase64);
                MessageBox.Show("Changes saved successfully.");
            }
            else {
                MessageBox.Show("Failed to save changes.");
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if(this._tableName != "ps_info_excel") {
                btnSaveChanges.Visible = true;
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private async void guna2Button9_Click(object sender, EventArgs e) {

            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().saveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != String.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            guna2Button9.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void guna2Button7_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            guna2Button9.Visible = true;
            lblUserComment.Visible = false;
            txtFieldComment.Text = lblUserComment.Text;
        }

        private void guna2Separator1_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// Change workbook sheet on combobox selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void guna2ComboBox1_SelectedIndexChanged_1(object sender, EventArgs e) {

            try {

                int selectedIndex = guna2ComboBox1.SelectedIndex;

                if (selectedIndex != _previousSelectedIndex) {

                    _previousSelectedIndex = selectedIndex;
                    _changedIndex = selectedIndex;

                    _currentSheetIndex = selectedIndex + 1;
                    generateSheet(_sheetsByte);
                }
            }
            catch (Exception ex) {
            }

        }
    }
}