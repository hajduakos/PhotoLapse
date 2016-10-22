using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PhotoLapseTools.Creators;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoLapseTests
{
    [TestClass]
    public class PhotoLapseTests
    {
        private static List<string> images = new List<string>() { "ff0000.png", "ffff00.png", "00ff00.png", "00ffff.png", "0000ff.png", "ff00ff.png" };
        private static string imagesPath = "../../images/";

        [TestMethod]
        public void TestStripes()
        {
            IPhotoLapseCreator stripe = new StripePhotoLapseCreator();
            Test(stripe, new List<float>() { 1, 1, 1, 1, 1, 1 }, "stripe111111.bmp");
            Test(stripe, new List<float>() { 1, 2, 1, 2, 1, 2 }, "stripe121212.bmp");
            Test(stripe, new List<float>() { 0, 1, 0, 1, 0, 1 }, "stripe010101.bmp");
            Test(stripe, new List<float>() { 1, 0, 1, 0, 1, 0 }, "stripe101010.bmp");
            Test(stripe, new List<float>() { 1, 0, 0, 0, 0, 0 }, "stripe100000.bmp");
            Test(stripe, new List<float>() { 0, 0, 0, 0, 0, 1 }, "stripe000001.bmp");
            Test(stripe, new List<float>() { 0, 0, 1, 0, 0, 0 }, "stripe001000.bmp");
            Test(stripe, new List<float>() { 1, 2, 3, 4, 5, 6 }, "stripe123456.bmp");
        }

        [TestMethod]
        public void TestGradient()
        {
            IPhotoLapseCreator grad = new GradientPhotoLapseCreator();
            Test(grad, new List<float>() { 1, 1, 1, 1, 1 }, "gradient11111.bmp");
        }

        public void Test(IPhotoLapseCreator creator, List<float> weights, string expected)
        {
            List<string> fullPath = images.Select(i => imagesPath + i).ToList();
            using (Bitmap resultBmp = creator.Process(fullPath, weights))
            using (Bitmap expectedBmp = new Bitmap(imagesPath + expected))
            {
               Assert.IsTrue(Compare(resultBmp, expectedBmp, 1));
            }
        }

        public void Generate(IPhotoLapseCreator creator, List<float> weights, string output)
        {
            List<string> fullPath = images.Select(i => imagesPath + i).ToList();
            using (Bitmap result = creator.Process(fullPath, weights))
            {
                result.Save(imagesPath + output);
            }
        }

        private static bool Compare(Bitmap actual, Bitmap expected, int tolerance)
        {
            if (actual.Width != expected.Width) return false;
            if (actual.Height != expected.Height) return false;
            BitmapData bmdAct = actual.LockBits(new Rectangle(0, 0, actual.Width, actual.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmdExp = expected.LockBits(new Rectangle(0, 0, actual.Width, actual.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int wMul3 = actual.Width * 3;
            int h = actual.Height;
            int x, y;
            bool result = true;

            unsafe
            {
                for (y = 0; y < h; ++y)
                {
                    // Get row
                    byte* rowAct = (byte*)bmdAct.Scan0 + (y * bmdAct.Stride);
                    byte* rowExp = (byte*)bmdExp.Scan0 + (y * bmdExp.Stride);
                    // Iterate through columns
                    for (x = 0; x < wMul3; ++x)
                    {
                        if (Math.Abs(rowAct[x] - rowExp[x]) > tolerance)
                        {
                            result = false;
                        }
                    }
                }
            }

            actual.UnlockBits(bmdAct);
            expected.UnlockBits(bmdExp);
            return result;
        }
    }
}
