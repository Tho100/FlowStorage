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
using System.IO;

namespace FlowSERVER1 {
    public partial class msiFORM : Form {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public msiFORM(String _fileName) {
            InitializeComponent();
            label1.Text = _fileName;
            label2.Text = "Uploaded By " + Form1.instance.label5.Text;
        }

        private void msiFORM_Load(object sender, EventArgs e) {

        }

        private void guna2Button2_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void guna2Button4_Click(object sender, EventArgs e) {
            try {
                String _selectMsiBytes = "SELECT CUST_FILE FROM file_info_msi WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                command = con.CreateCommand();
                command.CommandText = _selectMsiBytes;
                command.Parameters.AddWithValue("@username",Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filename", label1.Text);

                MySqlDataReader _readBytes = command.ExecuteReader();
                if(_readBytes.Read()) {
                    var _getMsiBytes = (byte[])_readBytes["CUST_FILE"];
                    SaveFileDialog _openDialog = new SaveFileDialog();
                    _openDialog.Filter = "MSI|*.msi";
                    if(_openDialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllBytes(_openDialog.FileName,_getMsiBytes);
                    }
                }
                _readBytes.Close();
            } catch (Exception eq) {
                MessageBox.Show("Failed to save this file.","Flowstorage");
            }
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
        }

        private void label1_Click(object sender, EventArgs e) {

        }
    }
}
