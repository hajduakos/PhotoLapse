using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PhotoLapse.Creators
{
    /// <summary>
    /// Gradient PhotoLapse creator
    /// </summary>
    public sealed class GradientPhotoLapseCreator : IPhotoLapseCreator
    {
        /// <summary>
        /// Create photolapse
        /// </summary>
        /// <param name="images">List of images</param>
        /// <param name="reporter">Reporter</param>
        /// <returns>Photolapse</returns>
        public Bitmap Process(List<string> images, Reporters.IReporter reporter = null)
        {
            List<float> weights = images.Select(i => 1f).Skip(1).ToList();
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
            int w, h, x, y;
            PixelFormat pxFormat;
            int count = images.Count;
            int stride, idx;

            // Check if at least 2 images are provided
            if (images.Count == 0) throw new Exception("No images to be processed.");
            if (images.Count < 2) throw new Exception("At least 2 images are required for this type of photolapse.");
            if (images.Count != weights.Count + 1) throw new Exception("Number of weights must be one less than the number of images.");
            float wSum = weights.Sum();
            if (wSum < 0.00001f) throw new Exception("Sum of weights must not be zero.");

            // Get dimensions and pixelformat for the first image
            using (Bitmap first = new Bitmap(images[0]))
            {
                w = first.Width; h = first.Height;
                pxFormat = first.PixelFormat;
            }
            if (reporter != null) reporter.Report(0, w);

            // There are count of images - 1 vertical stripes

            // Calculate the starting point of each stripe (float)
            float[] stripeStartF = new float[count - 1];
            stripeStartF[0] = 0;
            for (int i = 1; i < count - 1; ++i) stripeStartF[i] = stripeStartF[i - 1] + w * weights[i - 1] / wSum;

            // Calculate the starting point of each stripe rounded
            // The width of the image is added as an extra element
            int[] stripeStart = new int[count];
            for (int i = 0; i < count - 1; ++i) stripeStart[i] = (int)Math.Floor(stripeStartF[i]);
            stripeStart[count - 1] = w;
            
            // Create result
            Bitmap result = new Bitmap(w, h, pxFormat);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            stride = resultData.Stride;

            unsafe
            {
                byte* resultPtr = (byte*)resultData.Scan0;

                // Loop through the images and create a fade between the actual and next image
                for (int img = 0; img < count - 1; ++img)
                {
                    int start = stripeStart[img];
                    int end = stripeStart[img + 1];

                    using (Bitmap actual = new Bitmap(images[img]))
                    using (Bitmap next = new Bitmap(images[img + 1]))
                    {
                        if (actual.Width != w || actual.Height != h)
                            throw new Exception("Size mismatch at image [" + images[img] + "].");
                        if (next.Width != w || next.Height != h)
                            throw new Exception("Size mismatch at image [" + images[img + 1] + "].");

                        BitmapData actualData = actual.LockBits(new Rectangle(0, 0, w, h),
                            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        byte* actualPtr = (byte*)actualData.Scan0;
                        BitmapData nextData = next.LockBits(new Rectangle(0, 0, w, h),
                            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                        byte* nextPtr = (byte*)nextData.Scan0;

                        for (x = start; x < end; ++x)
                        {
                            float r1 = (x - start) / (float)(end - start - 1);
                            float r0 = 1 - r1;
                            for (y = 0; y < h; ++y)
                            {
                                idx = y * stride + x * 3;
                                resultPtr[idx] = (byte)(r0 * actualPtr[idx] + r1 * nextPtr[idx]);
                                resultPtr[idx + 1] = (byte)(r0 * actualPtr[idx + 1] + r1 * nextPtr[idx + 1]);
                                resultPtr[idx + 2] = (byte)(r0 * actualPtr[idx + 2] + r1 * nextPtr[idx + 2]);
                            }
                            if (reporter != null) reporter.Report(x, w);
                        }

                        actual.UnlockBits(actualData);
                        next.UnlockBits(nextData);

                    }
                }
            }

            if (reporter != null) reporter.Report(w, w);

            result.UnlockBits(resultData);
            
            return result;
        }
    }
}
