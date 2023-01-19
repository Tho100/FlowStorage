using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {
    public partial class SaverModel {
        public static MySqlConnection con = ConnectionModel.con;
        public static MySqlCommand command = ConnectionModel.command;
        public static String _getExt;
        private static void _openDialog(String _FileTitle, Byte[] _getBytes) {
            SaveFileDialog _dialog = new SaveFileDialog();
            _dialog.Filter = "|*." + _getExt;
            _dialog.FileName = _FileTitle;
            if (_dialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(_dialog.FileName, _getBytes);
            }
        }

        public static void SaveSelectedFile(String _FileTitle, String _TableName, String _DirectoryName) {
            _getExt = _FileTitle.Split('.').Last();

            try {
                RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your document.");
                ShowAlert.Show();
                Application.DoEvents();
                if(_TableName == "upload_info_directory") {
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);
                    command.Parameters.AddWithValue("@dirname", _DirectoryName);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (_byteReader.Read()) {
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        var _getBytes = (byte[])_byteReader["CUST_FILE"];
                        _openDialog(_FileTitle,_getBytes);
                    }
                    _byteReader.Close();
                } else if (_TableName == "folder_upload_info") {
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);
                    command.Parameters.AddWithValue("@foldtitle", _DirectoryName);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (_byteReader.Read()) {
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        var _getBytes = (byte[])_byteReader["CUST_FILE"];
                        _openDialog(_FileTitle, _getBytes);
                    }
                    _byteReader.Close();
                } else if (_TableName != "folder_upload_info" && _TableName != "upload_info_directory") {
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (_byteReader.Read()) {
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        var _getBytes = (byte[])_byteReader["CUST_FILE"];
                        _openDialog( _FileTitle, _getBytes);
                    }
                    _byteReader.Close();
                }
            }
            catch (Exception eq) {
                MessageBox.Show("Failed to download this file.", "Flowstorage");
            }
        }
    }
}
