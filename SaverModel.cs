﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FlowSERVER1 {

    /// <summary>
    /// Download file class
    /// </summary>

    public partial class SaverModel {
        private static MySqlConnection con = ConnectionModel.con;
        private static MySqlCommand command = ConnectionModel.command;
        private static String _getExt;
        public static SaverModel instance;
        public static bool stopFileRetrieval = false;

        /// <summary>
        /// 
        /// Retrieve data (byte) from references and save them
        /// by opening SaveFileDialog
        /// 
        /// </summary>
        /// <param name="_FileTitle"></param>
        /// <param name="_getBytes"></param>

        private static void _openDialog(String _FileTitle, Byte[] _getBytes) {
            Application.OpenForms
                     .OfType<Form>()
                     .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                     .ToList()
                     .ForEach(form => form.Close());
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
                List<String> _base64Encoded = new List<string>();
                if (_TableName == "upload_info_directory") {
                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.","Saver").ShowDialog());
                    ShowAlert.Start();

                    Application.DoEvents();
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND DIR_NAME = @dirname";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);
                    command.Parameters.AddWithValue("@dirname", _DirectoryName);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (stopFileRetrieval == true) {
                        _byteReader.Close();
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrieval = false;
                    }
                    if (_byteReader.Read()) {
                        _base64Encoded.Add(_byteReader.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        _openDialog(_FileTitle,_getBytes);
                    }
                    _byteReader.Close();
                } else if (_TableName == "folder_upload_info") {
                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.","Saver").ShowDialog());
                    ShowAlert.Start();
                    Application.DoEvents();
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename AND FOLDER_TITLE = @foldtitle";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);
                    command.Parameters.AddWithValue("@foldtitle", _DirectoryName);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (stopFileRetrieval == true) {
                        _byteReader.Close();
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrieval = false;
                    }
                    if (_byteReader.Read()) {
                        _base64Encoded.Add(_byteReader.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        _openDialog(_FileTitle, _getBytes);
                    }
                    _byteReader.Close();
                } else if (_TableName != "folder_upload_info" && _TableName != "upload_info_directory" && _TableName != "cust_sharing") {
                    Thread ShowAlert = new Thread(() => new RetrievalAlert("Flowstorage is retrieving your file.","Saver").ShowDialog());
                    ShowAlert.Start();
                    Application.DoEvents();
                    String _retrieveBytes = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filename";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if(stopFileRetrieval == true) {
                        _byteReader.Close();
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrieval = false;
                    }
                    if (_byteReader.Read()) {
                        _base64Encoded.Add(_byteReader.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        _openDialog( _FileTitle, _getBytes);
                    }
                    _byteReader.Close();
                } else if (_TableName == "cust_sharing") {
                    RetrievalAlert ShowAlert = new RetrievalAlert("Flowstorage is retrieving your file.","Saver");
                    ShowAlert.Show();
                    Application.DoEvents();

                    String _retrieveBytes = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filename";
                    command = con.CreateCommand();
                    command.CommandText = _retrieveBytes;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filename", _FileTitle);

                    MySqlDataReader _byteReader = command.ExecuteReader();
                    if (stopFileRetrieval == true) {
                        _byteReader.Close();
                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrieval = false;
                    }
                    if (_byteReader.Read()) {
                        _base64Encoded.Add(_byteReader.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        _openDialog(_FileTitle, _getBytes);
                    }
                    _byteReader.Close();
                }
            }
            catch (Exception eq) {
                Application.OpenForms
                         .OfType<Form>()
                         .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                         .ToList()
                         .ForEach(form => form.Close());
                //MessageBox.Show("Failed to download this file.", "Flowstorage",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
