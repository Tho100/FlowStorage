using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FlowSERVER1.Helper {
    public class ImageCompressor {

        public string compresImageToBase64(string sourceImagePath) {
            using (Image sourceImage = Image.FromFile(sourceImagePath)) {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 88L);

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
