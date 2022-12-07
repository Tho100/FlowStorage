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

namespace FlowSERVER1 {
    public partial class exlFORM : Form {
        public static exlFORM instance;
        public exlFORM(String title) {
            InitializeComponent();
            instance = this;
            label1.Text = title;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;

            string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
            string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
            string username = "root"; // epiz_33067528 | root
            string password = "nfreal-yt10";
            int mainPort_ = 15817;
            string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            MySqlConnection con = new MySqlConnection(constring);
            MySqlCommand command;

            con.Open();

            var form1 = Form1.instance;
            String selectSheets_Name = "SELECT SHEET_NAME FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(selectSheets_Name,con);
            command.Parameters.AddWithValue("@username",form1.label5.Text);
            command.Parameters.AddWithValue("@password", form1.label3.Text);
            command.Parameters.AddWithValue("@filename", title);

            List<String> sheetName_Values = new List<String>();

            MySqlDataReader readerSheets_ = command.ExecuteReader();
            while(readerSheets_.Read()) {
                sheetName_Values.Add(readerSheets_.GetString(0));
            }
            readerSheets_.Close();

            List<String> fixedSheetNames = sheetName_Values.Distinct().ToList();
            foreach(var sheetNameValues in fixedSheetNames) {
                guna2ComboBox1.Items.Add(sheetNameValues);
            }
            guna2ComboBox1.SelectedIndex = 0;
            /*try {

                string server = "0.tcp.ap.ngrok.io"; // 185.27.134.144 | localhost
                string db = "flowserver_db"; // epiz_33067528_information | flowserver_db
                string username = "root"; // epiz_33067528 | root
                string password = "nfreal-yt10";
                int mainPort_ = 15817;
                string constring = "SERVER=" + server + ";" + "Port=" + mainPort_ + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                con.Open();
                
                String selectXml = "SELECT CUST_FILE FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE_PATH = @path";
                command = new MySqlCommand(selectXml,con);
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password",Form1.instance.label3.Text);
                command.Parameters.AddWithValue("@path",label1.Text);

                List<string> xmlValues = new List<string>();

                MySqlDataReader readXml = command.ExecuteReader();
                while(readXml.Read()) {
                    xmlValues.Add(readXml.GetString(0));    
                }
                readXml.Close();

                StringReader mainReader = new StringReader(xmlValues[0]);
                DataSet dataset = new DataSet();
                dataset.ReadXml(mainReader);

                guna2DataGridView1.DataSource = dataset.Tables[0];

                String selectSheetName = "SELECT CUST_FILE_PATH FROM file_info_excel WHERE CUST_USERNAME = @username AND CUST_PASSWORD = @password AND CUST_FILE = @fileval";
                command = new MySqlCommand(selectSheetName,con);
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@password", Form1.instance.label3.Text);
                command.Parameters.AddWithValue("@fileval",xmlValues);
                

                List<string> sheetNames = new List<string>();
                MySqlDataReader readSheets = command.ExecuteReader();
                while(readSheets.Read()) {
                    sheetNames.Add(readSheets.GetString(0));
                }
                readSheets.Close();
                foreach(var item in sheetNames) {
                    MessageBox.Show(item);
                }
                MessageBox.Show(sheetNames.Count().ToString());
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }*/






            /*
            StringReader mainReader = new StringReader(getXML);
            DataSet dataset = new DataSet();
            dataset.ReadXml(mainReader);

            guna2DataGridView1.DataSource = dataset.Tables[0];*/

            /*
            
            String pathExl = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
            OleDbConnection conExl = new OleDbConnection(pathExl);
            conExl.Open();
            DataTable Sheets = conExl.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,null);
            
            List<string> sheetsValues = new List<string>();

            for (int i = 0; i < Sheets.Rows.Count; i++) {
                string worksheets = Sheets.Rows[i]["TABLE_NAME"].ToString();
                string sqlQuery = String.Format("SELECT * FROM [{0}]", worksheets);
                sheetsValues.Add(sqlQuery);
            }

            foreach (var item in sheetsValues) {
                var output = String.Join(";", Regex.Matches(item, @"\[(.+?)\$")
                                                    .Cast<Match>()
                                                    .Select(m => m.Groups[1].Value));
                guna2ComboBox1.Items.Add(output);
            }

            var firstSheetDefault = guna2ComboBox1.Items[0];
            guna2ComboBox1.SelectedIndex = 0;
            guna2ComboBox1.SelectedItem = firstSheetDefault;
            
            OleDbDataAdapter adptCon = new OleDbDataAdapter("select * from ["+guna2ComboBox1.SelectedItem+"$]", conExl);
            DataTable mainTable = new DataTable();
            adptCon.Fill(mainTable);

            guna2DataGridView1.DataSource = mainTable;*/
            /*
            try {
                string server = "localhost";
                string db = "flowserver_db";
                string username = "root";
                string password = "nfreal-yt10";
                string constring = "SERVER=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

                MySqlConnection con = new MySqlConnection(constring);
                MySqlCommand command;

                DataTable dt = getDVGTable(guna2DataGridView1);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
               
                StringWriter sm = new StringWriter();
                ds.WriteXml(sm);
                string resultXML = sm.ToString();

                
                                         command.Parameters.Add("@CUST_FILE_TXT", MySqlDbType.LongText);
                                string varDate = DateTime.Now.ToString("dd/MM/yyyy");

                        command.Parameters["@CUST_FILE_TXT_NAME"].Value = getName;
                
                String varDate = DateTime.Now.ToString("dd/MM/yyyy");

                //con.Open();

                String insertXML = "INSERT INTO file_info_excel(CUST_FILE_PATH,CUST_USERNAME,CUST_PASSWORD,UPLOAD_DATE,CUST_FILE) VALUES (@CUST_FILE_PATH,@CUST_USERNAME,@CUST_PASSWORD,@UPLOAD_DATE,@CUST_FILE)";
                command = new MySqlCommand(insertXML,con);
                command.Parameters.Add("@CUST_FILE_PATH",MySqlDbType.Text);
                command.Parameters.Add("@CUST_USERNAME",MySqlDbType.Text);
                command.Parameters.Add("@CUST_PASSWORD", MySqlDbType.Text);
                command.Parameters.Add("@UPLOAD_DATE", MySqlDbType.VarChar,255);
                command.Parameters.Add("@CUST_FILE", MySqlDbType.LongText);

                command.Parameters["@CUST_FILE_PATH"].Value = Path;
                command.Parameters["@CUST_USERNAME"].Value = Form1.instance.label5.Text;
                command.Parameters["@CUST_PASSWORD"].Value = Form1.instance.label3.Text;
                command.Parameters["@UPLOAD_DATE"].Value = varDate;
                command.Parameters["@CUST_FILE"].Value = resultXML;
                //command.ExecuteNonQuery();

                // !SELECT * FROM file_info_excel!
                // NOT INSERT INTO file_info_excel
            }
            catch (Exception eq) {
                MessageBox.Show(eq.Message);
            }*/
        }

        public DataTable getDVGTable(DataGridView dvg) {
            var dt = new DataTable();
            foreach(DataGridViewColumn column in dvg.Columns) {
                if(column.Visible) {
                    dt.Columns.Add();
                }
            }
            object[] cellValues = new object[dvg.Columns.Count];
            foreach(DataGridViewRow row in dvg.Rows) {
                for(int i=0; i<row.Cells.Count; i++) {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }
            return dt;
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

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            /*String pathExl = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + label4.Text + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
            OleDbConnection conExl = new OleDbConnection(pathExl);
            conExl.Open();
            DataTable Sheets = conExl.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            OleDbDataAdapter adptCon = new OleDbDataAdapter("select * from [" + guna2ComboBox1.SelectedItem + "$]", conExl);
            DataTable mainTable = new DataTable();
            adptCon.Fill(mainTable);

            guna2DataGridView1.DataSource = mainTable;*/
        }

        private void label3_Click(object sender, EventArgs e) {

        }

        private void guna2Button1_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
            guna2Button1.Visible = false;
            guna2Button3.Visible = true;
        }
    }
}
