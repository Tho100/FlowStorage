﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Apitron.PDF.Rasterizer;
using System.IO;
using System.Dynamic;
using PdfiumViewer;

namespace FlowSERVER1 {
    public partial class pdfFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public pdfFORM(String _FileTitle, String _tableName) {
            InitializeComponent();
            label1.Text = _FileTitle;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;

            String _getPdfBytes = "SELECT CUST_FILE FROM " + _tableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(_getPdfBytes,con);
            command = con.CreateCommand();
            command.CommandText = _getPdfBytes;
            command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", label1.Text);

            MySqlDataReader _readBytes = command.ExecuteReader();
            if(_readBytes.Read()) { 
                var getPdfValues = (byte[])_readBytes["CUST_FILE"];
                setupPdf(getPdfValues);
            }
            _readBytes.Close();
        }
        // @SUMMARY Convert bytes of PDF file to stream
        public void setupPdf(byte[] pdfBytes) {
            var _getStream = new MemoryStream(pdfBytes);
            LoadPdf(_getStream);
        }
        // @SUMMARY Load stream of bytes to pdf renderer
        public void LoadPdf(Stream stream) {
            var _pdfDocumentSetup = PdfDocument.Load(stream);
            pdfRenderer1.Load(_pdfDocumentSetup);
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

        private void pdfFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            String _getPdfBytes = "SELECT CUST_FILE FROM file_info_pdf WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
            command = new MySqlCommand(_getPdfBytes, con);
            command = con.CreateCommand();
            command.CommandText = _getPdfBytes;
            command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
            command.Parameters.AddWithValue("@filename", label1.Text);

            MySqlDataReader _readBytes = command.ExecuteReader();
            if (_readBytes.Read()) {
                SaveFileDialog _openDialog = new SaveFileDialog();
                _openDialog.Filter = "Acrobat Files|*.pdf";
                if(_openDialog.ShowDialog() == DialogResult.OK) {
                    var _getBytes = (byte[])_readBytes["CUST_FILE"];
                    File.WriteAllBytes(_openDialog.FileName,_getBytes);
                }
            }
            _readBytes.Close();
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void pdfViewer1_Load(object sender, EventArgs e) {

        }
    }
}