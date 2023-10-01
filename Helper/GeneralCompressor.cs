using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

namespace FlowSERVER1.Helper {
    public class GeneralCompressor {

        public byte[] compressFileData(byte[] data) {
            using (MemoryStream compressedStream = new MemoryStream()) {
                using (ZipArchive archive = new ZipArchive(compressedStream, ZipArchiveMode.Create, true)) {
                    ZipArchiveEntry entry = archive.CreateEntry("data");
                    using (Stream entryStream = entry.Open()) {
                        entryStream.Write(data, 0, data.Length);
                    }
                }

                return compressedStream.ToArray();
            }
        }

        public byte[] decompressFileData(byte[] compressedData) {
            using (MemoryStream compressedStream = new MemoryStream(compressedData)) {
                using (ZipArchive archive = new ZipArchive(compressedStream)) {
                    if (archive.Entries.Count > 0) {
                        ZipArchiveEntry entry = archive.Entries[0]; 
                        using (Stream entryStream = entry.Open()) {
                            using (MemoryStream decompressedStream = new MemoryStream()) {
                                entryStream.CopyTo(decompressedStream);
                                return decompressedStream.ToArray();
                            }
                        }
                    }
                }
            }
            return null;
        }

        public string compresImageToBase64(string sourceImagePath) {
            using (Image sourceImage = Image.FromFile(sourceImagePath)) {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 88L);

                ImageCodecInfo jpegEncoder = GetEncoderInfo(ImageFormat.Jpeg);

                using (MemoryStream memoryStream = new MemoryStream()) {
                    sourceImage.Save(memoryStream, jpegEncoder, encoderParameters);

                    byte[] imageBytes = memoryStream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);

                    return base64String;
                }
            }
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs) {
                if (codec.FormatID == format.Guid) {
                    return codec;
                }
            }
            return null;
        }

    }
}
