using ClosedXML.Excel;
using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FlowstorageDesktop {

    public partial class ExcelForm : Form {

        readonly public ExcelForm instance;

        private string _directoryName { get; set; }
        private string _tableName { get; set; }

        private int _previousSelectedIndex { get; set; } = 0;
        private int _currentSheetIndex { get; set; } = 1;
        private int _changedIndex { get; set; } = 0;
        private byte[] _sheetsByte { get; set; }
        private bool _isFromSharing { get; set; }

        readonly private UpdateChanges saveChanges = new UpdateChanges();

        /// <summary>
        /// 
        /// Load user excel workbook sheet based on table name 
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tableName"></param>
        /// <param name="directoryName"></param>
        /// <param name="uploaderName"></param>

        public ExcelForm(string fileName, string tableName, string directoryName, string uploaderName, bool isFromShared = false) {

            InitializeComponent();

            gridExcelSpreadsheet.CellValueChanged += dataGridView1_CellValueChanged;

            instance = this;

            this.lblFileName.Text = fileName;
            this._directoryName = directoryName;
            this._tableName = tableName;

            if (_isFromSharing) {
                btnEditComment.Visible = true;
                btnSaveComment.Visible = true;

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
                string comment = GetComment.getCommentPublicStorage(fileName: fileName);
                lblUserComment.Visible = true;
                lblUserComment.Text = string.IsNullOrEmpty(comment) ? "(No Comment)" : comment;
            }

            lblUploaderName.Text = uploaderName;

            GenerateSpreadsheet(LoaderModel.LoadFile(tableName, _directoryName, fileName, isFromShared));
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
        private void GenerateSpreadsheet(byte[] _getByte) {

            lblFileSize.Text = $"{GetFileSize.fileSize(_getByte):F2}Mb";

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

                                } else {
                                    dt.Rows.Add(row.Cells().Select(c => c.Value.ToString()).ToArray());

                                }
                            }

                            gridExcelSpreadsheet.DataSource = dt;

                        }
                    }
                }

            } catch (Exception) {
                new CustomAlert(
                    title: "Failed to load this workbook", subheader: "It may be broken or unsupported format.").Show();
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

        private void guna2Button2_Click(object sender, EventArgs e) => this.Close();

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
            SaverModel.SaveSelectedFile(lblFileName.Text, _tableName, _directoryName);
        }

        private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e) {

        }

        private void guna2Button8_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.TopMost = false;
        }

        private void guna2Button5_Click(object sender, EventArgs e) {
            new shareFileFORM(
                lblFileName.Text, _isFromSharing, _tableName, _directoryName).Show();
        }

        private byte[] UpdatedChangesBytes() {

            string selectedSheetName = guna2ComboBox1.SelectedItem.ToString();

            using (var memoryStream = new MemoryStream(_sheetsByte)) {
                using (var workbook = new XLWorkbook(memoryStream)) {
                    var worksheet = workbook.Worksheet(selectedSheetName);
                    DataTable dt = (DataTable)gridExcelSpreadsheet.DataSource;

                    var existingTable = worksheet.Tables.FirstOrDefault();
                    if (existingTable != null)
                        worksheet.Cell(1, 1).Worksheet.Tables.Remove(existingTable.Name);

                    worksheet.Clear();
                    worksheet.Cell(1, 1).InsertTable(dt);

                    using (var outputStream = new MemoryStream()) {
                        workbook.SaveAs(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
        }

        private async void btnSaveChanges_Click(object sender, EventArgs e) {

            try {

                if(CallDialogResultSave.CallDialogResult(_isFromSharing) == DialogResult.Yes) {

                    byte[] updatedBytes = UpdatedChangesBytes();

                    if (updatedBytes != null) {

                        string toBase64String = Convert.ToBase64String(updatedBytes);
                        string fileName = lblFileName.Text;
                        await saveChanges.SaveChangesUpdate(fileName, toBase64String, _tableName, _isFromSharing, _directoryName);

                        new CustomAlert(
                            title: "Changes saved",
                            subheader: "Changes has been saved successfully.").Show();

                    } else {
                        new CustomAlert(
                            title: "Something went wrong",
                            subheader: "Failed to save changes.").Show();
                    }
                }

            } catch (Exception) {
                new CustomAlert(title: "Something went wrong", "Failed to save changes.").Show();

            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (this._tableName != "ps_info_excel") {
                btnSaveChanges.Visible = true;
            }
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private async void btnSaveComment_Click(object sender, EventArgs e) {

            if (lblUserComment.Text != txtFieldComment.Text) {
                await new UpdateComment().SaveChangesComment(txtFieldComment.Text, lblFileName.Text);
            }

            lblUserComment.Text = txtFieldComment.Text != string.Empty ? txtFieldComment.Text : lblUserComment.Text;
            btnEditComment.Visible = true;
            btnSaveComment.Visible = false;
            txtFieldComment.Visible = false;
            lblUserComment.Visible = true;
            lblUserComment.Refresh();
        }

        private void btnEditComment_Click(object sender, EventArgs e) {
            txtFieldComment.Enabled = true;
            txtFieldComment.Visible = true;
            btnEditComment.Visible = false;
            btnSaveComment.Visible = true;
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

            int selectedIndex = guna2ComboBox1.SelectedIndex;

            if (selectedIndex != _previousSelectedIndex) {

                _previousSelectedIndex = selectedIndex;
                _changedIndex = selectedIndex;

                _currentSheetIndex = selectedIndex + 1;
                GenerateSpreadsheet(_sheetsByte);
            }
        }
    }
}