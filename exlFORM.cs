using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.IO;
using System.Xml;
using ExcelDataReader;
using ClosedXML.Excel;

namespace FlowSERVER1 {
    public partial class exlFORM : Form {

        public static exlFORM instance;
        private static String DirectoryName;
        private static String TableName;

        private static List<String> _sheetValues = new List<String>();
        private static int _currentSheetIndex = 1;
        private static int _changedIndex = 0;

        public exlFORM(String titleName, String _TableName, String _DirectoryName, String _UploaderName) {
            InitializeComponent();

            instance = this;
            label2.Text = "Uploaded By " + _UploaderName;
            label1.Text = titleName;
            DirectoryName = _DirectoryName;
            TableName = _TableName;

            try {
                if(_TableName == "file_info_excel") {
                    generateSheet(LoaderModel.LoadFile("file_info_excel",DirectoryName,titleName));
                } else if (_TableName == "upload_info_directory") {
                    generateSheet(LoaderModel.LoadFile("upload_info_directory", DirectoryName, titleName));
                } else if (_TableName == "folder_upload_info") {
                    generateSheet(LoaderModel.LoadFile("folder_upload_info", DirectoryName, titleName));
                } else if (_TableName == "cust_sharing") {
                    generateSheet(LoaderModel.LoadFile("cust_sharing", DirectoryName, titleName));
                }
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message,"Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                //MessageBox.Show("Failed to load this workbook. It may be broken or unsupported format.","Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        // GET INDEX OF SELECTED SHEET
        private void generateSheet(Byte[] _getByte) {
            MemoryStream _toStream = new MemoryStream(_getByte);
            using (XLWorkbook workBook = new XLWorkbook(_toStream)) {

                foreach (var _getSheetsName in workBook.Worksheets) {
                    _sheetValues.Add(_getSheetsName.Name);
                    guna2ComboBox1.Items.Add(_getSheetsName);
                }
                //for(int i=0; i<_sheetValues.Count; i++) {
                //guna2ComboBox1.Items.Add(_sheetValues[i]);
                // }
                //Read the first Sheet from Excel file.
                //guna2ComboBox1.SelectedIndex = _currentSheetIndex - 1;
                guna2ComboBox1.SelectedIndex = _changedIndex;
                guna2ComboBox1.SelectedIndexChanged += guna2ComboBox1_SelectedIndexChanged;
                _currentSheetIndex = _changedIndex+1;
                IXLWorksheet workSheet = workBook.Worksheet(_currentSheetIndex);

                //Create a new DataTable.
                DataTable dt = new DataTable();

                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows()) {
                    //Use the first row to add columns to DataTable.
                    if (firstRow) {
                        foreach (IXLCell cell in row.Cells()) {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else {
                        //Add rows to DataTable.
                        dt.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells()) {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }

                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //_currentSheetIndex = guna2ComboBox1.SelectedIndex;
            _changedIndex = guna2ComboBox1.SelectedIndex;
        }

        private void Form5_Load(object sender, EventArgs e) {

        }

        private void guna2Button3_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Normal;
            guna2Button1.Visible = true;
            guna2Button3.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }

        private void spreadsheet1_Load(object sender, EventArgs e) {

        }

        private void spreadsheet1_Click(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            this.TopMost = false;
            if(TableName == "file_info_excel") {
                SaverModel.SaveSelectedFile(label1.Text,"file_info_excel",DirectoryName);
            } else if (TableName == "upload_info_directory") {
                SaverModel.SaveSelectedFile(label1.Text, "upload_info_directory", DirectoryName);
            } else if (TableName == "folder_upload_info") {
                SaverModel.SaveSelectedFile(label1.Text, "folder_upload_info", DirectoryName);
            }
            else if (TableName == "cust_sharing") {
                SaverModel.SaveSelectedFile(label1.Text, "cust_sharing", DirectoryName);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private void spreadsheet1_Load_1(object sender, EventArgs e) {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) {

        }
    }
}
