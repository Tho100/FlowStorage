using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;

namespace FlowSERVER1 {
    /*
      @ Essential class for data retrieval
     */
    public partial class LoaderModel {
        public static MySqlCommand command = ConnectionModel.command;
        public static MySqlConnection con = ConnectionModel.con;
        public static Byte[] universalBytes;
        public static Byte[] LoadFile(String _TableName, String _DirectoryName,String _FileName) {
            if (_TableName != "upload_info_directory" && _TableName != "folder_upload_info") {
                String _readGifFiles = "SELECT CUST_FILE FROM " + _TableName + " WHERE CUST_USERNAME = @username AND CUST_FILE_PATH = @filepath";
                command = con.CreateCommand();
                command.CommandText = _readGifFiles;
                command.Parameters.AddWithValue("@username", Form1.instance.label5.Text);
                command.Parameters.AddWithValue("@filepath", _FileName);

                MySqlDataReader _readByteValues = command.ExecuteReader();
                if (_readByteValues.Read()) {
                    Application.OpenForms
                           .OfType<Form>()
                           .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                           .ToList()
                           .ForEach(form => form.Close());
                    var _byteValues = (byte[])_readByteValues["CUST_FILE"];
                    universalBytes = _byteValues;
                    //MemoryStream _memStream = new MemoryStream(_byteValues);
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
                if (_readByteValues.Read()) {
                    Application.OpenForms
                           .OfType<Form>()
                           .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                           .ToList()
                           .ForEach(form => form.Close());
                    var _byteValues = (byte[])_readByteValues["CUST_FILE"];
                     universalBytes =_byteValues;
                    //MemoryStream _memStream = new MemoryStream(_byteValues);
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
                if (_readByteValues.Read()) {
                    Application.OpenForms
                           .OfType<Form>()
                           .Where(form => String.Equals(form.Name, "RetrievalAlert"))
                           .ToList()
                           .ForEach(form => form.Close());
                    var _byteValues = (byte[])_readByteValues["CUST_FILE"];
                    universalBytes = _byteValues;
                    //MemoryStream _memStream = new MemoryStream(_byteValues);
                }
                _readByteValues.Close();
            }
            return universalBytes;
        }
    }
}
