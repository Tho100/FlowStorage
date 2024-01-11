using FlowstorageDesktop.AlertForms;
using FlowstorageDesktop.Global;
using FlowstorageDesktop.Helper;
using FlowstorageDesktop.Temporary;
using Microsoft.WindowsAPICodePack.Shell;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FlowstorageDesktop.Query {
    public class InsertFileDataQuery {

        readonly private MySqlConnection con = ConnectionModel.con;

        readonly private Crud crud = new Crud();

        readonly private GeneralCompressor compressor = new GeneralCompressor();
        readonly private TemporaryDataUser tempDataUser = new TemporaryDataUser();
        private string _todayDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        public async Task InsertFileDataVideo(string selectedItems, string nameTable, string fileName, string fileDataBase64Encoded) {

            try {

                var fileSizeInMb = Convert.FromBase64String(fileDataBase64Encoded).Length / 1024 / 1024;

                StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

                string encryptedFileName = EncryptionModel.Encrypt(fileName);
                string thumbnailCompressedBase64 = "";

                string insertQuery = $"INSERT INTO {nameTable} (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB) VALUES (@file_name, @username, @date, @file_data, @thumbnail_value)";
                using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@date", _todayDate);
                    command.Parameters.AddWithValue("@file_data", fileDataBase64Encoded);

                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                        var toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            string toBase64 = Convert.ToBase64String(stream.ToArray());
                            thumbnailCompressedBase64 = compressor.compressBase64Image(toBase64);
                            command.Parameters.AddWithValue("@thumbnail_value", thumbnailCompressedBase64);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

                if (HomePage.instance.lblCurrentPageText.Text == "Home") {
                    GlobalsData.base64EncodedThumbnailHome.Add(thumbnailCompressedBase64);

                } else if (HomePage.instance.lblCurrentPageText.Text == "Public Storage") {
                    GlobalsData.base64EncodedThumbnailPs.Add(thumbnailCompressedBase64);

                }

                ClosePopupForm.CloseUploadingPopup();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Upload failed", subheader: $"Failed to upload {fileName}");

            }

        }

        public async Task InsertFileData(string fileName, string fileDataBase64Encoded, string nameTable) {

            try {

                var fileSizeInMb = Convert.FromBase64String(fileDataBase64Encoded).Length / 1024 / 1024;

                StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

                string encryptedFileName = EncryptionModel.Encrypt(fileName);

                string insertQuery = $"INSERT INTO {nameTable} (CUST_USERNAME,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE) VALUES (@username, @file_name, @date, @file_data)";
                var param = new Dictionary<string, string>
                {
                    { "@username", tempDataUser.Username},
                    { "@file_name", encryptedFileName},
                    { "@date", _todayDate},
                    { "@file_data", fileDataBase64Encoded}
                };

                await crud.Insert(insertQuery, param);

                ClosePopupForm.CloseUploadingPopup();

            } catch (Exception) {
                BuildShowAlert(
                    title: "Upload failed", subheader: $"Failed to upload {fileName}");
            }

        }

        public async Task InsertFileVideoDataPublic(string selectedItems, string fileName, string fileDataBase64Encoded) {

            try {

                var fileSizeInMb = Convert.FromBase64String(fileDataBase64Encoded).Length / 1024 / 1024;

                StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

                string encryptedFileName = EncryptionModel.Encrypt(fileName);
                string encryptedComment = EncryptionModel.Encrypt(HomePage.instance.PublicStorageUserComment);

                string thumbnailCompressedBase64 = "";

                string insertQuery = $"INSERT INTO ps_info_video (CUST_FILE_PATH, CUST_USERNAME, UPLOAD_DATE, CUST_FILE, CUST_THUMB, CUST_TITLE, CUST_TAG) VALUES (@file_name, @username, @date, @file_data, @thumbnail_value, @title, @tag)";
                using (MySqlCommand command = new MySqlCommand(insertQuery, con)) {
                    command.Parameters.AddWithValue("@file_name", encryptedFileName);
                    command.Parameters.AddWithValue("@username", tempDataUser.Username);
                    command.Parameters.AddWithValue("@date", _todayDate);
                    command.Parameters.AddWithValue("@file_data", fileDataBase64Encoded);
                    command.Parameters.AddWithValue("@title", HomePage.instance.PublicStorageUserTitle);
                    command.Parameters.AddWithValue("@tag", HomePage.instance.PublicStorageUserTag);

                    using (var shellFile = ShellFile.FromFilePath(selectedItems)) {
                        var toBitMap = shellFile.Thumbnail.Bitmap;
                        using (var stream = new MemoryStream()) {
                            toBitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                            thumbnailCompressedBase64 = Convert.ToBase64String(stream.ToArray());

                            command.Parameters.AddWithValue("@thumbnail_value", thumbnailCompressedBase64);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }

                string insertQueryComment = $"INSERT INTO ps_info_comment (CUST_FILE_NAME, CUST_COMMENT) VALUES (@file_name, @comment)";
                var paramComment = new Dictionary<string, string>
                {
                    { "@file_name", encryptedFileName},
                    { "@comment", encryptedComment},
                };

                await crud.Insert(insertQueryComment, paramComment);

                GlobalsData.base64EncodedThumbnailPs.Add(thumbnailCompressedBase64);

                ClosePopupForm.CloseUploadingPopup();

                HomePage.instance.PublicStorageUserComment = null;

            } catch (Exception) {
                BuildShowAlert(
                    title: "Upload failed", subheader: $"Failed to upload {fileName}");

            }

        }

        public async Task InsertFileDataPublic(string fileName, string fileDataBase64Encoded, string nameTable) {

            try {

                var fileSizeInMb = Convert.FromBase64String(fileDataBase64Encoded).Length / 1024 / 1024;

                StartPopupForm.StartUploadingFilePopup(fileName, fileSizeInMb);

                string encryptedComment = EncryptionModel.Encrypt(HomePage.instance.PublicStorageUserComment);
                string encryptedFileName = EncryptionModel.Encrypt(fileName);

                string insertQuery = $"INSERT INTO {nameTable} (CUST_USERNAME,CUST_FILE_PATH,UPLOAD_DATE,CUST_FILE, CUST_TITLE, CUST_TAG) VALUES (@username, @file_name, @date, @file_data, @title, @tag)";
                var param = new Dictionary<string, string>
                {
                    { "@username", tempDataUser.Username},
                    { "@file_name", encryptedFileName},
                    { "@date", _todayDate},
                    { "@file_data", fileDataBase64Encoded},
                    { "@title", HomePage.instance.PublicStorageUserTitle},
                    { "@tag", HomePage.instance.PublicStorageUserTag},
                };

                await crud.Insert(insertQuery, param);

                string insertQueryComment = $"INSERT INTO ps_info_comment (CUST_FILE_NAME, CUST_COMMENT) VALUES (@file_name, @comment)";
                var paramComment = new Dictionary<string, string>
                {
                    { "@file_name", encryptedFileName},
                    { "@comment", encryptedComment},
                };

                await crud.Insert(insertQueryComment, paramComment);

                ClosePopupForm.CloseUploadingPopup();

                HomePage.instance.PublicStorageUserComment = null;

            } catch (Exception) {
                BuildShowAlert(
                    title: "Upload failed", subheader: $"Failed to upload {fileName}");

            }

        }

        public async Task InsertFileDataFolder(string filesFullPath, string folderName, string fileDataBase64Encoded, string thumbnailBase64 = null) {

            try {

                string encryptedFolderName = EncryptionModel.Encrypt(folderName);
                string encryptedFileName = EncryptionModel.Encrypt(Path.GetFileName(filesFullPath));

                string fileType = filesFullPath.Substring(filesFullPath.LastIndexOf('.') + 1);

                const string insertQuery = "INSERT INTO folder_upload_info(FOLDER_NAME,CUST_USERNAME,CUST_FILE,FILE_TYPE,UPLOAD_DATE,CUST_FILE_PATH,CUST_THUMB) VALUES (@folder_name, @username, @file_data, @file_type, @date, @file_name, @thumbnail)";

                var param = new Dictionary<string, string>
                {
                        { "@username", tempDataUser.Username},
                        { "@folder_name", encryptedFolderName},
                        { "@file_name", encryptedFileName},
                        { "@file_type", fileType},
                        { "@date", _todayDate},
                        { "@file_data", fileDataBase64Encoded},
                        { "@thumbnail", thumbnailBase64},
                };

                await crud.Insert(insertQuery, param);

            } catch (Exception) {
                BuildShowAlert(
                    title: "Upload failed", subheader: $"Failed to upload this folder.");

            }

        }

        private void BuildShowAlert(string title, string subheader) {
            new CustomAlert(
                title: title, subheader: subheader).Show();
        }

    }
}
