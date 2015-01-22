using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoLapseTools.Utils
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class PhotoUtils
    {
        /// <summary>
        /// Save Bitmap as Jpg
        /// </summary>
        /// <param name="bmp">Bitmap to be saved</param>
        /// <param name="fileName">Filename</param>
        /// <param name="quality">Jpg quality</param>
        public static void SaveJpg(Bitmap bmp,string fileName, long quality)
        {
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0]= new EncoderParameter(Encoder.Quality, quality);
            bmp.Save(fileName, GetEncoder(ImageFormat.Jpeg), myEncoderParameters); // Save image
        }

        // Helper function for saving in JPEG
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) if (codec.FormatID == format.Guid) return codec;
            return null;
        }
    }
}
