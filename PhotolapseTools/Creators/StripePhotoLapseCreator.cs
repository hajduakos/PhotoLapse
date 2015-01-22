using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoLapseTools.Creators
{
    /// <summary>
    /// Stripe PhotoLapse creator
    /// </summary>
    public class StripePhotoLapseCreator : IPhotoLapseCreator
    {
        /// <summary>
        /// Create photolapse
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public Bitmap Process(List<string> images, Reporters.IReporter reporter = null)
        {
            int w, h, x, y;
            PixelFormat pxFormat;
            int count = images.Count;
            int stride, idx;

            if (reporter != null) reporter.Report(0, count);

            // Check if at least 1 image is provided
            if (images.Count == 0) throw new Exception("No images to be processed.");

            // Get dimensions and pixel format for the first image
            using (Bitmap first = new Bitmap(images[0]))
            {
                w = first.Width; h = first.Height;
                pxFormat = first.PixelFormat;
            }

            // There are as many vertical stripes as the count of the images
            int stripeWidth = w / count;

            // Create result
            Bitmap result = new Bitmap(w, h, pxFormat);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            stride = resultData.Stride;

            unsafe
            {
                byte* resultPtr = (byte*)resultData.Scan0;

                // Loop through each image and create the stripe
                for (int img = 0; img < count; ++img)
                {
                    int start = img * stripeWidth;
                    int end = (img == count - 1) ? w : start + stripeWidth;

                    using (Bitmap actual = new Bitmap(images[img]))
                    {
                        if (actual.Width != w || actual.Height != h)
                            throw new Exception("Size mismatch at image [" + images[img] + "].");
                        BitmapData actualData = actual.LockBits(new Rectangle(0, 0, actual.Width, actual.Height),
                            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        byte* actualPtr = (byte*)actualData.Scan0;

                        for (x = start; x < end; ++x)
                        {
                            for (y = 0; y < h; ++y)
                            {
                                idx = y * stride + x * 3;
                                resultPtr[idx] = actualPtr[idx];
                                resultPtr[idx + 1] = actualPtr[idx + 1];
                                resultPtr[idx + 2] = actualPtr[idx + 2];
                            }
                        }
                    }
                    if (reporter != null) reporter.Report(img + 1, count);
                }
            }
                        
            return result;
        }
    }
}
