using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace FlowSERVER1 {
    /// <summary>
    /// File Loader Class
    /// </summary>
    public partial class LoaderModel {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static Byte[] universalBytes;
        public static bool stopFileRetrievalLoad = false;

        public static Byte[] LoadFile(String _TableName, String _DirectoryName,String _FileName, bool _isFromSharedTo = false) {

            try {

                List<String> _base64Encoded = new List<string>();

                if (_TableName != "upload_info_directory" && _TableName != "folder_upload_info" && _TableName != "cust_sharing") {

                    String _readGifFiles =  "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _FileName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();

                    if (_readByteValues.Read()) {

                        if (stopFileRetrievalLoad == true) {

                            _readByteValues.Close();

                            Application.OpenForms
                            .OfType<Form>()
                            .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                            .ToList()
                            .ForEach(form => form.Close());
                            stopFileRetrievalLoad = false;
                        }

                        Application.OpenForms
                                .OfType<Form>()
                                .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                                .ToList()
                                .ForEach(form => form.Close());

                        _base64Encoded.Add(_readByteValues.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        universalBytes = _getBytes;
                    }
                    _readByteValues.Close();
                }

                else if (_TableName == "upload_info_directory") {
                    String _readGifFiles = "SELECT CUST_FILE FROM upload_info_directory WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath AND DIR_NAME = @dirname";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _FileName);
                    command.Parameters.AddWithValue("@dirname", _DirectoryName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();
                    if (stopFileRetrievalLoad == true) {

                        _readByteValues.Close();

                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());

                        stopFileRetrievalLoad = false;
                    }

                    if (_readByteValues.Read()) {

                        Application.OpenForms
                               .OfType<Form>()
                               .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                               .ToList()
                               .ForEach(form => form.Close());

                        _base64Encoded.Add(_readByteValues.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        universalBytes = _getBytes;
                    }
                    _readByteValues.Close();

                }
                else if (_TableName == "folder_upload_info") {

                    String _readGifFiles = "SELECT CUST_FILE FROM folder_upload_info WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath AND FOLDER_TITLE = @foldtitle";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _FileName);
                    command.Parameters.AddWithValue("@foldtitle", _DirectoryName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();

                    if (stopFileRetrievalLoad == true) {

                        _readByteValues.Close();

                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());

                        stopFileRetrievalLoad = false;
                    }
                    if (_readByteValues.Read()) {

                        Application.OpenForms
                               .OfType<Form>()
                               .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                               .ToList()
                               .ForEach(form => form.Close());

                        _base64Encoded.Add(_readByteValues.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        universalBytes = _getBytes;
                    }
                    _readByteValues.Close();

                } else if (_TableName == "cust_sharing" && _isFromSharedTo == true) {

                    String _readGifFiles = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_FROM = @username AND CUST_FILE_PATH = @filepath";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _FileName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();

                    if (stopFileRetrievalLoad == true) {

                        _readByteValues.Close();

                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrievalLoad = false;
                    }
                    if (_readByteValues.Read()) {

                        Application.OpenForms
                               .OfType<Form>()
                               .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                               .ToList()
                               .ForEach(form => form.Close());

                        _base64Encoded.Add(_readByteValues.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        universalBytes = _getBytes;
                    }
                    _readByteValues.Close();
                }

                else if (_TableName == "cust_sharing") {

                    String _readGifFiles = "SELECT CUST_FILE FROM cust_sharing WHERE CUST_TO = @username AND CUST_FILE_PATH = @filepath";
                    command = con.CreateCommand();
                    command.CommandText = _readGifFiles;
                    command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                    command.Parameters.AddWithValue("@filepath", _FileName);

                    MySqlDataReader _readByteValues = command.ExecuteReader();

                    if (stopFileRetrievalLoad == true) {

                        _readByteValues.Close();

                        Application.OpenForms
                        .OfType<Form>()
                        .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                        .ToList()
                        .ForEach(form => form.Close());
                        stopFileRetrievalLoad = false;
                    }
                    if (_readByteValues.Read()) {

                        Application.OpenForms
                               .OfType<Form>()
                               .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                               .ToList()
                               .ForEach(form => form.Close());

                        _base64Encoded.Add(_readByteValues.GetString(0));
                        var _getBytes = Convert.FromBase64String(_base64Encoded[0]);
                        universalBytes = _getBytes;
                    }
                    _readByteValues.Close();
                }
            } catch (Exception) {
                // @ ignore exception                
            }
            return universalBytes;
        }
    }
}

