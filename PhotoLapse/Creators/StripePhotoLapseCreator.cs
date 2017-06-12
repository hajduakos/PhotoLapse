using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PhotoLapse.Creators
{
    /// <summary>
    /// Stripe PhotoLapse creator
    /// </summary>
    public sealed class StripePhotoLapseCreator : IPhotoLapseCreator
    {
        private int padding;

        public int Padding { get { return padding; } }

        public StripePhotoLapseCreator(int padding = 0)
        {
            this.padding = Math.Max(0, padding);
        }

        /// <summary>
        /// Create photolapse
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public Bitmap Process(List<string> images, Reporters.IReporter reporter = null)
        {
            List<float> weights = images.Select(i => 1f).ToList();
            return Process(images, weights, reporter);
        }

        /// <summary>
        /// Create photolapse with weights
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="weights">List of weights for each image</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public Bitmap Process(List<string> images, List<float> weights, Reporters.IReporter reporter = null)
        {
            int widthPadded, heightPadded, x, y;
            int width, height;
            int stridePadded, stride, idx, idxPadded;
            PixelFormat pxFormat;
            int imgCount = images.Count;
            
            // Check if at least 1 image is provided
            if (imgCount == 0) throw new Exception("No images to be processed.");
            if (imgCount != weights.Count) throw new Exception("Number of images and weights do not match.");
            float weightSum = weights.Sum();
            if (weightSum < 0.00001f) throw new Exception("Sum of weights must not be zero.");

            // Get dimensions and pixel format for the first image
            using (Bitmap firstImg = new Bitmap(images[0]))
            {
                width = firstImg.Width;
                height = firstImg.Height;
                widthPadded = firstImg.Width + (imgCount + 1) * padding;
                heightPadded = firstImg.Height + 2 * padding;
                pxFormat = firstImg.PixelFormat;
            }
            if (reporter != null) reporter.Report(0, width);

            // Calculate the starting point of each stripe (float)
            float[] stripeStartF = new float[imgCount];
            stripeStartF[0] = 0;
            for (int i = 1; i < imgCount; ++i) stripeStartF[i] = stripeStartF[i - 1] + width * weights[i - 1] / weightSum;

            // Calculate the starting point of each stripe rounded
            // The width of the image is added as an extra element
            int[] stripeStart = new int[imgCount + 1];
            for (int i = 0; i < imgCount; ++i) stripeStart[i] = (int)Math.Floor(stripeStartF[i]);
            stripeStart[imgCount] = width;
                        
            // Create result
            Bitmap resultImg = new Bitmap(widthPadded, heightPadded, pxFormat);
            using (Graphics gfx = Graphics.FromImage(resultImg)) { gfx.Clear(Color.White); }
            BitmapData resultData = resultImg.LockBits(new Rectangle(0, 0, resultImg.Width, resultImg.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            stridePadded = resultData.Stride;

            unsafe
            {
                byte* resultPtr = (byte*)resultData.Scan0;

                // Loop through each image and create the stripe
                for (int img = 0; img < imgCount; ++img)
                {
                    int start = stripeStart[img];
                    int end = stripeStart[img + 1];

                    using (Bitmap actual = new Bitmap(images[img]))
                    {
                        if (actual.Width != width || actual.Height != height)
                            throw new Exception("Size mismatch at image [" + images[img] + "].");
                        BitmapData actualData = actual.LockBits(new Rectangle(0, 0, actual.Width, actual.Height),
                            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        byte* actualPtr = (byte*)actualData.Scan0;
                        stride = actualData.Stride;

                        for (x = start; x < end; ++x)
                        {
                            for (y = 0; y < height; ++y)
                            {
                                idx = y * stride + x * 3;
                                idxPadded =  (padding + y) * stridePadded + ((img + 1) * padding + x) * 3;
                                resultPtr[idxPadded] = actualPtr[idx];
                                resultPtr[idxPadded + 1] = actualPtr[idx + 1];
                                resultPtr[idxPadded + 2] = actualPtr[idx + 2];
                            }
                            if (reporter != null) reporter.Report(x, width);
                        }
                        actual.UnlockBits(actualData);
                    }
                }
            }

            resultImg.UnlockBits(resultData);

            if (reporter != null) reporter.Report(width, width);
                        
            return resultImg;
        }
    }
}
