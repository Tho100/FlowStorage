using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace FlowSERVER1.Helper {
    public class ImageCompressor {

        public string compresImageToBase64(string sourceImagePath) {

            using (Image sourceImage = Image.FromFile(sourceImagePath)) {

                ImageFormat imageFormat = GetImageFormat(sourceImagePath);

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 62L); 

                ImageCodecInfo imageCodecInfo = GetEncoderInfo(imageFormat);

                using (MemoryStream memoryStream = new MemoryStream()) {

                    memoryStream.SetLength(0);

                    sourceImage.Save(memoryStream, imageCodecInfo, encoderParameters);

                    byte[] imageBytes = memoryStream.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);

                    return base64String;
                }
            }
        }

        private ImageFormat GetImageFormat(string imagePath) {
            string extension = System.IO.Path.GetExtension(imagePath);
            switch (extension.ToLower()) {
                case ".png":
                    return ImageFormat.Png;
                case ".jpeg":
                case ".jpg":
                    return ImageFormat.Jpeg;
                default:
                    throw new NotSupportedException("Unsupported image format.");
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
