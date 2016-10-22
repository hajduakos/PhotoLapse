using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PhotoLapseTools.Creators
{
    /// <summary>
    /// Stripe PhotoLapse creator
    /// </summary>
    public sealed class StripePhotoLapseCreator : PhotoLapseCreatorBase
    {
        /// <summary>
        /// Create photolapse with weights
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="weights">List of weights for each image</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public override Bitmap Process(List<string> images, List<float> weights, Reporters.IReporter reporter = null)
        {
            int w, h, x, y;
            PixelFormat pxFormat;
            int count = images.Count;
            int stride, idx;

            if (reporter != null) reporter.Report(0, count);

            // Check if at least 1 image is provided
            if (images.Count == 0) throw new Exception("No images to be processed.");
            if (images.Count != weights.Count) throw new Exception("Number of images and weights do not match.");
            float wSum = weights.Sum();
            if (wSum < 0.00001f) throw new Exception("Sum of weights must not be zero.");

            // Get dimensions and pixel format for the first image
            using (Bitmap first = new Bitmap(images[0]))
            {
                w = first.Width; h = first.Height;
                pxFormat = first.PixelFormat;
            }

            // Calculate the starting point of each stripe (float)
            float[] stripeStartF = new float[count];
            stripeStartF[0] = 0;
            for (int i = 1; i < count; ++i) stripeStartF[i] = stripeStartF[i - 1] + w * weights[i - 1] / wSum;

            // Calculate the starting point of each stripe rounded
            // The width of the image is added as an extra element
            int[] stripeStart = new int[count + 1];
            for (int i = 0; i < count; ++i) stripeStart[i] = (int)Math.Floor(stripeStartF[i]);
            stripeStart[count] = w;
                        
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
                    int start = stripeStart[img];
                    int end = stripeStart[img + 1];

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
                        actual.UnlockBits(actualData);
                    }
                    if (reporter != null) reporter.Report(img + 1, count);
                }
            }

            result.UnlockBits(resultData);
                        
            return result;
        }
    }
}
